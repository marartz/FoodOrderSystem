using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands
{
    public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<Result<TResult>> HandleAsync(TCommand command, User currentUser, CancellationToken cancellationToken = default);
    }
}
