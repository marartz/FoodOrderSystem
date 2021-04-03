using System;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Cuisines
{
    public class CuisineBuilder : TestObjectBuilderBase<Cuisine>
    {
        public CuisineBuilder WithValidConstrains()
        {
            WithName("cuisine-name");
            return this;
        }

        public CuisineBuilder WithId(CuisineId id)
        {
            WithConstantConstructorArgumentFor("id", id);
            return this;
        }

        public CuisineBuilder WithName(string name)
        {
            WithConstantConstructorArgumentFor("name", name);
            return this;
        }

        public CuisineBuilder WithCreatedOn(DateTimeOffset createdOn)
        {
            WithConstantConstructorArgumentFor("createdOn", createdOn);
            return this;
        }

        public CuisineBuilder WithCreatedBy(UserId createdBy)
        {
            WithConstantConstructorArgumentFor("createdBy", createdBy);
            return this;
        }

        public CuisineBuilder WithUpdatedOn(DateTimeOffset updatedOn)
        {
            WithConstantConstructorArgumentFor("updatedOn", updatedOn);
            return this;
        }

        public CuisineBuilder WithUpdatedBy(UserId updatedBy)
        {
            WithConstantConstructorArgumentFor("updatedBy", updatedBy);
            return this;
        }
    }
}
