using System;
using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Model.Restaurants
{
    public class RestaurantId : ValueType<Guid>
    {
        public RestaurantId(Guid value) : base(value)
        {
        }
    }
}
