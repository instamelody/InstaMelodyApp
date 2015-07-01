using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;

namespace InstaMelody.Data
{
    public class MelodyLoops : DataAccess
    {
        /// <summary>
        /// Gets the melody loop by identifier.
        /// </summary>
        /// <param name="melodyLoopId">The melody loop identifier.</param>
        /// <returns></returns>
        public MelodyLoop GetMelodyLoopById(Guid melodyLoopId)
        {
            MelodyLoop result = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT TOP 1 * FROM dbo.MelodyLoops
                                    WHERE Id = @Id AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Id",
                    Value = melodyLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = new MelodyLoop();
                            result = result.ParseFromDataReader(reader);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the melody loops by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<MelodyLoop> GetMelodyLoopsByUserId(Guid userId)
        {
            List<MelodyLoop> results = null;

            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM dbo.MelodyLoops
                                    WHERE UserId = @UserId AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.IsClosed && reader.HasRows)
                    {
                        results = new List<MelodyLoop>();
                        while (reader.Read())
                        {
                            var result = new MelodyLoop();
                            result = result.ParseFromDataReader(reader);
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Creates the melody loop.
        /// </summary>
        /// <param name="loop">The loop.</param>
        /// <returns></returns>
        public MelodyLoop CreateMelodyLoop(MelodyLoop loop)
        {
            var loopId = Guid.NewGuid();
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO dbo.MelodyLoops
                                    (Id, UserId, DateCreated, DateModified)
                                    VALUES (@Id, @UserId, @DateCreated, @DateCreated)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Id",
                    Value = loopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = loop.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateCreated",
                    Value = loop.DateCreated > DateTime.MinValue
                        ? loop.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return this.GetMelodyLoopById(loopId);
        }

        /// <summary>
        /// Updates the melody loop.
        /// </summary>
        /// <param name="loop">The loop.</param>
        /// <returns></returns>
        public MelodyLoop UpdateMelodyLoop(MelodyLoop loop)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MelodyLoops
                                    SET UserId = @UserId, DateModified = @DateModified
                                    WHERE Id = @Id AND IsDeleted = 0";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Id",
                    Value = loop.Id,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = loop.UserId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DateModified",
                    Value = loop.DateModified > DateTime.MinValue
                        ? loop.DateCreated
                        : DateTime.UtcNow,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return this.GetMelodyLoopById(loop.Id);
        }

        /// <summary>
        /// Deletes the melody loop.
        /// </summary>
        /// <param name="melodyLoopId">The melody loop identifier.</param>
        public void DeleteMelodyLoop(Guid melodyLoopId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"UPDATE dbo.MelodyLoops
                                    SET IsDeleted = 1
                                    WHERE IsDeleted = 0 AND Id = @LoopId";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "LoopId",
                    Value = melodyLoopId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
