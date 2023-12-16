using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSMES_5ANEW_PLUS.App_Base
{
    public class TimeStamp
    {
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public static string GetTimeStampByString(string dt)
        {
            TimeSpan ts = (Convert.ToDateTime(dt) - new DateTime(1970, 1, 1, 0, 0, 0, 0));
            // 字符串补齐至 13 位
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
    }
}