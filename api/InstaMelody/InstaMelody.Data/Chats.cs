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
        public Chat CreateChat(string name = null)
        {
            var chatId = Guid.NewGuid();

            var query = @"INSERT INTO dbo.Chats (Id, Name, DateCreated, DateModified)
                        VALUES (@Id, @Name, @DateCreated, @DateCreated)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Id",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = name != null
                        ? (object)name
                        : DBNull.Value,
                    SqlDbType = SqlDbType.VarChar,
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
            return GetChatById(chatId);
        }

        /// <summary>
        /// Updates the chat.
        /// </summary>
        /// <param name="chat">The chat.</param>
        /// <returns></returns>
        public Chat UpdateChat(Chat chat)
        {
            //ChatLoopId
            var query = @"UPDATE dbo.Chats
                        SET DateModified = @DateModified
                        WHERE IsDeleted = 0 AND Id = @ChatId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chat.Id,
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

            if (chat.ChatLoopId != null)
            {
                query = @"UPDATE dbo.Chats
                        SET ChatLoopId = @ChatLoopId, DateModified = @DateModified
                        WHERE IsDeleted = 0 AND Id = @ChatId";

                parameters.Add(new SqlParameter
                    {
                        ParameterName = "ChatLoopId",
                        Value = chat.ChatLoopId,
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Direction = ParameterDirection.Input
                    });
            }

            ExecuteNonQuery(query, parameters.ToArray());
            return GetChatById(chat.Id);
        }

        /// <summary>
        /// Creates the chat message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Could not Create a new ChatMessage</exception>
        public ChatMessage CreateChatMessage(ChatMessage message)
        {
            var query = @"INSERT INTO dbo.ChatMessages
                        (ChatId, MessageId, SenderId, DateCreated)
                        VALUES (@ChatId, @MessageId, @SenderId, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = message.ChatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
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
                    Value = message.SenderId,
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

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj))
            {
                var addedMessage = GetChatMessageById(Convert.ToInt32(obj));
                if (addedMessage != null)
                {
                    UpdateChatDateModified(addedMessage.ChatId);
                    return addedMessage;
                }
            }

            throw new DataException("Could not Create a new ChatMessage");
        }

        /// <summary>
        /// Gets the chat message by identifier.
        /// </summary>
        /// <param name="chatMessageId">The chat message identifier.</param>
        /// <returns></returns>
        public ChatMessage GetChatMessageById(int chatMessageId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.ChatMessages
                        WHERE IsDeleted = 0 AND Id = @ChatMessageId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatMessageId",
                    Value = chatMessageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<ChatMessage>(query, parameters.ToArray());
        }

        /// <summary>
        /// Adds the user to chat.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public ChatUser AddUserToChat(ChatUser user)
        {
            var query = @"INSERT INTO dbo.ChatUsers
                        (UserId, ChatId, DateCreated)
                        VALUES (@UserId, @ChatId, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = user.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = user.ChatId,
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

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj))
            {
                var addedUser = GetChatUserById(Convert.ToInt32(obj));
                if (addedUser != null)
                {
                    UpdateChatDateModified(addedUser.ChatId);
                    return addedUser;
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
            var query = @"SELECT TOP 1 * FROM dbo.Chats
                        Where Id = @ChatId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<Chat>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the chats by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<Chat> GetChatsByUserId(Guid userId)
        {
            var query = @"SELECT c.* FROM dbo.Chats c
                        JOIN dbo.ChatUsers u
                        ON c.Id = u.ChatId
                        WHERE c.IsDeleted = 0 AND u.IsDeleted = 0
                        AND u.UserId = @UserId";

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

            return GetRecordSet<Chat>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the messages by chat.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns></returns>
        public IList<ChatMessage> GetMessagesByChat(Guid chatId)
        {
            var query = @"SELECT * FROM dbo.ChatMessages
                        WHERE ChatId = @ChatId AND IsDeleted = 0
                        ORDER BY DateCreated DESC";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<ChatMessage>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the messages by chat.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public IList<ChatMessage> GetMessagesByChat(Guid chatId, int limit)
        {
            var query = string.Format(@"SELECT TOP {0} * FROM dbo.ChatMessages
                                        WHERE ChatId = @ChatId AND IsDeleted = 0
                                        ORDER BY DateCreated DESC", limit);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<ChatMessage>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the messages by chat.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="fromId">From identifier.</param>
        /// <returns></returns>
        public IList<ChatMessage> GetMessagesByChat(Guid chatId, int limit, int fromId)
        {
            var query = string.Format(@"SELECT TOP {0} * FROM dbo.ChatMessages
                                        WHERE ChatId = @ChatId AND IsDeleted = 0 AND Id > @AfterId
                                        ORDER BY DateCreated DESC", limit);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "AfterId",
                    Value = fromId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<ChatMessage>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the users in chat.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns></returns>
        public IList<ChatUser> GetUsersInChat(Guid chatId)
        {
            var query = @"SELECT * FROM dbo.ChatUsers
                        WHERE ChatId = @ChatId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<ChatUser>(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the chat.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        public void DeleteChat(Guid chatId)
        {
            var query = @"UPDATE dbo.Chats
                        SET IsDeleted = 1, DateModified = @DateModified
                        WHERE IsDeleted = 0 AND Id = @ChatId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
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

            DeleteChatMessages(chatId);
            DeleteChatUsers(chatId);
        }

        /// <summary>
        /// Deletes the chat user.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public void DeleteChatUser(Guid chatId, Guid userId)
        {
            var query = @"UPDATE dbo.ChatUsers
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0
                        AND ChatId = @ChatId AND UserId = @UserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
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
            UpdateChatDateModified(chatId);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the chat user by identifier.
        /// </summary>
        /// <param name="chatUserId">The chat user identifier.</param>
        /// <returns></returns>
        private ChatUser GetChatUserById(int chatUserId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.ChatUsers
                         WHERE IsDeleted = 0 AND Id = @ChatUserId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatUserId",
                    Value = chatUserId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<ChatUser>(query, parameters.ToArray());
        }

        /// <summary>
        /// Updates the chat date modified.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        private void UpdateChatDateModified(Guid chatId)
        {
            var query = @"UPDATE dbo.Chats
                        SET DateModified = @DateModified
                        WHERE IsDeleted = 0 AND Id = @ChatId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
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
        }

        /// <summary>
        /// Deletes the chat messages.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        private void DeleteChatMessages(Guid chatId)
        {
            var query = @"UPDATE dbo.ChatMessages
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND ChatId = @ChatId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the chat users.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        private void DeleteChatUsers(Guid chatId)
        {
            var query = @"UPDATE dbo.ChatUsers
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND ChatId = @ChatId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ChatId",
                    Value = chatId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        #endregion Private Methods
    }
}
