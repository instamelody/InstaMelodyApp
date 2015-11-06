using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstaMelody.Model
{
    public class UserMelody
    {
        private List<Guid> _postId;

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        public string Name { get; set; }

        public Guid UserId { get; set; }

        public bool IsExplicit { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public bool IsStationPostMelody { get; set; }

        public List<Guid> StationPostIds
        {
            get { return _postId; }
            set
            {
                _postId = value;
                IsStationPostMelody = (_postId != null && _postId.Count > 0);
            }
        }

        public IList<Melody> Parts { get; set; }

        #endregion Relationship Properties
    }
}
