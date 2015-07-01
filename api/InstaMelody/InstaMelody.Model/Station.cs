using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class Station
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int? StationImageId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        [MinLength(5, ErrorMessage = "Name must be at least 5 characters")]
        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Image Image { get; set; }

        public IList<StationMessage> Messages { get; set; }

        public IList<StationFollower> Followers { get; set; }

        public IList<StationCategory> Categories { get; set; } 

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public Station ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.UserId = (Guid) dataReader["UserId"];
            this.StationImageId = Convert.ToInt32(dataReader["StationImageId"]);
            this.Name = Convert.ToString(dataReader["Name"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.DateModified = Convert.ToDateTime(dataReader["DateModified"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
