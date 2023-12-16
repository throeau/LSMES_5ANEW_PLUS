using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LSMES_5ANEW_PLUS.App_Base;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Text;


namespace LSMES_5ANEW_PLUS.Business
{
    public class SyncCellPack
    {
        private string mWebSite;
        private string mEquipment;
        private string mLineNo;
        private string mBomno;
        private string mItemNo;
        private string mCustomerNo;
        private string mLabelCode;
        private string mBoxid;
        private string mOperator;
        private string mOrderNo;
        private int mQtyBox;
        private CellInfoList mList;
        public SyncCellPack()
        {
            mList = new CellInfoList();
        }
        public void Initialization (EntitySynCellPack entity)
        {
            mWebSite = entity.WEBSITE;
            mEquipment = entity.EQUIPMENT;
            mLineNo = entity.LINENO;
            mBomno = entity.BOMNO;
            mItemNo = entity.ITEMNO;
            mCustomerNo = entity.CUSTOMERNO;
            mLabelCode = entity.LABELCODE;
            mBoxid = entity.BOXID;
            mOperator = entity.OPERATORS;
            mOrderNo = entity.ORDERNO;
        }
        public CellInfoList GetCellsInfo()
        {
            try
            {
                mList.BOXIID = null;
                mList.CELL_LIST.Clear();
                foreach (string line in Pipeline(mBomno))
                {
                    string sql = string.Format("SELECT B7.BATTERYNO,E5VOLTAGE1,E5TESTTIME1,E6VOLTAGE2,E6TESTTIME2 FROM V_{2}_{0}_B7 B7 INNER JOIN V_{2}_{0}_BOX BOX ON B7.BATTERYNO = BOX.BATTERYNO WHERE BOXID = '{1}';", mBomno, mBoxid, line);
                    mList.BOXIID = mBoxid;
                    using (SqlConnection mConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                    {
                        SqlDataReader reader;
                        mConn.Open();
                        SqlCommand mComm = new SqlCommand(sql, mConn);
                        reader = mComm.ExecuteReader();
                        while (reader.Read())
                        {
                            CellInfo cell = new CellInfo();
                            cell.BOMNO = mBomno;
                            cell.ITEM_NO = mItemNo;
                            cell.ORDER_NO = mOrderNo;
                            cell.BATTERYNO = reader["BATTERYNO"].ToString();
                            cell.E5VOLTAGE1 = reader["E5VOLTAGE1"].ToString();
                            cell.E5TESTTIME = reader["E5TESTTIME1"].ToString();
                            cell.E6VOLTAGE2 = reader["E6VOLTAGE2"].ToString();
                            cell.E6TESTTIME = reader["E6TESTTIME2"].ToString();
                            mList.CELL_LIST.Add(cell);
                        }
                        reader.Close();
                        mConn.Close();
                    }
                }
                return mList;
            }
            catch (Exception ex)
            {
                SysLog mLog = new SysLog("SyncCellPack : public CellInfoList GetCellsInfo(CellBoxInfo entityBox) 发生异常. " + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 判断一个包装箱是否已经导入 SAP PACK 系统
        /// </summary>
        /// <returns>已导入：异常；未导入：非异常</returns>
        public AppException IsValidSAP()
        {
            string sql = string.Format("SELECT COUNT(1) QTY FROM Z_CELL_PACK zcp INNER JOIN Z_CELL_PACK_SN zcps ON ZCP.HANDLE = ZCPS.CELL_PACK_BO AND ZCP.NUM = '{0}';", mBoxid);
            try
            {
                StatusCell status;
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
                {
                    conn.Open();
                    OdbcDataReader reader;
                    OdbcCommand mComm = new OdbcCommand(sql, conn);
                    reader = mComm.ExecuteReader();
                    reader.Read();
                    mQtyBox = int.Parse(reader["QTY"].ToString());
                    reader.Close();
                    if (mQtyBox == 0)
                    {
                        status = StatusCell.Normal;
                        return new AppException(status);
                    }
                    else
                    {
                        status = StatusCell.Repeat;
                        return new AppException(status, new Exception("Class: CellInfoList, Function: IsValidSAP()]"));
                    }

                }
            }
            catch(Exception ex)
            {
                return new AppException("Class:CellInfoList,Function:IsValid(string boxid)],Sql statement:" + sql, ex);
            }
        }
        /// <summary>
        /// 获取 LSMES 包装箱中电池数量
        /// </summary>
        /// <returns></returns>
        public int QtyOfBoxByLsmes()
        {
            string sql = null;
            try
            {
                foreach (string line in Pipeline(mBomno))
                {
                    sql = string.Format("SELECT COUNT(1) QTY FROM V_{2}_{0}_B7 B7 INNER JOIN V_{2}_{0}_BOX BOX ON B7.BATTERYNO = BOX.BATTERYNO AND BOXID = '{1}';", mBomno, mBoxid, line);
                    using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                    {
                        conn.Open();
                        SqlCommand mComm = new SqlCommand(sql, conn);
                        SqlDataReader reader;
                        reader = mComm.ExecuteReader();
                        reader.Read();
                        if (int.Parse(reader["QTY"].ToString()) > 0)
                        {
                            return int.Parse(reader["QTY"].ToString());
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                AppException exp = new AppException("Class:CellInfoList,Function:QtyOfBoxByLsmes(string boxid,string bomno)],Sql statement:" + sql, ex);
                return 0;
            }
        }
        public List<string> Pipeline(string bomno)
        {
            string sql = string.Format("SELECT D.OTHER_NAME L FROM T_BOM_PIPELINE P INNER JOIN M_DEPARTMENT D ON P.PIPELINENAME = D.DEPARTMENTNAME WHERE P.BOMNO = '{0}';", bomno);
            List<string> Line = new List<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LSMES_5ANEW"].ConnectionString))
                {
                    conn.Open();
                    SqlCommand mComm = new SqlCommand(sql, conn);
                    SqlDataReader reader;
                    reader = mComm.ExecuteReader();
                    //reader.Read();

                    while (reader.Read())
                    {
                        Line.Add(reader["L"].ToString());
                    }
                    return Line;
                    //return reader["L"].ToString();
                }
            }
            catch (Exception ex)
            {
                //return "WJ1";
                return Line;
            }
        }
        /// <summary>
        /// 判定包装箱电池是否有效，在力神系统中
        /// </summary>
        /// <returns></returns>
        public AppException IsValidLSMES()
        {
            StatusCell status;
            try
            {
                mQtyBox = this.QtyOfBoxByLsmes();
                //判断包装箱是否为空
                if (mQtyBox == 0){
                    status = StatusCell.EmptyBOX;
                    return new AppException(status,new Exception("Class: CellInfoList, Function: IsValidLSMES(string boxid, string bomno)]"));
                }
                //判断包装箱内电池数量是否与包装箱标签的数量相符
                else if (mQtyBox != int.Parse(mBoxid.Substring(mBoxid.Length - 4, 3)))
                {
                    status = StatusCell.ErrorQTY;
                    return new AppException(status, new Exception("Class: CellInfoList, Function: IsValidLSMES(string boxid, string bomno)]"));
                }
                //包装箱内数量与包装箱标签数量相符
                else
                {
                    status = StatusCell.Normal;
                    return new AppException(status);
                }                
            }
            catch(Exception ex)
            {
                status = StatusCell.ErrorQTY;
                return new AppException(status, new Exception("Class: CellInfoList, Function: IsValidLSMES(string boxid, string bomno)]"));
            }
        }
        /// <summary>
        /// 创建 SAP PACK 导入箱号
        /// </summary>
        /// <returns>包装箱 HANDLE</returns>
        public string CreateShipStatement()
        {
            string sql = null;
            try
            {
                string uuid = System.Guid.NewGuid().ToString();
                sql = (string.Format("INSERT INTO Z_CELL_PACK (HANDLE,SITE,CATEGORY,RESRCE,LINE_ID,ITEM,CUSTOMER,GRADE,LABEL_CODE,NUM,NUM_ID,STATUS,QTY,CREATE_USER,CREATED_DATE_TIME,MODIFY_USER,MODIFIED_DATE_TIME,WEEK) VALUES ('{0}','{1}','BOX','{2}','{3}','{4}','{5}','AAA','{6}','{7}','{7}','CLOSE','{8}','{9}','{10}','{9}','{10}','{11}');", uuid, mWebSite, mEquipment, mLineNo, mItemNo, mCustomerNo, mLabelCode, mBoxid, mList.CELL_LIST.Count, mOperator, DateTime.Now.ToString(), SystemInfo.WeekOfYeayApple()));
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
                {
                    conn.Open();
                    OdbcCommand mComm = new OdbcCommand(sql, conn);
                    if (mComm.ExecuteNonQuery() == 1)
                    {
                        conn.Close();
                        return uuid;
                    }
                    else
                    {
                        conn.Close();
                        return null;
                    }
                }
            }
            catch(Exception ex)
            {
                new AppException("Class: CellInfoList, Function: CreateShipStatement(string boxid)],Sql statement:" + sql, ex);
                return null;
            }
        }
        /// <summary>
        /// 创建 SAP PACK 导入电芯
        /// </summary>
        /// <param name="Handle">包装箱 HANDLE</param>
        /// <param name="cellList">电芯码号</param>
        /// <returns>True：成功；False：失败</returns>
        public bool CreateShipCellStatement(string Handle)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (mList.CELL_LIST.Count == 0)
                {
                    return false;
                }
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
                {
                    conn.Open();
                    OdbcTransaction trans = conn.BeginTransaction();
                    OdbcCommand mComm = new OdbcCommand(null, conn, trans);
                    foreach (CellInfo cell in mList.CELL_LIST)
                    {
                        mComm.CommandText = string.Format("INSERT INTO Z_CELL_PACK_SN (HANDLE,SITE,RESRCE,CELL_PACK_BO,LINE_ID,ITEM,CUSTOMER,GRADE,SN,QTY,CREATE_USER,CREATED_DATE_TIME,MODIFY_USER,MODIFIED_DATE_TIME,WEEK) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','AAA','{7}','1','{8}','{9}','{8}','{9}','{10}');", System.Guid.NewGuid().ToString(), mWebSite, mEquipment, Handle, mLineNo, mItemNo, mCustomerNo, cell.BATTERYNO, mOperator, DateTime.Now.ToString(), SystemInfo.WeekOfYeayApple());
                        mComm.ExecuteNonQuery();
                    }
                    trans.Commit();
                    conn.Close();
                    return true;
                }
            }
            catch(Exception ex)
            {
                new AppException("Class: CellInfoList, Function: CreateShipCellStatement(,string Handle, CellInfoList cellList)]" + sql.ToString(), ex);
                return false; 
            }
        }
        /// <summary>
        /// 创建 SAP PACK 导入V1数据
        /// </summary>
        /// <returns>True：成功；False：失败</returns>
        public bool CreateOcv1Statement()
        {
            if (mList.CELL_LIST.Count == 0)
            {
                return false;
            }
            StringBuilder sql = new StringBuilder();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
                {
                    conn.Open();
                    OdbcTransaction trans = conn.BeginTransaction();
                    OdbcCommand mComm = new OdbcCommand(null, conn, trans);
                    foreach (CellInfo cell in mList.CELL_LIST)
                    {
                        mComm.CommandText = string.Format("INSERT INTO Z_OP09_SFC_PARAM (HANDLE,SITE,ITEM_NO,OPERATION_NO,RESOURCE_NO,PROD_BATCH_NO,SN,SFC,SHOP_ORDER_NO,CREATE_USER,CREATED_DATE_TIME,IS_CURRENT,A004,A003) VALUES ('{0}','{1}','{2}','OP09','F01R09103','{3}','{4}','{5}','{6}','{7}','{8}','Y','{9}','{10}');", System.Guid.NewGuid().ToString(), mWebSite, mItemNo, cell.BATTERYNO.Substring(0, 10), cell.BATTERYNO, mOrderNo + "_" + cell.BATTERYNO, mOrderNo, mOperator, DateTime.Now.ToString(), cell.E5VOLTAGE1, cell.E5TESTTIME);
                        mComm.ExecuteNonQuery();
                    }
                    trans.Commit();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                new AppException("Class: CellInfoList, Function: CreateOcv1Statement(CellInfoList cellList)]" + sql.ToString(), ex);
                return false;
            }
        }
        /// <summary>
        /// 创建 SAP PACK 导入V2数据
        /// </summary>
        /// <returns>True：成功；False：失败</returns>
        public bool CreateOcv2Statement()
        {
            if (mList.CELL_LIST.Count == 0)
            {
                return false;
            }
            StringBuilder sql = new StringBuilder();
            try
            {
                using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
                {
                    conn.Open();
                    OdbcTransaction trans = conn.BeginTransaction();
                    OdbcCommand mComm = new OdbcCommand(null, conn, trans);
                    foreach (CellInfo cell in mList.CELL_LIST)
                    {
                        mComm.CommandText = string.Format("INSERT INTO Z_OP11_SFC_PARAM (HANDLE,SITE,ITEM_NO,OPERATION_NO,RESOURCE_NO,PROD_BATCH_NO,SN,SFC,SHOP_ORDER_NO,CREATE_USER,CREATED_DATE_TIME,IS_CURRENT,A005,A004) VALUES ('{0}','{1}','{2}','OP11','F01R11103','{3}','{4}','{5}','{6}','{7}','{8}','Y','{9}','{10}');\n\r ", System.Guid.NewGuid().ToString(), mWebSite, mItemNo, cell.BATTERYNO.Substring(0, 10), cell.BATTERYNO, mOrderNo + "_" + cell.BATTERYNO, mOrderNo, mOperator, DateTime.Now.ToString(), cell.E6VOLTAGE2, cell.E6TESTTIME);
                        mComm.ExecuteNonQuery();
                    }
                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                new AppException("Class: CellInfoList, Function: CreateOcv2Statement(CellInfoList cellList)]" + sql.ToString(), ex);
                return false;
            }
        }
    }
}