using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public Guid ChatId { get; set; }

        public Guid MessageId { get; set; }

        public Guid SenderId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Message Message { get; set; }

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public ChatMessage ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.ChatId = (Guid)dataReader["ChatId"];
            this.MessageId = (Guid)dataReader["MessageId"];
            this.SenderId = (Guid)dataReader["SenderId"];
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
