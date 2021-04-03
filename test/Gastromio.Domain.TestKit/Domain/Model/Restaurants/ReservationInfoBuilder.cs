using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Restaurants
{
    public class ReservationInfoBuilder : TestObjectBuilderBase<ReservationInfo>
    {
        public ReservationInfoBuilder WithEnabled(bool enabled)
        {
            WithConstantConstructorArgumentFor("enabled", enabled);
            return this;
        }
    }
}
