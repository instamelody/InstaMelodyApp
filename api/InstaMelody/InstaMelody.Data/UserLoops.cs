using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class UserLoops : DataAccess
    {
        #region UserLoops

        /// <summary>
        /// Creates the user loop.
        /// </summary>
        /// <param name="loop">The loop.</param>
        /// <returns></returns>
        public UserLoop CreateUserLoop(UserLoop loop)
        {
            var userLoopId = Guid.NewGuid();

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.UserLoops
                                    (Id, Name, UserId, DateCreated, DateModified)
                                    VALUES (@UserMelodyId, @Name, @UserId, @DateCreated, @DateCreated)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = loop.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Name",
                    Value = loop.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = loop.DateCreated > DateTime.MinValue
                        ? loop.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return this.GetUserLoopById(userLoopId);
        }

        /// <summary>
        /// Gets the user loop by identifier.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        /// <returns></returns>
        public UserLoop GetUserLoopById(Guid userLoopId)
        {
            UserLoop result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.UserLoops
                                    WHERE IsDeleted = 0 AND Id = @UserLoopId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
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
                            result = new UserLoop();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the user loops by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<UserLoop> GetUserLoopsByUserId(Guid userId)
        {
            List<UserLoop> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.UserLoops
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
                        results = new List<UserLoop>();
                        while (reader.Read())
                        {
                            var result = new UserLoop();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the name of the user loop by user identifier and.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public UserLoop GetUserLoopByUserIdAndName(Guid userId, string name)
        {
            UserLoop result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.UserLoops
                                    WHERE IsDeleted = 0 AND UserId = @UserId AND Name = @Name";

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
                            result = new UserLoop();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the user loop date modified.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        /// <returns></returns>
        public UserLoop UpdateUserLoopDateModified(Guid userLoopId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserLoops
                                    SET DateModified = @DateModified
                                    WHERE IsDeleted = 0 AND Id = @UserLoopId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return this.GetUserLoopById(userLoopId);
        }

        /// <summary>
        /// Deletes the user loop.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        public void DeleteUserLoop(Guid userLoopId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserLoops
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND Id = @UserLoopId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            this.DeletePartsByUserLoopId(userLoopId);
        }

        #endregion UserLoops

        #region UserLoopParts

        /// <summary>
        /// Creates the user loop part.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <returns></returns>
        public UserLoopPart CreateUserLoopPart(UserLoopPart part)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.UserLoopParts
                                    (UserLoopId, UserMelodyId, OrderIndex, StartTime, StartEffect,
                                    StartEffectDuration, EndTime, EndEffect, EndEffectDuration, DateCreated)
                                    VALUES (@UserLoopId, @UserMelodyId, @OrderIndex, @StartTime, @StartEffect,
                                    @StartEffectDuration, @EndTime, @EndEffect, @EndEffectDuration, @DateCreated)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = part.UserLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = part.UserMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "OrderIndex",
                    Value = part.OrderIndex,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "StartTime",
                    Value = part.StartTime != null
                            ? (object)((TimeSpan)part.StartTime).Ticks
                            : DBNull.Value,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "StartEffect",
                    Value = part.StartEffect.ToString(),
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "StartEffectDuration",
                    Value = part.StartEffectDuration != null
                            ? (object)((TimeSpan)part.StartEffectDuration).Ticks
                            : DBNull.Value,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "EndTime",
                    Value = part.EndTime != null
                            ? (object)((TimeSpan)part.EndTime).Ticks
                            : DBNull.Value,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "EndEffect",
                    Value = part.EndEffect.ToString(),
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "EndEffectDuration",
                    Value = part.EndEffectDuration != null
                            ? (object)((TimeSpan)part.EndEffectDuration).Ticks
                            : DBNull.Value,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = part.DateCreated > DateTime.MinValue
                        ? part.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    return this.GetUserLoopPartById(Convert.ToInt32(obj));
                }
            }

            throw new DataException("Failed to create a new User Loop Part.");
        }

        /// <summary>
        /// Gets the user loop part by identifier.
        /// </summary>
        /// <param name="partId">The part identifier.</param>
        /// <returns></returns>
        public UserLoopPart GetUserLoopPartById(int partId)
        {
            UserLoopPart result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.UserLoopParts
                                    WHERE IsDeleted = 0 AND Id = @UserLoopPartId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserLoopPartId",
                    Value = partId,
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
                            result = new UserLoopPart();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the parts by user loop identifier.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        /// <returns></returns>
        public IList<UserLoopPart> GetPartsByUserLoopId(Guid userLoopId)
        {
            List<UserLoopPart> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.UserLoopParts
                                    WHERE IsDeleted = 0 AND UserLoopId = @UserLoopId
                                    ORDER BY OrderIndex ASC";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<UserLoopPart>();
                        while (reader.Read())
                        {
                            var result = new UserLoopPart();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the last order index for loop.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public int GetLastOrderIndexForLoop(Guid userLoopId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT MAX(OrderIndex) FROM dbo.UserLoopParts
                                    WHERE IsDeleted = 0 AND UserLoopId = @UserLoopId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    return Convert.ToInt32(obj);
                }
            }

            throw new DataException();
        }

        /// <summary>
        /// Deletes the loop part.
        /// </summary>
        /// <param name="partId">The part identifier.</param>
        public void DeleteLoopPart(int partId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserLoopParts
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND Id = @UserLoopPartId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserLoopPartId",
                    Value = partId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the parts by user loop identifier.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        private void DeletePartsByUserLoopId(Guid userLoopId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserLoopParts
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND UserLoopId = @UserLoopId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        #endregion UserLoopParts
    }
}
