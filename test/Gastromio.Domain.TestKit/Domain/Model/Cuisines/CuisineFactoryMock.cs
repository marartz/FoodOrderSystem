using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Users;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Domain.Model.Cuisines
{
    public class CuisineFactoryMock : Mock<ICuisineFactory>
    {
        public CuisineFactoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<ICuisineFactory, Result<Cuisine>> SetupCreate(string name, UserId createdBy)
        {
            return Setup(m => m.Create(name, createdBy));
        }
    }
}
