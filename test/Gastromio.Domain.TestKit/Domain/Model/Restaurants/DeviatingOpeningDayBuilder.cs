using System.Collections.Generic;
using System.Linq;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Restaurants
{
    public class DeviatingOpeningDayBuilder : TestObjectBuilderBase<DeviatingOpeningDay>
    {
        public DeviatingOpeningDayBuilder WithDate(Date date)
        {
            WithConstantConstructorArgumentFor("date", date);
            return this;
        }

        public DeviatingOpeningDayBuilder WithStatus(DeviatingOpeningDayStatus status)
        {
            WithConstantConstructorArgumentFor("status", status);
            return this;
        }

        public DeviatingOpeningDayBuilder WithoutOpeningPeriods()
        {
            return WithOpeningPeriods(Enumerable.Empty<OpeningPeriod>());
        }

        public DeviatingOpeningDayBuilder WithOpeningPeriods(IEnumerable<OpeningPeriod> openingPeriods)
        {
            WithConstantConstructorArgumentFor("openingPeriods", openingPeriods);
            return this;
        }
    }
}
