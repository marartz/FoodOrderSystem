using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.ChangeCuisine
{
    public class ChangeCuisineCommandHandler : ICommandHandler<ChangeCuisineCommand, bool>
    {
        private readonly ICuisineRepository cuisineRepository;

        public ChangeCuisineCommandHandler(ICuisineRepository cuisineRepository)
        {
            this.cuisineRepository = cuisineRepository;
        }

        public async Task<Result<bool>> HandleAsync(ChangeCuisineCommand command, User currentUser, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<bool>.Unauthorized();

            if (currentUser.Role < Role.SystemAdmin)
                return FailureResult<bool>.Forbidden();

            var cuisine = await cuisineRepository.FindByCuisineIdAsync(command.CuisineId, cancellationToken);
            if (cuisine == null)
                return FailureResult<bool>.Create(FailureResultCode.CuisineDoesNotExist);

            var result = cuisine.ChangeName(command.Name, currentUser.Id);
            if (result.IsFailure)
                return result;

            await cuisineRepository.StoreAsync(cuisine, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
