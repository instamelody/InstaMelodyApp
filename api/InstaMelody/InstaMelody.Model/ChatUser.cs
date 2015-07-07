using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class ChatUser
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ChatId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public ChatUser ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.UserId = (Guid)dataReader["UserId"];
            this.UserId = (Guid)dataReader["UserId"];
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
