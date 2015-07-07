using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class UserMelodyPart
    {
        public int Id { get; set; }

        public Guid UserMelodyId { get; set; }

        public int MelodyId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public UserMelodyPart ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.UserMelodyId = (Guid)dataReader["UserMelodyId"];
            this.MelodyId = Convert.ToInt32(dataReader["MelodyId"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
