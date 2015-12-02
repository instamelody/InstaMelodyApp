using System;

namespace InstaMelody.Model
{
    public class MessageLoop
    {
        public int Id { get; set; }

        public Guid MessageId { get; set; }

        public Guid UserLoopId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public UserMelody UserMelody { get; set; }

        #endregion Relationship Properties
    }
}
