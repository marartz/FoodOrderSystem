using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.SetImportIdOfRestaurant
{
    public class SetImportIdOfRestaurantCommand : ICommand<bool>
    {
        public SetImportIdOfRestaurantCommand(RestaurantId restaurantId, string importId)
        {
            RestaurantId = restaurantId;
            ImportId = importId;
        }

        public RestaurantId RestaurantId { get; }
        public string ImportId { get; }
    }
}
