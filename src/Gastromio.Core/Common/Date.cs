using System;

namespace Gastromio.Core.Common
{
    public readonly struct Date : IComparable, IComparable<Date>, IEquatable<Date>
    {
        private readonly DateTime dateTime;

        public Date(int year, int month, int day)
        {
            dateTime = new DateTime(year, month, day);
        }

        private Date(DateTime dateTime)
        {
            this.dateTime = dateTime;
        }

        public int Year
        {
            get
            {
                return dateTime.Year;
            }
        }

        public int Month
        {
            get
            {
                return dateTime.Month;
            }
        }

        public int Day
        {
            get
            {
                return dateTime.Day;
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                return dateTime.DayOfWeek;
            }
        }

        public Date AddDays(int days)
        {
            return new Date(dateTime.AddDays(days));
        }

        public Date AddWeeks(int weeks)
        {
            return new Date(dateTime.AddDays(weeks * 7));
        }

        public Date AddMonths(int months)
        {
            return new Date(dateTime.AddMonths(months));
        }

        public Date AddYears(int years)
        {
            return new Date(dateTime.AddYears(years));
        }

        public TimeSpan Subtract(Date other)
        {
            return dateTime - other.dateTime;
        }

        public DateTime ToDateTime(DateTimeKind kind)
        {
            return new DateTime(Year, Month, Day, 0, 0, 0, kind);
        }

        public DateTimeOffset ToLocalDateTimeOffset()
        {
            var offset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
            return new DateTimeOffset(Year, Month, Day, 0, 0, 0, offset);
        }

        public DateTimeOffset ToUtcDateTimeOffset()
        {
            return new DateTimeOffset(Year, Month, Day, 0, 0, 0, TimeSpan.Zero);
        }

        /// <summary>
        /// Gets the start date of the current week.
        /// </summary>
        /// <returns>The start date of the current week.</returns>
        public Date StartOfCalendarWeek()
        {
            var differenceOfDaysToMonday = (7 + (DayOfWeek - DayOfWeek.Monday)) % 7;
            return AddDays( -1 * differenceOfDaysToMonday);
        }

        /// <summary>
        /// Gets the end date of the current week.
        /// </summary>
        /// <returns>The end date of the current week.</returns>
        public Date EndOfCalendarWeek()
        {
            var differenceOfDaysToMonday = (7 + (DayOfWeek - DayOfWeek.Monday)) % 7;
            return AddDays(6 - differenceOfDaysToMonday);
        }

        /// <summary>
        /// Gets the start date of the current month.
        /// </summary>
        /// <returns>The start date of the current month.</returns>
        public Date StartOfCalendarMonth()
        {
            return new Date(Year, Month, 1);
        }

        /// <summary>
        /// Gets the end date of the current month.
        /// </summary>
        /// <returns>The end date of the current month.</returns>
        public Date EndOfCalendarMonth()
        {
            return StartOfCalendarMonth().AddMonths(1).AddDays(-1);
        }

        public override string ToString()
        {
            return dateTime.ToShortDateString();
        }

        public bool Equals(Date other)
        {
            return CompareTo(other) == 0;
        }

        public int CompareTo(Date other)
        {
            return dateTime.CompareTo(other.dateTime);
        }

        public override bool Equals(object obj)
        {
            return obj is Date other && Equals(other);
        }

        public override int GetHashCode()
        {
            return dateTime.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            return obj is Date other ? CompareTo(other) : -1;
        }

        public static Date Today
        {
            get
            {
                return new Date(DateTime.Today);
            }
        }

        public static int Compare(Date left, Date right)
        {
            return left.CompareTo(right);
        }

        public static bool operator==(Date left, Date right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator!=(Date left, Date right)
        {
            return left.CompareTo(right) != 0;
        }

        public static bool operator<(Date left, Date right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator<=(Date left, Date right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator>(Date left, Date right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator>=(Date left, Date right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
