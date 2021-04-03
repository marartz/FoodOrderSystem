using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Model.Cuisines
{
    public interface ICuisineFactory
    {
        Result<Cuisine> Create(string name, UserId createdBy);
    }
}
