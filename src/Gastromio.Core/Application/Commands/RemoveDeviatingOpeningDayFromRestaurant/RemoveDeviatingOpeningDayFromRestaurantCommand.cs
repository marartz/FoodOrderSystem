using System;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.RemoveDeviatingOpeningDayFromRestaurant
{
    public class RemoveDeviatingOpeningDayFromRestaurantCommand : ICommand<bool>
    {
        public RemoveDeviatingOpeningDayFromRestaurantCommand(RestaurantId restaurantId, Date date)
        {
            RestaurantId = restaurantId;
            Date = date;
        }

        public RestaurantId RestaurantId { get; }
        public Date Date { get; }
    }
}
