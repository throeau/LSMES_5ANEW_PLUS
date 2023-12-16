using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Data.Odbc;

namespace LSMES_5ANEW_PLUS.Business
{
    public class Customer
    {
        /// <summary>
        /// 获取 SAP 系统中德赛相关信息
        /// </summary>
        /// <param name="code">箱号</param>
        /// <returns></returns>
        public static CustomerDS GetCustomerDSBySAP(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return null;
                }
                DataTable dt = new DataTable();
                dt = SAP_Information.CustomDataMaintenanceBySAPBoxID(code);
                if (dt.Rows.Count < 1) return null;
                CustomerDS Customer = new CustomerDS();
                for (int i = 0; i < dt.Rows.Count; ++i)
                {
                    switch (dt.Rows[i]["ATTRIBUTE"].ToString())
                    {
                        case "CUSTOMER_ITEM_DESCRIPTION":
                            Customer.MODEL = dt.Rows[i]["VALUE"].ToString();
                            Customer.MODEL2 = dt.Rows[i]["VALUE"].ToString();
                            break;
                        case "CUSTOMER_ITEM":
                            Customer.PN= dt.Rows[i]["VALUE"].ToString();
                            Customer.PN2 = dt.Rows[i]["VALUE"].ToString();
                            break;
                        case "CORE_TYPE_CODE":
                            Customer.BOMNO = dt.Rows[i]["VALUE"].ToString();
                            break;
                        case "PO_CUSTOMER":
                            Customer.PO= dt.Rows[i]["VALUE"].ToString();
                            break;
                    }
                }
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Customer::GetCustomerDSBySAP => Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT TOP 1 TO_CHAR(FLOOR(ZCP.QTY)) C,RIGHT(LEFT(ZCPS.SN,7),4) LOT,TO_CHAR(ZCP.CREATED_DATE_TIME,'YYYY-MM-DD') SHIP_DATE FROM Z_CELL_PACK zcp INNER JOIN Z_CELL_PACK_SN ZCPS ON ZCP.HANDLE = ZCPS.CELL_PACK_BO WHERE ZCP.NUM = '{0}' ORDER BY LOT ASC;", code), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        Customer.BATTERY_QTY = reader["C"].ToString();
                        Customer.LOT = reader["LOT"].ToString();
                        Customer.SHIPDATE = reader["SHIP_DATE"].ToString();
                    }
                    reader.Close();
                    conn.Close();
                }
                return Customer;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        public static CustomerDS GetCustomerDSByShortBom(string code)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
            {
                mConn.Open();
                SqlDataReader SDR_Pipeline;
                SqlDataReader SDR_Customer;
                CustomerDS Customer = new CustomerDS();
                string sql = string.Format("SELECT OTHER_NAME,P.BOMNO FROM M_BOMNO M INNER JOIN T_BOM_PIPELINE P ON M.BOMNO = P.BOMNO INNER JOIN M_DEPARTMENT D ON P.PIPELINENAME = D.DEPARTMENTNAME WHERE SHORTBOM = '{0}';", code.Substring(0, 3));
                SqlCommand mComm = new SqlCommand(sql.ToString(), mConn);
                SDR_Pipeline = mComm.ExecuteReader();
                Dictionary<string, string> dicPipeline = new Dictionary<string, string>();
                if (!SDR_Pipeline.HasRows)
                {
                    Customer = GetCustomerDSBySAP(code);
                    return Customer;
                }
                while (SDR_Pipeline.Read())
                {
                    dicPipeline.Add(SDR_Pipeline[0].ToString(), SDR_Pipeline[1].ToString());
                }
                SDR_Pipeline.Close();
                foreach (string pl in dicPipeline.Keys)
                {
                    mComm.CommandText = string.Format("SELECT C.PKINFO2 AS MODEL,PKINFO1 AS PN,C.BOMNO,ADDRESS AS PO,B.LOT,CONVERT(NVARCHAR(10), GETDATE(), 120) AS SHIP_DATE,C.TEL,C.REMARKS,D.C FROM M_CUSTOMER C INNER JOIN (SELECT TOP 1 BOMNO,CUSTOMER_NO,RIGHT(LEFT(BATTERYNO,7),4) LOT,COUNT(1) C FROM V_{0}_{1}_BOX WHERE BOXID = '{2}' GROUP BY BOMNO,CUSTOMER_NO,RIGHT(LEFT(BATTERYNO,7),4) ORDER BY RIGHT(LEFT(BATTERYNO,7),4) ASC) B ON C.BOMNO = '{1}' AND CUSTOMER_NAME = '德赛' AND B.CUSTOMER_NO = C.CUSTOMER_NO COLLATE CHINESE_PRC_CI_AS INNER JOIN (SELECT COUNT(1) C,BOMNO FROM V_{0}_{1}_BOX WHERE BOXID = '{2}' GROUP BY BOMNO) D ON B.BOMNO = D.BOMNO;", pl, dicPipeline[pl], code);
                    SDR_Customer = mComm.ExecuteReader();
                    if (!SDR_Customer.HasRows)
                    {
                        Customer = GetCustomerDSBySAP(code);
                        return Customer;
                    }
                    while (SDR_Customer.Read())
                    {
                        Customer.MODEL = SDR_Customer[0].ToString();
                        Customer.PN = SDR_Customer[1].ToString();
                        Customer.BOMNO = SDR_Customer[2].ToString();
                        Customer.PO = SDR_Customer[3].ToString();
                        Customer.LOT = string.IsNullOrEmpty(Customer.LOT) ? SDR_Customer[4].ToString() : Customer.LOT + "/" + SDR_Customer[4].ToString();
                        Customer.SHIPDATE = SDR_Customer[5].ToString();
                        Customer.PN2 = SDR_Customer[6].ToString();
                        Customer.MODEL2 = SDR_Customer[7].ToString();
                        Customer.BATTERY_QTY = SDR_Customer[8].ToString();
                    }
                    SDR_Customer.Close();
                }
                return Customer;
            }
        }
        //public static CustomerCommon GetCustomerXPByShortBom(string shortbom)
        //{
        //    using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
        //    {
        //        mConn.Open();
        //        string sql = string.Format("SELECT C.PKINFO2 AS MODEL,PKINFO1 AS PN,C.BOMNO,ORDER_NO AS PO,CONVERT(NVARCHAR(10),GETDATE(),120) AS SHIP_DATE,C.TEL,C.REMARKS FROM M_BOMNO B INNER JOIN M_CUSTOMER C ON B.BOMNO = C.BOMNO AND CUSTOMER_NAME = '新普' WHERE B.SHORTBOM = '{0}';", shortbom);
        //        SqlCommand mComm = new SqlCommand(sql.ToString(), mConn);
        //        SqlDataReader SDR_Customer = mComm.ExecuteReader();
        //        CustomerCommon Customer = new CustomerCommon();
        //        while (SDR_Customer.Read())
        //        {
        //            Customer.VENDOR = SDR_Customer[0].ToString();
        //            Customer.PN = SDR_Customer[1].ToString();
        //            Customer.BOMNO = SDR_Customer[2].ToString();
        //            Customer.SUPPLIER = "TJ";
        //            Customer.VERSION = "00";
        //            Customer.CREATEDATE = DateTime.Now.ToString("yyMMdd");
        //        }
        //        SDR_Customer.Close();
        //        return Customer;
        //    }
        //}
        public static CustomerCommon GetCustomerXPByShortBom(string code)
        {
            CustomerCommon Customer = new CustomerCommon();
            SqlDataReader SDR_Pipeline;
            SqlDataReader SDR_Customer;
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
            {
                mConn.Open();
                string sql = string.Format("SELECT OTHER_NAME,P.BOMNO FROM M_BOMNO M INNER JOIN T_BOM_PIPELINE P ON M.BOMNO = P.BOMNO INNER JOIN M_DEPARTMENT D ON P.PIPELINENAME = D.DEPARTMENTNAME WHERE SHORTBOM = '{0}';", code.Substring(0, 3));
                SqlCommand mComm = new SqlCommand(sql.ToString(), mConn);
                SDR_Pipeline = mComm.ExecuteReader();
                Dictionary<string, string> dicPipeline = new Dictionary<string, string>();
                while (SDR_Pipeline.Read())
                {
                    dicPipeline.Add(SDR_Pipeline[0].ToString(), SDR_Pipeline[1].ToString());
                }
                SDR_Pipeline.Close();
                foreach (string pl in dicPipeline.Keys)
                {
                    mComm.CommandText = string.Format("SELECT C.PKINFO2 AS MODEL,PKINFO1 AS PN,C.BOMNO,ORDER_NO AS PO,CONVERT(NVARCHAR(10), GETDATE(), 120) AS SHIP_DATE,C.TEL,C.REMARKS FROM M_CUSTOMER C INNER JOIN (SELECT CUSTOMER_NO FROM V_{0}_{1}_BOX WHERE BOXID = '{2}' GROUP BY CUSTOMER_NO) B ON C.BOMNO = '{1}' AND CUSTOMER_NAME = '新普' AND B.CUSTOMER_NO = C.CUSTOMER_NO COLLATE CHINESE_PRC_CI_AS;", pl, dicPipeline[pl], code);
                    SDR_Customer = mComm.ExecuteReader();
                    while (SDR_Customer.Read())
                    {
                        Customer.VENDOR = SDR_Customer[5].ToString();
                        Customer.PN = SDR_Customer[1].ToString();
                        Customer.BOMNO = SDR_Customer[2].ToString();
                        Customer.SUPPLIER = "TJ";
                        Customer.VERSION = "00";
                        Customer.CREATEDATE = DateTime.Now.ToString("yyMMdd");
                    }
                    SDR_Customer.Close();
                }
            }
            return Customer;
        }
    }
}