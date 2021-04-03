using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.DeactivateRestaurant
{
    public class DeactivateRestaurantCommand : ICommand<bool>
    {
        public DeactivateRestaurantCommand(RestaurantId restaurantId)
        {
            RestaurantId = restaurantId;
        }

        public RestaurantId RestaurantId { get; }
    }
}
