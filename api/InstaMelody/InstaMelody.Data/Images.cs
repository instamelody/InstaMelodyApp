using System;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;
using System.Collections.Generic;

namespace InstaMelody.Data
{
    public class Images : DataAccess
    {
        /// <summary>
        /// Creates the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public Image CreateImage(Image image)
        {
            var query = @"INSERT INTO dbo.Images
                        (FileName, DateCreated)
                        VALUES (@FileName, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = image.FileName,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = image.DateCreated > DateTime.MinValue 
                        ? image.DateCreated 
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            var obj = ExecuteScalar(query, parameters.ToArray());
            if (!Convert.IsDBNull(obj))
            {
                return this.GetImageById(Convert.ToInt32(obj));
            }

            throw new DataException("Failed to create a new Image.");
        }

        /// <summary>
        /// Gets the image by identifier.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        public Image GetImageById(int imageId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Images
                        WHERE Id = @ImageId AND IsDeleted = 0";

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

            return GetRecord<Image>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the name of the image by file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public Image GetImageByFileName(string fileName)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Images
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

            return GetRecord<Image>(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the image.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        public void DeleteImage(int imageId)
        {
            var query = @"UPDATE dbo.Images
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND Id = @ImageId";

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
    }
}
