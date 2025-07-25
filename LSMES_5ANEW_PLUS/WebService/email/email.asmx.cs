using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.App_Base;
using System.Data.SqlClient;
using System.Data;
using LSMES_5ANEW_PLUS.Business;

namespace LSMES_5ANEW_PLUS.WebService.email
{
    /// <summary>
    /// email 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class email : System.Web.Services.WebService
    {

        [WebMethod]
        public string test(string email, string title, string body)
        {
            try
            {
                Mail.SendMail(email, System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], title, body);
                return "success";
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return "fail";
            }
        }
        [WebMethod]
        public string testAliYun(string email, string title, string body)
        {
            try
            {
                Mail.SendMail(email, "", "", "", title, body);
                return "success";
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return "fail";
            }
        }

        [WebMethod]
        public void TaskNotice(string email, string taskno)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(taskno)) return;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::CreateSchedule => SyncRemote db can not be open.");
                    }
                    TaskManagement man = new TaskManagement();
                    Tasks task = man.GetTask(taskno);

                    TableWeb tWeb = new TableWeb();
                    tWeb.addThead("任务编号");
                    tWeb.addThead("物料编号");
                    tWeb.addThead("任务状态");
                    tWeb.addThead("回传数量");
                    tWeb.addThead("任务创建时间");
                    tWeb.addThead("备注");

                    tWeb.addContext(task.SN);
                    tWeb.addContext(task.ITEM_NO);
                    tWeb.addContext(task.STATE);
                    tWeb.addContext(task.QTY_TOTAL.ToString());
                    tWeb.addContext(task.CREATED_DATE_TIME);

                    string title = null;
                    if (task.STATE == "启动")
                    {
                        title = task.CUSTOMER + " 新任务通知 —— 数据回传";
                        tWeb.addContext("新任务已启动，请尽快确认任务内数据");
                    }
                    else if (task.STATE == "确认中")
                    {
                        title = task.CUSTOMER + " 任务提醒 —— 数据回传";
                        tWeb.addContext("请尽快完成任务确认，并开始回传数据");
                    }

                    Mail.SendMail(email, System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], title, tWeb.TableHtml());
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return;
                }
            }
        }
    }
}
