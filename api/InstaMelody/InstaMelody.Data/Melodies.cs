using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    // TODO: refactor DAL
    public class Melodies : DataAccess
    {
        #region Melodies

        /// <summary>
        /// Creates the melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to create a new Melody.</exception>
        public Melody CreateMelody(Melody melody)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.Melodies
                                    (Name, Description, FileName, IsUserCreated, DateCreated, DateModified)
                                    VALUES (@Name, @Description, @FileName, @IsUserCreated, @DateCreated, @DateCreated)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

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
                    ParameterName = "IsUserCreated",
                    Value = melody.IsUserCreated,
                    SqlDbType = SqlDbType.Bit,
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

            throw new DataException("Failed to create a new Melody.");
        }

        /// <summary>
        /// Gets the melody by identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        /// <returns></returns>
        public Melody GetMelodyById(int melodyId)
        {
            Melody result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.Melodies
                                    WHERE IsDeleted = 0 AND Id = @MelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
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
                                    WHERE IsDeleted = 0 AND UPPER(FileName) = @FileName";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = fileName.ToUpper(),
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
        /// Gets all base melodies.
        /// </summary>
        /// <returns></returns>
        public IList<Melody> GetAllBaseMelodies()
        {
            List<Melody> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.Melodies
                                    WHERE IsDeleted = 0 AND IsUserCreated = 0";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<Melody>();
                        while (reader.Read())
                        {
                            var result = new Melody();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the base melodies by file group identifier.
        /// </summary>
        /// <param name="fileGroupId">The file group identifier.</param>
        /// <returns></returns>
        public IList<Melody> GetBaseMelodiesByFileGroupId(int fileGroupId)
        {
            List<Melody> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT m.* FROM dbo.Melodies m
                                    JOIN dbo.MelodyFileGroups f
                                    ON m.Id = f.MelodyId
                                    WHERE m.IsDeleted = 0 AND f.IsDeleted = 0
                                        AND m.IsUserCreated = 0
                                        AND f.FileGroupId = @FileGroupId";

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
                        results = new List<Melody>();
                        while (reader.Read())
                        {
                            var result = new Melody();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the base melodies by file group identifier.
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
                cmd.CommandText = @"SELECT m.* FROM dbo.Melodies m
                                    JOIN dbo.MelodyCategories c
                                    ON m.Id = c.MelodyId
                                    WHERE m.IsDeleted = 0 AND c.IsDeleted = 0
                                        AND m.IsUserCreated = 0
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
                            var result = new Melody();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
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
        public Melody UpdateMelody(Melody melody)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.Melodies
                                    SET Name = @Name, Description = @Description, FileName = @FileName,
                                        IsUserCreated = @IsUserCreated, DateModified = @DateModified
                                    WHERE IsDeleted = 0 AND Id = @MelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melody.Id,
                    SqlDbType = SqlDbType.Int,
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
                    ParameterName = "IsUserCreated",
                    Value = melody.IsUserCreated,
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = melody.DateCreated > DateTime.MinValue
                        ? melody.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();

            }

            return this.GetMelodyById(melody.Id);
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

            this.DeleteMelodyCategoriesByMelodyId(melodyId);
            this.DeleteMelodyFileGroupsByMelodyId(melodyId);
        }

        #endregion Melodies

        #region MelodyCategories

        /// <summary>
        /// Creates the melody category.
        /// </summary>
        /// <param name="melodyCategory">The melody category.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to create a new Melody Category.</exception>
        public MelodyCategory CreateMelodyCategory(MelodyCategory melodyCategory)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.MelodyCategories
                                    (MelodyId, CategoryId, DateCreated)
                                    VALUES (@MelodyId, @CategoryId, @DateCreated)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyCategory.MelodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = melodyCategory.CategoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = melodyCategory.DateCreated > DateTime.MinValue
                        ? melodyCategory.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    return this.GetMelodyCategoryById(Convert.ToInt32(obj));
                }
            }

            throw new DataException("Failed to create a new Melody Category.");
        }

        /// <summary>
        /// Gets the melody category by identifier.
        /// </summary>
        /// <param name="melodyCategoryId">The melody category identifier.</param>
        /// <returns></returns>
        public MelodyCategory GetMelodyCategoryById(int melodyCategoryId)
        {
            MelodyCategory result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.MelodyCategories
                                    WHERE IsDeleted = 0 AND Id = @MelodyCategoryId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyCategoryId",
                    Value = melodyCategoryId,
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
                            result = new MelodyCategory();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the melody categories by melody identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        /// <returns></returns>
        public IList<MelodyCategory> GetMelodyCategoriesByMelodyId(int melodyId)
        {
            List<MelodyCategory> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.MelodyCategories
                                    WHERE IsDeleted = 0 AND MelodyId = @MelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<MelodyCategory>();
                        while (reader.Read())
                        {
                            var result = new MelodyCategory();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the melody categories by category identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        public IList<MelodyCategory> GetMelodyCategoriesByCategoryId(int categoryId)
        {
            List<MelodyCategory> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.MelodyCategories
                                    WHERE IsDeleted = 0 AND CategoryId = @CategoryId";

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
                        results = new List<MelodyCategory>();
                        while (reader.Read())
                        {
                            var result = new MelodyCategory();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Deletes the melody categories by melody identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        private void DeleteMelodyCategoriesByMelodyId(int melodyId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MelodyCategories
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND MelodyId = @MelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the melody categories by category identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        internal void DeleteMelodyCategoriesByCategoryId(int categoryId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MelodyCategories
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND CategoryId = @CategoryId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = categoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        #endregion MelodyCategories

        #region MelodyFileGroups

        /// <summary>
        /// Creates the melody file group.
        /// </summary>
        /// <param name="melodyFileGroup">The melody file group.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">Failed to create a new Melody File Group.</exception>
        public MelodyFileGroup CreateMelodyFileGroup(MelodyFileGroup melodyFileGroup)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.MelodyFileGroups
                                    (MelodyId, FileGroupId, DateCreated)
                                    VALUES (@MelodyId, @FileGroupId, @DateCreated)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyFileGroup.MelodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "FileGroupId",
                    Value = melodyFileGroup.FileGroupId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = melodyFileGroup.DateCreated > DateTime.MinValue
                        ? melodyFileGroup.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    return this.GetMelodyFileGroupById(Convert.ToInt32(obj));
                }
            }

            throw new DataException("Failed to create a new Melody File Group.");
        }

        /// <summary>
        /// Gets the melody file group by identifier.
        /// </summary>
        /// <param name="melodyFileGroupId">The melody file group identifier.</param>
        /// <returns></returns>
        public MelodyFileGroup GetMelodyFileGroupById(int melodyFileGroupId)
        {
            MelodyFileGroup result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.MelodyFileGroups
                                    WHERE IsDeleted = 0 AND Id = @MelodyFileGroupId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyFileGroupId",
                    Value = melodyFileGroupId,
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
                            result = new MelodyFileGroup();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the melody file groups by melody identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        /// <returns></returns>
        public IList<MelodyFileGroup> GetMelodyFileGroupsByMelodyId(int melodyId)
        {
            List<MelodyFileGroup> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.MelodyFileGroups
                                    WHERE IsDeleted = 0 AND MelodyId = @MelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<MelodyFileGroup>();
                        while (reader.Read())
                        {
                            var result = new MelodyFileGroup();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the melody file groups by file group identifier.
        /// </summary>
        /// <param name="fileGroupId">The file group identifier.</param>
        /// <returns></returns>
        public IList<MelodyFileGroup> GetMelodyFileGroupsByFileGroupId(int fileGroupId)
        {
            List<MelodyFileGroup> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.MelodyFileGroups
                                    WHERE IsDeleted = 0 AND FileGroupId = @FileGroupId";

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
                        results = new List<MelodyFileGroup>();
                        while (reader.Read())
                        {
                            var result = new MelodyFileGroup();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Deletes the melody file groups by melody identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        private void DeleteMelodyFileGroupsByMelodyId(int melodyId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MelodyFileGroups
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND MelodyId = @MelodyId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the melody file groups by file group identifier.
        /// </summary>
        /// <param name="fileGroupId">The file group identifier.</param>
        internal void DeleteMelodyFileGroupsByFileGroupId(int fileGroupId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MelodyFileGroups
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND FileGroupId = @FileGroupId";

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
        }

        #endregion MelodyFileGroups
    }
}
