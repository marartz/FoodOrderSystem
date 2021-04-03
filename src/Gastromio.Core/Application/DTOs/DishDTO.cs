using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Gastromio.Core.Domain.Model.Dishes;

namespace Gastromio.Core.Application.DTOs
{
    public class DishDTO
    {
        public DishDTO(Guid id, string name, string description, string productInfo, int orderNo, IEnumerable<DishVariant> variants)
        {
            Id = id;
            Name = name;
            Description = description;
            ProductInfo = productInfo;
            OrderNo = orderNo;
            Variants = new ReadOnlyCollection<DishVariant>(variants.ToList());
        }

        internal DishDTO(Dish dish)
        {
            Id = dish.Id.Value;
            Name = dish.Name;
            Description = dish.Description;
            ProductInfo = dish.ProductInfo;
            OrderNo = dish.OrderNo;
            Variants = dish.Variants;
        }

        public Guid Id { get; }
        
        public string Name { get; }
        
        public string Description { get; }
        
        public string ProductInfo { get; }
        
        public int OrderNo { get; }
        
        public IReadOnlyCollection<DishVariant> Variants { get; }
    }
}
