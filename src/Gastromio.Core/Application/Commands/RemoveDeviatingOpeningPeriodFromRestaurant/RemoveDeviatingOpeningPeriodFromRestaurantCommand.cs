using System;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.RemoveDeviatingOpeningPeriodFromRestaurant
{
    public class RemoveDeviatingOpeningPeriodFromRestaurantCommand : ICommand<bool>
    {
        public RemoveDeviatingOpeningPeriodFromRestaurantCommand(RestaurantId restaurantId, Date date, TimeSpan start)
        {
            RestaurantId = restaurantId;
            Date = date;
            Start = start;
        }

        public RestaurantId RestaurantId { get; }
        public Date Date { get; }
        public TimeSpan Start { get; }
    }
}
