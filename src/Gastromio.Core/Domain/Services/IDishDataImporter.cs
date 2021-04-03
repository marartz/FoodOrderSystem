using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Services
{
    public interface IDishDataImporter
    {
        Task ImportDishAsync(ImportLog log, int rowIndex, DishRow dishRow,
            UserId curUserId, bool dryRun, CancellationToken cancellationToken = default);
    }
}
