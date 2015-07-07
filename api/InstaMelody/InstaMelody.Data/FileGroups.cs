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
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.FileGroups
                                    (Name, DateCreated, DateModified)
                                    VALUES (@Name, @DateCreated, @DateCreated)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Name",
                    Value = name,
                    SqlDbType = SqlDbType.VarChar,
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
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    return this.GetFileGroupById(Convert.ToInt32(obj));
                }
            }

            throw new DataException("Failed to create a new File Group.");
        }

        /// <summary>
        /// Gets the file groups.
        /// </summary>
        /// <returns></returns>
        public IList<FileGroup> GetFileGroups()
        {
            List<FileGroup> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.FileGroups
                                    WHERE IsDeleted = 0";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<FileGroup>();
                        while (reader.Read())
                        {
                            var result = new FileGroup();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the file group by identifier.
        /// </summary>
        /// <param name="fileGroupId">The file group identifier.</param>
        /// <returns></returns>
        public FileGroup GetFileGroupById(int fileGroupId)
        {
            FileGroup result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.FileGroups
                                    WHERE IsDeleted = 0 AND Id = @FileGroupId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "FileGroupId",
                    Value = fileGroupId,
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
                            result = new FileGroup();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the name of the file group by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public FileGroup GetFileGroupByName(string name)
        {
            FileGroup result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.FileGroups
                                    WHERE IsDeleted = 0 AND Name = @Name";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Name",
                    Value = name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = new FileGroup();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the file group.
        /// </summary>
        /// <param name="fileGroup">The file group.</param>
        /// <returns></returns>
        public FileGroup UpdateFileGroup(FileGroup fileGroup)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.FileGroups
                                    SET Name = @Name, DateModified = @DateModified
                                    WHERE Id = @FileGroupId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "FileGroupId",
                    Value = fileGroup.Id,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Name",
                    Value = fileGroup.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = fileGroup.DateModified > DateTime.MinValue
                        ? fileGroup.DateModified
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return this.GetFileGroupById(fileGroup.Id);
        }

        /// <summary>
        /// Deletes the file group.
        /// </summary>
        /// <param name="fileGroupId">The file group identifier.</param>
        public void DeleteFileGroup(int fileGroupId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.FileGroups
                                    SET IsDeleted = 1
                                    WHERE Id = @FileGroupId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "FileGroupId",
                    Value = fileGroupId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            var melodyDal = new Melodies();
            melodyDal.DeleteMelodyFileGroupsByFileGroupId(fileGroupId);
        }
    }
}
