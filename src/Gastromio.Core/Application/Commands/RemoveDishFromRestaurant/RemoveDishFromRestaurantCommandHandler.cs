using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.RemoveDishFromRestaurant
{
    public class RemoveDishFromRestaurantCommandHandler : ICommandHandler<RemoveDishFromRestaurantCommand, bool>
    {
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IDishCategoryRepository dishCategoryRepository;
        private readonly IDishRepository dishRepository;

        public RemoveDishFromRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IDishCategoryRepository dishCategoryRepository, IDishRepository dishRepository)
        {
            this.restaurantRepository = restaurantRepository;
            this.dishCategoryRepository = dishCategoryRepository;
            this.dishRepository = dishRepository;
        }

        public async Task<Result<bool>> HandleAsync(RemoveDishFromRestaurantCommand command, User currentUser, CancellationToken cancellationToken = default)
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

            var dishCategories = await dishCategoryRepository.FindByRestaurantIdAsync(command.RestaurantId, cancellationToken);
            var dishCategory = dishCategories?.FirstOrDefault(en => en.Id == command.DishCategoryId);
            if (dishCategory == null)
                return FailureResult<bool>.Create(FailureResultCode.DishCategoryDoesNotBelongToRestaurant);

            var dishes = await dishRepository.FindByDishCategoryIdAsync(dishCategory.Id, cancellationToken);
            var dish = dishes?.FirstOrDefault(en => en.Id == command.DishId);
            if (dish == null)
                return FailureResult<bool>.Create(FailureResultCode.DishDoesNotBelongToDishCategory);

            await dishRepository.RemoveAsync(command.DishId, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
