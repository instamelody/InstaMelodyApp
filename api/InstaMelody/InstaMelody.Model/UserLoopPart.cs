using System;
using System.Data.SqlClient;
using InstaMelody.Model.Enums;

namespace InstaMelody.Model
{
    public class UserLoopPart
    {
        public int Id { get; set; }

        public Guid UserLoopId { get; set; }

        public Guid UserMelodyId { get; set; }

        public int OrderIndex { get; set; }

        public TimeSpan? StartTime { get; set; }

        public LoopEffectsEnum StartEffect { get; set; }

        public TimeSpan? StartEffectDuration { get; set; }

        public TimeSpan? EndTime { get; set; }

        public LoopEffectsEnum EndEffect { get; set; }

        public TimeSpan? EndEffectDuration { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public UserMelody Melody { get; set; }

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public UserLoopPart ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.UserLoopId = (Guid)dataReader["UserLoopId"];
            this.UserMelodyId = (Guid)dataReader["UserMelodyId"];
            this.OrderIndex = Convert.ToInt32(dataReader["OrderIndex"]);
            this.StartEffect = (LoopEffectsEnum)Enum.Parse(typeof(LoopEffectsEnum), Convert.ToString(dataReader["StartEffect"]));
            this.EndEffect = (LoopEffectsEnum)Enum.Parse(typeof(LoopEffectsEnum), Convert.ToString(dataReader["EndEffect"]));
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);
            if (dataReader["StartTime"] != DBNull.Value)
            {
                this.StartTime = TimeSpan.FromTicks(Convert.ToInt64(dataReader["StartTime"]));
            }
            if (dataReader["StartEffectDuration"] != DBNull.Value)
            {
                this.StartEffectDuration = TimeSpan.FromTicks(Convert.ToInt64(dataReader["StartEffectDuration"]));
            }
            if (dataReader["EndTime"] != DBNull.Value)
            {
                this.EndTime = TimeSpan.FromTicks(Convert.ToInt64(dataReader["EndTime"]));
            }
            if (dataReader["EndEffectDuration"] != DBNull.Value)
            {
                this.EndEffectDuration = TimeSpan.FromTicks(Convert.ToInt64(dataReader["EndEffectDuration"]));
            }

            return this;
        }
    }
}
