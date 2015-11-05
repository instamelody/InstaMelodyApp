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

            var query = @"INSERT INTO dbo.UserLoops (Id, Name, UserId, IsExplicit, DateCreated, DateModified)
                        VALUES (@UserMelodyId, @Name, @UserId, @IsExplicit, @DateCreated, @DateCreated)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = loop.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = loop.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "IsExplicit",
                    Value = loop.IsExplicit,
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
            return GetUserLoopById(userLoopId);
        }

        /// <summary>
        /// Gets the user loop by identifier.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        /// <returns></returns>
        public UserLoop GetUserLoopById(Guid userLoopId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.UserLoops
                        WHERE IsDeleted = 0 AND Id = @UserLoopId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<UserLoop>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the user loops by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<UserLoop> GetUserLoopsByUserId(Guid userId)
        {
            var query = @"SELECT * FROM dbo.UserLoops
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

            return GetRecordSet<UserLoop>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the name of the user loop by user identifier and.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public UserLoop GetUserLoopByUserIdAndName(Guid userId, string name)
        {
            var query = @"SELECT TOP 1 * FROM dbo.UserLoops
                        WHERE IsDeleted = 0 AND UserId = @UserId AND Name = @Name";

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
                }
            };

            return GetRecord<UserLoop>(query, parameters.ToArray());
        }

        /// <summary>
        /// Updates the user loop date modified.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        /// <returns></returns>
        public UserLoop UpdateUserLoopDateModified(Guid userLoopId)
        {
            var query = @"UPDATE dbo.UserLoops
                        SET DateModified = @DateModified
                        WHERE IsDeleted = 0 AND Id = @UserLoopId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return GetUserLoopById(userLoopId);
        }

        /// <summary>
        /// Deletes the user loop.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        public void DeleteUserLoop(Guid userLoopId)
        {
            var query = @"UPDATE dbo.UserLoops
                        SET IsDeleted = 1, DateModified = @DateModified
                        WHERE IsDeleted = 0 AND Id = @UserLoopId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
            DeletePartsByUserLoopId(userLoopId);
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
            var query = @"INSERT INTO dbo.UserLoopParts
                        (UserLoopId, UserMelodyId, OrderIndex, StartTime, StartEffect,
                        StartEffectDuration, EndTime, EndEffect, EndEffectDuration, DateCreated)
                        VALUES (@UserLoopId, @UserMelodyId, @OrderIndex, @StartTime, @StartEffect,
                        @StartEffectDuration, @EndTime, @EndEffect, @EndEffectDuration, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = part.UserLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = part.UserMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "OrderIndex",
                    Value = part.OrderIndex,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "StartTime",
                    Value = part.StartTime != null
                            ? (object)((TimeSpan)part.StartTime).Ticks
                            : DBNull.Value,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "StartEffect",
                    Value = part.StartEffect.ToString(),
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "StartEffectDuration",
                    Value = part.StartEffectDuration != null
                            ? (object)((TimeSpan)part.StartEffectDuration).Ticks
                            : DBNull.Value,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "EndTime",
                    Value = part.EndTime != null
                            ? (object)((TimeSpan)part.EndTime).Ticks
                            : DBNull.Value,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "EndEffect",
                    Value = part.EndEffect.ToString(),
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "EndEffectDuration",
                    Value = part.EndEffectDuration != null
                            ? (object)((TimeSpan)part.EndEffectDuration).Ticks
                            : DBNull.Value,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            var obj = ExecuteScalar(query, parameters.ToArray()); 
            if (!Convert.IsDBNull(obj))
            {
                return GetUserLoopPartById(Convert.ToInt32(obj));
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
            var query = @"SELECT TOP 1 * FROM dbo.UserLoopParts
                        WHERE IsDeleted = 0 AND Id = @UserLoopPartId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserLoopPartId",
                    Value = partId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<UserLoopPart>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the parts by user loop identifier.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        /// <returns></returns>
        public IList<UserLoopPart> GetPartsByUserLoopId(Guid userLoopId)
        {
            var query = @"SELECT * FROM dbo.UserLoopParts
                        WHERE IsDeleted = 0 AND UserLoopId = @UserLoopId
                        ORDER BY OrderIndex ASC";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<UserLoopPart>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the last order index for loop.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public int GetLastOrderIndexForLoop(Guid userLoopId)
        {
            var query = @"SELECT MAX(OrderIndex) FROM dbo.UserLoopParts
                        WHERE IsDeleted = 0 AND UserLoopId = @UserLoopId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            var obj = ExecuteScalar(query, parameters.ToArray());
            return !Convert.IsDBNull(obj) ? Convert.ToInt32(obj) : 0;
        }

        /// <summary>
        /// Deletes the loop part.
        /// </summary>
        /// <param name="partId">The part identifier.</param>
        public void DeleteLoopPart(int partId)
        {
            var query = @"UPDATE dbo.UserLoopParts
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND Id = @UserLoopPartId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserLoopPartId",
                    Value = partId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
            ReIndexPartsSproc(partId);
        }

        /// <summary>
        /// Deletes the parts by user loop identifier.
        /// </summary>
        /// <param name="userLoopId">The user loop identifier.</param>
        private void DeletePartsByUserLoopId(Guid userLoopId)
        {
            var query = @"UPDATE dbo.UserLoopParts
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND UserLoopId = @UserLoopId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserLoopId",
                    Value = userLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Re-indexes the non-deleted User Loop Parts.
        /// </summary>
        /// <param name="loopPartId">The loop part identifier.</param>
        private void ReIndexPartsSproc(int loopPartId)
        {
            var query = @"SELECT TOP 1 UserLoopId FROM dbo.UserLoopParts WHERE Id = @UserLoopPartId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserLoopPartId",
                    Value = loopPartId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj))
            {
                var loopId = (Guid) obj;

                var query2 = @"ReindexUserMelodyParts";

                var parameters2 = new Dictionary<string, object>
                {
                    {"loopId", loopId}
                };

                ExecuteNoReturnSproc(query2, parameters2);
            }
        }

        #endregion UserLoopParts
    }
}
