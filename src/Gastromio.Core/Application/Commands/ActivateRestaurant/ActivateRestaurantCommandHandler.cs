using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.ActivateRestaurant
{
    public class ActivateRestaurantCommandHandler : ICommandHandler<ActivateRestaurantCommand, bool>
    {
        private readonly IRestaurantRepository restaurantRepository;

        public ActivateRestaurantCommandHandler(
            IRestaurantRepository restaurantRepository
        )
        {
            this.restaurantRepository = restaurantRepository;
        }

        public async Task<Result<bool>> HandleAsync(ActivateRestaurantCommand command, User currentUser, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<bool>.Unauthorized();

            if (currentUser.Role < Role.SystemAdmin)
                return FailureResult<bool>.Forbidden();

            var restaurant =
                await restaurantRepository.FindByRestaurantIdAsync(command.RestaurantId, cancellationToken);
            if (restaurant == null)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantDoesNotExist);

            var tempResult = restaurant.Activate(currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult;

            await restaurantRepository.StoreAsync(restaurant, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }

    }
}
