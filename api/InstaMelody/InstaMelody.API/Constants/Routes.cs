namespace InstaMelody.API.Constants
{
    public static class Routes
    {
        #region Route Prefixes

        #region Version 0.1

        /// <summary>
        /// The auth prefix.
        /// </summary>
        public const string PrefixAuth01 = "v0.1/Auth";

        /// <summary>
        /// The user prefix.
        /// </summary>
        public const string PrefixUser01 = "v0.1/User";

        /// <summary>
        /// The station prefix.
        /// </summary>
        public const string PrefixStation01 = "v0.1/Station";

        /// <summary>
        /// The category prefix.
        /// </summary>
        public const string PrefixCategory01 = "v0.1/Category";

        /// <summary>
        /// The message prefix.
        /// </summary>
        public const string PrefixMessage01 = "v0.1/Message";

        /// <summary>
        /// The melody prefix.
        /// </summary>
        public const string PrefixMelody01 = "v0.1/Melody";

        /// <summary>
        /// The prefix upload01
        /// </summary>
        public const string PrefixUpload01 = "v0.1/Upload";

        #endregion Version 0.1

        #endregion Route Prefixes

        #region Common Routes

        /// <summary>
        /// The route blank
        /// </summary>
        public const string RouteBlank = "";

        /// <summary>
        /// The route all
        /// </summary>
        public const string RouteAll = "All";

        /// <summary>
        /// The route date time.
        /// </summary>
        public const string RouteDateTime = "DateTime";

        /// <summary>
        /// The route new
        /// </summary>
        public const string RouteNew = "New";

        /// <summary>
        /// The route update
        /// </summary>
        public const string RouteUpdate = "Update";

        /// <summary>
        /// The route delete
        /// </summary>
        public const string RouteDelete = "Delete";

        /// <summary>
        /// The route user
        /// </summary>
        public const string RouteUser = "User";

        /// <summary>
        /// The route find
        /// </summary>
        public const string RouteFind = "Find";

        #endregion

        #region Authentication Routes

        /// <summary>
        /// The route end session
        /// </summary>
        public const string RouteEndSession = "End";

        /// <summary>
        /// The route update password
        /// </summary>
        public const string RouteUpdatePassword = "Password/Update";

        /// <summary>
        /// The route reset password
        /// </summary>
        public const string RouteResetPassword = "Password/Reset";

        #endregion Authentication Routes

        #region User Routes

        /// <summary>
        /// The route update image
        /// </summary>
        public const string RouteUpdateUserImage = "Update/Image";

        /// <summary>
        /// The route user friends
        /// </summary>
        public const string RouteUserFriends = "Friends";

        /// <summary>
        /// The route pending friends
        /// </summary>
        public const string RoutePendingFriends = "Friends/Pending";

        /// <summary>
        /// The route friend request
        /// </summary>
        public const string RouteFriendRequest = "Friend/Request";

        /// <summary>
        /// The route friend approve
        /// </summary>
        public const string RouteFriendApprove = "Friend/Approve";

        /// <summary>
        /// The route friend deny
        /// </summary>
        public const string RouteFriendDeny = "Friend/Deny";

        /// <summary>
        /// The route friend delete
        /// </summary>
        public const string RouteFriendDelete = "Friend/Delete";

        #endregion User Routes

        #region Melody Routes

        /// <summary>
        /// The route loop
        /// </summary>
        public const string RouteLoop = "Loop";

        /// <summary>
        /// The route loop attach
        /// </summary>
        public const string RouteLoopAttach = "Loop/Attach";

        /// <summary>
        /// The route loop delete
        /// </summary>
        public const string RouteLoopDelete = "Loop/Delete";

        #endregion Melody Routes

        #region Category Routes

        /// <summary>
        /// The route child categories
        /// </summary>
        public const string RouteChildCategories = "Children";

        #endregion Category Routes

        #region Messages Routes

        /// <summary>
        /// The route chat
        /// </summary>
        public const string RouteChat = "Chat";

        /// <summary>
        /// The route chat user
        /// </summary>
        public const string RouteChatUser = "Chat/User";

        /// <summary>
        /// The route chat message
        /// </summary>
        public const string RouteChatMessage = "Chat/Message";

        /// <summary>
        /// The route chat remove
        /// </summary>
        public const string RouteChatRemove = "Chat/Delete";

        #endregion Messages Routes

        #region Stations Routes

        /// <summary>
        /// The route remove categories
        /// </summary>
        public const string RouteDeleteCategories = "Delete/Categories";

        /// <summary>
        /// The route follow
        /// </summary>
        public const string RouteFollow = "Follow";

        /// <summary>
        /// The route unfollow
        /// </summary>
        public const string RouteUnfollow = "Unfollow";

        /// <summary>
        /// The route followers
        /// </summary>
        public const string RouteFollowers = "Followers";

        /// <summary>
        /// The route post
        /// </summary>
        public const string RoutePost = "Post";

        /// <summary>
        /// The route posts
        /// </summary>
        public const string RoutePosts = "Posts";

        /// <summary>
        /// The route message
        /// </summary>
        public const string RouteMessage = "Message";

        /// <summary>
        /// The route messages
        /// </summary>
        public const string RouteMessages = "Messages";

        /// <summary>
        /// The route like message
        /// </summary>
        public const string RouteLikeMessage = "Post/Like";

        /// <summary>
        /// The route unlike message
        /// </summary>
        public const string RouteUnlikeMessage = "Post/Unlike";

        /// <summary>
        /// The route reply post
        /// </summary>
        public const string RouteReplyPost = "Post/Reply";

        /// <summary>
        /// The route reply message
        /// </summary>
        public const string RouteReplyMessage = "Message/Reply";

        /// <summary>
        /// The route delete post
        /// </summary>
        public const string RouteDeletePost = "Post/Delete";

        /// <summary>
        /// The route delete message
        /// </summary>
        public const string RouteDeleteMessage = "Message/Delete";

        #endregion Stations Routes

        #region File Upload Routes

        /// <summary>
        /// The route upload token
        /// </summary>
        public const string RouteUploadToken = "{sessionToken}/{token}";

        #endregion File Upload Routes
    }
}