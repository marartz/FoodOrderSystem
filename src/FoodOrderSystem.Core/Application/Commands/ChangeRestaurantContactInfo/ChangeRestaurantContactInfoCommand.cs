﻿using FoodOrderSystem.Core.Application.DTOs;
using FoodOrderSystem.Core.Domain.Model.Restaurant;

namespace FoodOrderSystem.Core.Application.Commands.ChangeRestaurantContactInfo
{
    public class ChangeRestaurantContactInfoCommand : ICommand<bool>
    {
        public ChangeRestaurantContactInfoCommand(RestaurantId restaurantId, string phone, string fax, string webSite,
            string responsiblePerson, string emailAddress)
        {
            RestaurantId = restaurantId;
            Phone = phone;
            Fax = fax;
            WebSite = webSite;
            ResponsiblePerson = responsiblePerson;
            EmailAddress = emailAddress;
        }

        public RestaurantId RestaurantId { get; }

        public string Phone { get; }
        
        public string Fax { get; }
        
        public string WebSite { get; }
        
        public string ResponsiblePerson { get; }
        
        public string EmailAddress { get; }
    }
}
