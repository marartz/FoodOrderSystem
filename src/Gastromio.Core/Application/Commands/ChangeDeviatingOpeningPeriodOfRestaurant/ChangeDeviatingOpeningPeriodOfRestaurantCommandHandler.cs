using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.ChangeDeviatingOpeningPeriodOfRestaurant
{
    public class ChangeDeviatingOpeningPeriodOfRestaurantCommandHandler : ICommandHandler<ChangeDeviatingOpeningPeriodOfRestaurantCommand, bool>
    {
        private readonly IRestaurantRepository restaurantRepository;

        public ChangeDeviatingOpeningPeriodOfRestaurantCommandHandler(IRestaurantRepository restaurantRepository)
        {
            this.restaurantRepository = restaurantRepository;
        }

        public async Task<Result<bool>> HandleAsync(ChangeDeviatingOpeningPeriodOfRestaurantCommand command, User currentUser, CancellationToken cancellationToken = default)
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

            var result = restaurant.RemoveDeviatingOpeningPeriod(command.Date, command.OldStart, currentUser.Id);
            if (result.IsFailure)
                return result;

            var openingPeriod = new OpeningPeriod(command.NewStart, command.NewEnd);

            result = restaurant.AddDeviatingOpeningPeriod(command.Date, openingPeriod, currentUser.Id);
            if (result.IsFailure)
                return result;

            await restaurantRepository.StoreAsync(restaurant, cancellationToken);

            return result;
        }
    }
}
