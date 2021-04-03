using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.EnableDishCategory
{
    public class EnableDishCategoryCommandHandler : ICommandHandler<EnableDishCategoryCommand, bool>
    {
        private readonly IDishCategoryRepository dishCategoryRepository;

        public EnableDishCategoryCommandHandler(IDishCategoryRepository dishCategoryRepository)
        {
            this.dishCategoryRepository = dishCategoryRepository;
        }

        public async Task<Result<bool>> HandleAsync(EnableDishCategoryCommand command, User currentUser,
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

            curCategory.Enable(currentUser.Id);
            await dishCategoryRepository.StoreAsync(curCategory, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
