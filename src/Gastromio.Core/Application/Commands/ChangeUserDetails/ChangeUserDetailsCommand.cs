using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.ChangeUserDetails
{
    public class ChangeUserDetailsCommand : ICommand<bool>
    {
        public ChangeUserDetailsCommand(UserId userId, Role role, string email)
        {
            UserId = userId;
            Role = role;
            Email = email;
        }

        public UserId UserId { get; }
        public Role Role { get; }
        public string Email { get; }
    }
}
