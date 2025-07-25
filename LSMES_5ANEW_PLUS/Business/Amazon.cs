using LSMES_5ANEW_PLUS.App_Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Collections;
using System.Text;

namespace LSMES_5ANEW_PLUS.Business
{
    public class AmazonKazam
    {
        /// <summary>
        /// 新建/获取线体信息
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static public Hashtable Lines(AmazonLine line)
        {
            if (line == null) return null;
            string sql = null;
            //if (string.IsNullOrEmpty(line.HANDLE))  // 新建
            //{
            //    sql = $"BEGIN IF NOT EXISTS (SELECT * FROM AMAZON_LINE WHERE LINE  = '{line.LINE}') BEGIN INSERT INTO AMAZON_LINE (LINE,CREATED_USER) VALUES ('{line.LINE}','{line.CREATED_USER}') END END";
            //}
            //else // 获取
            //{
            //    sql = $"SELECT * FROM AMAZON_LINE WHERE LINE = '{line.LINE}' OR LINE <> '{line.LINE}' AND NOT EXISTS (SELECT 1 FROM AMAZON_LINE WHERE LINE = '{line.LINE}');";
            //}
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                sql = $"BEGIN IF NOT EXISTS (SELECT * FROM AMAZON_LINE WHERE LINE  = '{line.LINE}') BEGIN INSERT INTO AMAZON_LINE (LINE,CREATED_USER) VALUES ('{line.LINE}','{line.CREATED_USER}') END END";
                SqlCommand comm = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(line.LINE))
                {
                    comm.ExecuteNonQuery();
                }
                //sql = $"SELECT * FROM AMAZON_LINE WHERE LINE = '{line.LINE}' OR LINE <> '{line.LINE}' AND NOT EXISTS (SELECT 1 FROM AMAZON_LINE WHERE LINE = '{line.LINE}');";
                sql = "SELECT * FROM AMAZON_LINE;";
                comm.CommandText = sql;
                SqlDataReader reader;
                Hashtable lines = new Hashtable();
                try
                {
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        lines.Add(reader["LINE"].ToString(), reader["HANDLE"].ToString());
                    }
                    return lines;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 新建 / 获取工作站
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        static public Hashtable Stations(AmazonStation station)
        {
            if (station == null) return null;
            string sql = null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                sql = $"BEGIN IF NOT EXISTS (SELECT * FROM AMAZON_STATION WHERE STATION  = '{station.STATION}') BEGIN INSERT INTO AMAZON_STATION (STATION,CREATED_USER) VALUES ('{station.STATION}','{station.CREATED_USER}') END END";
                SqlCommand comm = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(station.STATION))
                    comm.ExecuteNonQuery();
                comm.CommandText = "SELECT * FROM AMAZON_STATION;";
                SqlDataReader reader;
                Hashtable stations = new Hashtable();
                try
                {
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        stations.Add(reader["STATION"].ToString(), reader["HANDLE"].ToString());
                    }
                    return stations;
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }
        /// <summary>
        /// 新建 / 获取 Fixure
        /// </summary>
        /// <param name="fixure"></param>
        /// <returns></returns>
        static public Hashtable Fixures(AmazonFixure fixure)
        {
            if (fixure == null) return null;
            string sql = null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                sql = $"BEGIN IF NOT EXISTS (SELECT * FROM AMAZON_FIXTURE WHERE FIXTURE  = '{fixure.FIXTURE}') BEGIN INSERT INTO AMAZON_FIXTURE (FIXTURE,CREATED_USER) VALUES ('{fixure.FIXTURE}','{fixure.CREATED_USER}') END END";
                SqlCommand comm = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(fixure.FIXTURE))
                    comm.ExecuteNonQuery();
                comm.CommandText = "SELECT * FROM AMAZON_FIXTURE;";
                SqlDataReader reader;
                Hashtable fixures = new Hashtable();
                try
                {
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        fixures.Add(reader["FIXTURE"].ToString(), reader["HANDLE"].ToString());
                    }
                    return fixures;
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }
        /// <summary>
        /// 新建 / 获取 SLOT
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        static public Hashtable Slots(AmazonSlot slot)
        {
            if (slot == null) return null;
            string sql = null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                sql = $"BEGIN IF NOT EXISTS (SELECT * FROM AMAZON_SLOT WHERE SLOT  = '{slot.SLOT}') BEGIN INSERT INTO AMAZON_SLOT (SLOT,CREATED_USER) VALUES ('{slot.SLOT}','{slot.CREATED_USER}') END END";
                SqlCommand comm = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(slot.SLOT))
                    comm.ExecuteNonQuery();
                comm.CommandText = "SELECT * FROM AMAZON_SLOT;";
                SqlDataReader reader;
                Hashtable slots = new Hashtable();
                try
                {
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        slots.Add(reader["SLOT"].ToString(), reader["HANDLE"].ToString());
                    }
                    return slots;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取 AmazonGroup
        /// </summary>
        /// <param name="project">CAVA、SPARANT、。。。。</param>
        /// <param name="type">CELL / PACK</param>
        /// <returns></returns>
        static public List<AmazonGroup> AmazonGroups(string project, string type)
        {
            if (string.IsNullOrEmpty(project) || string.IsNullOrEmpty(type)) return null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand($@"
                    SELECT AC.*,AL.LINE ,AS2.STATION ,AF.FIXTURE ,AS3.SLOT FROM AMAZON_GROUP AG 
                    INNER JOIN AMAZON_CONFIG AC ON AG.HANDLE_CONFIG = AC.HANDLE 
                    INNER JOIN AMAZON_LINE AL ON AG.HANDLE_LINE = AL.HANDLE 
                    INNER JOIN AMAZON_STATION AS2 ON AG.HANDLE_STATION = AS2.HANDLE 
                    INNER JOIN AMAZON_FIXTURE AF ON AG.HANDLE_FIXTURE = AF.HANDLE 
                    INNER JOIN AMAZON_SLOT AS3 ON AG.HANDLE_SLOT = AS3.HANDLE 
                    WHERE AG.STATE = 'Y' AND AC.PROJECT = '{project}' AND AC.PTYPE = '{type}';
                    ", conn);
                SqlDataReader reader = comm.ExecuteReader();
                List<AmazonGroup> groups = new List<AmazonGroup>();
                while (reader.Read())
                {
                    AmazonGroup group = new AmazonGroup();
                    group.HANDLE = reader["HANDLE"].ToString();
                    group.ITEM_NO = reader["ITEM_NO"].ToString();
                    group.FACTORY_ID = reader["FACTORY_ID"].ToString();
                    group.SUPPLIER = reader["SUPPLIER"].ToString();
                    group.PROJECT = reader["PROJECT"].ToString();
                    group.MODEL = reader["MODEL"].ToString();
                    group.PHASE = reader["PHASE"].ToString();
                    group.PTYPE = reader["PTYPE"].ToString();
                    group.COMMENTS = reader["COMMENTS"].ToString();
                    group.EMAIL = reader["EMAIL"].ToString();
                    group.CONTACT = reader["CONTACT"].ToString();
                    group.LINE = reader["LINE"].ToString();
                    group.STATION = reader["STATION"].ToString();
                    group.FIXTRUE = reader["FIXTURE"].ToString();
                    group.SLOT = reader["SLOT"].ToString();
                    groups.Add(group);
                }
                return groups;
            }
        }
        /// <summary>
        /// 创建绑定关系
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static public int CreateAmazonGroup(AmazonHandle entity)
        {
            if (entity == null) return 0;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand($@"UPDATE AMAZON_GROUP SET STATE = 'N' WHERE HANDLE_CONFIG = '{entity.HANDLE_CONFIG}' AND HANDLE_LINE = '{entity.HANDLE_LINE}'; INSERT INTO AMAZON_GROUP (HANDLE_CONFIG,HANDLE_LINE,HANDLE_STATION,HANDLE_FIXTURE,HANDLE_SLOT,CREATED_USER,STATE) 
                    VALUES ('{entity.HANDLE_CONFIG}','{entity.HANDLE_LINE}','{entity.HANDLE_STATION}','{entity.HANDLE_FIXURE}','{entity.HANDLE_SLOT}','{entity.CREATED_USER}','Y');", conn);
                try
                {
                    int count = comm.ExecuteNonQuery();
                    return count;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 获取标准    
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        static public List<AmazonStandard> AmazonStandard(string item)
        {
            if (string.IsNullOrEmpty(item)) return null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand($"SELECT AS2.* FROM AMAZON_STANDARD AS2 INNER JOIN AMAZON_CONFIG AC ON AS2.ITEM = AC.ITEM_NO WHERE AC.ITEM_NO = '{item}' OR AC.PROJECT = '{item}';", conn);
                try
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    List<AmazonStandard> standardList = new List<AmazonStandard>();
                    while (reader.Read())
                    {
                        AmazonStandard standard = new AmazonStandard();
                        standard.HANDLE = reader["HANDLE"].ToString();
                        standard.ITEM = reader["ITEM"].ToString();
                        standard.TEST_NAME = reader["TEST_NAME"].ToString();
                        standard.HIGH_LIMIT = reader["HIGH_LIMIT"].ToString();
                        standard.LOW_LIMIT = reader["LOW_LIMIT"].ToString();
                        standard.UNIT = reader["UNIT"].ToString();
                        standard.VERBO = reader["VERBO"].ToString();
                        standard.STATE = reader["STATE"].ToString();
                        standard.CREATED_USER = reader["CREATED_USER"].ToString();
                        standard.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                        standardList.Add(standard);
                    }
                    return standardList;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 返回指定型号，类型的条件
        /// </summary>
        /// <param name="project">型号，如：SPARTAN</param>
        /// <param name="type">类型，如：CELL</param>
        /// <returns></returns>
        static public Hashtable AmazonConditions(string project, string type)
        {
            if (string.IsNullOrEmpty(project) || string.IsNullOrEmpty(type))
                return null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        throw new Exception("AmazonKazam::AmazonConditions => Fail to connect.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT A.ITEM,LSL,USL,UNIT,ISEMPTY,EQUAL FROM AMAZON_CONFIG C INNER JOIN AMAZON_CONFIG_ADDITION A ON C.HANDLE = A.HANDLE_CONFIG WHERE A.STATE = 'Y' AND C.PROJECT = '{project}' AND C.PTYPE = '{type}';", conn);
                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        if (!reader.HasRows) throw new Exception("Data is empty.");
                        Hashtable hashCondition = new Hashtable();
                        while (reader.Read())
                        {
                            AmazonCondition entity = new AmazonCondition();
                            entity.LSL = string.IsNullOrEmpty(reader["LSL"].ToString()) ? -9999 : Convert.ToDouble(reader["LSL"]);
                            entity.USL = string.IsNullOrEmpty(reader["USL"].ToString()) ? -9999 : Convert.ToDouble(reader["USL"]);
                            entity.UNIT = reader["UNIT"].ToString();
                            entity.ISEMPTY = reader["ISEMPTY"].ToString();
                            entity.EQUAL = reader["EQUAL"].ToString();
                            hashCondition.Add(reader["ITEM"], entity);
                        }
                        return hashCondition;
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
        /// 获取 CELL 回传情况
        /// </summary>
        /// <param name="StartDate">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <returns></returns>
        static public List<AmazonStatistics> GetAmazonStatisticsCell(string StartDate, string EndDate)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        throw new Exception("AmazonKazam::AmazonStatistics => Fail to connect.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT 'LSN' AS Supplier,AC.PROJECT,'Package' AS Station,'{StartDate}' AS StartDate,'{EndDate}' AS EndDate, COUNT(AKPB.CELL_SN) AS Units, 'Y' AS KazamImplement, 'CELL' AS Remark FROM AMAZON_CONFIG ac INNER JOIN AMAZON_KAZAM_CELL_BACKUP akpb ON AC.HANDLE = AKPB.HANDLE_CONFIG WHERE AKPB.BACKUPED_DATE_TIME BETWEEN '{StartDate}' AND '{EndDate}' GROUP BY AC.PROJECT; ", conn);
                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        List<AmazonStatistics> statisticsList = new List<AmazonStatistics>();
                        while (reader.Read())
                        {
                            AmazonStatistics entity = new AmazonStatistics();
                            entity.Supplier = reader["Supplier"].ToString();
                            entity.Project = reader["PROJECT"].ToString();
                            entity.Station = reader["Station"].ToString();
                            entity.StartDate = reader["StartDate"].ToString();
                            entity.EndDate = reader["EndDate"].ToString();
                            entity.Units = reader["Units"].ToString();
                            entity.KazamImplement = reader["KazamImplement"].ToString();
                            entity.Remark = reader["Remark"].ToString();
                            statisticsList.Add(entity);
                        }
                        return statisticsList;
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
        /// 获取 PACK 回传情况
        /// </summary>
        /// <param name="StartDate">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <returns></returns>
        static public List<AmazonStatistics> GetAmazonStatisticsPack(string StartDate, string EndDate)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        throw new Exception("AmazonKazam::AmazonStatistics => Fail to connect.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT 'LSN' AS Supplier,AC.PROJECT,'Package' AS Station,'{StartDate}' AS StartDate,'{EndDate}' AS EndDate, COUNT(AKPB.CELL_SN) AS Units, 'Y' AS KazamImplement, 'PACK' AS Remark FROM AMAZON_CONFIG ac INNER JOIN AMAZON_KAZAM_PACK_BACKUP akpb ON AC.HANDLE = AKPB.HANDLE_CONFIG WHERE AKPB.BACKUPED_DATE_TIME BETWEEN '{StartDate}' AND '{EndDate}' GROUP BY AC.PROJECT; ", conn);
                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        List<AmazonStatistics> statisticsList = new List<AmazonStatistics>();
                        while (reader.Read())
                        {
                            AmazonStatistics entity = new AmazonStatistics();
                            entity.Supplier = reader["Supplier"].ToString();
                            entity.Project = reader["PROJECT"].ToString();
                            entity.Station = reader["Station"].ToString();
                            entity.StartDate = reader["StartDate"].ToString();
                            entity.EndDate = reader["EndDate"].ToString();
                            entity.Units = reader["Units"].ToString();
                            entity.KazamImplement = reader["KazamImplement"].ToString();
                            entity.Remark = reader["Remark"].ToString();
                            statisticsList.Add(entity);
                        }
                        return statisticsList;
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
        /// 统计 amazon kazam 回传情况
        /// </summary>
        /// <param name="type"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        static public List<AmazonStatistics> AmazonKazamStatistics(string type, string StartDate, string EndDate)
        {
            List<AmazonStatistics> statisticsList = new List<AmazonStatistics>();
            if (string.IsNullOrEmpty(type))
            {
                // 获取 CELL 回传信息
                statisticsList = GetAmazonStatisticsCell(StartDate, EndDate);
                // 获取 PACK 回传信息并与前一列表合并
                statisticsList.AddRange(GetAmazonStatisticsPack(StartDate, EndDate));
            }
            if (type.ToUpper() == "CELL")
            {
                // 获取 CELL 回传信息
                statisticsList = GetAmazonStatisticsCell(StartDate, EndDate);
            }
            if (type.ToUpper() == "PACK")
            {
                // 获取 PACK 回传信息
                statisticsList = GetAmazonStatisticsPack(StartDate, EndDate);
            }
            return statisticsList;
        }
        /// <summary>
        /// 判断码号是否有效
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        static public string IsValidBatterySn(string barcode)
        {
            if (barcode.Trim() == null)
            {
                return null;
            }
            return CreateBatteryInfo(barcode);
        }
        /// <summary>
        /// 生成数据串，符合 Amazon Kazam 标准的字符串
        /// </summary>
        /// <param name="barcode">码号</param>
        /// <returns></returns>
        static public string CreateBatteryInfo(string barcode)
        {
            if (string.IsNullOrEmpty(barcode)) return null;
            using(SqlConnection conn=new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open ();
                if (conn.State != System.Data.ConnectionState.Open) return null;
                SqlCommand comm = new SqlCommand($@"SELECT SUPPLIER
	                ,PROJECT
	                ,PHASE
	                ,'LISHEN' FACTORY_ID
	                ,PACK_PLT_NO
	                ,PACK_SHP_NO
	                ,PACK_SHP_DATE
	                ,PACK_CTN_NO
	                ,PACK_SHP_TIME
	                ,PACK_LOT_NO
	                ,UPPER(BATTERY_SN) BATTERY_SN
	                ,CELL_SN CELL1_SN
	                ,PCM_SN
	                ,CAST(PACK_OCV AS NUMERIC(18, 4)) / 1000 PACK_OCV
	                ,PACK_ACR
	                ,PACK_TDOC
	                ,PACK_DOC
	                ,PACK_TSC
	                ,PACK_SRV
	                ,PACK_TCOC
	                ,PACK_COC
	                ,PACK_LGTH
	                ,PACK_WDTH
	                ,PACK_THK
	                ,CAST(PCM_OVP1 AS NUMERIC(18, 4)) / 1000 PCM_OVP1
	                ,CAST(PCM_OVR1 AS NUMERIC(18, 4)) / 1000 PCM_OVR1
	                ,CAST(PCM_UVP1 AS NUMERIC(18, 4)) / 1000 PCM_UVP1
	                ,CAST(PCM_UVR1 AS NUMERIC(18, 4)) / 1000 PCM_UVR1
	                ,CAST(PCM_COC1 AS NUMERIC(18, 4)) / 1000 PCM_COC1
	                ,CAST(PCM_DOC1 AS NUMERIC(18, 4)) / 1000 PCM_DOC1
	                ,CAST(PCM_TOVP1 AS NUMERIC(18, 4)) / 1000 PCM_TOVP1
	                ,PCM_TUVP1
	                ,PCM_TCOC1
	                ,PCM_TDOC1
	                ,PCM_TSC1
	                ,PCM_PC
	                ,PCM_PDC
	                ,PCM_IMP
	                ,PCM_TIME
	                ,PACK_TIME
                    FROM AMAZON_KAZAM_PACK K
                    INNER JOIN AMAZON_CONFIG C ON K.HANDLE_CONFIG = C.HANDLE
	                AND C.STATE = 'Y'
	                AND K.STATE = 'Y'
                    WHERE BATTERY_SN = '{barcode}';
                ", conn);
                using(SqlDataReader reader = comm.ExecuteReader())
                {
                    StringBuilder sb = new StringBuilder();
                    while(reader.Read())
                    {
                        sb.Append("|supplier=" + reader["SUPPLIER"].ToString() + "|");
                        sb.Append("|project=" + reader["PROJECT"].ToString() + "|");
                        sb.Append("|phase=" + reader["PHASE"].ToString() + "|");
                        sb.Append("|factory_id=" + reader["FACTORY_ID"].ToString() + "|");
                        sb.Append("|pack_plt_no=" + reader["PACK_PLT_NO"].ToString() + "|");
                        sb.Append("|pack_shp_no=" + reader["PACK_SHP_NO"].ToString() + "|");
                        sb.Append("|pack_shp_date=" + reader["PACK_SHP_DATE"].ToString() + "|");
                        sb.Append("|pack_ctn_no=" + reader["PACK_CTN_NO"].ToString() + "|");
                        sb.Append("|pack_shp_time=" + reader["PACK_SHP_TIME"].ToString() + "|");
                        sb.Append("|pack_lot_no=" + reader["PACK_LOT_NO"].ToString() + "|");
                        sb.Append("|battery_sn=" + reader["BATTERY_SN"].ToString() + "|");
                        sb.Append("|cell1_sn=" + reader["CELL1_SN"].ToString() + "|");
                        sb.Append("|pcm_sn=" + reader["PCM_SN"].ToString() + "|");
                        sb.Append("|pack_ocv=" + reader["PACK_OCV"].ToString() + "|");
                        sb.Append("|pack_acr=" + reader["PACK_ACR"].ToString() + "|");
                        sb.Append("|pack_tdoc=" + reader["PACK_TDOC"].ToString() + "|");
                        sb.Append("|pack_doc=" + reader["PACK_DOC"].ToString() + "|");
                        sb.Append("|pack_tsc=" + reader["PACK_TSC"].ToString() + "|");
                        sb.Append("|pack_srv=" + reader["PACK_SRV"].ToString() + "|");
                        sb.Append("|pack_tcoc=" + reader["PACK_TCOC"].ToString() + "|");
                        sb.Append("|pack_coc=" + reader["PACK_COC"].ToString() + "|");
                        sb.Append("|pack_lgth=" + reader["PACK_LGTH"].ToString() + "|");
                        sb.Append("|pack_wdth=" + reader["PACK_WDTH"].ToString() + "|");
                        sb.Append("|pack_thk=" + reader["PACK_THK"].ToString() + "|");
                        sb.Append("|pcm_ovp1=" + reader["PCM_OVP1"].ToString() + "|");
                        sb.Append("|pcm_ovr1=" + reader["PCM_OVR1"].ToString() + "|");
                        sb.Append("|pcm_uvp1=" + reader["PCM_UVP1"].ToString() + "|");
                        sb.Append("|pcm_uvr1=" + reader["PCM_UVR1"].ToString() + "|");
                        sb.Append("|pcm_coc1=" + reader["PCM_COC1"].ToString() + "|");
                        sb.Append("|pcm_doc1=" + reader["PCM_DOC1"].ToString() + "|");
                        sb.Append("|pcm_tovp1=" + reader["PCM_TOVP1"].ToString() + "|");
                        sb.Append("|pcm_tuvp1=" + reader["PCM_TUVP1"].ToString() + "|");
                        sb.Append("|pcm_tcoc1=" + reader["PCM_TCOC1"].ToString() + "|");
                        sb.Append("|pcm_tdoc1=" + reader["PCM_TDOC1"].ToString() + "|");
                        sb.Append("|pcm_tsc1=" + reader["PCM_TSC1"].ToString() + "|");
                        sb.Append("|pcm_pc=" + reader["PCM_PC"].ToString() + "|");
                        sb.Append("|pcm_pdc=" + reader["PCM_PDC"].ToString() + "|");
                        sb.Append("|pcm_imp=" + reader["PCM_IMP"].ToString() + "|");
                        sb.Append("|pcm_time=" + reader["PCM_TIME"].ToString() + "|");
                        sb.Append("|pack_time=" + reader["PACK_TIME"].ToString() + "|");
                    }
                    return sb.ToString();
                }
            }
        }
    }
}