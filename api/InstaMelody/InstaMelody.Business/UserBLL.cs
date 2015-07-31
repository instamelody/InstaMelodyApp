using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using InstaMelody.Business.Properties;
using InstaMelody.Data;
using InstaMelody.Infrastructure;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Model.Enums;
using NLog;
using ModelUtilities = InstaMelody.Model.Utilities;

namespace InstaMelody.Business
{
    public class UserBll
    {
        #region Public Methods

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        public User FindUser(User user, Guid sessionToken)
        {
            try
            {
                Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(string.Format("Could not find a valid session. Token: {0}.", sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            var foundUser = FindUser(user);
            return GetUserWithImage(foundUser);
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

            User sessionUser;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(string.Format("Could not find a valid session. Token: {0}.", sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (!user.Id.Equals(default(Guid)))
            {
                foundUser = GetUserById(user.Id, sessionToken);
            }
            else if (!string.IsNullOrWhiteSpace(user.DisplayName))
            {
                foundUser = GetUserByDisplayName(user.DisplayName);
            }
            else if (!string.IsNullOrWhiteSpace(user.EmailAddress))
            {
                foundUser = GetUserByEmailAddress(user.EmailAddress);
            }

            if (foundUser != null && !foundUser.Id.Equals(sessionUser.Id))
            {
                var userDetail = string.IsNullOrWhiteSpace(user.DisplayName) ? user.EmailAddress : user.DisplayName;
                InstaMelodyLogger.Log(
                    string.Format("User: {0} cannot retrieve user info for another User: {1}", 
                        sessionUser.Id, userDetail), LogLevel.Error);
                throw new UnauthorizedAccessException(
                    string.Format("User: {0} cannot retrieve user info for another User: {1}", sessionUser.Id, userDetail));
            }

            return GetUserWithImage(foundUser);
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
            var auth = new AuthenticationBll();
            var session = auth.GetSession(sessionToken);
            if (session.Token.Equals(default(Guid)) || session.IsDeleted.Equals(true))
            {
                InstaMelodyLogger.Log(string.Format("Could not find a valid session. Token: {0}.", sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Invalid Session Token: {0}.", sessionToken));
            }

            var requestor = Utilities.GetUserBySession(sessionToken);
            if (requestor == null || !requestor.Id.Equals(userId))
            {
                InstaMelodyLogger.Log(
                    string.Format("User is not authorized to get another User's data. Token: {0}, Requested User Id: {1}", 
                        sessionToken, userId), LogLevel.Error);
                throw new UnauthorizedAccessException(
                    string.Format("User with session token {0} is not authorized to get the requested User's data.", session.Token));
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
                InstaMelodyLogger.Log(
                    string.Format("Error getting User. User Id: {0}, Token: {1}", 
                        userId, sessionToken), LogLevel.Error);
                throw new DataException(string.Format("Error getting user ID {0}.", userId));
            }

            return GetUserWithImage(result) ?? new User();
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

            return GetUserWithImage(user);
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

            return GetUserWithImage(user);
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
                InstaMelodyLogger.Log(
                    string.Format("A password is required to create a new User. Display Name: {0}, Email: {1}", 
                        newUser.DisplayName, newUser.EmailAddress), LogLevel.Error);
                throw new ArgumentException("A password is required to create a new User.");
            }

            User userExisting;
            // check for matching user by display name
            if (!string.IsNullOrWhiteSpace(newUser.DisplayName))
            {
                userExisting = GetUserByDisplayName(newUser.DisplayName);
                if (userExisting.Id != default(Guid))
                {
                    InstaMelodyLogger.Log(
                        string.Format("Tried to create User with an existing display name. Display Name: {0}",
                            newUser.DisplayName), LogLevel.Error);
                    throw new DataException(string.Format("User name {0} is not available.", newUser.DisplayName));
                }
            }

            // check for matching user by email
            if (!string.IsNullOrWhiteSpace(newUser.EmailAddress))
            {
                userExisting = GetUserByEmailAddress(newUser.EmailAddress);
                if (userExisting.Id != default(Guid))
                {
                    InstaMelodyLogger.Log(
                        string.Format("Tried to create User with an existing email. Email: {0}",
                            newUser.EmailAddress), LogLevel.Error);
                    throw new DataException(string.Format("The email address {0} is already in use.", newUser.EmailAddress));
                }
            }

            // make a copy of the User object so we aren't editing the input argument
            var userCopy = newUser.Clone();

            // generate a random salt string for hashing the password
            var auth = new AuthenticationBll();
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
            if (addedUserGuid == default(Guid))
            {
                InstaMelodyLogger.Log(
                    string.Format("Error adding User. Display Name: {0}, Email: {1}",
                        newUser.DisplayName, newUser.EmailAddress), LogLevel.Error);
                throw new Exception(string.Format("Error adding user: {0}", newUser.DisplayName));
            }
            var addedUser = dal.FindById(addedUserGuid);
            if (addedUser == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Error adding User. Display Name: {0}, Email: {1}",
                        newUser.DisplayName, newUser.EmailAddress), LogLevel.Error);
                throw new Exception(string.Format("Error adding user: {0}", newUser.DisplayName));
            }

            // remove sensitive information
            addedUser = addedUser.StripSensitiveInfo();

            return GetUserWithImage(addedUser);
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
            var sessionUser = Utilities.GetUserBySession(sessionToken);

            var unifiedUserData = Utilities.UpdateUserObject(sessionUser, withUserObject: userToUpdate);

            // check for validation errors
            var validationErrors = ModelUtilities.Validate(unifiedUserData, ignorePassword: true).ToList();
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
            var userExisting = GetUserByDisplayName(unifiedUserData.DisplayName);
            if (userExisting != null && !userExisting.Id.Equals(unifiedUserData.Id))
            {
                InstaMelodyLogger.Log(
                    string.Format("Tried to update User with an existing display name. Display Name: {0}",
                        unifiedUserData.DisplayName), LogLevel.Error);
                throw new DataException(string.Format("User name {0} is not available.", unifiedUserData.DisplayName));
            }

            userExisting = GetUserByEmailAddress(unifiedUserData.EmailAddress);
            if (userExisting != null && !userExisting.Id.Equals(unifiedUserData.Id))
            {
                InstaMelodyLogger.Log(
                    string.Format("Tried to update User with an existing email. Email: {0}",
                        unifiedUserData.EmailAddress), LogLevel.Error);
                throw new DataException(string.Format("The email address {0} is already in use.", unifiedUserData.EmailAddress));
            }

            var dal = new Users();
            unifiedUserData.DateModified = DateTime.UtcNow;
            var updatedUser = dal.UpdateUser(sessionUser.Id, unifiedUserData);
            if (updatedUser == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Faled to update User. User Id: {0}", 
                        sessionUser.Id), LogLevel.Error);
                throw new DataException(string.Format("Could not update User: {0}", sessionUser.Id));
            }

            // remove sensitive information
            updatedUser = updatedUser.StripSensitiveInfo();
            return GetUserWithImage(updatedUser);
        }

        /// <summary>
        /// Updates the user image.
        /// </summary>
        /// <param name="newImage">The new image.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.ArgumentException">
        /// Image not provided.
        /// or
        /// Could not find user.
        /// </exception>
        public ApiUserFileUpload UpdateUserImage(Image newImage, Guid sessionToken)
        {
            User sessionUser;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(string.Format("Could not find a valid session. Token: {0}.", sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (newImage == null
                || string.IsNullOrWhiteSpace(newImage.FileName))
            {
                InstaMelodyLogger.Log(
                    string.Format("Image not provided. Failed to Update User Image. User Id: {0}",
                        sessionUser.Id), LogLevel.Error);
                throw new ArgumentException("Image not provided.");
            }

            var userToUpdate = sessionUser.Clone();

            // delete existing profile image
            var imageBll = new FileBll();
            if (userToUpdate.UserImageId != null && !userToUpdate.UserImageId.Equals(default(int)))
            {
                imageBll.DeleteImage(new Image
                {
                    Id = (int)userToUpdate.UserImageId
                });
            }

            // add new image
            var addedImage = imageBll.AddImage(newImage);
            userToUpdate.UserImageId = addedImage.Id;

            // update user with image id
            var updatedUser = UpdateUserProfileImage(userToUpdate);
            updatedUser.Image = addedImage;

            // create file upload token
            var uploadBll = new FileBll();
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
            User sessionUser;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(string.Format("Could not find a valid session. Token: {0}.", sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            var foundUser = FindUser(user);
            if (foundUser == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("User not found. User Id: {0}",
                        user.Id), LogLevel.Error);
                throw new DataException("User not found.");
            }

            if (!sessionUser.Id.Equals(foundUser.Id))
            {
                InstaMelodyLogger.Log(
                    string.Format("User tried to delete another User's profile. User Id: {0}, Session User Id: {1}",
                        foundUser.Id, sessionUser.Id), LogLevel.Error);
                throw new UnauthorizedAccessException("A user can only delete their own profile.");
            }

            var dal = new Users();
            dal.DeleteUser(foundUser.Id);
            foundUser.IsDeleted = true;

            // remove sensitive information
            foundUser = foundUser.StripSensitiveInfo();

            // end and delete all sessions
            var auth = new AuthenticationBll();
            auth.DeleteAllUserSessions(foundUser.Id);

            // delete profile image
            if (foundUser.UserImageId != null)
            {
                var imageBll = new FileBll();
                imageBll.DeleteImage(new Image
                {
                    Id = (int)foundUser.UserImageId
                });
            }

            return foundUser;
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
            User sessionUser;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(string.Format("Could not find a valid session. Token: {0}.", sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (friend == null
                || (friend.Id.Equals(default(Guid))
                    && string.IsNullOrWhiteSpace(friend.DisplayName)
                    && string.IsNullOrWhiteSpace(friend.EmailAddress)))
            {
                InstaMelodyLogger.Log(string.Format("No friend information provided. Token: {0}", sessionToken), LogLevel.Error);
                throw new DataException("No friend information provided.");
            }

            // try to request friend by id, email, or display name
            var requestedFriend = FindUser(friend);
            if (requestedFriend == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not find requested friend in database. Token: {0}, Friend Id: {1}, Friend Display Name: {2}, Friend Email: {3}", 
                        sessionToken, friend.Id, friend.DisplayName, friend.EmailAddress), LogLevel.Error);
                throw new DataException("Could not find requested friend in database.");
            }

            // make friend request
            var friendDal = new UserFriends();
            friendDal.RequestFriend(sessionUser.Id, requestedFriend.Id);

            // TODO: send push notification to friend
            //Utilities.SendPushNotification(requestedFriend.Id);

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
            User sessionUser;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(string.Format("Could not find a valid session. Token: {0}.", sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (requestor == null
                || (requestor.Id.Equals(default(Guid))
                    && string.IsNullOrWhiteSpace(requestor.DisplayName)
                    && string.IsNullOrWhiteSpace(requestor.EmailAddress)))
            {
                InstaMelodyLogger.Log(
                    string.Format("No friend information provided to approve. Token: {0}.",
                        sessionToken), LogLevel.Error);
                throw new DataException("No friend information provided to approve.");
            }

            // try to request friend by id, email, or display name
            var requestingFriend = FindUser(requestor);
            if (requestingFriend == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not find requested friend in database. Token: {0}, Friend Id: {1}, Friend Display Name: {2}, Friend Email: {3}",
                        sessionToken, requestor.Id, requestor.DisplayName, requestor.EmailAddress), LogLevel.Error);
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
                InstaMelodyLogger.Log(
                    string.Format("User not authorized to approve friend request. User Id: {0}, Friend Id: {1}", 
                        sessionUser.Id, requestingFriend.Id), LogLevel.Error);
                throw new DataException(string.Format("User: {0} not authorized to execute request.", sessionUser.Id));
            }

            requestingFriend = requestingFriend.StripSensitiveInfoForFriends();
            return GetUserWithImage(requestingFriend);
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
            User sessionUser;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(string.Format("Could not find a valid session. Token: {0}.", sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (requestor == null
                || (requestor.Id.Equals(default(Guid))
                    && string.IsNullOrWhiteSpace(requestor.DisplayName)
                    && string.IsNullOrWhiteSpace(requestor.EmailAddress)))
            {
                InstaMelodyLogger.Log(
                    string.Format("No friend information provided to deny. Token: {0}.", 
                        sessionToken), LogLevel.Error);
                throw new DataException("No friend information provided to deny.");
            }

            // try to request friend by id, email, or display name
            var requestingFriend = FindUser(requestor);
            if (requestingFriend == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not find requested friend in database. Token: {0}, Friend Id: {1}, Friend Display Name: {2}, Friend Email: {3}",
                        sessionToken, requestor.Id, requestor.DisplayName, requestor.EmailAddress), LogLevel.Error);
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
                InstaMelodyLogger.Log(
                    string.Format("User not authorized to deny friend request. User Id: {0}, Friend Id: {1}",
                        sessionUser.Id, requestingFriend.Id), LogLevel.Error);
                throw new DataException(string.Format("User: {0} not authorized to execute request.", sessionUser.Id));
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
            User sessionUser;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(string.Format("Could not find a valid session. Token: {0}", sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (friend == null
                || (friend.Id.Equals(default(Guid))
                    && string.IsNullOrWhiteSpace(friend.DisplayName)
                    && string.IsNullOrWhiteSpace(friend.EmailAddress)))
            {
                InstaMelodyLogger.Log(string.Format("No friend information provided to delete. Token: {0}", sessionToken), LogLevel.Error);
                throw new DataException("No friend information provided to delete.");
            }

            // try to request friend by id, email, or display name
            User requestingFriend = FindUser(friend);
            if (requestingFriend == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not find requested friend in database. Token: {0}, Friend Id: {1}, Friend Display Name: {2}, Friend Email: {3}",
                        sessionToken, friend.Id, friend.DisplayName, friend.EmailAddress), LogLevel.Error);
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
            User sessionUser;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(string.Format("Could not find a valid session. Token: {0}.", sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            var dal = new UserFriends();
            var friends = dal.GetUserFriends(sessionUser.Id);
            if (friends == null || !friends.Any())
            {
                InstaMelodyLogger.Log(string.Format("Could not find friends for User: {0}", sessionUser.Id), LogLevel.Error);
                throw new DataException(string.Format("Could not find friends for User: {0}", sessionUser.Id));
            }

            foreach (var friend in friends)
            {
                friend.StripSensitiveInfoForFriends();
            }

            return GetUsersWithImages(friends);
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
            User sessionUser;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(string.Format("Could not find a valid session. Token: {0}.", sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            var dal = new UserFriends();
            var friends = dal.GetPendingFriends(sessionUser.Id);
            if (friends == null || !friends.Any())
            {
                InstaMelodyLogger.Log(string.Format("Could not find pending friends for User: {0}", sessionUser.Id), LogLevel.Error);
                throw new DataException(string.Format("Could not find pending friends for User: {0}", sessionUser.Id));
            }

            foreach (var friend in friends)
            {
                friend.StripSensitiveInfoForFriends();
            }

            return GetUsersWithImages(friends);
        }

        #endregion Public Methods
        
        #region Private Methods

        /// <summary>
        /// Updates the user image.
        /// </summary>
        /// <param name="userToUpdate">The user to update.</param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException">
        /// </exception>
        /// <exception cref="DataException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.Data.DataException"></exception>
        private User UpdateUserProfileImage(User userToUpdate)
        {
            var dal = new Users();
            var updatedUser = dal.UpdateUserProfileImage(userToUpdate.Id, userToUpdate.UserImageId);
            if (updatedUser == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not update profile image. User Id: {0}",
                        userToUpdate.Id), LogLevel.Error);
                throw new DataException(string.Format("Could not update profile image for User: {0}", userToUpdate.Id));
            }

            // remove sensitive information
            updatedUser = updatedUser.StripSensitiveInfo();
            return GetUserWithImage(updatedUser);
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
                var imageBll = new FileBll();
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
                results = users.Select(GetUserWithImage).ToList();
            }

            return results;
        }

        #endregion Private Methods

        #region Internal Methods

        /// <summary>
        /// Deletes the user image.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <exception cref="System.ArgumentException">User cannot be found.</exception>
        internal void DeleteUserImage(Guid userId)
        {
            var dal = new Users();
            var user = dal.FindById(userId);
            if (user == null)
            {
                InstaMelodyLogger.Log(string.Format("User cannot be found. User Id: {0}", userId), LogLevel.Error);
                throw new ArgumentException("User cannot be found.");
            }

            dal.UpdateUserProfileImage(userId, null);
        }

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        internal User FindUser(User user)
        {
            User foundUser = null;

            if (!user.Id.Equals(default(Guid)))
            {
                var bll = new Users();
                foundUser = bll.FindById(user.Id);
            }

            if (foundUser == null && !string.IsNullOrWhiteSpace(user.DisplayName))
            {
                foundUser = GetUserByDisplayName(user.DisplayName);
            }
            else if (foundUser == null && !string.IsNullOrWhiteSpace(user.EmailAddress))
            {
                foundUser = GetUserByEmailAddress(user.EmailAddress);
            }

            return foundUser;
        }

        #endregion Internal Methods
    }
}
