using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class MelodyFileGroup
    {
        public int Id { get; set; }

        public int MelodyId { get; set; }

        public int FileGroupId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public MelodyFileGroup ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.MelodyId = Convert.ToInt32(dataReader["MelodyId"]);
            this.FileGroupId = Convert.ToInt32(dataReader["FileGroupId"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
