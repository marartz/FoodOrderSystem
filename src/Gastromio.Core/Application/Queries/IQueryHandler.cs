using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Queries
{
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<Result<TResult>> HandleAsync(TQuery query, User currentUser, CancellationToken cancellationToken = default);
    }
}
