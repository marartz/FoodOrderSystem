using System;

namespace Gastromio.Core.Domain.Model.Restaurants
{
    public class OpeningPeriod
    {
        public const double EarliestOpeningTime = 4d;

        public OpeningPeriod(TimeSpan start, TimeSpan end)
        {
            if (end.TotalHours >= EarliestOpeningTime)
            {
                Start = start;
                End = end;
            }
            else
            {
                Start = start;
                End = TimeSpan.FromHours(end.TotalHours + 24d);
            }
        }

        public TimeSpan Start { get; }
        
        public TimeSpan End { get; }
    }
}
