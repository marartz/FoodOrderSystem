using System.Collections.Generic;
using Gastromio.Core.Application.DTOs;

namespace Gastromio.Core.Application.Queries.GetDishesOfRestaurantForAdmin
{
    public class GetDishesOfRestaurantForAdminQuery : IQuery<ICollection<DishCategoryDTO>>
    {
        public GetDishesOfRestaurantForAdminQuery(string restaurantId)
        {
            RestaurantId = restaurantId;
        }

        public string RestaurantId { get; }
    }
}
