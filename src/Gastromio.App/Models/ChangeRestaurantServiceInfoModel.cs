namespace Gastromio.App.Models
{
    public class ChangeRestaurantServiceInfoModel
    {
        public bool PickupEnabled { get; set; }
        public int? PickupAverageTime { get; set; }
        public decimal? PickupMinimumOrderValue { get; set; }
        public decimal? PickupMaximumOrderValue { get; set; }
        public bool DeliveryEnabled { get; set; }
        public int? DeliveryAverageTime { get; set; }
        public decimal? DeliveryMinimumOrderValue { get; set; }
        public decimal? DeliveryMaximumOrderValue { get; set; }
        public decimal? DeliveryCosts { get; set; }
        public bool ReservationEnabled { get; set; }
        public string ReservationSystemUrl { get; set; }
        public string HygienicHandling { get; set; }
    }
}
