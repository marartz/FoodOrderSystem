using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Application.Ports.Persistence
{
    public class DishCategoryRepositoryMock : Mock<IDishCategoryRepository>
    {
        public DishCategoryRepositoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IDishCategoryRepository, Task<IEnumerable<DishCategory>>> SetupFindByRestaurantIdAsync(
            RestaurantId restaurantId)
        {
            return Setup(m => m.FindByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IDishCategoryRepository, Task<DishCategory>> SetupFindByDishCategoryIdAsync(
            DishCategoryId dishCategoryId)
        {
            return Setup(m => m.FindByDishCategoryIdAsync(dishCategoryId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IDishCategoryRepository, Task> SetupStoreAsync(DishCategory dishCategory)
        {
            return Setup(m => m.StoreAsync(dishCategory, It.IsAny<CancellationToken>()));
        }

        public void VerifyStoreAsync(DishCategory dishCategory, Func<Times> times)
        {
            Verify(m => m.StoreAsync(dishCategory, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IDishCategoryRepository, Task> SetupRemoveByRestaurantIdAsync(RestaurantId restaurantId)
        {
            return Setup(m => m.RemoveByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()));
        }

        public void VerifyRemoveByRestaurantIdAsync(RestaurantId restaurantId, Func<Times> times)
        {
            Verify(m => m.RemoveByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IDishCategoryRepository, Task> SetupRemoveAsync(DishCategoryId dishCategoryId)
        {
            return Setup(m => m.RemoveAsync(dishCategoryId, It.IsAny<CancellationToken>()));
        }

        public void VerifyRemoveAsync(DishCategoryId dishCategoryId, Func<Times> times)
        {
            Verify(m => m.RemoveAsync(dishCategoryId, It.IsAny<CancellationToken>()), times);
        }
    }
}
