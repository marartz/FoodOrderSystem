using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports;
using Gastromio.Core.Application.Ports.Notification;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Application.Ports.Template;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Core.Domain.Model.Users;
using Microsoft.Extensions.Logging;

namespace Gastromio.Core.Application.Commands.ProcessPendingNotifications
{
    public class ProcessPendingNotificationsCommandHandler : ICommandHandler<ProcessPendingNotificationsCommand, bool>
    {
        private readonly ILogger<ProcessPendingNotificationsCommandHandler> logger;
        private readonly IOrderRepository orderRepository;
        private readonly ITemplateService templateService;
        private readonly IEmailNotificationService emailNotificationService;
        private readonly IMobileNotificationService mobileNotificationService;
        private readonly IConfigurationProvider configurationProvider;

        public ProcessPendingNotificationsCommandHandler(
            ILogger<ProcessPendingNotificationsCommandHandler> logger,
            IOrderRepository orderRepository,
            ITemplateService templateService,
            IEmailNotificationService emailNotificationService,
            IMobileNotificationService mobileNotificationService,
            IConfigurationProvider configurationProvider
        )
        {
            this.logger = logger;
            this.orderRepository = orderRepository;
            this.templateService = templateService;
            this.emailNotificationService = emailNotificationService;
            this.mobileNotificationService = mobileNotificationService;
            this.configurationProvider = configurationProvider;
        }

        public async Task<Result<bool>> HandleAsync(ProcessPendingNotificationsCommand command, User currentUser,
            CancellationToken cancellationToken = default)
        {
            var pendingCustomerNotificationOrders =
                await orderRepository.FindByPendingCustomerNotificationAsync(cancellationToken);

            foreach (var order in pendingCustomerNotificationOrders)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await TriggerCustomerNotificationAsync(order, cancellationToken);
            }

            var pendingRestaurantEmailNotificationOrders =
                await orderRepository.FindByPendingRestaurantEmailNotificationAsync(cancellationToken);

            foreach (var order in pendingRestaurantEmailNotificationOrders)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                await TriggerRestaurantEmailNotificationAsync(order, cancellationToken);
            }

            var pendingRestaurantMobileNotificationOrders =
                await orderRepository.FindByPendingRestaurantMobileNotificationAsync(cancellationToken);

            foreach (var order in pendingRestaurantMobileNotificationOrders)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                await TriggerRestaurantMobileNotificationAsync(order, cancellationToken);
            }

            return SuccessResult<bool>.Create(true);
        }

        private async Task TriggerCustomerNotificationAsync(Order order,
            CancellationToken cancellationToken)
        {
            if (order.CustomerNotificationInfo != null && order.CustomerNotificationInfo.Attempt > 0)
            {
                var delay = CalculateDelay(order.CustomerNotificationInfo);
                var delta = DateTimeOffset.UtcNow - order.CustomerNotificationInfo.Timestamp;
                if (delta < delay)
                {
                    logger.LogDebug("Delay sending customer email of order {0} by further {1} seconds", order.Id.Value,
                        (delay - delta).TotalSeconds);
                    return;
                }
            }

            var emailData = templateService.GetCustomerEmail(order);
            if (emailData == null)
            {
                logger.LogError("Could not retrieve email data for customer email");
                throw new InvalidOperationException("could not retrieve email data for customer email");
            }

            EmailAddress recipient;
            if (configurationProvider.IsTestSystem)
            {
                recipient = new EmailAddress("Gastromio-Bestellungen", configurationProvider.EmailRecipientForTest);
            }
            else
            {
                recipient = new EmailAddress($"{order.CustomerInfo.GivenName} {order.CustomerInfo.LastName}",
                    order.CustomerInfo.Email);
            }

            var notificationRequest = new EmailNotificationRequest(
                new EmailAddress("Gastromio.de", "noreply@gastromio.de"),
                new List<EmailAddress> {recipient},
                new List<EmailAddress>(),
                new List<EmailAddress>(),
                emailData.Subject,
                emailData.TextPart,
                emailData.HtmlPart
            );

            var (success, info) = await TriggerEmailNotificationAsync(notificationRequest, cancellationToken);
            order.RegisterCustomerNotificationAttempt(success, info);
            await orderRepository.StoreAsync(order, cancellationToken);
        }

        private async Task TriggerRestaurantEmailNotificationAsync(Order order,
            CancellationToken cancellationToken)
        {
            if (order.RestaurantEmailNotificationInfo != null && order.RestaurantEmailNotificationInfo.Attempt > 0)
            {
                var delay = CalculateDelay(order.RestaurantEmailNotificationInfo);
                var delta = DateTimeOffset.UtcNow - order.RestaurantEmailNotificationInfo.Timestamp;
                if (delta < delay)
                {
                    logger.LogDebug("Delay sending restaurant email of order {0} by further {1} seconds",
                        order.Id.Value, (delay - delta).TotalSeconds);
                    return;
                }
            }

            var emailData = templateService.GetRestaurantEmail(order);
            if (emailData == null)
            {
                logger.LogError("Could not retrieve email data for restaurant email");
                throw new InvalidOperationException("could not retrieve email data for restaurant email");
            }

            var recipientsTo = new List<EmailAddress>();
            var recipientsCc = new List<EmailAddress>();

            if (configurationProvider.IsTestSystem)
            {
                recipientsTo.Add(
                    new EmailAddress("Gastromio-Bestellungen", configurationProvider.EmailRecipientForTest));
            }
            else
            {
                recipientsTo.Add(new EmailAddress($"{order.CartInfo.RestaurantName}", order.CartInfo.RestaurantEmail));

                if (order.CartInfo.RestaurantNeedsSupport)
                {
                    recipientsCc.Add(new EmailAddress("Gastromio-Bestellungen", "bestellungen@gastromio.de"));
                    recipientsCc.Add(
                        new EmailAddress("Hotline - Coronahilfe Bocholt", "hotline@coronahilfe-bocholt.de"));
                }
            }

            var notificationRequest = new EmailNotificationRequest(
                new EmailAddress("Gastromio.de", "noreply@gastromio.de"),
                recipientsTo,
                new List<EmailAddress>(),
                new List<EmailAddress>(),
                emailData.Subject,
                emailData.TextPart,
                emailData.HtmlPart
            );

            var (success, info) = await TriggerEmailNotificationAsync(notificationRequest, cancellationToken);

            if (success)
            {
                foreach (var recipientCc in recipientsCc)
                {
                    notificationRequest = new EmailNotificationRequest(
                        new EmailAddress("Gastromio.de", "noreply@gastromio.de"),
                        new List<EmailAddress> {recipientCc},
                        new List<EmailAddress>(),
                        new List<EmailAddress>(),
                        emailData.Subject,
                        emailData.TextPart,
                        emailData.HtmlPart
                    );

                    await TriggerEmailNotificationAsync(notificationRequest,
                        cancellationToken); // don't break if sending notification failed
                }
            }

            order.RegisterRestaurantEmailNotificationAttempt(success, info);
            await orderRepository.StoreAsync(order, cancellationToken);
        }

        private async Task<(bool success, string info)> TriggerEmailNotificationAsync(
            EmailNotificationRequest notificationRequest, CancellationToken cancellationToken)
        {
            try
            {
                var notificationResponse =
                    await emailNotificationService.SendEmailNotificationAsync(notificationRequest,
                        cancellationToken);
                return notificationResponse.Success
                    ? (true, notificationResponse.Message)
                    : (false, notificationResponse.Message);
            }
            catch (Exception e)
            {
                var from = notificationRequest?.Sender?.Email ?? "";

                var to = notificationRequest?.RecipientsTo != null
                    ? string.Join(", ", notificationRequest.RecipientsTo)
                    : "";

                var cc = notificationRequest?.RecipientsCc != null
                    ? string.Join(", ", notificationRequest.RecipientsCc)
                    : "";

                var bcc = notificationRequest?.RecipientsBcc != null
                    ? string.Join(", ", notificationRequest.RecipientsBcc)
                    : "";

                logger.LogWarning(e,
                    $"exception while sending email notification from {from} to {to} (Cc: {cc}, Bcc: {bcc})");
                return (false, "Exception: " + e);
            }
        }

        private async Task TriggerRestaurantMobileNotificationAsync(Order order,
            CancellationToken cancellationToken)
        {
            if (order.CartInfo == null || string.IsNullOrWhiteSpace(order.CartInfo.RestaurantMobile) ||
                !Validators.IsValidPhoneNumber(order.CartInfo.RestaurantMobile))
            {
                order.RegisterRestaurantMobileNotificationAttempt(true,
                    "empty or invalid mobile number => skipping notification");
                await orderRepository.StoreAsync(order, cancellationToken);
                return;
            }

            if (order.RestaurantMobileNotificationInfo != null && order.RestaurantMobileNotificationInfo.Attempt > 0)
            {
                var delay = CalculateDelay(order.RestaurantMobileNotificationInfo);
                var delta = DateTimeOffset.UtcNow - order.RestaurantMobileNotificationInfo.Timestamp;
                if (delta < delay)
                {
                    logger.LogDebug("Delay sending restaurant mobile of order {0} by further {1} seconds",
                        order.Id.Value, (delay - delta).TotalSeconds);
                    return;
                }
            }

            var mobileMessage = templateService.GetRestaurantMobileMessage(order);
            if (mobileMessage == null)
            {
                logger.LogError("Could not retrieve message for mobile");
                throw new InvalidOperationException("could not retrieve message for mobile");
            }

            var to = configurationProvider.IsTestSystem
                ? configurationProvider.MobileRecipientForTest
                : order.CartInfo.RestaurantMobile;

            var notificationRequest = new MobileNotificationRequest(
                "Gastromio",
                to,
                mobileMessage
            );

            var (success, info) =
                await TriggerMobileNotificationAsync(notificationRequest, cancellationToken);
            order.RegisterRestaurantMobileNotificationAttempt(success, info);
            await orderRepository.StoreAsync(order, cancellationToken);
        }

        private async Task<(bool success, string info)> TriggerMobileNotificationAsync(
            MobileNotificationRequest notificationRequest, CancellationToken cancellationToken)
        {
            try
            {
                var notificationResponse =
                    await mobileNotificationService.SendMobileNotificationAsync(notificationRequest,
                        cancellationToken);
                return notificationResponse.Success
                    ? (true, notificationResponse.Message)
                    : (false, notificationResponse.Message);
            }
            catch (Exception e)
            {
                var from = notificationRequest?.From ?? "";
                var to = notificationRequest?.To ?? "";
                logger.LogWarning(e,
                    $"exception while sending mobile notification from {from} to {to}");
                return (false, "Exception: " + e);
            }
        }

        private TimeSpan CalculateDelay(NotificationInfo notificationInfo)
        {
            var seconds = (int) Math.Pow(1.5, notificationInfo.Attempt);
            if (seconds > 600) // 10 minutes
                seconds = 600;
            return TimeSpan.FromSeconds(seconds);
        }
    }
}
