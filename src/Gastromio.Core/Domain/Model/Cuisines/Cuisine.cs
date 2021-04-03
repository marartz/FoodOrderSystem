using System;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Model.Cuisines
{
    public class Cuisine
    {
        public Cuisine(
            CuisineId id,
            string name,
            DateTimeOffset createdOn,
            UserId createdBy,
            DateTimeOffset updatedOn,
            UserId updatedBy
        )
        {
            Id = id;
            Name = name;
            CreatedOn = createdOn;
            CreatedBy = createdBy;
            UpdatedOn = updatedOn;
            UpdatedBy = updatedBy;
        }

        public CuisineId Id { get; }
        public string Name { get; private set; }

        public DateTimeOffset CreatedOn { get; }

        public UserId CreatedBy { get; }

        public DateTimeOffset UpdatedOn { get; private set; }

        public UserId UpdatedBy { get; private set; }

        public Result<bool> ChangeName(string name, UserId updatedBy)
        {
            if (string.IsNullOrEmpty(name))
                return FailureResult<bool>.Create(FailureResultCode.CuisineNameIsRequired);
            if (name.Length > 100)
                return FailureResult<bool>.Create(FailureResultCode.CuisineNameTooLong, 100);

            Name = name;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = updatedBy;

            return SuccessResult<bool>.Create(true);
        }
    }
}
