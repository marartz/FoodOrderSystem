using System.Collections.Generic;
using System.Linq;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Domain.Model.Orders
{
    public class CartInfo
    {
        public CartInfo(
            OrderType orderType,
            RestaurantId restaurantId,
            string restaurantName,
            string restaurantInfo,
            string restaurantPhone,
            string restaurantEmail,
            string restaurantMobile,
            bool restaurantNeedsSupport,
            IEnumerable<OrderedDishInfo> orderedDishes
        )
        {
            OrderType = orderType;
            RestaurantId = restaurantId;
            RestaurantName = restaurantName;
            RestaurantInfo = restaurantInfo;
            RestaurantPhone = restaurantPhone;
            RestaurantEmail = restaurantEmail;
            RestaurantMobile = restaurantMobile;
            RestaurantNeedsSupport = restaurantNeedsSupport;
            OrderedDishes = orderedDishes.ToList();
        }

        public OrderType OrderType { get; }
        public RestaurantId RestaurantId { get; }
        public string RestaurantName { get; }
        public string RestaurantInfo { get; }
        public string RestaurantPhone { get; }
        public string RestaurantEmail { get; }
        public string RestaurantMobile { get; }
        public bool RestaurantNeedsSupport { get; }
        public IReadOnlyCollection<OrderedDishInfo> OrderedDishes { get; }
    }
}
