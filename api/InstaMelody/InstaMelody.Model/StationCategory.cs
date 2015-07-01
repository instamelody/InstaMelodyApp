using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class StationCategory
    {
        public int Id { get; set; }

        public int StationId { get; set; }

        public int CategoryId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Category Category { get; set; }

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public StationCategory ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.StationId = Convert.ToInt32(dataReader["StationId"]);
            this.CategoryId = Convert.ToInt32(dataReader["CategoryId"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
