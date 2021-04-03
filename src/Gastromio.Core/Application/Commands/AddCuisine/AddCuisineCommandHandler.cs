using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Users;


namespace Gastromio.Core.Application.Commands.AddCuisine
{
    public class AddCuisineCommandHandler : ICommandHandler<AddCuisineCommand, CuisineDTO>
    {
        private readonly ICuisineFactory cuisineFactory;
        private readonly ICuisineRepository cuisineRepository;

        public AddCuisineCommandHandler(ICuisineFactory cuisineFactory,
            ICuisineRepository cuisineRepository)
        {
            this.cuisineFactory = cuisineFactory;
            this.cuisineRepository = cuisineRepository;
        }

        public async Task<Result<CuisineDTO>> HandleAsync(AddCuisineCommand command, User currentUser,
            CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<CuisineDTO>.Unauthorized();

            if (currentUser.Role < Role.SystemAdmin)
                return FailureResult<CuisineDTO>.Forbidden();

            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return FailureResult<CuisineDTO>.Create(FailureResultCode.CuisineNameIsRequired);
            }

            var cuisine = await cuisineRepository.FindByNameAsync(command.Name, cancellationToken);
            if (cuisine != null)
                return FailureResult<CuisineDTO>.Create(FailureResultCode.CuisineAlreadyExists);

            var createResult = cuisineFactory.Create(command.Name, currentUser.Id);
            if (createResult.IsFailure)
                return createResult.Cast<CuisineDTO>();

            cuisine = createResult.Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            
            return SuccessResult<CuisineDTO>.Create(new CuisineDTO(cuisine));
        }
    }
}
