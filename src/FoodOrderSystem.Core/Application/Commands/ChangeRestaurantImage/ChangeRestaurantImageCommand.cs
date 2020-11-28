﻿using FoodOrderSystem.Core.Domain.Model.Restaurant;

namespace FoodOrderSystem.Core.Application.Commands.ChangeRestaurantImage
{
    public class ChangeRestaurantImageCommand : ICommand<bool>
    {
        public ChangeRestaurantImageCommand(RestaurantId restaurantId, string type, byte[] image)
        {
            RestaurantId = restaurantId;
            Type = type;
            Image = image;
        }

        public RestaurantId RestaurantId { get; }
        public string Type { get; }
        public byte[] Image { get; }
    }
}