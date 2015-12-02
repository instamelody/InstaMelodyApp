using System;
using InstaMelody.Model.Enums;

namespace InstaMelody.Model
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        public string Description { get; set; }

        public MediaTypeEnum MediaType { get; set; }

        public bool IsRead { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Image Image { get; set; }

        public UserMelody UserMelody { get; set; }

        public Video Video { get; set; }

        public UserLoop UserLoop { get; set; }

        #endregion Relationship Properties
    }
}
