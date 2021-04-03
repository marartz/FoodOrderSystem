using System;
using System.Collections.Generic;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Orders;

namespace Gastromio.Core.Application.Queries.OrderSearchForRestaurants
{
    public class OrderSearchForRestaurantsQuery : IQuery<ICollection<RestaurantDTO>>
    {
        public OrderSearchForRestaurantsQuery(string searchPhrase, OrderType? orderType, CuisineId cuisineId, DateTimeOffset? openingHour)
        {
            SearchPhrase = searchPhrase;
            OrderType = orderType;
            CuisineId = cuisineId;
            OpeningHour = openingHour;
        }

        public string SearchPhrase { get; }
        public OrderType? OrderType { get; }
        public CuisineId CuisineId { get; }
        public DateTimeOffset? OpeningHour { get; }
    }
}
