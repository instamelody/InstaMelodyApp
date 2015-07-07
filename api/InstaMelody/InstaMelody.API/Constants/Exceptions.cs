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

        #endregion Users Exceptions

        #region Stations Exceptions



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