using System;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.DTOs
{
    public class UserDTO
    {
        public UserDTO(Guid id, string role, string email)
        {
            Id = id;
            Role = role;
            Email = email;
        }

        internal UserDTO(User user)
        {
            Id = user.Id.Value;
            Role = user.Role.ToString();
            Email = user.Email;
        }

        public Guid Id { get; }

        public string Role { get; }

        public string Email { get; }
    }
}
