using Gastromio.Core.Domain.Model.RestaurantImages;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Queries.GetRestaurantImage
{
    public class GetRestaurantImageQuery : IQuery<RestaurantImage>
    {
        public GetRestaurantImageQuery(RestaurantId restaurantId, string type)
        {
            RestaurantId = restaurantId;
            Type = type;
        }

        public RestaurantId RestaurantId { get; }

        public string Type { get; }
    }
}
