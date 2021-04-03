using System.Collections.Generic;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Domain.Model.Dishes
{
    public class DishFactoryMock : Mock<IDishFactory>
    {
        public DishFactoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IDishFactory, Result<Dish>> SetupCreate(RestaurantId restaurantId, DishCategoryId categoryId,
            string name, string description, string productInfo, int orderNo, IEnumerable<DishVariant> variants,
            UserId createdBy)
        {
            return Setup(m => m.Create(
                restaurantId,
                categoryId,
                name,
                description,
                productInfo,
                orderNo,
                variants,
                createdBy
            ));
        }
    }
}
