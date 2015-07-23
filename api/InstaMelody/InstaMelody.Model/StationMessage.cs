using System;
using System.Collections.Generic;

namespace InstaMelody.Model
{
    public class StationMessage
    {
        public int Id { get; set; }

        public int StationId { get; set; }

        public Guid MessageId { get; set; }

        public bool IsPrivate { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region RelationshipProperties

        public Message Message { get; set; }

        public IList<StationMessageUserLike> Likes { get; set; } 

        #endregion RelationshipProperties
    }
}
