using System;
using System.Data.SqlClient;
using InstaMelody.Model.Enums;

namespace InstaMelody.Model
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        public string Description { get; set; }

        public MediaTypeEnum MediaType { get; set; }

        public bool IsRead { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Image Image { get; set; }

        public Melody Melody { get; set; }

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public Message ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = (Guid)dataReader["Id"];
            this.ParentId = dataReader["ParentId"] is DBNull
                ? null
                : (Guid?)dataReader["ParentId"];
            this.Description = Convert.ToString(dataReader["Description"]);
            this.MediaType = (MediaTypeEnum) Enum.Parse(typeof(MediaTypeEnum), Convert.ToString(dataReader["MediaType"]));
            this.IsRead = Convert.ToBoolean(dataReader["IsRead"]);
            this.DateRead = dataReader["DateRead"] is DBNull
                ? new DateTime() 
                : Convert.ToDateTime(dataReader["DateRead"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
