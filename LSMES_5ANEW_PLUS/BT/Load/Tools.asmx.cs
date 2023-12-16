using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Services;
using LSMES_5ANEW_PLUS.Business;

namespace LSMES_5ANEW_PLUS.BT.Load
{
    /// <summary>
    /// Tools 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Tools : System.Web.Services.WebService
    {
        [WebMethod]
        public XmlDocument toolsinfo(string equipmentno,string toolno,string postion)
        {
            try
            {
                ToolsInfo tool = new ToolsInfo();

                //return Serialize.SerializeDataTableXml(tool.LoadToolsInfo(equipmentno, toolno, postion));
                XmlDocument doc = new XmlDocument();
                doc.Load(tool.LoadToolsInfo(equipmentno, toolno, postion));
                return doc;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }


    }
}
