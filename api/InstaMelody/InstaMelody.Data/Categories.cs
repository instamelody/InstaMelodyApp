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
            var query = @"INSERT INTO dbo.Categories 
                        (ParentId, Name, DateCreated, DateModified, IsDeleted)
                        VALUES(@ParentId, @Name, @DateCreated, @DateCreated, 0)

                        SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = (object)category.ParentId ?? DBNull.Value,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = category.Name,
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
                return this.GetCategoryById(Convert.ToInt32(obj));
            }

            throw new DataException("Failed to Add a new Category");
        }

        /// <summary>
        /// Updates the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public Category UpdateCategory(Category category)
        {
            var query = @"UPDATE dbo.Categories 
                        SET ParentId = @ParentId, Name = @Name, 
                            DateModified = @DateModified
                        WHERE Id = @CategoryId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = category.Id,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = (object)category.ParentId ?? DBNull.Value,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "Name",
                    Value = category.Name,
                    SqlDbType = SqlDbType.VarChar,
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
            return this.GetCategoryById(category.Id);
        }

        /// <summary>
        /// Deletes the category.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        public void DeleteCategory(int categoryId)
        {
            var query = @"UPDATE dbo.Categories 
                        SET IsDeleted = 1, DateModified = @DateModified
                        WHERE Id = @CategoryId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "CategoryId",
                    Value = categoryId,
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

        /// <summary>
        /// Gets the category by identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        public Category GetCategoryById(int categoryId)
        {
            var query = @"SELECT * FROM dbo.Categories
                        WHERE Id = @CategoryId AND IsDeleted = 0";

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

            return GetRecord<Category>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the category by name and parent.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns></returns>
        public Category GetCategoryByNameAndParent(string name, int? parentId)
        {
            var query = @"SELECT * FROM dbo.Categories
                        WHERE Name = @Name AND IsDeleted = 0";

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

            if (parentId != null)
            {
                query += @"AND ParentId = @ParentId";
                parameters.Add(new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = parentId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
            }
            else
            {
                query += @"AND ParentId IS NULL";
            }

            return GetRecord<Category>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the parent categories.
        /// </summary>
        /// <returns></returns>
        public IList<Category> GetParentCategories()
        {
            var query = @"SELECT * FROM dbo.Categories
                        WHERE ParentId IS NULL AND IsDeleted = 0";

            return GetRecordSet<Category>(query);
        }

        /// <summary>
        /// Gets the child categories.
        /// </summary>
        /// <param name="parentCategoryId">The parent category identifier.</param>
        /// <returns></returns>
        public IList<Category> GetChildCategories(int parentCategoryId)
        {
            var query = @"SELECT * FROM dbo.Categories
                        WHERE ParentId = @ParentId AND IsDeleted = 0";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "ParentId",
                    Value = parentCategoryId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<Category>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <returns></returns>
        public IList<Category> GetAllCategories()
        {
            var query = @"SELECT * FROM dbo.Categories
                        WHERE IsDeleted = 0";

            return GetRecordSet<Category>(query);
        }
    }
}
