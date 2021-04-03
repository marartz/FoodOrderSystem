using System.Collections.Generic;

namespace Gastromio.Core.Domain.Model.Restaurants
{
    public class RegularOpeningDay : OpeningDay<RegularOpeningDay>
    {
        public RegularOpeningDay(int dayOfWeek, IEnumerable<OpeningPeriod> openingPeriods) : base(openingPeriods)
        {
            DayOfWeek = dayOfWeek;
        }

        public int DayOfWeek { get; }

        protected override RegularOpeningDay CreateNewInstance(IEnumerable<OpeningPeriod> openingPeriods)
        {
            return new RegularOpeningDay(DayOfWeek, openingPeriods);
        }
    }
}
