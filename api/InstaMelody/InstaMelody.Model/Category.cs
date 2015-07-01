using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class Category
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public IList<Category> Children { get; set; } 

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public Category ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.ParentId = dataReader["ParentId"] is DBNull ? 
                null : 
                (int?)Convert.ToInt32(dataReader["ParentId"]);
            this.Name = Convert.ToString(dataReader["Name"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.DateModified = Convert.ToDateTime(dataReader["DateModified"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
