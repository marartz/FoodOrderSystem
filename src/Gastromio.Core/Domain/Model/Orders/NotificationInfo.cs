using System;

namespace Gastromio.Core.Domain.Model.Orders
{
    public class NotificationInfo
    {
        public NotificationInfo(
            bool status,
            int attempt,
            string message,
            DateTimeOffset timestamp
        )
        {
            Status = status;
            Attempt = attempt;
            Message = message;
            Timestamp = timestamp;
        }

        public bool Status { get; }

        public int Attempt { get; }

        public string Message { get; }

        public DateTimeOffset Timestamp { get; }
    }
}
