using System;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.AddRegularOpeningPeriodToRestaurant
{
    public class AddRegularOpeningPeriodToRestaurantCommand : ICommand<bool>
    {
        public AddRegularOpeningPeriodToRestaurantCommand(RestaurantId restaurantId, int dayOfWeek, TimeSpan start, TimeSpan end)
        {
            RestaurantId = restaurantId;
            DayOfWeek = dayOfWeek;
            Start = start;
            End = end;
        }

        public RestaurantId RestaurantId { get; }

        public int DayOfWeek { get; }

        public TimeSpan Start { get; }
        
        public TimeSpan End { get; }
    }
}
