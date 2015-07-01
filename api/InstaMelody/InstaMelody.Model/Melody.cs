using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class Melody
    {
        public int Id { get; set; }

        public int? BaseMelodyId { get; set; }

        public Guid? UserId { get; set; }

        public bool IsUserMelody { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "File name is required.")]
        [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters")]
        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public string FilePath { get; set; }

        public Melody BaseMelody { get; set; }

        public IList<Category> Categories { get; set; } 

        #endregion Relationship Properties

        /// <summary>
        /// Clones the specified melody.
        /// </summary>
        /// <returns></returns>
        public Melody Clone()
        {
            var result = (Melody)this.MemberwiseClone();
            return result;
        }

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public Melody ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.BaseMelodyId = Convert.ToInt32(dataReader["BaseMelodyId"]);
            this.UserId = dataReader["UserId"] is DBNull
                ? new Guid()
                : (Guid) dataReader["UserId"];
            this.IsUserMelody = Convert.ToBoolean(dataReader["IsUserMelody"]);
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
