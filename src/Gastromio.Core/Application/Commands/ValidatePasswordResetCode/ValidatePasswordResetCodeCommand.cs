using System;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.ValidatePasswordResetCode
{
    public class ValidatePasswordResetCodeCommand : ICommand<bool>
    {
        public ValidatePasswordResetCodeCommand(UserId userId, byte[] passwordResetCode)
        {
            UserId = userId;
            PasswordResetCode = passwordResetCode;
        }

        public UserId UserId { get; }
        public byte[] PasswordResetCode { get; }
    }
}
