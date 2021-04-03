using System.Collections.Generic;
using Gastromio.Core.Application.DTOs;

namespace Gastromio.Core.Application.Queries.GetDishesOfRestaurantForOrder
{
    public class GetDishesOfRestaurantForOrderQuery : IQuery<ICollection<DishCategoryDTO>>
    {
        public GetDishesOfRestaurantForOrderQuery(string restaurantId)
        {
            RestaurantId = restaurantId;
        }

        public string RestaurantId { get; }
    }
}
