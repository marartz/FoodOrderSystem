using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.ChangeRestaurantName
{
    public class ChangeRestaurantNameCommand : ICommand<bool>
    {
        public ChangeRestaurantNameCommand(RestaurantId restaurantId, string name)
        {
            RestaurantId = restaurantId;
            Name = name;
        }

        public RestaurantId RestaurantId { get; }
        public string Name { get; }
    }
}
