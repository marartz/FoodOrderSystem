using System;
using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Model.RestaurantImages
{
    public class RestaurantImageId : ValueType<Guid>
    {
        public RestaurantImageId(Guid value) : base(value)
        {
        }
    }
}
