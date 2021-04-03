using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Queries
{
    public interface IQueryDispatcher
    {
        Task<Result<TResult>> PostAsync<TQuery, TResult>(TQuery query, UserId currentUserId, CancellationToken cancellationToken = default) where TQuery : IQuery<TResult>;
    }
}
