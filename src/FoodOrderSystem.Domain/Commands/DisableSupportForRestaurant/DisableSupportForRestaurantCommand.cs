﻿using FoodOrderSystem.Domain.Model.Restaurant;

namespace FoodOrderSystem.Domain.Commands.DisableSupportForRestaurant
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