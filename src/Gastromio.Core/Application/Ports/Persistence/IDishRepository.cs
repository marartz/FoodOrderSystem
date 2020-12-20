﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Domain.Model.Dish;
using Gastromio.Core.Domain.Model.DishCategory;
using Gastromio.Core.Domain.Model.Restaurant;

namespace Gastromio.Core.Application.Ports.Persistence
{
    public interface IDishRepository
    {
        Task<IEnumerable<Dish>> FindByRestaurantIdAsync(RestaurantId restaurantId, CancellationToken cancellationToken);

        Task<IEnumerable<Dish>> FindByDishCategoryIdAsync(DishCategoryId dishCategoryId, CancellationToken cancellationToken);

        Task<Dish> FindByDishIdAsync(DishId dishId, CancellationToken cancellationToken);

        Task StoreAsync(Dish dish, CancellationToken cancellationToken = default);

        Task RemoveByRestaurantIdAsync(RestaurantId restaurantId, CancellationToken cancellationToken = default);

        Task RemoveByDishCategoryIdAsync(DishCategoryId dishCategoryId, CancellationToken cancellationToken = default);

        Task RemoveAsync(DishId dishId, CancellationToken cancellationToken = default);
    }
}
