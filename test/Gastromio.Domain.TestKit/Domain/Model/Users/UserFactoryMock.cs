using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Domain.Model.Users
{
    public class UserFactoryMock : Mock<IUserFactory>
    {
        public UserFactoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IUserFactory, Result<User>> SetupCreate(Role role, string email, string password,
            bool checkPasswordPolicy, UserId createdBy)
        {
            return Setup(m => m.Create(role, email, password, checkPasswordPolicy, createdBy));
        }
    }
}
