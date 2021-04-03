using System;

namespace Gastromio.Core.Common
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTimeOffset StartOfDay(this DateTimeOffset self)
        {
            return new DateTimeOffset(self.Year, self.Month, self.Day, 0, 0, 0, TimeSpan.Zero);
        }

        public static Date ToUtcDate(this DateTimeOffset self)
        {
            var convertedSelf = self.ToUniversalTime();
            return new Date(convertedSelf.Year, convertedSelf.Month, convertedSelf.Day);
        }

        public static Date ToLocalDate(this DateTimeOffset self)
        {
            var convertedSelf = self.ToLocalTime();
            return new Date(convertedSelf.Year, convertedSelf.Month, convertedSelf.Day);
        }
    }
}
