using InstaMelody.Model.Enums;
using System;
using System.Data;

namespace InstaMelody.Model.Parser
{
    public abstract class DataParser
    {

        /// <summary>
        /// Parses from data reader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected static T ParseFromDataReader<T>(IDataRecord reader)
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
                    else if (propType == typeof(ActivityTypeEnum))
                    {
                        value = Enum.Parse(typeof(ActivityTypeEnum), Convert.ToString(reader[name]));
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
