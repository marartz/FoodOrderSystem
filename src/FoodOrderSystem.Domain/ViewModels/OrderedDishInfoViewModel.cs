﻿using System;

namespace FoodOrderSystem.Domain.ViewModels
{
    public class OrderedDishInfoViewModel
    {
        public Guid ItemId { get; set; }
        public Guid DishId { get; set; }
        public string DishName { get; set; }
        public Guid VariantId { get; set; }
        public string VariantName { get; set; }
        public double VariantPrice { get; set; }
        public int Count { get; set; }
        public string Remarks { get; set; }
    }
}