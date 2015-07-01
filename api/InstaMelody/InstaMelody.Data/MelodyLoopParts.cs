using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class MelodyLoopParts : DataAccess
    {
        /// <summary>
        /// Createps the part.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException"></exception>
        public MelodyLoopPart CreatepPart(MelodyLoopPart part)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.MelodyLoopParts
                                        (MelodyLoopId, MelodyId, OrderIndex, StartTime, 
                                        StartEffect, StartEffectDuration, EndTime,
                                        EndEffect, EndEffectDuration, DateCreated)
                                    VALUES (@MelodyLoopId, @MelodyId, @OrderIndex, @StartTime, 
                                        @StartEffect, @StartEffectDuration, @EndTime,
                                        @EndEffect, @EndEffectDuration, @DateCreated)

                                    SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyLoopId",
                    Value = part.MelodyLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyId",
                    Value = part.MelodyId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "OrderIndex",
                    Value = part.OrderIndex,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "StartTime",
                    Value = part.StartTime.Ticks,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "StartEffect",
                    Value = part.StartEffect.ToString(),
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "StartEffectDuration",
                    Value = part.StartEffectDuration.Ticks,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "EndTime",
                    Value = part.EndTime.Ticks,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "EndEffect",
                    Value = part.EndEffect.ToString(),
                    SqlDbType = SqlDbType.VarChar,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "EndEffectDuration",
                    Value = part.EndEffectDuration.Ticks,
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = part.DateCreated > DateTime.MinValue
                        ? part.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                var obj = cmd.ExecuteScalar();
                if (!Convert.IsDBNull(obj))
                {
                    return this.GetPartById(Convert.ToInt32(obj));
                }
            }
            throw new DataException();
        }

        /// <summary>
        /// Gets the part by identifier.
        /// </summary>
        /// <param name="melodyLoopPartId">The melody loop part identifier.</param>
        /// <returns></returns>
        public MelodyLoopPart GetPartById(int melodyLoopPartId)
        {
            MelodyLoopPart result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.MelodyLoopParts
                                    WHERE Id = @Id AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Id",
                    Value = melodyLoopPartId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = new MelodyLoopPart();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the parts by melody loop identifier.
        /// </summary>
        /// <param name="melodyLoopId">The melody loop identifier.</param>
        /// <returns></returns>
        public IList<MelodyLoopPart> GetPartsByMelodyLoopId(Guid melodyLoopId)
        {
            List<MelodyLoopPart> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.MelodyLoopParts
                                    WHERE IsDeleted = 0 AND MelodyLoopId = @MelodyLoopId
                                    ORDER BY OrderIndex ASC";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "MelodyLoopId",
                    Value = melodyLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<MelodyLoopPart>();
                        while (reader.Read())
                        {
                            var result = new MelodyLoopPart();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Deletes the part.
        /// </summary>
        /// <param name="melodyLoopPartId">The melody loop part identifier.</param>
        public void DeletePart(int melodyLoopPartId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MelodyLoopParts
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND Id = @PartId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "PartId",
                    Value = melodyLoopPartId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
