﻿using System;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Failures;

namespace Gastromio.Core.Domain.Model.RestaurantImages
{
    public class RestaurantImageId : ValueType<Guid>
    {
        public RestaurantImageId(Guid value) : base(value)
        {
            if (value == Guid.Empty)
                throw DomainException.CreateFrom(new RestaurantImageIdIsInvalidFailure());
        }
    }
}
