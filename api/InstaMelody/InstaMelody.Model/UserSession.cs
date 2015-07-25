using System;

namespace InstaMelody.Model
{
    public class UserSession
    {
        public Guid Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }
    }
}
