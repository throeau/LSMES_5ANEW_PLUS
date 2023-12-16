using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Configuration;

namespace Listener_Pack
{
    public class SysLog
    {
        public SysLog(string context)
        {
            //using (StreamWriter LogStream = new StreamWriter("D:\\LSMES_WEB_LOG_5ANEW_PLUS\\Log.log", true))
            //{
            //    LogStream.Write("-----------------------------\r\n");
            //    LogStream.Write(DateTime.Now);
            //    LogStream.Write("\r\n");
            //    LogStream.Write(context);
            //    LogStream.Write("\r\n");
            //}
            AddLog(context);
        }
        public void AddLog(string context)
        {
            using (StreamWriter LogStream = new StreamWriter(System.Configuration.ConfigurationManager.AppSettings["LogPath"]+ DateTime.Now.ToString("yyyyMMdd_HH")+".log", true))
            {
                LogStream.Write("-----------------------------\r\n");
                LogStream.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                LogStream.Write("\r\n");
                LogStream.Write(context);
                LogStream.Write("\r\n");
                LogStream.Close();
            }
        }
    }
}
