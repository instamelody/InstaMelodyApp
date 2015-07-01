using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace InstaMelody.Model
{
    public class MelodyLoop
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public IList<MelodyLoopPart> Parts { get; set; } 

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public MelodyLoop ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = (Guid)dataReader["Id"];
            this.UserId = (Guid) dataReader["UserId"];
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.DateModified = Convert.ToDateTime(dataReader["DateModified"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
