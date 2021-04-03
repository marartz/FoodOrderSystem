using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Restaurants;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Application.Ports.Persistence
{
    public class DishRepositoryMock : Mock<IDishRepository>
    {
        public DishRepositoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IDishRepository, Task<IEnumerable<Dish>>> SetupFindByRestaurantIdAsync(RestaurantId restaurantId)
        {
            return Setup(m => m.FindByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IDishRepository, Task<IEnumerable<Dish>>> SetupFindByDishCategoryIdAsync(
            DishCategoryId dishCategoryId)
        {
            return Setup(m => m.FindByDishCategoryIdAsync(dishCategoryId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IDishRepository, Task<Dish>> SetupFindByDishIdAsync(DishId dishId)
        {
            return Setup(m => m.FindByDishIdAsync(dishId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IDishRepository, Task> SetupStoreAsync(Dish dish)
        {
            return Setup(m => m.StoreAsync(dish, It.IsAny<CancellationToken>()));
        }

        public void VerifyStoreAsync(Dish dish, Func<Times> times)
        {
            Verify(m => m.StoreAsync(dish, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IDishRepository, Task> SetupRemoveByRestaurantIdAsync(RestaurantId restaurantId)
        {
            return Setup(m => m.RemoveByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()));
        }

        public void VerifyRemoveByRestaurantIdAsync(RestaurantId restaurantId, Func<Times> times)
        {
            Verify(m => m.RemoveByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IDishRepository, Task> SetupRemoveByDishCategoryIdAsync(DishCategoryId dishCategoryId)
        {
            return Setup(m => m.RemoveByDishCategoryIdAsync(dishCategoryId, It.IsAny<CancellationToken>()));
        }

        public void VerifyRemoveByDishCategoryIdAsync(DishCategoryId dishCategoryId, Func<Times> times)
        {
            Verify(m => m.RemoveByDishCategoryIdAsync(dishCategoryId, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IDishRepository, Task> SetupRemoveAsync(DishId dishId)
        {
            return Setup(m => m.RemoveAsync(dishId, It.IsAny<CancellationToken>()));
        }

        public void VerifyRemoveAsync(DishId dishId, Func<Times> times)
        {
            Verify(m => m.RemoveAsync(dishId, It.IsAny<CancellationToken>()), times);
        }
    }
}
