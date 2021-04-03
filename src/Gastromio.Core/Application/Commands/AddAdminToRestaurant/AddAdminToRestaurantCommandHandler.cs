using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.AddAdminToRestaurant
{
    public class AddAdminToRestaurantCommandHandler : ICommandHandler<AddAdminToRestaurantCommand, bool>
    {
        private readonly IRestaurantRepository restaurantRepository;

        public AddAdminToRestaurantCommandHandler(IRestaurantRepository restaurantRepository)
        {
            this.restaurantRepository = restaurantRepository;
        }

        public async Task<Result<bool>> HandleAsync(AddAdminToRestaurantCommand command, User currentUser,
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

            restaurant.AddAdministrator(command.UserId, currentUser.Id);

            await restaurantRepository.StoreAsync(restaurant, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
