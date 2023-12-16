using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace LSMES_5ANEW_PLUS.SRC.Common
{
    /// <summary>
    /// wcm 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class wcm : System.Web.Services.WebService
    {

        [WebMethod]
        public void authorization(string info)
        {
            SysLog log = new SysLog(info);
            string result = "{\"RESULT\": \"1\",\"MESSAGE\": \"接收成功\",}";
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
    }
}
