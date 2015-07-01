using System;

namespace InstaMelody.Model.ApiModels
{
    /// <summary>
    /// The API time.
    /// </summary>
    public class ApiTime
    {
        /// <summary>
        /// Gets or sets the date and time.
        /// </summary>
        public DateTime DateAndTime { get; set; }

        /// <summary>
        /// Gets or sets the unix date and time.
        /// </summary>
        public long UnixDateAndTime { get; set; }
    }
}
