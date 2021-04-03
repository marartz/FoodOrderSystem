using System.Collections.Generic;
using System.Linq;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Orders
{
    public class CartInfoBuilder : TestObjectBuilderBase<CartInfo>
    {
        public CartInfoBuilder WithOrderType(OrderType orderType)
        {
            WithConstantConstructorArgumentFor("orderType", orderType);
            return this;
        }

        public CartInfoBuilder WithRestaurantId(RestaurantId restaurantId)
        {
            WithConstantConstructorArgumentFor("restaurantId", restaurantId);
            return this;
        }

        public CartInfoBuilder WithRestaurantName(string restaurantName)
        {
            WithConstantConstructorArgumentFor("restaurantName", restaurantName);
            return this;
        }

        public CartInfoBuilder WithRestaurantInfo(string restaurantInfo)
        {
            WithConstantConstructorArgumentFor("restaurantInfo", restaurantInfo);
            return this;
        }

        public CartInfoBuilder WithRestaurantPhone(string restaurantPhone)
        {
            WithConstantConstructorArgumentFor("restaurantPhone", restaurantPhone);
            return this;
        }

        public CartInfoBuilder WithRestaurantEmail(string restaurantEmail)
        {
            WithConstantConstructorArgumentFor("restaurantEmail", restaurantEmail);
            return this;
        }

        public CartInfoBuilder WithRestaurantMobile(string restaurantMobile)
        {
            WithConstantConstructorArgumentFor("restaurantMobile", restaurantMobile);
            return this;
        }

        public CartInfoBuilder WithoutOrderedDishes()
        {
            return WithOrderedDishes(Enumerable.Empty<OrderedDishInfo>());
        }

        public CartInfoBuilder WithOrderedDishes(IEnumerable<OrderedDishInfo> orderedDishes)
        {
            WithConstantConstructorArgumentFor("orderedDishes", orderedDishes);
            return this;
        }
    }
}
