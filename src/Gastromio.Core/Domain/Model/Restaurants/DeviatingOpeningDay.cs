using System.Collections.Generic;
using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Model.Restaurants
{
    public class DeviatingOpeningDay : OpeningDay<DeviatingOpeningDay>
    {
        public DeviatingOpeningDay(Date date, DeviatingOpeningDayStatus status, IEnumerable<OpeningPeriod> openingPeriods) : base(openingPeriods)
        {
            Date = date;
            Status = status;
        }

        public Date Date { get; }

        public DeviatingOpeningDayStatus Status { get; }

        protected override DeviatingOpeningDay CreateNewInstance(IEnumerable<OpeningPeriod> openingPeriods)
        {
            return new DeviatingOpeningDay(Date, Status, openingPeriods);
        }
    }
}
