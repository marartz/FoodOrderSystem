using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Services
{
    public interface IRestaurantDataImporter
    {
        Task ImportRestaurantAsync(ImportLog log, int rowIndex, RestaurantRow restaurantRow,
            UserId curUserId, bool dryRun, CancellationToken cancellationToken = default);
    }
}
