using System;

namespace InstaMelody.Model
{
    public class MelodyCategory
    {
        public int Id { get; set; }

        public int MelodyId { get; set; }

        public int CategoryId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }
    }
}
