using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.ChangeDeviatingOpeningDayStatusOfRestaurant
{
    public class ChangeDeviatingOpeningDayStatusOfRestaurantCommand : ICommand<bool>
    {
        public ChangeDeviatingOpeningDayStatusOfRestaurantCommand(RestaurantId restaurantId, Date date,
            DeviatingOpeningDayStatus status)
        {
            RestaurantId = restaurantId;
            Date = date;
            Status = status;
        }

        public RestaurantId RestaurantId { get; }
        public Date Date { get; }
        public DeviatingOpeningDayStatus Status { get; }
    }
}
