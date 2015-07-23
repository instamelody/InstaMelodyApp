using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;
using InstaMelody.Model.Enums;

namespace InstaMelody.Data
{
    public class FileUploadTokens : DataAccess
    {
        /// <summary>
        /// Creates the token.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="expires">The expires.</param>
        /// <returns></returns>
        public FileUploadToken CreateToken(Guid userId, string fileName, FileUploadTypeEnum mediaType, DateTime expires)
        {
            var token = Guid.NewGuid();

            var query = @"INSERT INTO dbo.FileUploadTokens
                        (Token, UserId, FileName, MediaType, DateExpires, DateCreated)
                        VALUES (@Token, @UserId, @FileName, @MediaType, @DateExpires, @DateCreated)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = fileName,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "MediaType",
                    Value = mediaType.ToString(),
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateExpires",
                    Value = expires,
                    SqlDbType = SqlDbType.DateTime,
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
            return GetTokenDetails(token);
        }

        /// <summary>
        /// Gets the token details.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public FileUploadToken GetTokenDetails(Guid token)
        {
            var query = @"SELECT TOP 1 * FROM dbo.FileUploadTokens
                        WHERE Token = @Token";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<FileUploadToken>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the tokens for user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<FileUploadToken> GetTokensForUserId(Guid userId)
        {
            var query = @"SELECT * FROM dbo.FileUploadTokens
                        WHERE UserId = @UserId";

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

            return GetRecordSet<FileUploadToken>(query, parameters.ToArray());
        }

        /// <summary>
        /// Finds the token.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns></returns>
        public FileUploadToken FindToken(Guid userId, string fileName, MediaTypeEnum mediaType)
        {
            var query = @"SELECT TOP 1 * FROM dbo.FileUploadTokens
                        WHERE UserId = @UserId AND MediaType = @MediaType 
                        AND FileName = @FileName AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "MediaType",
                    Value = mediaType.ToString(),
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = fileName,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<FileUploadToken>(query, parameters.ToArray());
        }

        /// <summary>
        /// Expires the token.
        /// </summary>
        /// <param name="token">The token.</param>
        public void ExpireToken(Guid token)
        {
            var query = @"UPDATE dbo.FileUploadTokens
                        SET IsDeleted = 1
                        WHERE Token = @Token";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the old tokens.
        /// </summary>
        public void DeleteOldTokens()
        {
            var query = @"DELETE FROM dbo.FileUploadTokens
                        WHERE DateExpires < @DateExpires";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "DateExpires",
                    Value = DateTime.UtcNow.AddDays(-1),
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }
    }
}
