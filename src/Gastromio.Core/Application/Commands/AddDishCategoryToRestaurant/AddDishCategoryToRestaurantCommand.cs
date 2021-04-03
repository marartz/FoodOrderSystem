using System;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.AddDishCategoryToRestaurant
{
    public class AddDishCategoryToRestaurantCommand : ICommand<Guid>
    {
        public AddDishCategoryToRestaurantCommand(RestaurantId restaurantId, string name, DishCategoryId afterCategoryId)
        {
            RestaurantId = restaurantId;
            Name = name;
            AfterCategoryId = afterCategoryId;
        }

        public RestaurantId RestaurantId { get; }
        public string Name { get; }
        public DishCategoryId AfterCategoryId { get; }
    }
}
