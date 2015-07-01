using System;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class UserFriend
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RequestorId { get; set; }

        public bool IsPending { get; set; }

        public bool IsDenied { get; set; }

        public DateTime? DateApproved { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public UserFriend ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.UserId = (Guid)dataReader["UserId"];
            this.RequestorId = (Guid)dataReader["RequestorId"];
            this.IsPending = Convert.ToBoolean(dataReader["IsPending"]);
            this.IsDenied = Convert.ToBoolean(dataReader["IsDenied"]);
            this.DateApproved = dataReader["DateApproved"] is DBNull
                ? new DateTime()
                : Convert.ToDateTime(dataReader["DateApproved"]);
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.DateModified = Convert.ToDateTime(dataReader["DateModified"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
