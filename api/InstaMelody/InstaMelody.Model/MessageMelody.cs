using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class MessageMelody
    {
        public int Id { get; set; }

        public Guid MessageId { get; set; }

        public int MelodyId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Melody Melody { get; set; } 

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public MessageMelody ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.MessageId = (Guid)dataReader["MessageId"];
            this.MelodyId = Convert.ToInt32(dataReader["MelodyId"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
