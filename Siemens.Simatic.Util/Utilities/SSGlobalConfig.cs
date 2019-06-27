using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Util.Utilities.DAO;

namespace Siemens.Simatic.Util.Utilities
{
    public class SSGlobalConfig
    {
        public static DateTime SSMinDate = DateTime.MinValue;

        public static DateTime Now
        {
            get
            {
                DateTime? now = UtilDAO.GetDatabaseDatetime();
                if (now == null)
                    return DateTime.Now;

                return now.Value;
            }
        }

        public static DateTime UtcNow
        {
            get
            {
                DateTime? now = UtilDAO.GetDatabaseUtcDatetime();
                if (now == null)
                    return DateTime.Now;

                return now.Value;
            }
        }

        public static string DefaultPlantID
        {
            get { return "Aux"; }
        }

        public static string DefaultUserID
        {
            get { return "System"; }
        }
    }
}
