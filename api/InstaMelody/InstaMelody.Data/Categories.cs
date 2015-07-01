using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class Categories : DataAccess
    {
        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public Category AddCategory(Category category)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.Categories 
                                    (ParentId, Name, DateCreated, DateModified, IsDeleted)
                                    VALUES(@ParentId, @Name, @DateCreated, @DateCreated, 0)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = (object)category.ParentId ?? DBNull.Value,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Name",
                    Value = category.Name,
                    SqlDbType = SqlDbType.VarChar,
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
                    return this.GetCategoryById(Convert.ToInt32(obj));
                }
            }

            throw new DataException();
        }

        /// <summary>
        /// Updates the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public Category UpdateCategory(Category category)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.Categories 
                                    SET ParentId = @ParentId, Name = @Name, 
                                        DateModified = @DateModified
                                    WHERE Id = @CategoryId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = category.Id,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = (object)category.ParentId ?? DBNull.Value,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Name",
                    Value = category.Name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = category.DateModified > DateTime.MinValue
                        ? category.DateModified
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return this.GetCategoryById(category.Id);
        }

        /// <summary>
        /// Deletes the category.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        public void DeleteCategory(int categoryId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.Categories 
                                    SET IsDeleted = 1, DateModified = @DateModified
                                    WHERE Id = @CategoryId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = categoryId,
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

        /// <summary>
        /// Gets the category by identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        public Category GetCategoryById(int categoryId)
        {
            Category result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.Categories
                                    WHERE Id = @CategoryId AND IsDeleted = 0";

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
                        while (reader.Read())
                        {
                            result = new Category();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the category by name and parent.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns></returns>
        public Category GetCategoryByNameAndParent(string name, int? parentId)
        {
            Category result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.Categories
                                    WHERE Name = @Name AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Name",
                    Value = name,
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });

                if (parentId != null)
                {
                    cmd.CommandText += @"AND ParentId = @ParentId";
                    cmd.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "ParentId",
                        Value = parentId,
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input
                    });
                }
                else
                {
                    cmd.CommandText += @"AND ParentId IS NULL";
                }

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = new Category();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the parent categories.
        /// </summary>
        /// <returns></returns>
        public IList<Category> GetParentCategories()
        {
            var results = new List<Category>();

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.Categories
                                    WHERE ParentId IS NULL AND IsDeleted = 0";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var result = new Category(); 
                            results.Add(result.ParseFromDataReader(reader));
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the child categories.
        /// </summary>
        /// <param name="parentCategoryId">The parent category identifier.</param>
        /// <returns></returns>
        public IList<Category> GetChildCategories(int parentCategoryId)
        {
            var results = new List<Category>();

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.Categories
                                    WHERE ParentId = @ParentId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = parentCategoryId,
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
                            var result = new Category();
                            results.Add(result.ParseFromDataReader(reader));
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <returns></returns>
        public IList<Category> GetAllCategories()
        {
            var results = new List<Category>();

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.Categories
                                    WHERE IsDeleted = 0";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var result = new Category();
                            results.Add(result.ParseFromDataReader(reader));
                        }
                    }
                }
            }

            return results;
        }
    }
}
