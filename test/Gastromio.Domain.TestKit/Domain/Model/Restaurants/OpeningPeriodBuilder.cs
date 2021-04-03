using System;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Restaurants
{
    public class OpeningPeriodBuilder : TestObjectBuilderBase<OpeningPeriod>
    {
        public OpeningPeriodBuilder WithValidConstrains()
        {
            WithStart(TimeSpan.FromHours(16.5));
            WithEnd(TimeSpan.FromHours(22.5));
            return this;
        }

        public OpeningPeriodBuilder WithStart(TimeSpan start)
        {
            WithConstantConstructorArgumentFor("start", start);
            return this;
        }

        public OpeningPeriodBuilder WithEnd(TimeSpan end)
        {
            WithConstantConstructorArgumentFor("end", end);
            return this;
        }

    }
}
