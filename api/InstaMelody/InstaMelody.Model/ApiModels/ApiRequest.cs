using System;

namespace InstaMelody.Model.ApiModels
{
    public class ApiRequest
    {
        public Guid Token { get; set; }

        #region Relationship Properties

        public Category Category { get; set; }

        public UserMelody UserMelody { get; set; }

        public Melody Melody { get; set; }

        public Chat Chat { get; set; }

        public Message Message { get; set; }

        public User User { get; set; }

        public UserPassword UserPassword { get; set; }

        public Image Image { get; set; }

        #endregion Relationship Properties
    }
}
