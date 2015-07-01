using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

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
            var results = new List<UserSession>();

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.UserSessions WHERE UserId = @UserId";

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
                        while (reader.Read())
                        {
                            var result = new UserSession();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all active sessions by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<UserSession> FindActiveSessionsByUserId(Guid userId)
        {
            var results = new List<UserSession>();

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.UserSessions
                                    WHERE UserId = @UserId AND IsDeleted = 0";

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
                        while (reader.Read())
                        {
                            var result = new UserSession();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Ends all sessions by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public int EndAllSessionsByUserId(Guid userId)
        {
            var sessions = this.FindActiveSessionsByUserId(userId);
            var count = 0;

            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();
                foreach (var session in sessions)
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"UPDATE dbo.UserSessions
                                    SET IsDeleted = 1, LastActivity = @LastActivity
                                    WHERE UserId = @UserId AND IsDeleted = 0";

                    cmd.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "LastActivity",
                        Value = DateTime.UtcNow,
                        SqlDbType = SqlDbType.DateTime,
                        Direction = ParameterDirection.Input
                    });
                    cmd.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "UserId",
                        Value = session.UserId,
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Direction = ParameterDirection.Input
                    });

                    cmd.ExecuteNonQuery();

                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Ends the session by user identifier.
        /// </summary>
        /// <param name="token">The token.</param>
        public void EndSession(Guid token)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserSessions
                                SET IsDeleted = 1, LastActivity = @LastActivity
                                WHERE Token = @Token AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "LastActivity",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Finds the by token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public UserSession FindByToken(Guid token)
        {
            var result = new UserSession();

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.UserSessions
                                    WHERE Token = @Token AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
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
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the session.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public Guid AddSession(UserSession session)
        {
            session.Token = Guid.NewGuid();

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.UserSessions (Token, UserId, LastActivity, DateCreated, IsDeleted)
                                    VALUES(@Token, @UserId, @LastActivity, @DateCreated, 0)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Token",
                    Value = session.Token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = session.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "LastActivity",
                    Value = session.LastActivity > DateTime.MinValue ? session.LastActivity : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = session.DateCreated > DateTime.MinValue ? session.DateCreated : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return session.Token;
        }

        /// <summary>
        /// Updates the session activity.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public UserSession UpdateSessionActivity(Guid token)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserSessions
                                    SET LastActivity = @LastActivity
                                    WHERE Token = @Token AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "LastActivity",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return this.FindByToken(token);
        }

        /// <summary>
        /// Adds the session.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Guid AddSession(Guid userId)
        {
            var session = new UserSession
            {
                UserId = userId,
                LastActivity = DateTime.UtcNow,
                DateCreated = DateTime.UtcNow
            };
            return AddSession(session);
        }
    }
}
