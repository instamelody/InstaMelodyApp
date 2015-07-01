using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class UserMessage
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RecipientId { get; set; }

        public Guid MessageId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeletedBySender { get; set; }

        public bool IsDeletedByRecipient { get; set; }

        public bool IsDeleted { get; set; }

        #region RelationshipProperties

        public Message Message { get; set; }

        public IList<UserMessage> ReplyMessages { get; set; } 

        #endregion RelationshipProperties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public UserMessage ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.UserId = (Guid)dataReader["UserId"];
            this.RecipientId = (Guid)dataReader["RecipientId"];
            this.MessageId = (Guid)dataReader["MessageId"];
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeletedBySender = Convert.ToBoolean(dataReader["IsDeletedBySender"]);
            this.IsDeletedByRecipient = Convert.ToBoolean(dataReader["IsDeletedByRecipient"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public UserMessage Clone()
        {
            var result = (UserMessage)this.MemberwiseClone();
            return result;
        }
    }
}
