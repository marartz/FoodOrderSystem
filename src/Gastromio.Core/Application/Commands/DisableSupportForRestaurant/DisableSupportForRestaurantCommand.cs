using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.DisableSupportForRestaurant
{
    public class DisableSupportForRestaurantCommand : ICommand<bool>
    {
        public DisableSupportForRestaurantCommand(RestaurantId restaurantId)
        {
            RestaurantId = restaurantId;
        }

        public RestaurantId RestaurantId { get; }
    }
}
