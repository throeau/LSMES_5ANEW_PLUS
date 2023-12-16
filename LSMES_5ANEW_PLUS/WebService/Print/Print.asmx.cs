using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.App_Base;
using LSMES_5ANEW_PLUS.Business;
using Newtonsoft.Json;

namespace LSMES_5ANEW_PLUS.WebService.Print
{
    /// <summary>
    /// Print 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Print : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetCustomer(string code,string type = "DS")
        {
            if (type == "DS")
            {
                CustomerDS customer;
                customer = Customer.GetCustomerDSByShortBom(code);
                if (customer==null) customer = new CustomerDS();
                Datum.GetCaseDataByCaseID(code, ref customer);
                return JsonConvert.SerializeObject(customer);
            }
            else
            {
                CustomerCommon customer = new CustomerCommon();
                //customer = Customer.GetCustomerXPByShortBom(code.Substring(0, 3));
                customer = Customer.GetCustomerXPByShortBom(code);
                return JsonConvert.SerializeObject(customer);
            }
        }
        [WebMethod]
        public void GetBoxInfoWT(string bomno,string customer,string boxid)
        {
            BoxWT entity = new BoxWT();
            entity = Datum.GetBoxInfo(bomno, customer, boxid);
            if (entity != null)
            {
                string result = JsonConvert.SerializeObject(entity);
                Context.Response.Charset = "UTF-8";
                Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Context.Response.Write(result);
                Context.Response.End();
            }
        }
    }
}
