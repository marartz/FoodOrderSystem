using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Commands.RemoveAdminFromRestaurant
{
    public class RemoveAdminFromRestaurantCommand : ICommand<bool>
    {
        public RemoveAdminFromRestaurantCommand(RestaurantId restaurantId, UserId userId)
        {
            RestaurantId = restaurantId;
            UserId = userId;
        }

        public RestaurantId RestaurantId { get; }
        public UserId UserId { get; }
    }
}
