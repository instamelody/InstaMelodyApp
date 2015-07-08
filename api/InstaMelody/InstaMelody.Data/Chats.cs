using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class Chats : DataAccess
    {
        #region Public Methods

        /// <summary>
        /// Creates the chat.
        /// </summary>
        /// <returns></returns>
        public Chat CreateChat()
        {
            var chatId = Guid.NewGuid();

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.Chats
                                        (Id, DateCreated, DateModified)
                                    VALUES (@Id, @DateCreated, @DateCreated)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Id",
                    Value = chatId,
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

            return this.GetChatById(chatId);
        }

        /// <summary>
        /// Creates the chat message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Could not Create a new ChatMessage</exception>
        public ChatMessage CreateChatMessage(ChatMessage message)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.ChatMessages
                                    (ChatId, MessageId, SenderId, DateCreated)
                                    VALUES (@ChatId, @MessageId, @SenderId, @DateCreated)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = message.ChatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = message.MessageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "SenderId",
                    Value = message.SenderId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = message.DateCreated > DateTime.MinValue
                        ? message.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    var addedMessage = this.GetChatMessageById(Convert.ToInt32(obj));
                    if (addedMessage != null)
                    {
                        this.UpdateChatDateModified(addedMessage.ChatId);
                        return addedMessage;
                    }
                }
            }

            throw new DataException("Could not Create a new ChatMessage");
        }

        /// <summary>
        /// Adds the user to chat.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public ChatUser AddUserToChat(ChatUser user)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.ChatUsers
                                    (UserId, ChatId, DateCreated)
                                    VALUES (@UserId, @ChatId, @DateCreated)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = user.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = user.ChatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = user.DateCreated > DateTime.MinValue
                        ? user.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    var addedUser = this.GetChatUserById(Convert.ToInt32(obj));
                    if (addedUser != null)
                    {
                        this.UpdateChatDateModified(addedUser.ChatId);
                        return addedUser;
                    }
                }
            }

            throw new DataException(string.Format("Could not add User {0} to chat.", user.UserId));
        }

        /// <summary>
        /// Gets the chat by identifier.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns></returns>
        public Chat GetChatById(Guid chatId)
        {
            Chat result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.Chats
                                    Where Id = @ChatId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
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
                            result = new Chat();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the chats by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<Chat> GetChatsByUserId(Guid userId)
        {
            List<Chat> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT c.* FROM dbo.Chats c
                                    JOIN dbo.ChatUsers u
                                    ON c.Id = u.ChatId
                                    WHERE c.IsDeleted = 0 AND u.IsDeleted = 0
                                        AND u.UserId = @UserId";

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
                        results = new List<Chat>();
                        while (reader.Read())
                        {
                            var result = new Chat();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the messages by chat.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns></returns>
        public IList<ChatMessage> GetMessagesByChat(Guid chatId)
        {
            List<ChatMessage> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.ChatMessages
                                    WHERE ChatId = @ChatId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<ChatMessage>();
                        while (reader.Read())
                        {
                            var result = new ChatMessage();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the users in chat.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns></returns>
        public IList<ChatUser> GetUsersInChat(Guid chatId)
        {
            List<ChatUser> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.ChatUsers
                                    WHERE ChatId = @ChatId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<ChatUser>();
                        while (reader.Read())
                        {
                            var result = new ChatUser();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Deletes the chat.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        public void DeleteChat(Guid chatId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.Chats
                                    SET IsDeleted = 1, DateModified = @DateModified
                                    WHERE IsDeleted = 0 AND Id = @ChatId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
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

            this.DeleteChatMessages(chatId);
            this.DeleteChatUsers(chatId);
        }

        /// <summary>
        /// Deletes the chat user.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public void DeleteChatUser(Guid chatId, Guid userId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.ChatUsers
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0
                                        AND ChatId = @ChatId AND UserId = @UserId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            this.UpdateChatDateModified(chatId);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the chat message by identifier.
        /// </summary>
        /// <param name="chatMessageId">The chat message identifier.</param>
        /// <returns></returns>
        private ChatMessage GetChatMessageById(int chatMessageId)
        {
            ChatMessage result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.ChatMessages
                                    WHERE IsDeleted = 0 AND Id = @ChatMessageId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatMessageId",
                    Value = chatMessageId,
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
                            result = new ChatMessage();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the chat user by identifier.
        /// </summary>
        /// <param name="chatUserId">The chat user identifier.</param>
        /// <returns></returns>
        private ChatUser GetChatUserById(int chatUserId)
        {
            ChatUser result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.ChatUsers
                                    WHERE IsDeleted = 0 AND Id = @ChatUserId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatUserId",
                    Value = chatUserId,
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
                            result = new ChatUser();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the chat date modified.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        private void UpdateChatDateModified(Guid chatId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.Chats
                                    SET DateModified = @DateModified
                                    WHERE IsDeleted = 0 AND Id = @ChatId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
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
        /// Deletes the chat messages.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        private void DeleteChatMessages(Guid chatId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.ChatMessages
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND ChatId = @ChatId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the chat users.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        private void DeleteChatUsers(Guid chatId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.ChatUsers
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND ChatId = @ChatId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        #endregion Private Methods
    }
}
