using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Text;

namespace LSMES_5ANEW_PLUS.Business
{
    /// <summary>
    /// 对回传数据进行整合
    /// </summary>
    public class DataIntegration
    {
        /// <summary>
        /// 获取 Logic
        /// </summary>
        /// <param name="item">料号</param>
        /// <returns></returns>
        private string IndexLogic(string item)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("DataIntegration::Index => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT L.STATEMENT FROM DATA_SOURCES S INNER JOIN LOGIC L ON S.LOGIC = L.HANDLE INNER JOIN ITEM I ON S.HANDLE_ITEM = I.HANDLE WHERE I.ITEM = '{0}' AND S.REMARKS = 'INDEX' AND S.STATE = '1' AND L.STATE = '1';", item), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("DataIntegration::Index => Logic is empty.");
                    string statement = null;
                    while (reader.Read())
                    {
                        statement = reader["STATEMENT"].ToString();
                    }
                    reader.Close();
                    return statement;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取数据配置项目
        /// </summary>
        /// <param name="item">料号</param>
        /// <returns></returns>
        private DataTable DataSources(string item)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("DataIntegration::DataSources => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT S.* FROM DATA_SOURCES S INNER JOIN ITEM I ON S.HANDLE_ITEM = I.HANDLE WHERE I.STATE = '1' AND S.STATE = '1' AND I.ITEM = '{0}';", item), conn);
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
        /// 生成数据表
        /// </summary>
        /// <param name="mDS">数据源配置表</param>
        /// <param name="pallet">栈板清单</param>
        /// <returns></returns>
        public DataTable CreateData(string item, string pallet)
        {
            using (OdbcConnection conn = new OdbcConnection(Configuer.ConnectionStringBySAP))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("DataIntegration::CreateData => SAP ME db can not be open.");
                    }
                    DataTable mDs = DataSources(item);
                    OdbcCommand comm = new OdbcCommand(CreateSQL(item, mDs, pallet), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    DataTable mDs2 = new DataTable();
                    mDs2.Load(reader);
                    Arrange(ref mDs2, item);
                    return mDs2;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        private string CreateSQL(string item, DataTable mDS, string pallet)
        {
            try
            {
                string shortTable = null;
                StringBuilder field = new StringBuilder();
                StringBuilder table = new StringBuilder();
                for (int i = 0; i < mDS.Rows.Count; ++i)
                {
                    if (string.IsNullOrEmpty(mDS.Rows[i]["DATATABLE"].ToString()) && string.IsNullOrEmpty(mDS.Rows[i]["FIELD"].ToString()))
                    {
                        if (!string.IsNullOrEmpty(mDS.Rows[i]["REMARKS"].ToString()))
                        {
                            field.Append("'");
                            field.Append(mDS.Rows[i]["REMARKS"].ToString());
                            field.Append("' \"");
                            field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                            field.Append("\",");
                        }
                        else
                        {
                            field.Append("'' \"");
                            field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                            field.Append("\",");
                        }
                    }
                    else
                    {
                        // 字段
                        if (!string.IsNullOrEmpty(mDS.Rows[i]["PRECISION"].ToString()))
                            field.Append(" ROUND(");
                        if (!string.IsNullOrEmpty(mDS.Rows[i]["FIELD"].ToString()) && mDS.Rows[i]["REMARKS"].ToString() == "INDEX")
                        {
                            field.Append("_A.");
                            field.Append(mDS.Rows[i]["FIELD"].ToString());
                            field.Append(" \"");
                            field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                            field.Append(" \",");
                        }
                        else
                        {
                            field.Append(mDS.Rows[i]["DATATABLE"].ToString());
                            field.Append(".");
                            field.Append(mDS.Rows[i]["FIELD"].ToString());
                            if (!string.IsNullOrEmpty(mDS.Rows[i]["OPERATORS"].ToString()) && !string.IsNullOrEmpty(mDS.Rows[i]["PARAMETER"].ToString()))
                            {
                                switch (mDS.Rows[i]["OPERATORS"].ToString())
                                {
                                    case "MUL":
                                        field.Append("*");
                                        field.Append(mDS.Rows[i]["PARAMETER"].ToString());
                                        break;
                                    case "DIV":
                                        field.Append("/");
                                        field.Append(mDS.Rows[i]["PARAMETER"].ToString());
                                        break;
                                    case "ADD":
                                        field.Append("+");
                                        field.Append(mDS.Rows[i]["PARAMETER"].ToString());
                                        break;
                                    case "SUB":
                                        field.Append("-");
                                        field.Append(mDS.Rows[i]["PARAMETER"].ToString());
                                        break;
                                }
                            }
                            if (!string.IsNullOrEmpty(mDS.Rows[i]["PRECISION"].ToString()))
                            {
                                field.Append(",");
                                field.Append(mDS.Rows[i]["PRECISION"].ToString());
                                field.Append(") ");
                            }
                            field.Append(" \"");
                            field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                            field.Append("\",");
                            // 单位
                            if (!string.IsNullOrEmpty(mDS.Rows[i]["UNIT"].ToString()))
                            {
                                field.Append("'");
                                field.Append(mDS.Rows[i]["UNIT"].ToString());
                                field.Append("' \"");
                                field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                                field.Append("单位\",");
                            }
                            else
                            {
                                field.Append("'' \"");
                                field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                                field.Append("单位\",");
                            }
                            // 规格下限
                            if (!string.IsNullOrEmpty(mDS.Rows[i]["LSL"].ToString()))
                            {
                                field.Append("'");
                                field.Append(mDS.Rows[i]["LSL"].ToString());
                                field.Append("' \"");
                                field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                                field.Append("规格下限\",");
                            }
                            else
                            {
                                field.Append("'0' \"");
                                field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                                field.Append("规格下限\",");
                            }
                            // 规格上限
                            if (!string.IsNullOrEmpty(mDS.Rows[i]["USL"].ToString()))
                            {
                                field.Append("'");
                                field.Append(mDS.Rows[i]["USL"].ToString());
                                field.Append("' \"");
                                field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                                field.Append("规格上限\",");
                            }
                            else
                            {
                                field.Append("'0' \"");
                                field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                                field.Append("规格上限\",");
                            }
                            // 数据表
                            if (!table.ToString().Contains(mDS.Rows[i]["DATATABLE"].ToString()))
                            {
                                if (table.Length == 0)
                                {
                                    table.Append(mDS.Rows[i]["DATATABLE"].ToString());
                                    shortTable = mDS.Rows[i]["DATATABLE"].ToString();
                                }
                                else
                                {
                                    table.Append(" RIGHT JOIN ");
                                    table.Append(mDS.Rows[i]["DATATABLE"].ToString());
                                    table.Append(" ON ");
                                    table.Append(shortTable);
                                    table.Append(".SN = ");
                                    table.Append(mDS.Rows[i]["DATATABLE"].ToString());
                                    table.Append(".SN ");
                                    shortTable = mDS.Rows[i]["DATATABLE"].ToString();
                                }
                            }
                        }
                    }
                }
                string fields = field.ToString().Substring(0, field.Length - 1);
                pallet = "'" + pallet.Replace(",", "','") + "'";
                string subSql = string.Format(IndexLogic(item), pallet, item);
                StringBuilder sql = new StringBuilder("SELECT ");
                sql.Append(fields);
                sql.Append(" FROM ");
                sql.Append(table);
                sql.Append("RIGHT JOIN (");
                sql.Append(subSql);
                sql.Append(") _A ON _A.SN = ");
                sql.Append(shortTable);
                sql.Append(".SN");
                return sql.ToString();
            }
            catch(Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }

        }
        private void Arrange(ref DataTable dtSource, string item)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("DataIntegration::Arrange => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT SN,HEADLINE FROM DATA_TEMPLATE T INNER JOIN ITEM I ON T.HANDLE_ITEM = I.HANDLE WHERE I.ITEM = '{0}' ORDER BY SN ASC;", item), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        dtSource.Columns[reader["HEADLINE"].ToString()].SetOrdinal(Convert.ToInt32(reader["SN"].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
        }
    }
}