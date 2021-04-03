using System;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Restaurants
{
    public class ExternalMenuBuilder : TestObjectBuilderBase<ExternalMenu>
    {
        public ExternalMenuBuilder WithId(Guid id)
        {
            WithConstantConstructorArgumentFor("id", id);
            return this;
        }

        public ExternalMenuBuilder WithName(string name)
        {
            WithConstantConstructorArgumentFor("name", name);
            return this;
        }

        public ExternalMenuBuilder WithDescription(string description)
        {
            WithConstantConstructorArgumentFor("description", description);
            return this;
        }

        public ExternalMenuBuilder WithUrl(string url)
        {
            WithConstantConstructorArgumentFor("url", url);
            return this;
        }

    }
}
