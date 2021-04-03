using System;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.RemoveRegularOpeningPeriodFromRestaurant
{
    public class RemoveRegularOpeningPeriodFromRestaurantCommand : ICommand<bool>
    {
        public RemoveRegularOpeningPeriodFromRestaurantCommand(RestaurantId restaurantId, int dayOfWeek, TimeSpan start)
        {
            RestaurantId = restaurantId;
            DayOfWeek = dayOfWeek;
            Start = start;
        }

        public RestaurantId RestaurantId { get; }
        public int DayOfWeek { get; }
        public TimeSpan Start { get; }
    }
}
