using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Model.DishCategories
{
    public interface IDishCategoryFactory
    {
        Result<DishCategory> Create(
            RestaurantId restaurantId,
            string name,
            int orderNo,
            UserId createdBy
        );
    }
}
