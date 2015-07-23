using System;
using System.Collections.Generic;

namespace InstaMelody.Model
{
    public class Chat
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public IList<ChatMessage> Messages { get; set; }

        public IList<User> Users { get; set; }

        #endregion Relationship Properties
    }
}
