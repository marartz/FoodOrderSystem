using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.AddRestaurant
{
    public class AddRestaurantCommandHandler : ICommandHandler<AddRestaurantCommand, RestaurantDTO>
    {
        private readonly IRestaurantFactory restaurantFactory;
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IRestaurantImageRepository restaurantImageRepository;
        private readonly ICuisineRepository cuisineRepository;
        private readonly IPaymentMethodRepository paymentMethodRepository;
        private readonly IUserRepository userRepository;

        public AddRestaurantCommandHandler(
            IRestaurantFactory restaurantFactory,
            IRestaurantRepository restaurantRepository,
            IRestaurantImageRepository restaurantImageRepository,
            ICuisineRepository cuisineRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IUserRepository userRepository)
        {
            this.restaurantFactory = restaurantFactory;
            this.restaurantRepository = restaurantRepository;
            this.restaurantImageRepository = restaurantImageRepository;
            this.cuisineRepository = cuisineRepository;
            this.paymentMethodRepository = paymentMethodRepository;
            this.userRepository = userRepository;
        }

        public async Task<Result<RestaurantDTO>> HandleAsync(AddRestaurantCommand command, User currentUser, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<RestaurantDTO>.Unauthorized();

            if (currentUser.Role < Role.SystemAdmin)
                return FailureResult<RestaurantDTO>.Forbidden();

            var existingRestaurants =
                await restaurantRepository.FindByRestaurantNameAsync(command.Name, cancellationToken);
            if (existingRestaurants.Any())
                return FailureResult<RestaurantDTO>.Create(FailureResultCode.RestaurantAlreadyExists);

            var cuisines = (await cuisineRepository.FindAllAsync(cancellationToken))
                .ToDictionary(en => en.Id.Value, en => new CuisineDTO(en));

            var paymentMethods = (await paymentMethodRepository.FindAllAsync(cancellationToken))
                .ToDictionary(en => en.Id.Value, en => new PaymentMethodDTO(en));

            var createResult = restaurantFactory.CreateWithName(command.Name, currentUser.Id);
            if (createResult.IsFailure)
                return createResult.Cast<RestaurantDTO>();

            var restaurant = ((SuccessResult<Restaurant>) createResult).Value;
            
            await restaurantRepository.StoreAsync(restaurant, cancellationToken);

            var userIds = restaurant.Administrators;
            
            var users = await userRepository.FindByUserIdsAsync(userIds, cancellationToken);

            var userDict = users != null
                ? users.ToDictionary(user => user.Id, user => new UserDTO(user))
                : new Dictionary<UserId, UserDTO>();

            var restaurantImageTypes =
                await restaurantImageRepository.FindTypesByRestaurantIdsAsync(new[] {restaurant.Id}, cancellationToken);

            return SuccessResult<RestaurantDTO>.Create(new RestaurantDTO(restaurant, cuisines, paymentMethods,
                userDict, restaurantImageTypes));
        }
    }
}
