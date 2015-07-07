using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class Melody
    {
        public int Id { get; set; }

        public bool IsUserCreated { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "FileName is required.")]
        [StringLength(255, ErrorMessage = "FileName cannot exceed 255 characters")]
        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        public string FilePath { get; set; }

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public Melody ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.IsUserCreated = Convert.ToBoolean(dataReader["IsUserCreated"]);
            this.Name = Convert.ToString(dataReader["Name"]);
            this.Description = Convert.ToString(dataReader["Description"]);
            this.FileName = Convert.ToString(dataReader["FileName"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.DateModified = Convert.ToDateTime(dataReader["DateModified"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
