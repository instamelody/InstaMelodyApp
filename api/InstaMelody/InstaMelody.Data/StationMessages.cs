using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class StationMessages : DataAccess
    {
        #region Station Messages

        /// <summary>
        /// Creates the station message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to create a new Station Message.</exception>
        public StationMessage CreateStationMessage(StationMessage message)
        {
            var query = @"INSERT INTO dbo.StationMessages (StationId, MessageId, SenderId, ParentId, IsPrivate, DateCreated)
                        VALUES (@StationId, @MessageId, @SenderId, @ParentId, @IsPrivate, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = message.StationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = message.MessageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "SenderId",
                    Value = !message.SenderId.Equals(default(Guid))
                        ? (object)message.SenderId
                        : DBNull.Value,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = message.ParentId != null
                        ? (object)message.ParentId
                        : DBNull.Value,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "IsPrivate",
                    Value = message.IsPrivate,
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = message.DateCreated > DateTime.MinValue
                        ? message.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj))
            {
                return GetStationMessageById(Convert.ToInt32(obj));
            }

            throw new DataException("Failed to create a new Station Message.");
        }

        /// <summary>
        /// Gets the station message by identifier.
        /// </summary>
        /// <param name="stationMessageId">The station message identifier.</param>
        /// <returns></returns>
        public StationMessage GetStationMessageById(int stationMessageId)
        {
            var query = @"SELECT * FROM dbo.StationMessages
                        WHERE Id = @Id AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Id",
                    Value = stationMessageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<StationMessage>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the messages by station identifier.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        /// <param name="isPrivate">if set to <c>true</c> [is private].</param>
        /// <returns></returns>
        public IList<StationMessage> GetMessagesByStationId(int stationId, bool isPrivate)
        {
            var query = @"SELECT * FROM dbo.StationMessages
                        WHERE StationId = @StationId AND IsPrivate = @IsPrivate AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "IsPrivate",
                    Value = isPrivate,
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<StationMessage>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the replies by station message identifier.
        /// </summary>
        /// <param name="stationMessageId">The station message identifier.</param>
        /// <returns></returns>
        public IList<StationMessage> GetRepliesByStationMessageId(int stationMessageId)
        {
            var query = @"SELECT * FROM dbo.StationMessages
                        WHERE ParentId = @ParentId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = stationMessageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<StationMessage>(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the station message.
        /// </summary>
        /// <param name="stationMessageId">The station message identifier.</param>
        public void DeleteStationMessage(int stationMessageId)
        {
            var query = @"UPDATE dbo.StationMessages
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND (Id = @Id OR ParentId = @Id)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Id",
                    Value = stationMessageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the station messages by station identifier.
        /// </summary>
        /// <param name="stationId">The station identifier.</param>
        public void DeleteStationMessagesByStationId(int stationId)
        {
            var query = @"UPDATE dbo.StationMessages
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND StationId = @StationId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationId",
                    Value = stationId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        #endregion Station Messages

        #region Station Message User Likes

        /// <summary>
        /// Likes the station message.
        /// </summary>
        /// <param name="stationMessageId">The station message identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public void LikeStationMessage(int stationMessageId, Guid userId)
        {
            var query = @"INSERT INTO dbo.StationMessageUserLikes (StationMessageId, UserId, DateCreated)
                        VALUES (@StationMessageId, @UserId, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationMessageId",
                    Value = stationMessageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
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
        }

        /// <summary>
        /// Gets the likes by station message identifier.
        /// </summary>
        /// <param name="stationMessageId">The station message identifier.</param>
        /// <returns></returns>
        public IList<StationMessageUserLike> GetLikesByStationMessageId(int stationMessageId)
        {
            var query = @"SELECT * FROM dbo.StationMessageUserLikes
                        WHERE StationMessageId = @StationMessageId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationMessageId",
                    Value = stationMessageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<StationMessageUserLike>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the liked messages by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<StationMessage> GetLikedMessagesByUserId(Guid userId)
        {
            var query = @"SELECT m.* FROM dbo.StationMessages m
                        JOIN dbo.StationMessageUserLikes l
                        ON m.Id = l.StationMessageId
                        WHERE m.IsDeleted = 0 AND l.IsDeleted = 0
                        AND l.UserId = @UserId";

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

            return GetRecordSet<StationMessage>(query, parameters.ToArray());
        }

        /// <summary>
        /// Unlikes the station message.
        /// </summary>
        /// <param name="stationMessageId">The station message identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public void UnlikeStationMessage(int stationMessageId, Guid userId)
        {
            var query = @"UPDATE dbo.StationMessageUserLikes
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND StationMessageId = @StationMessageId
                        AND UserId = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "StationMessageId",
                    Value = stationMessageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        #endregion Station Message User Likes
    }
}
