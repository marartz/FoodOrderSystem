using System;

namespace Gastromio.Persistence.MongoDB
{
    public class NotificationInfoModel
    {
        public bool Status { get; set; }

        public int Attempt { get; set; }

        public string Message { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
