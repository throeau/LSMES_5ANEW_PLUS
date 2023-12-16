using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.Business;

namespace LSMES_5ANEW_PLUS.WebService.SyncRemote
{
    /// <summary>
    /// Recieve 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Recieve : System.Web.Services.WebService
    {

        //[WebMethod]
        //public string HelloWorld()
        //{
        //    return "Hello World";
        //}
        [WebMethod]
        public int Recieve_HW_Product(string json)
        {
            int count = Datum.Write_HW_Recieve_Log(json);
            return count;
        }

        [WebMethod]
        public int PrepareData()
        {
            int count = SAP_Information.PrepareData();
            return count;
        }
    }
}
