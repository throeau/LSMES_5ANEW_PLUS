using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace LSMES_5ANEW_PLUS.App_Base
{
    public class AppException:ApplicationException
    {
        public object obj { get; set; }
        public Exception Exceptions { get; set; }
        public bool IsException { get; set; }
        public AppException() { }
        /// <summary>
        /// 发生异常时，需要传递 Exception 对象
        /// </summary>
        /// <param name="_obj"></param>
        /// <param name="ex"></param>
        public AppException(object _obj,Exception ex) 
        {
            try
            {
                IsException = true;
                obj = _obj;
                Exceptions = ex;
                SysLog log = new SysLog("ExpMessage：" + obj.ToString() + "；// Exception：" + Exceptions.Message);
            }
            catch (Exception x)
            {

            }
        }
        /// <summary>
        /// 无异常时，传递任意对象
        /// </summary>
        /// <param name="_obj"></param>
        public AppException(object _obj)
        {
            IsException = false;
            obj = _obj;
            Exceptions = null;
        }
    }
}