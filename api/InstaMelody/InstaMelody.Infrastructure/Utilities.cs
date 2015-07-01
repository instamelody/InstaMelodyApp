using System;
using System.Text;
using System.Text.RegularExpressions;

namespace InstaMelody.Infrastructure
{
    public static class Utilities
    {
        /// <summary>
        /// The unix timestamp from date time.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public static long UnixTimestampFromDateTime(DateTimeOffset date)
        {
            // take the date, convert to UTC, use the "ticks" unit, then subtract the equivalent units of the 1/1/1970 start date.
            long result = 0;
            if (!date.Equals(default(DateTimeOffset)))
            {
                result = date.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks;
            }

            // then divide the number by the number of ticks/second. result = seconds since 1/1/1970.
            result /= TimeSpan.TicksPerSecond;
            return result;
        }

        /// <summary>
        /// The date time from unix timestamp.
        /// </summary>
        /// <param name="unixTime">
        /// The unix time.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTimeOffset DateTimeFromUnixTimestamp(long unixTime)
        {
            // Unix timestamp is seconds past epoch (1/1/1970)
            var result = new DateTimeOffset(1970, 1, 1, 0, 0, 0, new TimeSpan(0));
            result = result.AddSeconds(unixTime).UtcDateTime;
            return result;
        }

        /// <summary>
        /// The are strings different.
        /// </summary>
        /// <param name="one">
        /// The one.
        /// </param>
        /// <param name="two">
        /// The two.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool AreStringsDifferent(string one, string two)
        {
            if (one == null && two == null) return false;
            if (one == null || two == null) return true;
            return !one.Equals(two);
        }
    }
}
