using System;

namespace InstaMelody.Model
{
    public class MessageVideo
    {
        public int Id { get; set; }

        public Guid MessageId { get; set; }

        public int VideoId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Video Video { get; set; }

        #endregion Relationship Properties
    }
}
