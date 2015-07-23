using System;
using InstaMelody.Model.Enums;

namespace InstaMelody.Model
{
    public class FileUploadToken
    {
        public Guid Token { get; set; }

        public Guid UserId { get; set; }

        public string FileName { get; set; }

        public FileUploadTypeEnum MediaType { get; set; }

        public DateTime DateExpires { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsExpired
        {
            get { return DateExpires < DateTime.UtcNow; }
        }
    }
}
