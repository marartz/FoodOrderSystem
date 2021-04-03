using System;
using System.Collections.Generic;
using System.Linq;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Restaurants
{
    public class RegularOpeningDayBuilder : TestObjectBuilderBase<RegularOpeningDay>
    {
        public RegularOpeningDayBuilder WithDayOfWeek(int dayOfWeek)
        {
            WithConstantConstructorArgumentFor("dayOfWeek", dayOfWeek);
            return this;
        }

        public RegularOpeningDayBuilder WithoutOpeningPeriods()
        {
            return WithOpeningPeriods(Enumerable.Empty<OpeningPeriod>());
        }

        public RegularOpeningDayBuilder WithOpeningPeriods(IEnumerable<OpeningPeriod> openingPeriods)
        {
            WithConstantConstructorArgumentFor("openingPeriods", openingPeriods);
            return this;
        }

    }
}
