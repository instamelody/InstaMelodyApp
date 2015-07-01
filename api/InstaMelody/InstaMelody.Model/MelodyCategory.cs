using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class MelodyCategory
    {
        public int Id { get; set; }

        public int MelodyId { get; set; }

        public int CategoryId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public MelodyCategory ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.MelodyId = Convert.ToInt32(dataReader["MelodyId"]);
            this.CategoryId = Convert.ToInt32(dataReader["CategoryId"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
