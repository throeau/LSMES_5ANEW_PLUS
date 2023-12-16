using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using LSMES_5ANEW_PLUS.App_Base;
using LSMES_5ANEW_PLUS.Business;

namespace LSMES_5ANEW_PLUS.WebService.winding
{
    /// <summary>
    /// winding 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class winding : System.Web.Services.WebService
    {

        [WebMethod]
        public void Hipot(string info)
        {
            EntityHipot entity = new EntityHipot();
            entity = JsonConvert.DeserializeObject<EntityHipot>(info);
            ResultAssemble result = new ResultAssemble();
            if (Assemble.HiPot(entity))
            {
                result.Result = "success";
            }
            else
            {
                result.Result = "fail";
            }
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
    }
}
