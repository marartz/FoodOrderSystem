using System;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Users
{
    public class UserBuilder : TestObjectBuilderBase<User>
    {
        public UserBuilder WithId(UserId id)
        {
            WithConstantConstructorArgumentFor("id", id);
            return this;
        }

        public UserBuilder WithRole(Role role)
        {
            WithConstantConstructorArgumentFor("role", role);
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            WithConstantConstructorArgumentFor("email", email);
            return this;
        }

        public UserBuilder WithPasswordSalt(byte[] passwordSalt)
        {
            WithConstantConstructorArgumentFor("passwordSalt", passwordSalt);
            return this;
        }

        public UserBuilder WithPasswordHash(byte[] passwordHash)
        {
            WithConstantConstructorArgumentFor("passwordHash", passwordHash);
            return this;
        }

        public UserBuilder WithPasswordResetCode(byte[] passwordResetCode)
        {
            WithConstantConstructorArgumentFor("passwordResetCode", passwordResetCode);
            return this;
        }

        public UserBuilder WithPasswordResetExpiration(DateTimeOffset? passwordResetExpiration)
        {
            WithConstantConstructorArgumentFor("passwordResetExpiration", passwordResetExpiration);
            return this;
        }

        public UserBuilder WithCreatedOn(DateTimeOffset createdOn)
        {
            WithConstantConstructorArgumentFor("createdOn", createdOn);
            return this;
        }

        public UserBuilder WithCreatedBy(UserId createdBy)
        {
            WithConstantConstructorArgumentFor("createdBy", createdBy);
            return this;
        }

        public UserBuilder WithUpdatedOn(DateTimeOffset updatedOn)
        {
            WithConstantConstructorArgumentFor("updatedOn", updatedOn);
            return this;
        }

        public UserBuilder WithUpdatedBy(UserId updatedBy)
        {
            WithConstantConstructorArgumentFor("updatedBy", updatedBy);
            return this;
        }

    }
}
