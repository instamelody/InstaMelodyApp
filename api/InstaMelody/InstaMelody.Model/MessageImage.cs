using System;

namespace InstaMelody.Model
{
    public class MessageImage
    {
        public int Id { get; set; }

        public Guid MessageId { get; set; }

        public int ImageId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Image Image { get; set; }

        #endregion Relationship Properties
    }
}
