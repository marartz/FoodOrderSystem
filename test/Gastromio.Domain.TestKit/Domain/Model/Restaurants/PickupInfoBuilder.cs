using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Restaurants
{
    public class PickupInfoBuilder : TestObjectBuilderBase<PickupInfo>
    {
        public PickupInfoBuilder WithValidConstrains()
        {
            WithAverageTime(15);
            WithMinimumOrderValue(1.23m);
            WithMaximumOrderValue(123.45m);
            return this;
        }

        public PickupInfoBuilder WithEnabled(bool enabled)
        {
            WithConstantConstructorArgumentFor("enabled", enabled);
            return this;
        }

        public PickupInfoBuilder WithAverageTime(int? averageTime)
        {
            WithConstantConstructorArgumentFor("averageTime", averageTime);
            return this;
        }

        public PickupInfoBuilder WithMinimumOrderValue(decimal? minimumOrderValue)
        {
            WithConstantConstructorArgumentFor("minimumOrderValue", minimumOrderValue);
            return this;
        }

        public PickupInfoBuilder WithMaximumOrderValue(decimal? maximumOrderValue)
        {
            WithConstantConstructorArgumentFor("maximumOrderValue", maximumOrderValue);
            return this;
        }

    }
}
