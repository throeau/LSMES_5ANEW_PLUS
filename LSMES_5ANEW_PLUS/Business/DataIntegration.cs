using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Text;
using System.Collections;

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
                    SqlCommand comm = new SqlCommand(string.Format("SELECT L.STATEMENT FROM DATA_SOURCES S INNER JOIN LOGIC L ON S.LOGIC = L.HANDLE AND L.PIPELINE = S.PIPELINE INNER JOIN ITEM I ON S.HANDLE_ITEM = I.HANDLE WHERE I.ITEM = '{0}' AND S.REMARKS = 'INDEX' AND S.STATE = '1' AND L.STATE = '1';", item), conn);
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
        public DataTable CreateData(string item, string pallet, string taskno)
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
                    //string sql = CreateSQL(item, mDs, pallet);
                    TaskManagement man = new TaskManagement();
                    DataTable mDs3 = new DataTable();
                    mDs3 = man.GetDataSupplement(taskno);
                    string sql = CreateSQL2(item, mDs, pallet);
                    if (string.IsNullOrEmpty(sql)) return null;
                    StatementsLog entity = GetStatementsInfo(item);
                    entity.STATEMENTS = sql;
                    entity.TYPE = "获取回传数据";
                    entity.TASKNO = taskno;
                    if (!Log(entity)) return null;
                    OdbcCommand comm = new OdbcCommand(sql, conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    DataTable mDs2 = new DataTable();
                    mDs2.Load(reader);
                    Arrange(ref mDs2, ref mDs3, item);
                    return mDs2;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 创建回数据查询 SQL
        /// </summary>
        /// <param name="item"></param>
        /// <param name="mDS"></param>
        /// <param name="pallet"></param>
        /// <returns></returns>
        private string CreateSQL(string item, DataTable mDS, string pallet)
        {
            try
            {
                string shortTable = null;
                StringBuilder field = new StringBuilder();
                StringBuilder table = new StringBuilder();
                List<string> listTable = new List<string>();
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
                        if ((!string.IsNullOrEmpty(mDS.Rows[i]["FIELD"].ToString()) && mDS.Rows[i]["REMARKS"].ToString() == "INDEX") || (mDS.Rows[i]["FIELD"].ToString() == "PALLETNO" || mDS.Rows[i]["FIELD"].ToString() == "BOXID"))
                        {
                            field.Append("_A.");
                            field.Append(mDS.Rows[i]["FIELD"].ToString());
                            field.Append(" \"");
                            field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                            field.Append("\",");
                        }
                        else
                        {
                            listTable.Add(mDS.Rows[i]["DATATABLE"].ToString());
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
                            //// 单位
                            //if (!string.IsNullOrEmpty(mDS.Rows[i]["UNIT"].ToString()))
                            //{
                            //    field.Append("'");
                            //    field.Append(mDS.Rows[i]["UNIT"].ToString());
                            //    field.Append("' \"");
                            //    field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                            //    field.Append("单位\",");
                            //}
                            //else
                            //{
                            //    field.Append("'' \"");
                            //    field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                            //    field.Append("单位\",");
                            //}
                            //// 规格下限
                            //if (!string.IsNullOrEmpty(mDS.Rows[i]["LSL"].ToString()))
                            //{
                            //    field.Append("'");
                            //    field.Append(mDS.Rows[i]["LSL"].ToString());
                            //    field.Append("' \"");
                            //    field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                            //    field.Append("规格下限\",");
                            //}
                            //else
                            //{
                            //    field.Append("'0' \"");
                            //    field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                            //    field.Append("规格下限\",");
                            //}
                            //// 规格上限
                            //if (!string.IsNullOrEmpty(mDS.Rows[i]["USL"].ToString()))
                            //{
                            //    field.Append("'");
                            //    field.Append(mDS.Rows[i]["USL"].ToString());
                            //    field.Append("' \"");
                            //    field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                            //    field.Append("规格上限\",");
                            //}
                            //else
                            //{
                            //    field.Append("'0' \"");
                            //    field.Append(mDS.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                            //    field.Append("规格上限\",");
                            //}
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
                sql.Append(".SN AND ");
                for(int i=0; i<listTable.Count; ++i)
                {
                    sql.Append(string.Format("{0}.IS_CURRENT = 'Y' AND ", listTable[i]));
                }
                return sql.ToString().Substring(0, sql.ToString().Length - 4);
            }
            catch(Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }

        }
        /// <summary>
        /// 创建回数据查询 SQL
        /// </summary>
        /// <param name="item"></param>
        /// <param name="mDS"></param>
        /// <param name="pallet"></param>
        /// <returns></returns>
        private string CreateSQL2(string item, DataTable mDs, string pallet)
        {
            if (string.IsNullOrEmpty(item) || string.IsNullOrEmpty(pallet)) return null;
            StringBuilder field = new StringBuilder();
            Hashtable hashPropert = new Hashtable();
            Hashtable hashDataTable = new Hashtable();
            List<string> table = new List<string>();
            try
            {
                for (int i = 0; i < mDs.Rows.Count; ++i)
                {
                    // 数据字段
                    if (!string.IsNullOrEmpty(mDs.Rows[i]["DATATABLE"].ToString()) && !string.IsNullOrEmpty(mDs.Rows[i]["FIELD"].ToString()))
                    {
                        string _field;
                        string _fieldPropert = "";
                        _field = mDs.Rows[i]["DATATABLE"].ToString() + "." + mDs.Rows[i]["FIELD"].ToString();
                        string _op = "";
                        if (!string.IsNullOrEmpty(mDs.Rows[i]["OPERATORS"].ToString()) && !string.IsNullOrEmpty(mDs.Rows[i]["PARAMETER"].ToString()))
                        {
                            switch (mDs.Rows[i]["OPERATORS"].ToString())
                            {
                                case "MUL":
                                    _op = "*";
                                    _op += mDs.Rows[i]["PARAMETER"].ToString();
                                    break;
                                case "DIV":
                                    _op = "/";
                                    _op += mDs.Rows[i]["PARAMETER"].ToString();
                                    break;
                                case "ADD":
                                    _op = "+";
                                    _op += mDs.Rows[i]["PARAMETER"].ToString();
                                    break;
                                case "SUB":
                                    _op = "-";
                                    _op += mDs.Rows[i]["PARAMETER"].ToString();
                                    break;
                            }
                            _field += _op;
                        }
                        if (!string.IsNullOrEmpty(mDs.Rows[i]["PRECISION"].ToString()))
                        {
                            _field = string.Format("ROUND({0},{1})", _field, mDs.Rows[i]["PRECISION"].ToString());
                        }
                        _field += " \"" + mDs.Rows[i]["DATA_PROPERTY_NAME"].ToString()+"\",";
                        _fieldPropert += " \"" + mDs.Rows[i]["DATA_PROPERTY_NAME"].ToString() + "\",";
                        if (hashDataTable.ContainsKey(mDs.Rows[i]["DATATABLE"]))    // 表名已存在 hashtable 中
                        {
                            string listFields = hashDataTable[mDs.Rows[i]["DATATABLE"]].ToString();
                            listFields += _field;
                            hashDataTable[mDs.Rows[i]["DATATABLE"]] = listFields;
                            listFields = hashPropert[mDs.Rows[i]["DATATABLE"]].ToString();
                            listFields += _fieldPropert;
                            hashPropert[mDs.Rows[i]["DATATABLE"]] = listFields;
                        }
                        else // 表名不存在 hashtable 中，新建
                        {
                            hashDataTable.Add(mDs.Rows[i]["DATATABLE"], _field);
                            hashPropert.Add(mDs.Rows[i]["DATATABLE"], _fieldPropert);
                        }
                    }
                    // 固定值
                    if (!string.IsNullOrEmpty(mDs.Rows[i]["REMARKS"].ToString()))
                    {
                        field.Append("'");
                        field.Append(mDs.Rows[i]["REMARKS"].ToString());
                        field.Append("' \"");
                        field.Append(mDs.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                        field.Append("\",");
                    }
                    // 其它
                    if (string.IsNullOrEmpty(mDs.Rows[i]["DATATABLE"].ToString()) && string.IsNullOrEmpty(mDs.Rows[i]["REMARKS"].ToString()))
                    {
                        field.Append("'' \"");
                        field.Append(mDs.Rows[i]["DATA_PROPERTY_NAME"].ToString());
                        field.Append("\",");
                    }
                }
                // 查询出货码号、箱号，栈板号
                pallet = "'" + pallet.Replace(",", "','") + "'";
                string subSql = string.Format(IndexLogic(item), pallet, item);
                StringBuilder sql = new StringBuilder("WITH A AS( ");
                sql.Append(subSql);
                sql.Append(")");
                foreach (DictionaryEntry entry in hashDataTable)
                {
                    table.Add(entry.Key.ToString());
                    string _table = "_" + entry.Key.ToString();
                    sql.Append("," + _table + " AS (");
                    sql.Append("SELECT ");
                    sql.Append(entry.Key.ToString());
                    sql.Append(".SN,");
                    sql.Append(entry.Value.ToString().Substring(0, entry.Value.ToString().Length - 1));
                    sql.Append(" FROM A");
                    if (entry.Key.ToString() != "A")
                    {
                        sql.Append(",");
                        sql.Append(entry.Key);
                        sql.Append(" WHERE IS_CURRENT = 'Y' AND ");
                        sql.Append("A.SN = ");
                        sql.Append(entry.Key.ToString() + ".SN");
                    }
                    sql.Append(")");
                }
                sql.Append(" SELECT {0} ");
                sql.Append(field.ToString().Substring(0, field.Length - 1));
                sql.Append(" FROM A");
                string t = "";
                foreach (string _table in table)
                {
                    sql.Append(" LEFT JOIN ");
                    sql.Append("_" + _table);
                    sql.Append(" ON A.SN = ");
                    sql.Append("_" + _table + ".SN ");
                    t += "_" + _table + "."+ hashPropert[_table];
                }
                return string.Format(sql.ToString(), t).Replace("'INDEX'","A.SN");
            }
            catch(Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 1、对数据排序
        /// 2、如果存在补充数据，则进行补充
        /// </summary>
        /// <param name="dtSource">源数据</param>
        /// <param name="dtSource2">补充数据</param>
        /// <param name="item">料号</param>
        private void Arrange(ref DataTable dtSource, ref DataTable dtSource2, string item)
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
                        if (!dtSource.Columns.Contains(reader["HEADLINE"].ToString())) dtSource.Columns.Remove(reader["HEADLINE"].ToString());
                        else dtSource.Columns[reader["HEADLINE"].ToString()].SetOrdinal(Convert.ToInt32(reader["SN"].ToString()));

                    }
                    reader.Close();
                    //reader = comm.ExecuteReader();
                    //while (reader.Read())
                    //{
                    //    string headline = reader["HEADLINE"].ToString();
                    //    int sn = Convert.ToInt32(reader["SN"].ToString());
                    //}
                    //reader.Close();
                    if (dtSource2.Rows.Count > 0)   // 存在补充情况，只有容量补充（固定死）
                    {
                        for (int i = 0; i < dtSource2.Rows.Count; ++i)
                        {
                            dtSource.Rows[i]["容量"] = dtSource2.Rows[i]["DATA_VALUE"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
        }
        /// <summary>
        /// 通过任务号获取栈板信息
        /// </summary>
        /// <param name="taskno">任务号</param>
        /// <returns></returns>
        public Hashtable GetPalletInfoByTaskNo(string taskno)
        {
            if (string.IsNullOrEmpty(taskno)) return null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("DataIntegration::GetPalletInfoByTaskNo => SAP ME db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT TD.ITEM_NO,TD.SFC FROM TASK t INNER JOIN TASK_DETAILS td ON T.HANDLE = TD.HANDLE_TASK WHERE T.SN = '{0}';", taskno), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    Hashtable listPallet = new Hashtable();
                    while (reader.Read())
                    {
                        listPallet.Add(reader["SFC"], reader["ITEM_NO"]);
                    }
                    return listPallet;
                }
                catch(Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="entity">日志实体</param>
        /// <returns></returns>
        public bool Log(StatementsLog entity)
        {
            if (entity == null) return false;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("DataIntegration::Log => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO STATEMENTS_LOG (ITEM,BOMNO,STATEMENTS,PIPELINE,TYPE,REMARKS,TASKNO) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", entity.ITEM, entity.BOMNO, entity.STATEMENTS, entity.PIPELINE, entity.TYPE, entity.REMARKS, entity.TASKNO), conn);
                    if (comm.ExecuteNonQuery() == 1) return true;
                    return false;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return false;
                }
            }
        }
        /// <summary>
        /// 获取基本信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public StatementsLog GetStatementsInfo(string item)
        {
            if (string.IsNullOrEmpty(item)) return null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("DataIntegration::GetStatementsInfo => SAP ME db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT DISTINCT I.ITEM,I.BOMNO,DS.PIPELINE FROM ITEM I INNER JOIN DATA_SOURCES DS ON I.HANDLE = DS.HANDLE_ITEM AND DS.STATE = '1' WHERE I.STATE = '1' AND I.ITEM = '{0}';", item), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) return null;
                    StatementsLog entity = new StatementsLog();
                    while (reader.Read())
                    {
                        entity.ITEM = reader["ITEM"].ToString();
                        entity.BOMNO = reader["BOMNO"].ToString();
                        entity.PIPELINE = reader["PIPELINE"].ToString();
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
    }
}