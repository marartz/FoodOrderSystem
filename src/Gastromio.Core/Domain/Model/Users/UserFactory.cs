using System;
using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Model.Users
{
    public class UserFactory : IUserFactory
    {
        public Result<User> Create(
            Role role,
            string email,
            string password,
            bool checkPasswordPolicy,
            UserId createdBy
        )
        {
            var user = new User(
                new UserId(Guid.NewGuid()),
                Role.Customer,
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow,
                createdBy,
                DateTimeOffset.UtcNow,
                createdBy
            );

            var tempResult = user.ChangeDetails(role, email, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<User>();

            if (!string.IsNullOrEmpty(password))
            {
                tempResult = user.ChangePassword(password, checkPasswordPolicy, createdBy);
                if (tempResult.IsFailure)
                    return tempResult.Cast<User>();
            }

            return SuccessResult<User>.Create(user);
        }
    }
}
