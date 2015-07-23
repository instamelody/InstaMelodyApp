using System;

namespace InstaMelody.Model
{
    public class ChatUser
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ChatId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }
    }
}
