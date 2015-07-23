using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    // TODO: refactor DAL
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

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.Messages
                                    (Id, ParentId, Description, MediaType, DateCreated)
                                    VALUES (@MessageId, @ParentId, @Description, @MediaType, @DateCreated)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = message.Id,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = (message.ParentId.Equals(default(Guid)) || message.ParentId == null)
                        ? DBNull.Value
                        : (object)message.ParentId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Description",
                    Value = (object)message.Description ?? DBNull.Value,
                    SqlDbType = SqlDbType.Text,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MediaType",
                    Value = message.MediaType.ToString(),
                    SqlDbType = SqlDbType.VarChar,
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
                cmd.ExecuteNonQuery();
            }

            return this.GetMessageById(message.Id);
        }

        /// <summary>
        /// Gets the message by identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public Message GetMessageById(Guid messageId)
        {
            Message result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.Messages
                                    WHERE Id = @MessageId AND IsDeleted = 0";

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
                            result = new Message();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the top level message by identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public Message GetTopLevelMessageById(Guid messageId)
        {
            Message result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.Messages
                                    WHERE Id = @MessageId AND IsDeleted = 0 AND ParentId IS NULL";

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
                            result = new Message();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the messages by parent message identifier.
        /// </summary>
        /// <param name="parentMessageId">The parent message identifier.</param>
        /// <returns></returns>
        public IList<Message> GetMessagesByParentMessageId(Guid parentMessageId)
        {
            List<Message> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.Messages
                                    WHERE ParentId = @ParentId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = parentMessageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<Message>();

                        while (reader.Read())
                        {
                            var result = new Message();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Marks the messag as read.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <exception cref="System.Data.DataException"></exception>
        public void MarkMessagAsRead(Guid messageId)
        {
            var message = this.GetMessageById(messageId);
            if (message == null)
            {
                throw new DataException();
            }

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.Messages
                                    SET IsRead = 1, DateRead = @DateRead
                                    WHERE Id = @MessageId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = message.Id,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateRead",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        public void DeleteMessage(Guid messageId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.Messages
                                    SET IsDeleted = 1
                                    WHERE Id = @MessageId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        #region MessageImages

        /// <summary>
        /// Gets the image by message identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public MessageImage GetImageByMessageId(Guid messageId)
        {
            MessageImage result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.MessageImages
                                    WHERE MessageId = @MessageId AND IsDeleted = 0";

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
                            result = new MessageImage();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the message image.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        public MessageImage AddMessageImage(Guid messageId, int imageId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.MessageImages
                                    (MessageId, ImageId, DateCreated, IsDeleted)
                                    VALUES (@MessageId, @ImageId, @DateCreated, 0)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ImageId",
                    Value = imageId,
                    SqlDbType = SqlDbType.Int,
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

            return this.GetImageByMessageId(messageId);
        }

        /// <summary>
        /// Deletes the message image by message identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        public void DeleteMessageImageByMessageId(Guid messageId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MessageImages
                                    SET IsDeleted = 1
                                    WHERE MessageId = @MessageId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the message image by image identifier.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        public void DeleteMessageImageByImageId(int imageId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MessageImages
                                    SET IsDeleted = 1
                                    WHERE ImageId = @ImageId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ImageId",
                    Value = imageId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
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
            MessageVideo result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.MessageVideos
                                    WHERE MessageId = @MessageId AND IsDeleted = 0";

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
                            result = new MessageVideo();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the message video.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="videoId">The video identifier.</param>
        /// <returns></returns>
        public MessageVideo AddMessageVideo(Guid messageId, int videoId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.MessageVideos
                                    (MessageId, VideoId, DateCreated, IsDeleted)
                                    VALUES (@MessageId, @VideoId, @DateCreated, 0)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "VideoId",
                    Value = videoId,
                    SqlDbType = SqlDbType.Int,
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

            return this.GetVideoByMessageId(messageId);
        }

        /// <summary>
        /// Deletes the message video by message identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        public void DeleteMessageVideoByMessageId(Guid messageId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MessageVideos
                                    SET IsDeleted = 1
                                    WHERE MessageId = @MessageId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the message video by image identifier.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        public void DeleteMessageVideoByVideoId(int videoId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MessageVideos
                                    SET IsDeleted = 1
                                    WHERE VideoId = @VideoId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "VideoId",
                    Value = videoId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
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
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.MessageMelodies
                                    (MessageId, UserMelodyId, DateCreated, IsDeleted)
                                    VALUES (@MessageId, @UserMelodyId, @DateCreated, 0)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MessageId",
                    Value = messageId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
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

            return this.GetMessageMelodyByMessageId(messageId);
        }

        /// <summary>
        /// Gets the message melody by message identifier.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public MessageMelody GetMessageMelodyByMessageId(Guid messageId)
        {
            MessageMelody result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.MessageMelodies
                                    WHERE MessageId = @MessageId AND IsDeleted = 0";

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
                            result = new MessageMelody();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the message melody by melody identifier.
        /// </summary>
        /// <param name="userMelodyId">The user melody identifier.</param>
        public void DeleteMessageMelodyByMelodyId(Guid userMelodyId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MessageMelodies
                                    SET IsDeleted = 1
                                    WHERE UserMelodyId = @UserMelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserMelodyId",
                    Value = userMelodyId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        #endregion MessageMelodies
    }
}
