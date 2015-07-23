using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class FileGroups : DataAccess
    {
        /// <summary>
        /// Creates the file group.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to create a new File Group.</exception>
        public FileGroup CreateFileGroup(string name)
        {
            var query = @"INSERT INTO dbo.FileGroups
                        (Name, DateCreated, DateModified)
                        VALUES (@Name, @DateCreated, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = name,
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
                return GetFileGroupById(Convert.ToInt32(obj));
            }

            throw new DataException("Failed to create a new File Group.");
        }

        /// <summary>
        /// Gets the file groups.
        /// </summary>
        /// <returns></returns>
        public IList<FileGroup> GetFileGroups()
        {
            var query = @"SELECT * FROM dbo.FileGroups
                        WHERE IsDeleted = 0";

            return GetRecordSet<FileGroup>(query);
        }

        /// <summary>
        /// Gets the file group by identifier.
        /// </summary>
        /// <param name="fileGroupId">The file group identifier.</param>
        /// <returns></returns>
        public FileGroup GetFileGroupById(int fileGroupId)
        {
            var query = @"SELECT TOP 1 * FROM dbo.FileGroups
                        WHERE IsDeleted = 0 AND Id = @FileGroupId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "FileGroupId",
                    Value = fileGroupId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<FileGroup>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the name of the file group by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public FileGroup GetFileGroupByName(string name)
        {
            var query = @"SELECT TOP 1 * FROM dbo.FileGroups
                        WHERE IsDeleted = 0 AND Name = @Name";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<FileGroup>(query, parameters.ToArray());
        }

        /// <summary>
        /// Updates the file group.
        /// </summary>
        /// <param name="fileGroup">The file group.</param>
        /// <returns></returns>
        public FileGroup UpdateFileGroup(FileGroup fileGroup)
        {
            var query = @"UPDATE dbo.FileGroups
                        SET Name = @Name, DateModified = @DateModified
                        WHERE Id = @FileGroupId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "FileGroupId",
                    Value = fileGroup.Id,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = fileGroup.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = fileGroup.DateModified > DateTime.MinValue
                        ? fileGroup.DateModified
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
            return GetFileGroupById(fileGroup.Id);
        }

        /// <summary>
        /// Deletes the file group.
        /// </summary>
        /// <param name="fileGroupId">The file group identifier.</param>
        public void DeleteFileGroup(int fileGroupId)
        {
            var query = @"UPDATE dbo.FileGroups
                        SET IsDeleted = 1, DateModified = @DateModified
                        WHERE Id = @FileGroupId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "FileGroupId",
                    Value = fileGroupId,
                    SqlDbType = SqlDbType.Int,
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
    }
}
