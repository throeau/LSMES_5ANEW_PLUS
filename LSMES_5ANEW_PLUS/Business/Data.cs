using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using LSMES_5ANEW_PLUS.App_Base;
using LSMES_5ANEW_PLUS.SQLTools;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using System.Data.Odbc;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using Oracle.ManagedDataAccess.Client;

namespace LSMES_5ANEW_PLUS.Business
{
    public class Datum
    {
        public DataTable LoadCapacity(string bomno, string pipeline, string orderno)
        {
            try
            {
                SQLQueryBuilder mSQL = new SQLQueryBuilder(QueryType.SELECT);
                mSQL.Assemble("BATTERYNO", "BATTERYNO", null);
                mSQL.Assemble("E13OUT_CAPACITY2", "CAPACITY", null);
                mSQL.Condition("BOMNO", bomno.ToUpper(), true);
                mSQL.Condition("ORDERNO", orderno.ToUpper(), true);
                mSQL.Condition("E13OUT_CAPACITY2", "is not null", true);
                mSQL.DataTable = "V_" + pipeline.ToUpper() + "_" + bomno.ToUpper() + "_B7";
                mSQL.OrderBy("E13OUT_CAPACITY2", "ASC");
                mSQL.Distinct = true;
                SQLTools.SQLTools mSQLTool = new SQLTools.SQLTools(Configuer.ConnectionStringByLSMES_5ANEW);
                return mSQLTool.ExecuteQuery(mSQL.Build());
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + " Datum::LoadCapacity");
                return null;
            }
        }
        public void UpdateCapacity(string bomno, string pipeline, string orderno)
        {
            try
            {
                StringBuilder batteryList = new StringBuilder();
                StringBuilder batteryList2 = new StringBuilder();
                SQLTools.SQLTools mSQLTool = new SQLTools.SQLTools(Configuer.ConnectionStringByLSMES_5ANEW);
                DataTable dt = mSQLTool.ExecuteQuery("select batteryno from v_" + pipeline + "_" + bomno + "_b3 where orderno='" + orderno + "' and e13out_capacity2>338 order by e13out_capacity2 asc");
                int step = 0;
                string[] LBattery = new string[dt.Rows.Count / 2 + 1];
                string[] HBattery = new string[dt.Rows.Count / 2 + 1];
                foreach (DataRow dr in dt.Rows)
                {
                    if (step <= dt.Rows.Count / 2)
                    {
                        //LBattery[i] = dr[0].ToString();
                        //++i;
                        batteryList.Append("'");
                        batteryList.Append(dr[0]);
                        batteryList.Append("',");
                    }
                    else
                    {
                        //HBattery[j] = dr[0].ToString();
                        //++j;
                        batteryList2.Append("'");
                        batteryList2.Append(dr[0]);
                        batteryList2.Append("',");
                    }
                    ++step;
                }
                SysLog log = new SysLog("update v_" + pipeline + "_" + bomno + "_b3 set e13level='L' where batteryno in (" + batteryList.ToString().Substring(0, batteryList.ToString().Length - 1) + ")");
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + "Datum::UpdateCapacity");
            }
        }
        public DataTable LoadDeltaV(string lot)
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();
                sqlStr.Append("select t.pipeline,d.other_name,t.bomno,t.sizeno From t_real_flow t left join m_department d on t.pipeline=d.departmentname where t.bomno_sizeno='");
                sqlStr.Append(lot);
                sqlStr.Append("'");
                SQLTools.SQLTools mSQLTool = new SQLTools.SQLTools(Configuer.ConnectionStringByLSMES_5ANEW);
                DataTable dtt = mSQLTool.ExecuteQuery(sqlStr.ToString());
                sqlStr.Clear();
                sqlStr.Append("select BOMNO,SIZENO,BATTERYNO,CONVERT(FLOAT,E6DELTAV) AS E6DELTAV,CONVERT(FLOAT,E6KVALUE) AS E6KVALUE,LEVELNAME from v_");
                sqlStr.Append(dtt.Rows[0]["other_name"].ToString());
                sqlStr.Append("_");
                sqlStr.Append(dtt.Rows[0]["bomno"].ToString());
                sqlStr.Append("_b7 where sizeno='");
                sqlStr.Append(dtt.Rows[0]["sizeno"].ToString());
                sqlStr.Append("' AND E6KVALUE IS NOT NULL");
                return mSQLTool.ExecuteQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + "Datum::LoadDeltaV");
                return null;
            }
        }
        public void UpdateKValue(DataTable dt, string lot)
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();
                sqlStr.Append("select t.pipeline,d.other_name,t.bomno,t.sizeno From t_real_flow t left join m_department d on t.pipeline=d.departmentname where t.bomno_sizeno='");
                sqlStr.Append(lot);
                sqlStr.Append("'");
                SQLTools.SQLTools mSQLTool = new SQLTools.SQLTools(Configuer.ConnectionStringByLSMES_5ANEW);
                DataTable dtt = mSQLTool.ExecuteQuery(sqlStr.ToString());
                sqlStr.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    sqlStr.Append("update ");
                    sqlStr.Append("v_");
                    sqlStr.Append(dtt.Rows[0]["other_name"].ToString());
                    sqlStr.Append("_");
                    sqlStr.Append(dtt.Rows[0]["bomno"].ToString());
                    sqlStr.Append("b7");
                    sqlStr.Append(" set levelname='KValue_NG' where batteryno='");
                    sqlStr.Append(dr["batteryno"]);
                    sqlStr.Append("' ");
                }
                SysLog log = new SysLog(sqlStr.ToString());
                //mSQLTool.ExecuteQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + "Datum::UpdateKValue");
            }
        }
        public void UpdateV1(DataTable dt, string table)
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                    sqlStr.Append("update ");
                    sqlStr.Append(table);
                    sqlStr.Append(" set E5VOLTAGE1=");
                    sqlStr.Append(dr["E5VOLTAGE1"]);
                    sqlStr.Append(",E5LEVEL='");
                    sqlStr.Append(dr["E5LEVEL"]);
                    sqlStr.Append("',E5TESTTIME1='");
                    sqlStr.Append(dr["E5TESTTIME1"]);
                    sqlStr.Append("' where ");
                }
            }
            catch (Exception ex)
            {
            }
        }
        public DataTable LoadVoltage(string sqlStr)
        {
            try
            {
                SQLTools.SQLTools mSQLTool = new SQLTools.SQLTools(Configuer.ConnectionStringByLSMES_5ANEW);
                return mSQLTool.ExecuteQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + " Datum::LoadVoltage");
                return null;
            }
        }
        public DataTable LoadCustomer(string sqlStr)
        {
            try
            {
                SQLTools.SQLTools mSQLTool = new SQLTools.SQLTools(Configuer.ConnectionStringByLSMES_5ANEW);
                return mSQLTool.ExecuteQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + " Datum::LoadCustomer");
                return null;
            }
        }
        public DataTable LoadPackBomno()
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();
                sqlStr.Append("select packbomno from ak_bomno order by packbomno asc");
                SQLTools.SQLTools mSQLTool = new SQLTools.SQLTools(Configuer.ConnectionStringByLSMES_PACK);
                return mSQLTool.ExecuteQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + " Datum::LoadVoltage");
                return null;
            }
        }
        public DataTable LoadPackBarcode(string bomno, string boxid)
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();
                sqlStr.Append("select packcode,boxid,packbomno,creattime from vak_");
                sqlStr.Append(bomno);
                sqlStr.Append("_box where boxid='");
                sqlStr.Append(boxid);
                sqlStr.Append("'");
                SQLTools.SQLTools mSQLTool = new SQLTools.SQLTools(Configuer.ConnectionStringByLSMES_PACK);
                return mSQLTool.ExecuteQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + " Datum::LoadVoltage");
                return null;
            }
        }
        public void UpdateKValueOfOCV2(string bomno, string line, DataTable dt)
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();

            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(DateTime.Now.ToString() + "：UpdateKValueOfOCV2 = " + ex.Message);
            }
        }
        public DataTable LoadBC(string bomno, string boxid)
        {
            try
            {
                string sqlStr = "SELECT PACKBOMNO FROM AK_CUSTOMER WHERE ITEM='{0}'";
                SQLTools.SQLTools mSQLTool = new SQLTools.SQLTools(Configuer.ConnectionStringByLSMES_PACK);
                DataRow dr = mSQLTool.ExecuteQuery(string.Format(sqlStr, bomno)).Rows[0];
                sqlStr = "SELECT CELLCODE,PACKCODE,C.CREATTIME,BOXID,B.CREATTIME FROM VAK_{0}_BOX B INNER JOIN VAK_{0}_BC C ON B.PACKCODE=C.BATTERYNO WHERE B.BOXID= '{1}'";
                SQLTools.SQLTools mSQLTool2 = new SQLTools.SQLTools(Configuer.ConnectionStringByLSMES_PACK);
                return mSQLTool2.ExecuteQuery(string.Format(sqlStr, dr[0].ToString(), boxid));
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + " Datum::LoadBC");
                return null;
            }
        }
        public static int UpdateDSBoxID(string bomno, string Boxid_LSN, string Boxid_DS)
        {
            try
            {
                using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                {
                    mConn.Open();
                    string sql = string.Format("UPDATE V_WJ1_{0}_BOX SET SDBOXID = '{1}' WHERE BOXID = '{2}';", bomno, Boxid_DS, Boxid_LSN);
                    SqlCommand mComm = new SqlCommand(sql, mConn);
                    return mComm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + " Datum::UpdateDSBoxID");
                return 0;
            }
        }
        public static int UpdateDSCaseID(string bomno, string Casesid_LSN, string Casesid_DS)
        {
            try
            {
                using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                {
                    mConn.Open();
                    string sql = string.Format("UPDATE V_WJ1_{0}_CASE SET SDBOXID = '{1}' WHERE CASESID = '{2}';", bomno, Casesid_DS, Casesid_LSN);
                    SqlCommand mComm = new SqlCommand(sql, mConn);
                    return mComm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + " Datum::UpdateDSCaseID");
                return 0;
            }
        }
        public static void GetCaseDataByCaseID(string caseid, ref CustomerDS customer)
        {
            try
            {
                using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                {
                    mConn.Open();

                    string sql;
                    sql = string.Format("SELECT OTHER_NAME,P.BOMNO FROM M_BOMNO M INNER JOIN T_BOM_PIPELINE P ON M.BOMNO = P.BOMNO INNER JOIN M_DEPARTMENT D ON P.PIPELINENAME = D.DEPARTMENTNAME WHERE SHORTBOM = '{0}';", caseid.Substring(0, 3));
                    SqlCommand mComm = new SqlCommand(sql, mConn);
                    SqlDataReader SDR_CasesInfo = mComm.ExecuteReader();
                    if (!SDR_CasesInfo.HasRows)
                    {
                        Datum.GetCaseDataBySAPCaseID(caseid, ref customer);
                        return;
                    }
                    List<string> pipeline = new List<string>();
                    while (SDR_CasesInfo.Read())
                    {
                        pipeline.Add(SDR_CasesInfo[0].ToString());
                        customer.BOMNO = SDR_CasesInfo[1].ToString();
                    }
                    SDR_CasesInfo.Close();
                    foreach (string line in pipeline)
                    {
                        sql = string.Format("WITH A AS (SELECT COUNT(BOXID) AS BOX_QTY FROM V_{0}_{1}_CASE WHERE CASESID = '{2}'),B AS (SELECT COUNT(B.BATTERYNO) AS QTY,MIN(LEFT(B.CODEDATE, 3)) AS WEEK,CUSTOMER_NO FROM V_{0}_{1}_CASE C INNER JOIN V_{0}_{1}_BOX B ON C.BOXID = B.BOXID WHERE CASESID = '{2}' GROUP BY CUSTOMER_NO),CC AS (SELECT PKINFO2 AS MODEL,PKINFO1 AS PN,ADDRESS AS PO,CONVERT(NVARCHAR(10), GETDATE(), 120) AS SHIP_DATE,TEL,REMARKS FROM M_CUSTOMER INNER JOIN B ON M_CUSTOMER.CUSTOMER_NO COLLATE Chinese_PRC_CI_AS = B.CUSTOMER_NO WHERE CUSTOMER_NAME = '德赛' AND BOMNO = '{1}'),E AS (SELECT TOP 1 COUNT(1) C,RIGHT(LEFT(D.BATTERYNO,7),4) LOT FROM (SELECT B.BATTERYNO FROM  V_{0}_{1}_BOX B INNER JOIN  V_{0}_{1}_CASE C ON B.BOXID = C.BOXID AND C.CASESID = '{2}') D GROUP BY RIGHT(LEFT(D.BATTERYNO,7),4) ORDER BY RIGHT(LEFT(D.BATTERYNO,7),4) ASC) SELECT BOX_QTY,QTY,WEEK,CC.MODEL,CC.PN,CC.PO,CC.SHIP_DATE,CC.TEL,CC.REMARKS,E.LOT FROM A,B,CC,E;", line, customer.BOMNO, caseid);
                        mComm.CommandText = sql;
                        SDR_CasesInfo = mComm.ExecuteReader();
                        if (!SDR_CasesInfo.HasRows)
                        {
                            Datum.GetCaseDataBySAPCaseID(caseid, ref customer);
                            return;
                        }
                        while (SDR_CasesInfo.Read())
                        {
                            if (SDR_CasesInfo[0].ToString() != "0")
                            {
                                customer.BOX_QTY = SDR_CasesInfo[0].ToString();
                                customer.BATTERY_QTY = SDR_CasesInfo[1].ToString();
                                //customer.LOT = SDR_CasesInfo[2].ToString();
                                customer.MODEL = SDR_CasesInfo[3].ToString();
                                customer.PN = SDR_CasesInfo[4].ToString();
                                customer.PO = SDR_CasesInfo[5].ToString();
                                customer.SHIPDATE = SDR_CasesInfo[6].ToString();
                                customer.MODEL2 = SDR_CasesInfo[8].ToString();
                                customer.PN2 = SDR_CasesInfo[7].ToString();
                                customer.LOT = string.IsNullOrEmpty(SDR_CasesInfo[9].ToString()) ? customer.LOT : string.IsNullOrEmpty(customer.LOT) ? SDR_CasesInfo[9].ToString() : customer.LOT + "/" + SDR_CasesInfo[9].ToString();
                            }
                        }
                        SDR_CasesInfo.Close();
                    }
                    mConn.Close();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + " Datum::LoadBC");
            }
        }
        public static void GetCaseDataBySAPCaseID(string caseid, ref CustomerDS customer)
        {
            if (string.IsNullOrEmpty(caseid) || customer == null)
            {
                return;
            }
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetCaseDataBySAPCaseID => Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand(string.Format("WITH A AS (SELECT ZCP2.HANDLE,ZCP2.QTY QTY,TO_CHAR(ZCP.CREATED_DATE_TIME,'YYYY-MM-DD') CREATED_DATE_TIME FROM Z_CELL_PACK ZCP INNER JOIN Z_CELL_PACK ZCP2 ON ZCP.HANDLE = ZCP2.PARENT_BO WHERE ZCP.CATEGORY = 'TRAY' AND ZCP.NUM = '{0}' GROUP BY ZCP2.HANDLE,ZCP2.QTY,ZCP.CREATED_DATE_TIME),B AS (SELECT MIN(RIGHT(LEFT(ZCPS.SN, 7), 4)) LOT FROM Z_CELL_PACK_SN ZCPS INNER JOIN A ON ZCPS.CELL_PACK_BO = A.HANDLE)SELECT COUNT(A.HANDLE) BOX_QTY,TO_CHAR(FLOOR(SUM(A.QTY))) QTY,B.LOT,LEFT(B.LOT, 3) WEEK,A.CREATED_DATE_TIME SHIP_DATE FROM A,B GROUP BY QTY,B.LOT,A.CREATED_DATE_TIME;", caseid), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        GetBoxDataBySAPBoxID(caseid,ref customer);
                        return;
                    }
                    int temp_box_qty = 0;
                    int temp_qty = 0;
                    while (reader.Read())
                    {
                        temp_box_qty += Convert.ToInt32(reader["BOX_QTY"].ToString());
                        temp_qty+= Convert.ToInt32(reader["QTY"].ToString());
                        if (string.IsNullOrEmpty(customer.WEEK)) customer.WEEK = reader["WEEK"].ToString();
                        if (string.IsNullOrEmpty(customer.LOT)) customer.LOT = reader["LOT"].ToString();
                        if (string.IsNullOrEmpty(customer.SHIPDATE)) customer.SHIPDATE = reader["SHIP_DATE"].ToString();
                    }
                    customer.BOX_QTY = temp_box_qty.ToString();
                    customer.BATTERY_QTY = temp_qty.ToString();
                    reader.Close();
                }
                catch(Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return;
                }
            }

        }
        public static void GetBoxDataBySAPBoxID(string boxid, ref CustomerDS customer)
        {
            if (string.IsNullOrEmpty(boxid) || customer == null)
            {
                return;
            }
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetBoxDataBySAPBoxID => Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT 0 BOX_QTY,TO_CHAR(FLOOR(SUM(QTY))) BATTERY_QTY FROM Z_CELL_PACK WHERE CATEGORY = 'BOX' AND NUM = '{0}';", boxid), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!string.IsNullOrEmpty(reader["BATTERY_QTY"].ToString()))
                        {
                            customer.BOX_QTY = reader["BOX_QTY"].ToString();
                            customer.BATTERY_QTY = reader["BATTERY_QTY"].ToString().Split('.')[0];
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return;
                }
            }
        }
        public static EntityBattery GetBatterynoByBatch(string batch)
        {
            try
            {
                EntityBattery entiey = new EntityBattery();
                using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                {
                    mConn.Open();
                    SqlCommand mComm = new SqlCommand(string.Format("SELECT 'V_' + OTHER_NAME + '_' + P.BOMNO + '_BOX' FROM M_BOMNO M INNER JOIN T_BOM_PIPELINE P ON M.BOMNO = P.BOMNO INNER JOIN M_DEPARTMENT D ON P.PIPELINENAME = D.DEPARTMENTNAME WHERE M.SHORTBOM = '{0}'; ", batch.Substring(0, 3)), mConn);
                    SqlDataReader SDR_TableInfo = mComm.ExecuteReader();
                    List<string> sqlList = new List<string>();
                    while (SDR_TableInfo.Read())
                    {
                        sqlList.Add(string.Format("SELECT BATTERYNO FROM {1} WHERE BOXID = '{0}';", batch, SDR_TableInfo[0].ToString()));
                    }
                    SDR_TableInfo.Close();
                    foreach (string sql in sqlList)
                    {
                        SqlCommand mComm2 = new SqlCommand(sql, mConn);
                        SqlDataReader SDR_SnList = mComm2.ExecuteReader();
                        while (SDR_SnList.Read())
                        {
                            CellSn cell = new CellSn();
                            cell.SN = SDR_SnList[0].ToString();
                            entiey.SN_LIST.Add(cell);
                        }
                        SDR_SnList.Close();
                    }
                    if (entiey.SN_LIST.Count > 0)
                    {
                        entiey.REQ_ID = "REQ_LSN" + TimeStamp.GetTimeStamp();
                        entiey.RESULT = "1";
                        entiey.MESSAGE = "下载成功";
                        entiey.CARRIER_BATCH_NO = batch;
                    }
                    else
                    {
                        entiey.REQ_ID = "REQ_LSN" + TimeStamp.GetTimeStamp();
                        entiey.RESULT = "2";
                        entiey.MESSAGE = string.Format("未找到批次[{0}]的电芯数据；", batch);
                        entiey.CARRIER_BATCH_NO = batch;
                    }
                }
                return entiey;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message + " Datum::GetBatterynoByBatch");
                return null;
            }
        }
        public static int Write_HW_Recieve_Log(string content)
        {
            try
            {
                using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
                {
                    mConn.Open();
                    SqlCommand mComm = new SqlCommand(string.Format("INSERT INTO HW_RECIEVE_LOG (RECIEVE_INFO) VALUES ('{0}');", content), mConn);
                    int result = mComm.ExecuteNonQuery();
                    SysLog log = new SysLog(mComm.CommandText);
                    return result;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("Datum::Write_HW_Recieve_Log " + ex.Message);
                return 0;
            }
        }
        public static BoxWT GetBoxInfo(string bomno, string customer, string boxid)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
            {
                try
                {
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetBoxInfo => fail to open database.");
                    }
                    SqlCommand mComm = new SqlCommand(string.Format("SELECT A.*,COUNT(B.BATTERYNO) QTY FROM (SELECT TOP 1 B.BOMNO,B.BOXID,MIN(B.CREATTIME) PACKAGEDATE ,C.ADDRESS PO,C.VENDOR,O.REALREALDATE LOTDATE,C.TEL Description FROM V_WJ1_{0}_BOX B LEFT JOIN M_CUSTOMER C ON C.BOMNO = B.BOMNO AND C.CUSTOMER_NAME = '{1}' LEFT JOIN V_WJ1_{0}_B1 B1 ON B.BATTERYNO = B1.BATTERYNO LEFT JOIN M_ORDER O ON B1.ORDERNO = O.ORDERNO WHERE B.BOXID = '{2}' GROUP BY B.BOMNO,B.BOXID,B.CODEDATE,C.ADDRESS,C.VENDOR,O.REALREALDATE,C.TEL ORDER BY LOTDATE ASC) A INNER JOIN V_WJ1_{0}_BOX B ON A.BOXID = B.BOXID GROUP BY A.BOMNO,A.BOXID,A.PACKAGEDATE,A.PO,A.VENDOR,A.LOTDATE,A.Description;", bomno, customer, boxid), mConn);
                    SqlDataReader reader = mComm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception("Datum::GetBoxInfo => data be empty.");
                    }
                    BoxWT entity = new BoxWT();
                    reader.Read();
                    entity.BOMNO = reader["BOMNO"].ToString();
                    entity.BOXID = reader["BOXID"].ToString();
                    entity.Description = reader["DESCRIPTION"].ToString();
                    entity.LOTDATE = reader["LOTDATE"].ToString();
                    entity.PACKAGEDATE = reader["PACKAGEDATE"].ToString();
                    entity.PO = reader["PO"].ToString();
                    entity.QTY = reader["QTY"].ToString();
                    entity.VENDOR = reader["VENDOR"].ToString();
                    return entity;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取 Powergene 包装箱信息
        /// </summary>
        /// <param name="customer">客户</param>
        /// <param name="boxid">箱号</param>
        /// <returns></returns>
        public static BoxPowergene GetPowergeneBoxInfo(string customer, string boxid)
        {
            if (string.IsNullOrEmpty(customer) || string.IsNullOrEmpty(boxid))
            {
                return null;
            }
            BoxPowergene result = new BoxPowergene();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetPowergeneBoxInfo => SAP_PACK Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand($"SELECT CF.ATTRIBUTE,CF.VALUE,ZCP.QTY,TO_CHAR(ZCP.CREATED_DATE_TIME,'YYYY/MM/DD') CREATED_DATE_TIME FROM ITEM i INNER JOIN CUSTOM_FIELDS cf ON I.HANDLE = CF.HANDLE INNER JOIN Z_CELL_PACK ZCP ON I.ITEM = ZCP.ITEM AND I.CURRENT_REVISION = 'true' INNER JOIN CUSTOMER C ON C.CUSTOMER = ZCP.CUSTOMER WHERE C.CUSTOMER_NAME = '{customer}' AND ZCP.NUM = '{boxid}' AND (CF.ATTRIBUTE = 'PO_CUSTOMER' OR CF.ATTRIBUTE = 'CORE_TYPE_CODE');", conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) return null;
                    while (reader.Read())
                    {
                        if (reader["ATTRIBUTE"].ToString() == "PO_CUSTOMER")
                            result.PO_NUMBER = reader["VALUE"].ToString();
                        else
                        {
                            result.VERDOR_PN = reader["VALUE"].ToString();
                            result.TYPE = reader["VALUE"].ToString();
                        }
                        result.V_QTY = reader["QTY"].ToString();
                        result.DATE_CODE = reader["CREATED_DATE_TIME"].ToString();
                    }
                    reader.Close();
                    conn.Close();
                }
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["COLORS"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetPowergeneBoxInfo => SyncRemote Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT F.FIELDS_KEY,F.FIELDS_VALUE FROM FIELDS f INNER JOIN CUSTOMER c ON F.HANDLE_CUSTOMER = C.HANDLE INNER JOIN BOM B ON F.HANDLE_BOM = B.HANDLE AND F.STATE IS NULL AND C.STATE IS NULL AND B.STATE IS NULL AND C.CUSTOMER = '{customer}' AND B.BOM = '{result.TYPE}';", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) return null;
                    while (reader.Read())
                    {
                        if (string.IsNullOrEmpty(reader["FIELDS_VALUE"].ToString())) continue;

                        foreach (PropertyInfo item in result.GetType().GetProperties())
                        {
                            if (reader["FIELDS_KEY"].ToString() == item.Name)
                            {
                                item.SetValue(result, reader["FIELDS_VALUE"]);
                            }
                        }
                    }
                    reader.Close();
                    conn.Close();
                }
            }
            catch(Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }

            return result;
        }
        /// <summary>
        /// 获取 Anker 包装箱信息
        /// 使用了行转列，多行组合等 SQL 方法
        /// </summary>
        /// <param name="customer">客户</param>
        /// <param name="boxid">箱号</param>
        /// <returns></returns>

        public static BoxAnker GetAnkerBoxInfo(string customer, string boxid)
        {
            if (string.IsNullOrEmpty(customer) || string.IsNullOrEmpty(boxid))
            {
                return null;
            }
            BoxAnker result = new BoxAnker();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetPowergeneBoxInfo => SAP_PACK Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand($@"
                        SELECT 
	                    DISTINCT 
	                    CF.ATTRIBUTE
	                    ,CF.VALUE VALUE
                    FROM ITEM i
                    INNER JOIN CUSTOM_FIELDS cf ON I.HANDLE = CF.HANDLE
                    INNER JOIN Z_CELL_PACK ZCP ON I.ITEM = ZCP.ITEM
	                    AND I.CURRENT_REVISION = 'true'
                    INNER JOIN CUSTOMER C ON C.CUSTOMER = ZCP.CUSTOMER
                    WHERE C.CUSTOMER_NAME LIKE '%{customer}%'
	                    AND ZCP.NUM = '{boxid}'
	                    AND CF.ATTRIBUTE IN ('CUSTOMER_ITEM','CUSTOMER_ITEM_DESCRIPTION','PO_CUSTOMER','SUPPLIER_NUMBER')
                    UNION ALL
                    SELECT DISTINCT 'QTY' ATTR,TO_CHAR(ZCP.QTY,'9999999') VALUE FROM Z_CELL_PACK ZCP WHERE ZCP.NUM = '{boxid}'
                    UNION 
                    SELECT DISTINCT 'PRODUCT_DATE' ATTRIBUTE,TO_CHAR(ZCP.CREATED_DATE_TIME, 'YYYY/MM/DD') VALUE FROM Z_CELL_PACK ZCP WHERE ZCP.NUM = '{boxid}'
                    UNION 
                    SELECT ATTRIBUTE,STRING_AGG(VALUE,' ') FROM (SELECT DISTINCT 'GROUP_INFO' ATTRIBUTE ,(CASE ZGSP.PARAM_DESC WHEN '放电容量2' THEN TO_CHAR(ZGSP.STANDARD_LOWER_LIMIT,'999999') || '-' || TO_CHAR(ZGSP.STANDARD_UPPER_LIMIT,'999999') || 'mAh' WHEN '电压' THEN TO_CHAR(ZGSP.STANDARD_LOWER_LIMIT/1000,'999999.999') || '-' || TO_CHAR(ZGSP.STANDARD_UPPER_LIMIT/1000,'999999.999') || 'V' WHEN '内阻' THEN TO_CHAR(ZGSP.STANDARD_LOWER_LIMIT,'999999') || '-' || TO_CHAR(ZGSP.STANDARD_UPPER_LIMIT,'999999') || 'mΩ' END) VALUE FROM Z_GRADE ZG INNER JOIN Z_GRADE_STANDARD ZGS ON ZG.GRADE_STANDARD_BO = ZGS.HANDLE INNER JOIN Z_GRADE_STANDARD_PARAM ZGSP ON ZG.HANDLE = ZGSP.GRADE_BO INNER JOIN Z_CELL_PACK ZCP ON ZGS.ITEM_NO = ZCP.ITEM AND ZG.GRADE = ZCP.GRADE WHERE ZGS.IS_CURRENT = 'Y' AND ZCP.NUM = '{boxid}') GROUP BY ATTRIBUTE
                    UNION 
                    SELECT ATTRIBUTE,STRING_AGG(VALUE,' ') FROM (SELECT DISTINCT 'NOTE' ATTRIBUTE,RIGHT(LEFT(ZCPS.SN,8),5) VALUE FROM Z_CELL_PACK ZCP INNER JOIN Z_CELL_PACK_SN ZCPS ON ZCP.HANDLE = ZCPS.CELL_PACK_BO WHERE ZCP.NUM = '{boxid}') GROUP BY ATTRIBUTE;
                        ", conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) return null;
                    while (reader.Read())
                    {
                        if (string.IsNullOrEmpty(reader["ATTRIBUTE"].ToString())) continue;

                        foreach (PropertyInfo item in result.GetType().GetProperties())
                        {
                            if (reader["ATTRIBUTE"].ToString() == item.Name)
                            {
                                item.SetValue(result, reader["VALUE"]);
                            }
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 获取回传数据，从追溯系统
        /// </summary>
        /// <param name="ID">箱号/栈板号</param>
        /// <returns></returns>
        public static int DataSyncAmazonKazamPerformanceCell(string ID)
        {
            List<EntityAmazonPerformanceCell> Cells = new List<EntityAmazonPerformanceCell>();
            // 新系统使用 SAP_PACK 连接字符串
            // 老系统使用 LSMES_5ANEW 连接字符串
            try
            {
                if (ConfigurationManager.AppSettings["IsSAP"].Trim().ToUpper() != "TRUE")   // 老系统
                {
                    SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString);
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetAmazonKazamPerformanceCell => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT M.OTHER_NAME,B.BOMNO,B.REMARKS FROM T_BOM_PIPELINE_FLOW T INNER JOIN M_DEPARTMENT M ON T.PIPELINENAME = M.DEPARTMENTNAME INNER JOIN M_BOMNO B ON B.BOMNO = T.BOMNO WHERE B.SHORTBOM = '{0}' GROUP BY M.OTHER_NAME,B.BOMNO,B.REMARKS;", ID.Substring(0, 3)), mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception(string.Format("Datum::GetAmazonKazamPerformanceCell => Did not find bomno by {0}", ID));
                    }
                    reader.Read();
                    string bomno = reader[1].ToString();
                    string item_no = reader[2].ToString();
                    string pipeline = reader[0].ToString();
                    reader.Close();
                    EntityAmazonConfig config = AmazonKazamConfig(item_no);
                    comm.CommandText = string.Format("SELECT '{2}' HANDLE_CONFIG, B7.BATTERYNO cell_sn ,'{3}' cell_supplier ,'{4}' cell_phase ,B7.BOMNO cell_model ,B7.E13OUT_CAPACITY2 cell_capacity ,CONVERT(NVARCHAR(20),B7.E13ENDTIME,120) cell_capacity_time ,B7.E5VOLTAGE1 cell_ocv1 ,CONVERT(NVARCHAR(20),B7.E5TESTTIME1,120) cell_ocv1_time ,B7.E5RESISTANCE1 cell_acr1 ,CONVERT(NVARCHAR(20),B7.E5TESTTIME1,120) cell_acr1_time ,B7.E6VOLTAGE2 cell_ocv2 ,CONVERT(NVARCHAR(20),B7.E6TESTTIME2,120) cell_ocv2_time ,B7.E6RESISTANCE2 cell_acr2 ,CONVERT(NVARCHAR(20),B7.E6TESTTIME2,120) cell_acr2_time ,B7.E6KVALUE cell_k_value ,B7.E14WEIGHT cell_thickness ,'' cell_width, '' cell_length ,B7.E16BATTERYWEIGH cell_weight ,B7.DZ_VOLTAGE cell_shipping_ocv ,B7.DZ_RESISTANCE cell_shipping_acr ,CONVERT(NVARCHAR(20),B7.E18TESTTIME,120) cell_shipping_time ,B.BOXID cell_carton_no ,C.CASESID cell_pallet_no ,'' cell_lot_no ,'' cell_shipping_no ,CONVERT(NVARCHAR(10),C.CREATTIME,23) cell_shipping_date ,'' state FROM V_{5}_{1}_CASE C INNER JOIN V_{5}_{1}_BOX B ON C.BOXID = B.BOXID INNER JOIN V_{5}_{1}_B7 B7 ON B.BATTERYNO = B7.BATTERYNO WHERE C.CASESID = '{0}';", ID, bomno, config.handle, config.supplier, config.phase, pipeline);
                    reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        throw new Exception(string.Format("Datum::GetAmazonKazamPerformanceCell => No data to send was found by {0}", ID));
                    }
                    while (reader.Read())
                    {
                        EntityAmazonPerformanceCell cell = new EntityAmazonPerformanceCell();
                        cell.handle_config = reader["HANDLE_CONFIG"].ToString();
                        cell.cell_sn = reader["cell_sn"].ToString();
                        cell.cell_supplier = reader["cell_supplier"].ToString();
                        cell.cell_phase = reader["cell_phase"].ToString();
                        cell.cell_model = reader["cell_model"].ToString();
                        cell.cell_capacity = reader["cell_capacity"].ToString();
                        cell.cell_capacity_time = reader["cell_capacity_time"].ToString();
                        cell.cell_ocv1 = reader["cell_ocv1"].ToString();
                        cell.cell_ocv1_time = reader["cell_ocv1_time"].ToString();
                        cell.cell_acr1 = reader["cell_acr1"].ToString();
                        cell.cell_acr1_time = reader["cell_acr1_time"].ToString();
                        cell.cell_ocv2 = reader["cell_ocv2"].ToString();
                        cell.cell_ocv2_time = reader["cell_ocv2_time"].ToString();
                        cell.cell_acr2 = reader["cell_acr2"].ToString();
                        cell.cell_acr2_time = reader["cell_acr2_time"].ToString();
                        cell.cell_k_value = reader["cell_k_value"].ToString();
                        cell.cell_thickness = reader["cell_thickness"].ToString();
                        cell.cell_width = reader["cell_width"].ToString();
                        cell.cell_length = reader["cell_length"].ToString();
                        cell.cell_weight = reader["cell_weight"].ToString();
                        cell.cell_shipping_ocv = reader["cell_shipping_ocv"].ToString();
                        cell.cell_shipping_acr = reader["cell_shipping_acr"].ToString();
                        cell.cell_shipping_time = reader["cell_shipping_time"].ToString();
                        cell.cell_carton_no = reader["cell_carton_no"].ToString();
                        cell.cell_pallet_no = reader["cell_pallet_no"].ToString();
                        cell.cell_lot_no = reader["cell_lot_no"].ToString();
                        cell.cell_shipping_no = !string.IsNullOrEmpty(reader["cell_shipping_no"].ToString()) ? reader["cell_shipping_no"].ToString() : null;
                        cell.cell_shipping_date = !string.IsNullOrEmpty(reader["cell_shipping_date"].ToString()) ? reader["cell_shipping_date"].ToString() : null;
                        cell.state = reader["state"].ToString();
                        Cells.Add(cell);
                    }
                    reader.Close();
                }
                else  // 新系统
                {
                    OdbcConnection mConn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString);
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetAmazonKazamPerformanceCell => fail to open database.");
                    }
                    // 2024/04/11 注释，使用简化方式查询
                    //OdbcCommand comm = new OdbcCommand(string.Format("SELECT I.ITEM FROM CUSTOM_FIELDS cf INNER JOIN ITEM I ON CF.HANDLE = I.HANDLE INNER JOIN ITEM_GROUP IG ON IG.ITEM_GROUP = 'M15' INNER JOIN ITEM_GROUP_MEMBER IGM ON I.HANDLE = IGM.ITEM_BO AND IGM.ITEM_GROUP_BO = IG.HANDLE WHERE ATTRIBUTE='MAT_CODE' AND CF.VALUE = '{0}';", ID.Substring(0, 3)), mConn);
                    OdbcCommand comm = new OdbcCommand($"SELECT ITEM FROM Z_CELL_PACK WHERE NUM = '{ID}';", mConn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        throw new Exception(string.Format("Datum::GetAmazonKazamPerformanceCell => No data to send was found by {0}", ID));
                    }
                    string item_no = reader[0].ToString();
                    reader.Close();
                    EntityAmazonConfig config = AmazonKazamConfig(item_no);
                    comm.CommandText = string.Format("SELECT DISTINCT '{1}' HANDLE_CONFIG,ZCPS.SN cell_sn,'{2}' cell_supplier,'{3}' cell_phase,CF.VALUE  cell_model,ZOSP.A017 cell_capacity,ZOSP.A039 cell_capacity_time,ZOSP2.A004 cell_ocv1,ZOSP2.A003 cell_ocv1_time,ZOSP2.A002 cell_acr1,ZOSP2.A003 cell_acr1_time,ZOSP3.A005 cell_ocv2,ZOSP3.A004 cell_ocv2_time,ZOSP3.A003 cell_acr2,ZOSP3.A004 cell_acr2_time,ZOSP3.A012 cell_k_value,ZOSP4.A008 cell_thickness,'' cell_width,'' cell_length,ZOSP5.A002 cell_weight,ZOSP4.A001 cell_shipping_ocv,ZOSP4.A002 cell_shipping_acr,ZOSP4.A026 cell_shipping_time,ZCP2.NUM cell_carton_no,ZCP.NUM cell_pallet_no,'' cell_lot_no,'' cell_shipping_no,ZCP.CREATED_DATE_TIME cell_shipping_date ,'' STATE FROM Z_CELL_PACK zcp INNER JOIN Z_CELL_PACK zcp2 ON ZCP.HANDLE = ZCP2.PARENT_BO INNER JOIN Z_CELL_PACK_SN zcps ON ZCPS.CELL_PACK_BO = ZCP2.HANDLE INNER JOIN ITEM I ON ZCPS.ITEM = I.ITEM INNER JOIN CUSTOM_FIELDS CF ON I.HANDLE = CF.HANDLE AND CF.ATTRIBUTE = 'CORE_TYPE_CODE' LEFT JOIN Z_OP58_SFC_PARAM zosp ON ZCPS.SN = ZOSP.SN AND ZOSP.IS_CURRENT = 'Y' LEFT JOIN Z_OP09_SFC_PARAM zosp2 ON ZOSP2.SN = ZCPS.SN AND ZOSP2.IS_CURRENT = 'Y' LEFT JOIN Z_OP11_SFC_PARAM zosp3 ON ZOSP3.SN = ZCPS.SN AND ZOSP3.IS_CURRENT = 'Y' LEFT JOIN Z_OP13_SFC_PARAM zosp4 ON ZOSP4.SN = ZCPS.SN AND ZOSP4.IS_CURRENT = 'Y' LEFT JOIN Z_OP08_SFC_PARAM zosp5 ON ZOSP5.SN = ZCPS.SN AND ZOSP5.IS_CURRENT = 'Y' WHERE ZCP.NUM ='{0}';", ID, config.handle, config.supplier, config.phase);
                    reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        throw new Exception(string.Format("Datum::GetAmazonKazamPerformanceCell => No data to send was found by {0}", ID));
                    }
                    while (reader.Read())
                    {
                        EntityAmazonPerformanceCell cell = new EntityAmazonPerformanceCell();
                        cell.handle_config = reader["HANDLE_CONFIG"].ToString();
                        cell.cell_sn = reader["cell_sn"].ToString();
                        cell.cell_supplier = reader["cell_supplier"].ToString();
                        cell.cell_phase = reader["cell_phase"].ToString();
                        cell.cell_model = reader["cell_model"].ToString();
                        cell.cell_capacity = reader["cell_capacity"].ToString();
                        cell.cell_capacity_time = reader["cell_capacity_time"].ToString();
                        cell.cell_ocv1 = reader["cell_ocv1"].ToString();
                        cell.cell_ocv1_time = reader["cell_ocv1_time"].ToString();
                        cell.cell_acr1 = reader["cell_acr1"].ToString();
                        cell.cell_acr1_time = reader["cell_acr1_time"].ToString();
                        cell.cell_ocv2 = reader["cell_ocv2"].ToString();
                        cell.cell_ocv2_time = reader["cell_ocv2_time"].ToString();
                        cell.cell_acr2 = reader["cell_acr2"].ToString();
                        cell.cell_acr2_time = reader["cell_acr2_time"].ToString();
                        cell.cell_k_value = reader["cell_k_value"].ToString();
                        cell.cell_thickness = reader["cell_thickness"].ToString();
                        cell.cell_width = reader["cell_width"].ToString();
                        cell.cell_length = reader["cell_length"].ToString();
                        cell.cell_weight = reader["cell_weight"].ToString();
                        cell.cell_shipping_ocv = reader["cell_shipping_ocv"].ToString();
                        cell.cell_shipping_acr = reader["cell_shipping_acr"].ToString();
                        cell.cell_shipping_time = reader["cell_shipping_time"].ToString();
                        cell.cell_carton_no = reader["cell_carton_no"].ToString();
                        cell.cell_pallet_no = reader["cell_pallet_no"].ToString();
                        cell.cell_lot_no = reader["cell_lot_no"].ToString();
                        cell.cell_shipping_no = !string.IsNullOrEmpty(reader["cell_shipping_no"].ToString()) ? reader["cell_shipping_no"].ToString() : null;
                        cell.cell_shipping_date = !string.IsNullOrEmpty(reader["cell_shipping_date"].ToString()) ? reader["cell_shipping_date"].ToString() : null;
                        cell.state = reader["state"].ToString();
                        Cells.Add(cell);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetAmazonKazamPerformanceCell => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("DELETE FROM AMAZON_KAZAM_CELL WHERE (CELL_CARTON_NO ='{0}' OR CELL_PALLET_NO ='{0}') AND (CELL_SHIPPING_NO = '' OR CELL_SHIPPING_NO IS NULL) AND (CELL_SHIPPING_DATE = '' OR CELL_SHIPPING_DATE IS NULL);", ID), mConn);
                    int rows = comm.ExecuteNonQuery();
                    StringBuilder sql = new StringBuilder();
                    for (int i = 0; i < Cells.Count; ++i)
                    {
                        sql.Append("INSERT INTO AMAZON_KAZAM_CELL (HANDLE_CONFIG,cell_sn,cell_supplier,cell_phase,cell_model,cell_capacity,cell_capacity_time,cell_ocv1,cell_ocv1_time,cell_acr1,cell_acr1_time,cell_ocv2,cell_ocv2_time,cell_acr2,cell_acr2_time,cell_k_value,cell_thickness,cell_width,cell_length,cell_weight,cell_shipping_ocv,cell_shipping_acr,cell_shipping_time,cell_carton_no,cell_pallet_no,cell_lot_no,cell_shipping_no,cell_shipping_date) VALUES (");
                        sql.Append("'");
                        sql.Append(Cells[i].handle_config);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_sn);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_supplier);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_phase);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_model);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_capacity);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_capacity_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_ocv1);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_ocv1_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_acr1);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_acr1_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_ocv2);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_ocv2_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_acr2);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_acr2_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_k_value);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_thickness);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_width);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_length);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_weight);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_shipping_ocv);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_shipping_acr);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_shipping_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_carton_no);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_pallet_no);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_lot_no);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_shipping_no);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_shipping_date);
                        sql.Append("');");
                    }
                    SqlTransaction tran = mConn.BeginTransaction();
                    comm.CommandText = sql.ToString();
                    comm.Transaction = tran;
                    int affectRow = comm.ExecuteNonQuery();
                    tran.Commit();
                    return affectRow;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return Cells.Count;
                }
            }
        }
        public static int UpdateAmazonKazamPerformanceCell(List<EntityAmazonPerformanceCell> Cells)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::UpdateAmazonKazamPerformanceCell => fail to open database.");
                    }
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sql2 = new StringBuilder();
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = mConn;
                    for (int i = 0; i < Cells.Count; ++i)
                    {
                        sql2.Append(string.Format("DELETE FROM AMAZON_KAZAM_CELL WHERE CELL_SN = '{0}';", Cells[i].cell_sn));
                        sql.Append("INSERT INTO AMAZON_KAZAM_CELL (HANDLE_CONFIG,cell_sn,cell_supplier,cell_phase,cell_model,cell_capacity,cell_capacity_time,cell_ocv1,cell_ocv1_time,cell_acr1,cell_acr1_time,cell_ocv2,cell_ocv2_time,cell_acr2,cell_acr2_time,cell_k_value,cell_thickness,cell_width,cell_length,cell_weight,cell_shipping_ocv,cell_shipping_acr,cell_shipping_time,cell_carton_no,cell_pallet_no,cell_lot_no,cell_shipping_no,cell_shipping_date) VALUES (");
                        sql.Append("'");
                        sql.Append(Cells[i].handle_config);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_sn);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_supplier);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_phase);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_model);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_capacity);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_capacity_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_ocv1);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_ocv1_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_acr1);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_acr1_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_ocv2);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_ocv2_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_acr2);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_acr2_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_k_value);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_thickness);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_width);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_length);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_weight);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_shipping_ocv);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_shipping_acr);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_shipping_time);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_carton_no);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_pallet_no);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_lot_no);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_shipping_no);
                        sql.Append("','");
                        sql.Append(Cells[i].cell_shipping_date);
                        sql.Append("');");
                        comm.CommandText = sql2.ToString();
                        comm.ExecuteNonQuery();
                        comm.CommandText = sql.ToString();
                        comm.ExecuteNonQuery();
                        sql2.Clear();
                        sql.Clear();
                    }
                    //SqlCommand comm = new SqlCommand(sql2.ToString(), mConn);
                    //SqlTransaction tran = mConn.BeginTransaction();
                    //comm.Transaction = tran;
                    //comm.ExecuteNonQuery();
                    //comm.CommandText = sql.ToString();
                    //int affect_Rows = comm.ExecuteNonQuery();
                    //tran.Commit();
                    //return affect_Rows;
                    return Cells.Count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 从发送服务器获取相关数据
        /// </summary>
        /// <param name="ID">箱号/栈板号</param>
        /// <returns></returns>
        public static List<EntityAmazonPerformanceCell> GetAmazonKazamPerformanceCell(string ID)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetAmazonKazamPerformanceCell => fail to open database.");
                    }
                    ID = "'" + ID.Replace(",", "','") + "'";
                    List<EntityAmazonPerformanceCell> Cells = new List<EntityAmazonPerformanceCell>();
                    SqlCommand comm = new SqlCommand($"SELECT * FROM AMAZON_KAZAM_CELL WHERE cell_pallet_no IN ({ID}) OR cell_carton_no IN ({ID}) AND STATE = 'READY';", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        EntityAmazonPerformanceCell cell = new EntityAmazonPerformanceCell();
                        cell.handle_config = reader["HANDLE_CONFIG"].ToString();
                        cell.cell_sn = reader["cell_sn"].ToString();
                        cell.cell_supplier = reader["cell_supplier"].ToString();
                        cell.cell_phase = reader["cell_phase"].ToString();
                        cell.cell_model = reader["cell_model"].ToString();
                        cell.cell_capacity = reader["cell_capacity"].ToString();
                        cell.cell_capacity_time = reader["cell_capacity_time"].ToString() == "1900/1/1 0:00:00" ? "" : ((DateTime)reader["cell_capacity_time"]).ToString("yyyy-MM-dd HH:mm:ss");
                        cell.cell_ocv1 = reader["cell_ocv1"].ToString();
                        cell.cell_ocv1_time = reader["cell_ocv1_time"].ToString() == "1900/1/1 0:00:00" ? "" : ((DateTime)reader["cell_ocv1_time"]).ToString("yyyy-MM-dd HH:mm:ss");
                        cell.cell_acr1 = reader["cell_acr1"].ToString();
                        cell.cell_acr1_time = reader["cell_acr1_time"].ToString()== "1900/1/1 0:00:00" ? "" : ((DateTime)reader["cell_acr1_time"]).ToString("yyyy-MM-dd HH:mm:ss");
                        cell.cell_ocv2 = reader["cell_ocv2"].ToString();
                        cell.cell_ocv2_time = reader["cell_ocv2_time"].ToString() == "1900/1/1 0:00:00" ? "" : ((DateTime)reader["cell_ocv2_time"]).ToString("yyyy-MM-dd HH:mm:ss");
                        cell.cell_acr2 = reader["cell_acr2"].ToString();
                        cell.cell_acr2_time = reader["cell_acr2_time"].ToString() == "1900/1/1 0:00:00" ? "" : ((DateTime)reader["cell_acr2_time"]).ToString("yyyy-MM-dd HH:mm:ss");
                        cell.cell_k_value = reader["cell_k_value"].ToString();
                        cell.cell_thickness = reader["cell_thickness"].ToString();
                        cell.cell_width = reader["cell_width"].ToString();
                        cell.cell_length = reader["cell_length"].ToString();
                        cell.cell_weight = reader["cell_weight"].ToString();
                        cell.cell_shipping_ocv = reader["cell_shipping_ocv"].ToString();
                        cell.cell_shipping_acr = reader["cell_shipping_acr"].ToString();
                        cell.cell_shipping_time = reader["cell_shipping_time"].ToString() == "1900/1/1 0:00:00" ? "" : ((DateTime)reader["cell_shipping_time"]).ToString("yyyy-MM-dd HH:mm:ss");
                        cell.cell_carton_no = reader["cell_carton_no"].ToString();
                        cell.cell_pallet_no = reader["cell_pallet_no"].ToString();
                        cell.cell_lot_no = reader["cell_lot_no"].ToString();
                        cell.cell_shipping_no = reader["cell_shipping_no"].ToString();
                        cell.cell_shipping_date = reader["cell_shipping_date"].ToString() == "1900/1/1 0:00:00" ? "" : ((DateTime)reader["cell_shipping_date"]).ToString("yyyy-MM-dd");
                        cell.state = reader["state"].ToString();
                        Cells.Add(cell);
                    }
                    return Cells;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取 Amazon Kazam 配置 Handle
        /// </summary>
        /// <param name="item_no">料号</param>
        /// <returns></returns>
        public static string HandleAmazonKazamConfig(string item_no)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::HandleAmazonKazamConfig => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT HANDLE FROM AMAZON_CONFIG WHERE ITEM_NO = '{0}' AND STATE = 'Y';", item_no), mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception(string.Format("Datum::HandleAmazonKazamConfig => Did not HANDLE_CONFIG by {0}", item_no));
                    }
                    reader.Read();
                    string handle = reader[0].ToString();
                    reader.Close();
                    return handle;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取 Amazon Kazam config
        /// </summary>
        /// <param name="item_no">料号</param>
        /// <returns></returns>
        public static EntityAmazonConfig AmazonKazamConfig(string item_no)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::AmazonKazamConfig => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT * FROM AMAZON_CONFIG WHERE ITEM_NO = '{0}' AND STATE = 'Y';", item_no), mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception(string.Format("Datum::AmazonKazamConfig => No data was found by {0}", item_no));
                    }
                    EntityAmazonConfig entity = new EntityAmazonConfig();
                    while (reader.Read())
                    {
                        entity.handle = reader["handle"].ToString();
                        entity.item_no = reader["item_no"].ToString();
                        entity.factory_id = reader["factory_id"].ToString();
                        entity.supplier = reader["supplier"].ToString();
                        entity.project = reader["project"].ToString();
                        entity.phase = reader["phase"].ToString();
                        entity.ptype = reader["ptype"].ToString();
                        entity.state = reader["state"].ToString();
                        entity.comments = reader["comments"].ToString();
                        entity.created_date_time = reader["created_date_time"].ToString();
                    }
                    reader.Close();
                    return entity;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 统计可供回传的电池
        /// </summary>
        /// <param name="type">CELL / PACK</param>
        /// <returns></returns>
        public static List<AmazonKazamStatistics> Statistics(string type)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::Statistics => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = mConn;
                    if (type.ToUpper() == "CELL")
                    {
                        comm.CommandText = "SELECT C.ITEM_NO,C.PROJECT,C.PHASE,C.PTYPE,C.STATE,A.CELL_PALLET_NO,SUM(CASE WHEN A.STATE = 'READY' THEN 1 ELSE 0 END) AS READY,COUNT(1) AS TOTAL FROM AMAZON_KAZAM_CELL A INNER JOIN AMAZON_CONFIG C ON C.HANDLE = A.HANDLE_CONFIG GROUP BY C.ITEM_NO,C.PROJECT,C.PHASE,C.PTYPE,C.STATE,A.CELL_PALLET_NO;";
                    }
                    else if (type.ToUpper() == "PACK")
                    {
                        comm.CommandText = "SELECT C.ITEM_NO,C.PROJECT,C.PHASE,C.PTYPE,C.STATE,A.PACK_PLT_NO,SUM(CASE WHEN A.STATE = 'Y' THEN 1 ELSE 0 END) AS READY,COUNT(1) AS TOTAL FROM AMAZON_KAZAM_PACK A INNER JOIN AMAZON_CONFIG C ON C.HANDLE = A.HANDLE_CONFIG GROUP BY C.ITEM_NO,C.PROJECT,C.PHASE,C.PTYPE,C.STATE,A.PACK_PLT_NO;";
                    }
                    else
                    {
                        throw new Exception("Statistics cannot be performed because the type does not match");
                    }
                    SqlDataReader reader = comm.ExecuteReader();
                    List<AmazonKazamStatistics> Entitys = new List<AmazonKazamStatistics>();
                    while (reader.Read())
                    {
                        AmazonKazamStatistics entity = new AmazonKazamStatistics();
                        entity.item_no = reader["item_no"].ToString();
                        entity.project = reader["project"].ToString();
                        entity.phase = reader["phase"].ToString();
                        entity.ready = reader["ready"].ToString();
                        entity.total = reader["total"].ToString();
                        entity.type = reader["ptype"].ToString();
                        entity.state = reader["state"].ToString();
                        if (type.ToUpper() == "CELL")
                        {
                            entity.pallet_no = reader["CELL_PALLET_NO"].ToString();
                        }
                        else if (type.ToUpper() == "PACK")
                        {
                            entity.pallet_no = reader["PACK_PLT_NO"].ToString();
                        }
                        Entitys.Add(entity);
                    }
                    return Entitys;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 将可以回传的电池 state 更新为 READY
        /// </summary>
        /// <param name="barcodeList">码号列表</param>
        /// <param name="type">CELL / PACK</param>
        /// <returns></returns>
        public static int Ready(List<AmazonBarcode> barcodeList, string type)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::Ready => fail to open database.");
                    }
                    StringBuilder code = new StringBuilder();

                    SqlCommand comm = new SqlCommand();
                    comm.Connection = mConn;
                    if (type.ToUpper() == "CELL")
                    {
                        for (int i = 0; i < barcodeList.Count; ++i)
                        {
                            //code.Append(string.Format("'{0}',", barcodeList[i].barcode));
                            //comm.CommandText = string.Format("UPDATE AMAZON_KAZAM_CELL SET STATE  = 'READY' WHERE CELL_SN IN ({0});", code.ToString().Substring(0, code.Length - 1));
                            code.Append(string.Format("UPDATE AMAZON_KAZAM_CELL SET STATE  = 'READY' WHERE CELL_SN = '{0}';", barcodeList[i].barcode));
                        }
                        comm.CommandText = code.ToString();
                    }
                    else if (type.ToUpper() == "PACK")
                    {
                        for (int i = 0; i < barcodeList.Count; ++i)
                        {
                            //code.Append(string.Format("'{0}',", barcodeList[i].barcode));
                            //comm.CommandText = string.Format("UPDATE AMAZON_KAZAM_CELL SET STATE  = 'READY' WHERE CELL_SN IN ({0});", code.ToString().Substring(0, code.Length - 1));
                            code.Append(string.Format("UPDATE AMAZON_KAZAM_PACK SET STATE  = 'Y' WHERE BATTERY_SN = '{0}';", barcodeList[i].barcode));
                        }
                        comm.CommandText = code.ToString();
                    }
                    else
                    {
                        throw new Exception("type is error");
                    }
                    SqlTransaction tran = mConn.BeginTransaction();
                    comm.Transaction = tran;
                    int affect_rows = comm.ExecuteNonQuery();
                    tran.Commit();
                    return affect_rows;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 查询已完成回传数量，指定栈板
        /// </summary>
        /// <param name="id">栈板</param>
        /// <returns></returns>
        public static int AmazonQtyBackupByPalletNo(string id)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::QtyBackupByPalletNo => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT COUNT(1) QTY FROM AMAZON_KAZAM_CELL_BACKUP WHERE CELL_PALLET_NO = '{0}';", id), mConn);
                    int affect_rows = int.Parse(comm.ExecuteScalar().ToString());
                    return affect_rows;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// Amazon 备份数据还原至待上传数据中
        /// </summary>
        /// <param name="id">栈板号</param>
        /// <returns></returns>
        public static int RollBackBackupAmazon(string id,string type)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::RollBackBackupAmazon => fail to open database.");
                    }
                    string sql = null, sql2 = null;
                    if (type.Trim().ToUpper() == "CELL")
                    {
                        sql = string.Format("INSERT INTO AMAZON_KAZAM_CELL SELECT DISTINCT HANDLE_CONFIG,CELL_SN,CELL_SUPPLIER,CELL_PHASE,CELL_MODEL,CELL_CAPACITY,CELL_CAPACITY_TIME,CELL_OCV1,CELL_OCV1_TIME,CELL_ACR1,CELL_ACR1_TIME,CELL_OCV2,CELL_OCV2_TIME,CELL_ACR2,CELL_ACR2_TIME,CELL_K_VALUE,CELL_THICKNESS,CELL_WIDTH,CELL_LENGTH,CELL_WEIGHT,CELL_SHIPPING_OCV,CELL_SHIPPING_ACR,CELL_SHIPPING_TIME,CELL_CARTON_NO,CELL_PALLET_NO,CELL_LOT_NO,CELL_SHIPPING_NO,CELL_SHIPPING_DATE,STATE,CREATED_DATE_TIME FROM AMAZON_KAZAM_CELL_BACKUP WHERE CELL_CARTON_NO = '{0}' OR  CELL_PALLET_NO = '{0}';", id);
                        sql2 = string.Format("DELETE FROM AMAZON_KAZAM_CELL_BACKUP WHERE CELL_CARTON_NO = '{0}' OR CELL_PALLET_NO = '{0}';", id);
                    }
                    else if (type.Trim().ToUpper() == "PACK")
                    {
                        sql = string.Format("INSERT INTO AMAZON_KAZAM_PACK SELECT DISTINCT HANDLE_CONFIG,PACK_PLT_NO,PACK_SHP_DATE,PACK_SHP_NO,PACK_CTN_NO,PACK_SHP_TIME,PACK_LOT_NO,BATTERY_SN,CELL_SN,PCM_SN,PACK_OCV,PACK_ACR,PACK_TDOC,PACK_DOC,PACK_TSC,PACK_SRV,PACK_TCOC,PACK_COC,PACK_LGTH,PACK_WDTH,PACK_THK,PCM_OVP1,PCM_OVR1,PCM_UVP1,PCM_UVR1,PCM_COC1,PCM_DOC1,PCM_TOVP1,PCM_TUVP1,PCM_TCOC1,PCM_TDOC1,PCM_TSC1,PCM_PC,PCM_PDC,PCM_IMP,STATE,CREATED_DATE_TIME FROM AMAZON_KAZAM_PACK_BACKUP WHERE PACK_PLT_NO = '{0}';", id);
                        sql2 = string.Format("DELETE FROM AMAZON_KAZAM_PACK_BACKUP WHERE PACK_PLT_NO = '{0}';", id);
                    }
                    SqlCommand comm = new SqlCommand(sql, mConn);
                    SqlTransaction tran = mConn.BeginTransaction();
                    comm.Transaction = tran;
                    int affect_rows = comm.ExecuteNonQuery();
                    comm.CommandText = sql2;
                    comm.ExecuteNonQuery();
                    tran.Commit();
                    return affect_rows;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return -1;
                }
            }
        }
        /// <summary>
        /// 查询 LSMES 系统中是否存在该码号
        /// </summary>
        /// <param name="batteryno"></param>
        /// <returns></returns>
        public static int QtyBatteryNoByLSMES(string batteryno)
        {
            if (string.IsNullOrEmpty(batteryno)) return 0;
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::QtyBatteryNoByLSMES => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT COUNT(1) FROM T_REAL_FLOW_BATTERY WHERE BATTERYNO = '{0}';", batteryno), mConn);
                    int count = Convert.ToInt32(comm.ExecuteScalar());
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 查询 SAP ME 系统中是否存在该码号
        /// </summary>
        /// <param name="batteryno"></param>
        /// <returns></returns>
        public static int QtyBatteryNoBySapME(string batteryno)
        {
            if (string.IsNullOrEmpty(batteryno)) return 0;
            using (OdbcConnection mConn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_ME_WJ1"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::QtyBatteryNoBySapME => fail to open database.");
                    }
                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT COUNT(1) FROM Z_SFC_EXTEND zse WHERE SCAN_SN = '{0}';", batteryno), mConn);
                    int count = Convert.ToInt32(comm.ExecuteScalar());
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 查询 SAP PACK 系统中是否存在该码号
        /// </summary>
        /// <param name="batteryno"></param>
        /// <returns></returns>
        public static int QtyBatteryNoBySapPACK(string batteryno)
        {
            if (string.IsNullOrEmpty(batteryno)) return 0;
            using (OdbcConnection mConn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::QtyBatteryNoBySapPACK => fail to open database.");
                    }
                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT COUNT(1) FROM Z_SFC_BINDING_SN zsbs WHERE CELL_SN = '{0}';", batteryno), mConn);
                    int count = Convert.ToInt32(comm.ExecuteScalar());
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 查询日志是否包含此码号
        /// </summary>
        /// <param name="batteryno"></param>
        /// <returns></returns>
        public static int QtyBatteryNoByAvailabilityLog(string batteryno)
        {
            if (string.IsNullOrEmpty(batteryno)) return 0;
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AVAILABILITY"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::QtyBatteryNoByAvailabilityLog => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT COUNT(1) FROM AVAILABILITY_LOG WHERE BATTERYNO = '{0}';", batteryno), mConn);
                    int count = Convert.ToInt32(comm.ExecuteScalar());
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }

        }
        public static int AvailabilityLog (string batteryno,string source,string created_user,string contexts)
        {
            if (string.IsNullOrEmpty(batteryno)) return 0;
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AVAILABILITY"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::ResultAvailability => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO AVAILABILITY_LOG (BATTERYNO,SYSTEM_SOURCE,CREATED_USER,CONTEXTS) VALUES ('{0}','{1}','{2}','{3}');", batteryno, source, created_user, contexts), mConn);
                    int count = Convert.ToInt32(comm.ExecuteNonQuery());
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 解析喷码机上传的信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string Analysis(string info)
        {
            if (string.IsNullOrEmpty(info)) return null;
            try
            {
                string lot = Regex.Match(info, "(?<=])[0-9A-Z]{10}(?=,)").Groups[0].Value;
                string sn = Regex.Match(info, "(?<=])[0-9A-Z]{3}(?=,)").Groups[0].Value;
                if (string.IsNullOrEmpty(lot) || string.IsNullOrEmpty(sn)) throw new Exception("Regular expression parsing failed.");
                string barcode = lot + sn;
                return barcode;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 获取当前设备所生产型号信息
        /// </summary>
        /// <param name="resrce_no">设备编号</param>
        /// <returns></returns>
        public static ProductionModel ProductionModel(string resrce_no)
        {
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
            {
                ProductionModel entity = new App_Base.ProductionModel();
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::ProductionModel => fail to open database.");
                    }
                    OdbcCommand comm = new OdbcCommand(
                        $@"SELECT TOP 1 RS.RESRCE
                            , CF1.VALUE CORE_TYPE_CODE
                            , RS.ITEM
                            , CF2.VALUE MAT_CODE
                            , RS.SHOP_ORDER
                            , RS.PROCESS_LOT
                            , RS.CREATED_DATE_TIME
                        FROM Z_RESOURCE_SFC RS
                        INNER JOIN Z_SFC_BINDING_PROCESS zsbp ON RS.SFC = ZSBP.SFC
                            AND ZSBP.STATUS = 'Y'
                        INNER JOIN SFC ON SFC.SFC = RS.SFC
                            AND RS.SITE = SFC.SITE
                        INNER JOIN SFC_ROUTING SR ON SR.SFC_BO = sfc.HANDLE
                        INNER JOIN SFC_ROUTER SR1 ON SR1.SFC_ROUTING_BO = SR.HANDLE
                        INNER JOIN SFC_STEP SS ON SS.SFC_ROUTER_BO = SR1.HANDLE
                            AND SS.QTY_IN_WORK > 0
                            AND SS.OPERATION_BO = 'OperationBO:1000,' || RS.OPERATION || ',#'
                        INNER JOIN ITEM I ON I.HANDLE = SFC.ITEM_BO
                            AND I.CURRENT_REVISION = 'true'
                        INNER JOIN CUSTOM_FIELDS cf1 ON sfc.ITEM_BO = cf1.HANDLE
                            AND cf1.ATTRIBUTE = 'CORE_TYPE_CODE'
                        INNER JOIN CUSTOM_FIELDS cf2 ON sfc.ITEM_BO = cf2.HANDLE
                            AND cf2.ATTRIBUTE = 'MAT_CODE'
                        WHERE RS.RESRCE = '{resrce_no}'
                        ORDER BY RS.CREATED_DATE_TIME DESC; "
                        //$@"
                        //SELECT  WC.WORK_CENTER , RE.RESRCE ,SO.SHOP_ORDER ,SOT.ORDER_TYPE , SOT.DESCRIPTION ORDER_TYPE_DESC,  
                        //I.ITEM , I.DESCRIPTION ITEM_DESC , CF2.VALUE CORE_TYPE_CODE , CF3.VALUE MAT_CODE ,BB.BOM , RR.ROUTER ,SO.MODIFIED_DATE_TIME                                           
                        //FROM WORK_CENTER wc                                                                        
                        //LEFT JOIN CUSTOM_FIELDS cf ON CF.HANDLE = WC.HANDLE AND CF.ATTRIBUTE = 'CURRENT_SHOP_ORDER'
                        //LEFT JOIN SHOP_ORDER so ON SO.SHOP_ORDER = CF.VALUE AND SO.SITE = WC.SITE                  
                        //LEFT JOIN BOM BB ON BB.HANDLE = IFNULL(SO.BOM_BO , SO.PLANNED_BOM_BO)                      
                        //LEFT JOIN ROUTER RR ON RR.HANDLE = IFNULL(SO.ROUTER_BO , SO.PLANNED_ROUTER_BO)             
                        //LEFT JOIN SHOP_ORDER_TYPE sot ON SOT.HANDLE = SO.SHOP_ORDER_TYPE_BO                        
                        //LEFT JOIN ITEM i ON I.HANDLE = IFNULL(SO.ITEM_BO , SO.PLANNED_ITEM_BO) AND I.CURRENT_REVISION = 'true'                   
                        //LEFT JOIN CUSTOM_FIELDS cf2 ON CF2.HANDLE = I.HANDLE AND CF2.ATTRIBUTE = 'CORE_TYPE_CODE'
                        //LEFT JOIN CUSTOM_FIELDS cf3 ON I.HANDLE = cf3.HANDLE AND cf3.ATTRIBUTE = 'MAT_CODE'
                        //LEFT JOIN WORK_CENTER_MEMBER WCM ON WCM.WORK_CENTER_BO = WC.HANDLE
                        //LEFT JOIN RESRCE RE ON RE.HANDLE = WCM.WORK_CENTER_OR_RESOURCE_GBO
                        //WHERE RE.RESRCE = '{resrce_no}';"
                        , conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        entity.CORE_TYPE_CODE = reader["CORE_TYPE_CODE"].ToString();
                        entity.RESRCE = reader["RESRCE"].ToString();
                        entity.ITEM = reader["ITEM"].ToString();
                        entity.MAT_CODE = reader["MAT_CODE"].ToString();
                        entity.SHOP_ORDER = reader["SHOP_ORDER"].ToString();
                        entity.PROCESS_LOT = reader["PROCESS_LOT"].ToString();
                    }
                    reader.Close();
                    return entity;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return entity;
                }
            }
        }
        /// <summary>
        /// 获取海陆通基本信息
        /// </summary>
        /// <param name="customer">客户</param>
        /// <param name="boxid">箱号</param>
        /// <returns></returns>
        public static BoxHiLT GetHiLTBoxInfo(string customer, string boxid)
        {
            if (string.IsNullOrEmpty(customer) || string.IsNullOrEmpty(boxid))
            {
                return null;
            }
            BoxHiLT result = new BoxHiLT();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetPowergeneBoxInfo => SAP_PACK Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand($@"
                    SELECT DISTINCT CF.ATTRIBUTE
	                    ,CF.VALUE VALUE
                    FROM ITEM i
                    INNER JOIN CUSTOM_FIELDS cf ON I.HANDLE = CF.HANDLE
                    INNER JOIN Z_CELL_PACK ZCP ON I.ITEM = ZCP.ITEM
	                    AND I.CURRENT_REVISION = 'true'
                    INNER JOIN CUSTOMER C ON C.CUSTOMER = ZCP.CUSTOMER
                    WHERE C.CUSTOMER_NAME LIKE '%{customer}%'
	                    AND ZCP.NUM = '{boxid}'
	                    AND CF.ATTRIBUTE IN (
		                    'CUSTOMER_ITEM'
		                    ,'CORE_TYPE_CODE'
		                    )
                    UNION 
                    SELECT DISTINCT 'QTY' ATTRIBUTE, TO_CHAR(QTY,'9999') VALUE FROM Z_CELL_PACK ZCP WHERE ZCP.NUM = '{boxid}'
                    UNION 
                    SELECT DISTINCT 'CREATED_DATE_TIME' ATTRIBUTE, TO_CHAR(ZCP.CREATED_DATE_TIME,'YYYY-MM-DD') VALUE FROM Z_CELL_PACK ZCP WHERE ZCP.NUM = '{boxid}';
                        ", conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) return null;
                    while (reader.Read())
                    {
                        if (string.IsNullOrEmpty(reader["ATTRIBUTE"].ToString())) continue;

                        foreach (PropertyInfo item in result.GetType().GetProperties())
                        {
                            if (reader["ATTRIBUTE"].ToString() == item.Name)
                            {
                                item.SetValue(result, reader["VALUE"]);
                            }
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 获取五期乙临时上传数据工序
        /// </summary>
        /// <returns></returns>
        public static Hashtable GetProcess()
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetProcess => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT * FROM PROCESS WHERE STATE = 'Y';", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    Hashtable hashProcess = new Hashtable();
                    while (reader.Read())
                    {
                        EntityProcess process = new EntityProcess();
                        hashProcess.Add(reader["PROCESS"], reader["HANDLE"]);
                    }
                    reader.Close();
                    return hashProcess;
                }
                catch(Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 根据工序 HANDLE 获取要上传的数据项目
        /// </summary>
        /// <param name="handle">工序 HANDLE</param>
        /// <returns></returns>
        public static Hashtable GetRelation(string handle)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetRelation => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT R.HANDLE ,R.FIELD ,R.PRIMARY_KEY FROM PROCESS P INNER JOIN RELATION R ON P.HANDLE = R.HANDLE_PROCESS WHERE P.STATE = 'Y' AND R.STATE = 'Y' AND P.HANDLE = '{handle}' ORDER BY HANDLE ASC ;", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    Hashtable hashField = new Hashtable();
                    while (reader.Read())
                    {
                        //if (string.IsNullOrEmpty(reader["PRIMARY_KEY"].ToString()))
                            hashField.Add(reader["FIELD"], reader["HANDLE"]);
                        //else
                        //    hashField.Add(reader["FIELD"] + "_" + reader["PRIMARY_KEY"].ToString(), reader["HANDLE"]);
                    }
                    return hashField;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 导入临时库
        /// </summary>
        /// <param name="listSeries"></param>
        /// <returns></returns>
        public static RESULT_WQY CheckIn(string type, DataTable dtSource)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::CheckIn => fail to open database.");
                    }
                    Hashtable primary = PrimaryKey(type);
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sql2 = new StringBuilder();
                    StringBuilder fields = new StringBuilder();
                    StringBuilder values = new StringBuilder();
                    Hashtable hashFields = new Hashtable();
                    hashFields = Fields(type);
                    Hashtable hashPrimary = PrimaryKey(type);
                    int count = 0;
                    foreach(DictionaryEntry entry in hashFields)
                    {
                        fields.Append(entry.Value.ToString());
                        fields.Append(",");
                    }
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = mConn;
                    for (int i = 0; i < dtSource.Rows.Count; ++i)
                    {
                        bool state = true;
                        sql.Clear();
                        sql.Append($@"INSERT INTO {type} ({fields.ToString().Substring(0, fields.Length - 1)}) VALUES (");
                        values.Clear();
                        foreach(DictionaryEntry entry in hashFields)
                        {
                            values.Append("'");
                            string value = dtSource.Rows[i][entry.Key.ToString()].ToString();
                            if (string.IsNullOrEmpty(value))
                            {
                                state = false;
                                break;
                            }
                            values.Append(value);
                            values.Append("',");
                        }
                        sql.Append(values.ToString().Substring(0, values.Length - 1));
                        sql.Append(");");
                        if (!state)
                        {
                            continue;
                        }
                        sql2.Append(sql);
                        //comm.CommandText = sql.ToString();
                        //comm.ExecuteNonQuery();
                    }
                    SqlTransaction tran = mConn.BeginTransaction();
                    comm.CommandText = sql2.ToString();
                    comm.Transaction = tran;
                    count = comm.ExecuteNonQuery();
                    tran.Commit();

                    // 判断是否有创建同步 sql 的必要
                    comm.CommandText = $"SELECT COUNT(1) FROM PROCESS P INNER JOIN RELATION R ON P.HANDLE = R.HANDLE_PROCESS WHERE P.STATE = 'Y' AND P.IS_UPLOAD = 'Y' AND R.STATE = 'Y' AND R.CATALOGS = 'PRD_CELL_LINK' AND P.INSTANCE = '{type}';";
                    if (Convert.ToInt32(comm.ExecuteScalar()) > 0)
                    {
                        //创建批次
                        count = CreateBatch(type, dtSource);
                        //创建 wip
                        count = CreateWIP(type, dtSource);
                    }

                    RESULT_WQY result = new RESULT_WQY();
                    result.TOTAL = count.ToString();


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
        /// 获取主键
        /// </summary>
        /// <returns></returns>
        public static Hashtable PrimaryKey(string type)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::PrimaryKey => fail to open database.");
                    }
                    Hashtable primary = new Hashtable();
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = mConn;
                    if (string.IsNullOrEmpty(type))
                        comm.CommandText = "SELECT PARAMETER ,FIELD FROM RELATION WHERE PRIMARY_KEY = 'Y';";
                    else
                        comm.CommandText = $"SELECT R.PARAMETER, R.FIELD FROM PROCESS P INNER JOIN RELATION R ON P.HANDLE = R.HANDLE_PROCESS WHERE PRIMARY_KEY = 'Y' AND INSTANCE = '{type}';";
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        primary.Add(reader["FIELD"].ToString(), reader["PARAMETER"]);
                    }
                    reader.Close();
                    return primary;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static int CreateSqlByLink(string type)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::CreateSQL => fail to open database.");
                    }
                    string sql = $"SELECT R.[PARAMETER], PRIMARY_KEY FROM PROCESS P INNER JOIN RELATION R ON P.HANDLE = R.HANDLE_PROCESS WHERE P.STATE = 'Y' AND P.IS_UPLOAD = 'Y' AND R.STATE = 'Y' AND R.CATALOGS = 'PRD_CELL_LINK' AND P.INSTANCE = '{type}';";
                    SqlCommand comm = new SqlCommand(sql, mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    // 该工序不需要上传至 Oracle，仅留存本地即可
                    if (!reader.HasRows) return 0;
                    StringBuilder sbStr = new StringBuilder();
                    //StringBuilder sbStr2 = new StringBuilder();
                    string primary_key = null;
                    Hashtable hashParameter = new Hashtable();
                    Hashtable hashDate = IsDate();
                    Hashtable hashSort = GetSort();
                    Hashtable hashSync = IsSync(type);
                    while (reader.Read())
                    {
                        if (!string.IsNullOrEmpty(reader["PRIMARY_KEY"].ToString()))
                        {
                            primary_key = reader["PARAMETER"].ToString();
                        }

                        if (!hashSync.ContainsKey(reader["PARAMETER"].ToString()))
                            continue;
                        else
                            hashParameter.Add(reader["PARAMETER"], reader["PARAMETER"]);
                        if (hashSort.ContainsKey(type))
                            sbStr.Append($"I.{reader["PARAMETER"].ToString()}");
                        else
                            sbStr.Append($"{reader["PARAMETER"].ToString()}");
                        sbStr.Append(",");
                        //sbStr2.Append($"{reader["PARAMETER"].ToString()} AS {reader["PARAMETER"].ToString()},");
                    }
                    reader.Close();
                    if (hashSort.ContainsKey(type))
                        sql = $@"SELECT DISTINCT {sbStr.ToString().Substring(0, sbStr.Length - 1)}
                                    FROM {type} I
                                    INNER JOIN (
	                                    SELECT {primary_key}
		                                    ,MAX({hashSort[type]}) {hashSort[type]}
	                                    FROM {type}
	                                    WHERE STATE IS NULL
	                                    GROUP BY {primary_key},{hashSort[type]}
	                                    HAVING COUNT(*) >= 1
	                                    ) A ON I.{primary_key} = A.{primary_key}
	                                    AND I.{hashSort[type]} = A.{hashSort[type]}
                                    WHERE I.STATE IS NULL;";
                    else
                        sql = $"SELECT {sbStr.ToString().Substring(0, sbStr.Length - 1)} FROM {type} WHERE STATE IS NULL;";
                    comm.CommandText = sql;
                    reader = comm.ExecuteReader();
                    StringBuilder sbBarcodes = new StringBuilder();
                    if (string.IsNullOrEmpty(primary_key)) throw new Exception("no primary key.");
                    //string paraList = sbStr.ToString().Substring(0, sbStr.Length - 1);
                    sbStr.Clear();
                    StringBuilder sbSql = new StringBuilder();
                    int count = 0;
                    comm.CommandText = null;
                    List<string> listBarcodes = new List<string>();
                    while (reader.Read())
                    {
                        StringBuilder sbTemp = new StringBuilder();
                        StringBuilder sbValues = new StringBuilder();
                        StringBuilder sbStr2 = new StringBuilder();
                        StringBuilder paraList = new StringBuilder();
                        foreach(DictionaryEntry entry in hashParameter)
                        {
                            if (entry.Key.ToString() == primary_key)
                            {
                                listBarcodes.Add(reader[entry.Key.ToString()].ToString());
                                continue;
                            }
                            paraList.Append($"{entry.Key.ToString()},");
                            if (hashDate.ContainsKey(entry.Key))
                            {
                                sbTemp.Append($"dest.{entry.Key.ToString()} = to_date(''{reader[entry.Key.ToString()].ToString()}'', ''YYYY-MM-DD HH24:MI:SS''),");
                                sbValues.Append($"to_date(''{reader[entry.Key.ToString()].ToString()}'', ''YYYY-MM-DD HH24:MI:SS''),");
                            }
                            else
                            {
                                sbTemp.Append($"dest.{entry.Key.ToString()} = ''{reader[entry.Key.ToString()].ToString()}'',");
                                sbValues.Append($"''{reader[entry.Key.ToString()].ToString()}'',");
                            }
                            sbStr2.Append($"''{reader[entry.Key.ToString()].ToString()}'' AS {entry.Key.ToString().ToString()},");
                        }
                        sbBarcodes.Append($"'{reader[primary_key].ToString()}',");
                        Guid newGuid = Guid.NewGuid();
                        sbStr.Append($@"INSERT INTO SEND_INFO (CATALOG,CONTEXTS) VALUES ('{type}','MERGE INTO PRD_CELL_LINK dest
                                        USING (
	                                        SELECT ''{reader[primary_key].ToString()}'' AS lot_no, ''{newGuid.ToString()}'' AS NID, {sbStr2.ToString().Substring(0,sbStr2.Length-1)}
	                                        FROM dual
	                                        ) src
	                                        ON (dest.LOT_NO = src.lot_no)
                                        WHEN MATCHED
	                                        THEN
		                                        UPDATE
		                                        SET {sbTemp.ToString().Substring(0,sbTemp.Length - 1)}
                                        WHEN NOT MATCHED
	                                        THEN
		                                        INSERT (
			                                        NID
			                                        ,LOT_NO
			                                        ,{paraList.ToString().Substring(0,paraList.Length-1)}
			                                        )
		                                        VALUES (
			                                        ''{newGuid.ToString()}''
			                                        ,src.lot_no
			                                        ,{sbValues.ToString().Substring(0, sbValues.Length-1)}
			                                        );')");
                        //sbSql.Append($"INSERT INTO SEND_INFO (CATALOG,CONTEXTS) VALUES {sbStr.ToString()}");
                        //comm.CommandText += sbStr.ToString();
                        //sbStr.Clear();
                    }
                    reader.Close();
                    SqlTransaction tran = mConn.BeginTransaction();
                    comm.Transaction = tran;
                    comm.CommandText = sbStr.ToString();
                    count = comm.ExecuteNonQuery();
                    tran.Commit();
                    int count2 = UpdateStateByBarcode(type, primary_key, listBarcodes);
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 获取指定表中文件字段与表字段对应关系
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public static Hashtable Fields(string table)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::Fields => fail to open database.");
                    }
                    Hashtable hashFields = new Hashtable();
                    //SqlCommand comm = new SqlCommand($"SELECT C.NAME,P.VALUE FROM SYS.EXTENDED_PROPERTIES P ,SYS.COLUMNS C WHERE P.MAJOR_ID=OBJECT_ID('{table}') AND P.MAJOR_ID=C.OBJECT_ID AND P.MINOR_ID=C.COLUMN_ID;", mConn);
                    SqlCommand comm = new SqlCommand($"SELECT DISTINCT NAME,VALUE FROM (SELECT C.NAME,P.VALUE FROM SYS.EXTENDED_PROPERTIES P ,SYS.COLUMNS C WHERE P.MAJOR_ID=OBJECT_ID('{table}') AND P.MAJOR_ID=C.OBJECT_ID AND P.MINOR_ID=C.COLUMN_ID) A INNER JOIN RELATION R ON A.NAME = R.[PARAMETER] AND R.STATE = 'Y' AND R.CATALOGS = 'PRD_CELL_LINK';", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        hashFields.Add(reader["VALUE"].ToString(), reader["NAME"].ToString());
                    }
                    reader.Close();
                    return hashFields;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static Hashtable GetInstance()
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::Instance => fail to open database.");
                    }
                    Hashtable hashInstance = new Hashtable();
                    SqlCommand comm = new SqlCommand($"SELECT PROCESS ,INSTANCE FROM PROCESS;", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        hashInstance.Add(reader["PROCESS"].ToString(), reader["INSTANCE"].ToString());
                    }
                    reader.Close();
                    return hashInstance;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static Hashtable IsDate()
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::IsDate => fail to open database.");
                    }
                    Hashtable hashDate = new Hashtable();
                    SqlCommand comm = new SqlCommand($"SELECT DISTINCT PARAMETER, FIELD FROM RELATION WHERE IS_DATE = 'Y' AND STATE = 'Y';", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        hashDate.Add(reader["PARAMETER"].ToString(), reader["FIELD"].ToString());
                    }
                    reader.Close();
                    return hashDate;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 更新指定数据表 STATE
        /// </summary>
        /// <param name="type">数据表</param>
        /// <param name="primary_key">码号所在字段</param>
        /// <param name="listBarcode">码号清单</param>
        /// <returns></returns>
        public static int UpdateStateByBarcode(string type, string primary_key, List<string> listBarcode)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::UpdateState => fail to open database.");
                    }
                    StringBuilder sql = new StringBuilder();
                    for(int i = 0; i < listBarcode.Count; ++i)
                    {
                        sql.Append($"'{listBarcode[i]}',");
                    }
                    SqlCommand comm = new SqlCommand($"UPDATE {type} SET STATE = 'Y' WHERE {primary_key} IN ({sql.ToString().Substring(0, sql.Length - 1)});", mConn);
                    int count = comm.ExecuteNonQuery();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 更新指定数据表 STATE   
        /// </summary>
        /// <param name="type">数据表</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int UpdateStateByID(string type, int id)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::UpdateStateByID => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand($"UPDATE {type} SET STATE = 'Y' WHERE ID = {id};", mConn);
                    int count = comm.ExecuteNonQuery();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static int UpdateStateByIDs(string type,List<string> listID)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::UpdateStateByIDs => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = mConn;
                    StringBuilder sbSQL = new StringBuilder();
                    for(int i = 0; i < listID.Count; ++i)
                    {
                        sbSQL.Append($"UPDATE {type} SET STATE = 'Y' WHERE ID = {listID[i]};");
                    }
                    SqlTransaction tran = mConn.BeginTransaction();
                    comm.Transaction = tran;
                    comm.CommandText = sbSQL.ToString();
                    int count = comm.ExecuteNonQuery();
                    tran.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static int SyncOracleByLink()
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::SyncOracleByLink => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT ID, CONTEXTS FROM SEND_INFO WHERE STATE IS NULL ;", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    int count = 0;
                    while (reader.Read())
                    {

                        if (SyncLink(reader["CONTEXTS"].ToString()) == 1)
                        {
                            if (UpdateStateByID("SEND_INFO", Convert.ToInt32(reader["ID"].ToString())) == 1)
                            {
                                ++count;
                            }
                        }
                    }
                    reader.Close();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static int SyncOracleByBatch()
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::SyncOracleByLink => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT ID, CONTEXTS FROM BATCH WHERE STATE IS NULL ;", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    int count = 0;
                    while (reader.Read())
                    {
                        if (SyncBatch(reader["CONTEXTS"].ToString()) > 0)
                        {
                            if (UpdateStateByID("BATCH", Convert.ToInt32(reader["ID"].ToString())) == 1)
                            {
                                ++count;
                            }
                        }
                    }
                    reader.Close();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static int SyncOracleByWip()
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::SyncOracleByWip => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT ID, CONTEXTS FROM WIP WHERE STATE IS NULL ;", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    int count = 0;
                    List<string> listID = new List<string>();
                    while (reader.Read())
                    {
                        if (SyncWip(reader["CONTEXTS"].ToString()) > 0)
                        {
                            listID.Add(reader["ID"].ToString());
                            //if (UpdateStateByID("WIP", Convert.ToInt32(reader["ID"].ToString())) == 1)
                            //{
                                ++count;
                            //}
                        }
                    }
                    reader.Close();
                    count = UpdateStateByIDs("WIP", listID);
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static int SyncLink(string contexts)
        {
            using (OracleConnection mConn = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORACLE_WY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::SyncLink => fail to open database.");
                    }

                    OracleCommand comm = new OracleCommand($"{contexts.Substring(0, contexts.Length - 1)}", mConn);
                    int count = comm.ExecuteNonQuery();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static int SyncBatch(string contexts)
        {
            using (OracleConnection mConn = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORACLE_WY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::SyncBatch => fail to open database.");
                    }

                    OracleCommand comm = new OracleCommand($"{contexts.Substring(0, contexts.Length - 1)}", mConn);
                    int count = comm.ExecuteNonQuery();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static int SyncWip(string contexts)
        {
            using (OracleConnection mConn = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORACLE_WY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::SyncWip => fail to open database.");
                    }

                    OracleCommand comm = new OracleCommand($"{contexts.Substring(0, contexts.Length - 1)}", mConn);
                    int count = comm.ExecuteNonQuery();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static Hashtable GetBom()
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    Hashtable hashBom = new Hashtable();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetBom => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT P.PROCESS,R.PARAMETER FROM PROCESS P INNER JOIN RELATION R ON P.HANDLE = R.HANDLE_PROCESS WHERE IS_SORT = 'Y' GROUP BY P.PROCESS,R.IS_SORT,R.PARAMETER;", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        hashBom.Add(reader["PROCESS"].ToString(),reader["PARAMETER"].ToString());
                    }
                    reader.Close();
                    return hashBom;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static Hashtable GetSort()
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    Hashtable hashSort = new Hashtable();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::GetSort => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT P.PROCESS,R.PARAMETER FROM PROCESS P INNER JOIN RELATION R ON P.HANDLE = R.HANDLE_PROCESS WHERE IS_SORT = 'Y' GROUP BY P.PROCESS,R.IS_SORT,R.PARAMETER;", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        hashSort.Add(reader["PROCESS"].ToString(), reader["PARAMETER"].ToString());
                    }
                    reader.Close();
                    return hashSort;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static int CreateBatch(string type, DataTable dtData)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    Hashtable hashSort = new Hashtable();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::CreateBatch => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT R.PARAMETER FROM PROCESS P INNER JOIN RELATION R ON P.HANDLE = R.HANDLE_PROCESS WHERE P.INSTANCE = '{type}' AND R.PRIMARY_KEY = 'Y';", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    reader.Read();
                    string parameter = reader["PARAMETER"].ToString();
                    Hashtable hashField = Fields(type);
                    string field = null;
                    foreach(DictionaryEntry entry in hashField)
                    {
                        if (entry.Value.ToString() == parameter) field = entry.Key.ToString();
                    }
                    reader.Close();

                    StringBuilder sbBarcodes = new StringBuilder();
                    StringBuilder sbSql = new StringBuilder();

                    for (int i = 0; i < dtData.Rows.Count; ++i)
                    {
                        sbBarcodes.Append($"'{dtData.Rows[i][field].ToString()}',");
                    }
                    string flag = null;
                    if (type == "FD")
                        flag = "Y";
                    else
                        flag = "N";
                    comm.CommandText = $@"SELECT BO.BOMNO + A.BATCH BATCH_NO
	                                        ,A.BATCH
	                                        ,BO.ITEM_NO
	                                        ,BO.ORDER_NO
                                        FROM (
	                                        SELECT LEFT({parameter}, 3) AS SHORT_BOM
		                                        ,RIGHT(LEFT({parameter}, 10), 7) AS BATCH
	                                        FROM {type}
                                            WHERE STATE IS NULL AND {parameter} IN ({sbBarcodes.ToString().Substring(0, sbBarcodes.Length-1)})
	                                        GROUP BY LEFT({parameter}, 3)
		                                        ,RIGHT(LEFT({parameter}, 10), 7)
	                                        ) A
                                        INNER JOIN BOM_ORDER BO ON A.SHORT_BOM = BO.SHORT_BOM
	                                        AND STATE = 'Y'
	                                        AND IS_GRADE = '{flag}';
                                        ";
                    List<Batch> listResult = new List<Batch>();
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        Batch result = new Batch();
                        result.BATCH = reader["BATCH"].ToString();
                        result.BATCH_NO = reader["BATCH_NO"].ToString();
                        result.ITEM_NO = reader["ITEM_NO"].ToString();
                        result.ORDER_NO = reader["ORDER_NO"].ToString();
                        listResult.Add(result);
                    }
                    reader.Close();
                    for (int i = 0; i < listResult.Count; ++i)
                    {
                        Guid newGuid = Guid.NewGuid();
                        string context = $@"MERGE INTO PRD_CELL_BATCH dest
                                    USING (
	                                    SELECT ''{listResult[i].BATCH}'' AS BATCH
		                                    ,''{listResult[i].BATCH_NO}'' AS BATCH_NO
		                                    ,''{listResult[i].ITEM_NO}'' AS MATERIAL_CODE
		                                    ,''{listResult[i].ORDER_NO}'' AS ORDER_NO
	                                    FROM dual
	                                    ) src
	                                    ON (dest.BATCH = src.BATCH AND dest.MATERIAL_CODE = src.MATERIAL_CODE)
                                    WHEN MATCHED
	                                    THEN
		                                    UPDATE
		                                    SET dest.ORDER_NO = ''{listResult[i].ORDER_NO}'',dest.CREATE_BY = ''UplodTools''
                                    WHEN NOT MATCHED
	                                    THEN
		                                    INSERT (
                                                NID
			                                    ,BATCH
			                                    ,BATCH_NO
			                                    ,MATERIAL_CODE
			                                    ,ORDER_NO
                                                ,CREATE_BY
			                                    )
		                                    VALUES (
  			                                    ''{newGuid.ToString()}''
			                                    ,''{listResult[i].BATCH}''
			                                    ,''{listResult[i].BATCH_NO}''
			                                    ,''{listResult[i].ITEM_NO}''
			                                    ,''{listResult[i].ORDER_NO}''
                                                ,''UplodTools''
			                                    );";
                        sbSql.Append($"INSERT INTO BATCH (CONTEXTS) VALUES ('{context}');");
                    }
                    comm.CommandText = sbSql.ToString();
                    SqlTransaction tran = mConn.BeginTransaction();
                    comm.Transaction = tran;
                    int count = comm.ExecuteNonQuery();
                    tran.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static int CreateWIP(string type, DataTable dtData)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    Hashtable hashSort = new Hashtable();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::CreateWIP => fail to open database.");
                    }
                    StringBuilder sql = new StringBuilder();

                    SqlCommand comm = new SqlCommand($"SELECT R.PARAMETER FROM PROCESS P INNER JOIN RELATION R ON P.HANDLE = R.HANDLE_PROCESS WHERE P.INSTANCE = '{type}' AND R.PRIMARY_KEY = 'Y';", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    reader.Read();
                    string parameter = reader["PARAMETER"].ToString();
                    Hashtable hashField = Fields(type);
                    string field = null;
                    foreach (DictionaryEntry entry in hashField)
                    {
                        if (entry.Value.ToString() == parameter) field = entry.Key.ToString();
                    }
                    reader.Close();

                    StringBuilder sbBarcodes = new StringBuilder();
                    StringBuilder sbSql = new StringBuilder();

                    for (int i = 0; i < dtData.Rows.Count; ++i)
                    {
                        string flag = null;
                        if (type == "FD")
                            flag = "Y";
                        else
                            flag = "N";
                        comm.CommandText = $@"SELECT BO.BOMNO + A.BATCH BATCH_NO
	                                        ,A.BATCH
	                                        ,BO.ITEM_NO
	                                        ,BO.ORDER_NO
                                        FROM (
	                                        SELECT LEFT({parameter}, 3) AS SHORT_BOM
		                                        ,RIGHT(LEFT({parameter}, 10), 7) AS BATCH
	                                        FROM {type}
                                            WHERE STATE IS NULL AND {parameter} IN ('{dtData.Rows[i][field].ToString()}')
	                                        GROUP BY LEFT({parameter}, 3)
		                                        ,RIGHT(LEFT({parameter}, 10), 7)
	                                        ) A
                                        INNER JOIN BOM_ORDER BO ON A.SHORT_BOM = BO.SHORT_BOM
	                                        AND STATE = 'Y'
	                                        AND IS_GRADE = '{flag}';
                                        ";
                        List<Batch> listResult = new List<Batch>();
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            Guid newGuid = Guid.NewGuid();
                            string context = $@"MERGE INTO PRD_WIP_CELL dest
                                        USING (
	                                        SELECT ''{reader["ITEM_NO"].ToString()}'' AS MATERIAL_CODE
		                                        ,''{reader["ORDER_NO"].ToString()}'' AS ORDER_NO
		                                        ,''{reader["BATCH_NO"].ToString()}'' AS BATCH
		                                        ,'''' AS GRADE
		                                        ,'''' AS GRADE_BATCH
                                                ,''{dtData.Rows[i][field].ToString()}'' AS LOT_NO
	                                        FROM dual
	                                        ) src
	                                        ON (dest.LOT_NO = src.LOT_NO)
                                        WHEN MATCHED
	                                        THEN
		                                        UPDATE
		                                        SET dest.ORDER_NO = ''{reader["ORDER_NO"].ToString()}''
			                                        ,dest.MATERIAL_CODE = ''{reader["ITEM_NO"].ToString()}''
			                                        ,dest.BATCH = ''{reader["BATCH_NO"].ToString()}''
                                                    ,dest.CREATE_BY = ''UplodTools''
                                        WHEN NOT MATCHED
	                                        THEN
		                                        INSERT (
                                                    NID
                                                    ,LOT_NO
			                                        ,MATERIAL_CODE
			                                        ,ORDER_NO
			                                        ,BATCH
			                                        ,GRADE
			                                        ,GRADE_BATCH
			                                        ,CREATE_BY
                                                    ,IS_OK
			                                        )
		                                        VALUES (
  			                                        ''{newGuid.ToString()}''
                                                    ,''{dtData.Rows[i][field].ToString()}''
			                                        ,''{reader["ITEM_NO"].ToString()}''
			                                        ,''{reader["ORDER_NO"].ToString()}''
			                                        ,''{reader["BATCH_NO"].ToString()}''
			                                        ,''''
			                                        ,''''
			                                        ,''UplodTools''
                                                    ,''1''
			                                        );";
                            sbSql.Append($"INSERT INTO WIP(CONTEXTS) VALUES('{context}');");
                        }
                        reader.Close();

                    }
                    comm.CommandText = sbSql.ToString();
                    SqlTransaction tran = mConn.BeginTransaction();
                    comm.Transaction = tran;
                    int count = comm.ExecuteNonQuery();
                    tran.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        //public static int SyncBatch(Batch batch)
        //{
        //    using (OracleConnection mConn = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORACLE_WY_TEST"].ConnectionString))
        //    {
        //        mConn.Open();
        //        try
        //        {
        //            if (mConn.State != ConnectionState.Open)
        //            {
        //                throw new Exception("Datum::CreateBatch => fail to open database.");
        //            }

        //            OracleCommand comm = new OracleCommand($@"MERGE INTO PRD_CELL_BATCH dest
        //                                                        USING (
	       //                                                         SELECT '{batch.BATCH}' AS BATCH
		      //                                                          ,'{batch.BATCH_NO}' AS BATCH_NO
		      //                                                          ,'{batch.ITEM_NO}' AS MATERIAL_CODE
		      //                                                          ,'{batch.ORDER_NO}' AS ORDER_NO
	       //                                                         FROM dual
	       //                                                         ) src
	       //                                                         ON (dest.BATCH = src.BATCH)
        //                                                        WHEN NOT MATCHED
	       //                                                         THEN
		      //                                                          INSERT (
			     //                                                           BATCH
			     //                                                           ,BATCH_NO
			     //                                                           ,OCV1TP012
			     //                                                           ,ORDER_NO
			     //                                                           )
		      //                                                          VALUES (
			     //                                                           '{batch.BATCH}'
			     //                                                           ,'{batch.BATCH_NO}'
			     //                                                           ,'{batch.ITEM_NO}'
			     //                                                           ,'{batch.ORDER_NO}'
			     //                                                           );", mConn);
        //            int count = comm.ExecuteNonQuery();
        //            return count;
        //        }
        //        catch (Exception ex)
        //        {
        //            SysLog log = new SysLog(ex.Message);
        //            return 0;
        //        }
        //    }
        //}
        //public static int SyncWIP(WIP wip)
        //{
        //    using (OracleConnection mConn = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORACLE_WY_TEST"].ConnectionString))
        //    {
        //        mConn.Open();
        //        try
        //        {
        //            if (mConn.State != ConnectionState.Open)
        //            {
        //                throw new Exception("Datum::CreateWIP => fail to open database.");
        //            }
        //            OracleCommand comm = new OracleCommand($@"MERGE INTO PRD_WIP_CELL dest
        //                                                        USING (
	       //                                                         SELECT '{wip.MATERIAL_CODE}' AS MATERIAL_CODE
		      //                                                          ,'{wip.ORDER_NO}' AS ORDER_NO
		      //                                                          ,'{wip.BATCH}' AS BATCH
		      //                                                          ,'{wip.GRADE}' AS GRADE
		      //                                                          ,'{wip.GRADE_BATCH}' AS GRADE_BATCH
	       //                                                         FROM dual
	       //                                                         ) src
	       //                                                         ON (dest.MATERIAL_CODE = src.MATERIAL_CODE AND dest.ORDER_NO = src.ORDER_NO AND dest.BATCH = src.BATCH)
        //                                                        WHEN NOT MATCHED
	       //                                                         THEN
		      //                                                          INSERT (
			     //                                                           MATERIAL_CODE
			     //                                                           ,ORDER_NO
			     //                                                           ,BATCH
			     //                                                           ,GRADE
			     //                                                           ,GRADE_BATCH
			     //                                                           )
		      //                                                          VALUES (
			     //                                                           '{wip.MATERIAL_CODE}'
			     //                                                           ,'{wip.ORDER_NO}'
			     //                                                           ,'{wip.BATCH}'
			     //                                                           ,'{wip.GRADE}'
			     //                                                           ,'{wip.GRADE_BATCH}'
			     //                                                           );", mConn);
        //            int count = comm.ExecuteNonQuery();
        //            return count;
        //        }
        //        catch (Exception ex)
        //        {
        //            SysLog log = new SysLog(ex.Message);
        //            return 0;
        //        }
        //    }
        //}
        /// <summary>
        /// 计算净液量
        /// </summary>
        /// <returns></returns>
        public static int Calculate()
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    Hashtable hashSort = new Hashtable();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::Calculate => fail to open database.");
                    }
                    string sql = $@"SELECT DEGATP023
                                        , DEGATP002
                                        , PARAMETER_FIXED
                                        , INJETP025
                                        ,(DEGATP002 + PARAMETER_FIXED - INJETP025) AS DEGATP004
                                    FROM DEGAS D
                                    INNER JOIN ZY Z ON D.STATE IS NULL
                                        AND D.DEGATP023 = Z.INJETP036
                                    INNER JOIN BOM_ORDER BO ON LEFT(D.DEGATP023, 3) = BO.SHORT_BOM;";
                    SqlCommand comm = new SqlCommand(sql, mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    StringBuilder sbSql = new StringBuilder();
                    while (reader.Read())
                    {
                        sbSql.Append($"UPDATE DEGAS SET DEGATP004 = '{reader["DEGATP004"].ToString()}' WHERE DEGATP023 = '{reader["DEGATP023"].ToString()}';");
                    }
                    reader.Close();
                    SqlTransaction tran = mConn.BeginTransaction();
                    comm.Transaction = tran;
                    comm.CommandTimeout = 300;
                    comm.CommandText = sbSql.ToString();
                    int count = comm.ExecuteNonQuery();
                    tran.Commit();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static Hashtable IsSync(string type)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WQY_TEST"].ConnectionString))
            {
                mConn.Open();
                try
                {
                    Hashtable hashSort = new Hashtable();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("Datum::IsSync => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT R.PARAMETER FROM PROCESS P INNER JOIN RELATION R ON P.HANDLE = R.HANDLE_PROCESS WHERE IS_SYNC = 'Y' AND P.INSTANCE = '{type}';", mConn);
                    SqlDataReader reader = comm.ExecuteReader();
                    Hashtable hashSync = new Hashtable();
                    while (reader.Read())
                    {
                        hashSync.Add(reader["PARAMETER"].ToString(), reader["PARAMETER"].ToString());
                    }
                    return hashSync;
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