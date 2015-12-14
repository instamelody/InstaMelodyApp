namespace InstaMelody.API.Constants
{
    public static class Exceptions
    {
        #region Authentication Exceptions

        /// <summary>
        /// The null authentication
        /// </summary>
        public const string NullAuth = "NULL Authentication request.";

        /// <summary>
        /// The null update password
        /// </summary>
        public const string NullUpdatePassword = "NULL Update Password request.";

        /// <summary>
        /// The failed authentication.
        /// </summary>
        public const string FailedAuthentication = "Failed Authentication.";

        /// <summary>
        /// The failed validation.
        /// </summary>
        public const string FailedValidation = "Failed to Validate Session.";

        /// <summary>
        /// The failed end session.
        /// </summary>
        public const string FailedEndSession = "Failed to End Session.";

        /// <summary>
        /// The failed update password
        /// </summary>
        public const string FailedUpdatePassword = "Failed to update the User password.";

        /// <summary>
        /// The failed reset password
        /// </summary>
        public const string FailedResetPassword = "Failed to reset the User password.";

        #endregion Authentication Exceptions

        #region Users Exceptions

        /// <summary>
        /// The null user
        /// </summary>
        public const string NullUser = "NULL User request.";

        /// <summary>
        /// The user not found.
        /// </summary>
        public const string UserNotFound = "User Not Found.";

        /// <summary>
        /// The user profile not found.
        /// </summary>
        public const string UserProfileNotFound = "No User Profile Data Was Found.";

        /// <summary>
        /// The failed new user.
        /// </summary>
        public const string FailedNewUser = "Failed to Add New User.";

        /// <summary>
        /// The failed update user.
        /// </summary>
        public const string FailedUpdateUser = "Failed to Update User Info.";

        /// <summary>
        /// The failed find users.
        /// </summary>
        public const string FailedFindUsers = "Failed to Find any Users.";

        /// <summary>
        /// The failed user unlock.
        /// </summary>
        public const string FailedUserUnlock = "Failed to Unlock User {0}.";

        /// <summary>
        /// The failed add friend
        /// </summary>
        public const string FailedAddFriend = "Failed to add the requested User as a friend.";

        /// <summary>
        /// The failed approve friend
        /// </summary>
        public const string FailedApproveFriend = "Failed to approve the requested User as a friend.";

        /// <summary>
        /// The failed deny friend
        /// </summary>
        public const string FailedDenyFriend = "Failed to deny the requested User as a friend.";

        /// <summary>
        /// The failed delete friend
        /// </summary>
        public const string FailedDeleteFriend = "Failed to delete the requested User as a friend.";

        /// <summary>
        /// The failed get friends
        /// </summary>
        public const string FailedGetFriends = "Failed to find friends.";

        /// <summary>
        /// The null receipt
        /// </summary>
        public const string NullReceipt = "Null Receipt request.";

        /// <summary>
        /// The failed create receipt
        /// </summary>
        public const string FailedCreateReceipt = "Failed to create a new App Purchase Receipt record.";

        /// <summary>
        /// The failed get receipt
        /// </summary>
        public const string FailedGetReceipt = "Failed to retrieve the requested App Purchase Receipt(s) for this User.";

        /// <summary>
        /// The failed validate receipt
        /// </summary>
        public const string FailedValidateReceipt = "Failed to validate the requested App Purchase Receipt(s) for this User.";

        /// <summary>
        /// The failed delete receipt
        /// </summary>
        public const string FailedDeleteReceipt = "Failed to delete the requested App Purchase Receipt";

        public const string FailedGetActivity = "Failed to get the requested User activity.";

        #endregion Users Exceptions

        #region Stations Exceptions

        /// <summary>
        /// The null stations
        /// </summary>
        public const string NullStations = "NULL Station request.";

        /// <summary>
        /// The failed create station
        /// </summary>
        public const string FailedCreateStation = "Failed to create a new Station.";

        /// <summary>
        /// The failed update station
        /// </summary>
        public const string FailedUpdateStation = "Failed to update Station: {0}.";

        /// <summary>
        /// The failed remove categories
        /// </summary>
        public const string FailedRemoveCategories = "Failed to remove Categories from Station: {0}.";

        /// <summary>
        /// The failed publish station
        /// </summary>
        public const string FailedPublishStation = "Failed to publish Station: {0}.";

        /// <summary>
        /// The failed unpublish station
        /// </summary>
        public const string FailedUnpublishStation = "Failed to unpublish Station: {0}.";

        /// <summary>
        /// The failed follow station
        /// </summary>
        public const string FailedFollowStation = "Failed to follow Station: {0}.";

        /// <summary>
        /// The failed unfollow station
        /// </summary>
        public const string FailedUnfollowStation = "Failed to un-follow Station: {0}.";

        /// <summary>
        /// The failed like station
        /// </summary>
        public const string FailedLikeStation = "Failed to like Station: {0}.";

        /// <summary>
        /// The failed unlike station
        /// </summary>
        public const string FailedUnlikeStation = "Failed to unlike Station: {0}.";

        /// <summary>
        /// The failed get stations
        /// </summary>
        public const string FailedGetStations = "Failed to get Stations.";

        /// <summary>
        /// The failed get top stations
        /// </summary>
        public const string FailedGetTopStations = "Failed to get the top Stations.";

        /// <summary>
        /// The failed get station followers
        /// </summary>
        public const string FailedGetStationFollowers = "Failed to get followers list for Station: {0}.";

        /// <summary>
        /// The failed send message
        /// </summary>
        public const string FailedSendMessage = "Failed to send a Message to Station: {0}.";

        /// <summary>
        /// The failed create post
        /// </summary>
        public const string FailedCreatePost = "Failed to create a Post for Station: {0}.";

        /// <summary>
        /// The failed get station posts
        /// </summary>
        public const string FailedGetStationPosts = "Failed to get the Posts for Station: {0}.";

        /// <summary>
        /// The failed get station messages
        /// </summary>
        public const string FailedGetStationMessages = "Failed to get the Messages for Station: {0}.";

        /// <summary>
        /// The failed reply station message
        /// </summary>
        public const string FailedReplyStationMessage = "Failed to create a reply for Message/Post: {0}.";

        /// <summary>
        /// The failed like post
        /// </summary>
        public const string FailedLikePost = "Failed to Like the Post: {0}.";

        /// <summary>
        /// The failed unlike post
        /// </summary>
        public const string FailedUnlikePost = "Failed to Unlike the Post: {0}.";

        #endregion Stations Exceptions

        #region Messages Exceptions

        /// <summary>
        /// The null message
        /// </summary>
        public const string NullChat = "NULL Chat request.";

        /// <summary>
        /// The failed create chat
        /// </summary>
        public const string FailedCreateChat = "Failed to create a Chat.";

        /// <summary>
        /// The failed add user to chat
        /// </summary>
        public const string FailedAddUserToChat = "Failed to add the requested User to the Chat.";

        /// <summary>
        /// The failed send message
        /// </summary>
        public const string FailedSendChatMessage = "Failed to send a new Chat Message.";

        /// <summary>
        /// The failed get chat message
        /// </summary>
        public const string FailedGetChatMessage = "Failed to get a Chat Message for Chat: {0} with Id: {1}.";

        /// <summary>
        /// The failed get chat
        /// </summary>
        public const string FailedGetChat = "Failed to get Chat with Id {0}.";

        /// <summary>
        /// The failed get user chats
        /// </summary>
        public const string FailedGetUserChats = "Failed to get Chats for User.";

        #endregion Messages Exceptions

        #region Melodies Exceptions

        /// <summary>
        /// The null melodies
        /// </summary>
        public const string NullMelodies = "NULL Melody request.";

        /// <summary>
        /// The failed get melodies
        /// </summary>
        public const string FailedGetMelodies = "Failed to retrieve Melodies.";

        /// <summary>
        /// The failed get user melodies
        /// </summary>
        public const string FailedGetUserMelodies = "Failed to retrieve Melodies for this User.";

        /// <summary>
        /// The failed create melody
        /// </summary>
        public const string FailedCreateMelody = "Failed to create a new Melody.";

        /// <summary>
        /// The null loop
        /// </summary>
        public const string NullLoop = "NULL Loop request.";

        /// <summary>
        /// The failed create loop
        /// </summary>
        public const string FailedCreateLoop = "Failed to create a new Loop.";

        /// <summary>
        /// The failed update loop
        /// </summary>
        public const string FailedUpdateLoop = "Failed to update Loop {0}.";

        /// <summary>
        /// The failed attach to loop
        /// </summary>
        public const string FailedAttachToLoop = "Failed to attach a new part to Loop {0}.";

        /// <summary>
        /// The failed delete loop
        /// </summary>
        public const string FailedDeleteLoop = "Failed to delete Loop {0}.";

        #endregion Melodies Exceptions

        #region Categories Exceptions

        /// <summary>
        /// The null category
        /// </summary>
        public const string NullCategory = "NULL Category request.";

        /// <summary>
        /// The failed add category
        /// </summary>
        public const string FailedAddCategory = "Failed to add a new Category.";

        /// <summary>
        /// The failed update category
        /// </summary>
        public const string FailedUpdateCategory = "Failed to update Category: {0}.";

        /// <summary>
        /// The failed delete category
        /// </summary>
        public const string FailedDeleteCategory = "Failed to delete Category: {0}";

        /// <summary>
        /// The failed find categories
        /// </summary>
        public const string FailedFindCategories = "Failed to find any Categories.";

        /// <summary>
        /// The failed category parameter
        /// </summary>
        public const string FailedCategoryParameter = "Please pass an 'id' parameter to retrieve child Categories.";

        /// <summary>
        /// The failed find child categories
        /// </summary>
        public const string FailedFindChildCategories = "Failed to find any child Categories for Category: {0}";

        #endregion Categories Exceptions

        #region File Upload Exceptions

        /// <summary>
        /// The null file upload
        /// </summary>
        public const string NullFileUpload = "NULL or Incomplete File Upload request.";

        /// <summary>
        /// The failed file upload
        /// </summary>
        public const string FailedFileUpload = "File Upload failed because the request was not properly formatted.";

        #endregion File Upload Exceptions
    }
}