using System;

namespace Gastromio.Persistence.MongoDB
{
    public class OrderModel
    {
        public Guid Id { get; set; }

        public CustomerInfoModel CustomerInfo { get; set; }

        public CartInfoModel CartInfo { get; set; }

        public string Comments { get; set; }

        public Guid PaymentMethodId { get; set; }

        public string PaymentMethodName { get; set; }

        public string PaymentMethodDescription { get; set; }

        public double Costs { get; set; }

        public double TotalPrice { get; set; }

        public DateTime? ServiceTime { get; set; }

        public NotificationInfoModel CustomerNotificationInfo { get; set; }

        public NotificationInfoModel RestaurantNotificationInfo { get; set; }

        public NotificationInfoModel RestaurantMobileNotificationInfo { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }
    }
}
