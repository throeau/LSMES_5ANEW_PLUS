using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.App_Base;

namespace LSMES_5ANEW_PLUS.WebService.email
{
    /// <summary>
    /// email 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class email : System.Web.Services.WebService
    {

        [WebMethod]
        public string test(string email, string title, string body)
        {
            try
            {
                Mail.SendMail(email, System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], title, body);
                return "success";
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return "fail";
            }
        }
    }
}
