using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Configuration;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using LSMES_5ANEW_PLUS.Business;


namespace LSMES_5ANEW_PLUS.SRC.Common
{
    /// <summary>
    /// MESWebServiceGetCode 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class MESWebServiceGetCode : System.Web.Services.WebService
    {
        #region 变量常量信息
        private string RET_ERROR = "ERROR";

        //输入的电池编号不存在时:返回"NULL"字符串
        private string RET_NULL = "NULL";

        //输入电池编号或批次号为空
        private string TRAY_NULL = "电池编号或批次号{0}不存在！";
        //电池信息为空
        private string BATTERY_NULL = "电池编号或批次号:{0}对应电池信息为空！";
        //数据库连接失败
        private string DATABASE_ERROR = "数据库链接失败";
        //多个型号电池信息
        private string MULTIBOM = "电池编号或批次号{0}有{1}个型号的电池！";

        //分选标准不存在
        private string STANDARD_NULL = "电池编号或批次号号:{0}与设备:{1}没有对应的分档标准！";
        //输入电池编号或批次号为空
        private string BOMNO_NULL = "电池型号{0}或流水线名称{1}不存在！";

        private string EMPTY_SAP = "IS_NULL=\"Y\"";

        #endregion
        [WebMethod(Description = "ZY_GetPara", EnableSession = true)]
        public XmlDocument ZY_GetPara(string code)
        {
            //获取调用Webservice客户端的IP地址
            string strIpAddress = this.Context.Request.UserHostAddress;
            XmlDocument doc = new XmlDocument();
            doc = getXmlResultForZY_GetParaOfSAP_ME(code);
            if (doc != null)
            {
                return doc;
            }
            return getXmlResultForZY_GetPara(code, strIpAddress);
        }
        [WebMethod(Description = "ZY_GetData", EnableSession = true)]
        public XmlDocument ZY_GetData(string code, string node, string operator1, string equipmentid)//,此XML主要用于ADO   
        {
            //获取调用Webservice客户端的IP地址
            string strIpAddress = this.Context.Request.UserHostAddress;
            XmlDocument doc = new XmlDocument();
            doc = getXmlResultForZY_GetDataOfSAP_ME(code, node, operator1, strIpAddress);
            if (doc != null)
            {
                return doc;
            }
            return getXmlResultForZY_GetData(code, node, operator1, equipmentid, strIpAddress);
        }
        [WebMethod(Description = "getXmlResultForZY_GetParaNew", EnableSession = true)]
        public XmlDocument getXmlResultForZY_GetParaNew(string code)
        {
            //获取调用Webservice客户端的IP地址
            string strIpAddress = this.Context.Request.UserHostAddress;
            XmlDocument doc = new XmlDocument();
            doc = getXmlResultForZY_GetParaNewOfSAP_ME(code, strIpAddress);
            if (doc != null)
            {
                return doc;
            }
            return getXmlResultForZY_GetParaNew1(code, strIpAddress);
        }
        public XmlDocument getXmlResultForZY_GetParaOfSAP_ME(string code)
        {
            if (string.IsNullOrEmpty(code.Trim()))
            {
                string ErrMsg2 = string.Format(BATTERY_NULL, code);
                this.WriteFileLog(null, ErrMsg2);
                return null;
            }
            try
            {
                string result = HttpGet(ConfigurationManager.AppSettings["ZY_GetPara"], "ZY_GetPara", "code=" + code);
                if (result.Contains(EMPTY_SAP))
                {
                    return null;
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);
                    return doc;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        [WebMethod(Description = "IR/OCV针床", EnableSession = true)]
        public XmlDocument GetIROCV(string code)//,此XML主要用于ADO   
        {
            //获取调用Webservice客户端的IP地址
            string strIpAddress = this.Context.Request.UserHostAddress;
            XmlDocument doc = new XmlDocument();
            doc = getXmlResultIROCVOfSAP_ME(code, strIpAddress);
            if (doc!= null)
            {
                return doc;
            }
            return getXmlResultIROCV(code, strIpAddress);
        }
        [WebMethod(Description = "下载批次数据", EnableSession = true)]
        public XmlDocument LoadValidData(string code, string nodename)//,此XML主要用于ADO   
        {
            //获取调用Webservice客户端的IP地址
            string strIpAddress = this.Context.Request.UserHostAddress;
            XmlDocument doc = new XmlDocument();
            doc = LoadvalidDataOfSAP_ME(code, nodename, strIpAddress);
            if (doc != null)
            {
                return doc;
            }
            return LoadvalidData(code, nodename, strIpAddress);
        }
        [WebMethod(Description = "开始生产前验证", EnableSession = true)]
        public XmlDocument LoadValidDataNew(string code, string nodename, string userID, string EQUIPMENTNO, string MAInfo)//,此XML主要用于ADO   
        {
            //获取调用Webservice客户端的IP地址
            string strIpAddress = this.Context.Request.UserHostAddress;
            XmlDocument doc = new XmlDocument();
            doc = LoadvalidDataOfSAP_ME(code, nodename, strIpAddress);
            if (doc != null)
            {
                return doc;
            }
            return LoadvalidData(code, nodename, strIpAddress);
        }
        [WebMethod(Description = "杭可/擎天分容", EnableSession = true)]
        public XmlDocument GetFenRong(string code)//,此XML主要用于ADO   
        {
            //获取调用Webservice客户端的IP地址
            string strIpAddress = this.Context.Request.UserHostAddress;
            XmlDocument doc = new XmlDocument();
            doc = GetXmlResultForFenRongOfSAP_ME(code.ToUpper(), strIpAddress);
            if (doc != null)
            {
                return doc;
            }
            return getXmlResultForFenRong(code.ToUpper(), strIpAddress);
        }
        [WebMethod(Description = "OCV分档", EnableSession = true)]
        public XmlDocument GetOCV_FENDANG(string BOMNO, string LineName)//,此XML主要用于ADO   
        {
            //获取调用Webservice客户端的IP地址
            string strIpAddress = this.Context.Request.UserHostAddress;
            XmlDocument doc = new XmlDocument();
            doc = getXmlResultForOCV_FENDDANGOfSAP_ME(BOMNO, LineName, strIpAddress);
            if (doc != null)
            {
                return doc;
            }
            return getXmlResultForOCV_FENDDANG(BOMNO, LineName, strIpAddress);
        }
        [WebMethod(Description = "OCV分档属性取得", EnableSession = true)]
        public XmlDocument GetOCV_FENDANG_Attributes(string code)//,此XML主要用于ADO   
        {
            //获取调用Webservice客户端的IP地址
            string strIpAddress = this.Context.Request.UserHostAddress;
            XmlDocument doc = new XmlDocument();
            doc = getXmlResultForOCV_FENDDANG_AttributesOfSAP_ME(code, strIpAddress);
            if (doc != null)
            {
                return doc;
            }
            return getXmlResultForOCV_FENDDANG_Attributes(code, strIpAddress);
        }


        /// <summary>返回错误的xml文件
        /// </summary>
        /// <param name="checkValue"></param>
        private XmlDocument getXml(string checkValue)
        {

            XmlDocument levelSet = new XmlDocument();
            XmlElement root = levelSet.CreateElement("Root");
            levelSet.AppendChild(root);

            XmlNode rootNode = levelSet.SelectSingleNode("Root");
            XmlElement subNode = levelSet.CreateElement("A");
            subNode.SetAttribute("SIZENO", checkValue);
            subNode.SetAttribute("TUOPANNO", checkValue);
            subNode.SetAttribute("POSTIONNO", checkValue);
            subNode.SetAttribute("BATTERYNO", checkValue);
            rootNode.AppendChild(subNode);
            return levelSet;

        }

        private XmlDocument getResult(string sql)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception(DATABASE_ERROR);
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(comm);
                    XmlDocument levelSet = new XmlDocument();
                    XmlReader reader = comm.ExecuteXmlReader();
                    XmlElement root = levelSet.CreateElement("Root");
                    levelSet.AppendChild(root);
                    XmlNode node = levelSet.ReadNode(reader);
                    int i = 0;
                    while (node != null)
                    {
                        i++;
                        root.AppendChild(node);
                        node = levelSet.ReadNode(reader);
                    }
                    if (i == 0)
                    {
                        levelSet = null;
                    }
                    reader.Close();
                    return levelSet;
                }
                catch (Exception ex)
                {
                    WriteFileLog(null, ex.Message);
                    return null;
                }
            }

        }

        /// <summary>记录获取电池XML文档的日志信息
        /// 
        /// </summary>
        /// <param name="IPAddress"></param>
        /// <param name="TrayNo"></param>
        private void WriteFileLog(string IPAddress, string TrayNo)
        {
            string outDate = DateTime.Today.ToString("yyyyMMdd");
            //日志路径
            string XmlLogPath = @ConfigurationManager.ConnectionStrings["LogPath"].ConnectionString + outDate + @"\";

            //创建文件夹

            if (!Directory.Exists(XmlLogPath))
            {
                Directory.CreateDirectory(XmlLogPath);
            }

            string XmlLogDate = DateTime.Today.ToString("yyyyMMdd");
            string XmlLogFile = XmlLogPath + XmlLogDate + ".log";
            StreamWriter sw = new StreamWriter(XmlLogFile, true, Encoding.Default);
            sw.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + IPAddress + "," + TrayNo);
            sw.WriteLine();
            sw.Close();
            return;
        }

        private DataSet getRealFlow(string code)//,此XML主要用于ADO   
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception(DATABASE_ERROR);
                    }
                    string sql = null;
                    if (code.Length == 13 || code.Length == 14)
                    {
                        sql = string.Format("SELECT T.*,D.OTHER_NAME FROM T_REAL_FLOW_BATTERY T INNER JOIN M_DEPARTMENT D ON T.PIPELINE = D.DEPARTMENTNAME WHERE BATTERYNO = '{0}'", code);
                    }
                    else
                    {
                        sql = string.Format("SELECT T.*,D.OTHER_NAME FROM T_REAL_FLOW T INNER JOIN M_DEPARTMENT D ON T.PIPELINE = D.DEPARTMENTNAME WHERE BOMNO_SIZENO = '{0}'", code);
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(comm);
                    DataSet set = new DataSet();
                    adapter.Fill(set);
                    return set;
                }
                catch (Exception ex)
                {
                    WriteFileLog(null, ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取电池 / 批次基本信息
        /// </summary>
        /// <param name="code">电池 / 批次</param>
        /// <param name="nodename">工序名称</param>
        /// <returns></returns>
        private DataTable getBaseInfo(string code,string nodename)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception(DATABASE_ERROR);
                    }
                    SqlCommand comm = null;
                    // 电池码号 & 工序名称
                    if (code.Length == 13 || code.Length == 14)
                    {
                        comm = new SqlCommand(string.Format("SELECT T.BOMNO,T.SIZENO,T.BOMNO_SIZENO,D.OTHER_NAME,TT.NOW_NODEID,TT.NOW_NODENAME,TT.FLOW_NAME,PIPELINE FROM T_REAL_FLOW_BATTERY T INNER JOIN M_DEPARTMENT D ON T.PIPELINE = D.DEPARTMENTNAME AND T.BATTERYNO = '{0}' INNER JOIN T_REAL_FLOW TT ON TT.NOW_NODENAME = '{1}' AND T.BOMNO_SIZENO = TT.BOMNO_SIZENO;", code, nodename), conn);
                    }
                    // 批次编号 & 工序名称
                    else
                    {
                        comm = new SqlCommand(string.Format("SELECT T.*,D.OTHER_NAME FROM T_REAL_FLOW T INNER JOIN M_DEPARTMENT D ON T.PIPELINE = D.DEPARTMENTNAME WHERE BOMNO_SIZENO = '{0}' AND NOW_NODENAME = '{1}';", code, nodename.ToUpper()), conn);
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(comm);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    WriteFileLog(null, ex.Message);
                    return null;
                }
            }
        }

        public int SaveNodeAction(string pipeLine, string BOMNO, string sizeNO, string flow, string node, string userID, short action)
        {
            string sql = "INSERT INTO T_NODE_ACTION (PipeLineName,BOMNO,SizeNO,FlowName,NodeName,Action,AddTime,AddUserID) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',GETDATE(),'{6}');";
            if (action == 1 || action == 3)
            {
                sql += " UPDATE T_NODE_BUFFER SET STATE = 2 WHERE PIPELINENAME = '{0}' AND FLOWNAME = '{3}' AND BOMNO = '{1}' AND SIZENO = '{2}';";
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception(DATABASE_ERROR);
                    }
                    SqlCommand comm = new SqlCommand(string.Format(sql, pipeLine, BOMNO, sizeNO, flow, node, action, userID), conn);
                    return comm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    WriteFileLog(null, ex.Message);
                    return 0;
                }
            }

        }
        /// <summary>检索返回xml的结果 IR/OCV针床
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="IPAddress"></param>
        /// <returns></returns>
        private XmlDocument getXmlResultIROCV(string code, string IPAddress)
        {
            try
            {
                //string checkVlaue = checkInputCode(code);
                if (string.IsNullOrEmpty(code.Trim()))
                {
                    this.WriteFileLog(IPAddress, string.Format(TRAY_NULL, code));
                    return getXml(RET_NULL);
                }
                DataSet ds1 = getRealFlow(code);

                if (ds1 == null || ds1.Tables[0].Rows.Count <= 0)
                {
                    string ErrMsg2 = string.Format(BATTERY_NULL, code);
                    this.WriteFileLog(IPAddress, ErrMsg2);
                    return getXml(RET_NULL);
                }
                string linename = ds1.Tables[0].Rows[0]["OTHER_NAME"].ToString();
                string bomno = ds1.Tables[0].Rows[0]["BOMNO"].ToString();
                //string tableB7 = ServerCommon.getTableName(linename, bomno, "B7");
                //string tableB8 = ServerCommon.getTableName(linename, bomno, "B8");
                string tableB7 = "V_" + linename + "_" + bomno + "_B7";
                string tableB8 = "V_" + linename + "_" + bomno + "_B8";
                //SqlParameter[] paras ={
                //      SqlHelper.MakeInPara("@CODE",SqlDbType.VarChar,500,code)
                //      };

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT                                                      ");
                sb.Append("    *                                                       ");
                sb.Append("FROM                                                        ");
                sb.Append("    (                                                       ");
                sb.Append("        SELECT                                              ");
                sb.Append("            A.SIZENO                                        ");
                sb.Append("            ,A.TUOPANNO                                     ");
                sb.Append("            ,A.BATTERYNO                                    ");
                sb.Append("            ,A.POSTION                                      ");
                sb.Append("            ,B.ORDERNO                                      ");
                sb.Append("            ,B.BOMNO                                        ");
                sb.Append("            ,B.E1DIFF                                       ");
                sb.Append("            ,B.E1STATE                                      ");
                sb.Append("            ,B.E5VOLTAGE1                                   ");
                sb.Append("            ,B.E5RESISTANCE1                                ");
                sb.Append("            ,B.E5TESTTIME1                                  ");
                sb.Append("            ,B.E5LEVEL                                      ");
                sb.Append("            ,B.E6VOLTAGE2                                   ");
                sb.Append("            ,B.E6RESISTANCE2                                ");
                sb.Append("            ,B.E6TESTTIME2                                  ");
                sb.Append("            ,B.E6DELTAV                                     ");
                sb.Append("            ,B.E6LEVEL                                      ");
                sb.Append("            ,B.E7DELTAV2                                    "); //20150604 add
                sb.Append("            ,B.E7VOLTAGE3                                   ");
                sb.Append("            ,B.E7RESISTANCE3                                ");
                sb.Append("            ,B.E7TESTTIME3                                  ");
                sb.Append("            ,B.E7LEVEL                                      ");
                sb.Append("            ,B.E8DELTAV                                     ");//20150604 add
                sb.Append("            ,B.E8VOLTAGE1                                   ");
                sb.Append("            ,B.E8RESISTANCE1                                ");
                sb.Append("            ,B.E8TESTTIME1                                  ");

                sb.Append("            ,B.E8LEVEL                                      ");//20150604 add

                sb.Append("            ,B.E9VOLTAGE2                                   ");
                sb.Append("            ,B.E9RESISTANCE2                                ");
                sb.Append("            ,B.E9TESTTIME2                                  ");
                sb.Append("            ,B.E9LEVEL                                      ");//20150604 add

                sb.Append("            ,B.E9DELTAV2                                    ");//20150604 add

                sb.Append("            ,B.E10VOLTAGE1                                  ");
                sb.Append("            ,B.E10RESISTANCE1                               ");
                sb.Append("            ,B.E10TESTTIME1                                 ");
                sb.Append("            ,B.E14WEIGHT                                    ");
                sb.Append("            ,B.E14TESTTIME                                  ");
                sb.Append("            ,B.E4OUT_CAPACITY2                              ");
                sb.Append("            ,B.E4ENDTIME                                    ");
                sb.Append("            ,B.E13OUT_CAPACITY2                             ");
                sb.Append("            ,B.E13LEVEL                                     ");
                sb.Append("            ,B.E13ENDTIME                                   ");
                sb.Append("            ,B.LEVELNAME                                    ");
                sb.Append("            ,B.E16FLAG                                      ");//20140425 add
                sb.Append("        FROM                                                ");
                sb.Append("            " + tableB7 + "                                       ");
                sb.Append("            B                                               ");
                sb.Append("            ," + tableB8 + " A                                   ");
                sb.Append("        WHERE                                               ");
                sb.Append("                B.BATTERYNO = A.BATTERYNO                   ");
                sb.Append("            AND A.BOMNO_SIZENO = '{0}'                      ");
                sb.Append("            AND A.STATE = '1'                               ");
                sb.Append("    )                                                       ");
                sb.Append("    A                                                       ");
                sb.Append("ORDER BY                                                    ");
                sb.Append("    A.TUOPANNO                                              ");
                sb.Append("    ,A.POSTION FOR    XML    AUTO                           ");

                XmlDocument levelSet = getResult(string.Format(sb.ToString(), code));


                if (levelSet != null)
                {
                    this.WriteFileLog(IPAddress, string.Format(sb.ToString(), code));
                    return levelSet;
                }
                else
                {
                    string ErrMsg2 = string.Format(BATTERY_NULL, code);
                    this.WriteFileLog(IPAddress, ErrMsg2);
                    return getXml(RET_NULL);
                }
            }
            catch (Exception ex)
            {
                this.WriteFileLog(IPAddress, ex.Message);
                return getXml(RET_ERROR);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="IPAddress"></param>
        /// <returns></returns>
        private XmlDocument getXmlResultIROCVOfSAP_ME(string code, string IPAddress)
        {
            if (string.IsNullOrEmpty(code.Trim()))
            {
                string ErrMsg2 = string.Format(BATTERY_NULL, code);
                this.WriteFileLog(IPAddress, ErrMsg2);
                return null;
            }
            try
            {
                // SAP ME 没有实现 getXmlResultIROCV 接口
                string result = HttpGet(ConfigurationManager.AppSettings["GetIROCV"], "GetIROCV", "code=" + code);
                if (result.Contains(EMPTY_SAP))
                {
                    return null;
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);
                    return doc;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>检索返回xml的结果
        /// </summary>
        /// <param name="code">电池编号或批次号</param>
        /// <param name="nodename">工序名称</param>
        /// <returns></returns>
        private XmlDocument LoadvalidData(string code, string nodename, string IPAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(code.Trim()) || string.IsNullOrEmpty(nodename.Trim()))
                {
                    this.WriteFileLog(IPAddress, string.Format(TRAY_NULL, code));
                    return getXml(RET_NULL);
                }
                DataTable dt = getBaseInfo(code, nodename);
                if (dt==null || dt.Rows.Count == 0)
                {
                    this.WriteFileLog(IPAddress, string.Format(TRAY_NULL, code));
                    return getXml(RET_NULL);
                }
                string b1 = "V_" + dt.Rows[0]["OTHER_NAME"].ToString() + "_" + dt.Rows[0]["BOMNO"].ToString() + "_B1";
                string b2 = "V_" + dt.Rows[0]["OTHER_NAME"].ToString() + "_" + dt.Rows[0]["BOMNO"].ToString() + "_B2";
                string b7 = "V_" + dt.Rows[0]["OTHER_NAME"].ToString() + "_" + dt.Rows[0]["BOMNO"].ToString() + "_B7";
                string be = "V_" + dt.Rows[0]["OTHER_NAME"].ToString() + "_" + dt.Rows[0]["BOMNO"].ToString() + "_BE";
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(" SELECT        *    FROM (                                                       ");
                //20150925
                sb.AppendLine(" SELECT                                                                          ");
                sb.AppendLine("  B1.SIZENO                                                                      ");
                sb.AppendLine(" ,B1.BATTERYNO                                                                   ");
                sb.AppendLine(" ,B1.ORDERNO                                                                     ");
                sb.AppendLine(" ,B1.BOMNO                                                                       ");
                sb.AppendLine(" ,ISNULL( BE.BADTYPE , '' ) AS STATE                                             ");
                sb.AppendLine(" ,B1.E1DIFF                                                                      ");
                sb.AppendLine(" ,B1.E1STATE                                                                     ");
                sb.AppendLine(" ,E5VOLTAGE1                                                                     ");
                sb.AppendLine(" ,E5RESISTANCE1                                                                  ");
                sb.AppendLine(" ,E6VOLTAGE2                                                                     ");
                sb.AppendLine(" ,E6RESISTANCE2                                                                  ");
                sb.AppendLine(" ,E7VOLTAGE3                                                                     ");
                sb.AppendLine(" ,E7RESISTANCE3                                                                  ");
                sb.AppendLine(" ,E8VOLTAGE1                                                                     ");
                sb.AppendLine(" ,E8RESISTANCE1                                                                  ");
                sb.AppendLine(" ,E9VOLTAGE2                                                                     ");
                sb.AppendLine(" ,E9RESISTANCE2                                                                  ");
                sb.AppendLine(" ,E10VOLTAGE1                                                                    ");
                sb.AppendLine(" ,E10RESISTANCE1                                                                 ");
                sb.AppendLine(" ,E16JYLWEIGH                                                                    ");
                sb.AppendLine(" ,E16FLAG                                                                        ");
                sb.AppendLine(" ,E6DELTAV                                                                       ");
                sb.AppendLine(" ,E8DELTAV                                                                       ");
                sb.AppendLine(" ,E9DELTAV2                                                                      ");
                sb.AppendLine(" ,E13OUT_CAPACITY2                                                               ");
                sb.AppendLine(" ,E13LEVEL                                                                       ");
                sb.AppendLine(" ,E6KVALUE                                                                       ");
                sb.AppendLine(" ,E7KVALUE2                                                                      ");
                sb.AppendLine(" ,E8KVALUE                                                                       ");
                sb.AppendLine(" ,E9KVALUE2                                                                      ");
                sb.AppendLine(" ,E5LEVEL                                                                        ");
                sb.AppendLine(" ,E6LEVEL                                                                        ");
                sb.AppendLine(" ,E7LEVEL                                                                        ");
                sb.AppendLine(" ,E8LEVEL                                                                        ");
                sb.AppendLine(" ,E9LEVEL                                                                        ");
                sb.AppendLine(" ,SHELLRESISTANCE_LEVELNAME                                                      ");
                sb.AppendLine(" ,VR_LEVELNAME                                                                   ");
                sb.AppendLine(" ,B1.E14CHECKFLAG                                                                ");
                sb.AppendLine(" ,B1.E14WEIGHT                                                                   ");
                //20181031
                sb.AppendLine(" ,B2.E3LEVEL                                                                     ");
                sb.AppendLine(" ,B1.LEVELNAME                                                                   ");
                //20160324
                sb.AppendLine(" ,E5TESTTIME1,E6TESTTIME2                                                        ");
                sb.AppendLine(" ,B1.E1BEFORWEIGHT                                                              ");
                //
                sb.AppendLine(" FROM                                                                            ");
                sb.AppendLine("     " + b1 + "                       B1                                    ");
                sb.AppendLine("     left join                                                                   ");
                sb.AppendLine("     " + be + "                                                                 ");
                sb.AppendLine("     BE                                                                          ");
                sb.AppendLine("     on                                                                          ");
                sb.AppendLine("         B1.BATTERYNO = BE.BATTERYNO                                             ");
                sb.AppendLine("     left join                                                                   ");
                sb.AppendLine("     " + b7 + "                                                                 ");
                sb.AppendLine("     B7                                                                          ");
                sb.AppendLine("     on                                                                          ");
                sb.AppendLine("         B7.BATTERYNO = B1.BATTERYNO                                             ");
                //20181031
                sb.AppendLine("     left join                                                                   ");
                sb.AppendLine("     " + b2 + "                                                                 ");
                sb.AppendLine("     B2                                                                          ");
                sb.AppendLine("     on                                                                          ");
                sb.AppendLine("         B2.BATTERYNO = B1.BATTERYNO                                             ");
                sb.AppendLine(" where                                                                           ");
                sb.AppendLine("         B1.BOMNO = '{0}'                                                       ");
                sb.AppendLine("     AND B1.SIZENO = '{1}'                                                     ");
                sb.AppendLine("     ) as BATTERY                                                                ");
                sb.AppendLine(" ORDER BY E13OUT_CAPACITY2 ASC                                                   ");
                sb.AppendLine("   FOR    XML    AUTO                                                            ");

                string sql = string.Format(sb.ToString(), dt.Rows[0]["BOMNO"].ToString(), dt.Rows[0]["SIZENO"].ToString());
                XmlDocument levelSet = getResult(sql);

                if (levelSet != null && levelSet.DocumentElement != null && levelSet.DocumentElement.ChildNodes != null && levelSet.DocumentElement.ChildNodes.Count > 0)
                {
                    this.WriteFileLog(IPAddress, sql);
                    SaveNodeAction(dt.Rows[0]["PIPELINE"].ToString(), dt.Rows[0]["BOMNO"].ToString(), dt.Rows[0]["SIZENO"].ToString(), dt.Rows[0]["FLOW_NAME"].ToString(), dt.Rows[0]["NOW_NODENAME"].ToString(), null, 1);
                    return levelSet;
                }
                else
                {
                    return getXml(RET_NULL);
                }
            }
            catch (Exception ex)
            {
                this.WriteFileLog(IPAddress, ex.Message);
                return getXml(RET_ERROR);
            }
        }
        private XmlDocument LoadvalidDataOfSAP_ME(string code,string nodename,string IPAddress)
        {
            if (string.IsNullOrEmpty(code.Trim()) || string.IsNullOrEmpty(nodename.Trim()))
            {
                this.WriteFileLog(IPAddress, string.Format(TRAY_NULL, code));
                return getXml(RET_NULL);
            }
            try
            {
                string result = HttpGet(ConfigurationManager.AppSettings["LoadValidData"], "LoadValidData", "code=" + code + "&nodename=" + nodename);
                if (result.Contains(EMPTY_SAP))
                {
                    return null;
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);
                    return doc;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>检索返回xml的结果 杭可/擎天分容
        /// </summary>
        /// <param name="code">电池编号或批次号号</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private XmlDocument getXmlResultForFenRong(string code, string IPAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(code.Trim()))
                {
                    this.WriteFileLog(IPAddress, string.Format(TRAY_NULL, code));
                    return getXml(RET_NULL);
                }
                DataSet ds = getRealFlow(code);
                if (ds == null || ds.Tables[0].Rows.Count <= 0)
                {
                    string ErrMsg2 = string.Format(BATTERY_NULL, code);
                    this.WriteFileLog(IPAddress, ErrMsg2);
                    return getXml(RET_NULL);
                }
                string pipeline = ds.Tables[0].Rows[0]["OTHER_NAME"].ToString();
                string bomno = ds.Tables[0].Rows[0]["BOMNO"].ToString();
                string b5 = "V_" + pipeline + "_" + bomno + "_B5";
                string b7 = "V_" + pipeline + "_" + bomno + "_B7";
                string b8 = "V_" + pipeline + "_" + bomno + "_B8";
                StringBuilder sb = new StringBuilder();
                sb.Append("    SELECT                                                      ");
                sb.Append("    A.ID                                                    ");
                sb.Append("    ,A.SIZENO                                               ");
                sb.Append("    ,A.BATTERYNO                                            ");
                sb.Append("    ,A.TUOPANNO                                             ");
                sb.Append("    ,A.POSTION                                              ");
                sb.Append("    ,A.BOMNO_SIZENO                                         ");
                //20130509XQ-1 a start
                sb.Append("    ,B.E1DIFF                                               ");
                sb.Append("    ,B.E1STATE                                              ");
                sb.Append("    ,B.E5VOLTAGE1                                           ");
                sb.Append("    ,B.E5RESISTANCE1                                        ");
                sb.Append("    ,B.E5LEVEL                                              ");
                sb.Append("    ,B.E6VOLTAGE2                                           ");
                sb.Append("    ,B.E6RESISTANCE2                                        ");
                sb.Append("    ,B.E6DELTAV                                             ");
                sb.Append("    ,B.E6LEVEL                                              ");
                sb.Append("    ,B.E8VOLTAGE1                                           ");
                sb.Append("    ,B.E8RESISTANCE1                                        ");
                sb.Append("    ,C.E8LEVEL                                              ");
                sb.Append("    ,B.E13OUT_CAPACITY2                                     ");
                sb.Append("    ,B.E13LEVEL                                             ");
                //20130509XQ-1 a end
                sb.Append("FROM                                                        ");
                sb.Append("    " + b8 + "   A                                    ");
                sb.Append("   ," + b7 + "   B                                     "); //20130509XQ-1 a
                sb.Append("   ," + b5 + "   C                                    "); //20130509XQ-1 a
                sb.Append("WHERE                                                       ");
                sb.Append("   A.BATTERYNO = B.BATTERYNO                                ");
                sb.Append("   AND  A.BATTERYNO = C.BATTERYNO                                ");
                sb.Append("   AND  A.BOMNO_SIZENO = '{0}'                              ");
                //sb.Append("     A.BOMNO_SIZENO = @CODE                                ");
                sb.Append("  AND   A.STATE = '1'                                ");
                sb.Append("ORDER BY                                                    ");
                sb.Append("    A.SIZENO                                                ");
                sb.Append("    ,A.TUOPANNO                                             ");
                sb.Append("    ,A.POSTION    FOR    XML    AUTO                        ");
                string sql = string.Format(sb.ToString(), code);
                SysLog log = new SysLog(sql);
                XmlDocument levelSet = getResult(sql);
                if (levelSet != null)
                {
                    this.WriteFileLog(IPAddress, string.Format(sb.ToString(), code));
                    return levelSet;
                }
                else
                {
                    string ErrMsg2 = string.Format(BATTERY_NULL, code);
                    this.WriteFileLog(IPAddress, ErrMsg2);
                    return getXml(RET_NULL);
                }
            }
            catch (Exception ex)
            {
                WriteFileLog(IPAddress, ex.Message);
                return getXml(RET_ERROR);
            }
        }
        /// <summary>检索返回xml的结果 杭可/擎天分容
        /// </summary>
        /// <param name="code">电池编号或批次号号</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private XmlDocument getXmlResultForOCV_FENDDANG(string BOMNO, string LineName, string IPAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(BOMNO.Trim()))
                {
                    string ErrMsg1 = string.Format(BOMNO_NULL, BOMNO, LineName);
                    this.WriteFileLog(IPAddress,ErrMsg1);
                    return getXml(RET_NULL);
                }


                StringBuilder sb = new StringBuilder();
                //sb.Append("SELECT                                                      "); //20140910修改分档后出现5份数据的问题
                sb.Append("SELECT     distinct                                         ");
                sb.Append("    A.*                                                     ");
                sb.Append("FROM                                                        ");
                sb.Append("    (                                                       ");
                sb.Append("        SELECT                                              ");
                sb.Append("            A.BOMNO                                         ");
                sb.Append("            ,B.PIPELINENAME                                 ");
                sb.Append("            ,A.LEVELNAME                                    ");
                sb.Append("            ,A.SHOWCONDITION                                ");
                sb.Append("            ,A.REALCONDITION                                ");
                sb.Append("        FROM                                                ");
                sb.Append("            T_BOM_TRANCHE  A                                 ");
                sb.Append("            LEFT JOIN                                       ");
                sb.Append("            T_BOM_PIPELINE_FLOW   B                              ");
                sb.Append("            ON                                              ");
                sb.Append("                A.BOMNO = B.BOMNO                           ");
                sb.Append("        WHERE                                               ");
                sb.Append("                A.STATE = '启用'                            ");
                sb.Append("            AND A.BOMNO = '{0}'                            ");
                if (!string.IsNullOrEmpty(LineName))
                {
                    sb.Append("            AND B.PIPELINENAME = '{1}'          ");
                }
                sb.Append("    )                                                       ");
                sb.Append("    A                                                       ");
                sb.Append("    FOR  XML  AUTO                                          ");


                string sql = string.Format(sb.ToString(), BOMNO, LineName);
                XmlDocument levelSet = getResult(sql);
                if (levelSet != null)
                {
                    this.WriteFileLog(IPAddress, sql);
                    return levelSet;
                }
                else
                {
                    string ErrMsg2 = string.Format(BATTERY_NULL, BOMNO);
                    this.WriteFileLog(IPAddress, ErrMsg2);
                    return getXml(RET_NULL);
                }
            }
            catch (Exception ex)
            {
                WriteFileLog(IPAddress, ex.Message);
                return getXml(RET_ERROR);
            }
        }
        private XmlDocument getXmlResultForOCV_FENDDANGOfSAP_ME(string BOMNO, string LineName, string IPAddress)
        {
            if (string.IsNullOrEmpty(BOMNO.Trim()) || string.IsNullOrEmpty(LineName.Trim()))
            {
                this.WriteFileLog(IPAddress, string.Format(TRAY_NULL, BOMNO));
                return getXml(RET_NULL);
            }
            try
            {
                string result = HttpGet(ConfigurationManager.AppSettings["GetOCV_FENDANG"], "GetOCV_FENDANG", "BOMNO=" + BOMNO + "&LineName=" + LineName);
                if (result.Contains(EMPTY_SAP))
                {
                    return null;
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);
                    if (ConfigurationManager.AppSettings["RearrangeRankStandard"].Trim().ToUpper() == "TRUE")
                    {
                        doc = SAP_Information.RearrangeRankStandard(doc);
                    }
                    return doc;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>检索返回xml的结果 IR/OCV针床
        /// </summary>
        /// <param name="code"></param>
        /// <param name="IPAddress"></param>
        /// <returns></returns>
        private XmlDocument getXmlResultForOCV_FENDDANG_Attributes(string code, string IPAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    string ErrMsg1 = string.Format(TRAY_NULL, code);
                    this.WriteFileLog(IPAddress, ErrMsg1);
                    return getXml(RET_NULL);
                }

                DataSet ds1 = getRealFlow(code);

                string linename = "";
                string bomno = "";
                string sizeno = "";
                string tableB7 = "";
                string tableB8 = "";
                string tableB5 = ""; //20130509XQ-1 a

                if (ds1 == null || ds1.Tables[0].Rows.Count <= 0)
                {
                    string ErrMsg2 = string.Format(BATTERY_NULL, code);
                    this.WriteFileLog(IPAddress, ErrMsg2);
                    return getXml(RET_NULL);
                }
                else
                {
                    linename = ds1.Tables[0].Rows[0]["OTHER_NAME"].ToString();
                    bomno = ds1.Tables[0].Rows[0]["BOMNO"].ToString();
                    sizeno = ds1.Tables[0].Rows[0]["SIZENO"].ToString();
                    tableB7 = "V_" + linename + "_" + bomno + "_B7";
                    tableB8 = "V_" + linename + "_" + bomno + "_B8";
                    tableB5 = "V_" + linename + "_" + bomno + "_B5";
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * from (                                            "); //20130529 add
                sb.Append("SELECT top 5000                                             ");//单批次下最多999块电池
                sb.Append("     A.ORDERNO                                              ");
                sb.Append("    ,A.BOMNO                                                ");
                sb.Append("    ,A.SIZENO                                               ");
                sb.Append("    ,A.BATTERYNO                                            ");
                sb.Append("    ,A.E1DIFF                                               ");
                sb.Append("    ,A.E1STATE                                               ");
                sb.Append("    ,A.E5VOLTAGE1                                           ");
                sb.Append("    ,A.E5RESISTANCE1                                        ");
                sb.Append("    ,A.E5TESTTIME1                                          ");
                sb.Append("    ,A.E5LEVEL                                          ");
                sb.Append("    ,A.E6VOLTAGE2                                           ");
                sb.Append("    ,A.E6RESISTANCE2                                        ");
                sb.Append("    ,A.E6TESTTIME2                                          ");
                sb.Append("    ,A.E6LEVEL                                              ");
                sb.Append("    ,A.E6DELTAV                                             ");
                sb.Append("    ,A.E7VOLTAGE3                                           ");
                sb.Append("    ,A.E7RESISTANCE3                                        ");
                sb.Append("    ,A.E7TESTTIME3                                          ");
                sb.Append("    ,A.E7LEVEL                                              ");
                sb.Append("    ,A.E8VOLTAGE1                                           ");
                sb.Append("    ,A.E8RESISTANCE1                                        ");
                sb.Append("    ,A.E8TESTTIME1                                          ");
                sb.Append("    ,C.E8LEVEL                                              ");//20130509XQ-1 a
                sb.Append("    ,A.E9VOLTAGE2                                           ");
                sb.Append("    ,A.E9RESISTANCE2                                        ");
                sb.Append("    ,A.E9TESTTIME2                                          ");
                sb.Append("    ,A.E10VOLTAGE1                                          ");
                sb.Append("    ,A.E10RESISTANCE1                                       ");
                sb.Append("    ,A.E10TESTTIME1                                         ");
                sb.Append("    ,A.E14WEIGHT                                            ");
                sb.Append("    ,A.E14TESTTIME                                          ");
                sb.Append("    ,A.E4OUT_CAPACITY2                                      ");
                sb.Append("    ,A.E4ENDTIME                                            ");
                //add 
                if (getdataexchangeflagbybatteryno(bomno) <= 0)
                {
                    sb.Append("    ,A.E13OUT_CAPACITY2                                     ");//or
                }
                else
                {
                    sb.Append("    ,A.E6KVALUE as E13OUT_CAPACITY2                         ");
                }

                //add
                sb.Append("    ,A.E13LEVEL                                             ");
                sb.Append("    ,A.E13ENDTIME                                           ");
                sb.Append("    ,A.LEVELNAME                                            ");
                sb.Append("FROM                                                        ");
                sb.Append("    " + tableB7 + "    A                                    ");
                sb.Append("    ," + tableB5 + "   C                                    ");//20130509XQ-1 a
                sb.Append("WHERE                                                       ");
                sb.Append("  A.BATTERYNO = C.BATTERYNO                                 ");
                sb.Append("  AND   A.SIZENO ='{1}'          ");
                sb.Append("  ) A          ");  //20130529 add

                sb.Append(" FOR    XML    AUTO                                         ");

                XmlDocument levelSet = getResult(string.Format(sb.ToString(), code, sizeno));


                if (levelSet != null)
                {
                    this.WriteFileLog(IPAddress, string.Format(sb.ToString(), code, sizeno));
                    return levelSet;
                }
                else
                {
                    string ErrMsg2 = string.Format(BATTERY_NULL, code);
                    this.WriteFileLog(IPAddress, ErrMsg2);
                    return getXml(RET_NULL);
                }
            }
            catch (Exception ex)
            {
                this.WriteFileLog(IPAddress, ex.Message);
                return getXml(RET_ERROR);
            }
        }
        private XmlDocument getXmlResultForOCV_FENDDANG_AttributesOfSAP_ME(string code, string IPAddress)
        {
            if (string.IsNullOrEmpty(code.Trim()))
            {
                this.WriteFileLog(IPAddress, string.Format(TRAY_NULL, code));
                return getXml(RET_NULL);
            }
            try
            {
                string result = HttpGet(ConfigurationManager.AppSettings["GetOCV_FENDANG_Attributes"], "GetOCV_FENDANG_Attributes", "code=" + code);
                if (result.Contains(EMPTY_SAP))
                {
                    return null;
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);
                    return doc;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        private int getdataexchangeflagbybatteryno(string BOMNO)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception(DATABASE_ERROR);
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT COUNT(1)  FROM T_BOM_TRANCHE WHERE BOMNO = '{0}' AND DATAEXCHANGE = '是'", BOMNO), conn);
                    return int.Parse(comm.ExecuteScalar().ToString());
                }
                catch (Exception ex)
                {
                    this.WriteFileLog("", ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 调用 degas 参数（力神老系统）
        /// </summary>
        /// <param name="code"></param>
        /// <param name="IPAddress"></param>
        /// <returns></returns>
        private XmlDocument getXmlResultForZY_GetParaNew1(string code, string IPAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" IF EXISTS ( SELECT  BOMNO                                                       ");
            sb.AppendLine("             FROM    [T_BOM_LIQUIDWEIGHT_SET]                                    ");
            sb.AppendLine("             WHERE   BOMNO = '{0}' )                                            ");
            sb.AppendLine("     BEGIN                                                                       ");
            sb.AppendLine("         SELECT  [BOMNO],                                                       ");
            sb.AppendLine("                 [CONSTANT] AS [Const] ,                                         ");
            sb.AppendLine("                 [STANDARD2] AS [Standard] ,                                     ");
            sb.AppendLine("                 [MAX2] AS [Upper] ,                                             ");
            sb.AppendLine("                 [MIN2] AS [Lower] ,                                              ");
            sb.AppendLine("                 [STANDARD4] AS [WEIGHTSTANDARD],                                         ");
            sb.AppendLine("                 [MAX4] AS [WEIGHTSTANDARDMAX],                                     ");
            sb.AppendLine("                 [Min4] AS [WEIGHTMIN],                                             ");
            sb.AppendLine("                 [STANDARD5] AS [PRESSURESTANDARD],                                               ");
            sb.AppendLine("                 [MAX5] AS [PRESSUREMAX],                                               ");
            sb.AppendLine("                 [Min5] AS [PRESSUREMIN],                                               ");
            sb.AppendLine("                 [STANDARD6] AS [TIMESTANDARD],                                               ");
            sb.AppendLine("                 [MAX6] AS [TIMEMAX],                                               ");
            sb.AppendLine("                 [Min6] AS [TIMEMIN],                                               ");
            sb.AppendLine("                 [STANDARD7] AS [TEMPERATURESTANDARD],                                               ");
            sb.AppendLine("                 [MAX7] AS [TEMPERATUREMAX],                                               ");
            sb.AppendLine("                 [Min7] AS [TEMPERATUREMIN],                                               ");
            sb.AppendLine("                 [STANDARD8] AS [LIQUIDCONTROLSTANDARD],                                               ");
            sb.AppendLine("                 [MAX8] AS [LIQUIDCONTROLMAX],                                               ");
            sb.AppendLine("                 [Min9] AS [LIQUIDCONTROLMIN],                                               ");
            sb.AppendLine("                 [Min8] AS [BATTERYCAPACITYMIN]                                           ");
            sb.AppendLine("         FROM    [T_BOM_LIQUIDWEIGHT_SET] AS A                                   ");
            sb.AppendLine("         WHERE   BOMNO = '{0}'                                                  ");
            sb.AppendLine("         FOR     XML AUTO                                    ");
            sb.AppendLine("     END                                                                         ");
            sb.AppendLine(" ELSE                                                                            ");
            sb.AppendLine("     BEGIN                                                                       ");
            sb.AppendLine("         SELECT  *                                                               ");
            sb.AppendLine("         FROM    ( SELECT    '' AS BOMNO,                                       ");
            sb.AppendLine("                             '' AS [Const] ,                                     ");
            sb.AppendLine("                             '' AS [Standard] ,                                  ");
            sb.AppendLine("                             '' AS [Upper] ,                                     ");
            sb.AppendLine("                             '' AS [Lower] ,                                     ");
            sb.AppendLine("                             '' AS [WEIGHTSTANDARD],                                     ");
            sb.AppendLine("                             '' AS [WEIGHTSTANDARDMAX],                                  ");
            sb.AppendLine("                             '' AS [WEIGHTMIN],                                     ");
            sb.AppendLine("                             '' AS [PRESSURESTANDARD],                                      ");
            sb.AppendLine("                             '' AS [PRESSUREMAX],                                      ");
            sb.AppendLine("                             '' AS [PRESSUREMIN],                                      ");
            sb.AppendLine("                             '' AS [TIMESTANDARD],                                      ");
            sb.AppendLine("                             '' AS [TIMEMAX],                                      ");
            sb.AppendLine("                             '' AS [TIMEMIN],                                      ");
            sb.AppendLine("                             '' AS [TEMPERATURESTANDARD],                                      ");
            sb.AppendLine("                             '' AS [TEMPERATUREMAX],                                      ");
            sb.AppendLine("                             '' AS [TEMPERATUREMIN],                                      ");
            sb.AppendLine("                             '' AS [LIQUIDCONTROLSTANDARD],                                      ");
            sb.AppendLine("                             '' AS [LIQUIDCONTROLMAX],                                      ");
            sb.AppendLine("                             '' AS [LIQUIDCONTROLMIN],                                      ");
            sb.AppendLine("                             '' AS [BATTERYCAPACITYMIN]                                      ");
            sb.AppendLine("                 ) AS A                                                          ");
            sb.AppendLine("         FOR     XML AUTO                                        ");
            sb.AppendLine("     END                                                                         ");

            XmlDocument levelSet = getResult(string.Format(sb.ToString(), code));


            if (levelSet != null)
            {
                this.WriteFileLog(IPAddress, string.Format(sb.ToString(), code));
                return levelSet;
            }
            else
            {
                string ErrMsg2 = string.Format(BATTERY_NULL, code);
                this.WriteFileLog(IPAddress, ErrMsg2);
                return getXml(RET_NULL);
            }
        }
        /// <summary>
        /// 调用 degas 参数（SAP ME）
        /// </summary>
        /// <param name="coe"></param>
        /// <param name="IPAddress"></param>
        /// <returns></returns>
        private XmlDocument getXmlResultForZY_GetParaNewOfSAP_ME(string code,string IPAddress)
        {
            if (string.IsNullOrEmpty(code.Trim()))
            {
                string ErrMsg2 = string.Format(BATTERY_NULL, code);
                this.WriteFileLog(IPAddress, ErrMsg2);
                return null;
            }
            try
            {
                string result = HttpGet(ConfigurationManager.AppSettings["getXmlResultForZY_GetParaNew"], "getXmlResultForZY_GetParaNew", "code=" + code);
                if (result.Contains(EMPTY_SAP))
                {
                    return null;
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);
                    return doc;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 向 SAP ME 服务器发送请求 —— GET 方式
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postMethod"></param>
        /// <param name="postDataName"></param>
        /// <param name="postDataValue"></param>
        /// <returns></returns>
        public string HttpGet(string Url, string postMethod, string postData)
        {
            if (postMethod.Length > 0)
            {
                Url += postMethod;
            }
            if (postData.Length > 0)
            {
                Url += "?" + postData;
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.Timeout = 10000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
        /// <summary>
        /// 向 SAP ME 服务发送请求 —— GET 方式
        /// </summary>
        /// <param name="code">批次编号（完整）</param>
        /// <param name="IPAddress"></param>
        /// <returns></returns>
        public XmlDocument GetXmlResultForFenRongOfSAP_ME(string code, string IPAddress)
        {
            if (string.IsNullOrEmpty(code.Trim()))
            {
                this.WriteFileLog(IPAddress, string.Format(TRAY_NULL, code));
                return getXml(RET_NULL);
            }
            try
            {
                string result = HttpGet(ConfigurationManager.AppSettings["GetFenRong"], "GetFenRong", "code=" + code);
                if (result.Contains(EMPTY_SAP))
                {
                    return null;
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);
                    return doc;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        private XmlDocument getXmlResultForZY_GetDataOfSAP_ME(string code,string node,string operator1,string IPAddress)
        {
            if (string.IsNullOrEmpty(code.Trim()) || string.IsNullOrEmpty(node.Trim()))
            {
                this.WriteFileLog(IPAddress, string.Format(TRAY_NULL, code));
                return getXml(RET_NULL);
            }
            try
            {
                string result = HttpGet(ConfigurationManager.AppSettings["ZY_GetData"], "ZY_GetData", string.Format("code={0}&node={1}&operator1=", code, node));
                if (result.Contains(EMPTY_SAP))
                {
                    return null;
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);
                    return doc;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        private XmlDocument getXmlResultForZY_GetData(string code, string node, string operator1, string equipmentid, string IPAddress)
        {

            StringBuilder sb;
            //DataTable dt = getPipelineAndBomno(code, node);
            DataTable dt = getBaseInfo(code, node);
            //string tableB7;
            string linename="";
            string bomno="";
            string sizeno="";
            string tableB7="";
            if (dt.Rows.Count > 0)
            {
                linename = dt.Rows[0]["OTHER_NAME"].ToString();
                bomno = dt.Rows[0]["BOMNO"].ToString();
                sizeno = dt.Rows[0]["SIZENO"].ToString();
                tableB7 = "V_" + linename + "_" + bomno + "_B7";

                //tableB7 = ServerCommon.getTableName(dt.Rows[0]["PIPELINE"].ToString(), dt.Rows[0]["BOMNO"].ToString(), "B7");

                sb = new StringBuilder();
                //sb.AppendLine(" IF EXISTS ( SELECT                                                              ");
                //sb.AppendLine("                     [NOW_NODENAME] ,                                            ");
                //sb.AppendLine("                     [BOMNO_SIZENO]                                              ");
                //sb.AppendLine("             FROM    [T_REAL_FLOW] T ,                       ");
                //sb.AppendLine("                     " + tableB7 + " B7                ");
                //sb.AppendLine("             WHERE   [NOW_NODENAME] = [NAME]                                     ");
                //sb.AppendLine("                     AND B7.[BOMNO] + B7.[SIZENO] = [BOMNO_SIZENO]               ");
                //sb.AppendLine("                     AND [NOW_NODENAME] = @NODE                                  ");
                //sb.AppendLine("                     AND [BOMNO_SIZENO] = @CODE )                                ");
                //sb.AppendLine("     BEGIN                                                                       ");
                sb.AppendLine("         SELECT  [SIZENO] ,                                                      ");
                sb.AppendLine(" 				[BATTERYNO] ,                                                   ");
                sb.AppendLine("                 [ORDERNO] ,                                                     ");
                sb.AppendLine("                 [BOMNO] ,                                                       ");
                sb.AppendLine("                 [E1BEFORWEIGHT] ,                                               ");
                sb.AppendLine("                 [E1DIFF] ,                                                      ");
                sb.AppendLine("                 [E1STATE]                                                       ");
                sb.AppendLine("         FROM    {0}  A                                         ");

                sb.AppendLine("         WHERE SIZENO =  '{1}'     ");
                sb.AppendLine("         AND [E1BEFORWEIGHT] IS NOT NULL AND E1DIFF IS NOT NULL AND E1STATE  IS NOT NULL    FOR XML AUTO      ");



                //sb.AppendLine("         WHERE   [NAME] = @NODE                                                  ");
                //sb.AppendLine("                 AND [BOMNO] + [SIZENO] = @CODE                                  ");
                //sb.AppendLine("                 FOR XML AUTO                                                    ");
                //sb.AppendLine("     END                                                                         ");
                //sb.AppendLine(" ELSE                                                                            ");
                //sb.AppendLine("     BEGIN                                                                       ");
                //sb.AppendLine("         SELECT  *                                                               ");
                //sb.AppendLine("         FROM    ( SELECT    '' AS [SIZENO] ,                                    ");
                //sb.AppendLine("                             '' AS [BATTERYNO] ,                                 ");
                //sb.AppendLine("                             '' AS [ORDERNO] ,                                   ");
                //sb.AppendLine("                             '' AS [BOMNO] ,                                     ");
                //sb.AppendLine("                             '' AS [E1BEFORWEIGHT] ,                             ");
                //sb.AppendLine("                             '' AS [E1DIFF] ,                                    ");
                //sb.AppendLine("                             '' AS [E1STATE]                                     ");
                //sb.AppendLine("                 ) A FOR XML AUTO                                                ");
                //sb.AppendLine("     END                                                                         ");
            }
            else
            {
                sb = new StringBuilder();
                sb.AppendLine("         SELECT  *                                                               ");
                sb.AppendLine("         FROM    ( SELECT    '' AS [SIZENO] ,                                    ");
                sb.AppendLine("                             '' AS [BATTERYNO] ,                                 ");
                sb.AppendLine("                             '' AS [ORDERNO] ,                                   ");
                sb.AppendLine("                             '' AS [BOMNO] ,                                     ");
                sb.AppendLine("                             '' AS [E1BEFORWEIGHT] ,                             ");
                sb.AppendLine("                             '' AS [E1DIFF] ,                                    ");
                sb.AppendLine("                             '' AS [E1STATE]                                     ");
                sb.AppendLine("                 ) A FOR XML AUTO                                                ");
            }
            //SqlParameter[] paras ={
            //           SqlHelper.MakeInPara("@CODE",SqlDbType.VarChar,500,code),
            //           SqlHelper.MakeInPara("@NODE",SqlDbType.VarChar,500,node)
                                         //};
            XmlDocument levelSet = new XmlDocument();
            //levelSet = this.ExecuteSelectXML(sb.ToString(), paras);
            levelSet = getResult(string.Format(sb.ToString(), tableB7, sizeno));


            if (levelSet != null)
            {
                this.WriteFileLog(IPAddress, string.Format(sb.ToString(), tableB7, sizeno));
                return levelSet;
            }
            else
            {
                string ErrMsg2 = string.Format(BATTERY_NULL, code);
                //this.WriteFileLogForXmlDocument(IPAddress, code, ErrMsg2);

                StringBuilder sbnull = new StringBuilder();
                sbnull.AppendLine("         SELECT  *                                                               ");
                sbnull.AppendLine("         FROM    ( SELECT    '' AS [SIZENO] ,                                    ");
                sbnull.AppendLine("                             '' AS [BATTERYNO] ,                                 ");
                sbnull.AppendLine("                             '' AS [ORDERNO] ,                                   ");
                sbnull.AppendLine("                             '' AS [BOMNO] ,                                     ");
                sbnull.AppendLine("                             '' AS [E1BEFORWEIGHT] ,                             ");
                sbnull.AppendLine("                             '' AS [E1DIFF] ,                                    ");
                sbnull.AppendLine("                             '' AS [E1STATE]                                     ");
                sbnull.AppendLine("                 ) A FOR XML AUTO                                                ");
                XmlDocument levelSet1 = new XmlDocument();
                //levelSet1 = this.ExecuteSelectXML(sbnull.ToString(), null);
                levelSet1 = getResult(string.Format(sbnull.ToString()));
                return levelSet1;
            }
        }
        private XmlDocument getXmlResultForZY_GetPara(string code, string IPAddress)
        {
            string bomno = code;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" IF EXISTS ( SELECT  BOMNO                                                       ");
            sb.AppendLine("             FROM    [T_BOM_LIQUIDWEIGHT_SET]                                    ");
            sb.AppendLine("             WHERE   BOMNO = '{0}' )                                            ");
            sb.AppendLine("     BEGIN                                                                       ");
            sb.AppendLine("         SELECT  [BOMNO] ,                                                       ");
            sb.AppendLine("                 [CONSTANT] AS [Const] ,                                         ");
            sb.AppendLine("                 [STANDARD2] AS [Standard] ,                                     ");
            sb.AppendLine("                 [MAX2] AS [Upper] ,                                             ");
            sb.AppendLine("                 [MIN2] AS [Lower]                                               ");
            sb.AppendLine("         FROM    [T_BOM_LIQUIDWEIGHT_SET] AS A                                   ");
            sb.AppendLine("         WHERE   BOMNO = '{0}'                                                  ");
            sb.AppendLine("         FOR     XML AUTO                                    ");
            sb.AppendLine("     END                                                                         ");
            sb.AppendLine(" ELSE                                                                            ");
            sb.AppendLine("     BEGIN                                                                       ");
            sb.AppendLine("         SELECT  *                                                               ");
            sb.AppendLine("         FROM    ( SELECT    '' AS BOMNO ,                                       ");
            sb.AppendLine("                             '' AS [Const] ,                                     ");
            sb.AppendLine("                             '' AS [Standard] ,                                  ");
            sb.AppendLine("                             '' AS [Upper] ,                                     ");
            sb.AppendLine("                             '' AS [Lower]                                       ");
            sb.AppendLine("                 ) AS A                                                          ");
            sb.AppendLine("         FOR     XML AUTO                                        ");
            sb.AppendLine("     END                                                                         ");

            XmlDocument levelSet = new XmlDocument();
            levelSet = getResult(string.Format(sb.ToString(), bomno));

            if (levelSet != null)
            {
                this.WriteFileLog(IPAddress, string.Format(sb.ToString(), code));
                return levelSet;
            }
            else
            {
                string ErrMsg2 = string.Format(BATTERY_NULL, code);
                this.WriteFileLog(IPAddress, code);
                return null;
            }
        }

    }
}
