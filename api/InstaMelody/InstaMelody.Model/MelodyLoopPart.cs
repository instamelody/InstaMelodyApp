using System;
using System.Data.SqlClient;
using InstaMelody.Model.Enums;

namespace InstaMelody.Model
{
    public class MelodyLoopPart
    {
        public int Id { get; set; }

        public Guid MelodyLoopId { get; set; }

        public int MelodyId { get; set; }

        public int OrderIndex { get; set; }
        
        public TimeSpan StartTime { get; set; }

        public LoopEffectsEnum StartEffect { get; set; }

        public TimeSpan StartEffectDuration { get; set; }

        public TimeSpan EndTime { get; set; }

        public LoopEffectsEnum EndEffect { get; set; }

        public TimeSpan EndEffectDuration { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        #region Relationship Properties

        public Melody Melody { get; set; }

        #endregion Relationship Properties

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        public MelodyLoopPart ParseFromDataReader(SqlDataReader dataReader)
        {
            this.Id = Convert.ToInt32(dataReader["Id"]);
            this.MelodyLoopId =(Guid)dataReader["MelodyLoopId"];
            this.MelodyId = Convert.ToInt32(dataReader["MelodyId"]);
            this.OrderIndex = Convert.ToInt32(dataReader["OrderIndex"]);
            this.StartTime = TimeSpan.FromTicks(Convert.ToInt64(dataReader["StartTime"]));
            this.StartEffect = (LoopEffectsEnum)Enum.Parse(typeof(LoopEffectsEnum), Convert.ToString(dataReader["StartEffect"]));
            this.StartEffectDuration = TimeSpan.FromTicks(Convert.ToInt64(dataReader["StartEffectDuration"]));
            this.EndTime = TimeSpan.FromTicks(Convert.ToInt64(dataReader["EndTime"]));
            this.EndEffect = (LoopEffectsEnum)Enum.Parse(typeof(LoopEffectsEnum), Convert.ToString(dataReader["EndEffect"]));
            this.EndEffectDuration = TimeSpan.FromTicks(Convert.ToInt64(dataReader["EndEffectDuration"]));
            this.DateCreated = Convert.ToDateTime(dataReader["DateCreated"]);
            this.IsDeleted = Convert.ToBoolean(dataReader["IsDeleted"]);

            return this;
        }
    }
}
