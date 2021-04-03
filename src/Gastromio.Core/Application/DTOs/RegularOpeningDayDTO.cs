using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.DTOs
{
    public class RegularOpeningDayDTO
    {
        public RegularOpeningDayDTO(int dayOfWeek, IEnumerable<OpeningPeriod> openingPeriods)
        {
            DayOfWeek = dayOfWeek;
            OpeningPeriods = new ReadOnlyCollection<OpeningPeriodDTO>(
                openingPeriods
                    ?.Select(en => new OpeningPeriodDTO(en))
                    .ToList() ?? new List<OpeningPeriodDTO>());
        }

        public int DayOfWeek { get; }

        public IReadOnlyCollection<OpeningPeriodDTO> OpeningPeriods { get; }
    }
}
