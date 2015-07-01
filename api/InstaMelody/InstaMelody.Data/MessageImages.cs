using System;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class MessageImages : DataAccess
    {
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
    }
}
