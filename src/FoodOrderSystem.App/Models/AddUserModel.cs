﻿using System.ComponentModel.DataAnnotations;

namespace FoodOrderSystem.App.Models
{
    public class AddUserModel
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
