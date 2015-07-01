using System;
using System.Configuration;

namespace InstaMelody.Data
{
    public class DataAccess
    {
        public string ConnString
        {
            get
            {
                var connSettings = Properties.Settings.Default;
                if (connSettings == null) throw new Exception();


                // TODO: uncomment connection string before deployment
                //return connSettings.ConnectionString;
                return connSettings.DevConnectionString;
            }
        }
    }
}
