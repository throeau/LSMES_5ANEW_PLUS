using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.App_Base;

namespace LSMES_5ANEW_PLUS.WebService.Job
{
    /// <summary>
    /// Job 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Job : System.Web.Services.WebService
    {

        [WebMethod]
        public void Run()
        {
            Scheduler.Run();
        }
        [WebMethod]
        public void Stop()
        {
            Scheduler.Stop();
        }
        [WebMethod]
        public void Reset()
        {
            Scheduler.Stop();
            Scheduler.Run();
        }
        [WebMethod]
        public void IsAlive()
        {
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(Scheduler.IsAlive().ToString());
            Context.Response.End();
        }
    }
}
