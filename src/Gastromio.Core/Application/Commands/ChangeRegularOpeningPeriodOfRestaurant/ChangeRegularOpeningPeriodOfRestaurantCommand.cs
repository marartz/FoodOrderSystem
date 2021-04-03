using System;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.ChangeRegularOpeningPeriodOfRestaurant
{
    public class ChangeRegularOpeningPeriodOfRestaurantCommand : ICommand<bool>
    {
        public ChangeRegularOpeningPeriodOfRestaurantCommand(RestaurantId restaurantId, int dayOfWeek, TimeSpan oldStart, TimeSpan newStart, TimeSpan newEnd)
        {
            RestaurantId = restaurantId;
            DayOfWeek = dayOfWeek;
            OldStart = oldStart;
            NewStart = newStart;
            NewEnd = newEnd;
        }

        public RestaurantId RestaurantId { get; }

        public int DayOfWeek { get; }

        public TimeSpan OldStart { get; }

        public TimeSpan NewStart { get; }

        public TimeSpan NewEnd { get; }
    }
}
