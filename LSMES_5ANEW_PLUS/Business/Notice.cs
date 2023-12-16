using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using LSMES_5ANEW_PLUS.App_Base;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LSMES_5ANEW_PLUS.Business
{
    public class Notice
    {
        public void SendEquipmentState(string equipmentno, string state, string created_date_time)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW))
            {
                try
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = "SELECT E.EMPLOYEENO,E.EMPLOYEENAME,E.EMAIL FROM M_MAIL M INNER JOIN M_EMPLOYEE E ON M.EMPLOYEENO=E.EMPLOYEENO;";
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    reader.Close();
                    conn.Close();
                    StringBuilder recipients = new StringBuilder();
                    foreach (DataRow row in dt.Rows)
                    {
                        recipients.Append(row["EMAIL"]);
                        recipients.Append(";");
                    }
                    //测试邮件发送
                    Mail.SendMail(recipients.ToString(), System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"],"设备状态变更", "蓝牙封装设备状态更新 [ " + equipmentno + " ] \n\r"+ created_date_time + "：设备状态更新为“" + state + "”");
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }

        }
    }
}