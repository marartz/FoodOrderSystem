namespace Gastromio.Core.Domain.Model.Restaurants
{
    public class ReservationInfo
    {
        public ReservationInfo(bool enabled, string reservationSystemUrl)
        {
            Enabled = enabled;
            ReservationSystemUrl = reservationSystemUrl;
        }

        public bool Enabled { get; }

        public string ReservationSystemUrl { get; }
    }
}
