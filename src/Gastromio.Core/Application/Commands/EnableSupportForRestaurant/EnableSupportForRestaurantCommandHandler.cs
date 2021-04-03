using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.EnableSupportForRestaurant
{
    public class EnableSupportForRestaurantCommandHandler : ICommandHandler<EnableSupportForRestaurantCommand, bool>
    {
        private readonly IRestaurantRepository restaurantRepository;

        public EnableSupportForRestaurantCommandHandler(IRestaurantRepository restaurantRepository)
        {
            this.restaurantRepository = restaurantRepository;
        }

        public async Task<Result<bool>> HandleAsync(EnableSupportForRestaurantCommand command, User currentUser,
            CancellationToken cancellationToken = default)
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

            var tempResult = restaurant.EnableSupport(currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult;

            await restaurantRepository.StoreAsync(restaurant, cancellationToken);
            
            return SuccessResult<bool>.Create(true);
        }
    }
}
