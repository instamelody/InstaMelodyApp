using System;
using InstaMelody.Model.Enums;

namespace InstaMelody.Model
{
    public class UserLoopPart
    {
        public int Id { get; set; }

        public Guid UserLoopId { get; set; }

        public Guid UserMelodyId { get; set; }

        public int OrderIndex { get; set; }

        public TimeSpan? StartTime { get; set; }

        public LoopEffectsEnum StartEffect { get; set; }

        public TimeSpan? StartEffectDuration { get; set; }

        public TimeSpan? EndTime { get; set; }

        public LoopEffectsEnum EndEffect { get; set; }

        public TimeSpan? EndEffectDuration { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public UserMelody UserMelody { get; set; }

        #endregion Relationship Properties
    }
}
