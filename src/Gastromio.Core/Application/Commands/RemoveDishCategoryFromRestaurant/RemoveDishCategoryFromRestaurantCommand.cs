using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.RemoveDishCategoryFromRestaurant
{
    public class RemoveDishCategoryFromRestaurantCommand : ICommand<bool>
    {
        public RemoveDishCategoryFromRestaurantCommand(RestaurantId restaurantId, DishCategoryId dishCategoryId)
        {
            RestaurantId = restaurantId;
            DishCategoryId = dishCategoryId;
        }

        public RestaurantId RestaurantId { get; }
        public DishCategoryId DishCategoryId { get; }
    }
}
