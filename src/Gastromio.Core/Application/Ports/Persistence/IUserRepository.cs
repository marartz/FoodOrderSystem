using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Ports.Persistence
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> FindAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<User>> SearchAsync(string searchPhrase, CancellationToken cancellationToken = default);

        Task<(long total, IEnumerable<User> items)> SearchPagedAsync(string searchPhrase, Role? role, int skip = 0,
            int take = -1, CancellationToken cancellationToken = default);

        Task<IEnumerable<User>> FindByRoleAsync(Role role, CancellationToken cancellationToken = default);

        Task<User> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<User> FindByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<User>> FindByUserIdsAsync(IEnumerable<UserId> userIds, CancellationToken cancellationToken = default);

        Task StoreAsync(User user, CancellationToken cancellationToken = default);

        Task RemoveAsync(UserId userId, CancellationToken cancellationToken = default);
    }
}
