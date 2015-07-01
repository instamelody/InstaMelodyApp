using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class MelodyCategories : DataAccess
    {
        /// <summary>
        /// Adds the melody category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public MelodyCategory AddMelodyCategory(MelodyCategory category)
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
                    Value = category.MelodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = category.CategoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = category.DateCreated > DateTime.MinValue
                        ? category.DateCreated
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

            throw new DataException();
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
                                    WHERE Id = @Id AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Id",
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
        /// Deletes the melody category.
        /// </summary>
        /// <param name="melodyCategoryId">The melody category identifier.</param>
        public void DeleteMelodyCategory(int melodyCategoryId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MelodyCategories
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND Id = @Id";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Id",
                    Value = melodyCategoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the melody categories by melody identifier.
        /// </summary>
        /// <param name="melodyId">The melody identifier.</param>
        public void DeleteMelodyCategoriesByMelodyId(int melodyId)
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
    }
}
