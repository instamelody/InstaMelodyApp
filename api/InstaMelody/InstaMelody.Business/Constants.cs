namespace InstaMelody.Business
{
    /// <summary>
    /// The constants.
    /// </summary>
    internal class Constants
    {
        /// <summary>
        /// The alphabet start.
        /// Index for the unicode character "A"
        /// </summary>
        public const short AlphabetStart = 65;

        /// <summary>
        /// The integer size.
        /// The Number of Bytes used to store an integer (32 bits).
        /// </summary>
        public const int IntSize = 4;

        /// <summary>
        /// The activity friend
        /// </summary>
        public const string ActivityFriend = "{0} is now friends with {1}.";

        /// <summary>
        /// The activity station like
        /// </summary>
        public const string ActivityStationLike = "{0} liked {1}.";

        /// <summary>
        /// The activity station message user like
        /// </summary>
        public const string ActivityStationMessageUserLike = "{0} liked a message from {1}.";

        /// <summary>
        /// The activity station post
        /// </summary>
        public const string ActivityStationPost = "{0} posted to {1}.";

        /// <summary>
        /// The activity station post reply
        /// </summary>
        public const string ActivityStationPostReply = "{0} replied to a post from {1}.";
    }
}
