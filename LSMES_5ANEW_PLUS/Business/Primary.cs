using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace LSMES_5ANEW_PLUS.Business
{
    public class Primary
    {
        /// <summary>
        /// 获取 Amazon 主数据
        /// </summary>
        /// <param name="item_no">Amazon 产品在 SAP ME 中物料编号</param>
        /// <returns></returns>
        public static DataTable PrimaryDataAmazon(string item_no)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                DataTable mDt = new DataTable();
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::PerformanceAmazonByBox => SAP PACK Database can not be opened.");
                    }
                    string sql = null;
                    if (string.IsNullOrEmpty(item_no))
                    {
                        sql = "SELECT * FROM AMAZON_CONFIG;";
                    }
                    else
                    {
                        sql = string.Format("SELECT * FROM AMAZON_CONFIG WHERE ITEM_NO = '{0}';", item_no);
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
                    mDt.Load(comm.ExecuteReader());
                    return mDt;
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