using System;
using System.Globalization;
using System.Security.Cryptography;
using InstaMelody.Infrastructure.Enums;

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


        /// <summary>
        ///     The generate random integer.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static int GenerateRandomInt(int intSize)
        {
            var rand = RandomNumberGenerator.Create();

            var source = new byte[intSize];
            var result = new int[1];

            rand.GetBytes(source);
            Buffer.BlockCopy(source, 0, result, 0, intSize);

            return result[0];
        }

        /// <summary>
        /// The get bytes.
        /// </summary>
        /// <param name="inputString">The Input String.</param>
        /// <returns>
        /// The <see cref="byte" />.
        /// </returns>
        public static byte[] GetBytes(string inputString)
        {
            var bytes = new byte[inputString.Length * sizeof(char)];
            Buffer.BlockCopy(inputString.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// The byte array to string.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ByteArrayToString(byte[] bytes)
        {
            var ndx = 0;
            var result = string.Empty;

            while (ndx < bytes.Length)
            {
                result += bytes[ndx].ToString(CultureInfo.InvariantCulture) + ",";
                ndx++;
            }

            return result;
        }

        /// <summary>
        /// Gets the apns string.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string GetApnsTypeString(APNSTypeEnum type)
        {
            switch (type)
            {
                case APNSTypeEnum.FriendRequest:
                    return "friend-request";
                case APNSTypeEnum.ChatCreated:
                    return "new-chat";
                case APNSTypeEnum.ChatNewMessage:
                    return "new-chat-message";
                case APNSTypeEnum.ChatNewUser:
                case APNSTypeEnum.ChatRemoveUser:
                    return "chat-user-added-removed";
            }

            return null;
        }

        /// <summary>
        /// Gets the apns alert string.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public static string GetApnsAlertString(APNSTypeEnum type, string sender)
        {
            switch (type)
            {
                case APNSTypeEnum.FriendRequest:
                    return string.Format("New friend request from {0}.", sender);
                case APNSTypeEnum.ChatCreated:
                    return string.Format("New chat request from {0}.", sender);
                case APNSTypeEnum.ChatNewMessage:
                    return string.Format("New message from {0}.", sender);
            }

            return null;
        }
    }
}
