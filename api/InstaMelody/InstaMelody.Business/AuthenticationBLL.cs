using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;

using InstaMelody.Data;
using InstaMelody.Model;
using InstaMelody.Model.ApiModels;
using InstaMelody.Business.Properties;
using InstaMelody.Infrastructure;
using NLog;

using ModelUtilities = InstaMelody.Model.Utilities;

namespace InstaMelody.Business
{
    public class AuthenticationBLL
    {
        #region Public Methods

        /// <summary>
        /// Authenticates the specified user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">No User / Password Match Was Found</exception>
        /// <exception cref="System.Data.DataException">
        /// </exception>
        public ApiToken Authenticate(string userName, string emailAddress, string password)
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
                throw new UnauthorizedAccessException("No User / Password Match Was Found");
            }

            // check for a password match
            if (!this.DoesPasswordMatch(existingUser, password))
            {
                FailedLogin(existingUser);
                throw new UnauthorizedAccessException("No User / Password Match Was Found");
            }

            // everything looks OK, so update the user and create a session token
            var credentialedUser = userDal.SuccessfulUserLogin(existingUser);
            if (credentialedUser == null)
            {
                throw new DataException(string.Format("Error validating credentials for user: {0}", userName));
            }

            var sessionDal = new UserSessions();
            sessionDal.EndAllSessionsByUserId(credentialedUser.Id);
            var sessionToken = sessionDal.AddSession(credentialedUser.Id);
            if (sessionToken == null)
            {
                throw new DataException(string.Format("Error creating session for user: {0}", userName));
            }

            // return the user with the session token
            return new ApiToken
            {
                Token = sessionToken
            };
        }

        /// <summary>
        /// Validates the session.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        public ApiToken ValidateSession(Guid userId, Guid sessionToken)
        {
            var dal = new UserSessions();
            var sessions = dal.FindActiveSessionsByUserId(userId);

            // find the session being requested
            var session = sessions.FirstOrDefault(s => s.Token.Equals(sessionToken));
            if (session == null)
            {
                throw new UnauthorizedAccessException(string.Format("Cannot validate session token {0} for User {1}", sessionToken, userId));
            }

            dal.UpdateSessionActivity(session.Token);

            return new ApiToken
            {
                Token = session.Token
            };
        }

        /// <summary>
        /// Ends the session.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <exception cref="System.UnauthorizedAccessException">
        /// </exception>
        /// <exception cref="System.Data.DataException">
        /// </exception>
        public void EndSession(Guid userId, Guid sessionToken)
        {
            var requestor = Utilities.GetUserBySession(sessionToken);
            if (requestor == null)
            {
                throw new UnauthorizedAccessException(string.Format("Invalid requestor Session: {0}.", sessionToken));
            }
            else if (requestor.Id != userId)
            {
                throw new UnauthorizedAccessException(string.Format("User {0} is not authorized to end session for User {1}.", requestor.Id, userId));
            }

            var dal = new UserSessions();
            var foundSession = this.GetSession(sessionToken);
            if (foundSession == null)
            {
                throw new DataException(string.Format("Error finding Session Id {0} for User {1}", sessionToken, userId));
            }

            dal.EndSession(foundSession.Token);
            var checkSession = this.GetSession(foundSession.Token);
            if (checkSession.Token != default(Guid))
            {
                throw new DataException(string.Format("Error ending Session Id {0} for User {1}", sessionToken, userId));
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
                sessionUser = Utilities.GetUserBySession(sessionToken);
            }
            catch (Exception)
            {
                throw new UnauthorizedAccessException(string.Format("Could not find a valid session for Session: {0}.", sessionToken));
            }

            if (string.IsNullOrWhiteSpace(password.OldPassword)
                || string.IsNullOrWhiteSpace(password.NewPassword))
            {
                throw new DataException("Both old and new password values must be provided to reset the user password.");
            }

            var hashOldPassword = this.HashSaltedPassword(sessionUser.HashSalt, password.OldPassword);
            if (!hashOldPassword.Equals(sessionUser.Password))
            {
                throw new UnauthorizedAccessException("Provided old password value does not match. Password update failed.");
            }

            var dal = new Users();

            // update user password
            var newHashPassword = this.HashSaltedPassword(sessionUser.HashSalt, password.NewPassword);
            var updatedUser = dal.UpdateUserPassword(sessionUser.Id, newHashPassword);
            if (!updatedUser.Password.Equals(newHashPassword))
            {
                throw new DataException("Failed to update User password.");
            }

            // end all existing sessions
            this.DeleteAllUserSessions(updatedUser.Id);

            // login user
            return this.Authenticate(updatedUser.DisplayName, updatedUser.EmailAddress, updatedUser.Password);
        }

        public User ResetPassword(string userName)
        {
            // TODO:
            throw new NotImplementedException();
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
            this.DeleteAllUserSessions(user.Id);

            throw new UnauthorizedAccessException(
                string.Format("User account has been locked due to {0} concecutive failed login attempts", 
                    Settings.Default.MaxFailedLogins));
        }

        /// <summary>
        /// Unlocks the user account.
        /// </summary>
        /// <param name="user">The user.</param>
        private void UnlockUserAccount(User user)
        {
            var dal = new Users();
            dal.UnlockUserAccount(user.Id);
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
        private string ByteArrayToString(byte[] bytes)
        {
            int ndx = 0;
            string result = string.Empty;

            while (ndx < bytes.Length)
            {
                result += bytes[ndx].ToString(CultureInfo.InvariantCulture) + ",";
                ndx++;
            }

            return result;
        }

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
            string salt = string.Empty;

            // loop once for each character in the salt string length
            for (int index = 0; index < saltLength; index++)
            {
                // start with a value 0 - 129
                int rand = Math.Abs(this.GenerateRandomInt()) % 130;

                // an even number = letter; an odd number = digit
                if (rand % 2 == 0)
                {
                    // random value / 5 = 0 - 25 range (alphabet size)
                    int newCharIndex = rand / 5;

                    // apply an the alphabet start as an index offset
                    // this gives us the index of the character we want
                    int newCharUnicode = newCharIndex + Constants.AlphabetStart;

                    // get the unicode character for that index
                    char newChar = Convert.ToChar(newCharUnicode);

                    // append the new random latter to the salt string.
                    salt += newChar.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    // random value / 13 = 0 - 9 range (digit to add)
                    int newDigit = rand / 13;

                    // append the new digit (after converting to a string) to the salt string
                    salt += newDigit.ToString(CultureInfo.InvariantCulture);
                }
            }

            return salt;
        }

        /// <summary>
        ///     The generate random integer.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        private int GenerateRandomInt()
        {
            RandomNumberGenerator rand = RandomNumberGenerator.Create();

            var source = new byte[Constants.IntSize];
            var result = new int[1];

            rand.GetBytes(source);
            Buffer.BlockCopy(source, 0, result, 0, Constants.IntSize);

            return result[0];
        }

        /// <summary>
        /// The get bytes.
        /// </summary>
        /// <param name="inputString">The Input String.</param>
        /// <returns>
        /// The <see cref="byte" />.
        /// </returns>
        private static byte[] GetBytes(string inputString)
        {
            var bytes = new byte[inputString.Length * sizeof(char)];
            Buffer.BlockCopy(inputString.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        ///     The generate temp password.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private string GenerateTempPassword()
        {
            return this.GenerateSalt(Settings.Default.TempPasswordLength);
        }

        #endregion Private Methods

        #region Internal Methods

        /// <summary>
        /// Doeses the password match.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        internal bool DoesPasswordMatch(User user, string password)
        {
            var hashedPassword = this.HashSaltedPassword(user.HashSalt, password);
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
                saltToUse = this.GenerateSalt(Settings.Default.SaltLength);
            }

            var saltedPassword = saltToUse + password;
            InstaMelodyLogger.Log(string.Format(" - SaltedPassword: {0}", saltedPassword), LogLevel.Trace);
            var saltedPasswordBytes = GetBytes(saltedPassword);
            InstaMelodyLogger.Log(string.Format(" - SaltedPasswordBytes: {0}", this.ByteArrayToString(saltedPasswordBytes)), LogLevel.Trace);

            var sha256 = SHA256.Create();
            var hashedPasswordBytes = sha256.ComputeHash(saltedPasswordBytes);
            InstaMelodyLogger.Log(string.Format(" - HashedPasswordBytes: {0}", this.ByteArrayToString(hashedPasswordBytes)), LogLevel.Trace);
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
            var userSession = dal.FindByToken(sessionToken);
            if (userSession == null)
            {
                userSession = new UserSession();
            }
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
