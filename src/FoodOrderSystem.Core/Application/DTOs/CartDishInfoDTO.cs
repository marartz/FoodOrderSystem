using System;
using FoodOrderSystem.Core.Domain.Model.Dish;

namespace FoodOrderSystem.Core.Application.DTOs
{
    public class CartDishInfoDTO
    {
        public CartDishInfoDTO(
            Guid itemId,
            DishId dishId,
            Guid variantId,
            int count,
            string remarks
        )
        {
            ItemId = itemId;
            DishId = dishId;
            VariantId = variantId;
            Count = count;
            Remarks = remarks;
        }
        
        public Guid ItemId { get; }
        public DishId DishId { get; }
        public Guid VariantId { get; }
        public int Count { get; }
        public string Remarks { get; }
    }
}