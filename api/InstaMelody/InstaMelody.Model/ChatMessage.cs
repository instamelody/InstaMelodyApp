using System;

namespace InstaMelody.Model
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public Guid ChatId { get; set; }

        public Guid MessageId { get; set; }

        public Guid SenderId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Message Message { get; set; }

        #endregion Relationship Properties
    }
}
