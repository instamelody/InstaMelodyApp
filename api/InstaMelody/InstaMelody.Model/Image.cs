using System;
using System.ComponentModel.DataAnnotations;

namespace InstaMelody.Model
{
    public class Image
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "FileName cannot exceed 255 characters")]
        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        public string FilePath { get; set; }
    }
}
