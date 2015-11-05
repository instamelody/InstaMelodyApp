using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class Messages : DataAccess
    {
        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public Message AddMessage(Message message)
        {
            message.Id = Guid.NewGuid();

            var query = @"INSERT INTO dbo.Messages
                        (Id, ParentId, Description, MediaType, DateCreated)
                        VALUES (@MessageId, @ParentId, @Description, @MediaType, @DateCreated)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = message.Id,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = (message.ParentId.Equals(default(Guid)) || message.ParentId == null)
                        ? DBNull.Value
                        : (object)message.ParentId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Description",
                    Value = (object)message.Description ?? DBNull.Value,
                    SqlDbType = SqlDbType.Text,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "MediaType",
                    Value = message.MediaType.ToString(),
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

            return GetMessageById(message.Id);
        }

        /// <summary>
        /// Gets the message by identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public Message GetMessageById(Guid messageId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Messages
                        WHERE Id = @MessageId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<Message>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the top level message by identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public Message GetTopLevelMessageById(Guid messageId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Messages
                        WHERE Id = @MessageId AND IsDeleted = 0 AND ParentId IS NULL";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<Message>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the messages by parent message identifier.
        /// </summary>
        /// <param name="parentMessageId">The parent message identifier.</param>
        /// <returns></returns>
        public IList<Message> GetMessagesByParentMessageId(Guid parentMessageId)
        {
            var query = @"SELECT * FROM dbo.Messages
                        WHERE ParentId = @ParentId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = parentMessageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<Message>(query, parameters.ToArray());
        }

        /// <summary>
        /// Marks the messag as read.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <exception cref="System.Data.DataException"></exception>
        public void MarkMessagAsRead(Guid messageId)
        {
            var message = GetMessageById(messageId);
            if (message == null)
            {
                throw new DataException();
            }

            var query = @"UPDATE dbo.Messages
                        SET IsRead = 1, DateRead = @DateRead
                        WHERE Id = @MessageId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = message.Id,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateRead",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        public void DeleteMessage(Guid messageId)
        {
            var query = @"UPDATE dbo.Messages
                        SET IsDeleted = 1
                        WHERE Id = @MessageId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        #region MessageImages

        /// <summary>
        /// Gets the image by message identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public MessageImage GetImageByMessageId(Guid messageId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.MessageImages
                        WHERE MessageId = @MessageId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<MessageImage>(query, parameters.ToArray());
        }

        /// <summary>
        /// Adds the message image.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        public MessageImage AddMessageImage(Guid messageId, int imageId)
        {
            var query = @"INSERT INTO dbo.MessageImages
                        (MessageId, ImageId, DateCreated, IsDeleted)
                        VALUES (@MessageId, @ImageId, @DateCreated, 0)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "ImageId",
                    Value = imageId,
                    SqlDbType = SqlDbType.Int,
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

            return GetImageByMessageId(messageId);
        }

        /// <summary>
        /// Deletes the message image by message identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        public void DeleteMessageImageByMessageId(Guid messageId)
        {
            var query = @"UPDATE dbo.MessageImages
                        SET IsDeleted = 1
                        WHERE MessageId = @MessageId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the message image by image identifier.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        public void DeleteMessageImageByImageId(int imageId)
        {
            var query = @"UPDATE dbo.MessageImages
                        SET IsDeleted = 1
                        WHERE ImageId = @ImageId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ImageId",
                    Value = imageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        #endregion MessageImages

        #region MessageVideos

        /// <summary>
        /// Gets the video by message identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public MessageVideo GetVideoByMessageId(Guid messageId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.MessageVideos
                        WHERE MessageId = @MessageId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<MessageVideo>(query, parameters.ToArray());
        }

        /// <summary>
        /// Adds the message video.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="videoId">The video identifier.</param>
        /// <returns></returns>
        public MessageVideo AddMessageVideo(Guid messageId, int videoId)
        {
            var query = @"INSERT INTO dbo.MessageVideos
                        (MessageId, VideoId, DateCreated, IsDeleted)
                        VALUES (@MessageId, @VideoId, @DateCreated, 0)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "VideoId",
                    Value = videoId,
                    SqlDbType = SqlDbType.Int,
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

            return GetVideoByMessageId(messageId);
        }

        /// <summary>
        /// Deletes the message video by message identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        public void DeleteMessageVideoByMessageId(Guid messageId)
        {
            var query = @"UPDATE dbo.MessageVideos
                        SET IsDeleted = 1
                        WHERE MessageId = @MessageId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the message video by image identifier.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        public void DeleteMessageVideoByVideoId(int videoId)
        {
            var query = @"UPDATE dbo.MessageVideos
                        SET IsDeleted = 1
                        WHERE VideoId = @VideoId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "VideoId",
                    Value = videoId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        #endregion MessageVideos

        #region MessageMelodies

        /// <summary>
        /// Adds the message melody.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="userMelodyId">The user melody identifier.</param>
        /// <returns></returns>
        public MessageMelody AddMessageMelody(Guid messageId, Guid userMelodyId)
        {
            var query = @"INSERT INTO dbo.MessageMelodies
                        (MessageId, UserMelodyId, DateCreated, IsDeleted)
                        VALUES (@MessageId, @UserMelodyId, @DateCreated, 0)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
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

            return GetMessageMelodyByMessageId(messageId);
        }

        /// <summary>
        /// Gets the message melody by message identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public MessageMelody GetMessageMelodyByMessageId(Guid messageId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.MessageMelodies
                        WHERE MessageId = @MessageId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<MessageMelody>(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the message melody by melody identifier.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        public void DeleteMessageMelodyByMelodyId(Guid userMelodyId)
        {
            var query = @"UPDATE dbo.MessageMelodies
                        SET IsDeleted = 1
                        WHERE UserMelodyId = @UserMelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        #endregion MessageMelodies
    }
}
