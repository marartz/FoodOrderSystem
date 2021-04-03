using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.AddPaymentMethodToRestaurant
{
    public class AddPaymentMethodToRestaurantCommandHandler : ICommandHandler<AddPaymentMethodToRestaurantCommand, bool>
    {
        private readonly IRestaurantRepository restaurantRepository;

        public AddPaymentMethodToRestaurantCommandHandler(IRestaurantRepository restaurantRepository)
        {
            this.restaurantRepository = restaurantRepository;
        }

        public async Task<Result<bool>> HandleAsync(AddPaymentMethodToRestaurantCommand command, User currentUser, CancellationToken cancellationToken = default)
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

            restaurant.AddPaymentMethod(command.PaymentMethodId, currentUser.Id);

            await restaurantRepository.StoreAsync(restaurant, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
