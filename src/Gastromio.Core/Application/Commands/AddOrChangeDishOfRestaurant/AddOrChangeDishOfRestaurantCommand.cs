using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Restaurants;


namespace Gastromio.Core.Application.Commands.AddOrChangeDishOfRestaurant
{
    public class AddOrChangeDishOfRestaurantCommand : ICommand<Guid>
    {
        public AddOrChangeDishOfRestaurantCommand(RestaurantId restaurantId, DishCategoryId dishCategoryId,
            Guid dishId, string name, string description, string productInfo, int orderNo, IEnumerable<DishVariant> variants)
        {
            RestaurantId = restaurantId;
            DishCategoryId = dishCategoryId;
            DishId = dishId;
            Name = name;
            Description = description;
            ProductInfo = productInfo;
            OrderNo = orderNo;
            Variants = new ReadOnlyCollection<DishVariant>(variants.ToList());
        }

        public RestaurantId RestaurantId { get; }
        
        public DishCategoryId DishCategoryId { get; }

        public Guid DishId { get; }
        
        public string Name { get; }
        
        public string Description { get; }
        
        public string ProductInfo { get; }
        
        public int OrderNo { get; }
        
        public IReadOnlyCollection<DishVariant> Variants { get; }
    }
}
