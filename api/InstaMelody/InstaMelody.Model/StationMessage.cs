using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class StationMessage
    {
        public int Id { get; set; }

        public int StationId { get; set; }

        public Guid MessageId { get; set; }

        public bool IsPrivate { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region RelationshipProperties

        public Message Message { get; set; }

        #endregion RelationshipProperties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public StationMessage ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.StationId = Convert.ToInt32(dataReader["StationId"]);
            this.MessageId = (Guid) dataReader["MessageId"];
            this.IsPrivate = Convert.ToBoolean(dataReader["IsPrivate"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.DateModified = Convert.ToDateTime(dataReader["DateModified"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
