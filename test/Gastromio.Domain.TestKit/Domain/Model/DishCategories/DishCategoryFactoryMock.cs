using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Domain.Model.DishCategories
{
    public class DishCategoryFactoryMock : Mock<IDishCategoryFactory>
    {
        public DishCategoryFactoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IDishCategoryFactory, Result<DishCategory>> SetupCreate(RestaurantId restaurantId, string name,
            int orderNo, UserId createdBy)
        {
            return Setup(m => m.Create(restaurantId, name, orderNo, createdBy));
        }
    }
}
