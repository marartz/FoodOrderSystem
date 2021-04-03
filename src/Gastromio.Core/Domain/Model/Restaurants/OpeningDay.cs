using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Model.Restaurants
{
    public abstract class OpeningDay<TSpecificOpeningDay>
    {
        private readonly List<OpeningPeriod> openingPeriods;

        protected OpeningDay(IEnumerable<OpeningPeriod> openingPeriods)
        {
            this.openingPeriods = openingPeriods?.ToList() ?? new List<OpeningPeriod>();
        }

        public IReadOnlyCollection<OpeningPeriod> OpeningPeriods
        {
            get
            {
                return new ReadOnlyCollection<OpeningPeriod>(openingPeriods);
            }
        }

        public OpeningPeriod FindPeriodAtTime(TimeSpan time)
        {
            return openingPeriods.SingleOrDefault(en => en.Start <= time && time <= en.End);
        }

        public Result<TSpecificOpeningDay> AddPeriod(OpeningPeriod openingPeriod)
        {
            if (OpeningPeriods.Any(en => en.Start == openingPeriod.Start && en.End == openingPeriod.End))
                return SuccessResult<TSpecificOpeningDay>.Create(CreateNewInstance(OpeningPeriods.AsEnumerable()));

            if (openingPeriod.Start.TotalHours < OpeningPeriod.EarliestOpeningTime)
                return FailureResult<TSpecificOpeningDay>.Create(FailureResultCode.RestaurantOpeningPeriodBeginsTooEarly);

            if (!(openingPeriod.End.TotalHours > openingPeriod.Start.TotalHours))
                return FailureResult<TSpecificOpeningDay>.Create(FailureResultCode.RestaurantOpeningPeriodEndsBeforeStart);

            var anyOverlapping = OpeningPeriods.Any(period => PeriodsOverlapping(openingPeriod, period));

            if (anyOverlapping)
                return FailureResult<TSpecificOpeningDay>.Create(FailureResultCode.RestaurantOpeningPeriodIntersects);

            var changedOpeningPeriods = new List<OpeningPeriod>();
            changedOpeningPeriods.AddRange(OpeningPeriods);
            changedOpeningPeriods.Add(openingPeriod);

            return SuccessResult<TSpecificOpeningDay>.Create(CreateNewInstance(changedOpeningPeriods));
        }

        public TSpecificOpeningDay RemovePeriod(TimeSpan start)
        {
            return CreateNewInstance(openingPeriods.Where(en => en.Start != start));
        }

        protected abstract TSpecificOpeningDay CreateNewInstance(IEnumerable<OpeningPeriod> openingPeriods);

        private static bool PeriodsOverlapping(OpeningPeriod x, OpeningPeriod y)
        {
            return x.Start.TotalHours <= y.End.TotalHours && y.Start.TotalHours <= x.End.TotalHours;
        }
    }
}
