﻿using Gastromio.Core.Domain.Model.DishCategory;
using Gastromio.Core.Domain.Model.Restaurant;

namespace Gastromio.Core.Application.Commands.ChangeDishCategoryOfRestaurant
{
    public class ChangeDishCategoryOfRestaurantCommand : ICommand<bool>
    {
        public ChangeDishCategoryOfRestaurantCommand(RestaurantId restaurantId, DishCategoryId dishCategoryId, string name)
        {
            RestaurantId = restaurantId;
            DishCategoryId = dishCategoryId;
            Name = name;
        }

        public RestaurantId RestaurantId { get; }
        public DishCategoryId DishCategoryId { get; }
        public string Name { get; }
    }
}
