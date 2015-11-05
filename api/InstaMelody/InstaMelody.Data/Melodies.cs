using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
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
            var query = @"INSERT INTO dbo.Melodies
                        (Name, Description, FileName, IsUserCreated, IsPremiumContent, DateCreated, DateModified)
                        VALUES (@Name, @Description, @FileName, @IsUserCreated, @IsPremiumContent, @DateCreated, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = melody.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Description",
                    Value = string.IsNullOrWhiteSpace(melody.Description)
                        ? DBNull.Value
                        : (object)melody.Description,
                    SqlDbType = SqlDbType.Text,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = melody.FileName,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "IsUserCreated",
                    Value = melody.IsUserCreated,
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "IsPremiumContent",
                    Value = melody.IsPremiumContent,
                    SqlDbType = SqlDbType.Bit,
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
                return GetMelodyById(Convert.ToInt32(obj));
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
            var query = @"SELECT TOP 1 * FROM dbo.Melodies
                        WHERE IsDeleted = 0 AND Id = @MelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<Melody>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the name of the melody by file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public Melody GetMelodyByFileName(string fileName)
        {
            var query = @"SELECT TOP 1 * FROM dbo.Melodies
                        WHERE IsDeleted = 0 AND UPPER(FileName) = @FileName";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = fileName.ToUpper(),
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<Melody>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets all base melodies.
        /// </summary>
        /// <returns></returns>
        public IList<Melody> GetAllBaseMelodies()
        {
            var query = @"SELECT * FROM dbo.Melodies
                        WHERE IsDeleted = 0 AND IsUserCreated = 0";

            return GetRecordSet<Melody>(query);
        }

        /// <summary>
        /// Gets the base melodies by file group identifier.
        /// </summary>
        /// <param name="fileGroupId">The file group identifier.</param>
        /// <returns></returns>
        public IList<Melody> GetBaseMelodiesByFileGroupId(int fileGroupId)
        {
            var query = @"SELECT m.* FROM dbo.Melodies m
                        JOIN dbo.MelodyFileGroups f
                        ON m.Id = f.MelodyId
                        WHERE m.IsDeleted = 0 AND f.IsDeleted = 0
                        AND m.IsUserCreated = 0
                        AND f.FileGroupId = @FileGroupId";

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

            return GetRecordSet<Melody>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the base melodies by file group identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        public IList<Melody> GetBaseMelodiesByCategoryId(int categoryId)
        {
            var query = @"SELECT m.* FROM dbo.Melodies m
                        JOIN dbo.MelodyCategories c
                        ON m.Id = c.MelodyId
                        WHERE m.IsDeleted = 0 AND c.IsDeleted = 0
                        AND m.IsUserCreated = 0
                        AND c.CategoryId = @CategoryId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = categoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<Melody>(query, parameters.ToArray());
        }

        /// <summary>
        /// Updates the melody.
        /// </summary>
        /// <param name="melody">The melody.</param>
        /// <returns></returns>
        public Melody UpdateMelody(Melody melody)
        {
            var query = @"UPDATE dbo.Melodies
                        SET Name = @Name, Description = @Description, FileName = @FileName,
                        IsUserCreated = @IsUserCreated, IsPremiumContent = @IsPremiumContent, 
                        DateModified = @DateModified
                        WHERE IsDeleted = 0 AND Id = @MelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melody.Id,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = melody.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Description",
                    Value = string.IsNullOrWhiteSpace(melody.Description)
                        ? DBNull.Value
                        : (object)melody.Description,
                    SqlDbType = SqlDbType.Text,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "FileName",
                    Value = melody.FileName,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "IsUserCreated",
                    Value = melody.IsUserCreated,
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "IsPremiumContent",
                    Value = melody.IsPremiumContent,
                    SqlDbType = SqlDbType.Bit,
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

            return GetMelodyById(melody.Id);
        }

        /// <summary>
        /// Deletes the melody.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        public void DeleteMelody(int melodyId)
        {
            var query = @"UPDATE dbo.Melodies
                        SET IsDeleted = 1, DateModified = @DateModified
                        WHERE IsDeleted = 0 AND Id = @MelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
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

            DeleteMelodyCategoriesByMelodyId(melodyId);
            DeleteMelodyFileGroupsByMelodyId(melodyId);
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
            var query = @"INSERT INTO dbo.MelodyCategories
                        (MelodyId, CategoryId, DateCreated)
                        VALUES (@MelodyId, @CategoryId, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyCategory.MelodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = melodyCategory.CategoryId,
                    SqlDbType = SqlDbType.Int,
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
                return GetMelodyCategoryById(Convert.ToInt32(obj));
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
            var query = @"SELECT TOP 1 * FROM dbo.MelodyCategories
                        WHERE IsDeleted = 0 AND Id = @MelodyCategoryId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyCategoryId",
                    Value = melodyCategoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<MelodyCategory>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the melody categories by melody identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        /// <returns></returns>
        public IList<MelodyCategory> GetMelodyCategoriesByMelodyId(int melodyId)
        {
            var query = @"SELECT * FROM dbo.MelodyCategories
                        WHERE IsDeleted = 0 AND MelodyId = @MelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<MelodyCategory>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the melody categories by category identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        public IList<MelodyCategory> GetMelodyCategoriesByCategoryId(int categoryId)
        {
            var query = @"SELECT * FROM dbo.MelodyCategories
                        WHERE IsDeleted = 0 AND CategoryId = @CategoryId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = categoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<MelodyCategory>(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the melody categories by melody identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        private void DeleteMelodyCategoriesByMelodyId(int melodyId)
        {
            var query = @"UPDATE dbo.MelodyCategories
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND MelodyId = @MelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the melody categories by category identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        internal void DeleteMelodyCategoriesByCategoryId(int categoryId)
        {
            var query = @"UPDATE dbo.MelodyCategories
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND CategoryId = @CategoryId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = categoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
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
            var query = @"INSERT INTO dbo.MelodyFileGroups
                        (MelodyId, FileGroupId, DateCreated)
                        VALUES (@MelodyId, @FileGroupId, @DateCreated)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyFileGroup.MelodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "FileGroupId",
                    Value = melodyFileGroup.FileGroupId,
                    SqlDbType = SqlDbType.Int,
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
                return GetMelodyFileGroupById(Convert.ToInt32(obj));
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
            var query = @"SELECT TOP 1 * FROM dbo.MelodyFileGroups
                        WHERE IsDeleted = 0 AND Id = @MelodyFileGroupId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyFileGroupId",
                    Value = melodyFileGroupId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecord<MelodyFileGroup>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the melody file groups by melody identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        /// <returns></returns>
        public IList<MelodyFileGroup> GetMelodyFileGroupsByMelodyId(int melodyId)
        {
            var query = @"SELECT * FROM dbo.MelodyFileGroups
                        WHERE IsDeleted = 0 AND MelodyId = @MelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<MelodyFileGroup>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the melody file groups by file group identifier.
        /// </summary>
        /// <param name="fileGroupId">The file group identifier.</param>
        /// <returns></returns>
        public IList<MelodyFileGroup> GetMelodyFileGroupsByFileGroupId(int fileGroupId)
        {
            var query = @"SELECT * FROM dbo.MelodyFileGroups
                        WHERE IsDeleted = 0 AND FileGroupId = @FileGroupId";

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

            return GetRecordSet<MelodyFileGroup>(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the melody file groups by melody identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        private void DeleteMelodyFileGroupsByMelodyId(int melodyId)
        {
            var query = @"UPDATE dbo.MelodyFileGroups
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND MelodyId = @MelodyId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = melodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            ExecuteNonQuery(query, parameters.ToArray());
        }

        /// <summary>
        /// Deletes the melody file groups by file group identifier.
        /// </summary>
        /// <param name="fileGroupId">The file group identifier.</param>
        internal void DeleteMelodyFileGroupsByFileGroupId(int fileGroupId)
        {
            var query = @"UPDATE dbo.MelodyFileGroups
                        SET IsDeleted = 1
                        WHERE IsDeleted = 0 AND FileGroupId = @FileGroupId";

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

            ExecuteNonQuery(query, parameters.ToArray());
        }

        #endregion MelodyFileGroups
    }
}
