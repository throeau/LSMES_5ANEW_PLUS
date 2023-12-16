using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace LSMES_5ANEW_PLUS.Business
{
    public class ProductPlan
    {
        /// <summary>
        /// 获取产能基础数据
        /// </summary>
        /// <param name="item_no">物料编号</param>
        /// <returns></returns>
        public static string GetCapacity(string item_no)
        {
            string sql;
            if (string.IsNullOrEmpty(item_no))  //全部基础数据
            {
                sql = "SELECT O.OPERATION,O.DESCRIPTION,E.EQUIPMENT_NO,EC.ITEM_NO,ROUND((EC.T_CAPACITY * W.HOUR)/2,0) AS CAPACITY FROM SAP_ME_WJ1_EQUIPMENT_CAPACITY EC INNER JOIN SAP_ME_WJ1_EQUIPMENT E ON EC.HANDLE_EQUIPMENT = E.HANDLE AND EC.IS_CURRENT = 'Y' INNER JOIN SAP_ME_WJ1_OPERATION O ON E.HANDLE_OPERATION = O.HANDLE AND O.IS_VALID = 'Y' INNER JOIN SAP_ME_WJ1_WORKDATE W ON W.IS_CURRENT = 'Y' ORDER BY OPERATION,EQUIPMENT_NO ASC;";
            }
            else  //指定物料的基础数据
            {
                sql = string.Format("SELECT O.OPERATION,O.DESCRIPTION,E.EQUIPMENT_NO,EC.ITEM_NO,ROUND((EC.T_CAPACITY * W.HOUR)/2,0) AS CAPACITY FROM SAP_ME_WJ1_EQUIPMENT_CAPACITY EC INNER JOIN SAP_ME_WJ1_EQUIPMENT E ON EC.HANDLE_EQUIPMENT = E.HANDLE AND EC.IS_CURRENT = 'Y' INNER JOIN SAP_ME_WJ1_OPERATION O ON E.HANDLE_OPERATION = O.HANDLE AND O.IS_VALID = 'Y' INNER JOIN SAP_ME_WJ1_WORKDATE W ON W.IS_CURRENT = 'Y' WHERE ITEM_NO = '{0}' ORDER BY OPERATION,EQUIPMENT_NO ASC;", item_no);
            }
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PPI"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("PPI database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    DataTable dts = new DataTable();
                    dts.Load(reader);
                    return JsonConvert.SerializeObject(dts);
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static string GetWIP(string item_no)
        {
            try
            {
                if (string.IsNullOrEmpty(item_no))
                {
                    throw new Exception("Item_no is not specified.");
                }
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PPI"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("PPI database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand(String.Format("SELECT O.OPERATION,O.DESCRIPTION,W.ITEM_NO,W.WIP FROM SAP_ME_WJ1_WIP W INNER JOIN SAP_ME_WJ1_OPERATION O ON W.HANDLE_OPERATION = O.HANDLE AND IS_CURRENT = 'Y' AND ITEM_NO = '{0}';", item_no), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    DataTable dts = new DataTable();
                    dts.Load(reader);
                    return JsonConvert.SerializeObject(dts);
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 获取电力检修信息
        /// </summary>
        /// <returns></returns>
        public  static string GetPowerPlan()
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PPI"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("PPI database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT * FROM SAP_ME_WJ1_POWER_PLAN WHERE IS_CURRENT = 'Y';", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    DataTable dts = new DataTable();
                    dts.Load(reader);
                    return JsonConvert.SerializeObject(dts);
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
    }
}