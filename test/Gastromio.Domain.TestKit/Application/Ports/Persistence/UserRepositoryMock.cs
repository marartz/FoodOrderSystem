using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Domain.Model.Users;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Application.Ports.Persistence
{
    public class UserRepositoryMock : Mock<IUserRepository>
    {
        public UserRepositoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IUserRepository, Task<IEnumerable<User>>> SetupFindAllAsync()
        {
            return Setup(m => m.FindAllAsync(It.IsAny<CancellationToken>()));
        }

        public ISetup<IUserRepository, Task<IEnumerable<User>>> SetupSearchAsync(string searchPhrase)
        {
            return Setup(m => m.SearchAsync(searchPhrase, It.IsAny<CancellationToken>()));
        }

        public ISetup<IUserRepository, Task<(long total, IEnumerable<User> items)>> SetupSearchPagedAsync(
            string searchPhrase, Role? role, int skip = 0, int take = -1)
        {
            return Setup(m => m.SearchPagedAsync(searchPhrase, role, skip, take, It.IsAny<CancellationToken>()));
        }

        public ISetup<IUserRepository, Task<IEnumerable<User>>> SetupFindByRoleAsync(Role role)
        {
            return Setup(m => m.FindByRoleAsync(role, It.IsAny<CancellationToken>()));
        }

        public ISetup<IUserRepository, Task<User>> SetupFindByEmailAsync(string email)
        {
            return Setup(m => m.FindByEmailAsync(email, It.IsAny<CancellationToken>()));
        }

        public ISetup<IUserRepository, Task<User>> SetupFindByUserIdAsync(UserId userId)
        {
            return Setup(m => m.FindByUserIdAsync(userId, It.IsAny<CancellationToken>()));
        }

        public ISetup<IUserRepository, Task<IEnumerable<User>>> SetupFindByUserIdsAsync(IEnumerable<UserId> userIds)
        {
            return Setup(m => m.FindByUserIdsAsync(userIds, It.IsAny<CancellationToken>()));
        }

        public ISetup<IUserRepository, Task> SetupStoreAsync(User user)
        {
            return Setup(m => m.StoreAsync(user, It.IsAny<CancellationToken>()));
        }

        public void VerifyStoreAsync(User user, Func<Times> times)
        {
            Verify(m => m.StoreAsync(user, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IUserRepository, Task> SetupRemoveAsync(UserId userId)
        {
            return Setup(m => m.RemoveAsync(userId, It.IsAny<CancellationToken>()));
        }

        public void VerifyRemoveAsync(UserId userId, Func<Times> times)
        {
            Verify(m => m.RemoveAsync(userId, It.IsAny<CancellationToken>()), times);
        }
    }
}
