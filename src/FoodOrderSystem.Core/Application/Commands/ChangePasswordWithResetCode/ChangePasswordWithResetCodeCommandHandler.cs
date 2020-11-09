using System;
using System.Threading;
using System.Threading.Tasks;
using FoodOrderSystem.Core.Application.Ports.Persistence;
using FoodOrderSystem.Core.Common;
using FoodOrderSystem.Core.Domain.Model.User;

namespace FoodOrderSystem.Core.Application.Commands.ChangePasswordWithResetCode
{
    public class ChangePasswordWithResetCodeCommandHandler : ICommandHandler<ChangePasswordWithResetCodeCommand, bool>
    {
        private readonly IUserRepository userRepository;

        public ChangePasswordWithResetCodeCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        
        public async Task<Result<bool>> HandleAsync(ChangePasswordWithResetCodeCommand command, User currentUser,
            CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var user = await userRepository.FindByUserIdAsync(command.UserId, cancellationToken);
            
            if (user == null)
                return FailureResult<bool>.Create(FailureResultCode.PasswordResetCodeIsInvalid);

            var result = user.ChangePasswordWithResetCode(command.PasswordResetCode, command.Password);
            if (result.IsFailure)
                return result;

            await userRepository.StoreAsync(user, cancellationToken);
            return result;
        }
    }
}