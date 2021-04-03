using System;

namespace Gastromio.Core.Common
{
    public static class DateTimeExtensions
    {
        public static DateTimeOffset ToDateTimeOffset(this DateTime self, TimeSpan offset)
        {
            return new DateTimeOffset(self, offset);
        }
    }
}
