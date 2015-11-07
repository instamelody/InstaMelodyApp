using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model;
using System.Text;

namespace InstaMelody.Data
{
    public class UserActivity : DataAccess
    {
        /// <summary>
        /// Gets the activity.
        /// </summary>
        /// <returns></returns>
        public IList<Model.UserActivity> GetActivity()
        {
            return GetRecordSet<Model.UserActivity>("SELECT * FROM dbo.UserActivity ORDER BY DateOfActivity DESC");
        }

        /// <summary>
        /// Gets the activity.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<Model.UserActivity> GetActivity(Guid userId)
        {
            var query = @"SELECT * FROM dbo.UserActivity
                        WHERE UserId = @UserId
                        ORDER BY DateOfActivity DESC";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<Model.UserActivity>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the activity.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="afterDateTime">The after date time.</param>
        /// <returns></returns>
        public IList<Model.UserActivity> GetActivity(Guid userId, DateTime afterDateTime)
        {
            var query = @"SELECT * FROM dbo.UserActivity
                        WHERE UserId = @UserId AND DateOfActivity > @AfterDate
                        ORDER BY DateOfActivity DESC";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "UserId",
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter
                {
                    ParameterName = "AfterDate",
                    Value = afterDateTime,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            return GetRecordSet<Model.UserActivity>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the activity.
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <returns></returns>
        public IList<Model.UserActivity> GetActivity(List<Guid> userIds)
        {
            var parameters = new List<SqlParameter>();

            var inputVars = new StringBuilder();
            for (var i = 0; i < userIds.Count; i++)
            {
                var userId = userIds[i];
                var inputVar = string.Format("User{0}", i);
                inputVars.AppendFormat("@{0},", inputVar);

                parameters.Add(new SqlParameter
                {
                    ParameterName = inputVar,
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
            }


            var query = string.Format(@"SELECT * FROM dbo.UserActivity
                        WHERE UserId IN ({0})
                        ORDER BY DateOfActivity DESC", inputVars.ToString().TrimEnd(','));

            return GetRecordSet<Model.UserActivity>(query, parameters.ToArray());
        }

        /// <summary>
        /// Gets the activity.
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <param name="afterDateTime">The after date time.</param>
        /// <returns></returns>
        public IList<Model.UserActivity> GetActivity(List<Guid> userIds, DateTime afterDateTime)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "AfterDate",
                    Value = afterDateTime,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input
                }
            };

            var inputVars = new StringBuilder();
            for (var i = 0; i < userIds.Count; i++)
            {
                var userId = userIds[i];
                var inputVar = string.Format("User{0}", i);
                inputVars.AppendFormat("@{0},", inputVar);

                parameters.Add(new SqlParameter
                {
                    ParameterName = inputVar,
                    Value = userId,
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input
                });
            }


            var query = string.Format(@"SELECT * FROM dbo.UserActivity
                        WHERE UserId IN ({0}) AND DateOfActivity > @AfterDate
                        ORDER BY DateOfActivity DESC", inputVars.ToString().TrimEnd(','));

            return GetRecordSet<Model.UserActivity>(query, parameters.ToArray());
        }
    }
}
