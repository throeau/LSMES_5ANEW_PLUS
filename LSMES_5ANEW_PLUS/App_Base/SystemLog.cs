using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Collections;
using System.Configuration;

namespace LSMES_5ANEW_PLUS
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
        public void AddLog(string context){
            using (StreamWriter LogStream = new StreamWriter("D:\\LSMES_WEB_LOG_5ANEW_PLUS\\"+ DateTime.Now.ToString("yyyyMMdd_HH")+".log", true))
            {
                LogStream.Write("-----------------------------\r\n");
                LogStream.Write(DateTime.Now);
                LogStream.Write("\r\n");
                LogStream.Write(context);
                LogStream.Write("\r\n");
                LogStream.Close();
            }
        }
    }
}