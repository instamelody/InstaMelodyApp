using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstaMelody.Model
{
    public class UserLoop
    {
        private Guid _chatId;

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        public string Name { get; set; }

        public Guid UserId { get; set; }

        public bool IsExplicit { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public bool IsChatLoop { get; set; }

        public Guid ChatId
        {
            get { return _chatId; }
            set
            {
                _chatId = value;
                IsChatLoop = (_chatId != null && !_chatId.Equals(default(Guid)));
            }
        }

        public IList<UserLoopPart> Parts { get; set; }

        #endregion Relationship Properties
    }
}
