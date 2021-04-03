using System;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Model.DishCategories
{
    public class DishCategoryFactory : IDishCategoryFactory
    {
        public Result<DishCategory> Create(
            RestaurantId restaurantId,
            string name,
            int orderNo,
            UserId createdBy
        )
        {
            var dishCategory = new DishCategory(
                new DishCategoryId(Guid.NewGuid()),
                restaurantId,
                null,
                0,
                true,
                DateTimeOffset.UtcNow,
                createdBy,
                DateTimeOffset.UtcNow,
                createdBy
            );

            var tempResult = dishCategory.ChangeName(name, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<DishCategory>();

            tempResult = dishCategory.ChangeOrderNo(orderNo, createdBy);

            return tempResult.IsFailure
                ? tempResult.Cast<DishCategory>()
                : SuccessResult<DishCategory>.Create(dishCategory);
        }
    }
}
