using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace LSMES_5ANEW_PLUS.App_Base
{
    public class SystemInfo
    {
        public static int WeekOfYear()
        {
            GregorianCalendar gc = new GregorianCalendar();
            DateTime datetime = DateTime.Now;
            return gc.GetWeekOfYear(datetime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
        public static string WeekOfYeayApple()
        {
            GregorianCalendar gc = new GregorianCalendar();
            DateTime datetime = DateTime.Now;
            return WeekOfYear() < 10 ? datetime.Year.ToString().Substring(datetime.Year.ToString().Length - 1, 1) + "0" + WeekOfYear().ToString() : datetime.Year.ToString().Substring(datetime.Year.ToString().Length - 1, 1) + WeekOfYear().ToString();
        }
    }
}