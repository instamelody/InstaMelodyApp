using System;

namespace InstaMelody.Model
{
    public class UserMelodyPart
    {
        public int Id { get; set; }

        public Guid UserMelodyId { get; set; }

        public int MelodyId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }
    }
}
