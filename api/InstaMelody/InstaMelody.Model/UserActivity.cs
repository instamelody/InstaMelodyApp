using System;
using InstaMelody.Model.Enums;

namespace InstaMelody.Model
{
    public class UserActivity
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string UserDisplayName { get; set; }

        public string EntityName { get; set; }

        public ActivityTypeEnum ActivityType { get; set; }

        public string ActivityTypeString
        {
            get
            {
                return this.ActivityType.ToString();
            }
        }

        public DateTime DateOfActivity { get; set; }

        #region Relationship Properties

        public string Activity { get; set; }

        #endregion Relationship Properties
    }
}
