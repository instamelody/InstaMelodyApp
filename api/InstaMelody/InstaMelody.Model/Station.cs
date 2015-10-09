using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstaMelody.Model
{
    public class Station
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int? StationImageId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        [MinLength(5, ErrorMessage = "Name must be at least 5 characters")]
        public string Name { get; set; }

        public bool IsPublished { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Image Image { get; set; }

        public IList<Category> Categories { get; set; }

        public IList<User> Followers { get; set; }

        public IList<StationMessage> Messages { get; set; }

        public int Likes { get; set; }

        #endregion Relationship Properties

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Station Clone()
        {
            var result = (Station)this.MemberwiseClone();
            return result;
        }
    }
}
