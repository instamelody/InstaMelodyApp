using System;

namespace InstaMelody.Model
{
    public class UserFriend
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RequestorId { get; set; }

        public bool IsPending { get; set; }

        public bool IsDenied { get; set; }

        public DateTime? DateApproved { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }
    }
}
