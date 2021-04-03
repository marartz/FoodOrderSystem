using System;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.RemoveExternalMenuFromRestaurant
{
    public class RemoveExternalMenuFromRestaurantCommand : ICommand<bool>
    {
        public RemoveExternalMenuFromRestaurantCommand(RestaurantId restaurantId, Guid externalMenuId)
        {
            RestaurantId = restaurantId;
            ExternalMenuId = externalMenuId;
        }
        
        public RestaurantId RestaurantId { get; }

        public Guid ExternalMenuId { get; }
    }
}
