using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class UserFriends : DataAccess
    {
        /// <summary>
        /// Requests the friend.
        /// </summary>
        /// <param name="requestiorId">The requestior identifier.</param>
        /// <param name="friendId">The friend identifier.</param>
        /// <returns></returns>
        public void RequestFriend(Guid requestiorId, Guid friendId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.UserFriends (UserId, RequestorId, DateCreated, DateModified)
                                    VALUES (@UserId, @RequestorId, @DateCreated, @DateCreated)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = friendId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "RequestorId",
                    Value = requestiorId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
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
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Approves the request.
        /// </summary>
        /// <param name="friendId">The friend identifier.</param>
        /// <param name="requestiorId">The requestior identifier.</param>
        public void ApproveRequest(Guid friendId, Guid requestiorId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserFriends 
                                    SET IsPending = 0, IsDenied = 0, 
                                        DateApproved = @DateApproved, DateModified = @DateApproved
                                    WHERE UserId = @UserId AND RequestorId = @RequestorId
                                        AND IsPending = 1";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = friendId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "RequestorId",
                    Value = requestiorId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateApproved",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Denies the request.
        /// </summary>
        /// <param name="friendId">The friend identifier.</param>
        /// <param name="requestiorId">The requestior identifier.</param>
        public void DenyRequest(Guid friendId, Guid requestiorId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserFriends 
                                    SET IsPending = 0, IsDenied = 1, 
                                        DateApproved = NULL, DateModified = @DateModified
                                    WHERE UserId = @UserId AND RequestorId = @RequestorId
                                        AND IsPending = 1";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = friendId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "RequestorId",
                    Value = requestiorId,
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
        }

        /// <summary>
        /// Deletes the friend.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="friendId">The friend identifier.</param>
        public void DeleteFriend(Guid userId, Guid friendId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserFriends 
                                    SET IsDeleted = 1, DateModified = @DateModified
                                    WHERE (UserId = @UserId AND RequestorId = @RequestorId)
                                    OR (UserId = @RequestorId AND RequestorId = @UserId)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "RequestorId",
                    Value = friendId,
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
        }

        /// <summary>
        /// Determines whether the specified user identifier is requestor.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="friendId">The friend identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public bool CanApproveDeny(Guid userId, Guid friendId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT COUNT(*) FROM dbo.UserFriends
                                    WHERE UserId = @UserId
                                    AND RequestorId = @RequestorId
                                    AND IsDeleted = 0 AND IsPending = 1";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "RequestorId",
                    Value = friendId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var count = cmd.ExecuteScalar();
                if (Convert.IsDBNull(count)) throw new Exception();

                if (Convert.ToInt32(count) > 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the friends.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<User> GetUserFriends(Guid userId)
        {
            var results = new List<User>();

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = 
@"SELECT * FROM dbo.Users u JOIN dbo.UserFriends f
    ON (u.Id = f.UserId OR u.Id = f.RequestorId)
    WHERE u.IsDeleted = 0 AND (f.UserId = @UserId OR f.RequestorId = @UserId)
    AND u.Id != @UserId AND f.IsPending = 0 AND f.IsDenied = 0 AND f.IsDeleted = 0";

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
                            var result = new User();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the pending friends.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<User> GetPendingFriends(Guid userId)
        {
            var results = new List<User>();

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText =
@"SELECT * FROM dbo.Users u JOIN dbo.UserFriends f
    ON (u.Id = f.UserId OR u.Id = f.RequestorId)
    WHERE u.IsDeleted = 0 AND (f.UserId = @UserId OR f.RequestorId = @UserId)
    AND u.Id != @UserId AND f.IsPending = 1 AND f.IsDenied = 0 AND f.IsDeleted = 0";

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
                            var result = new User();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the user friend by identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="friendUserId">The friend user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public bool AreUsersFriends(Guid userId, Guid friendUserId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT COUNT(*) FROM dbo.UserFriends
                                    WHERE (UserId = @UserId AND RequestorId = @FriendId) 
                                    OR (UserId = @FriendId AND RequestorId = @UserId)
                                    AND IsDeleted = 0 AND IsPending = 0 AND IsDenied = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }); 
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "FriendId",
                    Value = friendUserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var id = cmd.ExecuteScalar();
                if (Convert.IsDBNull(id)) throw new Exception();

                return Convert.ToInt32(id) > 0;
            }
        }
    }
}
