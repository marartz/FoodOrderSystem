using System;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.AddDeviatingOpeningDayToRestaurant
{
    public class AddDeviatingOpeningDayToRestaurantCommand : ICommand<bool>
    {
        public AddDeviatingOpeningDayToRestaurantCommand(RestaurantId restaurantId, Date date, DeviatingOpeningDayStatus status)
        {
            RestaurantId = restaurantId;
            Date = date;
            Status = status;
        }
        
        public RestaurantId RestaurantId { get; }
        public Date Date { get; }
        public DeviatingOpeningDayStatus Status { get; }
    }
}
