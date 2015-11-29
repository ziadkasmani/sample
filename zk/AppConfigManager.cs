using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeyVoteClassLibrary.Helper
{
    public static class AppConfigManager
    {
        public static string ConnectionString { get { return Properties.Settings.Default.ConnectionString; } }

        public static string HeyFolderPath { get { return Properties.Settings.Default.HeyFolderPath; } }

        public static string CookieKey { get { return Properties.Settings.Default.CookieKey; } }

        public static string DestinationTableName { get { return Properties.Settings.Default.DestinationTableName; } }

        public static string ContactDestTableName { get { return Properties.Settings.Default.ContactDestTableName; } }

        public static int WebPageSize { get { return Convert.ToInt32(Properties.Settings.Default.WebPageSize); } }

        public static int MobilePageSize { get { return Convert.ToInt32(Properties.Settings.Default.MobilePageSize); } }

        public static int KeepAliveTime { get { return Convert.ToInt32(Properties.Settings.Default.KeepAliveTime); } }

    }
}


