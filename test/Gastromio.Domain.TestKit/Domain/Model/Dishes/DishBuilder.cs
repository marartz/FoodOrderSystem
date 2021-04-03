using System;
using System.Collections.Generic;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Dishes
{
    public class DishBuilder : TestObjectBuilderBase<Dish>
    {
        public DishBuilder WithValidConstrains()
        {
            WithName("dish-name");
            WithDescription("dish-description");
            WithProductInfo("dish-product-info");
            WithOrderNo(1);
            return this;
        }

        public DishBuilder WithId(DishId id)
        {
            WithConstantConstructorArgumentFor("id", id);
            return this;
        }

        public DishBuilder WithRestaurantId(RestaurantId restaurantId)
        {
            WithConstantConstructorArgumentFor("restaurantId", restaurantId);
            return this;
        }

        public DishBuilder WithCategoryId(DishCategoryId categoryId)
        {
            WithConstantConstructorArgumentFor("categoryId", categoryId);
            return this;
        }

        public DishBuilder WithName(string name)
        {
            WithConstantConstructorArgumentFor("name", name);
            return this;
        }

        public DishBuilder WithDescription(string description)
        {
            WithConstantConstructorArgumentFor("description", description);
            return this;
        }

        public DishBuilder WithProductInfo(string productInfo)
        {
            WithConstantConstructorArgumentFor("productInfo", productInfo);
            return this;
        }

        public DishBuilder WithOrderNo(int orderNo)
        {
            WithConstantConstructorArgumentFor("orderNo", orderNo);
            return this;
        }

        public DishBuilder WithoutVariants()
        {
            return WithVariants(new List<DishVariant>());
        }

        public DishBuilder WithVariants(IList<DishVariant> variants)
        {
            WithConstantConstructorArgumentFor("variants", variants);
            return this;
        }

        public DishBuilder WithCreatedOn(DateTimeOffset createdOn)
        {
            WithConstantConstructorArgumentFor("createdOn", createdOn);
            return this;
        }

        public DishBuilder WithCreatedBy(UserId createdBy)
        {
            WithConstantConstructorArgumentFor("createdBy", createdBy);
            return this;
        }

        public DishBuilder WithUpdatedOn(DateTimeOffset updatedOn)
        {
            WithConstantConstructorArgumentFor("updatedOn", updatedOn);
            return this;
        }

        public DishBuilder WithUpdatedBy(UserId updatedBy)
        {
            WithConstantConstructorArgumentFor("updatedBy", updatedBy);
            return this;
        }
    }
}
