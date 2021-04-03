using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Notification;
using Microsoft.Extensions.Logging;
using PhoneNumbers;

namespace Gastromio.Notification.Sms77
{
    public class Sms77MobileNotificationService : IMobileNotificationService
    {
        private readonly ILogger<Sms77MobileNotificationService> logger;
        private readonly Sms77MobileConfiguration configuration;

        private const string ApiUrl = @"https://gateway.sms77.io/api/sms";

        public Sms77MobileNotificationService(ILogger<Sms77MobileNotificationService> logger,
            Sms77MobileConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task<MobileNotificationResponse> SendMobileNotificationAsync(
            MobileNotificationRequest mobileNotificationRequest,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(configuration.ApiToken))
            {
                logger.LogWarning("Skipped sending mobile notification due to missing SMS configuration");
                return new MobileNotificationResponse(true, "skipped due to missing SMS configuration");
            }

            try
            {
                logger.LogInformation("Sending SMS from {0} to {1} with text: {2}",
                    mobileNotificationRequest.From,
                    mobileNotificationRequest.To,
                    mobileNotificationRequest.Text
                );

                if (!GetUnifiedPhoneNumber(mobileNotificationRequest.To, out var unifiedPhoneNumber))
                {
                    logger.LogWarning($"Number '{mobileNotificationRequest.To}' is not valid => skipping sending notification via SMS");
                    return new MobileNotificationResponse(true, "Mobile number is not valid");
                }

                var requestContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("basic", configuration.ApiToken);

                var urlBuilder = new StringBuilder();
                urlBuilder.Append(ApiUrl);

                urlBuilder.Append("?from=");
                urlBuilder.Append(Uri.EscapeDataString(mobileNotificationRequest.From));

                urlBuilder.Append("&to=");
                urlBuilder.Append(Uri.EscapeDataString(mobileNotificationRequest.To));

                urlBuilder.Append("&text=");
                urlBuilder.Append(Uri.EscapeDataString(mobileNotificationRequest.Text));

                urlBuilder.Append("&debug=0");

                urlBuilder.Append("&details=1");

                var url = urlBuilder.ToString();

                var response = await httpClient.PostAsync(url, requestContent, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning($"Could not send SMS to {unifiedPhoneNumber} due to: {response.ReasonPhrase}");
                    return new MobileNotificationResponse(false, response.ReasonPhrase);
                }

                var responseText = await response.Content.ReadAsStringAsync();

                logger.LogInformation($"Successfully sent: {responseText}");
                return new MobileNotificationResponse(true, null);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error during sending mobile notification via Mailjet");
                return new MobileNotificationResponse(false, e.Message);
            }
        }

        internal static bool GetUnifiedPhoneNumber(string phoneNumberText, out string unifiedPhoneNumber)
        {
            try
            {
                var phoneNumberUtil = PhoneNumberUtil.GetInstance();
                var phoneNumber = phoneNumberUtil.Parse(phoneNumberText, "DE");
                if (!phoneNumberUtil.IsValidNumberForRegion(phoneNumber, "DE"))
                {
                    unifiedPhoneNumber = null;
                    return false;
                }

                unifiedPhoneNumber = phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.E164);
                return true;
            }
            catch (Exception)
            {
                unifiedPhoneNumber = null;
                return false;
            }
        }
    }
}
