using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using InstaMelody.Data;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Business.Properties;
using InstaMelody.Infrastructure;
using NLog;

using ModelUtilities = InstaMelody.Model.Utilities;

namespace InstaMelody.Business
{
    public class AuthenticationBll
    {
        #region Public Methods

        /// <summary>
        /// Authenticates the specified user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="password">The password.</param>
        /// <param name="deviceToken">The device token.</param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException">
        /// No User / Password Match Was Found
        /// or
        /// No User / Password Match Was Found
        /// </exception>
        /// <exception cref="DataException">
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">No User / Password Match Was Found</exception>
        /// <exception cref="System.Data.DataException"></exception>
        public ApiToken Authenticate(string userName, string emailAddress, string password, string deviceToken)
        {
            User existingUser = null;
            var userDal = new Users();

            // try to find user by display name
            if (!string.IsNullOrWhiteSpace(userName))
            {
                existingUser = userDal.FindByDisplayName(userName);
            }

            // if no user was found with that display name, try the email address
            if (existingUser == null && !string.IsNullOrWhiteSpace(emailAddress))
            {
                existingUser = userDal.FindByEmail(emailAddress);
            }

            // if no user was found, then eject!
            if (existingUser == null || existingUser.IsDeleted)
            {
                InstaMelodyLogger.Log(
                    string.Format("No User / Password Match Was Found. UserName: {0}, Email: {1}, Password: {2}", 
                        userName, emailAddress, password), LogLevel.Error);
                throw new UnauthorizedAccessException("No User / Password Match Was Found");
            }

            // check for a password match
            if (!DoesPasswordMatch(existingUser, password))
            {
                FailedLogin(existingUser);
                InstaMelodyLogger.Log(
                    string.Format("Password mismatch. UserName: {0}, Email: {1}, Password: {2}",
                        userName, emailAddress, password), LogLevel.Error);
                throw new UnauthorizedAccessException("No User / Password Match Was Found");
            }

            // everything looks OK, so update the user and create a session token
            var credentialedUser = userDal.SuccessfulUserLogin(existingUser);
            if (credentialedUser == null)
            {
                InstaMelodyLogger.Log(
                       string.Format("Error validating credentials. UserName: {0}, Email: {1}, Password: {2}",
                           userName, emailAddress, password), LogLevel.Error);
                throw new DataException(string.Format("Error validating credentials for user: {0}", userName));
            }

            var sessionDal = new UserSessions();
            //sessionDal.EndAllSessionsByUserId(credentialedUser.Id);
            var sessionToken = sessionDal.AddSession(credentialedUser.Id, deviceToken);
            if (!sessionToken.Equals(default(Guid)))
            {
                return new ApiToken { Token = sessionToken };
            }

            InstaMelodyLogger.Log(
                string.Format("Error creating session. UserName: {0}, Email: {1}, Password: {2}",
                    userName, emailAddress, password), LogLevel.Error);
            throw new DataException(string.Format("Error creating session for user: {0}", userName));
        }

        /// <summary>
        /// Authenticates the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="facebookId">The facebook identifier.</param>
        /// <param name="twitterId">The twitter identifier.</param>
        /// <param name="deviceToken">The device token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">No User found with the provided information.</exception>
        /// <exception cref="System.Data.DataException">
        /// </exception>
        public ApiToken Authenticate(string facebookId, string twitterId, string deviceToken)
        {
            User existingUser = null;
            var userDal = new Users();

            // find user by facebook token
            if (!string.IsNullOrWhiteSpace(facebookId))
            {
                existingUser = userDal.FindByFacebookId(facebookId);
            }

            // if no user was found with the facebook token, try the twitter token
            if (existingUser == null && !string.IsNullOrWhiteSpace(twitterId))
            {
                existingUser = userDal.FindByTwitterId(twitterId);
            }

            // if no user was found, then eject!
            if (existingUser == null || existingUser.IsDeleted)
            {
                InstaMelodyLogger.Log(
                    string.Format("No User / Token Match Was Found. Facebook Id: {0}, Twitter Id: {1}",
                        facebookId, twitterId), LogLevel.Error);
                throw new UnauthorizedAccessException("No User found with the provided information.");
            }

            // everything looks OK, so update the user and create a session token
            var credentialedUser = userDal.SuccessfulUserLogin(existingUser);
            if (credentialedUser == null)
            {
                InstaMelodyLogger.Log(
                       string.Format("Error validating credentials. Id: {0}, Facebook Id: {1}, Twitter Id: {2}",
                           facebookId, twitterId), LogLevel.Error);
                throw new DataException("Error validating credentials.");
            }

            var sessionDal = new UserSessions();
            //sessionDal.EndAllSessionsByUserId(credentialedUser.Id);
            var sessionToken = sessionDal.AddSession(credentialedUser.Id, deviceToken);
            if (!sessionToken.Equals(default(Guid)))
            {
                return new ApiToken { Token = sessionToken };
            }

            InstaMelodyLogger.Log(
                string.Format("Error creating session. Id: {0}, Facebook Id: {1}, Twitter Id: {2}",
                           facebookId, twitterId), LogLevel.Error);
            throw new DataException("Error validating credentials.");
        }

        /// <summary>
        /// Authenticates the with token.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="facebookToken">The facebook token.</param>
        /// <param name="twitterToken">The twitter token.</param>
        /// <param name="deviceToken">The device token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">No User found with the provided information.</exception>
        /// <exception cref="System.Data.DataException">
        /// </exception>
        public ApiToken AuthenticateWithToken(Guid userId, string facebookToken, string twitterToken, string deviceToken)
        {
            User existingUser = null;
            var userDal = new Users();

            // find user by facebook token
            if (!string.IsNullOrWhiteSpace(facebookToken))
            {
                existingUser = userDal.FindByFacebookToken(facebookToken);
            }

            // if no user was found with the facebook token, try the twitter token
            if (existingUser == null && !string.IsNullOrWhiteSpace(twitterToken))
            {
                existingUser = userDal.FindByTwitterToken(twitterToken);
            }

            // if no user was found, then eject!
            if (existingUser == null || existingUser.IsDeleted || !existingUser.Id.Equals(userId))
            {
                InstaMelodyLogger.Log(
                    string.Format("No User / Token Match Was Found. Id: {0}, Facebook Token: {1}, Twitter Token: {2}",
                        userId, facebookToken, twitterToken), LogLevel.Error);
                throw new UnauthorizedAccessException("No User found with the provided information.");
            }

            // everything looks OK, so update the user and create a session token
            var credentialedUser = userDal.SuccessfulUserLogin(existingUser);
            if (credentialedUser == null)
            {
                InstaMelodyLogger.Log(
                       string.Format("Error validating credentials. Id: {0}, Facebook Token: {1}, Twitter Token: {2}",
                           userId, facebookToken, twitterToken), LogLevel.Error);
                throw new DataException(string.Format("Error validating credentials for user: {0}", userId));
            }

            var sessionDal = new UserSessions();
            //sessionDal.EndAllSessionsByUserId(credentialedUser.Id);
            var sessionToken = sessionDal.AddSession(credentialedUser.Id, deviceToken);
            if (!sessionToken.Equals(default(Guid)))
            {
                return new ApiToken { Token = sessionToken };
            }

            InstaMelodyLogger.Log(
                string.Format("Error creating session. Id: {0}, Facebook Token: {1}, Twitter Token: {2}",
                           userId, facebookToken, twitterToken), LogLevel.Error);
            throw new DataException(string.Format("Error creating session for user: {0}", userId));
        }

        /// <summary>
        /// Validates the session.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="deviceToken">The device token.</param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        public ApiToken ValidateSession(Guid sessionToken, string deviceToken)
        {
            var dal = new UserSessions();
            var sessions = dal.FindActiveSessions(deviceToken);

            // find the session being requested
            var session = sessions.FirstOrDefault(s => s.Token.Equals(sessionToken) && s.DeviceToken.Equals(deviceToken));
            if (session == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot validate session token. Token: {0}, Device Token: {1}",
                        sessionToken, deviceToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Cannot validate session token: {0}.", sessionToken));
            }

            dal.UpdateSessionActivity(session.Token);

            return new ApiToken { Token = session.Token };
        }

        /// <summary>
        /// Validates the session.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public ApiToken ValidateSession(Guid userId, Guid sessionToken)
        {
            var dal = new UserSessions();
            var sessions = dal.FindActiveSessions(userId);

            // find the session being requested
            var session = sessions.FirstOrDefault(s => s.Token.Equals(sessionToken));
            if (session == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot validate session token. User Id: {0}, Token: {1},",
                        userId, sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Cannot validate session token: {0}.", sessionToken));
            }

            dal.UpdateSessionActivity(session.Token);

            return new ApiToken { Token = session.Token };
        }

        /// <summary>
        /// Ends the session.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="deviceToken">The device token.</param>
        /// <exception cref="UnauthorizedAccessException">User is not authorized to end session for this User.</exception>
        /// <exception cref="DataException">
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.Data.DataException"></exception>
        public void EndSession(Guid sessionToken, string deviceToken)
        {
            var requestor = Utilities.GetUserBySession(sessionToken);
            if (requestor == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Invalid requestor. Device Token: {0}, Token: {1}",
                        deviceToken, sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Invalid requestor Session: {0}.", sessionToken));
            }

            if (!requestor.DeviceToken.Equals(deviceToken))
            {
                InstaMelodyLogger.Log(
                    string.Format("User is not authorized to end session for User. Device Token: {0}, Requestor Id: {1}, Token: {2}",
                        deviceToken, requestor.Id, sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException("User is not authorized to end session for this User.");
            }

            var dal = new UserSessions();
            var foundSession = GetSession(sessionToken);
            if (foundSession == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Error finding Session Id for User. Device Token: {0}, Token: {1}",
                        deviceToken, sessionToken), LogLevel.Error);
                throw new DataException(string.Format("Error finding Session Id {0} for User.", sessionToken));
            }

            dal.EndSession(foundSession.Token, deviceToken);
            var checkSession = GetSession(foundSession.Token);
            if (checkSession.Token != default(Guid))
            {
                InstaMelodyLogger.Log(
                    string.Format("Error finding Session Id for User. Device Token: {0}, Token: {1}",
                        deviceToken, sessionToken), LogLevel.Error);
                throw new DataException(string.Format("Error ending Session Id {0} for User.", sessionToken));
            }
        }

        /// <summary>
        /// Ends the session.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <exception cref="UnauthorizedAccessException">
        /// User is not authorized to end session for this User.
        /// </exception>
        /// <exception cref="DataException">
        /// </exception>
        public void EndSession(Guid userId, Guid sessionToken)
        {
            var requestor = Utilities.GetUserBySession(sessionToken);
            if (requestor == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Invalid requestor. User Id: {0}, Token: {1}",
                        userId, sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Invalid requestor Session: {0}.", sessionToken));
            }

            if (!requestor.Id.Equals(userId))
            {
                InstaMelodyLogger.Log(
                    string.Format("User is not authorized to end session for User. User Id: {0}, Requestor Id: {1}, Token: {2}",
                        userId, requestor.Id, sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException("User is not authorized to end session for this User.");
            }

            var dal = new UserSessions();
            var foundSession = GetSession(sessionToken);
            if (foundSession == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Error finding Session Id for User. User Id: {0}, Token: {1}",
                        userId, sessionToken), LogLevel.Error);
                throw new DataException(string.Format("Error finding Session Id {0} for User.", sessionToken));
            }

            dal.EndSession(userId, foundSession.Token);
            var checkSession = GetSession(foundSession.Token);
            if (checkSession.Token != default(Guid))
            {
                InstaMelodyLogger.Log(
                    string.Format("Error finding Session Id for User. User Id: {0}, Token: {1}",
                        userId, sessionToken), LogLevel.Error);
                throw new DataException(string.Format("Error ending Session Id {0} for User.", sessionToken));
            }
        }

        /// <summary>
        /// Updates the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">Provided old password value does not match. Password update failed.</exception>
        /// <exception cref="System.Data.DataException">Both old and new password values must be provided to reset the user password.</exception>
        public ApiToken UpdatePassword(UserPassword password, Guid sessionToken)
        {
            User sessionUser;
            try
            {
                sessionUser = Utilities.GetUserBySession(sessionToken, includeSensitiveInfo: true);
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(
                    string.Format("Could not find a valid Session. Token: {0}",
                        sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (string.IsNullOrWhiteSpace(password.OldPassword)
                || string.IsNullOrWhiteSpace(password.NewPassword))
            {
                InstaMelodyLogger.Log(
                    string.Format("Both old and new password values must be provided to reset the user password. Token: {0}",
                        sessionToken), LogLevel.Error);
                throw new DataException("Both old and new password values must be provided to reset the user password.");
            }

            var hashOldPassword = HashSaltedPassword(sessionUser.HashSalt, password.OldPassword);
            if (!hashOldPassword.Equals(sessionUser.Password))
            {
                InstaMelodyLogger.Log(
                    string.Format("Provided old password value does not match. Password update failed. Token: {0}",
                        sessionToken), LogLevel.Error);
                throw new UnauthorizedAccessException("Provided old password value does not match. Password update failed.");
            }

            var dal = new Users();

            // update user password
            var newHashPassword = HashSaltedPassword(sessionUser.HashSalt, password.NewPassword);
            var updatedUser = dal.UpdateUserPassword(sessionUser.Id, newHashPassword);

            // end all existing sessions
            DeleteAllUserSessions(updatedUser.Id);

            // login user
            return Authenticate(updatedUser.DisplayName, updatedUser.EmailAddress, password.NewPassword, sessionUser.DeviceToken);
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <exception cref="ArgumentException">Cannot find the requested User.</exception>
        public User ResetPassword(User user)
        {
            var userBll = new UserBll();
            var foundUser = userBll.FindUser(user);
            if (foundUser == null)
            {
                InstaMelodyLogger.Log(
                    string.Format("Cannot find the requested User. User Id: {0}, User Name: {1}, Email Address: {2}",
                        user.Id, user.DisplayName, user.EmailAddress), LogLevel.Error);
                throw new ArgumentException("Cannot find the requested User.");
            }

            // generate temp password and email to user
            var tempPw = GenerateTempPassword();
            SendTempPasswordEmail(foundUser, tempPw);

            var dal = new Users();

            // unlock user account
            dal.UnlockUserAccount(foundUser.Id);

            // update user password to temp password
            var hashTempPw = HashSaltedPassword(foundUser.HashSalt, tempPw);
            dal.UpdateUserPassword(foundUser.Id, hashTempPw);

            // end all existing sessions
            DeleteAllUserSessions(foundUser.Id);

            return foundUser;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Faileds the login.
        /// </summary>
        /// <param name="user">The user.</param>
        private void FailedLogin(User user)
        {
            var dal = new Users();
            dal.FailedUserLogin(user);

            var retrievedUser = dal.FindById(user.Id);
            if (retrievedUser.NumberLoginFailures >= Settings.Default.MaxFailedLogins)
            {
                LockUserAccount(user);
            }
        }

        /// <summary>
        /// Locks the user account.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <exception cref="System.UnauthorizedAccessException">User account has been locked due to concecutive failed login attempts</exception>
        private void LockUserAccount(User user)
        {
            var dal = new Users();
            dal.LockUserAccount(user.Id);

            InstaMelodyLogger.Log(
                string.Format("User account has been locked due to {0} concecutive failed login attempt. User Id: {1}",
                    Settings.Default.MaxFailedLogins, user.Id), LogLevel.Error);

            throw new UnauthorizedAccessException(
                string.Format("User account has been locked due to {0} concecutive failed login attempts. Please request a new password.", 
                    Settings.Default.MaxFailedLogins));
        }

        /// <summary>
        ///     The generate temp password.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private string GenerateTempPassword()
        {
            return GenerateSalt(Settings.Default.TempPasswordLength);
        }

        /// <summary>
        /// Sends the temporary password email.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="tempPassword">The temporary password.</param>
        /// <exception cref="ArgumentException">Cannot send email to a NULL User.</exception>
        private void SendTempPasswordEmail(User user, string tempPassword)
        {
            if (user == null || string.IsNullOrEmpty(user.EmailAddress))
            {
                InstaMelodyLogger.Log("Cannot send email to a NULL User.", LogLevel.Error);
                throw new ArgumentException("Cannot send email to a NULL User.");
            }

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("Greetings {0},", user.DisplayName));
            sb.AppendLine("");
            sb.AppendLine("Here is your new InstaMelody password:");
            sb.AppendLine(string.Format("<strong>{0}</strong>", tempPassword));
            sb.AppendLine("");
            sb.AppendLine("Please login using your temporary password to use InstaMelody.");
            sb.AppendLine("");
            sb.AppendLine("Sincerely,");
            sb.AppendLine("The InstaMelody Team");

            try
            {
                Utilities.SendEmail(
                    user.EmailAddress, 
                    new MailAddress("noreply@instamelody.com", "InstaMelody"), 
                    "Your InstaMelody Temporary Password",
                    sb.ToString().Replace("\n", "<br />"));
            }
            catch (Exception)
            {
                InstaMelodyLogger.Log(
                    string.Format("Failed to send Temporary Password Email. Email Address: {0}, Email Body: {1}",
                        user.EmailAddress, sb),
                    LogLevel.Fatal);

                throw new Exception("Failed to send Temporary Password Email.");
            }
        }

        #endregion Private Methods

        #region Internal Methods

        /// <summary>
        /// The generate salt.
        /// </summary>
        /// <param name="saltLength">
        /// The salt length.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string GenerateSalt(int saltLength)
        {
            var salt = string.Empty;

            // loop once for each character in the salt string length
            for (var index = 0; index < saltLength; index++)
            {
                // start with a value 0 - 129
                var rand = Math.Abs(Infrastructure.Utilities.GenerateRandomInt(Constants.IntSize)) % 130;

                // an even number = letter; an odd number = digit
                if (rand % 2 == 0)
                {
                    // random value / 5 = 0 - 25 range (alphabet size)
                    var newCharIndex = rand / 5;

                    // apply an the alphabet start as an index offset
                    // this gives us the index of the character we want
                    var newCharUnicode = newCharIndex + Constants.AlphabetStart;

                    // get the unicode character for that index
                    var newChar = Convert.ToChar(newCharUnicode);

                    // append the new random latter to the salt string.
                    salt += newChar.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    // random value / 13 = 0 - 9 range (digit to add)
                    var newDigit = rand / 13;

                    // append the new digit (after converting to a string) to the salt string
                    salt += newDigit.ToString(CultureInfo.InvariantCulture);
                }
            }

            return salt;
        }

        /// <summary>
        /// Doeses the password match.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        internal bool DoesPasswordMatch(User user, string password)
        {
            var hashedPassword = HashSaltedPassword(user.HashSalt, password);
            var result = hashedPassword.Equals(user.Password);

            return result;
        }

        /// <summary>
        /// Hashes the salted password.
        /// </summary>
        /// <param name="salt">The salt.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        internal string HashSaltedPassword(string salt, string password)
        {
            InstaMelodyLogger.Log(string.Format("STARTING HASH FUNCTION - Salt: {0} | Password: {1}", salt, password), LogLevel.Trace);

            var saltToUse = salt;
            if (string.IsNullOrWhiteSpace(saltToUse))
            {
                saltToUse = GenerateSalt(Settings.Default.SaltLength);
            }

            var saltedPassword = saltToUse + password;
            InstaMelodyLogger.Log(string.Format(" - SaltedPassword: {0}", saltedPassword), LogLevel.Trace);
            var saltedPasswordBytes = Infrastructure.Utilities.GetBytes(saltedPassword);
            InstaMelodyLogger.Log(string.Format(" - SaltedPasswordBytes: {0}", Infrastructure.Utilities.ByteArrayToString(saltedPasswordBytes)), LogLevel.Trace);

            var sha256 = SHA256.Create();
            var hashedPasswordBytes = sha256.ComputeHash(saltedPasswordBytes);
            InstaMelodyLogger.Log(string.Format(" - HashedPasswordBytes: {0}", Infrastructure.Utilities.ByteArrayToString(hashedPasswordBytes)), LogLevel.Trace);
            var hashedSaltedPassword = Convert.ToBase64String(hashedPasswordBytes);
            InstaMelodyLogger.Log(string.Format(" - Base64String: {0}", hashedSaltedPassword), LogLevel.Trace);
            InstaMelodyLogger.Log(string.Format("Salt: {0} | Password: {1} | Hash: {2}", saltToUse, password, hashedSaltedPassword), LogLevel.Trace);

            return hashedSaltedPassword;
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        internal UserSession GetSession(Guid sessionToken)
        {
            var dal = new UserSessions();
            var userSession = dal.FindByToken(sessionToken) ?? new UserSession();
            return userSession;
        }

        /// <summary>
        /// Deletes all user sessions.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        internal void DeleteAllUserSessions(Guid userId)
        {
            var dal = new UserSessions();
            dal.EndAllSessionsByUserId(userId);
        }

        #endregion Internal Methods
    }
}
