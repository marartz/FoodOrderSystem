using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Ports.Persistence
{
    public interface IRestaurantRepository
    {
        Task<IEnumerable<Restaurant>> SearchAsync(string searchPhrase, OrderType? orderType, CuisineId cuisineId,
            DateTimeOffset? openingHour, bool? isActive, CancellationToken cancellationToken = default);

        Task<(long total, IEnumerable<Restaurant> items)> SearchPagedAsync(string searchPhrase, OrderType? orderType,
            CuisineId cuisineId, DateTimeOffset? openingHour, bool? isActive, int skip = 0, int take = -1, CancellationToken cancellationToken = default);

        Task<Restaurant> FindByRestaurantIdAsync(RestaurantId restaurantId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Restaurant>> FindByRestaurantNameAsync(string restaurantName, CancellationToken cancellationToken = default);

        Task<Restaurant> FindByImportIdAsync(string importId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Restaurant>> FindByCuisineIdAsync(CuisineId cuisineId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Restaurant>> FindByPaymentMethodIdAsync(PaymentMethodId paymentMethodId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Restaurant>> FindByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Restaurant>> FindAllAsync(CancellationToken cancellationToken = default);

        Task StoreAsync(Restaurant restaurant, CancellationToken cancellationToken = default);

        Task RemoveAsync(RestaurantId restaurantId, CancellationToken cancellationToken = default);
    }
}
