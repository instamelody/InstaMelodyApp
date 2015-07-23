using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class StationFollower
    {
        public int Id { get; set; }

        public int StationId { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public User User { get; set; }

        #endregion Relationship Properties
    }
}
