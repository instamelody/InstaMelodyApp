using System;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;
using System.Collections.Generic;

namespace InstaMelody.Data
{
    public class Videos : DataAccess
    {
        /// <summary>
        /// Creates the video.
        /// </summary>
        /// <param name="video">The video.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public Video CreateVideo(Video video)
        {
            var query = @"INSERT INTO dbo.Videos
                        (FileName, DateCreated)
                        VALUES (@FileName, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = video.FileName,
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

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj))
            {
                return this.GetVideoById(Convert.ToInt32(obj));
            }

            throw new DataException("Failed to create a new Video.");
        }

        /// <summary>
        /// Gets the video by identifier.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <returns></returns>
        public Video GetVideoById(int videoId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Videos
                        WHERE Id = @VideoId AND IsDeleted = 0";

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

            return GetRecord<Video>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the name of the video by file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public Video GetVideoByFileName(string fileName)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Videos
                        WHERE FileName = @FileName AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = fileName,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<Video>(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the video.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        public void DeleteVideo(int videoId)
        {
            var query = @"UPDATE dbo.Videos
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND Id = @VideoId";

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
    }
}
