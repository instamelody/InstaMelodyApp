using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class Video
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "FileName cannot exceed 255 characters")]
        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        public string FilePath { get; set; }

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public Video ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.FileName = Convert.ToString(dataReader["FileName"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
