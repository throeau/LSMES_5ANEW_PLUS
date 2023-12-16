using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using LSMES_5ANEW_PLUS.Business;
using LSMES_5ANEW_PLUS.App_Base;
using System.Configuration;


namespace LSMES_5ANEW_PLUS
{
    public class UpdateRemote
    {
        /// <summary>
        /// 上传注液数据
        /// </summary>
        /// <param name="pipeline">设备号</param>
        /// <param name="bomno">电池BOM</param>
        /// <param name="batteryno">电池码号</param>
        /// <param name="pumpno">注液泵号</param>
        /// <param name="beforeWeight">注液前重量</param>
        /// <param name="beforeTime">注液前时间</param>
        /// <param name="afterWeight">注液后重量</param>
        /// <param name="afterTime">注液后时间</param>
        /// <param name="diff">注液量</param>
        /// <param name="state">注液等级</param>
        /// <returns>0：成功 || 1：失败</returns>
        /// 
        int count = 0;
        int runCount = 50;
        List<string> sqlStr = new List<string>();

        public int UpdateE1Data(string equipment, string bomno, string batteryno, string pumpno, string beforeWeight, string beforeTime, string afterWeight, string afterTime, string diff, string state)
        {
            Configuer m_Config = new Configuer();
            try
            {
                StringBuilder m_Sqlbuilder = new StringBuilder();
                string mB1, mB7;
                m_Sqlbuilder.Append("SELECT 'V_'+D.OTHER_NAME+'_'+T.BOMNO+'_B1' AS B1,'V_'+D.OTHER_NAME+'_'+T.BOMNO+'_B7' AS B7 FROM T_REAL_FLOW_BATTERY T LEFT JOIN M_DEPARTMENT D on T.PIPELINE=D.DEPARTMENTNAME WHERE BATTERYNO='");
                m_Sqlbuilder.Append(batteryno);
                m_Sqlbuilder.Append("'");
                //B1表SQL
                m_Sqlbuilder.Clear();
                m_Sqlbuilder.Append("UPDATE ");
                m_Sqlbuilder.Append(" SET E1COMPUTERNO='");
                m_Sqlbuilder.Append(equipment);
                m_Sqlbuilder.Append("',E1PUMPNO='");
                m_Sqlbuilder.Append(pumpno);
                m_Sqlbuilder.Append("',E1BEFORWEIGHT='");
                m_Sqlbuilder.Append(beforeWeight);
                m_Sqlbuilder.Append("',E1INPUTTIME1='");
                m_Sqlbuilder.Append(beforeTime);
                m_Sqlbuilder.Append("',E1AFTERWEIGHT='");
                m_Sqlbuilder.Append(afterWeight);
                m_Sqlbuilder.Append("',E1INPUTTIME2='");
                m_Sqlbuilder.Append(afterTime);
                m_Sqlbuilder.Append("',E1DIFF='");
                m_Sqlbuilder.Append(diff);
                m_Sqlbuilder.Append("',E1STATE='");
                m_Sqlbuilder.Append(state);
                m_Sqlbuilder.Append("' WHERE BATTERYNO='");
                m_Sqlbuilder.Append(batteryno);
                m_Sqlbuilder.Append("' ");
                //B7表SQL
                m_Sqlbuilder.Append("UPDATE ");
                m_Sqlbuilder.Append(" SET E1DIFF='");
                m_Sqlbuilder.Append(diff);
                m_Sqlbuilder.Append("',E1STATE='");
                m_Sqlbuilder.Append(state);
                m_Sqlbuilder.Append("',E1BEFORWEIGHT='");
                m_Sqlbuilder.Append(beforeWeight);
                m_Sqlbuilder.Append("' WHERE BATTERYNO='");
                m_Sqlbuilder.Append(batteryno);
                m_Sqlbuilder.Append("' ");
                //m_RemoteObject.ExecuteQuery(m_Sqlbuilder.ToString());
                return 0;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("UpdateRemote::UpdateE1Data => " + ex.Message);
                return 1;
            }
        }
        public int InsertE1Data(string bomno_sizeno, string equipment, string bomno, string batteryno, string pumpno, string beforeWeight, string beforeTime, string afterWeight, string afterTime, string diff, string state)
        {
            try
            {
                StringBuilder mSQLBuilder = new StringBuilder("insert into s_order_size_b1 (BOMNO_SIZENO,E1COMPUTERNO,BOMNO,BATTERYNO,E1PUMPNO,E1BEFORWEIGHT,E1INPUTTIME1,E1AFTERWEIGHT,E1INPUTTIME2,E1DIFF,E1STATE) values ('");
                mSQLBuilder.Append(bomno_sizeno);
                mSQLBuilder.Append("','");
                mSQLBuilder.Append(equipment);
                mSQLBuilder.Append("','");
                mSQLBuilder.Append(bomno);
                mSQLBuilder.Append("','");
                mSQLBuilder.Append(batteryno);
                mSQLBuilder.Append("','");
                mSQLBuilder.Append(pumpno);
                mSQLBuilder.Append("','");
                mSQLBuilder.Append(beforeWeight);
                mSQLBuilder.Append("','");
                mSQLBuilder.Append(beforeTime);
                mSQLBuilder.Append("','");
                mSQLBuilder.Append(afterWeight);
                mSQLBuilder.Append("','");
                mSQLBuilder.Append(afterTime);
                mSQLBuilder.Append("','");
                mSQLBuilder.Append(diff);
                mSQLBuilder.Append("','");
                mSQLBuilder.Append(state);
                mSQLBuilder.Append("')");
                using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW_PLUS"].ConnectionString))
                {
                    mConn.Open();
                    SqlCommand mComm = new SqlCommand(mSQLBuilder.ToString(), mConn);
                    mComm.ExecuteNonQuery();
                    mConn.Close();
                }
                return 0;
            }
            catch(Exception ex)
            {
                SysLog mLog = new SysLog(ex.Message);
                return 1;
            }
        }
        public int UpdateE6DataFromWj1Sap(string jsonStr)
        {
            StringBuilder sql = new StringBuilder();
            count = 0;
            try
            {
                E6Performance obj = JsonConvert.DeserializeObject<E6Performance>(jsonStr);
                foreach (E6 item in obj.Data)
                {
                    if (!string.IsNullOrEmpty(item.BOMNO) && !string.IsNullOrEmpty(item.BATTERYNO))
                    {
                        sql.Append("UPDATE V_WJ1_");
                        sql.Append(item.BOMNO);
                        sql.Append("_B7 SET E6KVALUE=");
                        if (item.E8KVALUE != null)
                        {
                            sql.Append("'");
                            sql.Append(item.E8KVALUE);
                            sql.Append("'");
                        }
                        else
                        {
                            sql.Append("NULL");
                        }
                        sql.Append(",E8KVALUE=");
                        if (item.E8KVALUE != null)
                        {
                            sql.Append("'");
                            sql.Append(item.E8KVALUE);
                            sql.Append("'");
                        }
                        else
                        {
                            sql.Append("NULL");
                        }
                        sql.Append(",E6LEVEL='");
                        if (item.STATUS =="Y.")
                        {
                            sql.Append(item.E6LEVEL);
                        }
                        else
                        {
                            sql.Append(item.STATUS.Split('.')[1]);
                        }
                        sql.Append("',E6TESTTIME2='");
                        sql.Append(item.E6TESTTIME2);
                        sql.Append("',E6VOLTAGE2='");
                        sql.Append(item.E6VOLTAGE2);
                        sql.Append("',E6RESISTANCE2='");
                        sql.Append(item.E6RESISTANCE2);
                        sql.Append("',E6DELTAV='");
                        sql.Append(item.E6DELTAV);
                        sql.Append("',E6DELTAT='");
                        sql.Append(item.E6DELTAT);
                        sql.Append("' WHERE BATTERYNO='");
                        sql.Append(item.BATTERYNO);
                        sql.Append("';");
                    }
                    ++count;
                    if (count % runCount == 0)
                    {
                        sqlStr.Add(sql.ToString());
                        sql.Clear();
                    }
                }
                sqlStr.Add(sql.ToString());
                sql.Clear();
                using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                {
                    mConn.Open();
                    SqlTransaction tran = mConn.BeginTransaction();
                    SqlCommand mComm = new SqlCommand();
                    mComm.Connection = mConn;
                    mComm.Transaction = tran;
                    for (int i = 0; i < sqlStr.Count; ++i)
                    {
                        mComm.CommandText = sqlStr[i];
                        mComm.ExecuteNonQuery();
                    }
                    tran.Commit();
                    mConn.Close();
                }
                return 0;
            }
            catch(Exception ex)
            {
                SysLog mLog = new SysLog("UpdateRemote::UpdateE6DataFromWj1Sap => " + ex.Message);
                mLog.AddLog(sql.ToString());
                return 1;
            }
        }
        public int UpdateE5DataFromWj1Sap(string jsonStr)
        {
            StringBuilder sql = new StringBuilder();
            count = 0;
            try
            {
                E5Performance obj = JsonConvert.DeserializeObject<E5Performance>(jsonStr);
                foreach (E5 item in obj.Data)
                {
                    if (!string.IsNullOrEmpty(item.BOMNO) && !string.IsNullOrEmpty(item.BATTERYNO))
                    {
                        sql.Append("UPDATE V_WJ1_");
                        sql.Append(item.BOMNO);
                        sql.Append("_B7 SET E5LEVEL='");
                        if (item.STATUS == "Y.")
                        {
                            sql.Append(item.E5LEVEL);
                        }
                        else
                        {
                            sql.Append(item.STATUS.Split('.')[1]);
                        }
                        sql.Append("',E5TESTTIME1='");
                        sql.Append(item.E5TESTTIME1);
                        sql.Append("',E5VOLTAGE1='");
                        sql.Append(item.E5VOLTAGE1);
                        sql.Append("',E5RESISTANCE1='");
                        sql.Append(item.E5RESISTANCE1);
                        sql.Append("' WHERE BATTERYNO='");
                        sql.Append(item.BATTERYNO);
                        sql.Append("';");
                    }
                    ++count;
                    if (count % runCount == 0)
                    {
                        sqlStr.Add(sql.ToString());
                        sql.Clear();
                    }
                }
                sqlStr.Add(sql.ToString());
                sql.Clear();
                using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                {
                    mConn.Open();
                    SqlTransaction tran = mConn.BeginTransaction();
                    SqlCommand mComm = new SqlCommand();
                    mComm.Connection = mConn;
                    mComm.Transaction = tran;
                    for (int i = 0; i < sqlStr.Count; ++i)
                    {
                        mComm.CommandText = sqlStr[i];
                        mComm.ExecuteNonQuery();
                    }
                    tran.Commit();
                    mConn.Close();
                }
                return 0;
            }
            catch (Exception ex)
            {
                SysLog mLog = new SysLog("UpdateRemote::UpdateE5DataFromWj1Sap => " + ex.Message);
                mLog.AddLog(sql.ToString());
                return 1;
            }
        }

        public int UpdateE3DataFromWj1Sap(string jsonStr)
        {
            
                StringBuilder sqlE3 = new StringBuilder();
                //StringBuilder sqlE7 = new StringBuilder();
            double result = 0;
            double result2 = 0;
            try
            {
                E3Performance obj = JsonConvert.DeserializeObject<E3Performance>(jsonStr);
                foreach (E3 item in obj.Data)
                {
                    if (!string.IsNullOrEmpty(item.BOMNO) && !string.IsNullOrEmpty(item.BATTERYNO))
                    {
                        /*--------------------------------- B3表 ------------------------------*/
                        sqlE3.Append("UPDATE V_WJ1_");
                        sqlE3.Append(item.BOMNO);
                        sqlE3.Append("_B3 ");
                        sqlE3.Append("SET E13EQUNO='");
                        sqlE3.Append(item.E13EQUNO.ToString());
                        sqlE3.Append("',E13ENDTIME='");
                        sqlE3.Append(item.E13ENDTIME.ToString());
                        sqlE3.Append("',E13OUT_CAPACITY2='");
                        ////////////////////////添加补偿值////////////////////////
                        double.TryParse(item.E13OUT_CAPACITY2, out result);
                        foreach (string key in ConfigurationManager.AppSettings.Keys)
                        {
                            if (key.IndexOf("E13OUT_CAPACITY2") > -1)
                            {
                                if (key.Split('|')[1] == item.BOMNO)
                                {
                                    double.TryParse(ConfigurationManager.AppSettings[key], out result2);
                                }
                            }
                        }
                        sqlE3.Append((result + result2).ToString());
                        ////////////////////////添加补偿值////////////////////////
                        sqlE3.Append("',E13END_VOLTAGE='");
                        sqlE3.Append(item.E13END_VOLTAGE.ToString());
                        sqlE3.Append("',E13AREANO='");
                        sqlE3.Append(item.E13AREANO.ToString());
                        sqlE3.Append("',E13POSNO='");
                        sqlE3.Append(item.E13POSNO.ToString());
                        sqlE3.Append("',E13LEVEL='");
                        sqlE3.Append(item.E13LEVEL.ToString());
                        sqlE3.Append("',E13START_VOLTAGE='");
                        sqlE3.Append(item.E13START_VOLTAGE.ToString());
                        sqlE3.Append("' WHERE BATTERYNO='");
                        sqlE3.Append(item.BATTERYNO);
                        sqlE3.Append("';");
                        /*--------------------------------- B7表 ------------------------------*/
                        //sqlE7.Append("UPDATE V_WJ1_");
                        //sqlE7.Append(item.BOMNO);
                        //sqlE7.Append("_B7 ");
                        //sqlE7.Append("SET E13ENDTIME='");
                        //sqlE7.Append(item.E13ENDTIME.ToString());
                        //sqlE7.Append("',E13OUT_CAPACITY2='");
                        //sqlE7.Append((result + result2).ToString());
                        //sqlE7.Append("',E13LEVEL='");
                        //sqlE7.Append(item.E13LEVEL.ToString());
                        //sqlE7.Append("' WHERE BATTERYNO='");
                        //sqlE7.Append(item.BATTERYNO);
                        //sqlE7.Append("';");
                        sqlE3.Append("UPDATE V_WJ1_");
                        sqlE3.Append(item.BOMNO);
                        sqlE3.Append("_B7 ");
                        sqlE3.Append("SET E13ENDTIME='");
                        sqlE3.Append(item.E13ENDTIME.ToString());
                        sqlE3.Append("',E13OUT_CAPACITY2='");
                        sqlE3.Append((result + result2).ToString());
                        sqlE3.Append("',E13LEVEL='");
                        sqlE3.Append(item.E13LEVEL.ToString());
                        sqlE3.Append("' WHERE BATTERYNO='");
                        sqlE3.Append(item.BATTERYNO);
                        sqlE3.Append("';");

                    }
                }
                using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                {
                    mConn.Open();
                    SqlTransaction tran = mConn.BeginTransaction();
                    SqlCommand mComm = new SqlCommand(sqlE3.ToString(), mConn, tran);
                    mComm.ExecuteNonQuery();
                    tran.Commit();
                    //mComm.CommandText = sqlE7.ToString();
                    //mComm.ExecuteNonQuery();
                    //tran.Commit();
                    mConn.Close();
                }
                return 0;
            }
            catch (Exception ex)
            {
                SysLog mLog = new SysLog("UpdateRemote::UpdateE3DataFromWj1Sap => " + ex.Message);
                mLog.AddLog(sqlE3.ToString());
                return 1;
            }
        }
        public int UpdateE16DataFromWj1Sap(string jsonStr)
        {
            StringBuilder sqlE7 = new StringBuilder();
            try
            {
                E16Performance obj = JsonConvert.DeserializeObject<E16Performance>(jsonStr);
                foreach (E16 item in obj.Data)
                {
                    if (!string.IsNullOrEmpty(item.BOMNO) && !string.IsNullOrEmpty(item.BATTERYNO))
                    {
                        /*--------------------------------- B7表 ------------------------------*/
                        sqlE7.Append("UPDATE V_WJ1_");
                        sqlE7.Append(item.BOMNO);
                        sqlE7.Append("_B7 ");
                        sqlE7.Append("SET E16FLAG='");
                        if (item.STATUS == "Y.")
                        {
                            sqlE7.Append("合格");
                        }
                        else
                        {
                            sqlE7.Append(item.STATUS.Split('.')[1]);
                        }
                        sqlE7.Append("',E16TESTTIME='");
                        sqlE7.Append(item.E16TESTTIME.ToString());
                        sqlE7.Append("',E16BATTERYWEIGH='");
                        sqlE7.Append(item.E16BATTERYWEIGH.ToString());
                        sqlE7.Append("',E16CHANO='");
                        sqlE7.Append(item.E16CHANO.ToString());
                        sqlE7.Append("',E16JYLWEIGH='");
                        sqlE7.Append(item.E16JYLWEIGH.ToString());
                        sqlE7.Append("' WHERE BATTERYNO='");
                        sqlE7.Append(item.BATTERYNO);
                        sqlE7.Append("';");
                    }
                }
                using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                {
                    mConn.Open();
                    SqlTransaction tran = mConn.BeginTransaction();
                    SqlCommand mComm = new SqlCommand(sqlE7.ToString(), mConn, tran);
                    mComm.ExecuteNonQuery();
                    tran.Commit();
                    mConn.Close();
                }
                return 0;
            }
            catch (Exception ex)
            {
                SysLog mLog = new SysLog("UpdateRemote::UpdateE16DataFromWj1Sap => " + ex.Message);
                mLog.AddLog(sqlE7.ToString());
                return 1;
            }
        }

        /// <summary>
        /// 将设备上传的数据（原始JSON）保存到 RECIEVED_INFO 中
        /// </summary>
        /// <param name="jsonStr">原始JSON</param>
        /// <returns>true：成功；false：失败</returns>
        public bool RecievedOCV(string jsonStr)
        {
            if (!string.IsNullOrEmpty(jsonStr))
            {
                using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PROCESS_DATA"].ConnectionString))
                {
                    try
                    {
                        mConn.Open();
                        SqlCommand mComm = new SqlCommand("INSERT INTO RECIEVED_INFO (RECIEVED_INFO) VALUES ('" + jsonStr + "');", mConn);
                        mComm.ExecuteNonQuery();
                        mConn.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        SysLog log = new SysLog(ex.Message);
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        public BatteryPerformance SavePerformance(string jsonStr)
        {
            if (!string.IsNullOrEmpty(jsonStr))
            {
                StringBuilder sql = new StringBuilder();
                try
                {
                    BatteryPerformance obj = JsonConvert.DeserializeObject<BatteryPerformance>(jsonStr);
                    for (int i = 0; i < obj.Data.Count; ++i)
                    {
                        switch (obj.DTYPE)
                        {
                            case "OCV1":
                                sql.Append(string.Format("INSERT INTO OCV1 (BOMNO,SIZENO,BATTERYNO,EQUIPMENTNO,CHANNEL,VOLTAGE,RESISTANCE,TESTDATE,LEVEL,CARRIOR,POSITION,VOLTAGE_UPPER,VOLTAGE_LOWER,RESISTANCE_UPPER,RESISTANCE_LOWER,PTYPE) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}');", obj.Data[i].BOMNO, obj.Data[i].SIZENO, obj.Data[i].BATTERYNO, obj.Data[i].EQUIPMENTNO, obj.Data[i].CHANNEL, obj.Data[i].VOLTAGE, obj.Data[i].RESISTANCE, obj.Data[i].TESTDATE, obj.Data[i].LEVEL, obj.Data[i].CARRIOR, obj.Data[i].POSITION, obj.Data[i].VOLTAGE_UPPER, obj.Data[i].VOLTAGE_LOWER, obj.Data[i].RESISTANCE_UPPER, obj.Data[i].RESISTANCE_LOWER, obj.PTYPE));
                                break;
                            case "OCV2":
                                sql.Append(string.Format("INSERT INTO OCV2 (BOMNO,SIZENO,BATTERYNO,EQUIPMENTNO,CHANNEL,VOLTAGE,RESISTANCE,DELTAV,KVALUE,TESTDATE,LEVEL,CARRIOR,POSITION,VOLTAGE_UPPER,VOLTAGE_LOWER,RESISTANCE_UPPER,RESISTANCE_LOWER,DELTAV_UPPER,DELTAV_LOWER,DELTAV_AVG,KVALUE_UPPER,KVALUE_LOWER,KVALUE_AVG,Sigma,PTYPE) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}');", obj.Data[i].BOMNO, obj.Data[i].SIZENO, obj.Data[i].BATTERYNO, obj.Data[i].EQUIPMENTNO, obj.Data[i].CHANNEL, obj.Data[i].VOLTAGE, obj.Data[i].RESISTANCE, obj.Data[i].DELTAV, obj.Data[i].KVALUE, obj.Data[i].TESTDATE, obj.Data[i].LEVEL, obj.Data[i].CARRIOR, obj.Data[i].POSITION, obj.Data[i].VOLTAGE_UPPER, obj.Data[i].VOLTAGE_LOWER, obj.Data[i].RESISTANCE_UPPER, obj.Data[i].RESISTANCE_LOWER, obj.Data[i].DELTAV_UPPER, obj.Data[i].DELTAV_LOWER, obj.Data[i].DELTAV_AVG, obj.Data[i].KVALUE_UPPER, obj.Data[i].KVALUE_LOWER, obj.Data[i].KVALUE_AVG, obj.Data[i].SIGMA, obj.PTYPE));
                                break;
                            case "OCV3":
                                sql.Append(string.Format("INSERT INTO OCV3 (BOMNO,SIZENO,BATTERYNO,EQUIPMENTNO,CHANNEL,VOLTAGE,RESISTANCE,DELTAV,KVALUE,TESTDATE,LEVEL,CARRIOR,POSITION,VOLTAGE_UPPER,VOLTAGE_LOWER,RESISTANCE_UPPER,RESISTANCE_LOWER,DELTAV_UPPER,DELTAV_LOWER,DELTAV_AVG,KVALUE_UPPER,KVALUE_LOWER,KVALUE_AVG,Sigma,PTYPE) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}');", obj.Data[i].BOMNO, obj.Data[i].SIZENO, obj.Data[i].BATTERYNO, obj.Data[i].EQUIPMENTNO, obj.Data[i].CHANNEL, obj.Data[i].VOLTAGE, obj.Data[i].RESISTANCE, obj.Data[i].DELTAV, obj.Data[i].KVALUE, obj.Data[i].TESTDATE, obj.Data[i].LEVEL, obj.Data[i].CARRIOR, obj.Data[i].POSITION, obj.Data[i].VOLTAGE_UPPER, obj.Data[i].VOLTAGE_LOWER, obj.Data[i].RESISTANCE_UPPER, obj.Data[i].RESISTANCE_LOWER, obj.Data[i].DELTAV_UPPER, obj.Data[i].DELTAV_LOWER, obj.Data[i].DELTAV_AVG, obj.Data[i].KVALUE_UPPER, obj.Data[i].KVALUE_LOWER, obj.Data[i].KVALUE_AVG, obj.Data[i].SIGMA, obj.PTYPE));
                                break;
                            case "OCV4":
                                sql.Append(string.Format("INSERT INTO OCV4 (BOMNO,SIZENO,BATTERYNO,EQUIPMENTNO,CHANNEL,VOLTAGE,RESISTANCE,TESTDATE,LEVEL,CARRIOR,POSITION,VOLTAGE_UPPER,VOLTAGE_LOWER,RESISTANCE_UPPER,RESISTANCE_LOWER,PTYPE) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}');", obj.Data[i].BOMNO, obj.Data[i].SIZENO, obj.Data[i].BATTERYNO, obj.Data[i].EQUIPMENTNO, obj.Data[i].CHANNEL, obj.Data[i].VOLTAGE, obj.Data[i].RESISTANCE, obj.Data[i].TESTDATE, obj.Data[i].LEVEL, obj.Data[i].CARRIOR, obj.Data[i].POSITION, obj.Data[i].VOLTAGE_UPPER, obj.Data[i].VOLTAGE_LOWER, obj.Data[i].RESISTANCE_UPPER, obj.Data[i].RESISTANCE_LOWER, obj.PTYPE));
                                break;
                            default:
                                throw new Exception("SavePerformance() ==> BatteryPerformance.DTYPE 出现不支持类型：" + obj.DTYPE);
                        }
                    }
                    using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PROCESS_DATA"].ConnectionString))
                    {
                        mConn.Open();
                        SqlTransaction tran = mConn.BeginTransaction();
                        SqlCommand mComm = new SqlCommand(sql.ToString(), mConn, tran);
                        mComm.ExecuteNonQuery();
                        tran.Commit();
                        mConn.Close();
                    }
                    return obj;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public int UpdatePerformance(BatteryPerformance BatteryList)
        {
            if (BatteryList.Data.Count > 0)
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder sql7 = new StringBuilder();
                StringBuilder sql8 = new StringBuilder();
                try
                {
                    foreach (OCV obj in BatteryList.Data)
                    {
                        switch (BatteryList.DTYPE)
                        {
                            case "OCV1":
                                sql.Append(string.Format("UPDATE V_WJ1_{10}_B4 SET E5EQUNO = '{1}',E5TESTTIME1 = '{2}',E5VOLTAGE1 = '{3}',E5RESISTANCE1 = '{4}',E5PANELNO1 = '{5}',E5LEVEL = '{6}',E5REMARKS = '{7}',UPDATER = '{8}',UPDATETIME = {9} WHERE BATTERYNO = '{0}'; ", obj.BATTERYNO, obj.EQUIPMENTNO, obj.TESTDATE, obj.VOLTAGE, obj.RESISTANCE, obj.CHANNEL, obj.LEVEL, "VOLTAGE: [ " + obj.VOLTAGE_LOWER + "," + obj.VOLTAGE_UPPER + " ]; RESISTANCE: [ " + obj.RESISTANCE_LOWER + "," + obj.RESISTANCE_LOWER + " ]", "AutoUpload", "GETDATE()", obj.BOMNO));
                                sql7.Append(string.Format("UPDATE V_WJ1_{7}_B7 SET E5TESTTIME1 = '{1}',E5VOLTAGE1 = '{2}',E5RESISTANCE1 = '{3}',UPDATER = '{4}',UPDATETIME = {5},E5LEVEL = '{6}' WHERE BATTERYNO = '{0}';", obj.BATTERYNO, obj.TESTDATE, obj.VOLTAGE, obj.RESISTANCE, "AutoUpload", "GETDATE()", obj.LEVEL, obj.BOMNO));
                                sql8.Append(string.Format("UPDATE V_WJ1_{6}_B8 SET TUOPANNO = '{1}',POSTION = '{2}',STATE = '{3}',UPDATER = '{4}',UPDATETIME = {5} WHERE BATTERYNO = '{0}';", obj.BATTERYNO, obj.CARRIOR, obj.CHANNEL, obj.LEVEL == "合格" ? "1" : "0", "AutoUpload", "GETDATE()", obj.BOMNO));
                                break;
                            case "OCV2":
                                sql.Append(string.Format("UPDATE V_WJ1_{15}_B4 SET E6EQUNO = '{1}',E6TESTTIME2 = '{2}',E6VOLTAGE2 = '{3}',E6RESISTANCE2 = '{4}',E6PANELNO2 = '{5}',E6LEVEL = '{6}',E6REMARKS = '{7}',E6DELTAV = '{8}',UPDATER = '{9}',UPDATETIME = {10},E6DELTAT = '{11}',E6KVALUE = '{12}',E6AVGKVALUE = '{13}',E6AVGDELTAV = '{14}' WHERE BATTERYNO = '{0}';", obj.BATTERYNO, obj.EQUIPMENTNO, obj.TESTDATE, obj.VOLTAGE, obj.RESISTANCE, obj.CHANNEL, obj.LEVEL, "VOLTAGE: [ " + obj.VOLTAGE_LOWER + "," + obj.VOLTAGE_UPPER + " ]; RESISTANCE: [ " + obj.RESISTANCE_LOWER + "," + obj.RESISTANCE_LOWER + " ]", obj.DELTAV, "AutoUpload", "GETDATE()", obj.DELTAT, obj.KVALUE, obj.KVALUE_AVG, obj.DELTAV_AVG, obj.BOMNO));
                                sql7.Append(string.Format("UPDATE V_WJ1_{7}_B7 SET E6TESTTIME2 = '{1}',E6VOLTAGE2 = '{2}',E6RESISTANCE2 = '{3}',UPDATER = '{4}',UPDATETIME = {5},E6LEVEL = '{6}' WHERE BATTERYNO = '{0}';", obj.BATTERYNO, obj.TESTDATE, obj.VOLTAGE, obj.RESISTANCE, "AutoUpload", "GETDATE()", obj.LEVEL, obj.BOMNO));
                                sql8.Append(string.Format("UPDATE V_WJ1_{6}_B8 SET TUOPANNO = '{1}',POSTION = '{2}',STATE = '{3}',UPDATER = '{4}',UPDATETIME = {5} WHERE BATTERYNO = '{0}';", obj.BATTERYNO, obj.CARRIOR, obj.CHANNEL, obj.LEVEL == "合格" ? "1" : "0", "AutoUpload", "GETDATE()", obj.BOMNO));
                                break;
                            case "OCV3":
                                break;
                            case "OCV4":
                                break;
                            default:
                                throw new Exception("SavePerformance() ==> BatteryPerformance.DTYPE 出现不支持类型：" + BatteryList.DTYPE);
                        }
                    }
                    using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                    {
                        mConn.Open();
                        SqlTransaction tran = mConn.BeginTransaction();
                        SqlCommand mComm = new SqlCommand(sql.ToString(), mConn, tran);
                        try
                        {
                            SysLog log = new SysLog(mComm.CommandText);
                            mComm.ExecuteNonQuery();

                            log.AddLog(sql7.ToString());
                            mComm.CommandText = sql7.ToString();
                            mComm.ExecuteNonQuery();

                            log.AddLog(sql8.ToString());
                            mComm.CommandText = sql8.ToString();
                            mComm.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch(Exception ex)
                        {
                            SysLog log = new SysLog("SQL 异常，开始回滚. " + ex.Message + "SQL Statement: " + mComm.CommandText);
                            tran.Rollback();
                            mConn.Close();
                            return 0;
                        }
                    }
                    return BatteryList.Data.Count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
            else
            {
                SysLog log = new SysLog("没有可更新的数据.");
                return 0;
            }
        }

        public int UpdateHipot(string bomno,string barcode,string hipot)
        {
            using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
            {
                try
                {
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open)
                    {
                        throw new Exception("UpdateHipot => Failed to connect to the database.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO V_WJ1_{0}_B8 (BOMNO,BATTERYNO,FDSIZENO,CREATTIME) VALUES ('{0}','{1}','{2}','{3}');", bomno, barcode, hipot, DateTime.Now.ToString("s")));
                    return comm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
    }
}