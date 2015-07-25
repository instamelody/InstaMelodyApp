using System;

namespace InstaMelody.Model
{
    public class MelodyFileGroup
    {
        public int Id { get; set; }

        public int MelodyId { get; set; }

        public int FileGroupId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }
    }
}
