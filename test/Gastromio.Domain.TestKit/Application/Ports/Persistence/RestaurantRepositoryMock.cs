using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Application.Ports.Persistence
{
    public class RestaurantRepositoryMock : Mock<IRestaurantRepository>
    {
        public RestaurantRepositoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IRestaurantRepository, Task<IEnumerable<Restaurant>>> SetupSearchAsync(string searchPhrase,
            OrderType? orderType, CuisineId cuisineId, DateTimeOffset? openingHour, bool? isActive)
        {
            return Setup(m => m.SearchAsync(searchPhrase, orderType, cuisineId, openingHour, isActive,
                It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantRepository, Task<(long total, IEnumerable<Restaurant> items)>> SetupSearchPagedAsync(
            string searchPhrase, OrderType? orderType, CuisineId cuisineId, DateTimeOffset? openingHour, bool? isActive,
            int skip = 0, int take = -1)
        {
            return Setup(m => m.SearchPagedAsync(searchPhrase, orderType, cuisineId, openingHour, isActive, skip, take,
                It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantRepository, Task<Restaurant>> SetupFindByRestaurantIdAsync(RestaurantId restaurantId)
        {
            return Setup(m => m.FindByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()));
        }

        public void VerifyFindByRestaurantIdAsync(RestaurantId restaurantId, Func<Times> times)
        {
            Verify(m => m.FindByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IRestaurantRepository, Task<IEnumerable<Restaurant>>> SetupFindByRestaurantNameAsync(
            string restaurantName)
        {
            return Setup(m => m.FindByRestaurantNameAsync(restaurantName, It.IsAny<CancellationToken>()));
        }

        public void VerifyFindByRestaurantNameAsync(string restaurantName, Func<Times> times)
        {
            Verify(m => m.FindByRestaurantNameAsync(restaurantName, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IRestaurantRepository, Task<Restaurant>> SetupFindByImportIdAsync(string importId)
        {
            return Setup(m => m.FindByImportIdAsync(importId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantRepository, Task<IEnumerable<Restaurant>>> SetupFindByCuisineIdAsync(CuisineId cuisineId)
        {
            return Setup(m => m.FindByCuisineIdAsync(cuisineId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantRepository, Task<IEnumerable<Restaurant>>> SetupFindByPaymentMethodIdAsync(
            PaymentMethodId paymentMethodId)
        {
            return Setup(m => m.FindByPaymentMethodIdAsync(paymentMethodId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantRepository, Task<IEnumerable<Restaurant>>> SetupFindByUserIdAsync(UserId userId)
        {
            return Setup(m => m.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantRepository, Task<IEnumerable<Restaurant>>> SetupFindAllAsync()
        {
            return Setup(m => m.FindAllAsync(It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantRepository, Task> SetupStoreAsync(Restaurant restaurant)
        {
            return Setup(m => m.StoreAsync(restaurant, It.IsAny<CancellationToken>()));
        }

        public void VerifyStoreAsync(Restaurant restaurant, Func<Times> times)
        {
            Verify(m => m.StoreAsync(restaurant, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IRestaurantRepository, Task> SetupRemoveAsync(RestaurantId restaurantId)
        {
            return Setup(m => m.RemoveAsync(restaurantId, It.IsAny<CancellationToken>()));
        }

        public void VerifyRemoveAsync(RestaurantId restaurantId, Func<Times> times)
        {
            Verify(m => m.RemoveAsync(restaurantId, It.IsAny<CancellationToken>()), times);
        }
    }
}
