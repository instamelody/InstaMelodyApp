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

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.FileUploadTokens
                                    (Token, UserId, FileName, MediaType, DateExpires, DateCreated)
                                    VALUES (@Token, @UserId, @FileName, @MediaType, @DateExpires, @DateCreated)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
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
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = fileName,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MediaType",
                    Value = mediaType.ToString(),
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateExpires",
                    Value = expires,
                    SqlDbType = SqlDbType.DateTime,
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
            return this.GetTokenDetails(token);
        }

        /// <summary>
        /// Gets the token details.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public FileUploadToken GetTokenDetails(Guid token)
        {
            FileUploadToken result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.FileUploadTokens
                                    WHERE Token = @Token";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
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
                            result = new FileUploadToken();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the tokens for user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<FileUploadToken> GetTokensForUserId(Guid userId)
        {
            List<FileUploadToken> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.FileUploadTokens
                                    WHERE UserId = @UserId";

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
                        results = new List<FileUploadToken>();
                        while (reader.Read())
                        {
                            var result = new FileUploadToken();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Expires the token.
        /// </summary>
        /// <param name="token">The token.</param>
        public void ExpireToken(Guid token)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.FileUploadTokens
                                    SET IsDeleted = 1
                                    WHERE Token = @Token";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Token",
                    Value = token,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the old tokens.
        /// </summary>
        public void DeleteOldTokens()
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"DELETE FROM dbo.FileUploadTokens
                                    WHERE DateExpires < @DateExpires";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateExpires",
                    Value = DateTime.UtcNow.AddDays(-1),
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
