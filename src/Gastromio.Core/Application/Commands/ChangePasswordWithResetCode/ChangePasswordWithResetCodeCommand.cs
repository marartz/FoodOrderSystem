using System;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.ChangePasswordWithResetCode
{
    public class ChangePasswordWithResetCodeCommand : ICommand<bool>
    {
        public ChangePasswordWithResetCodeCommand(UserId userId, byte[] passwordResetCode, string password)
        {
            UserId = userId;
            PasswordResetCode = passwordResetCode;
            Password = password;
        }
        
        public UserId UserId { get; }
        public byte[] PasswordResetCode { get; }
        public string Password { get; }
    }
}
