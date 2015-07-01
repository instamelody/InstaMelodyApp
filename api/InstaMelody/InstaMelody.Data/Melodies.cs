using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class Melodies : DataAccess
    {
        /// <summary>
        /// Creates the melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public Melody CreateMelody(Melody melody)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.Melodies
                                    (BaseMelodyId, UserId, IsUserMelody, Name, Description, FileName, DateCreated, DateModified)
                                    VALUES (@BaseId, @UserId, @IsUserMelody, @Name, @Description, @FileName, @DateCreated, @DateCreated)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "BaseId",
                    Value = melody.BaseMelodyId != null
                        ? (object)melody.BaseMelodyId
                        : DBNull.Value,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = melody.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "IsUserMelody",
                    Value = (melody.UserId != null && !melody.UserId.Equals(default(Guid))),
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Name",
                    Value = melody.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Description",
                    Value = string.IsNullOrWhiteSpace(melody.Description) 
                        ? DBNull.Value 
                        : (object)melody.Description,
                    SqlDbType = SqlDbType.Text,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = melody.FileName,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = melody.DateCreated > DateTime.MinValue
                        ? melody.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    return this.GetMelodyById(Convert.ToInt32(obj));
                }
            }

            throw new DataException();
        }

        /// <summary>
        /// Gets the melody by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Melody GetMelodyById(int id)
        {
            Melody result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.Melodies
                                    WHERE Id = @Id AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Id",
                    Value = id,
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
                            result = new Melody();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the name of the melody by file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public Melody GetMelodyByFileName(string fileName)
        {
            Melody result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.Melodies
                                    WHERE FileName = @FileName AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = fileName,
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
                            result = new Melody();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the name of the melody by user identifier and file.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public Melody GetMelodyByUserIdAndFileName(Guid userId, string fileName)
        {
            Melody result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.Melodies
                                    WHERE FileName = @FileName AND UserId = @UserId AND IsDeleted = 0";

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

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = new Melody();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the melodies by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<Melody> GetMelodiesByUserId(Guid userId)
        {
            List<Melody> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.Melodies
                                    WHERE IsDeleted = 0 AND IsUserMelody = 1 
                                    AND UserId = @UserId";

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
                        results = new List<Melody>();
                        while (reader.Read())
                        {
                            var melody = new Melody();
                            melody = melody.ParseFromDataReader(reader);
                            results.Add(melody);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the melodies by user identifier and category.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        public IList<Melody> GetMelodiesByUserIdAndCategoryId(Guid userId, int categoryId)
        {
            List<Melody> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT m.* FROM dbo.Melodies m JOIN dbo.MelodyCategories c
                                    ON m.Id = c.MelodyId
                                    WHERE m.IsDeleted = 0 AND c.IsDeleted = 0 
                                    AND m.IsUserMelody = 1
1                                   AND m.UserId = @UserId
                                    AND c.CategoryId = @CategoryId";
                
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = categoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<Melody>();
                        while (reader.Read())
                        {
                            var melody = new Melody();
                            melody = melody.ParseFromDataReader(reader);
                            results.Add(melody);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the base melodies.
        /// </summary>
        /// <returns></returns>
        public IList<Melody> GetBaseMelodies()
        {
            List<Melody> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.Melodies
                                    WHERE IsDeleted = 0 AND IsUserMelody = 0";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<Melody>();
                        while (reader.Read())
                        {
                            var melody = new Melody();
                            melody = melody.ParseFromDataReader(reader);
                            results.Add(melody);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the melodies by category identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        public IList<Melody> GetBaseMelodiesByCategoryId(int categoryId)
        {
            List<Melody> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT m.* FROM dbo.Melodies m JOIN dbo.MelodyCategories c
                                    ON m.Id = c.MelodyId
                                    WHERE m.IsDeleted = 0 AND c.IsDeleted = 0 
                                    AND m.IsUserMelody = 0
                                    AND c.CategoryId = @CategoryId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = categoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<Melody>();
                        while (reader.Read())
                        {
                            var melody = new Melody();
                            melody = melody.ParseFromDataReader(reader);
                            results.Add(melody);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Updates the melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public Melody UpdateMelody(Melody melody)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.Melodies
                                    SET BaseMelodyId = @BaseId, UserId = @UserId,
                                        IsUserMelody = @IsUserMelody, Name = @Name,
                                        Description = @Description, FileName = @FileName, 
                                        DateModified = @DateModified
                                    WHERE Id = @MelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melody.Id,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "BaseId",
                    Value = melody.BaseMelodyId != null
                        ? (object)melody.BaseMelodyId
                        : DBNull.Value,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = melody.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "IsUserMelody",
                    Value = (melody.UserId != null && !melody.UserId.Equals(default(Guid))),
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Name",
                    Value = melody.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Description",
                    Value = string.IsNullOrWhiteSpace(melody.Description)
                        ? DBNull.Value
                        : (object)melody.Description,
                    SqlDbType = SqlDbType.Text,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = melody.FileName,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = melody.DateModified > DateTime.MinValue
                        ? melody.DateModified
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    return this.GetMelodyById(Convert.ToInt32(obj));
                }
            }

            throw new DataException();
        }

        /// <summary>
        /// Deletes the melody.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        public void DeleteMelody(int melodyId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.Melodies
                                    SET IsDeleted = 1, DateModified = @DateModified
                                    WHERE IsDeleted = 0 AND Id = @MelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
