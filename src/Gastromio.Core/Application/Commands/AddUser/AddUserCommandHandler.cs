using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.AddUser
{
    public class AddUserCommandHandler : ICommandHandler<AddUserCommand, UserDTO>
    {
        private readonly IUserFactory userFactory;
        private readonly IUserRepository userRepository;

        public AddUserCommandHandler(IUserFactory userFactory, IUserRepository userRepository)
        {
            this.userFactory = userFactory;
            this.userRepository = userRepository;
        }

        public async Task<Result<UserDTO>> HandleAsync(AddUserCommand command, User currentUser, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<UserDTO>.Unauthorized();

            if (currentUser.Role < Role.SystemAdmin)
                return FailureResult<UserDTO>.Forbidden();

            var user = await userRepository.FindByEmailAsync(command.Email, cancellationToken);
            if (user != null)
                return FailureResult<UserDTO>.Create(FailureResultCode.UserAlreadyExists);

            var createResult = userFactory.Create(
                command.Role,
                command.Email,
                command.Password,
                true,
                currentUser.Id
            );
            
            if (createResult.IsFailure)
                return createResult.Cast<UserDTO>();

            user = createResult.Value;

            await userRepository.StoreAsync(user, cancellationToken);

            return SuccessResult<UserDTO>.Create(new UserDTO(user));
        }
    }
}
