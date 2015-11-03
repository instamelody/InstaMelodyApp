using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class UserMelodies : DataAccess
    {
        #region UserMelodies

        /// <summary>
        /// Creates the user melody.
        /// </summary>
        /// <param name="userMelody">The user melody.</param>
        /// <returns></returns>
        public UserMelody CreateUserMelody(UserMelody userMelody)
        {
            var userMelodyId = Guid.NewGuid();

            var query = @"INSERT INTO dbo.UserMelodies
                        (Id, Name, UserId, IsExplicit, DateCreated)
                        VALUES (@UserMelodyId, @Name, @UserId, @IsExplicit, @DateCreated)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userMelody.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = userMelody.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "IsExplicit",
                    Value = userMelody.IsExplicit,
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = userMelody.DateCreated > DateTime.MinValue
                        ? userMelody.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return GetUserMelodyById(userMelodyId);
        }

        /// <summary>
        /// Gets the user melody by identifier.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        /// <param name="showDeleted">if set to <c>true</c> [show deleted].</param>
        /// <returns></returns>
        public UserMelody GetUserMelodyById(Guid userMelodyId, bool showDeleted = false)
        {
            var query = @"SELECT TOP 1 * FROM dbo.UserMelodies
                        WHERE Id = @UserMelodyId";

            if (!showDeleted)
            {
                query += @" AND IsDeleted = 0";
            }

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<UserMelody>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the user melody by melody identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        /// <returns></returns>
        public UserMelody GetUserMelodyByMelodyId(int melodyId)
        {
            var query = @"SELECT TOP 1 m.* FROM dbo.UserMelodies m
                        JOIN dbo.UserMelodyParts p
                        ON p.UserMelodyId = m.Id
                        WHERE p.IsDeleted = 0 AND m.IsDeleted = 0
                        AND p.MelodyId = @MelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<UserMelody>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the user melodies by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<UserMelody> GetUserMelodiesByUserId(Guid userId)
        {
            var query = @"SELECT * FROM dbo.UserMelodies
                        WHERE IsDeleted = 0 AND UserId = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<UserMelody>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the name of the user melody by user identifier and.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="showDeleted">if set to <c>true</c> [show deleted].</param>
        /// <returns></returns>
        public UserMelody GetUserMelodyByUserIdAndName(Guid userId, string name, bool showDeleted = false)
        {
            var query = @"SELECT TOP 1 * FROM dbo.UserMelodies
                        WHERE UserId = @UserId
                        AND Name = @Name";

            if (!showDeleted)
            {
                query += @" AND IsDeleted = 0";
            }

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
                    ParameterName = "Name",
                    Value = name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
            };

            return GetRecord<UserMelody>(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the user melody.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        public void DeleteUserMelody(Guid userMelodyId)
        {
            var query = @"UPDATE dbo.UserMelodies
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND Id = @UserMelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());

            DeletePartsByUserMelodyId(userMelodyId);
        }

        #endregion UserMelodies

        #region UserMelodyParts

        /// <summary>
        /// Creates the user melody part.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        /// <param name="melodyId">The melody identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to create a new User Melody Part.</exception>
        public UserMelodyPart CreateUserMelodyPart(Guid userMelodyId, int melodyId)
        {
            var query = @"INSERT INTO dbo.UserMelodyParts
                        (UserMelodyId, MelodyId, DateCreated)
                        VALUES (@UserMelodyId, @MelodyId, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                    new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
            };

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj))
            {
                return GetUserMelodyPartById(Convert.ToInt32(obj));
            }

            throw new DataException("Failed to create a new User Melody Part.");
        }

        /// <summary>
        /// Gets the user melody part by identifier.
        /// </summary>
        /// <param name="melodyPartId">The melody part identifier.</param>
        /// <returns></returns>
        public UserMelodyPart GetUserMelodyPartById(int melodyPartId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.UserMelodyParts
                        WHERE IsDeleted = 0 AND Id = @MelodyPartId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyPartId",
                    Value = melodyPartId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<UserMelodyPart>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the parts by user melody identifier.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        /// <returns></returns>
        public IList<Melody> GetPartsByUserMelodyId(Guid userMelodyId)
        {
            var query = @"SELECT m.* FROM dbo.Melodies m
                        JOIN dbo.UserMelodyParts p
                        ON m.Id = p.MelodyId
                        WHERE m.IsDeleted = 0 AND p.IsDeleted = 0
                        AND p.UserMelodyId = @UserMelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<Melody>(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the parts by user melody identifier.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        private void DeletePartsByUserMelodyId(Guid userMelodyId)
        {
            var query = @"UPDATE dbo.UserMelodyParts
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND UserMelodyId = @UserMelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        #endregion UserMelodyParts
    }
}
