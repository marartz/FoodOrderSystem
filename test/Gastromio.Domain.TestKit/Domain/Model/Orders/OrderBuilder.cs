using System;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Orders
{
    public class OrderBuilder : TestObjectBuilderBase<Order>
    {
        public OrderBuilder WithId(OrderId id)
        {
            WithConstantConstructorArgumentFor("id", id);
            return this;
        }

        public OrderBuilder WithCustomerInfo(CustomerInfo customerInfo)
        {
            WithConstantConstructorArgumentFor("customerInfo", customerInfo);
            return this;
        }

        public OrderBuilder WithCartInfo(CartInfo cartInfo)
        {
            WithConstantConstructorArgumentFor("cartInfo", cartInfo);
            return this;
        }

        public OrderBuilder WithComments(string comments)
        {
            WithConstantConstructorArgumentFor("comments", comments);
            return this;
        }

        public OrderBuilder WithPaymentMethodId(PaymentMethodId paymentMethodId)
        {
            WithConstantConstructorArgumentFor("paymentMethodId", paymentMethodId);
            return this;
        }

        public OrderBuilder WithPaymentMethodName(string paymentMethodName)
        {
            WithConstantConstructorArgumentFor("paymentMethodName", paymentMethodName);
            return this;
        }

        public OrderBuilder WithPaymentMethodDescription(string paymentMethodDescription)
        {
            WithConstantConstructorArgumentFor("paymentMethodDescription", paymentMethodDescription);
            return this;
        }

        public OrderBuilder WithCosts(decimal costs)
        {
            WithConstantConstructorArgumentFor("costs", costs);
            return this;
        }

        public OrderBuilder WithTotalPrice(decimal totalPrice)
        {
            WithConstantConstructorArgumentFor("totalPrice", totalPrice);
            return this;
        }

        public OrderBuilder WithServiceTime(DateTimeOffset? serviceTime)
        {
            WithConstantConstructorArgumentFor("serviceTime", serviceTime);
            return this;
        }

        public OrderBuilder WithCustomerNotificationInfo(NotificationInfo customerNotificationInfo)
        {
            WithConstantConstructorArgumentFor("customerNotificationInfo", customerNotificationInfo);
            return this;
        }

        public OrderBuilder WithRestaurantEmailNotificationInfo(NotificationInfo restaurantEmailNotificationInfo)
        {
            WithConstantConstructorArgumentFor("restaurantEmailNotificationInfo", restaurantEmailNotificationInfo);
            return this;
        }

        public OrderBuilder WithRestaurantMobileNotificationInfo(NotificationInfo restaurantMobileNotificationInfo)
        {
            WithConstantConstructorArgumentFor("restaurantMobileNotificationInfo", restaurantMobileNotificationInfo);
            return this;
        }

        public OrderBuilder WithCreatedOn(DateTimeOffset createdOn)
        {
            WithConstantConstructorArgumentFor("createdOn", createdOn);
            return this;
        }

        public OrderBuilder WithUpdatedOn(DateTimeOffset updatedOn)
        {
            WithConstantConstructorArgumentFor("updatedOn", updatedOn);
            return this;
        }

        public OrderBuilder WithUpdatedBy(UserId updatedBy)
        {
            WithConstantConstructorArgumentFor("updatedBy", updatedBy);
            return this;
        }
    }
}
