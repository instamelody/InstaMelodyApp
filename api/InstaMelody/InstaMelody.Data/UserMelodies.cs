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

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.UserMelodies
                                    (Id, Name, UserId, DateCreated)
                                    VALUES (@UserMelodyId, @Name, @UserId, @DateCreated)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userMelody.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Name",
                    Value = userMelody.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = userMelody.DateCreated > DateTime.MinValue
                        ? userMelody.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return this.GetUserMelodyById(userMelodyId);
        }

        /// <summary>
        /// Gets the user melody by identifier.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        /// <returns></returns>
        public UserMelody GetUserMelodyById(Guid userMelodyId)
        {
            UserMelody result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.UserMelodies
                                    WHERE IsDeleted = 0 AND Id = @UserMelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = new UserMelody();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the user melody by melody identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        /// <returns></returns>
        public UserMelody GetUserMelodyByMelodyId(int melodyId)
        {
            UserMelody result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 m.* FROM dbo.UserMelodies m
                                    JOIN dbo.UserMelodyParts p
                                    ON p.UserMelodyId = m.Id
                                    WHERE p.IsDeleted = 0 AND m.IsDeleted = 0
                                        AND p.MelodyId = @MelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = new UserMelody();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the user melodies by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<UserMelody> GetUserMelodiesByUserId(Guid userId)
        {
            List<UserMelody> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.UserMelodies
                                    WHERE IsDeleted = 0 AND UserId = @UserId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<UserMelody>();
                        while (reader.Read())
                        {
                            var result = new UserMelody();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the name of the user melody by user identifier and.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public UserMelody GetUserMelodyByUserIdAndName(Guid userId, string name)
        {
            UserMelody result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.UserMelodies
                                    WHERE IsDeleted = 0 AND UserId = @UserId
                                        AND Name = @Name";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Name",
                    Value = name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = new UserMelody();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the user melody.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        public void DeleteUserMelody(Guid userMelodyId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserMelodies
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND Id = @UserMelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            this.DeletePartsByUserMelodyId(userMelodyId);
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
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.UserMelodyParts
                                    (UserMelodyId, MelodyId, DateCreated)
                                    VALUES (@UserMelodyId, @MelodyId, @DateCreated)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    return this.GetUserMelodyPartById(Convert.ToInt32(obj));
                }
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
            UserMelodyPart result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.UserMelodyParts
                                    WHERE IsDeleted = 0 AND Id = @MelodyPartId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyPartId",
                    Value = melodyPartId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = new UserMelodyPart();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the parts by user melody identifier.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        /// <returns></returns>
        public IList<Melody> GetPartsByUserMelodyId(Guid userMelodyId)
        {
            List<Melody> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT m.* FROM dbo.Melodies m
                                    JOIN dbo.UserMelodyParts p
                                    ON m.Id = p.MelodyId
                                    WHERE m.IsDeleted = 0 AND p.IsDeleted = 0
                                        AND p.UserMelodyId = @UserMelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<Melody>();
                        while (reader.Read())
                        {
                            var result = new Melody();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Deletes the parts by user melody identifier.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        private void DeletePartsByUserMelodyId(Guid userMelodyId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserMelodyParts
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND UserMelodyId = @UserMelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        #endregion UserMelodyParts
    }
}
