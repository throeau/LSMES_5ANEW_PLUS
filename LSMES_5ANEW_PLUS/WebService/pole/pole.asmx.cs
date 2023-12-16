using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using LSMES_5ANEW_PLUS.Business;
using System.Configuration;


namespace LSMES_5ANEW_PLUS.WebService.pole
{
    /// <summary>
    /// pole 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class pole : System.Web.Services.WebService
    {

        [WebMethod]
        public void load(string info,string type)
        {
            if (string.IsNullOrEmpty(info) || string.IsNullOrEmpty(type)) return;
            type = ConfigurationManager.AppSettings[type].Trim();
            ReslutPole result = new ReslutPole();
            try
            {
                List<RowPole> rows = JsonConvert.DeserializeObject<List<RowPole>>(info);
                int count = Pole.Load(rows, type);
                if (count == rows.Count)
                {
                    if (Pole.Save() == count)
                    {
                        string users = Pole.notificationUsers("POLE");
                        string title = Pole.notificationSubject(type);
                        Pole.Notify(users, title, count.ToString());
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                result.result = "fail";
                result.count = 0;
            }
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
    }
}
