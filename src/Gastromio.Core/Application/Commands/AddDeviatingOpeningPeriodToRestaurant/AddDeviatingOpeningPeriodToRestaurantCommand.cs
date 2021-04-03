using System;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.AddDeviatingOpeningPeriodToRestaurant
{
    public class AddDeviatingOpeningPeriodToRestaurantCommand : ICommand<bool>
    {
        public AddDeviatingOpeningPeriodToRestaurantCommand(RestaurantId restaurantId, Date date, TimeSpan start, TimeSpan end)
        {
            RestaurantId = restaurantId;
            Date = date;
            Start = start;
            End = end;
        }
        
        public RestaurantId RestaurantId { get; }
        public Date Date { get; }
        public TimeSpan Start { get; }
        public TimeSpan End { get; }
    }
}
