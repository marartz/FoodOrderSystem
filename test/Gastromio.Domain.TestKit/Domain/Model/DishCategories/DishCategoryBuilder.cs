using System;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.DishCategories
{
    public class DishCategoryBuilder : TestObjectBuilderBase<DishCategory>
    {
        protected override void AddDefaultConstraints()
        {
            WithLengthConstrainedStringConstructorArgumentFor("name", 0, 100);
            WithRangeConstrainedIntegerConstructorArgumentFor("orderNo", 0, 100);
            base.AddDefaultConstraints();
        }

        public DishCategoryBuilder WithId(DishCategoryId id)
        {
            WithConstantConstructorArgumentFor("id", id);
            return this;
        }

        public DishCategoryBuilder WithRestaurantId(RestaurantId restaurantId)
        {
            WithConstantConstructorArgumentFor("restaurantId", restaurantId);
            return this;
        }

        public DishCategoryBuilder WithName(string name)
        {
            WithConstantConstructorArgumentFor("name", name);
            return this;
        }

        public DishCategoryBuilder WithOrderNo(int orderNo)
        {
            WithConstantConstructorArgumentFor("orderNo", orderNo);
            return this;
        }

        public DishCategoryBuilder WithCreatedOn(DateTimeOffset createdOn)
        {
            WithConstantConstructorArgumentFor("createdOn", createdOn);
            return this;
        }

        public DishCategoryBuilder WithCreatedBy(UserId createdBy)
        {
            WithConstantConstructorArgumentFor("createdBy", createdBy);
            return this;
        }

        public DishCategoryBuilder WithUpdatedOn(DateTimeOffset updatedOn)
        {
            WithConstantConstructorArgumentFor("updatedOn", updatedOn);
            return this;
        }

        public DishCategoryBuilder WithUpdatedBy(UserId updatedBy)
        {
            WithConstantConstructorArgumentFor("updatedBy", updatedBy);
            return this;
        }
    }
}
