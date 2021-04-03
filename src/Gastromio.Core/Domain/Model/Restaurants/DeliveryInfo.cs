namespace Gastromio.Core.Domain.Model.Restaurants
{
    public class DeliveryInfo
    {
        public DeliveryInfo(bool enabled, int? averageTime, decimal? minimumOrderValue, decimal? maximumOrderValue,
            decimal? costs)
        {
            Enabled = enabled;
            AverageTime = averageTime;
            MinimumOrderValue = minimumOrderValue;
            MaximumOrderValue = maximumOrderValue;
            Costs = costs;
        }

        public bool Enabled { get; }

        public int? AverageTime { get; }

        public decimal? MinimumOrderValue { get; }

        public decimal? MaximumOrderValue { get; }

        public decimal? Costs { get; }
    }
}
