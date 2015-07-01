using System;

namespace InstaMelody.Model.ApiModels
{
    public class ApiRequest
    {
        public Guid Token { get; set; }

        #region Relationship Properties

        public Category Category { get; set; }

        public Melody Melody { get; set; }

        public MelodyCategory MelodyCategory { get; set; }

        public MelodyLoop MelodyLoop { get; set; }

        public MelodyLoopPart MelodyLoopPart { get; set; }

        public Message Message { get; set; }

        public MessageImage MessageImage { get; set; }

        public MessageMelody MessageMelody { get; set; }

        public Station Station { get; set; }

        public StationCategory StationCategory { get; set; }

        public StationFollower StationFollower { get; set; }

        public StationMessage StationMessage { get; set; }

        public User User { get; set; }

        public UserFriend UserFriend { get; set; }

        public UserMessage UserMessage { get; set; }

        public UserPassword UserPassword { get; set; }

        public Image Image { get; set; }

        #endregion Relationship Properties
    }
}
