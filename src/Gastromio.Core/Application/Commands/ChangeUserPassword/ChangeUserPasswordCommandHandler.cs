using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommandHandler : ICommandHandler<ChangeUserPasswordCommand, bool>
    {
        private readonly IUserRepository userRepository;

        public ChangeUserPasswordCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Result<bool>> HandleAsync(ChangeUserPasswordCommand command, User currentUser, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<bool>.Unauthorized();

            if (currentUser.Role < Role.SystemAdmin)
                return FailureResult<bool>.Forbidden();

            var user = await userRepository.FindByUserIdAsync(command.UserId, cancellationToken);
            if (user == null)
                return FailureResult<bool>.Create(FailureResultCode.UserDoesNotExist);

            var result = user.ChangePassword(command.Password, true, currentUser.Id);
            if (result.IsFailure)
                return result;

            await userRepository.StoreAsync(user, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
