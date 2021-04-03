using System;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Model.RestaurantImages
{
    public class RestaurantImage
    {
        public RestaurantImage(
            RestaurantImageId id,
            RestaurantId restaurantId,
            string type,
            byte[] data,
            DateTimeOffset createdOn,
            UserId createdBy,
            DateTimeOffset updatedOn,
            UserId updatedBy
        )
        {
            Id = id;
            RestaurantId = restaurantId;
            Type = type;
            Data = data;
            CreatedOn = createdOn;
            CreatedBy = createdBy;
            UpdatedOn = updatedOn;
            UpdatedBy = updatedBy;
        }

        public RestaurantImageId Id { get; }

        public RestaurantId RestaurantId { get; }

        public string Type { get; }

        public byte[] Data { get; }

        public DateTimeOffset CreatedOn { get; }

        public UserId CreatedBy { get; }

        public DateTimeOffset UpdatedOn { get; }

        public UserId UpdatedBy { get; }
    }
}
