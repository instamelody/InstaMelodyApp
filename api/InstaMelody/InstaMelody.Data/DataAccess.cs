using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model.Parser;
using InstaMelody.Model.Enums;

namespace InstaMelody.Data
{
    public class DataAccess : DataParser
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        /// <exception cref="System.Exception"></exception>
        public static string ConnString
        {
            get
            {
                var connSettings = Properties.Settings.Default;
                if (connSettings == null) throw new Exception();

#if DEBUG
                return connSettings.DevConnectionString;
#else
                return connSettings.ConnectionString;
#endif
            }
        }

        /// <summary>
        /// Executes a non query.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="parameters">The parameters.</param>
        protected void ExecuteNonQuery(string queryString, SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = queryString;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes the no return sproc.
        /// </summary>
        /// <param name="sprocName">Name of the sproc.</param>
        /// <param name="parameters">The parameters.</param>
        protected void ExecuteNoReturnSproc(string sprocName, Dictionary<string, object> parameters)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sprocName;

                foreach (var parameter in parameters)
                {
                    cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a scalar query.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected object ExecuteScalar(string queryString, SqlParameter[] parameters)
        {
            object value;
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = queryString;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                value = cmd.ExecuteScalar();
            }
            return value;
        }

        /// <summary>
        /// Executes the and parse from reader.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected T GetRecord<T>(string queryString, SqlParameter[] parameters = null)
        {
            var result = default(T);
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = queryString;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.IsClosed || !reader.HasRows) 
                        return result;

                    while (reader.Read())
                    {
                        result = ParseFromDataReader<T>(reader);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the record set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryString">The query string.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected IList<T> GetRecordSet<T>(string queryString, SqlParameter[] parameters = null)
        {
            var results = new List<T>();
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = queryString;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.IsClosed || !reader.HasRows)
                        return null;

                    while (reader.Read())
                    {
                        var result = ParseFromDataReader<T>(reader);
                        results.Add(result);
                    }
                }
            }
            return results;
        }
    }
}
