using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Domain.Model.RestaurantImages;
using Gastromio.Core.Domain.Model.Restaurants;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Application.Ports.Persistence
{
    public class RestaurantImageRepositoryMock : Mock<IRestaurantImageRepository>
    {
        public RestaurantImageRepositoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IRestaurantImageRepository, Task<RestaurantImage>> SetupFindByRestaurantImageIdAsync(
            RestaurantImageId restaurantImageId)
        {
            return Setup(m => m.FindByRestaurantImageIdAsync(restaurantImageId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantImageRepository, Task<RestaurantImage>> SetupFindByRestaurantIdAndTypeAsync(
            RestaurantId restaurantId, string type)
        {
            return Setup(m => m.FindByRestaurantIdAndTypeAsync(restaurantId, type, It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantImageRepository, Task<IEnumerable<RestaurantImage>>> SetupFindByRestaurantIdAsync(
            RestaurantId restaurantId)
        {
            return Setup(m => m.FindByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantImageRepository, Task<IEnumerable<string>>> SetupFindTypesByRestaurantIdAsync(
            RestaurantId restaurantId)
        {
            return Setup(m => m.FindTypesByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantImageRepository, Task<IDictionary<RestaurantId, IEnumerable<string>>>>
            SetupFindTypesByRestaurantIdsAsync(IEnumerable<RestaurantId> restaurantIds)
        {
            return Setup(m => m.FindTypesByRestaurantIdsAsync(restaurantIds, It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantImageRepository, Task> SetupStoreAsync(RestaurantImage restaurantImage)
        {
            return Setup(m => m.StoreAsync(restaurantImage, It.IsAny<CancellationToken>()));
        }

        public void VerifyStoreAsync(RestaurantImage restaurantImage, Func<Times> times)
        {
            Verify(m => m.StoreAsync(restaurantImage, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IRestaurantImageRepository, Task> SetupRemoveByRestaurantImageId(
            RestaurantImageId restaurantImageId, string type)
        {
            return Setup(m => m.RemoveByRestaurantImageId(restaurantImageId, type, It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantImageRepository, Task> SetupRemoveByRestaurantIdAndTypeAsync(RestaurantId restaurantId,
            string type)
        {
            return Setup(m => m.RemoveByRestaurantIdAndTypeAsync(restaurantId, type, It.IsAny<CancellationToken>()));
        }

        public ISetup<IRestaurantImageRepository, Task> SetupRemoveByRestaurantIdAsync(RestaurantId restaurantId)
        {
            return Setup(m => m.RemoveByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()));
        }

        public void VerifyRemoveByRestaurantIdAsync(RestaurantId restaurantId, Func<Times> times)
        {
            Verify(m => m.RemoveByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()), times);
        }
    }
}
