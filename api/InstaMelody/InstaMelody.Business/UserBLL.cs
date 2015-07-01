using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using InstaMelody.Business.Properties;
using InstaMelody.Data;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Model.Enums;
using ModelUtilities = InstaMelody.Model.Utilities;

namespace InstaMelody.Business
{
    public class UserBLL
    {
        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        public User FindUser(User user, Guid sessionToken)
        {
            User foundUser = null;

            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null || sessionUser == default(User))
            {
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (!user.Id.Equals(default(Guid)))
            {
                foundUser = this.GetUserById(user.Id, sessionToken);
            }
            
            if (foundUser == null && !string.IsNullOrWhiteSpace(user.DisplayName))
            {
                foundUser = this.GetUserByDisplayName(user.DisplayName);
            }
            else if (foundUser == null && !string.IsNullOrWhiteSpace(user.EmailAddress))
            {
                foundUser = this.GetUserByEmailAddress(user.EmailAddress);
            }

            return this.GetUserWithImage(foundUser);
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">
        /// </exception>
        public User GetUser(User user, Guid sessionToken)
        {
            User foundUser = null;
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null || sessionUser == default(User))
            {
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (!user.Id.Equals(default(Guid)))
            {
                foundUser = this.GetUserById(user.Id, sessionToken);
            }
            else if (!string.IsNullOrWhiteSpace(user.DisplayName))
            {
                foundUser = this.GetUserByDisplayName(user.DisplayName);
            }
            else if (!string.IsNullOrWhiteSpace(user.EmailAddress))
            {
                foundUser = this.GetUserByEmailAddress(user.EmailAddress);
            }

            if (foundUser != null && !foundUser.Id.Equals(sessionUser.Id))
            {
                throw new UnauthorizedAccessException(
                    string.Format("User: {0} cannot retrieve user info for User: {1}", sessionUser.Id, 
                    user.Id.Equals(default(Guid)) 
                    ? (string.IsNullOrWhiteSpace(user.DisplayName) ? user.EmailAddress : user.DisplayName) 
                    : user.Id.ToString()));
            }

            return this.GetUserWithImage(foundUser);
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">
        /// </exception>
        /// <exception cref="System.Data.DataException"></exception>
        public User GetUserById(Guid userId, Guid sessionToken)
        {
            var auth = new AuthenticationBLL();
            var session = auth.GetSession(sessionToken);
            if (session.Token.Equals(default(Guid)) || session.IsDeleted.Equals(true))
            {
                throw new UnauthorizedAccessException(string.Format("Invalid Session Token: {0}.", sessionToken));
            }

            var requestor = Utilities.GetUserBySession(sessionToken);
            if (requestor == null || !requestor.Id.Equals(userId))
            {
                throw new UnauthorizedAccessException(
                    string.Format("User with session token {0} is not authorized to get User {1}'s data.", session.Token, userId));
            }

            User result;
            try
            {
                var dal = new Users();
                result = dal.FindById(userId);

                // remove sensitive information
                result = result.StripSensitiveInfo();
            }
            catch (Exception)
            {
                throw new DataException(string.Format("Error getting user ID {0}.", userId));
            }

            return this.GetUserWithImage(result) ?? new User();
        }

        /// <summary>
        /// Gets the user by the display name.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <returns></returns>
        public User GetUserByDisplayName(string displayName)
        {
            var dal = new Users();
            var user = dal.FindByDisplayName(displayName);

            // remove sensitive information
            user = user.StripSensitiveInfo();

            return this.GetUserWithImage(user);
        }

        /// <summary>
        /// Gets the user by email address.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns></returns>
        public User GetUserByEmailAddress(string emailAddress)
        {
            var dal = new Users();
            var user = dal.FindByEmail(emailAddress);

            // remove sensitive information
            user = user.StripSensitiveInfo();

            return this.GetUserWithImage(user);
        }

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="newUser">The new user.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// A password is required to create a new user
        /// </exception>
        /// <exception cref="System.Data.DataException">
        /// </exception>
        /// <exception cref="System.Exception">
        /// </exception>
        public User AddNewUser(User newUser)
        {
            // check for validation errors
            var validationErrors = ModelUtilities.Validate(newUser).ToList();
            if (validationErrors.Any())
            {
                var sb = new StringBuilder();
                foreach (var error in validationErrors)
                {
                    sb.AppendFormat("{0}; ", error);
                }
                throw new ArgumentException(sb.ToString());
            }

            // check password is not empty
            if (string.IsNullOrWhiteSpace(newUser.Password))
            {
                throw new ArgumentException("A password is required to create a new user");
            }

            User userExisting = null;
            // check for matching user by display name
            if (!string.IsNullOrWhiteSpace(newUser.DisplayName))
            {
                userExisting = this.GetUserByDisplayName(newUser.DisplayName);
                if (userExisting.Id != default(Guid))
                {
                    throw new DataException(string.Format("User name {0} is not available.", newUser.DisplayName));
                }
            }

            // check for matching user by email
            if (!string.IsNullOrWhiteSpace(newUser.EmailAddress))
            {
                userExisting = this.GetUserByEmailAddress(newUser.EmailAddress);
                if (userExisting.Id != default(Guid))
                {
                    throw new DataException(string.Format("The email address {0} is already in use.", newUser.EmailAddress));
                }
            }

            // make a copy of the User object so we aren't editing the input argument
            var userCopy = newUser.Clone();

            // generate a random salt string for hashing the password
            var auth = new AuthenticationBLL();
            userCopy.HashSalt = auth.GenerateSalt(Settings.Default.SaltLength);
            userCopy.Password = auth.HashSaltedPassword(userCopy.HashSalt, userCopy.Password);

            // provide the date time stamps
            var timeStamp = DateTime.UtcNow;
            userCopy.DateCreated = timeStamp;
            userCopy.DateModified = timeStamp;
            userCopy.NumberLoginFailures = 0;
            userCopy.IsLocked = false;
            userCopy.IsDeleted = false;

            // add user to db & retrieve record
            var dal = new Users();
            var addedUserGuid = dal.AddUser(userCopy);
            if (addedUserGuid == null || addedUserGuid == default(Guid))
            {
                throw new Exception(string.Format("Error adding user: {0}", newUser.DisplayName));
            }
            var addedUser = dal.FindById(addedUserGuid);
            if (addedUser == null)
            {
                throw new Exception(string.Format("Error adding user: {0}", newUser.DisplayName));
            }

            // remove sensitive information
            addedUser = addedUser.StripSensitiveInfo();

            return this.GetUserWithImage(addedUser);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="userToUpdate">The user to update.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">
        /// </exception>
        /// <exception cref="System.Data.DataException">
        /// </exception>
        public User UpdateUser(User userToUpdate, Guid sessionToken)
        {
            var auth = new AuthenticationBLL();
            var session = auth.GetSession(sessionToken);
            if (session.Token.Equals(default(Guid)) || session.IsDeleted.Equals(true))
            {
                throw new UnauthorizedAccessException(string.Format("Invalid Session Token: {0}.", sessionToken));
            }

            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null || !sessionUser.Id.Equals(userToUpdate.Id))
            {
                throw new UnauthorizedAccessException(
                    string.Format("User with session token {0} is not authorized to update User {1}'s data.", session.Token, userToUpdate.Id));
            }

            // check for validation errors
            var validationErrors = ModelUtilities.Validate(userToUpdate).ToList();
            if (validationErrors.Any())
            {
                var sb = new StringBuilder();
                foreach (var error in validationErrors)
                {
                    sb.AppendFormat("{0}; ", error);
                }
                throw new ArgumentException(sb.ToString());
            }

            // check to make sure updated user name or email address is not taken
            if (!userToUpdate.DisplayName.Equals(sessionUser.DisplayName)
                || !userToUpdate.EmailAddress.Equals(sessionUser.EmailAddress))
            {
                var userExisting = this.GetUserByDisplayName(userToUpdate.DisplayName);
                if (userExisting.Id != default(Guid))
                {
                    throw new DataException(string.Format("User name {0} is not available.", userToUpdate.DisplayName));
                }

                userExisting = this.GetUserByEmailAddress(userToUpdate.EmailAddress);
                if (userExisting.Id != default(Guid))
                {
                    throw new DataException(string.Format("The email address {0} is already in use.", userToUpdate.EmailAddress));
                }
            }

            // make a copy of the User object so we aren't editing the input argument
            var userCopy = userToUpdate.Clone();

            var dal = new Users();
            userCopy.DateModified = DateTime.UtcNow;
            var updatedUser = dal.UpdateUser(sessionUser.Id, userCopy);
            if (updatedUser == null)
            {
                throw new DataException(string.Format("Could not update User: {0}", sessionUser.Id));
            }

            // remove sensitive information
            updatedUser = updatedUser.StripSensitiveInfo();
            return this.GetUserWithImage(updatedUser);
        }

        /// <summary>
        /// Updates the user image.
        /// </summary>
        /// <param name="userToUpdate">The user to update.</param>
        /// <param name="newImage">The new image.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        public ApiUserFileUpload UpdateUserImage(User userToUpdate, Image newImage, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            if (newImage == null
                || string.IsNullOrWhiteSpace(newImage.FileName))
            {
                throw new ArgumentException("Image not provided.");
            }

            // find user profile
            var existingUser = this.FindUser(userToUpdate, sessionToken);
            if (existingUser == null || existingUser.Id.Equals(default(Guid)))
            {
                throw new ArgumentException("Could not find user.");
            }

            var imageBll = new ImageBLL();

            // delete existing profile image
            if (existingUser.UserImageId != null && !existingUser.UserImageId.Equals(default(int)))
            {
                imageBll.DeleteImage(new Image
                {
                    Id = (int)existingUser.UserImageId
                });
            }

            // add new image
            var addedImage = imageBll.AddImage(newImage);
            existingUser.UserImageId = addedImage.Id;

            // update user with image id
            var updatedUser = this.UpdateUserProfileImage(existingUser, sessionToken);
            updatedUser.Image = addedImage;

            // create file upload token
            var uploadBll = new FileUploadBLL();
            var uploadToken = uploadBll.CreateToken(new FileUploadToken
            {
                UserId = updatedUser.Id,
                MediaType = FileUploadTypeEnum.UserImage,
                FileName = addedImage.FileName
            });

            return new ApiUserFileUpload
            {
                User = updatedUser,
                FileUploadToken = uploadToken
            };
        }

        public User UnlockUser(Guid userId, Guid sessionToken)
        {
            // TODO:
            throw new NotImplementedException();
        }

        public User LockUser(Guid userId)
        {
            // TODO:
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">A user can only delete their own profile.</exception>
        /// <exception cref="System.Data.DataException"></exception>
        public User DeleteUser(User user, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null || !sessionUser.Id.Equals(user.Id))
            {
                throw new UnauthorizedAccessException("A user can only delete their own profile.");
            }

            var dal = new Users();
            var existingUser = dal.FindById(user.Id);
            if (existingUser == null)
            {
                throw new DataException(string.Format("User ID {0} not found.", user.Id));
            }

            dal.DeleteUser(user.Id);
            user.IsDeleted = true;

            // remove sensitive information
            user = user.StripSensitiveInfo();

            // end and delete all sessions
            var auth = new AuthenticationBLL();
            auth.DeleteAllUserSessions(user.Id);

            // delete profile image
            if (user.UserImageId != null)
            {
                var imageBll = new ImageBLL();
                imageBll.DeleteImage(new Image
                {
                    Id = (int)user.UserImageId
                });
            }

            return user;
        }

        /// <summary>
        /// Requests the friend.
        /// </summary>
        /// <param name="friend">The friend.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Could not validate session.</exception>
        /// <exception cref="System.Data.DataException">No friend information provided
        /// or
        /// Could not find requested friend in database.</exception>
        public string RequestFriend(User friend, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            if (friend == null
                || (friend.Id.Equals(default(Guid))
                    && string.IsNullOrWhiteSpace(friend.DisplayName)
                    && string.IsNullOrWhiteSpace(friend.EmailAddress)))
            {
                throw new DataException("No friend information provided.");
            }

            var userDal = new Users();

            // try to request friend by id, email, or display name
            User requestedFriend = null;
            if (!friend.Id.Equals(default(Guid)))
            {
                requestedFriend = userDal.FindById(friend.Id);
            }

            if (requestedFriend == null)
            {
                requestedFriend = userDal.FindByDisplayName(friend.DisplayName) ??
                                  userDal.FindByEmail(friend.EmailAddress);
            }

            if (requestedFriend == null)
            {
                throw new DataException("Could not find requested friend in database.");
            }

            // make friend request
            var friendDal = new UserFriends();
            friendDal.RequestFriend(sessionUser.Id, requestedFriend.Id);

            return requestedFriend.DisplayName;
        }

        /// <summary>
        /// Approves the friend request.
        /// </summary>
        /// <param name="requestor">The requestor.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Could not validate session.</exception>
        /// <exception cref="System.Data.DataException">No friend information provided to approve.
        /// or
        /// Could not find requested friend in database.</exception>
        public User ApproveFriendRequest(User requestor, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            if (requestor == null
                || (requestor.Id.Equals(default(Guid))
                    && string.IsNullOrWhiteSpace(requestor.DisplayName)
                    && string.IsNullOrWhiteSpace(requestor.EmailAddress)))
            {
                throw new DataException("No friend information provided to approve.");
            }

            var userDal = new Users();

            // try to request friend by id, email, or display name
            User requestingFriend = null;
            if (!requestor.Id.Equals(default(Guid)))
            {
                requestingFriend = userDal.FindById(requestor.Id);
            }

            if (requestingFriend == null)
            {
                requestingFriend = userDal.FindByDisplayName(requestor.DisplayName) ??
                                  userDal.FindByEmail(requestor.EmailAddress);
            }

            if (requestingFriend == null)
            {
                throw new DataException("Could not find requested friend in database.");
            }

            // approve friend request
            var friendDal = new UserFriends();
            if (friendDal.CanApproveDeny(sessionUser.Id, requestingFriend.Id))
            {
                friendDal.ApproveRequest(sessionUser.Id, requestingFriend.Id);
            }
            else
            {
                throw new DataException(string.Format("User: {0} not authorized to execute this request.", sessionUser.Id));
            }

            requestingFriend = requestingFriend.StripSensitiveInfo();
            return this.GetUserWithImage(requestingFriend);
        }

        /// <summary>
        /// Denies the friend request.
        /// </summary>
        /// <param name="requestor">The requestor.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Could not validate session.</exception>
        /// <exception cref="System.Data.DataException">No friend information provided to deny.
        /// or
        /// Could not find requested friend in database.</exception>
        public string DenyFriendRequest(User requestor, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            if (requestor == null
                || (requestor.Id.Equals(default(Guid))
                    && string.IsNullOrWhiteSpace(requestor.DisplayName)
                    && string.IsNullOrWhiteSpace(requestor.EmailAddress)))
            {
                throw new DataException("No friend information provided to deny.");
            }

            var userDal = new Users();

            // try to request friend by id, email, or display name
            User requestingFriend = null;
            if (!requestor.Id.Equals(default(Guid)))
            {
                requestingFriend = userDal.FindById(requestor.Id);
            }

            if (requestingFriend == null)
            {
                requestingFriend = userDal.FindByDisplayName(requestor.DisplayName) ??
                                  userDal.FindByEmail(requestor.EmailAddress);
            }

            if (requestingFriend == null)
            {
                throw new DataException("Could not find requested friend in database.");
            }

            // deny friend request
            var friendDal = new UserFriends();
            if (friendDal.CanApproveDeny(sessionUser.Id, requestingFriend.Id))
            {
                friendDal.DenyRequest(sessionUser.Id, requestingFriend.Id);
            }
            else
            {
                throw new DataException(string.Format("User: {0} not authorized to execute this request.", sessionUser.Id));
            }

            return requestingFriend.DisplayName;
        }

        /// <summary>
        /// Deletes the friend.
        /// </summary>
        /// <param name="friend">The friend.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Could not validate session.</exception>
        /// <exception cref="System.Data.DataException">No friend information provided to delete.
        /// or
        /// Could not find requested friend in database.</exception>
        public string DeleteFriend(User friend, Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            if (friend == null
                || (friend.Id.Equals(default(Guid))
                    && string.IsNullOrWhiteSpace(friend.DisplayName)
                    && string.IsNullOrWhiteSpace(friend.EmailAddress)))
            {
                throw new DataException("No friend information provided to delete.");
            }

            var userDal = new Users();

            // try to request friend by id, email, or display name
            User requestingFriend = null;
            if (!friend.Id.Equals(default(Guid)))
            {
                requestingFriend = userDal.FindById(friend.Id);
            }

            if (requestingFriend == null)
            {
                requestingFriend = userDal.FindByDisplayName(friend.DisplayName) ??
                                  userDal.FindByEmail(friend.EmailAddress);
            }

            if (requestingFriend == null)
            {
                throw new DataException("Could not find requested friend in database.");
            }

            // deny friend request
            var friendDal = new UserFriends();
            friendDal.DeleteFriend(sessionUser.Id, requestingFriend.Id);

            return requestingFriend.DisplayName;
        }

        /// <summary>
        /// Gets the friends by user.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Could not validate session.</exception>
        /// <exception cref="System.Data.DataException"></exception>
        public IList<User> GetFriendsByUser(Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            var dal = new UserFriends();
            var friends = dal.GetUserFriends(sessionUser.Id);
            if (friends == null || !friends.Any())
            {
                throw new DataException(string.Format("Could not find friends for User: {0}", sessionUser.Id));
            }

            return this.GetUsersWithImages(friends);
        }

        /// <summary>
        /// Gets the friends by user.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Could not validate session.</exception>
        /// <exception cref="System.Data.DataException"></exception>
        public IList<User> GetPendingFriendsByUser(Guid sessionToken)
        {
            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null)
            {
                throw new UnauthorizedAccessException("Could not validate session.");
            }

            var dal = new UserFriends();
            var friends = dal.GetPendingFriends(sessionUser.Id);
            if (friends == null || !friends.Any())
            {
                throw new DataException(string.Format("Could not find pending friends for User: {0}", sessionUser.Id));
            }

            return this.GetUsersWithImages(friends);
        }





        /// <summary>
        /// Updates the user image.
        /// </summary>
        /// <param name="userToUpdate">The user to update.</param>
        /// <param name="image">The image.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">
        /// </exception>
        /// <exception cref="System.Data.DataException"></exception>
        private User UpdateUserProfileImage(User userToUpdate, Guid sessionToken)
        {
            var auth = new AuthenticationBLL();
            var session = auth.GetSession(sessionToken);
            if (session.Token.Equals(default(Guid)) || session.IsDeleted.Equals(true))
            {
                throw new UnauthorizedAccessException(string.Format("Invalid Session Token: {0}.", sessionToken));
            }

            var sessionUser = Utilities.GetUserBySession(sessionToken);
            if (sessionUser == null || !sessionUser.Id.Equals(userToUpdate.Id))
            {
                throw new UnauthorizedAccessException(
                    string.Format("User with session token {0} is not authorized to update User {1}'s data.", session.Token, userToUpdate.Id));
            }

            var dal = new Users();
            var updatedUser = dal.UpdateUserProfileImage(sessionUser.Id, userToUpdate.UserImageId);
            if (updatedUser == null)
            {
                throw new DataException(string.Format("Could not update profile image for User: {0}", sessionUser.Id));
            }

            // remove sensitive information
            updatedUser = updatedUser.StripSensitiveInfo();
            return this.GetUserWithImage(updatedUser);
        }

        /// <summary>
        /// Gets the user with image.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        private User GetUserWithImage(User user)
        {
            if (user.UserImageId != null && !user.UserImageId.Equals(default(int)))
            {
                var imageBll = new ImageBLL();
                var image = imageBll.GetImage(new Image
                {
                    Id = (int)user.UserImageId
                });
                if (image != null)
                {
                    user.Image = image;
                }
            }
            return user;
        }

        /// <summary>
        /// Gets the users with images.
        /// </summary>
        /// <param name="users">The users.</param>
        /// <returns></returns>
        private IList<User> GetUsersWithImages(IList<User> users)
        {
            List<User> results = null;

            if (users != null && users.Any())
            {
                results = users.Select(this.GetUserWithImage).ToList();
            }

            return results;
        }




        /// <summary>
        /// Deletes the user image.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <exception cref="System.ArgumentException">User cannot be found.</exception>
        internal void DeleteUserImage(Guid userId)
        {
            var dal = new Users();
            var user = dal.FindById(userId);
            if (user == null) throw new ArgumentException("User cannot be found.");

            dal.UpdateUserProfileImage(userId, null);
        }

        /// <summary>
        /// Ares the users friends.
        /// </summary>
        /// <param name="user1Id">The user1 identifier.</param>
        /// <param name="user2Id">The user2 identifier.</param>
        /// <returns></returns>
        internal bool AreUsersFriends(Guid user1Id, Guid user2Id)
        {
            var bll = new UserFriends();
            return bll.AreUsersFriends(user1Id, user2Id);
        }
    }
}
