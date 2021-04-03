using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, UserDTO>
    {
        private readonly IUserRepository userRepository;

        public LoginCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Result<UserDTO>> HandleAsync(LoginCommand command, User currentUser, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (string.IsNullOrWhiteSpace(command.Email))
                return FailureResult<UserDTO>.Create(FailureResultCode.LoginEmailRequired);

            if (string.IsNullOrWhiteSpace(command.Password))
                return FailureResult<UserDTO>.Create(FailureResultCode.LoginPasswordRequired);

            var user = await userRepository.FindByEmailAsync(command.Email, cancellationToken);
            if (user == null)
                return FailureResult<UserDTO>.Unauthorized(FailureResultCode.WrongCredentials);

            var validationResult = user.ValidatePassword(command.Password);
            if (validationResult.IsFailure)
                return FailureResult<UserDTO>.Unauthorized(FailureResultCode.WrongCredentials);
            if (!validationResult.Value)
                return FailureResult<UserDTO>.Unauthorized(FailureResultCode.WrongCredentials);

            return SuccessResult<UserDTO>.Create(new UserDTO(user));
        }
    }
}
