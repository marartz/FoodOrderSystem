using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.ChangeDishCategoryOfRestaurant
{
    public class ChangeDishCategoryOfRestaurantCommandHandler : ICommandHandler<ChangeDishCategoryOfRestaurantCommand, bool>
    {
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IDishCategoryRepository dishCategoryRepository;

        public ChangeDishCategoryOfRestaurantCommandHandler(IRestaurantRepository restaurantRepository,
            IDishCategoryRepository dishCategoryRepository)
        {
            this.restaurantRepository = restaurantRepository;
            this.dishCategoryRepository = dishCategoryRepository;
        }

        public async Task<Result<bool>> HandleAsync(ChangeDishCategoryOfRestaurantCommand command, User currentUser, CancellationToken cancellationToken = default)
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
            var dishCategoryList = dishCategories?.ToList();
            var dishCategory = dishCategoryList?.FirstOrDefault(en => en.Id == command.DishCategoryId);
            if (dishCategory == null)
                return FailureResult<bool>.Create(FailureResultCode.DishCategoryDoesNotBelongToRestaurant);
            
            if (dishCategoryList.Any(en => en.Id != command.DishCategoryId && string.Equals(en.Name, command.Name)))
                return FailureResult<bool>.Create(FailureResultCode.DishCategoryAlreadyExists);

            dishCategory.ChangeName(command.Name, currentUser.Id);

            await dishCategoryRepository.StoreAsync(dishCategory, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
