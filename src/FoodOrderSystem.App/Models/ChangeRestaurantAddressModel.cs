﻿using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.App.Models
{
    public class ChangeRestaurantAddressModel
    {
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
    }
}
