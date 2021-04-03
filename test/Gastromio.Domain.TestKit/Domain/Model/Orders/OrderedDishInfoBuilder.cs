using System;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Orders
{
    public class OrderedDishInfoBuilder : TestObjectBuilderBase<OrderedDishInfo>
    {
        public OrderedDishInfoBuilder WithItemId(Guid itemId)
        {
            WithConstantConstructorArgumentFor("itemId", itemId);
            return this;
        }

        public OrderedDishInfoBuilder WithDishId(DishId dishId)
        {
            WithConstantConstructorArgumentFor("dishId", dishId);
            return this;
        }

        public OrderedDishInfoBuilder WithDishName(string dishName)
        {
            WithConstantConstructorArgumentFor("dishName", dishName);
            return this;
        }

        public OrderedDishInfoBuilder WithVariantId(Guid variantId)
        {
            WithConstantConstructorArgumentFor("variantId", variantId);
            return this;
        }

        public OrderedDishInfoBuilder WithVariantName(Guid variantName)
        {
            WithConstantConstructorArgumentFor("variantName", variantName);
            return this;
        }

        public OrderedDishInfoBuilder WithVariantPrice(decimal variantPrice)
        {
            WithConstantConstructorArgumentFor("variantPrice", variantPrice);
            return this;
        }

        public OrderedDishInfoBuilder WithCount(int count)
        {
            WithConstantConstructorArgumentFor("count", count);
            return this;
        }

        public OrderedDishInfoBuilder WithRemarks(string remarks)
        {
            WithConstantConstructorArgumentFor("remarks", remarks);
            return this;
        }
    }
}
