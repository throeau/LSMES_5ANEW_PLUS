using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.Business;

namespace LSMES_5ANEW_PLUS.WebService.Person
{
    /// <summary>
    /// Person 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Person : System.Web.Services.WebService
    {

        [WebMethod]
        public string Authentication(string uid,string pwd)
        {
            try
            {
                LSMES_5ANEW_PLUS.Business.Person person = new LSMES_5ANEW_PLUS.Business.Person();
                Authentication authentication = new Authentication(ref person);
                return authentication.Login(uid, pwd);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }

        }
    }
}
