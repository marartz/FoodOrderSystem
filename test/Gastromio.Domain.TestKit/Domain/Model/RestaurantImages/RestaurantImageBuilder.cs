using System;
using Gastromio.Core.Domain.Model.RestaurantImages;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.RestaurantImages
{
    public class RestaurantImageBuilder : TestObjectBuilderBase<RestaurantImage>
    {
        public RestaurantImageBuilder WithId(RestaurantImageId id)
        {
            WithConstantConstructorArgumentFor("id", id);
            return this;
        }

        public RestaurantImageBuilder WithRestaurantId(RestaurantId restaurantId)
        {
            WithConstantConstructorArgumentFor("restaurantId", restaurantId);
            return this;
        }

        public RestaurantImageBuilder WithType(string type)
        {
            WithConstantConstructorArgumentFor("type", type);
            return this;
        }

        public RestaurantImageBuilder WithData(byte[] data)
        {
            WithConstantConstructorArgumentFor("data", data);
            return this;
        }

        public RestaurantImageBuilder WithCreatedOn(DateTimeOffset createdOn)
        {
            WithConstantConstructorArgumentFor("createdOn", createdOn);
            return this;
        }

        public RestaurantImageBuilder WithCreatedBy(UserId createdBy)
        {
            WithConstantConstructorArgumentFor("createdBy", createdBy);
            return this;
        }

        public RestaurantImageBuilder WithUpdatedOn(DateTimeOffset updatedOn)
        {
            WithConstantConstructorArgumentFor("updatedOn", updatedOn);
            return this;
        }

        public RestaurantImageBuilder WithUpdatedBy(UserId updatedBy)
        {
            WithConstantConstructorArgumentFor("updatedBy", updatedBy);
            return this;
        }
    }
}
