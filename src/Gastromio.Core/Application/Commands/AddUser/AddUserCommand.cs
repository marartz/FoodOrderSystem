using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.AddUser
{
    public class AddUserCommand : ICommand<UserDTO>
    {
        public AddUserCommand(Role role, string email, string password)
        {
            Role = role;
            Email = email;
            Password = password;
        }

        public Role Role { get; }
        public string Email { get; }
        public string Password { get; }
    }
}
