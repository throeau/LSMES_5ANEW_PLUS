using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.Business;

namespace LSMES_5ANEW_PLUS.WebService.inputMAInfo
{
    /// <summary>
    /// storingPolepiece 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class storingPolepiece : System.Web.Services.WebService
    {

        [WebMethod]
        public string Polepiece(string STOREHOUSENAME, string MAFLAG, string MATYPE, string MABOMNO, string MATECHARGENM, string SUPPLIERNM, string INPUTCOUNT, string INPUT_USERID, string INPUT_NAME, string INPUT_TIME, string GLAIRNO, string HOTDATE, string EQUIPMENTNO, string TOOLNO, string TOOLNAME, string UNIT, string REMARKS, string STATE, string CREATER, string CREATTIME, string UPDATER, string UPDATETIME)
        {
            return StorehouseManagement.intoMAStore("3", STOREHOUSENAME, MAFLAG, MATYPE, MABOMNO, MATECHARGENM, SUPPLIERNM, INPUTCOUNT, INPUT_USERID, INPUT_NAME, INPUT_TIME, GLAIRNO, HOTDATE, null, null, null, null, null, "0", CREATER, CREATTIME, UPDATER, UPDATETIME);
        }

    }
}
