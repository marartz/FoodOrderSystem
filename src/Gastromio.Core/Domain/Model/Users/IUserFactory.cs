using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Model.Users
{
    public interface IUserFactory
    {
        Result<User> Create(
            Role role,
            string email,
            string password,
            bool checkPasswordPolicy,
            UserId createdBy
        );
    }
}
