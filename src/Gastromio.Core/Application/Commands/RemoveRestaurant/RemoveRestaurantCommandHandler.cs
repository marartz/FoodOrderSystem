using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.RemoveRestaurant
{
    public class RemoveRestaurantCommandHandler : ICommandHandler<RemoveRestaurantCommand, bool>
    {
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IRestaurantImageRepository restaurantImageRepository;
        private readonly IDishCategoryRepository dishCategoryRepository;
        private readonly IDishRepository dishRepository;

        public RemoveRestaurantCommandHandler(
            IRestaurantRepository restaurantRepository,
            IRestaurantImageRepository restaurantImageRepository,
            IDishCategoryRepository dishCategoryRepository,
            IDishRepository dishRepository
        )
        {
            this.restaurantRepository = restaurantRepository;
            this.restaurantImageRepository = restaurantImageRepository;
            this.dishCategoryRepository = dishCategoryRepository;
            this.dishRepository = dishRepository;
        }

        public async Task<Result<bool>> HandleAsync(RemoveRestaurantCommand command, User currentUser, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<bool>.Unauthorized();

            if (currentUser.Role < Role.SystemAdmin)
                return FailureResult<bool>.Forbidden();

            await dishRepository.RemoveByRestaurantIdAsync(command.RestaurantId, cancellationToken);
            await dishCategoryRepository.RemoveByRestaurantIdAsync(command.RestaurantId, cancellationToken);
            await restaurantImageRepository.RemoveByRestaurantIdAsync(command.RestaurantId, cancellationToken);
            await restaurantRepository.RemoveAsync(command.RestaurantId, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
