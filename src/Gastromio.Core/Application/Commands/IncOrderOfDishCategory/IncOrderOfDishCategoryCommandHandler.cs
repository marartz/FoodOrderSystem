using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.IncOrderOfDishCategory
{
    public class IncOrderOfDishCategoryCommandHandler : ICommandHandler<IncOrderOfDishCategoryCommand, bool>
    {
        private readonly IDishCategoryRepository dishCategoryRepository;

        public IncOrderOfDishCategoryCommandHandler(IDishCategoryRepository dishCategoryRepository)
        {
            this.dishCategoryRepository = dishCategoryRepository;
        }

        public async Task<Result<bool>> HandleAsync(IncOrderOfDishCategoryCommand command, User currentUser,
            CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<bool>.Unauthorized();

            if (currentUser.Role < Role.RestaurantAdmin)
                return FailureResult<bool>.Forbidden();

            var curCategory =
                await dishCategoryRepository.FindByDishCategoryIdAsync(command.CategoryId, cancellationToken);
            if (curCategory == null)
                return FailureResult<bool>.Create(FailureResultCode.DishCategoryDoesNotExist);

            var curCategories =
                (await dishCategoryRepository.FindByRestaurantIdAsync(curCategory.RestaurantId, cancellationToken))
                .OrderBy(en => en.OrderNo).ToList();

            var pos = curCategories.FindIndex(en => en.Id == command.CategoryId);

            if (pos >= curCategories.Count - 1)
                return SuccessResult<bool>.Create(true);
            
            var tempResult = curCategories[pos].ChangeOrderNo(pos + 1, currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult;

            tempResult = curCategories[pos + 1].ChangeOrderNo(pos, currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult;

            await dishCategoryRepository.StoreAsync(curCategories[pos], cancellationToken);
            await dishCategoryRepository.StoreAsync(curCategories[pos + 1], cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
