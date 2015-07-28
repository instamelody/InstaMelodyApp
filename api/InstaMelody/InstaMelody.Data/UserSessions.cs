using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class UserSessions : DataAccess
    {
        /// <summary>
        /// Finds all sessions by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<UserSession> FindByUserId(Guid userId)
        {
            var query = @"SELECT * FROM dbo.UserSessions WHERE UserId = @UserId";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<UserSession>(query, parameters.ToArray());
        }

        /// <summary>
        /// Finds all active sessions by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<UserSession> FindActiveSessions(Guid userId)
        {
            var query = @"SELECT * FROM dbo.UserSessions
                        WHERE UserId = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<UserSession>(query, parameters.ToArray());
        }

        /// <summary>
        /// Finds the active sessions.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <returns></returns>
        public IList<UserSession> FindActiveSessions(string deviceToken)
        {
            var query = @"SELECT * FROM dbo.UserSessions
                        WHERE DeviceToken = @DeviceToken AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "DeviceToken",
                    Value = deviceToken,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<UserSession>(query, parameters.ToArray());
        }

        /// <summary>
        /// Ends all sessions by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public void EndAllSessionsByUserId(Guid userId)
        {
            var query = @"UPDATE dbo.UserSessions
                        SET IsDeleted = 1, LastActivity = @LastActivity
                        WHERE UserId = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    ParameterName = "LastActivity",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
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

        /// <summary>
        /// Ends the session by user identifier.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="deviceToken">The device token.</param>
        public void EndSession(Guid token, string deviceToken)
        {
            var query = @"UPDATE dbo.UserSessions
                        SET IsDeleted = 1, LastActivity = @LastActivity
                        WHERE Token = @Token AND DeviceToken = @DeviceToken AND IsDeleted = 0";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    ParameterName = "LastActivity",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DeviceToken",
                    Value = deviceToken,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Ends the session.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The token.</param>
        public void EndSession(Guid userId, Guid token)
        {
            var query = @"UPDATE dbo.UserSessions
                        SET IsDeleted = 1, LastActivity = @LastActivity
                        WHERE Token = @Token AND UserId = @UserId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    ParameterName = "LastActivity",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
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

        /// <summary>
        /// Finds the by token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public UserSession FindByToken(Guid token)
        {
            var query = @"SELECT TOP 1 * FROM dbo.UserSessions
                        WHERE Token = @Token AND IsDeleted = 0";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<UserSession>(query, parameters.ToArray());
        }

        /// <summary>
        /// Adds the session.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public Guid AddSession(UserSession session)
        {
            session.Token = Guid.NewGuid();

            var query = @"INSERT INTO dbo.UserSessions (Token, UserId, DeviceToken, LastActivity, DateCreated, IsDeleted)
                        VALUES(@Token, @UserId, @DeviceToken, @LastActivity, @DateCreated, 0)";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    ParameterName = "Token",
                    Value = session.Token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = session.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DeviceToken",
                    Value = session.DeviceToken,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "LastActivity",
                    Value = session.LastActivity > DateTime.MinValue ? session.LastActivity : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = session.DateCreated > DateTime.MinValue ? session.DateCreated : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return session.Token;
        }

        /// <summary>
        /// Updates the session activity.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public UserSession UpdateSessionActivity(Guid token)
        {
            var query = @"UPDATE dbo.UserSessions
                                    SET LastActivity = @LastActivity
                                    WHERE Token = @Token AND IsDeleted = 0";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    ParameterName = "LastActivity",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());

            return FindByToken(token);
        }

        /// <summary>
        /// Adds the session.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="deviceToken">The device token.</param>
        /// <returns></returns>
        public Guid AddSession(Guid userId, string deviceToken)
        {
            var session = new UserSession
            {
                UserId = userId,
                DeviceToken = deviceToken,
                LastActivity = DateTime.UtcNow,
                DateCreated = DateTime.UtcNow
            };
            return AddSession(session);
        }
    }
}
