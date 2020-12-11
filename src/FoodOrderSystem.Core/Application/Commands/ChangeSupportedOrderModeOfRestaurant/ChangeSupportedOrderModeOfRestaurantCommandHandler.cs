using System;
using System.Threading;
using System.Threading.Tasks;
using FoodOrderSystem.Core.Application.Ports.Persistence;
using FoodOrderSystem.Core.Common;
using FoodOrderSystem.Core.Domain.Model.User;

namespace FoodOrderSystem.Core.Application.Commands.ChangeSupportedOrderModeOfRestaurant
{
    public class
        ChangeSupportedOrderModeOfRestaurantCommandHandler : ICommandHandler<ChangeSupportedOrderModeOfRestaurantCommand
            , bool>
    {
        private readonly IRestaurantRepository restaurantRepository;

        public ChangeSupportedOrderModeOfRestaurantCommandHandler(
            IRestaurantRepository restaurantRepository
        )
        {
            this.restaurantRepository = restaurantRepository;
        }
        
        public async Task<Result<bool>> HandleAsync(ChangeSupportedOrderModeOfRestaurantCommand command, User currentUser,
            CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<bool>.Unauthorized();

            if (currentUser.Role < Role.RestaurantAdmin)
                return FailureResult<bool>.Forbidden();

            var restaurant = await restaurantRepository.FindByRestaurantIdAsync(command.RestaurantId, cancellationToken);
            if (restaurant == null)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantDoesNotExist);

            if (currentUser.Role == Role.RestaurantAdmin && !restaurant.HasAdministrator(currentUser.Id))
                return FailureResult<bool>.Forbidden();

            var result = restaurant.ChangeSupportedOrderMode(command.SupportedOrderMode, currentUser.Id);
            if (result.IsFailure)
                return result;

            await restaurantRepository.StoreAsync(restaurant, cancellationToken);

            return result;
        }
    }
}
