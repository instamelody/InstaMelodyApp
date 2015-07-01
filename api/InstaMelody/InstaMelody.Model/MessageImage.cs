using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class MessageImage
    {
        public int Id { get; set; }

        public Guid MessageId { get; set; }

        public int ImageId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Image Image { get; set; }

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public MessageImage ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.MessageId = (Guid) dataReader["MessageId"];
            this.ImageId = Convert.ToInt32(dataReader["ImageId"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
