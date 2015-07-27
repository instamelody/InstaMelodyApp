using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InstaMelody.Model.Enums;

namespace InstaMelody.Data
{
    public abstract class DataAccess
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        /// <exception cref="System.Exception"></exception>
        private static string ConnString
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

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private static T ParseFromDataReader<T>(IDataRecord reader)
        {
            var type = typeof(T);
            var instance = Activator.CreateInstance<T>();
            var iType = instance.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                try
                {
                    object value;
                    var name = property.Name;
                    var propType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;


                    if (propType == typeof(TimeSpan))
                    {
                        value = (reader[name] == DBNull.Value) ? null : (object)TimeSpan.FromTicks(Convert.ToInt64(reader[name]));
                    }
                    else if (propType == typeof(FileUploadTypeEnum))
                    {
                        value = Enum.Parse(typeof(FileUploadTypeEnum), Convert.ToString(reader[name]));
                    }
                    else if (propType == typeof(LoopEffectsEnum))
                    {
                        value = Enum.Parse(typeof(LoopEffectsEnum), Convert.ToString(reader[name]));
                    }
                    else if (propType == typeof(MediaTypeEnum))
                    {
                        value = Enum.Parse(typeof(MediaTypeEnum), Convert.ToString(reader[name]));
                    }
                    else
                    {
                        value = (reader[name] == DBNull.Value) ? null : Convert.ChangeType(reader[name], propType);
                    }

                    iType.GetProperty(name).SetValue(instance, value);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return instance;
        }
    }
}
