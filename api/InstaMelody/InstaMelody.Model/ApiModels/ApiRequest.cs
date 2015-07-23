using System;
using System.Collections.Generic;

namespace InstaMelody.Model.ApiModels
{
    public class ApiRequest
    {
        public Guid Token { get; set; }

        #region Relationship Properties

        public Category Category { get; set; }

        public IList<Category> Categories { get; set; } 

        public UserMelody UserMelody { get; set; }

        public Melody Melody { get; set; }

        public Chat Chat { get; set; }

        public Message Message { get; set; }

        public User User { get; set; }

        public IList<User> Users { get; set; }

        public UserLoop Loop { get; set; }

        public UserLoopPart LoopPart { get; set; }

        public UserPassword UserPassword { get; set; }

        public Image Image { get; set; }

        public Station Station { get; set; }

        #endregion Relationship Properties
    }
}
