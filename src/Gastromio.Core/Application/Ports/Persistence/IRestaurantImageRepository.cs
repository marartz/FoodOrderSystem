using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Domain.Model.RestaurantImages;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Ports.Persistence
{
    public interface IRestaurantImageRepository
    {
        Task<RestaurantImage> FindByRestaurantImageIdAsync(RestaurantImageId restaurantImageId,
            CancellationToken cancellationToken = default);

        Task<RestaurantImage> FindByRestaurantIdAndTypeAsync(RestaurantId restaurantId, string type,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<RestaurantImage>> FindByRestaurantIdAsync(RestaurantId restaurantId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> FindTypesByRestaurantIdAsync(RestaurantId restaurantId,
            CancellationToken cancellationToken = default);

        Task<IDictionary<RestaurantId, IEnumerable<string>>> FindTypesByRestaurantIdsAsync(IEnumerable<RestaurantId> restaurantIds,
            CancellationToken cancellationToken = default);

        Task StoreAsync(RestaurantImage restaurantImage, CancellationToken cancellationToken = default);

        Task RemoveByRestaurantImageId(RestaurantImageId restaurantImageId, string type,
            CancellationToken cancellationToken = default);

        Task RemoveByRestaurantIdAndTypeAsync(RestaurantId restaurantId, string type,
            CancellationToken cancellationToken = default);

        Task RemoveByRestaurantIdAsync(RestaurantId restaurantId, CancellationToken cancellationToken = default);
    }
}
