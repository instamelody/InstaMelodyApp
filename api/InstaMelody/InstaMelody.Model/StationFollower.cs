using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class StationFollower
    {
        public int Id { get; set; }

        public int StationId { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public User User { get; set; }

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public StationFollower ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.StationId = Convert.ToInt32(dataReader["StationId"]);
            this.UserId = (Guid)dataReader["UserId"];
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
