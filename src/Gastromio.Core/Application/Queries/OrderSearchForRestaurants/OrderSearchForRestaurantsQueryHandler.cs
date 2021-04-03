using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Queries.OrderSearchForRestaurants
{
    public class OrderSearchForRestaurantsQueryHandler : IQueryHandler<OrderSearchForRestaurantsQuery, ICollection<RestaurantDTO>>
    {
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IRestaurantImageRepository restaurantImageRepository;
        private readonly ICuisineRepository cuisineRepository;
        private readonly IPaymentMethodRepository paymentMethodRepository;
        private readonly IUserRepository userRepository;

        public OrderSearchForRestaurantsQueryHandler(
            IRestaurantRepository restaurantRepository,
            IRestaurantImageRepository restaurantImageRepository,
            ICuisineRepository cuisineRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IUserRepository userRepository
        )
        {
            this.restaurantRepository = restaurantRepository;
            this.restaurantImageRepository = restaurantImageRepository;
            this.cuisineRepository = cuisineRepository;
            this.paymentMethodRepository = paymentMethodRepository;
            this.userRepository = userRepository;
        }

        public async Task<Result<ICollection<RestaurantDTO>>> HandleAsync(OrderSearchForRestaurantsQuery query, User currentUser, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var cuisines = (await cuisineRepository.FindAllAsync(cancellationToken))
                .ToDictionary(en => en.Id.Value, en => new CuisineDTO(en));

            var paymentMethods = (await paymentMethodRepository.FindAllAsync(cancellationToken))
                .ToDictionary(en => en.Id.Value, en => new PaymentMethodDTO(en));

            var (_, items) = await restaurantRepository.SearchPagedAsync(query.SearchPhrase, query.OrderType,
                query.CuisineId, query.OpeningHour, true, 0, -1, cancellationToken);

            var itemList = items.ToList();

            var userIds = itemList != null
                ? itemList.SelectMany(restaurant => restaurant.Administrators).Distinct()
                : Enumerable.Empty<UserId>();
            
            var users = await userRepository.FindByUserIdsAsync(userIds, cancellationToken);

            var userDict = users != null
                ? users.ToDictionary(user => user.Id, user => new UserDTO(user))
                : new Dictionary<UserId, UserDTO>();

            var restaurantImageTypes =
                await restaurantImageRepository.FindTypesByRestaurantIdsAsync(
                    itemList.Select(restaurant => restaurant.Id), cancellationToken);

            return SuccessResult<ICollection<RestaurantDTO>>.Create(itemList.Select(en =>
                new RestaurantDTO(en, cuisines, paymentMethods, userDict, restaurantImageTypes)).ToList());
        }
    }
}
