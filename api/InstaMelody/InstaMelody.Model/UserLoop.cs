using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstaMelody.Model
{
    public class UserLoop
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        public string Name { get; set; }

        public Guid UserId { get; set; }

        public bool IsExplicit { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public IList<UserLoopPart> Parts { get; set; }

        #endregion Relationship Properties
    }
}
