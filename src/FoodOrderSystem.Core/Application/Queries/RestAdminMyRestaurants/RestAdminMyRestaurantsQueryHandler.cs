﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FoodOrderSystem.Core.Application.DTOs;
using FoodOrderSystem.Core.Application.Ports.Persistence;
using FoodOrderSystem.Core.Common;
using FoodOrderSystem.Core.Domain.Model.PaymentMethod;
using FoodOrderSystem.Core.Domain.Model.User;

namespace FoodOrderSystem.Core.Application.Queries.RestAdminMyRestaurants
{
    public class RestAdminMyRestaurantsQueryHandler : IQueryHandler<RestAdminMyRestaurantsQuery, ICollection<RestaurantDTO>>
    {
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IRestaurantImageRepository restaurantImageRepository;
        private readonly ICuisineRepository cuisineRepository;
        private readonly IPaymentMethodRepository paymentMethodRepository;
        private readonly IUserRepository userRepository;

        public RestAdminMyRestaurantsQueryHandler(
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

        public async Task<Result<ICollection<RestaurantDTO>>> HandleAsync(RestAdminMyRestaurantsQuery query, User currentUser, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (currentUser == null)
                return FailureResult<ICollection<RestaurantDTO>>.Unauthorized();

            if (currentUser.Role < Role.RestaurantAdmin)
                return FailureResult<ICollection<RestaurantDTO>>.Forbidden();

            var cuisines = (await cuisineRepository.FindAllAsync(cancellationToken))
                .ToDictionary(en => en.Id.Value, en => new CuisineDTO(en));

            var paymentMethods = (await paymentMethodRepository.FindAllAsync(cancellationToken))
                .ToDictionary(en => en.Id.Value, en => new PaymentMethodDTO(en));

            var restaurants = (await restaurantRepository.FindByUserIdAsync(currentUser.Id, cancellationToken)).ToList();

            var userIds = restaurants.SelectMany(restaurant => restaurant.Administrators).Distinct();
            
            var users = await userRepository.FindByUserIdsAsync(userIds, cancellationToken);

            var userDict = users != null
                ? users.ToDictionary(user => user.Id, user => new UserDTO(user))
                : new Dictionary<UserId, UserDTO>();

            var restaurantImageTypes =
                await restaurantImageRepository.FindTypesByRestaurantIdsAsync(
                    restaurants.Select(restaurant => restaurant.Id), cancellationToken);

            return SuccessResult<ICollection<RestaurantDTO>>.Create(restaurants.Select(en =>
                new RestaurantDTO(en, cuisines, paymentMethods, userDict, restaurantImageTypes)).ToList());
        }
    }
}
