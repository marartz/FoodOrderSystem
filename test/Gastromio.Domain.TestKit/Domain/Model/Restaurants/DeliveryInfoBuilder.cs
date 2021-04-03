using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Restaurants
{
    public class DeliveryInfoBuilder : TestObjectBuilderBase<DeliveryInfo>
    {
        public DeliveryInfoBuilder WithValidConstrains()
        {
            WithAverageTime(15);
            WithMinimumOrderValue(1.23m);
            WithMaximumOrderValue(123.45m);
            WithCosts(3.45m);
            return this;
        }

        public DeliveryInfoBuilder WithEnabled(bool enabled)
        {
            WithConstantConstructorArgumentFor("enabled", enabled);
            return this;
        }

        public DeliveryInfoBuilder WithAverageTime(int? averageTime)
        {
            WithConstantConstructorArgumentFor("averageTime", averageTime);
            return this;
        }

        public DeliveryInfoBuilder WithMinimumOrderValue(decimal? minimumOrderValue)
        {
            WithConstantConstructorArgumentFor("minimumOrderValue", minimumOrderValue);
            return this;
        }

        public DeliveryInfoBuilder WithMaximumOrderValue(decimal? maximumOrderValue)
        {
            WithConstantConstructorArgumentFor("maximumOrderValue", maximumOrderValue);
            return this;
        }

        public DeliveryInfoBuilder WithCosts(decimal? costs)
        {
            WithConstantConstructorArgumentFor("costs", costs);
            return this;
        }
    }
}
