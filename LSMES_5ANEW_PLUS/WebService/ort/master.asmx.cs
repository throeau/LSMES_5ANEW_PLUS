using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using LSMES_5ANEW_PLUS.App_Base;
using LSMES_5ANEW_PLUS.Business;

namespace LSMES_5ANEW_PLUS.WebService.ort
{
    /// <summary>
    /// master 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class master : System.Web.Services.WebService
    {
        [WebMethod]
        public void GetWorkArea()
        {
            List<WorkArea> Entitys = new List<WorkArea>();
            Entitys = ORT.GetWorkArea();
            if (Entitys != null)
            {
                string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(Entitys));
                Context.Response.Charset = "UTF-8";
                Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Context.Response.Write(result);
                Context.Response.End();
            }
        }
        [WebMethod]
        public void GetOperations()
        {
            List<Operations> Entitys = new List<Operations>();
            Entitys = ORT.GetOperations();
            if (Entitys != null)
            {
                string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(Entitys));
                Context.Response.Charset = "UTF-8";
                Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Context.Response.Write(result);
                Context.Response.End();
            }
        }
        [WebMethod]
        public void GetTypeArea()
        {
            List<TypeArea> Entitys = new List<TypeArea>();
            Entitys = ORT.GetTypeArea();
            if (Entitys != null)
            {
                string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(Entitys));
                Context.Response.Charset = "UTF-8";
                Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Context.Response.Write(result);
                Context.Response.End();
            }
        }
        [WebMethod]
        public void GetBom()
        {
            List<Bom> Entitys = new List<Bom>();
            Entitys = ORT.GetBom();
            if (Entitys != null)
            {
                string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(Entitys));
                Context.Response.Charset = "UTF-8";
                Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Context.Response.Write(result);
                Context.Response.End();
            }
        }
        [WebMethod]
        public void GetTypeTask()
        {
            List<TypeTask> Entitys = new List<TypeTask>();
            Entitys = ORT.GetTypeTask();
            if (Entitys != null)
            {
                string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(Entitys));
                Context.Response.Charset = "UTF-8";
                Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Context.Response.Write(result);
                Context.Response.End();
            }
        }
        [WebMethod]
        public void GetORTTestItem(string handle)
        {
            List<ORTTestItem> Entitys = new List<ORTTestItem>();
            handle = Base64Helper.Base64Decode(handle);
            Entitys = ORT.GetTestItem(handle);
            if (Entitys != null)
            {
                string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(Entitys));
                Context.Response.Charset = "UTF-8";
                Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Context.Response.Write(result);
                Context.Response.End();
            }
        }
        [WebMethod]
        public void GetMasterOrt()
        {
            MasterOrt Entity = new MasterOrt();
            Entity = ORT.GetMasterOrt();
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(Entity));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
    }
}
