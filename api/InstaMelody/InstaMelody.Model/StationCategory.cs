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
    }
}
