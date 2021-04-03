using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.ChangeUserDetails
{
    public class ChangeUserDetailsCommandHandler : ICommandHandler<ChangeUserDetailsCommand, bool>
    {
        private readonly IUserRepository userRepository;

        public ChangeUserDetailsCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Result<bool>> HandleAsync(ChangeUserDetailsCommand command, User currentUser, CancellationToken cancellationToken = default)
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

            user.ChangeDetails(command.Role, command.Email, currentUser.Id);

            await userRepository.StoreAsync(user, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
