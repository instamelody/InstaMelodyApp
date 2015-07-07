using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class MessageVideo
    {
        public int Id { get; set; }

        public Guid MessageId { get; set; }

        public int VideoId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Video Video { get; set; }

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public MessageVideo ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.MessageId = (Guid)dataReader["MessageId"];
            this.VideoId = Convert.ToInt32(dataReader["VideoId"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
