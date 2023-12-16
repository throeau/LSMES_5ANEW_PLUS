using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.Business;
using Newtonsoft.Json;

namespace LSMES_5ANEW_PLUS.WebService.LoadInfoSAP
{
    /// <summary>
    /// LoadSapInformation 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class LoadSapInformation : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetWorkCenter()
        {
            SAP_Information sap = new SAP_Information();
            return JsonConvert.SerializeObject(sap.GetWorkCenter());
        }
        [WebMethod]
        public string GetResource()
        {
            SAP_Information sap = new SAP_Information();
            return JsonConvert.SerializeObject(sap.GetResource());
        }
        [WebMethod]
        public string GetWipByWj1(string item,string dt)
        {
            if (string.IsNullOrEmpty(dt))
            {
                dt = DateTime.Now.ToString("yyyy-MM-dd");
            }
            return JsonConvert.SerializeObject(SAP_Information.GetWipBwySapMeWj1(item, dt));
        }
        [WebMethod]
        public bool InitEquipment()
        {
            return SAP_Information.InitEquipment(SAP_Information.GetResourceBySapMeWj1());
        }
        [WebMethod]
        public bool InitEquipmentCapacity(string oper)
        {
            return SAP_Information.InitEquipmentCapacity(SAP_Information.GetOperationCapacity(), oper);
        }
        [WebMethod]
        public bool InitWipByWj1(string item_no,string dt,string oper)
        {
            if (string.IsNullOrEmpty(dt))
            {
                dt = DateTime.Now.ToString("yyyy-MM-dd");
            }
            return SAP_Information.InitWIP(item_no,SAP_Information.GetWipBwySapMeWj1(item_no, dt), oper);
        }
        [WebMethod]
        public string GetProductPlanBase(string item_no)
        {
            return ProductPlan.GetCapacity(item_no);
        }
        [WebMethod]
        public string GetWIP(string item_no)
        {
            return ProductPlan.GetWIP(item_no);
        }
        [WebMethod]
        public string GetPowerPlan()
        {
            return ProductPlan.GetPowerPlan();
        }
        [WebMethod]
        public string GetEquipmentWip(string item_no,string dt)
        {
            return JsonConvert.SerializeObject(SAP_Information.GetEquipmentWipBwySapMeWj1(item_no, dt));
        }
        [WebMethod]
        public void GetItem()
        {
            string jsonStr = JsonConvert.SerializeObject(SAP_Information.GetItemBySapMeWj1());
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(jsonStr);
            Context.Response.End();
        }
    }
}
