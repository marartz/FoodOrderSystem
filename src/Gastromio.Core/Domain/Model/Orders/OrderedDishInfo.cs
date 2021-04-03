using System;
using Gastromio.Core.Domain.Model.Dishes;

namespace Gastromio.Core.Domain.Model.Orders
{
    public class OrderedDishInfo
    {
        public OrderedDishInfo(
            Guid itemId,
            DishId dishId,
            string dishName,
            Guid variantId,
            string variantName,
            decimal variantPrice,
            int count,
            string remarks
        )
        {
            ItemId = itemId;
            DishId = dishId;
            DishName = dishName;
            VariantId = variantId;
            VariantName = variantName;
            VariantPrice = variantPrice;
            Count = count;
            Remarks = remarks;
        }
        
        public Guid ItemId { get; }
        public DishId DishId { get; }
        public string DishName { get; }
        public Guid VariantId { get; }
        public string VariantName { get; }
        public decimal VariantPrice { get; }
        public int Count { get; }
        public string Remarks { get; }
    }
}
