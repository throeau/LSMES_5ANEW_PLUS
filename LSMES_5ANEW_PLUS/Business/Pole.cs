using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Newtonsoft.Json;
using System.Data.Odbc;
using LSMES_5ANEW_PLUS.App_Base;
using System.Data;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace LSMES_5ANEW_PLUS.Business
{
    public class Pole
    {
        private static Hashtable hashColumns = new Hashtable();
        private static List<string> sqlList = new List<string>();
        /// <summary>
        /// 初始化待处理数据表字段对应关系
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int initColumns(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return 0;
            }
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Pole::initColumns => Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT COMMENTS,COLUMN_NAME FROM SYS.TABLE_COLUMNS WHERE TABLE_NAME = '{0}';", type), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception(string.Format("Pole::{0} => comments from table {0} were not queried.", type));
                    }
                    hashColumns.Clear();
                    sqlList.Clear();
                    while (reader.Read())
                    {
                        hashColumns.Add(reader["COMMENTS"].ToString(), reader["COLUMN_NAME"].ToString());
                    }
                    reader.Close();
                    return hashColumns.Count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }

            }
        }
        /// <summary>
        /// 导入极片数据
        /// </summary>
        /// <param name="rows"></param>
        /// <returns>成功数量</returns>
        public static int Load(List<RowPole> rows, string type)
        {
            if (rows == null && string.IsNullOrEmpty(type))
            {
                return 0;
            }
            try
            {
                if (initColumns(type) == 0)
                {
                    return 0;
                }
                for (int i = 0; i < rows.Count; ++i)
                {
                    StringBuilder key = new StringBuilder();
                    StringBuilder value = new StringBuilder();
                    for (int j = 0; j < rows[i].Columns.Count; ++j)
                    {
                        try
                        {
                            key.Append(hashColumns[rows[i].Columns[j].Key.Trim()].ToString());
                            key.Append(",");
                            value.Append("'");
                            value.Append(rows[i].Columns[j].Value.Trim());
                            value.Append("',");
                        }
                        catch (Exception ex)
                        {
                            SysLog log = new SysLog(ex.Message + "\n" + string.Format("rows:{0},column:{1},{2}", i.ToString(), j.ToString(), rows[i].Columns[j].Key.Trim()));
                            break;
                            //return 0;
                        }
                    }
                    //sql.Append(string.Format("INSERT INTO {2} (HANDLE,{0},CREATED_USER,CREATED_DATE_TIME) VALUES (STRTOBIN(SYSUUID,'UTF-8'),{1},'PoleClient',CURRENT_TIMESTAMP);", key.ToString().Substring(0, key.ToString().Length - 1), value.ToString().Substring(0, value.ToString().Length - 1), type));
                    sqlList.Add(string.Format("INSERT INTO {2} (HANDLE,{0},CREATED_USER,CREATED_DATE_TIME) VALUES (STRTOBIN(SYSUUID,'UTF-8'),{1},'PoleClient',CURRENT_TIMESTAMP);", key.ToString().Substring(0, key.ToString().Length - 1), value.ToString().Substring(0, value.ToString().Length - 1), type));
                }
                return sqlList.Count == rows.Count ? sqlList.Count : 0;
            }
            catch (Exception ex)
            {
                StackFrame sf = new StackFrame(true);
                int lineNumber = sf.GetFileLineNumber();
                int colNumber = sf.GetFileColumnNumber();
                string fileName = sf.GetFileName();
                string methodName = sf.GetMethod().Name;
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 通过事务的方式添加至数据库
        /// </summary>
        /// <returns></returns>
        public static int Save()
        {
            if (sqlList.Count == 0)
            {
                return 0;
            }
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
            {
                conn.Open();
                OdbcTransaction tran = conn.BeginTransaction();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Pole::Save => Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand();
                    comm.Connection = conn;
                    comm.Transaction = tran;
                    int rowsAffect = 0;
                    for (int i = 0; i < sqlList.Count; ++i)
                    {
                        comm.CommandText = sqlList[i];
                        rowsAffect += comm.ExecuteNonQuery();
                    }
                    tran.Commit();
                    return rowsAffect;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 获取当前数据表所对应的主题
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string notificationSubject(string type)
        {
            if (string.IsNullOrEmpty(type)) return null;
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Pole::notificationSubject => Database can not be opened.");
                    }
                    string result = null;
                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT COMMENTS FROM SYS.TABLES WHERE TABLE_NAME = '{0}';", type), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        result = reader["COMMENTS"].ToString();
                    }
                    reader.Close();
                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取通知人邮箱地址
        /// </summary>
        /// <param name="type">固定值：POLE</param>
        /// <returns></returns>
        public static string notificationUsers(string type)
        {
            if (string.IsNullOrEmpty(type)) return null;
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Pole::notificationUsers => Database can not be opened.");
                    }
                    string mailList = null;
                    SqlCommand comm = new SqlCommand(string.Format("SELECT EC.USERNAME ,EC.EMAIL FROM CUSTOMER C INNER JOIN EMAIL_GROUP EG ON C.HANDLE = EG.HANDLE_CUSTOMER INNER JOIN EMAIL_CONFIG EC ON EG.HANDLE_EMAIL = EC.HANDLE WHERE C.CUSTOMER = '{0}';", type), mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        mailList += reader["EMAIL"].ToString() + ";";
                    }
                    reader.Close();
                    return mailList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static void Notify(string users, string type, string qty)
        {
            try
            {
                TableWeb tWeb = new TableWeb();
                tWeb.addThead("数据类型");
                tWeb.addThead("上传数量");
                tWeb.addThead("状态");
                tWeb.addThead("上传时间");

                tWeb.addContext(type);
                tWeb.addContext(qty);
                tWeb.addContext("完成");
                tWeb.addContext(DateTime.Now.ToString("G"));

                Mail.SendMail(users, System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], "极片数据上传任务", tWeb.TableHtml());
            }
            catch(Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
    }
}