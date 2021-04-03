using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.RemoveRestaurant
{
    public class RemoveRestaurantCommand : ICommand<bool>
    {
        public RemoveRestaurantCommand(RestaurantId restaurantId)
        {
            RestaurantId = restaurantId;
        }

        public RestaurantId RestaurantId { get; }
    }
}
