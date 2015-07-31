using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstaMelody.Model
{
    public class Category
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public List<Category> Children { get; set; } 

        #endregion Relationship Properties
    }
}
