using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace LSMES_5ANEW_PLUS.WebService.Updata
{
    /// <summary>
    /// ElectricalProperties 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class ElectricalProperties : System.Web.Services.WebService
    {

        [WebMethod]
        public int InsertE1Data(string bomno_sizeno, string equipment, string bomno, string batteryno, string pumpno, string beforeWeight, string beforeTime, string afterWeight, string afterTime, string diff, string state)
        {
            try
            {
                UpdateRemote mBussinesss = new UpdateRemote();
                return mBussinesss.InsertE1Data(bomno_sizeno, equipment, bomno, batteryno, pumpno, beforeWeight, beforeTime, afterWeight, afterTime, diff, state);
            }
            catch (Exception ex)
            {
                SysLog mLog = new SysLog(ex.Message);
                return 1;
            }
        }
    }
}
