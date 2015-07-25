using System;

namespace InstaMelody.Model
{
    public class StationMessageUserLike
    {
        public int Id { get; set; }

        public int StationMessageId { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region RelationshipProperties

        public User User { get; set; }

        #endregion RelationshipProperties
    }
}
