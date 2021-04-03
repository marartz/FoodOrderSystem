using System;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.DTOs
{
    public static class DeviatingOpeningDayStatusConversionExtensions
    {
        public static string ToModel(this DeviatingOpeningDayStatus deviatingOpeningDayStatus)
        {
            switch (deviatingOpeningDayStatus)
            {
                case DeviatingOpeningDayStatus.Open:
                    return "open";
                case DeviatingOpeningDayStatus.Closed:
                    return "closed";
                case DeviatingOpeningDayStatus.FullyBooked:
                    return "fully-booked";
                default:
                    throw new ArgumentOutOfRangeException(nameof(deviatingOpeningDayStatus), deviatingOpeningDayStatus, null);
            }
        }

        public static DeviatingOpeningDayStatus ToDeviatingOpeningDayStatus(this string deviatingOpeningDayStatus)
        {
            switch (deviatingOpeningDayStatus)
            {
                case "open":
                    return DeviatingOpeningDayStatus.Open;
                case "closed":
                    return DeviatingOpeningDayStatus.Closed;
                case "fully-booked":
                    return DeviatingOpeningDayStatus.FullyBooked;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deviatingOpeningDayStatus), deviatingOpeningDayStatus, null);
            }
        }
    }
}
