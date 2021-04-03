using System;
using System.Linq;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Model.Orders
{
    public class Order
    {
        public Order(
            OrderId id,
            CustomerInfo customerInfo,
            CartInfo cartInfo,
            string comments,
            PaymentMethodId paymentMethodId,
            string paymentMethodName,
            string paymentMethodDescription,
            decimal costs,
            decimal totalPrice,
            DateTimeOffset? serviceTime,
            NotificationInfo customerNotificationInfo,
            NotificationInfo restaurantEmailNotificationInfo,
            NotificationInfo restaurantMobileNotificationInfo,
            DateTimeOffset createdOn,
            DateTimeOffset? updatedOn,
            UserId updatedBy
        )
        {
            Id = id;
            CustomerInfo = customerInfo;
            CartInfo = cartInfo;
            Comments = comments;
            PaymentMethodId = paymentMethodId;
            PaymentMethodName = paymentMethodName;
            PaymentMethodDescription = paymentMethodDescription;
            Costs = costs;
            TotalPrice = totalPrice;
            ServiceTime = serviceTime;
            CustomerNotificationInfo = customerNotificationInfo;
            RestaurantEmailNotificationInfo = restaurantEmailNotificationInfo;
            RestaurantMobileNotificationInfo = restaurantMobileNotificationInfo;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            UpdatedBy = updatedBy;
        }

        public OrderId Id { get; }

        public CustomerInfo CustomerInfo { get; }

        public CartInfo CartInfo { get; }

        public string Comments { get; }

        public PaymentMethodId PaymentMethodId { get; }

        public string PaymentMethodName { get; }

        public string PaymentMethodDescription { get; }

        public decimal Costs { get; }

        public decimal TotalPrice { get; }

        public DateTimeOffset? ServiceTime { get; }

        public NotificationInfo CustomerNotificationInfo { get; private set; }

        public NotificationInfo RestaurantEmailNotificationInfo { get; private set; }

        public NotificationInfo RestaurantMobileNotificationInfo { get; private set; }

        public DateTimeOffset CreatedOn { get; }

        public DateTimeOffset? UpdatedOn { get; private set; }

        public UserId UpdatedBy { get; private set; }

        public decimal GetValueOfOrder()
        {
            return CartInfo?.OrderedDishes?.Sum(orderedDish => orderedDish.Count * orderedDish.VariantPrice) ?? 0;
        }

        public decimal GetTotalPrice()
        {
            return GetValueOfOrder() + Costs;
        }

        public Result<bool> RegisterCustomerNotificationAttempt(bool status, string message)
        {
            CustomerNotificationInfo = CustomerNotificationInfo == null
                ? new NotificationInfo(status, 1, message, DateTimeOffset.UtcNow)
                : new NotificationInfo(status, CustomerNotificationInfo.Attempt + 1, message, DateTimeOffset.UtcNow);
            UpdatedOn = DateTimeOffset.UtcNow;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> RegisterRestaurantEmailNotificationAttempt(bool status, string message)
        {
            RestaurantEmailNotificationInfo = RestaurantEmailNotificationInfo == null
                ? new NotificationInfo(status, 1, message, DateTimeOffset.UtcNow)
                : new NotificationInfo(status, RestaurantEmailNotificationInfo.Attempt + 1, message, DateTimeOffset.UtcNow);
            UpdatedOn = DateTimeOffset.UtcNow;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> RegisterRestaurantMobileNotificationAttempt(bool status, string message)
        {
            RestaurantMobileNotificationInfo = RestaurantMobileNotificationInfo == null
                ? new NotificationInfo(status, 1, message, DateTimeOffset.UtcNow)
                : new NotificationInfo(status, RestaurantMobileNotificationInfo.Attempt + 1, message, DateTimeOffset.UtcNow);
            UpdatedOn = DateTimeOffset.UtcNow;

            return SuccessResult<bool>.Create(true);
        }
    }
}
