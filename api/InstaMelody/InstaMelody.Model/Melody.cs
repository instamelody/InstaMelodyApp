using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class Melody
    {
        public int Id { get; set; }

        public bool IsUserCreated { get; set; }

        public bool IsPremiumContent { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "FileName is required.")]
        [StringLength(255, ErrorMessage = "FileName cannot exceed 255 characters")]
        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        public string FilePath { get; set; }
    }
}
