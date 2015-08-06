using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class Users : DataAccess
    {
        /// <summary>
        /// Adds a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Guid AddUser(User user)
        {
            user.Id = Guid.NewGuid();

            var query = @"INSERT INTO dbo.Users (Id, UserImageId, EmailAddress, DisplayName, FirstName, LastName, PhoneNumber, IsFemale,
                        HashSalt, Password, TwitterUsername, TwitterUserId, TwitterToken, TwitterSecret, FacebookUserId, 
                        FacebookToken, LastLoginSuccess, LastLoginFailure, NumberLoginFailures, IsLocked,
                        DateCreated, DateModified, IsDeleted)
                        VALUES(@UserId, @ImageId, @EmailAddress, @DisplayName, @FirstName, @LastName, @PhoneNumber, @IsFemale, @HashSalt, @Password, 
                        @TwitterUsername, @TwitterUserId, @TwitterToken, @TwitterSecret, @FacebookUserId, @FacebookToken, 
                        @LastLoginSuccess, @LastLoginFailure, @NumberLoginFailures, @IsLocked, @DateCreated, @DateModified, 0)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = user.Id,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "ImageId",
                    Value = (object)user.UserImageId ?? DBNull.Value,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "EmailAddress",
                    Value = (object)user.EmailAddress ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DisplayName",
                    Value = (object)user.DisplayName ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "FirstName",
                    Value = (object)user.FirstName ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "LastName",
                    Value = (object)user.LastName ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "PhoneNumber",
                    Value = (object)user.PhoneNumber ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "IsFemale",
                    Value = user.IsFemale,
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "HashSalt",
                    Value = (object)user.HashSalt ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Password",
                    Value = (object)user.Password ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "TwitterUsername",
                    Value = (object)user.TwitterUsername ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "TwitterUserId",
                    Value = (object)user.TwitterUserId ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "TwitterToken",
                    Value = (object)user.TwitterToken ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "TwitterSecret",
                    Value = (object)user.TwitterSecret ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "FacebookUserId",
                    Value = (object)user.FacebookUserId ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "FacebookToken",
                    Value = (object)user.FacebookToken ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "LastLoginSuccess",
                    Value = (object)user.LastLoginSuccess ?? DBNull.Value,
                    SqlDbType = SqlDbType.DateTime2,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "LastLoginFailure",
                    Value = (object)user.LastLoginFailure ?? DBNull.Value,
                    SqlDbType = SqlDbType.DateTime2,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "NumberLoginFailures",
                    Value = user.NumberLoginFailures,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "IsLocked",
                    Value = user.IsLocked,
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = user.DateCreated > DateTime.MinValue ? user.DateCreated : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = user.DateModified > DateTime.MinValue ? user.DateModified : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return user.Id;
        }

        /// <summary>
        /// Gets a user by Id.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public User FindById(Guid userId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Users
                        WHERE Id = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
            };

            return GetRecord<User>(query, parameters.ToArray());
        }

        /// <summary>
        /// Finds the user by display name.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <returns></returns>
        public User FindByDisplayName(string displayName)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Users
                        WHERE DisplayName = @DisplayName AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "DisplayName",
                    Value = displayName,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
            };

            return GetRecord<User>(query, parameters.ToArray());
        }

        /// <summary>
        /// Finds the user by email.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns></returns>
        public User FindByEmail(string emailAddress)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Users
                        WHERE EmailAddress = @Email AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Email",
                    Value = emailAddress,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
            };

            return GetRecord<User>(query, parameters.ToArray());
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public User UpdateUser(Guid userId, User user)
        {
            var query = @"UPDATE dbo.Users SET EmailAddress = @EmailAddress, DisplayName = @DisplayName, 
                    	FirstName = @FirstName, LastName = @LastName, IsFemale = @IsFemale, PhoneNumber = @PhoneNumber, 
                    	TwitterUsername = @TwitterUsername, TwitterUserId = @TwitterUserId, 
                        TwitterToken = @TwitterToken, TwitterSecret = @TwitterSecret, 
                        FacebookUserId = @FacebookUserId, FacebookToken = @FacebookToken, 
                        DateModified = @DateModified
                        WHERE Id = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "EmailAddress",
                    Value = (object)user.EmailAddress ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DisplayName",
                    Value = (object)user.DisplayName ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "FirstName",
                    Value = (object)user.FirstName ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "LastName",
                    Value = (object)user.LastName ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "IsFemale",
                    Value = user.IsFemale,
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "PhoneNumber",
                    Value = (object)user.PhoneNumber ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "TwitterUsername",
                    Value = (object)user.TwitterUsername ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "TwitterUserId",
                    Value = (object)user.TwitterUserId ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "TwitterToken",
                    Value = (object)user.TwitterToken ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "TwitterSecret",
                    Value = (object)user.TwitterSecret ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "FacebookUserId",
                    Value = (object)user.FacebookUserId ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "FacebookToken",
                    Value = (object)user.FacebookToken ?? DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = user.DateModified > DateTime.MinValue ? user.DateModified : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return FindById(userId);
        }

        /// <summary>
        /// Updates the user password.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public User UpdateUserPassword(Guid userId, string password)
        {
            var query = @"UPDATE dbo.Users 
                        SET Password = @Password, DateModified = @DateModified
                        WHERE Id = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Password",
                    Value = password,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return FindById(userId);
        }

        /// <summary>
        /// Updates the user profile image.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        public User UpdateUserProfileImage(Guid userId, int? imageId)
        {
            var query = @"UPDATE dbo.Users 
                        SET UserImageId = @UserImageId, DateModified = @DateModified
                        WHERE Id = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserImageId",
                    Value = (object)imageId ?? DBNull.Value,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return FindById(userId);
        }

        /// <summary>
        /// Updates user information when the user login fails.
        /// </summary>
        /// <param name="user">The user.</param>
        public void FailedUserLogin(User user)
        {
            var query = @"UPDATE dbo.Users
                        SET NumberLoginFailures = @NumberLoginFailures, 
                        LastLoginFailure = @LastLoginFailure
                        WHERE Id = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "NumberLoginFailures",
                    Value = user.NumberLoginFailures + 1,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "LastLoginFailure",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = user.Id,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Locks the user account.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public void LockUserAccount(Guid userId)
        {
            var query = @"UPDATE dbo.Users
                        SET IsLocked = 1
                        WHERE Id = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Unlocks the user account.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public void UnlockUserAccount(Guid userId)
        {
            var query = @"UPDATE dbo.Users
                        SET IsLocked = 0,
                        NumberLoginFailures = 0,
                        LastLoginFailure = NULL
                        WHERE Id = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Updates user information when the user login succeeds.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public User SuccessfulUserLogin(User user)
        {
            var query = @"UPDATE dbo.Users
                        SET NumberLoginFailures = 0, 
                        LastLoginSuccess = @LastLoginSuccess
                        WHERE Id = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "LastLoginSuccess",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = user.Id,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return FindById(user.Id);
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public void DeleteUser(Guid userId)
        {
            var query = @"UPDATE dbo.Users
                        SET IsDeleted = 1, DateModified = @DateModified
                        WHERE Id = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }, 
                new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }
    }
}
