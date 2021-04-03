using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.AddCuisineToRestaurant
{
    public class AddCuisineToRestaurantCommand : ICommand<bool>
    {
        public AddCuisineToRestaurantCommand(RestaurantId restaurantId, CuisineId cuisineId)
        {
            RestaurantId = restaurantId;
            CuisineId = cuisineId;
        }

        public RestaurantId RestaurantId { get; }
        public CuisineId CuisineId { get; }
    }
}
