using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class UserSession
    {
        public Guid Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public UserSession ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Token = (Guid)dataReader["Token"];
            this.UserId = (Guid)dataReader["UserId"];
            this.LastActivity = Convert.ToDateTime(dataReader["LastActivity"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
