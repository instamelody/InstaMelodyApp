using System;
using System.Collections.Generic;

namespace InstaMelody.Model
{
    public class StationMessage
    {
        public int Id { get; set; }

        public int StationId { get; set; }

        public Guid MessageId { get; set; }

        public Guid SenderId { get; set; }

        public int? ParentId { get; set; }

        public bool IsPrivate { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region RelationshipProperties

        public IList<StationMessage> Replies { get; set; } 

        public Message Message { get; set; }

        public IList<StationMessageUserLike> Likes { get; set; } 

        #endregion RelationshipProperties
    }
}
