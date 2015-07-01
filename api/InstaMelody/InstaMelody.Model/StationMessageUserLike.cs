using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class StationMessageUserLike
    {
        public int Id { get; set; }

        public int StationMessageId { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region RelationshipProperties

        public User User { get; set; }

        public StationMessage StationMessage { get; set; }

        #endregion RelationshipProperties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public StationMessageUserLike ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.StationMessageId = Convert.ToInt32(dataReader["StationMessageId"]);
            this.UserId = (Guid)dataReader["UserId"];
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.DateModified = Convert.ToDateTime(dataReader["DateModified"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
