using System;

namespace InstaMelody.Model
{
    public class StationLike
    {
        public int Id { get; set; }

        public int StationId { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }
    }
}
