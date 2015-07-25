using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstaMelody.Model
{
    public class UserMelody
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        public string Name { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public IList<Melody> Parts { get; set; }

        #endregion Relationship Properties
    }
}
