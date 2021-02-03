﻿using System;
using System.Collections.Generic;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.DishCategory;
using Gastromio.Core.Domain.Model.Restaurant;
using Gastromio.Core.Domain.Model.User;

namespace Gastromio.Core.Domain.Model.Dish
{
    public class DishFactory : IDishFactory
    {
        public Result<Dish> Create(
            RestaurantId restaurantId,
            DishCategoryId categoryId,
            string name,
            string description,
            string productInfo,
            int orderNo,
            IEnumerable<DishVariant> variants,
            UserId createdBy
        )
        {
            if (restaurantId.Value == Guid.Empty)
                return FailureResult<Dish>.Create(FailureResultCode.DishRestaurantIdRequired, nameof(restaurantId));
            if (categoryId.Value == Guid.Empty)
                return FailureResult<Dish>.Create(FailureResultCode.DishCategoryIdRequired, nameof(categoryId));

            var dish = new Dish(
                new DishId(Guid.NewGuid()),
                restaurantId,
                categoryId,
                DateTimeOffset.UtcNow,
                createdBy,
                DateTimeOffset.UtcNow,
                createdBy
            );

            var tempResult = dish.ChangeName(name, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<Dish>();

            tempResult = dish.ChangeDescription(description, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<Dish>();

            tempResult = dish.ChangeProductInfo(productInfo, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<Dish>();

            tempResult = dish.ChangeOrderNo(orderNo, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<Dish>();

            foreach (var variant in variants)
            {
                tempResult = dish.AddVariant(variant.VariantId, variant.Name, variant.Price, createdBy);
                if (tempResult.IsFailure)
                    return tempResult.Cast<Dish>();
            }

            return SuccessResult<Dish>.Create(dish);
        }
    }
}
