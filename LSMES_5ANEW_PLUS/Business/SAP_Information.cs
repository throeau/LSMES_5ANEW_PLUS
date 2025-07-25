using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Odbc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Collections;
using LSMES_5ANEW_PLUS.App_Base;
using System.Xml;
using System.Configuration;

namespace LSMES_5ANEW_PLUS.Business
{
    public class SAP_Information
    {
        public WorkCenter GetWorkCenter()
        {
            string sql = "SELECT WORK_CENTER ,DESCRIPTION FROM WORK_CENTER WHERE DESCRIPTION LIKE 'CE四期蓝牙PACK手动%线' ORDER BY DESCRIPTION ASC;";
            try
            {
                WorkCenter wc = new WorkCenter();
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
                {
                    conn.Open();
                    OdbcDataReader reader;
                    OdbcCommand mComm = new OdbcCommand(sql, conn);
                    reader = mComm.ExecuteReader();
                    while (reader.Read())
                    {
                        EntityWorkCenter entity = new EntityWorkCenter();
                        entity.WORK_CENTER = reader["WORK_CENTER"].ToString();
                        entity.DESCRIPTION = reader["DESCRIPTION"].ToString();
                        wc.WORK_CENTER_LIST.Add(entity);
                    }
                }
                return wc;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("Exception:" + ex.Message + " [Class:SAP_Information,Function:GetWorkCenter()],Sql statement:" + sql);
                return null;
            }
        }
        public Resource GetResource()
        {
            string sql = "SELECT RESRCE,DESCRIPTION FROM RESRCE WHERE DESCRIPTION LIKE '%PACK手动线性能测试%' ORDER BY RESRCE ASC;";
            try
            {
                Resource rs = new Resource();
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
                {
                    conn.Open();
                    OdbcDataReader reader;
                    OdbcCommand mComm = new OdbcCommand(sql, conn);
                    reader = mComm.ExecuteReader();
                    while (reader.Read())
                    {
                        EntityResource entity = new EntityResource();
                        entity.RESOURCE = reader["RESRCE"].ToString();
                        entity.DESCRIPTION = reader["DESCRIPTION"].ToString();
                        rs.Resource_LIST.Add(entity);
                    }
                }
                return rs;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("Exception:" + ex.Message + " [Class:SAP_Information,Function:GetResource()],Sql statement:" + sql);
                return null;
            }

        }

        public static HW_Recieved ParsingPerformanceData(string json)
        {
            try
            {
                HW_Recieved info = JsonConvert.DeserializeObject<HW_Recieved>(json);
                return info;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        public static DataTable GetHWPerformanceDataBySAP()
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    comm.CommandText = "SELECT * FROM HW_RECIEVE_LOG WHERE STATE = '0'";
                    SqlDataReader reader = comm.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    reader.Close();
                    conn.Close();
                    return dt;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
            return null;
        }
        public static bool UpdateHWPerformanceDataBySAP(string id)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    comm.CommandText = string.Format("UPDATE HW_RECIEVE_LOG SET STATE = '1' WHERE ID = '{0}'", id);
                    if (comm.ExecuteNonQuery() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("Failed to update record.");
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return false;
                }
            }
        }
        public static int SetHWPerformanceDataBySAP(DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    int i = 0;
                    SqlTransaction tran;
                    SqlCommand comm = new SqlCommand();
                    tran = conn.BeginTransaction();
                    comm.Connection = conn;
                    string sql = null;
                    for (; i < dt.Rows.Count; ++i)
                    {
                        sql = string.Format("UPDATE HW_RECIEVE_LOG SET STATE = '1' WHERE ID = {0};", dt.Rows[i]["ID"].ToString().Trim());
                        comm.CommandText = sql;
                        comm.ExecuteNonQuery();
                    }
                    tran.Commit();
                    conn.Close();
                    return i;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static int PrepareData()
        {
            try
            {
                DataTable dt = new DataTable();
                dt = GetHWPerformanceDataBySAP();
                HW_Recieved entity = new HW_Recieved();
                foreach (DataRow row in dt.Rows)
                {
                    entity = ParsingPerformanceData(row["RECIEVE_INFO"].ToString());
                    if (PrepareBox(entity) == 1)
                    {
                        if (PrepareBattery(entity) != entity.SN_LIST.Count)
                        {
                            throw new Exception(string.Format("Exception，PrepareData：PrepareBattery is not {0}.", entity.SN_LIST.Count));
                        }
                        else
                        {
                            UpdateHWPerformanceDataBySAP(row["ID"].ToString());
                        }
                    }
                    else
                    {
                        throw new Exception("Exception，PrepareData：PrepareBox is not 1.");
                    }
                }
                return entity.SN_LIST.Count;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        public static int PrepareBox(HW_Recieved entity)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    int count = 0;
                    if (entity == null)
                    {
                        throw new Exception("Exception，PrepareBox：HW_Recieved is empty.");
                    }
                    else
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand();
                        comm.Connection = conn;
                        // 固定工厂代码为：Z0001K_001
                        comm.CommandText = string.Format("INSERT INTO HW_RECIEVE_BOX (REQ_ID,SITE,ITEM,ITEM_TYPE,CUSTOMER_NAME,CUSTOMER_ITEM,FACTORY_CODE,SUPPLIES_NO,BOX_NO,SEND_TIME,RECIEVED_DATE_TIME) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',GETDATE());", entity.REQ_ID, entity.SITE, entity.ITEM, entity.ITEM_TYPE, entity.CUSTOMER_NAME, entity.CUSTOMER_ITEM, "Z0001K_001", entity.SUPPLIES_NO, entity.BOX_NO, entity.SEND_TIME);
                        // 工厂代码与供应商代码相同，Z0001K
                        //comm.CommandText = string.Format("INSERT INTO HW_RECIEVE_BOX (REQ_ID,SITE,ITEM,ITEM_TYPE,CUSTOMER_NAME,CUSTOMER_ITEM,FACTORY_CODE,SUPPLIES_NO,BOX_NO,SEND_TIME,RECIEVED_DATE_TIME) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',GETDATE());", entity.REQ_ID, entity.SITE, entity.ITEM, entity.ITEM_TYPE, entity.CUSTOMER_NAME, entity.CUSTOMER_ITEM, entity.FACTORY_CODE, entity.SUPPLIES_NO, entity.BOX_NO, entity.SEND_TIME);
                        count = comm.ExecuteNonQuery();
                        conn.Close();
                    }
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public static int PrepareBattery(HW_Recieved entity)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    int count = 0;
                    if (entity == null)
                    {
                        throw new Exception("Exception，PrepareBattery：HW_Recieved is empty.");
                    }
                    else
                    {
                        conn.Open();
                        SqlTransaction tran;
                        SqlCommand comm = new SqlCommand();
                        tran = conn.BeginTransaction();
                        comm.Connection = conn;
                        comm.Transaction = tran;
                        foreach (Battery bat in entity.SN_LIST)
                        {
                            foreach (BatteryParameter pam in bat.DC_INFO)
                            {
                                comm.CommandText = string.Format("INSERT INTO HW_RECIEVE_BATTERY (REF_BOX_NO,SN,PRODUCT_LINE,ORDER_NO,OPERATION,CUSTOMER_OPERATION,PARAM_NO,PARAM_VALUE,LS_PARAM_DESC,CUSTOMER_PARAM_DESC,LS_LOWER_LIMIT,LS_UPPER_LIMIT,CUSTOMER_LOWER_LIMIT,CUSTOMER_UPPER_LIMIT,UNIT,TEST_START_TIME,TEST_END_TIME,TEST_EQUIP,TEST_RESULT) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}');", entity.BOX_NO, bat.SN, bat.PRODUCT_LINE, bat.ORDER_NO, pam.OPERATION, pam.CUSTOMER_OPERATION, pam.PARAM_NO, pam.PARAM_VALUE, pam.LS_PARAM_DESC, pam.CUSTOMER_PARAM_DESC, pam.LS_LOWER_LIMIT, pam.LS_UPPER_LIMIT, pam.CUSTOMER_LOWER_LIMIT, pam.CUSTOMER_UPPER_LIMIT, pam.UNIT, pam.TEST_START_TIME, pam.TEST_END_TIME, pam.TEST_EQUIP, pam.TEST_RESULT);
                                comm.ExecuteNonQuery();
                            }
                            ++count;
                        }
                        tran.Commit();
                        conn.Close();
                    }
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
        /// 获取指定料号在制情况（五期甲一 SAP ME）
        /// </summary>
        /// <param name="item">物料编号</param>
        /// <param name="dt">日期</param>
        /// <returns></returns>
        public static DataTable GetWipBwySapMeWj1(string item, string dt)
        {
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_ME_WJ1"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        string sql = string.Format("SELECT TO_CHAR(OQ.DATE_TIME,'YYYY-MM-DD') DATE_TIME,O.OPERATION, O.DESCRIPTION OP_DESC, OQ.ITEM_NO,I.DESCRIPTION ITEM_DESC, SUM(QUEUE_QTY) QUEUE_QTY, SUM(WORK_QTY) WORK_QTY,SUM(QUEUE_QTY)+SUM(WORK_QTY) QTY FROM Z_OPERATION_QTY OQ INNER JOIN OPERATION O ON O.SITE = OQ.SITE AND O.OPERATION = OQ.OPERATION_NO AND O.CURRENT_REVISION = 'true' INNER JOIN ITEM I ON I.SITE = OQ.SITE AND I.ITEM = OQ.ITEM_NO WHERE OQ.SITE = '{0}' AND OQ.DATE_TIME = '{1}' AND (N('{2}') IS NULL OR I.ITEM = '{2}') GROUP BY OQ.DATE_TIME,O.OPERATION,O.DESCRIPTION,OQ.ITEM_NO,I.DESCRIPTION;", "1000", dt, item);
                        OdbcCommand comm = new OdbcCommand(sql, conn);
                        OdbcDataReader reader = comm.ExecuteReader();
                        DataTable dts = new DataTable();
                        dts.Load(reader);
                        reader.Close();
                        return dts;
                    }
                    else
                    {
                        throw new Exception("Hana Database can not be opened.");
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取SAP ME 资源信息
        /// </summary>
        /// <returns></returns>
        public static DataTable GetResourceBySapMeWj1()
        {
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_ME_WJ1"].ConnectionString))
            {
                try
                {
                    DataTable dts = new DataTable();

                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("HANA Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand("SELECT O.OPERATION ,O.DESCRIPTION ,R.RESRCE FROM OPERATION O INNER JOIN RESOURCE_TYPE RT ON O.RESOURCE_TYPE_BO = RT.HANDLE INNER JOIN RESOURCE_TYPE_RESOURCE RTR ON RTR.RESOURCE_TYPE_BO = RT.HANDLE INNER JOIN RESRCE R ON RTR.RESOURCE_BO = R.HANDLE;", conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    dts.Load(reader);
                    reader.Close();
                    if (dts.Rows.Count == 0)
                    {
                        throw new Exception("Failed to get SAP ME Resrce info.");
                    }
                    return dts;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 初始化设备与工序对应关系
        /// </summary>
        /// <param name="dt">设备信息</param>
        /// <returns></returns>
        public static bool InitEquipment(DataTable dt)
        {
            try
            {
                if (dt == null)
                {
                    throw new Exception("SAP ME Resource is empty.");
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return false;
            }
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PPI"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("PPI Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand("DELETE FROM SAP_ME_WJ1_EQUIPMENT;", conn);
                    comm.ExecuteNonQuery();
                    comm.CommandText = "SELECT HANDLE,OPERATION,DESCRIPTION FROM SAP_ME_WJ1_OPERATION WHERE IS_VALID = 'Y' ORDER BY OPERATION ASC; ";
                    SqlDataReader reader = comm.ExecuteReader();
                    DataTable dts = new DataTable();
                    dts.Load(reader);
                    StringBuilder sqlStr = new StringBuilder();
                    foreach (DataRow row_opt in dts.Rows)
                    {
                        foreach (DataRow row_res in dt.Rows)
                        {
                            if (row_opt["OPERATION"].ToString() == row_res["OPERATION"].ToString())
                            {
                                sqlStr.Append(string.Format("INSERT INTO SAP_ME_WJ1_EQUIPMENT (HANDLE_OPERATION,EQUIPMENT_NO,DESCRIPTION) VALUES ('{0}','{1}','{2}');", row_opt["HANDLE"], row_res["RESRCE"], row_res["DESCRIPTION"]));
                            }
                        }
                    }
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    comm.CommandText = sqlStr.ToString();
                    comm.ExecuteNonQuery();
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return false;
                }
            }
        }
        /// <summary>
        /// 获取工序理论产能
        /// </summary>
        /// <returns></returns>
        public static DataTable GetOperationCapacity()
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_ME_WJ1"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("HANA Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand("SELECT ITEM_NO,OPERATION_NO OPERATION,UOM_PRODUCTION FROM Z_OPERATION_UOM zou ORDER BY ITEM_NO,OPERATION_NO;", conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    DataTable dts = new DataTable();
                    dts.Load(reader);
                    reader.Close();
                    if (dts.Rows.Count == 0)
                    {
                        throw new Exception("Failed to get SAP ME Resrce info.");
                    }
                    return dts;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 初始化设备理论产能
        /// </summary>
        /// <param name="dt">SAP ME 中定义的设备理论产能</param>
        /// <param name="oper">进行初始化的人员</param>
        /// <returns>true：成功；false：失败</returns>
        public static bool InitEquipmentCapacity(DataTable dt, string oper)
        {
            try
            {
                if (dt == null)
                {
                    throw new Exception("SAP ME OperationCapacity is empty.");
                }
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PPI"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("PPI Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand("DELETE FROM SAP_ME_WJ1_EQUIPMENT_CAPACITY;", conn);
                    comm.ExecuteNonQuery();
                    comm.CommandText = "SELECT E.HANDLE,E.HANDLE_OPERATION,O.OPERATION,O.DESCRIPTION FROM SAP_ME_WJ1_EQUIPMENT E INNER JOIN SAP_ME_WJ1_OPERATION O ON E.HANDLE_OPERATION = O.HANDLE;";
                    SqlDataReader reader = comm.ExecuteReader();
                    DataTable dts = new DataTable();
                    dts.Load(reader);
                    StringBuilder sqlStr = new StringBuilder();
                    foreach (DataRow row_dts in dts.Rows)
                    {
                        foreach (DataRow row_dt in dt.Rows)
                        {
                            if (row_dts["OPERATION"].ToString() == row_dt["OPERATION"].ToString())
                            {
                                sqlStr.Append(string.Format("INSERT INTO SAP_ME_WJ1_EQUIPMENT_CAPACITY (HANDLE_OPERATION,HANDLE_EQUIPMENT,ITEM_NO,T_CAPACITY,CREATED_OPERATOR) VALUES ('{0}','{1}','{2}','{3}','{4}');", row_dts["HANDLE_OPERATION"].ToString(), row_dts["HANDLE"].ToString(), row_dt["ITEM_NO"].ToString(), double.Parse(row_dt["UOM_PRODUCTION"].ToString()), oper));
                            }
                        }
                    }
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    comm.CommandText = sqlStr.ToString();
                    comm.ExecuteNonQuery();
                    tran.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 初始化在制数据
        /// </summary>
        /// <param name="dt">SAP ME 在制数据</param>
        /// <param name="oper"></param>
        /// <returns></returns>
        public static bool InitWIP(string item, DataTable dt, string oper)
        {
            try
            {
                if (dt == null)
                {
                    throw new Exception("SAP ME Wip data is empty.");
                }
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PPI"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("PPI Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("UPDATE SAP_ME_WJ1_WIP SET IS_CURRENT = 'N' WHERE ITEM_NO = '{0}';", item), conn);
                    comm.ExecuteNonQuery();
                    comm.CommandText = "SELECT O.HANDLE HANDLE_O,O.OPERATION,E.HANDLE HANDLE_E,E.EQUIPMENT_NO FROM SAP_ME_WJ1_OPERATION O INNER JOIN SAP_ME_WJ1_EQUIPMENT E ON O.IS_VALID = 'Y' AND E.HANDLE_OPERATION = O.HANDLE;";
                    SqlDataReader reader = comm.ExecuteReader();
                    DataTable dts = new DataTable();
                    dts.Load(reader);
                    StringBuilder sqlStr = new StringBuilder();
                    foreach (DataRow row_dts in dts.Rows)
                    {
                        foreach (DataRow row_dt in dt.Rows)
                        {
                            if (row_dt["OPERATION"].ToString() == row_dts["OPERATION"].ToString())
                            {
                                sqlStr.Append(string.Format("INSERT INTO SAP_ME_WJ1_WIP (HANDLE_OPERATION,ITEM_NO,WIP,CREATED_OPERATOR,HANDLE_EQUIPMENT) VALUES ('{0}','{1}','{2}','{3}','{4}');", row_dts["HANDLE_O"].ToString(), row_dt["ITEM_NO"].ToString(), row_dt["QTY"].ToString(), oper, row_dts["HANDLE_O"].ToString()));
                            }
                        }
                    }
                    comm.CommandText = sqlStr.ToString();
                    comm.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable GetEquipmentWipBwySapMeWj1(string item, string dt)
        {
            try
            {
                if (string.IsNullOrEmpty(item))
                {
                    throw new Exception("Item_no is empty.");
                }
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_ME_WJ1"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Hana Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand(String.Format("SELECT ZSE.ITEM_NO AS ITEM_NO,SUBSTR_AFTER(RESOURCE_BO, ',') AS RESURE,TO_CHAR(PARTITION_DATE,'YYYY-MM-DD') AS DATE_TIME,COUNT(1) AS QTY FROM Z_SFC_STEP zss INNER JOIN Z_SFC_EXTEND ZSE ON SUBSTR_AFTER(ZSS.SFC_BO, ',') = ZSE.SFC AND ZSE.IS_CURRENT = 'Y'	AND (ZSS.QTY_IN_WORK = 1 OR ZSS.QTY_IN_QUEUE = 1) AND CURRENT_DATE = TO_CHAR(PARTITION_DATE,'YYYY-MM-DD') AND ZSE.ITEM_NO = '{0}' GROUP BY ITEM_NO,SUBSTR_AFTER(RESOURCE_BO, ','),TO_CHAR(PARTITION_DATE,'YYYY-MM-DD') ORDER BY ITEM_NO,RESURE ASC;", item), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    DataTable dts = new DataTable();
                    dts.Load(reader);
                    reader.Close();
                    return dts;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        public static ItemList GetItemBySapMeWj1()
        {
            try
            {
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_ME_WJ1"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Hana Database can not be opened.");
                    }

                    OdbcCommand comm = new OdbcCommand("SELECT I.ITEM,I.DESCRIPTION FROM ITEM_GROUP G INNER JOIN ITEM_GROUP_MEMBER M ON G.HANDLE = M.ITEM_GROUP_BO AND G.ITEM_GROUP = 'M13' INNER JOIN ITEM I ON M.ITEM_BO = I.HANDLE ", conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    ItemList list = new ItemList();
                    while (reader.Read())
                    {
                        ItemNo item = new ItemNo();
                        item.ITEM = reader["ITEM"].ToString();
                        item.DESCRIPTION = reader["DESCRIPTION"].ToString();
                        list.ItemLists.Add(item);
                    }
                    reader.Close();
                    return list;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 通过 PACK 箱号获取整箱电池码号
        /// </summary>
        /// <param name="boxid"></param>
        /// <returns></returns>
        public static DataTable PerformanceAmazonByBox(string boxid)
        {
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
            {
                DataTable mDt = new DataTable();
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::PerformanceAmazonByBox => SAP PACK Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT DISTINCT ZPM.MODEL ITEM_NO ,ZPM4.NUM PACK_PLT_NO ,ZPM4.CREATED_DATE_TIME PACK_SHP_DATE ,ZPM.NUM PACK_CTN_NO ,ZPM.CREATED_DATE_TIME PACK_SHP_TIME ,'' PACK_LOT_NO ,ZPM3.NUM_ID BATTERY_SN ,ZPM3.NUM CELL_SN ,ZOSP2.PCM_0001 PCM_SN ,ZOSP.A002 PACK_OCV ,ZOSP.A003 PACK_ACR ,ZOSP.A010 PACK_TDOC ,ZOSP.A011 PACK_DOC ,ZOSP.A018 PACK_TSC ,ZOSP.A019 PACK_SRV ,ZOSP.A021 PACK_TCOC ,ZOSP.A022 PACK_COC ,ZOSP3.A002 PACK_LGTH ,ZOSP3.A003 PACK_WDTH ,ZOSP3.A004 PACK_THK ,ZPD.PCM_OVP1 ,ZPD.PCM_OVR1 ,ZPD.PCM_UVP1 ,ZPD.PCM_UVR1 ,ZPD.PCM_COC1 ,ZPD.PCM_DOC1 ,ZPD.PCM_TOVP1 ,ZPD.PCM_TUVP1 ,ZPD.PCM_TCOC1 ,ZPD.PCM_TDOC1 ,ZPD.PCM_TSC1 ,ZPD.PCM_PC ,ZPD.PCM_PDC ,ZPD.PCM_IMP FROM Z_PACK_MASTER ZPM INNER JOIN Z_PACK_MASTER zpm2 ON ZPM.HANDLE = ZPM2.REF_OBJ AND ZPM.NUM IN ('{0}') AND ZPM.REF_TYPE = 'BOX' INNER JOIN Z_PACK_MASTER zpm3 ON ZPM2.HANDLE = ZPM3.REF_OBJ LEFT JOIN Z_PACK_MASTER ZPM4 ON ZPM.REF_OBJ =ZPM4.HANDLE AND ZPM4.REF_TYPE = 'TRAY' INNER JOIN Z_SFC_EXTEND ZSE ON ZSE.SCAN_SN = ZPM3.NUM_ID AND ZSE.IS_CURRENT = 'Y' AND ZSE.IS_CURRENT = 'Y' AND ZSE.STATUS = 'Y' AND ZSE.IS_PACK = 'Y' INNER JOIN Z_OP42_SFC_PARAM ZOSP ON ZOSP.SFC = ZSE.SFC AND ZOSP.IS_CURRENT = 'Y' INNER JOIN Z_OP36_SFC_PARAM ZOSP2 ON ZOSP2.SFC = ZSE.SFC AND ZOSP2.IS_CURRENT = 'Y' INNER JOIN Z_OP41_SFC_PARAM ZOSP3 ON ZOSP3.SFC =ZSE.SFC AND ZOSP3.IS_CURRENT = 'Y' LEFT JOIN Z_PCM_DATA ZPD ON ZOSP2.PCM_0001 = ZPD.PCM_SN ;", boxid), conn);
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
        /// <summary>
        /// 向数据回传服务器（LSN）同步数据
        /// </summary>
        /// <param name="mDt">SAP ME数据</param>
        /// <returns></returns>
        public static ResultAmazon DataSyncAmazonKazam(DataTable mDt, string operators)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                ResultAmazon result = new ResultAmazon();
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::DataSyncAmazonKazam => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    switch (operators.ToUpper())
                    {
                        case "CREATE":
                            comm.CommandText = string.Format("SELECT HANDLE FROM AMAZON_CONFIG WHERE STATE = 'Y' AND PTYPE = 'PACK' AND ITEM_NO = '{0}';", mDt.Rows[0][0].ToString().ToUpper());
                            break;
                        case "UPDATE":
                            comm.CommandText = string.Format("SELECT HANDLE FROM AMAZON_CONFIG WHERE STATE = 'Y' AND PTYPE = 'PACK' AND HANDLE = '{0}';", mDt.Rows[0][0].ToString().ToUpper());
                            CleanData(mDt);
                            break;
                    }
                    string handle = comm.ExecuteScalar().ToString();
                    if (string.IsNullOrEmpty(handle))
                    {
                        throw new Exception("Amazon config handle is not exist.");
                    }
                    StringBuilder sql = new StringBuilder();
                    for (int i = 0; i < mDt.Rows.Count; ++i)
                    {
                        sql.Append("INSERT INTO AMAZON_KAZAM_PACK (HANDLE_CONFIG,PACK_PLT_NO,PACK_SHP_DATE,PACK_SHP_NO,PACK_CTN_NO,PACK_SHP_TIME,PACK_LOT_NO,BATTERY_SN,CELL_SN,PCM_SN,PACK_OCV,PACK_ACR,PACK_TDOC,PACK_DOC,PACK_TSC,PACK_SRV,PACK_TCOC,PACK_COC,PACK_LGTH,PACK_WDTH,PACK_THK,PCM_OVP1,PCM_OVR1,PCM_UVP1,PCM_UVR1,PCM_COC1,PCM_DOC1,PCM_TOVP1,PCM_TUVP1,PCM_TCOC1,PCM_TDOC1,PCM_TSC1,PCM_PC,PCM_PDC,PCM_IMP,PCM_TIME,PACK_TIME) VALUES (");
                        sql.Append("'");
                        sql.Append(handle);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_PLT_NO"].ToString());
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_SHP_DATE"].ToString());
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_SHP_NO"].ToString());
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_CTN_NO"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_SHP_TIME"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_LOT_NO"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["BATTERY_SN"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["CELL_SN"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_SN"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_OCV"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_ACR"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_TDOC"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_DOC"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_TSC"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_SRV"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_TCOC"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_COC"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_LGTH"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_WDTH"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_THK"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_OVP1"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_OVR1"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_UVP1"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_UVR1"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_COC1"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_DOC1"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_TOVP1"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_TUVP1"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_TCOC1"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_TDOC1"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_TSC1"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_PC"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_PDC"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_IMP"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PCM_TIME"]);
                        sql.Append("','");
                        sql.Append(mDt.Rows[i]["PACK_TIME"]);
                        sql.Append("');");
                    }
                    comm.CommandText = sql.ToString();
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    int row_affect = comm.ExecuteNonQuery();
                    if (row_affect != mDt.Rows.Count)
                    {
                        result.RESULT = "fail";
                        tran.Rollback();
                    }
                    else
                    {
                        result.RESULT = "success";
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.RESULT = "fail";
                }
                return result;
            }
        }
        /// <summary>
        /// 清理数据
        /// </summary>
        /// <param name="mDt"></param>
        /// <returns></returns>
        public static ResultAmazon CleanData(DataTable mDt)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                ResultAmazon result = new ResultAmazon();
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::DataSyncAmazonKazam => Database can not be opened.");
                    }
                    StringBuilder sql = new StringBuilder();
                    for (int i = 0; i < mDt.Rows.Count; ++i)
                    {
                        //sql.Append(string.Format("DELETE FROM AMAZON_KAZAM_PACK WHERE BATTERY_SN = '{0}' AND (PACK_SHP_NO = '' OR PACK_SHP_NO IS NULL OR PACK_SHP_DATE = '' OR PACK_SHP_DATE IS NULL);", mDt.Rows[i]["BATTERY_SN"]));
                        sql.Append(string.Format("DELETE FROM AMAZON_KAZAM_PACK WHERE BATTERY_SN = '{0}';", mDt.Rows[i]["BATTERY_SN"]));
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    if (comm.ExecuteNonQuery() == mDt.Rows.Count)
                    {
                        result.RESULT = "success";
                        tran.Commit();
                    }
                    else
                    {
                        result.RESULT = "fail";
                        tran.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.RESULT = "fail";
                }
                return result;
            }
        }
        /// <summary>
        /// 获取电池指定参数的数据
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public static EntityAmazonBatteryParameter SyncPackData(string barcode, string para)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                EntityAmazonBatteryParameter result = new EntityAmazonBatteryParameter();
                result.Serial = barcode;
                result.Parameter = para;
                result.DateTime = DateTime.Now.ToString("s");
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::SyncBatteryData => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT {1} FROM AMAZON_KAZAM_PACK WHERE BATTERY_SN = '{0}'", barcode, para), conn);
                    result.Value = comm.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
                return result;
            }
        }
        /// <summary>
        /// 将指定电池数据转移至备份表中，并删除就绪表中数据
        /// </summary>
        /// <param name="barcode">电池码号</param>
        /// <returns></returns>
        public static ResultAmazon BackupPackData(string barcode)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                ResultAmazon result = new ResultAmazon();
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::DataSyncAmazonKazam => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO AMAZON_KAZAM_PACK_BACKUP (HANDLE_CONFIG,PACK_PLT_NO,PACK_SHP_DATE,PACK_CTN_NO,PACK_SHP_TIME,PACK_SHP_NO,PACK_LOT_NO,BATTERY_SN,CELL_SN,PCM_SN,PACK_OCV,PACK_ACR,PACK_TDOC,PACK_DOC,PACK_TSC,PACK_SRV,PACK_TCOC,PACK_COC,PACK_LGTH,PACK_WDTH,PACK_THK,PCM_OVP1,PCM_OVR1,PCM_UVP1,PCM_UVR1,PCM_COC1,PCM_DOC1,PCM_TOVP1,PCM_TUVP1,PCM_TCOC1,PCM_TDOC1,PCM_TSC1,PCM_PC,PCM_PDC,PCM_IMP,CREATED_DATE_TIME) select HANDLE_CONFIG,PACK_PLT_NO,PACK_SHP_DATE,PACK_CTN_NO,PACK_SHP_TIME,PACK_SHP_NO,PACK_LOT_NO,BATTERY_SN,CELL_SN,PCM_SN,PACK_OCV,PACK_ACR,PACK_TDOC,PACK_DOC,PACK_TSC,PACK_SRV,PACK_TCOC,PACK_COC,PACK_LGTH,PACK_WDTH,PACK_THK,PCM_OVP1,PCM_OVR1,PCM_UVP1,PCM_UVR1,PCM_COC1,PCM_DOC1,PCM_TOVP1,PCM_TUVP1,PCM_TCOC1,PCM_TDOC1,PCM_TSC1,PCM_PC,PCM_PDC,PCM_IMP,CREATED_DATE_TIME from amazon_kazam_pack WHERE BATTERY_SN = '{0}';DELETE FROM AMAZON_KAZAM_PACK WHERE BATTERY_SN = '{0}';", barcode), conn);
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    if (comm.ExecuteNonQuery() == 2)
                    {
                        tran.Commit();
                        result.RESULT = "success";
                    }
                    else
                    {
                        tran.Rollback();
                        result.RESULT = "fail";
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.RESULT = "fail";
                }
                return result;
            }
        }
        public static ResultAmazon BackupCellData(string barcode)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                ResultAmazon result = new ResultAmazon();
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::DataSyncAmazonKazam => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO AMAZON_KAZAM_CELL_BACKUP (HANDLE_CONFIG,CELL_SN,CELL_SUPPLIER,CELL_PHASE,CELL_MODEL,CELL_CAPACITY,CELL_CAPACITY_TIME,CELL_OCV1,CELL_OCV1_TIME,CELL_ACR1,CELL_ACR1_TIME,CELL_OCV2,CELL_OCV2_TIME,CELL_ACR2,CELL_ACR2_TIME,CELL_K_VALUE,CELL_THICKNESS,CELL_WIDTH,CELL_LENGTH,CELL_WEIGHT,CELL_SHIPPING_OCV,CELL_SHIPPING_ACR,CELL_SHIPPING_TIME,CELL_CARTON_NO,CELL_PALLET_NO,CELL_LOT_NO,CELL_SHIPPING_NO,CELL_SHIPPING_DATE,CREATED_DATE_TIME) select HANDLE_CONFIG,CELL_SN,CELL_SUPPLIER,CELL_PHASE,CELL_MODEL,CELL_CAPACITY,CELL_CAPACITY_TIME,CELL_OCV1,CELL_OCV1_TIME,CELL_ACR1,CELL_ACR1_TIME,CELL_OCV2,CELL_OCV2_TIME,CELL_ACR2,CELL_ACR2_TIME,CELL_K_VALUE,CELL_THICKNESS,CELL_WIDTH,CELL_LENGTH,CELL_WEIGHT,CELL_SHIPPING_OCV,CELL_SHIPPING_ACR,CELL_SHIPPING_TIME,CELL_CARTON_NO,CELL_PALLET_NO,CELL_LOT_NO,CELL_SHIPPING_NO,CELL_SHIPPING_DATE,CREATED_DATE_TIME from amazon_kazam_CELL WHERE CELL_SN = '{0}';DELETE FROM AMAZON_KAZAM_CELL WHERE CELL_SN = '{0}';", barcode), conn);
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    if (comm.ExecuteNonQuery() == 2)
                    {
                        tran.Commit();
                        result.RESULT = "success";
                    }
                    else
                    {
                        tran.Rollback();
                        result.RESULT = "fail";
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.RESULT = "fail";
                }
                return result;
            }
        }
        /// <summary>
        /// 保存向 KAZAM 发送的 JSON 信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        //public static ResultAmazon BackupSendInfo(string json)
        //{
        //    using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
        //    {
        //        ResultAmazon result = new ResultAmazon();

        //    }
        //}
        /// <summary>
        /// 获取指定电池全部参数
        /// </summary>
        /// <param name="barcode">电池码号</param>
        /// <returns></returns>
        public static List<EntityAmazonBatteryParameter> SyncPackData(string barcode)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                List<EntityAmazonBatteryParameter> result = new List<EntityAmazonBatteryParameter>();
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::SyncBatteryData => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT SUPPLIER,PROJECT,PHASE,'LISHEN' FACTORY_ID,PACK_PLT_NO,PACK_SHP_NO,PACK_SHP_DATE,PACK_CTN_NO,PACK_SHP_TIME,PACK_LOT_NO,UPPER(BATTERY_SN) BATTERY_SN,CELL_SN CELL1_SN,PCM_SN,CAST(PACK_OCV AS NUMERIC(18,4))/1000 PACK_OCV,PACK_ACR,PACK_TDOC,PACK_DOC,PACK_TSC,PACK_SRV,PACK_TCOC,PACK_COC,PACK_LGTH,PACK_WDTH,PACK_THK,CAST(PCM_OVP1 AS NUMERIC(18,4))/1000 PCM_OVP1,CAST(PCM_OVR1 AS NUMERIC(18,4))/1000 PCM_OVR1,CAST(PCM_UVP1 AS NUMERIC(18,4))/1000 PCM_UVP1,CAST(PCM_UVR1 AS NUMERIC(18,4))/1000 PCM_UVR1,CAST(PCM_COC1 AS NUMERIC(18,4))/1000 PCM_COC1,CAST(PCM_DOC1 AS NUMERIC(18,4))/1000 PCM_DOC1,CAST(PCM_TOVP1 AS NUMERIC(18,4))/1000 PCM_TOVP1,PCM_TUVP1,PCM_TCOC1,PCM_TDOC1,PCM_TSC1,PCM_PC,PCM_PDC,PCM_IMP,PCM_TIME,PACK_TIME FROM AMAZON_KAZAM_PACK K INNER JOIN AMAZON_CONFIG C ON K.HANDLE_CONFIG = C.HANDLE AND C.STATE = 'Y' AND K.STATE = 'Y' WHERE BATTERY_SN = '{0}';", barcode), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    reader.Read();
                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        EntityAmazonBatteryParameter entity = new EntityAmazonBatteryParameter();
                        entity.Serial = barcode; ;
                        entity.DateTime = DateTime.Now.ToString("s");
                        entity.Parameter = reader.GetName(i);
                        entity.Value = reader[i].ToString();
                        result.Add(entity);
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
                return result;
            }
        }
        /// <summary>
        /// 获取指定电芯全部参数
        /// </summary>
        /// <param name="barcode">电芯码号</param>
        /// <returns></returns>
        public static List<EntityAmazonBatteryParameter> SyncCellData(string barcode)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                List<EntityAmazonBatteryParameter> result = new List<EntityAmazonBatteryParameter>();
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::SyncBatteryData => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT CELL_SN,CELL_SUPPLIER,CELL_PHASE,CELL_MODEL,CELL_CAPACITY,CELL_CAPACITY_TIME,CELL_OCV1,CELL_OCV1_TIME,CELL_ACR1,CELL_ACR1_TIME,CELL_OCV2,CELL_OCV2_TIME,CELL_ACR2,CELL_ACR2_TIME,CELL_K_VALUE,CELL_THICKNESS,CELL_WIDTH,CELL_LENGTH,CELL_WEIGHT,CELL_SHIPPING_OCV,CELL_SHIPPING_ACR,CELL_SHIPPING_TIME,CELL_CARTON_NO,CELL_PALLET_NO,CELL_LOT_NO,CELL_SHIPPING_NO,CONVERT(varchar(12), CELL_SHIPPING_DATE, 23) CELL_SHIPPING_DATE FROM AMAZON_KAZAM_CELL A INNER JOIN AMAZON_CONFIG C ON A.HANDLE_CONFIG = C.HANDLE WHERE C.STATE = 'Y' AND A.CELL_SN = '{0}';", barcode), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    reader.Read();
                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        EntityAmazonBatteryParameter entity = new EntityAmazonBatteryParameter();
                        entity.Serial = barcode; ;
                        entity.DateTime = DateTime.Now.ToString("s");
                        entity.Parameter = reader.GetName(i);
                        entity.Value = reader[i].ToString();
                        result.Add(entity);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
                return result;
            }
        }

        /// <summary>
        /// 获取指定箱体内的电池码号清单
        /// </summary>
        /// <param name="barcode">箱号</param>
        /// <returns></returns>
        public static List<string> BatteryNoByBox(string barcode)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::BatteryNoByBox => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT BATTERY_SN FROM AMAZON_KAZAM_PACK WHERE PACK_CTN_NO = '{0}';", barcode), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    List<string> BatteryList = new List<string>();
                    while (reader.Read())
                    {
                        BatteryList.Add(reader["BATTERY_SN"].ToString());
                    }
                    return BatteryList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取全部数据（箱/栈板）
        /// </summary>
        /// <param name="barcode">箱号/栈板号</param>
        /// <returns></returns>
        public static DataTable AllParametersByBarcode(string barcode)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::AllParametersByBarcode => Database can not be opened.");
                    }
                    string[] box = barcode.Split(',');
                    barcode = "";
                    for (int i = 0; i < box.Length; ++i)
                    {
                        barcode += string.Format("'{0}',", box[i]);
                    }
                    if (barcode.LastIndexOf(',') == barcode.Length - 1)
                    {
                        barcode = barcode.Substring(0, barcode.Length - 1);
                    }

                    SqlCommand comm = new SqlCommand(string.Format("SELECT P.*,C.FACTORY_ID FROM AMAZON_KAZAM_PACK P INNER JOIN AMAZON_CONFIG C ON P.HANDLE_CONFIG = C.HANDLE WHERE P.STATE = 'Y' AND PACK_PLT_NO IN ({0}) OR PACK_CTN_NO IN ({0});", barcode), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    DataTable mDt = new DataTable();
                    mDt.Load(reader);
                    return mDt;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 通过 PACK 箱号获取整箱电池码号
        /// </summary>
        /// <param name="boxid"></param>
        /// <returns></returns>
        public static DataTable PerformanceAmazonByBox(string boxid, string pack_plt_no, string pack_lot_no, string pack_shp_no, string pack_shp_date)
        {
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
            {
                DataTable mDt = new DataTable();
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::PerformanceAmazonByBox => SAP PACK Database can not be opened.");
                    }
                    string[] box = boxid.Split(',');
                    boxid = "";
                    for (int i = 0; i < box.Length; ++i)
                    {
                        boxid += string.Format("'{0}',", box[i]);
                    }
                    if (boxid.LastIndexOf(',') == boxid.Length - 1)
                    {
                        boxid = boxid.Substring(0, boxid.Length - 1);
                    }
                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT REF_TYPE FROM Z_PACK_MASTER zpm WHERE NUM = {0};", boxid), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception("The pallet number or container number does not exist.");
                    }
                    reader.Read();
                    if (reader[0].ToString() == "BOX")
                    {
                        comm.CommandText = string.Format("SELECT DISTINCT ZPM.MODEL ITEM_NO,'' PACK_PLT_NO,'' PACK_SHP_DATE,ZPM.NUM PACK_CTN_NO,ZPM.CREATED_DATE_TIME PACK_SHP_TIME,'' PACK_LOT_NO,'' PACK_SHP_NO,ZPM3.NUM_ID BATTERY_SN,ZPM3.NUM CELL_SN,ZOSP2.PCM_0001 PCM_SN,ZOSP.A002 PACK_OCV,ZOSP.A003 PACK_ACR,ZOSP.A010 PACK_TDOC,ZOSP.A011 PACK_DOC,ZOSP.A018 PACK_TSC,ZOSP.A019 PACK_SRV,ZOSP.A021 PACK_TCOC,ZOSP.A022 PACK_COC,ZOSP3.A002 PACK_LGTH,ZOSP3.A003 PACK_WDTH,ZOSP3.A004 PACK_THK,ZPD.PCM_OVP1,ZPD.PCM_OVR1,ZPD.PCM_UVP1,ZPD.PCM_UVR1,ZPD.PCM_COC1,ZPD.PCM_DOC1,ZPD.PCM_TOVP1,ZPD.PCM_TUVP1,ZPD.PCM_TCOC1,ZPD.PCM_TDOC1,ZPD.PCM_TSC1,ZPD.PCM_PC,ZPD.PCM_PDC,ZPD.PCM_IMP ,ZPD.PCM_TIME ,ZOSP.CREATED_DATE_TIME PACK_TIME FROM Z_PACK_MASTER zpm INNER JOIN Z_PACK_MASTER zpm2 ON ZPM.HANDLE = ZPM2.REF_OBJ AND ZPM.NUM = {0} INNER JOIN Z_PACK_MASTER zpm3 ON ZPM2.HANDLE = ZPM3.REF_OBJ INNER JOIN Z_SFC_EXTEND ZSE ON ZSE.SCAN_SN = ZPM3.NUM_ID AND ZSE.IS_CURRENT = 'Y' AND ZSE.IS_CURRENT = 'Y' AND ZSE.STATUS = 'Y' AND ZSE.IS_PACK = 'Y' INNER JOIN Z_OP42_SFC_PARAM ZOSP ON ZOSP.SFC = ZSE.SFC AND ZOSP.IS_CURRENT = 'Y' INNER JOIN Z_OP36_SFC_PARAM ZOSP2 ON ZOSP2.SFC = ZSE.SFC AND ZOSP2.IS_CURRENT = 'Y' INNER JOIN Z_OP41_SFC_PARAM ZOSP3 ON ZOSP3.SFC = ZSE.SFC AND ZOSP3.IS_CURRENT = 'Y' LEFT JOIN Z_PCM_DATA ZPD ON ZOSP2.PCM_0001 = ZPD.PCM_SN;", boxid);
                    }
                    else if (reader[0].ToString() == "TRAY")
                    {
                        comm.CommandText = string.Format("SELECT DISTINCT ZPM.MODEL ITEM_NO ,ZPM.NUM PACK_PLT_NO,'' PACK_SHP_DATE ,ZPM2.NUM PACK_CTN_NO ,ZPM.CREATED_DATE_TIME PACK_SHP_TIME ,'' PACK_LOT_NO ,'' PACK_SHP_NO ,ZPM4.NUM_ID BATTERY_SN ,ZPM4.NUM CELL_SN ,ZOSP2.PCM_0001 PCM_SN ,ZOSP.A002 PACK_OCV ,ZOSP.A003 PACK_ACR ,ZOSP.A010 PACK_TDOC ,ZOSP.A011 PACK_DOC ,ZOSP.A018 PACK_TSC ,ZOSP.A019 PACK_SRV ,ZOSP.A021 PACK_TCOC ,ZOSP.A022 PACK_COC ,ZOSP3.A002 PACK_LGTH ,ZOSP3.A003 PACK_WDTH ,ZOSP3.A004 PACK_THK ,ZPD.PCM_OVP1 ,ZPD.PCM_OVR1 ,ZPD.PCM_UVP1 ,ZPD.PCM_UVR1 ,ZPD.PCM_COC1 ,ZPD.PCM_DOC1 ,ZPD.PCM_TOVP1 ,ZPD.PCM_TUVP1 ,ZPD.PCM_TCOC1 ,ZPD.PCM_TDOC1 ,ZPD.PCM_TSC1 ,ZPD.PCM_PC ,ZPD.PCM_PDC ,ZPD.PCM_IMP ,ZPD.PCM_TIME ,ZOSP.CREATED_DATE_TIME PACK_TIME FROM Z_PACK_MASTER zpm INNER JOIN Z_PACK_MASTER zpm2 ON ZPM.HANDLE = ZPM2.REF_OBJ AND ZPM.NUM = {0} INNER JOIN Z_PACK_MASTER zpm3 ON ZPM2.HANDLE = ZPM3.REF_OBJ INNER JOIN Z_PACK_MASTER zpm4 ON ZPM3.HANDLE = ZPM4.REF_OBJ INNER JOIN Z_SFC_EXTEND ZSE ON ZSE.SCAN_SN = ZPM4.NUM_ID AND ZSE.IS_CURRENT = 'Y' AND ZSE.IS_CURRENT = 'Y' AND ZSE.STATUS = 'Y' AND ZSE.IS_PACK = 'Y' INNER JOIN Z_OP42_SFC_PARAM ZOSP ON ZOSP.SFC = ZSE.SFC AND ZOSP.IS_CURRENT = 'Y' INNER JOIN Z_OP36_SFC_PARAM ZOSP2 ON ZOSP2.SFC = ZSE.SFC AND ZOSP2.IS_CURRENT = 'Y' INNER JOIN Z_OP41_SFC_PARAM ZOSP3 ON ZOSP3.SFC = ZSE.SFC AND ZOSP3.IS_CURRENT = 'Y' LEFT JOIN Z_PCM_DATA ZPD ON ZOSP2.PCM_0001 = ZPD.PCM_SN;", boxid);
                    }
                    else
                    {
                        throw new Exception("unknow type.");
                    }
                    reader.Close();
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
        /// <summary>
        /// 老分档机从 SAP 下载数据后，数据列需要调整顺序
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static XmlDocument RearrangeRankStandard(XmlDocument doc)
        {
            try
            {
                XmlElement Elements = doc.DocumentElement;
                if (Elements.ChildNodes.Count == 0)
                {
                    throw new Exception("SAP_Information::RearrangeRankStandard => XmlDocument is invalid");
                }
                XmlNodeList nodeList = Elements.ChildNodes;
                for (int i = 0; i < nodeList.Count; ++i)
                {
                    XmlNode node = nodeList[i];
                    string _show = node.Attributes["SHOWCONDITION"].Value;
                    //string _real = node.Attributes["REALCONDITION"].Value;
                    string[] showList = _show.Split(new string[] { "并且" }, StringSplitOptions.None);
                    //string[] realList = _real.Split(new string[] { "AND" }, StringSplitOptions.None);
                    string[] show = new string[4];
                    //string[] real = new string[4];
                    StringBuilder _showCondition = new StringBuilder();
                    //StringBuilder _realCondition = new StringBuilder();
                    for (int j = 0; j < showList.Length; ++j)
                    {
                        if (showList[j].Contains("电压"))
                        {
                            show[0] = showList[j];
                            //real[0] = realList[j];
                        }
                        else if (showList[j].Contains("内阻"))
                        {
                            show[1] = showList[j];
                            //real[1] = realList[j];
                        }
                        else if (showList[j].Contains("KValue"))
                        {
                            show[3] = showList[j];
                            //real[2] = realList[j];
                        }
                        else if (showList[j].Contains("放电容量2"))
                        {
                            show[2] = showList[j];
                            //real[3] = realList[j];
                        }
                        else
                        {
                            if (_showCondition.Length == 0)
                            {
                                _showCondition.Append(showList[j]);
                                //_realCondition.Append(realList[j]);
                            }
                            else
                            {
                                _showCondition.Append(" 并且 ");
                                _showCondition.Append(showList[j]);
                                //_realCondition.Append(" AND ");
                                //_realCondition.Append(realList[j]);
                            }
                        }
                    }
                    string showCondition = "", realCondition = "";
                    for (int k = 0; k < 4; ++k)
                    {
                        if (!string.IsNullOrEmpty(show[k]))
                        {
                            if (showCondition.Length == 0)
                            {
                                showCondition += show[k];
                                //realCondition += real[k];
                            }
                            else
                            {
                                showCondition += " 并且 ";
                                showCondition += show[k];
                                //realCondition += " AND ";
                                //realCondition += real[k];
                            }
                        }
                    }
                    if (showCondition.Length == 0)
                    {
                        showCondition = _showCondition.ToString();
                    }
                    else if (_showCondition.Length != 0)
                    {
                        showCondition += " 并且 ";
                        showCondition += _showCondition.ToString();
                    }
                    //if (realCondition.Length == 0)
                    //{
                    //    realCondition = _realCondition.ToString();
                    //}
                    //else if (_realCondition.Length != 0)
                    //{
                    //    realCondition += " AND ";
                    //    realCondition = _realCondition.ToString();
                    //}
                    node.Attributes["SHOWCONDITION"].Value = showCondition;
                    //node.Attributes["REALCONDITION"].Value = realCondition;
                }
                return doc;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return doc;
            }
        }
        /// <summary>
        /// 获取物料维护中“自定义字段”数据
        /// </summary>
        /// <param name="code">箱号，如：90004437</param>
        /// <returns></returns>
        public static DataTable CustomDataMaintenanceBySAPBoxID(string code)
        {
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::CustomDataMaintenanceBySAP => Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT CF.ATTRIBUTE,CF.VALUE FROM ITEM i INNER JOIN CUSTOM_FIELDS cf ON I.HANDLE = CF.HANDLE INNER JOIN Z_CELL_PACK ZCP ON ZCP.ITEM = I.ITEM WHERE ZCP.NUM = '{0}';", code), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取 amazon 项目与LSN料号对应关系
        /// </summary>
        /// <returns></returns>
        public static Hashtable AmazonProject()
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::AmazonProject => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT PROJECT,ITEM_NO,PTYPE FROM AMAZON_CONFIG;", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    Hashtable project = new Hashtable();
                    while (reader.Read())
                    {
                        project.Add(reader["PROJECT"].ToString() + "_" + reader["PTYPE"].ToString(), reader["ITEM_NO"]);
                    }
                    return project;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static Hashtable AmazonProject2()
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::AmazonProject => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT PROJECT,HANDLE,PTYPE FROM AMAZON_CONFIG;", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    Hashtable project = new Hashtable();
                    while (reader.Read())
                    {
                        project.Add(reader["PROJECT"].ToString() + "_" + reader["PTYPE"].ToString(), reader["HANDLE"]);
                    }
                    return project;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取参数清单
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static List<AmazonParameter> AmazonParameters(string item)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SyncRemote"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SAP_Information::AmazonParameters => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT AC.ITEM_NO,AC.PROJECT,ACA.ITEM,ACA.LSL,ACA.USL,ACA.UNIT,ACA.ISEMPTY,ACA.EQUAL FROM AMAZON_CONFIG ac INNER JOIN AMAZON_CONFIG_ADDITION aca ON AC.HANDLE = ACA.HANDLE_CONFIG WHERE AC.ITEM_NO = '{0}';", item), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("SAP_Information::AmazonParameters => Parameters is empty.");
                    List<AmazonParameter> parameterList = new List<AmazonParameter>();
                    while (reader.Read())
                    {
                        AmazonParameter para = new AmazonParameter();
                        para.ITEM = reader["ITEM"].ToString();
                        para.ITEM_NO = reader["ITEM_NO"].ToString();
                        para.LSL = reader["LSL"].ToString();
                        para.USL = reader["USL"].ToString();
                        para.UNIT = reader["UNIT"].ToString();
                        para.ISEMPTY = reader["ISEMPTY"].ToString();
                        para.PROJECT = reader["PROJECT"].ToString();
                        para.EQUAL = reader["EQUAL"].ToString();
                        parameterList.Add(para);
                    }
                    return parameterList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取计算列
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static Hashtable CalculateColumn(string line, string op)
        {
            try
            {
                if (string.IsNullOrEmpty(line)) line = "SAP_ME_WJ1";
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings[line].ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("Hana Database can not be opened.");
                    }
                    OdbcCommand comm = new OdbcCommand($"SELECT COLUMN_NAME ,COLUMN_DESC FROM Z_CO_CALCULATE_COLUMN WHERE OPERATION = '{op}' ;", conn);
                    using (OdbcDataReader reader = comm.ExecuteReader())
                    {
                        Hashtable hashColumns = new Hashtable();
                        while (reader.Read())
                        {
                            hashColumns.Add(reader["COLUMN_DESC"], reader["COLUMN_NAME"]);
                        }
                        return hashColumns;
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
    }
}