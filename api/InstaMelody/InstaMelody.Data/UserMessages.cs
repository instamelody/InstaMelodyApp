using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;


namespace InstaMelody.Data
{
    public class UserMessages : DataAccess
    {
        /// <summary>
        /// Adds the user message.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="recipientId">The recipient identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public UserMessage AddUserMessage(Guid userId, Guid recipientId, Guid messageId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.UserMessages
                                    (UserId, RecipientId, MessageId, DateCreated)
                                    VALUES (@UserId, @RecipientId, @MessageId, @DateCreated)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "RecipientId",
                    Value = recipientId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
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
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    return this.GetUserMessageById(Convert.ToInt32(obj));
                }
            }

            throw new DataException();
        }

        /// <summary>
        /// Gets the user message by identifier.
        /// </summary>
        /// <param name="userMessageId">The user message identifier.</param>
        /// <returns></returns>
        public UserMessage GetUserMessageById(int userMessageId)
        {
            UserMessage result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.UserMessages
                                    WHERE IsDeleted = 0 AND Id = @UserMessageId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserMessageId",
                    Value = userMessageId,
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
                            result = new UserMessage();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the user message by message identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public UserMessage GetUserMessageByMessageId(Guid messageId)
        {
            UserMessage result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.UserMessages
                                    WHERE IsDeleted = 0 AND MessageId = @MessageId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
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
                            result = new UserMessage();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the messages by user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<UserMessage> GetUserMessagesByUser(Guid userId)
        {
            List<UserMessage> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.UserMessages
                                    WHERE (UserId = @UserId AND IsDeleted = 0 AND IsDeletedBySender = 0) 
                                    OR (RecipientId = @UserId AND IsDeleted = 0 AND IsDeletedByRecipient = 0)";

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
                        results = new List<UserMessage>();

                        while (reader.Read())
                        {
                            var result = new UserMessage();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Deletes the user message.
        /// </summary>
        /// <param name="userMessageId">The user message identifier.</param>
        public void DeleteUserMessage(int userMessageId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserMessages
                                    SET IsDeleted = 1
                                    WHERE Id = @UserMessageId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = userMessageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the user message for sender.
        /// </summary>
        /// <param name="userMessageId">The user message identifier.</param>
        public void DeleteUserMessageForSender(int userMessageId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserMessages
                                    SET IsDeletedBySender = 1
                                    WHERE Id = @UserMessageId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = userMessageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the user message for recipient.
        /// </summary>
        /// <param name="userMessageId">The user message identifier.</param>
        public void DeleteUserMessageForRecipient(int userMessageId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.UserMessages
                                    SET IsDeletedByRecipient = 1
                                    WHERE Id = @UserMessageId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = userMessageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
