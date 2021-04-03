using System;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Orders
{
    public class NotificationInfoBuilder : TestObjectBuilderBase<NotificationInfo>
    {
        public NotificationInfoBuilder WithStatus(bool status)
        {
            WithConstantConstructorArgumentFor("status", status);
            return this;
        }

        public NotificationInfoBuilder WithAttempt(int attempt)
        {
            WithConstantConstructorArgumentFor("attempt", attempt);
            return this;
        }

        public NotificationInfoBuilder WithMessage(string message)
        {
            WithConstantConstructorArgumentFor("message", message);
            return this;
        }

        public NotificationInfoBuilder WithTimestamp(DateTimeOffset timestamp)
        {
            WithConstantConstructorArgumentFor("timestamp", timestamp);
            return this;
        }
    }
}
