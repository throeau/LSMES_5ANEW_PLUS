using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using LSMES_5ANEW_PLUS.App_Base;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using Apache.NMS;
using Apache.NMS.Util;
using Apache.NMS.ActiveMQ;
using Newtonsoft.Json;
using System.Collections;
using System.Threading;
using System.Data.Odbc;


namespace LSMES_5ANEW_PLUS.Business
{
    public class ORT
    {
        IConnectionFactory mqFactory;
        IConnection mqConnection;
        ISession mqSession;
        IMessageProducer mqProducer;
        IMessageConsumer mqConsumer;
        string mqResponse;
        /// <summary>
        /// 查询区域
        /// </summary>
        /// <returns></returns>
        public static List<WorkArea> GetWorkArea()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetWorkArea => Unable to connect to ort platform.");
                    }
                    List<WorkArea> EntityWorkAreaList = new List<WorkArea>();
                    NpgsqlCommand comm = new NpgsqlCommand("select handle ,area from work_area;", conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        WorkArea EntityWorkArea = new WorkArea();
                        EntityWorkArea.handle = reader[0].ToString();
                        EntityWorkArea.area = reader[1].ToString();
                        EntityWorkAreaList.Add(EntityWorkArea);
                    }
                    reader.Close();
                    return EntityWorkAreaList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 查询任务状态
        /// </summary>
        /// <returns></returns>
        public static List<TaskState> GetTaskState()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetWorkArea => Unable to connect to ort platform.");
                    }
                    List<TaskState> EntityTaskStateList = new List<TaskState>();
                    NpgsqlCommand comm = new NpgsqlCommand("select state,description from task_state ts ;", conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        TaskState entity = new TaskState();
                        entity.state = reader["state"].ToString();
                        entity.description = reader["description"].ToString();
                        EntityTaskStateList.Add(entity);
                    }
                    reader.Close();
                    return EntityTaskStateList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 查询工序
        /// </summary>
        /// <returns></returns>
        public static List<Operations> GetOperations()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetOperations => Unable to connect to ort platform.");
                    }
                    List<Operations> EntityOperationsList = new List<Operations>();
                    NpgsqlCommand comm = new NpgsqlCommand("select handle ,operation_name from Operations;", conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        Operations EntityOperations = new Operations();
                        EntityOperations.handle = reader[0].ToString();
                        EntityOperations.operations = reader[1].ToString();
                        EntityOperationsList.Add(EntityOperations);
                    }
                    reader.Close();
                    return EntityOperationsList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 查询物料类型
        /// </summary>
        /// <returns></returns>
        public static List<TypeArea> GetTypeArea()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetTypeArea => Unable to connect to ort platform.");
                    }
                    List<TypeArea> EntityTypeAreaList = new List<TypeArea>();
                    NpgsqlCommand comm = new NpgsqlCommand("select handle ,type_area ,comments from type_area;", conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        TypeArea EntityTypeArea = new TypeArea();
                        EntityTypeArea.handle = reader[0].ToString();
                        EntityTypeArea.type_area = reader[1].ToString();
                        EntityTypeArea.comments = reader[2].ToString();
                        EntityTypeAreaList.Add(EntityTypeArea);
                    }
                    reader.Close();
                    return EntityTypeAreaList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 查询型号与料号
        /// </summary>
        /// <returns></returns>
        public static List<Bom> GetBom()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetBom => Unable to connect to ort platform.");
                    }
                    List<Bom> EntityBomList = new List<Bom>();
                    NpgsqlCommand comm = new NpgsqlCommand("select handle ,bom_no ,item_no ,types from bom;", conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        Bom EntityBom = new Bom();
                        EntityBom.handle = reader[0].ToString();
                        EntityBom.bomno = reader[1].ToString();
                        EntityBom.itemno = reader[2].ToString();
                        EntityBom.types = reader[3].ToString();
                        EntityBomList.Add(EntityBom);
                    }
                    reader.Close();
                    return EntityBomList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 查询检验类型
        /// </summary>
        public static List<TypeTask> GetTypeTask()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetTypeTask => Unable to connect to ort platform.");
                    }
                    List<TypeTask> EntityTypeTaskList = new List<TypeTask>();
                    NpgsqlCommand comm = new NpgsqlCommand("select handle ,types_task from task_types;", conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        TypeTask EntityTypeTask = new TypeTask();
                        EntityTypeTask.handle = reader[0].ToString();
                        EntityTypeTask.types = reader[1].ToString();
                        EntityTypeTaskList.Add(EntityTypeTask);
                    }
                    reader.Close();
                    return EntityTypeTaskList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 查询测试项目
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static List<ORTTestItem> GetTestItem(string handle)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetTypeTask => Unable to connect to ort platform.");
                    }
                    string sql = null;
                    if (string.IsNullOrEmpty(handle))
                    {
                        sql = "select * from inspection_item ii where state = '1';";
                    }
                    else
                    {
                        sql = string.Format("select i.handle ,i.inspection_name from inspection_item_group g inner join task_types t on g.handle_task_type = t.handle inner join inspection_item i on i.handle = g.handle_inspection_item where handle_task_type = '{0}';", handle);
                    }
                    List<ORTTestItem> EntityTestItemList = new List<ORTTestItem>();
                    NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        ORTTestItem EntityTestItem = new ORTTestItem();
                        EntityTestItem.handle = reader[0].ToString();
                        EntityTestItem.inspection_name = reader[1].ToString();
                        EntityTestItemList.Add(EntityTestItem);
                    }
                    reader.Close();
                    return EntityTestItemList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取主数据
        /// </summary>
        /// <returns></returns>
        public static MasterOrt GetMasterOrt()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetTypeTask => Unable to connect to ort platform.");
                    }
                    MasterOrt MasterOrtEntity = new MasterOrt();
                    MasterOrtEntity.BomList = GetBom();
                    MasterOrtEntity.OperationsList = GetOperations();
                    MasterOrtEntity.TypeAreaList = GetTypeArea();
                    MasterOrtEntity.TypeTaskList = GetTypeTask();
                    MasterOrtEntity.WorkAreaList = GetWorkArea();
                    MasterOrtEntity.TaskStateList = GetTaskState();
                    return MasterOrtEntity;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 创建抽检任务
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="TaskList"></param>
        /// <returns></returns>
        public static ResultORT CreateTaskInspection(string handle, List<TaskInspection> TaskList)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                ResultORT result = new ResultORT();
                NpgsqlCommand comm;
                NpgsqlTransaction tran = null;
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("CreateTaskInspection => Unable to connect to ort platform.");
                    }
                    StringBuilder sbr = new StringBuilder();
                    foreach (TaskInspection task in TaskList)
                    {
                        sbr.Append(string.Format("insert into task_details (handle,handle_work_area,handle_operation,handle_types_task,handle_bom,handle_inspection_item,state,creator,handle_types_area,handle_task) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');", task.handle, task.handle_work_area, task.handle_operation, task.handle_types_task, task.handle_bom, task.handle_task_item, "RUN", task.creator, task.handle_types_area, handle));
                    }
                    sbr.Append(string.Format("insert into task (handle,state,creator) values  ('{0}','{1}','{2}');", handle, "RUN", "admin"));
                    //sbr.Append(string.Format("insert into locked (handle,handle_task,creator) values  ('{0}','{1}','{2}');", UuidHelper.NewUuidString(), handle, "admin"));
                    comm = new NpgsqlCommand(sbr.ToString(), conn);
                    tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    if (comm.ExecuteNonQuery() > 0)
                    {
                        result.Result = "success";
                    }
                    else
                    {
                        result.Result = "fail";
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.Result = "fail";
                    tran.Rollback();
                }
                return result;
            }
        }
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable GetTaskInfo(string type)
        {
            string sql = null;
            switch (type)
            {
                case "ALL":
                    sql = "select handle 任务编号,state 状态,creator 创建人,created_date_time 创建时间 from task order by created_date_time desc;";
                    break;
                case "RECIEVED":
                    sql = "select handle 任务编号,state 状态,creator 创建人,created_date_time 创建时间 from task where state = 'RECIEVED' order by created_date_time desc;";
                    break;
                case "RUN":
                    sql = "select handle 任务编号,state 状态,creator 创建人,created_date_time 创建时间 from task where state = 'RUN' order by created_date_time desc;";
                    break;
                case "PASS":
                    sql = "select handle 任务编号,state 状态,creator 创建人,created_date_time 创建时间 from task where state = 'PASS' order by created_date_time desc;";
                    break;
                case "FAIL":
                    sql = "select handle 任务编号,state 状态,creator 创建人,created_date_time 创建时间 from task where state = 'FAIL' order by created_date_time desc;";
                    break;
                case "PAUSE":
                    sql = "select handle 任务编号,state 状态,creator 创建人,created_date_time 创建时间 from task where state = 'PAUSE' order by created_date_time desc;";
                    break;
            }
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetTaskDetails => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    DataTable mDt = new DataTable();
                    mDt.Load(reader);
                    reader.Close();
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
        /// 获取指定任务的详细信息
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static DataTable GetTaskDetails(string handle)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetTaskDetails => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("SELECT d.handle 项目编号,wa.area 区域,o.operation_name || '/' || o.\"comments\" 工序,ta.type_area 类型,b.bom_no 型号,b.item_no 料号,ii.inspection_name 检验项目,tt.types_task 检验类型,d.creator 创建人,d.created_date_time 创建时间 FROM task t INNER JOIN task_details d ON t.handle = d.handle_task AND t.handle = '{0}' INNER JOIN work_area wa ON d.handle_work_area = wa.handle INNER JOIN operations o ON d.handle_operation = o.handle INNER JOIN type_area ta ON d.handle_types_area = ta.handle INNER JOIN \"bom\" b ON d.handle_bom = b.handle INNER JOIN inspection_item ii ON d.handle_inspection_item = ii.handle INNER JOIN task_types tt ON d.handle_types_task = tt.handle;", handle), conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    DataTable mDt = new DataTable();
                    mDt.Load(reader);
                    reader.Close();
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
        /// 获取指定任务详细信息，包含：检验任务
        /// </summary>
        /// <param name="barcode">样本码号</param>
        /// <returns></returns>
        public static DataTable GetTaskDetailsByBarcode(string barcode)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetTaskDetails => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("SELECT DISTINCT t.handle 任务编号,d.handle 项目编号,wa.area 区域,o.operation_name || '/' || o.\"comments\" 工序,ta.type_area 类型,b.bom_no 型号,b.item_no 料号,ii.inspection_name 检验项目,tt.types_task 检验类型,d.creator 创建人,d.created_date_time 创建时间 FROM task t INNER JOIN task_details d ON t.handle = d.handle_task INNER JOIN work_area wa ON d.handle_work_area = wa.handle INNER JOIN operations o ON d.handle_operation = o.handle INNER JOIN type_area ta ON d.handle_types_area = ta.handle INNER JOIN \"bom\" b ON d.handle_bom = b.handle INNER JOIN inspection_item ii ON d.handle_inspection_item = ii.handle INNER JOIN task_types tt ON d.handle_types_task = tt.handle inner join task_sample ts on ts.handle_task = t.handle and ts.sfc = '{0}' and t.state = 'RECIEVED';", barcode), conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    DataTable mDt = new DataTable();
                    mDt.Load(reader);
                    reader.Close();
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
        /// 设置判定结果
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ResultORT SetTaskResults(ResultTaskList task)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                NpgsqlTransaction tran = conn.BeginTransaction();
                ResultORT resultOrt = new ResultORT();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetTaskDetails => Unable to connect to ort platform. ");
                    }
                    string result = "PASS";
                    StringBuilder sbr = new StringBuilder();
                    foreach (ResultItem item in task.ItemList)
                    {
                        sbr.Append(string.Format("update task_details set state = '{1}' where handle = '{0}';", item.handle, item.result));
                        if (item.result == "FAIL")
                            result = "FAIL";
                    }
                    sbr.Append(string.Format("update task set state = '{1}' where handle = '{0}';", task.task.handle, result));
                    NpgsqlCommand comm = new NpgsqlCommand(sbr.ToString(), conn);
                    comm.Transaction = tran;
                    int rowAffect = comm.ExecuteNonQuery();
                    tran.Commit();
                    if (rowAffect > 0)
                    {
                        resultOrt.Result = "success";
                    }
                    else
                    {
                        resultOrt.Result = "fail";
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    resultOrt.Result = "fail";
                }
                return resultOrt;
            }
        }
        public static ResultORT SetSampling(List<SampleORT> sampleList)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                NpgsqlTransaction tran = conn.BeginTransaction();
                ResultORT resultOrt = new ResultORT();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetTaskDetails => Unable to connect to ort platform. ");
                    }
                    StringBuilder sbr = new StringBuilder();
                    foreach (SampleORT sample in sampleList)
                    {
                        sbr.Append(string.Format("insert into task_sample (handle,handle_task,handle_task_details,sfc,product_batch,state,result,comments,creator) values ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},'{8}');", UuidHelper.NewUuid().ToString(), sample.handle_task, sample.handle_item, sample.barcode, sample.barcode.Substring(0, 8), "SAMPLING", "NULL", "NULL", "admin"));
                    }
                    sbr.Append(string.Format("update task set state = 'SAMPLING' where handle = '{0}';", sampleList[0].handle_task));
                    NpgsqlCommand comm = new NpgsqlCommand(sbr.ToString(), conn);
                    comm.Transaction = tran;
                    int rowAffect = comm.ExecuteNonQuery();
                    tran.Commit();
                    if (rowAffect > 0)
                    {
                        resultOrt.Result = "success";
                    }
                    else
                    {
                        resultOrt.Result = "fail";
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    resultOrt.Result = "fail";
                }
                return resultOrt;
            }
        }
        /// <summary>
        /// 样本条件（每一类型必须满足的数量）
        /// </summary>
        /// <returns></returns>
        public static Hashtable SampleingCondition(string task_no)
        {
            Hashtable hashInspectionNum = new Hashtable();
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::StaticSampleing => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("select inspection_name,num from task t inner join task_details td on t.handle = td.handle_task inner join inspection_item ii on td.handle_inspection_item = ii.handle where t.handle = '{0}';", task_no), conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        hashInspectionNum.Add(reader["inspection_name"], reader["num"]);
                    }
                    reader.Close();
                    return hashInspectionNum;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static DataTable GetSampleInfo(string barcode)
        {
            barcode = barcode.Trim();
            if (string.IsNullOrEmpty(barcode))
            {
                return null;
            }
            string sql = null;
            Regex reg = new Regex("^[a-f0-9]{8}(-[a-f0-9]{4}){3}-[a-f0-9]{12}$", RegexOptions.Compiled);
            if (reg.IsMatch(barcode))   // uuid
            {
                sql = string.Format("select tt.types_task 任务类型,wa.area 取样区域,b.bom_no 型号,b.item_no 料号,ii.inspection_name 测试项目,count(ts.handle) 样本数量 from task t inner join task_details td on t.handle = td.handle_task inner join work_area wa on td.handle_work_area = wa.handle inner join \"bom\" b on td.handle_bom = b.handle inner join operations o on td.handle_operation = o.handle inner join task_types tt on td.handle_types_task = tt.handle inner join task_sample ts on t.handle = ts.handle_task and ts.handle_task_details = td.handle inner join inspection_item ii on td.handle_inspection_item = ii.handle where t.handle = '{0}' group by tt.types_task,wa.area,b.bom_no,b.item_no,ii.inspection_name;", barcode);
            }
            else // 普通字符串
            {
                //sql = string.Format("select tt.types_task 任务类型,wa.area 取样区域,b.bom_no 型号,b.item_no 料号,ii.inspection_name 测试项目,count(ts.handle) 样本数量 from task t inner join task_details td on t.handle = td.handle_task inner join work_area wa on td.handle_work_area = wa.handle inner join \"bom\" b on td.handle_bom = b.handle inner join operations o on td.handle_operation = o.handle inner join task_types tt on td.handle_types_task = tt.handle inner join task_sample ts on t.handle = ts.handle_task and ts.handle_task_details = td.handle inner join inspection_item ii on td.handle_inspection_item = ii.handle where t.handle = (select handle_task from task_sample where sfc = '{0}') group by tt.types_task,wa.area,b.bom_no,b.item_no,ii.inspection_name;", barcode);
                sql = string.Format("select t.handle 任务编号,tt.types_task 任务类型,wa.area 取样区域,b.bom_no 型号,b.item_no 料号,ii.inspection_name 测试项目,ii.num 样本数量,ii.phases 阶段 FROM task t INNER JOIN task_details td ON t.handle = td.handle_task INNER JOIN work_area wa ON td.handle_work_area = wa.handle INNER JOIN \"bom\" b ON td.handle_bom = b.handle INNER JOIN task_types tt ON td.handle_types_task = tt.handle INNER JOIN inspection_item ii ON td.handle_inspection_item = ii.handle INNER JOIN task_sample ts ON ts.handle_task = t.handle WHERE sfc = '{0}' and t.state = 'SAMPLING' GROUP BY tt.types_task,wa.area,b.bom_no,b.item_no,ii.inspection_name,t.handle,ii.num ;", barcode);

            }
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetSampleInfo => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    DataTable mDt = new DataTable();
                    mDt.Load(reader);
                    reader.Close();
                    return mDt;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static DataTable GetSampleDetails(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
            {
                return null;
            }
            string sql = null;
            Regex reg = new Regex("^[a-f0-9]{8}(-[a-f0-9]{4}){3}-[a-f0-9]{12}$", RegexOptions.Compiled);
            if (reg.IsMatch(barcode))   // uuid
            {
                sql = string.Format("select ts.handle handle_sample, ts.handle_task ,ts.handle_task_details ,ii.inspection_name ,ts.sfc from task_sample ts inner join task_details td on ts.handle_task_details = td.handle inner join inspection_item ii on td.handle_inspection_item = ii.handle where ts.handle_task = '{0}' ;", barcode);
            }
            else // 普通字符串
            {
                //sql = string.Format("select ts.handle handle_sample, ts.handle_task ,ts.handle_task_details ,ii.inspection_name ,ts.sfc from task_sample ts inner join task_sample ts2 on ts.handle_task = ts2.handle_task and ts2.sfc = '{0}' inner join task_details td on ts.handle_task_details = td.handle inner join inspection_item ii on td.handle_inspection_item = ii.handle ", barcode);
                sql = string.Format("select ts2.handle handle_sample, ts2.handle_task,ts2.handle_task_details ,ii.inspection_name,ts2.sfc from task_sample ts inner join task_sample ts2 on ts.handle_task = ts2.handle_task inner join task_details td on ts2.handle_task_details = td.handle inner join inspection_item ii on td.handle_inspection_item = ii.handle where ts.sfc = '{0}';", barcode);
            }
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetSampleInfo => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    DataTable mDt = new DataTable();
                    mDt.Load(reader);
                    reader.Close();
                    return mDt;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static ResultORT SetSampleRecieved(List<SampleRecieved> sampleList)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                NpgsqlTransaction tran = null;
                ResultORT result = new ResultORT();

                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SetSampleRecieved => Unable to connect to ort platform. ");
                    }
                    StringBuilder sbr = new StringBuilder();
                    foreach (SampleRecieved entity in sampleList)
                    {
                        sbr.Append(string.Format("insert into sample_recieve (handle,handle_sample,handle_task_details,state,creator) values ('{0}','{1}','{2}','{3}','{4}');", UuidHelper.NewUuid().ToString(), entity.handle_sample, entity.handle_task_details, "RECIEVED", entity.creator));
                    }
                    sbr.Append(string.Format("update task set state = 'RECIEVED' where handle = '{0}';", sampleList[0].handle_task));
                    tran = conn.BeginTransaction();
                    NpgsqlCommand comm = new NpgsqlCommand(sbr.ToString(), conn);
                    comm.Transaction = tran;
                    if (comm.ExecuteNonQuery() == sampleList.Count + 1)
                    {
                        tran.Commit();
                        result.Result = "success";
                    }
                    else
                    {
                        throw new Exception("SetSampleRecieved => fail to update.");
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    tran.Rollback();
                    result.Result = "fail";
                }
                return result;
            }
        }
        public static DataTable GetTaskInfo(TaskSearch task)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SetSampleRecieved => Unable to connect to ort platform. ");
                    }
                    string sql = null;
                    string condition, condition2, condition3, condition4, condition5, condition6 = null;
                    if (!string.IsNullOrEmpty(task.area))
                    {
                        condition = "wa.area = '" + task.area + "'";
                    }
                    else
                    {
                        condition = "1 = 1";
                    }
                    if (!string.IsNullOrEmpty(task.type_area))
                    {
                        condition2 = "ta.type_area = '" + task.type_area + "'";
                    }
                    else
                    {
                        condition2 = "1 = 1";
                    }
                    if (!string.IsNullOrEmpty(task.operation_name))
                    {
                        condition3 = "o.operation_name = '" + task.operation_name + "'";
                    }
                    else
                    {
                        condition3 = "1 = 1";
                    }
                    if (!string.IsNullOrEmpty(task.product_batch))
                    {
                        condition4 = "ts.product_batch = '" + task.product_batch + "'";
                    }
                    else
                    {
                        condition4 = "1 = 1";
                    }
                    if (!string.IsNullOrEmpty(task.item_no))
                    {
                        condition5 = "b.item_no = '" + task.item_no + "'";
                    }
                    else
                    {
                        condition5 = "1 = 1";
                    }
                    if (string.IsNullOrEmpty(task.state) || task.state == "unlocked")
                    {
                        sql = "select t.handle 任务编号,wa.area 区域,o.operation_name 工序,ta.type_area 类型,b.bom_no 型号,b.item_no 料号,ts.product_batch 批次,d.creator 创建人,d.created_date_time 创建时间 FROM task t INNER JOIN task_details d ON t.handle = d.handle_task INNER JOIN work_area wa ON d.handle_work_area = wa.handle INNER JOIN operations o ON d.handle_operation = o.handle INNER JOIN type_area ta ON d.handle_types_area = ta.handle INNER JOIN bom b ON d.handle_bom = b.handle INNER JOIN inspection_item ii ON d.handle_inspection_item = ii.handle INNER JOIN task_types tt ON d.handle_types_task = tt.handle inner join task_sample ts on t.handle = ts.handle_task where t.state = 'FAIL' and {0} and {1} and {2} and {3} and {4} and {5} AND T.HANDLE NOT IN (select DISTINCT HANDLE_TASK from LOCKED_LOG) group by t.handle,wa.area,o.operation_name,ta.type_area,b.bom_no,b.item_no,ts.product_batch,d.creator,d.created_date_time;";
                        condition6 = "1 = 1";
                    }
                    else if (task.state == "locked")
                    {
                        sql = "select t.handle 任务编号,wa.area 区域,o.operation_name 工序,ta.type_area 类型,b.bom_no 型号,b.item_no 料号,ts.product_batch 批次,d.creator 创建人,d.created_date_time 创建时间 FROM task t INNER JOIN task_details d ON t.handle = d.handle_task INNER JOIN work_area wa ON d.handle_work_area = wa.handle INNER JOIN operations o ON d.handle_operation = o.handle INNER JOIN type_area ta ON d.handle_types_area = ta.handle INNER JOIN bom b ON d.handle_bom = b.handle INNER JOIN inspection_item ii ON d.handle_inspection_item = ii.handle INNER JOIN task_types tt ON d.handle_types_task = tt.handle inner join task_sample ts on t.handle = ts.handle_task where t.state = 'FAIL' and {0} and {1} and {2} and {3} and {4} and {5} AND T.HANDLE IN (select HANDLE_TASK from LOCKED) group by t.handle,wa.area,o.operation_name,ta.type_area,b.bom_no,b.item_no,ts.product_batch,d.creator,d.created_date_time;";
                        condition6 = "1 = 1";
                    }
                    sql = string.Format(sql, condition, condition2, condition3, condition4, condition5, condition6);
                    NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
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
        public static DataTable GetSampleSFC(string handle)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetSampleSFC => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("select sfc 码号,created_date_time 取样时间 from task_sample where handle_task_details = '{0}'", handle), conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
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
        public static ResultORT SetLocded(List<TaskLocked> taskList)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                ResultORT result = new ResultORT();
                conn.Open();
                NpgsqlTransaction tran = null;
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("GetSampleSFC => Unable to connect to ort platform. ");
                    }
                    StringBuilder sbr = new StringBuilder();
                    foreach (TaskLocked task in taskList)
                    {
                        sbr.Append(string.Format("insert into locked (handle,handle_task,state,comments,creator,created_date_time) values ('{0}','{1}','{2}','{3}','{4}','{5}');", UuidHelper.NewUuidString(), task.handle_task, task.state, task.comments, task.creator, task.created_date_time));
                        sbr.Append(string.Format("insert into locked_log select * from locked where handle_task = '{0}';", task.handle_task));
                    }
                    tran = conn.BeginTransaction();
                    NpgsqlCommand comm = new NpgsqlCommand(sbr.ToString(), conn);
                    comm.Transaction = tran;
                    comm.ExecuteNonQuery();
                    tran.Commit();
                    result.Result = "success";
                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    tran.Rollback();
                    result.Result = "fail";
                    return result;
                }
            }
        }
        public static ResultORT UnLock(List<TaskLocked> taskList)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                ResultORT result = new ResultORT();
                conn.Open();
                NpgsqlTransaction tran = null;
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("UnLock => Unable to connect to ort platform. ");
                    }
                    StringBuilder sbr = new StringBuilder();
                    foreach (TaskLocked task in taskList)
                    {
                        //sbr.Append(string.Format("insert into locked_log select * from locked where handle_task = '{0}';", task.handle_task));
                        sbr.Append(string.Format("update locked set state = '{2}',comments = '{1}',created_date_time = '{3}' where handle_task = '{0}';", task.handle_task, task.comments, task.state, task.created_date_time));
                        sbr.Append(string.Format("insert into locked_log (handle,handle_task,handle_task_sample,state,comments,creator,created_date_time) select '{1}', handle_task,handle_task_sample,state,comments,creator,created_date_time from locked where handle_task = '{0}';", task.handle_task, UuidHelper.NewUuidString()));
                        sbr.Append(string.Format("delete from locked where handle_task = '{0}';", task.handle_task));
                    }
                    tran = conn.BeginTransaction();
                    NpgsqlCommand comm = new NpgsqlCommand(sbr.ToString(), conn);
                    comm.Transaction = tran;
                    comm.ExecuteNonQuery();
                    tran.Commit();
                    result.Result = "success";
                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    tran.Rollback();
                    result.Result = "fail";
                    return result;
                }
            }
        }
        //public static ResultInterceptORT PostQueues(string content)
        protected void OnMessages(object message)
        {
            try
            {
                ITextMessage ResponseMessage = message as ITextMessage;


            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
            finally
            {
                mqProducer.Close();
                mqConsumer.Close();
                mqSession.Close();
                mqConnection.Close();
                mqProducer.Dispose();
                mqConsumer.Dispose();
                mqSession.Dispose();
                mqConnection.Dispose();
                mqProducer = null;
                mqSession = null;
                mqConnection = null;
                mqConsumer = null;
            }
        }
        public string PostInterceptQueues(string content, string queue, bool pNeedResponse)
        {
            // 返回结果
            ResultInterceptORT result = new ResultInterceptORT();
            //if (SaveInterceptORT(content).Result == "fail")
            //{
            //    result.RESULT = "2";
            //    result.MESSAGE = "ORT::PostQueues => ORT 系统记录拦截请求失败，未向 SAP ME 发送拦截请求.";
            //    //return result;
            //    return null;
            //}
            try
            {
                Uri connectUri = new Uri(ConfigurationManager.AppSettings["uri"]);
                mqFactory = new NMSConnectionFactory(connectUri);
                using (mqConnection = mqFactory.CreateConnection(ConfigurationManager.AppSettings["uid"], ConfigurationManager.AppSettings["pwd"]))
                {
                    mqConnection.Start();
                    using (mqSession = mqConnection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                    {
                        // 目标 queue
                        IDestination destination = mqSession.GetQueue(ConfigurationManager.AppSettings[queue]);
                        // 临时 queue，向目标 queue 发送消息后，不能直接 consumer 目标 queue
                        // 直接 consumer 会自己消费自己，而且得不到结果
                        // 通过建立临时 queue，接收返回值
                        IDestination TempQueue = mqSession.CreateTemporaryQueue();
                        // 生产者
                        mqProducer = mqSession.CreateProducer(destination);
                        // 消息不持久化（Active MQ 重启后消息消失）
                        mqProducer.DeliveryMode = MsgDeliveryMode.NonPersistent;
                        if (pNeedResponse)
                        {
                            // 消费者（使用临时 queue）
                            mqConsumer = mqSession.CreateConsumer(TempQueue);
                            mqConsumer.Listener += new MessageListener(OnMessages);
                        }
                        // 生产者所要发送的消息
                        //ITextMessage tmsg = producer.CreateTextMessage();
                        ITextMessage tmsg = mqSession.CreateTextMessage();
                        tmsg.Text = content;
                        // 将临时 queue 作为返回目标赋予消息
                        tmsg.NMSReplyTo = TempQueue;
                        // 超时时间为 1s
                        TimeSpan receiveTimeout = new TimeSpan(0, 0, 1);
                        mqProducer.RequestTimeout = receiveTimeout;
                        // 发送消息
                        mqProducer.Send(tmsg);
                        ITextMessage message = null;
                        string msg = null;
                        try
                        {
                            int count = 0;
                            while (count < 3)
                            {
                                ++count;
                                //message = consumer.Receive(receiveTimeout) as ITextMessage;
                                if (message == null)
                                {
                                    continue;
                                    //throw new Exception("向 SAP ME 发送拦截任务，未能收到其反馈，请排查 ORT 与 SAP ME 接口状态.");
                                }
                                else
                                {
                                    break;
                                }
                                //if (SaveResultInterceptORT(content).Result == "fail")
                                //{
                                //    throw new Exception("ORT::PostQueues => ORT 系统记录拦截回复失败.");
                                //}
                                //result = JsonConvert.DeserializeObject<ResultInterceptORT>(message.Text);
                                //msg = message.Text;
                            }
                            throw new Exception("向 SAP ME 发送拦截任务，未能收到其反馈，请排查 ORT 与 SAP ME 接口状态.");
                        }
                        catch (Exception ex)
                        {
                            result.RESULT = "2";
                            result.MESSAGE = ex.Message;
                            SysLog log = new SysLog(ex.Message);
                        }
                        if (!pNeedResponse)
                        {
                            mqProducer.Close();
                            mqSession.Close();
                            mqConnection.Close();
                            mqProducer.Dispose();
                            mqSession.Dispose();
                            mqConnection.Dispose();
                            mqProducer = null;
                            mqSession = null;
                            mqConnection = null;
                        }
                        //return result;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                result.RESULT = "2";
                result.MESSAGE = "连接 SAP ME 失败.";
                //return result;
                return null;
            }
        }
        public static ResultORT SaveInterceptORT(string content)
        {
            InterceptORT entityIntercept = new InterceptORT();
            ResultORT result = new ResultORT();
            try
            {
                entityIntercept = JsonConvert.DeserializeObject<InterceptORT>(content);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("ORT::SaveInterceptORT => " + ex.Message);
                result.Result = "fail";
                return result;
            }
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::SaveInterceptORT => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("insert into intercept_mq_send_log (handle,message_id,req_id,send_date_time,ort_task,operations,intercept_operations,site,info) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", UuidHelper.NewUuidString(), entityIntercept.MESSAGE_ID, entityIntercept.REQ_ID, entityIntercept.SEND_DATE_TIME, entityIntercept.ORT_NO, entityIntercept.OPERATION, entityIntercept.INTERCEPT_OPERATION, entityIntercept.SITE, content), conn);
                    if (comm.ExecuteNonQuery() > 0)
                    {
                        result.Result = "success";
                        return result;
                    }
                    else
                    {
                        result.Result = "fail";
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog("ORT::SaveInterceptORT => " + ex.Message);
                    result.Result = "fail";
                    return result;
                }
            }
        }
        public static ResultORT SaveResultInterceptORT(string content)
        {
            ResultORT result = new ResultORT();
            ResultInterceptORT entityResultInterceptORT = new ResultInterceptORT();
            try
            {
                entityResultInterceptORT = JsonConvert.DeserializeObject<ResultInterceptORT>(content);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("ORT::SaveInterceptORT => " + ex.Message);
                result.Result = "fail";
                return result;
            }
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::SaveResultInterceptORT => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("insert into intercept_mq_recieve_log (req_id,intercept_task,result,message,info) values ('{0}','{1}','{2}','{3}','{4}');", entityResultInterceptORT.REQ_ID, entityResultInterceptORT.INTERCEPT_TASK, entityResultInterceptORT.RESULT, entityResultInterceptORT.MESSAGE, content), conn);
                    if (comm.ExecuteNonQuery() > 0)
                    {
                        result.Result = "success";
                        return result;
                    }
                    else
                    {
                        result.Result = "fail";
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog("ORT::SaveResultInterceptORT => " + ex.Message);
                    result.Result = "fail";
                    return result;
                }
            }

        }
        public static string PostReleaseQueues(string content, string queue)
        {
            // 返回结果
            ResultReleaseORT result = new ResultReleaseORT();
            if (SaveReleaseORT(content).Result == "fail")
            {
                result.RESULT = "2";
                result.MESSAGE = "ORT::PostQueues => ORT 系统记录放行请求失败，未向 SAP ME 发送放行请求.";
                //return result;
                return null;
            }
            try
            {
                Uri connectUri = new Uri(ConfigurationManager.AppSettings["uri"]);
                IConnectionFactory factory = new NMSConnectionFactory(connectUri);
                using (IConnection connection = factory.CreateConnection())
                {
                    connection.Start();
                    using (ISession session = connection.CreateSession())
                    {
                        // 目标 queue
                        IDestination destination = session.GetQueue(ConfigurationManager.AppSettings[queue]);
                        // 临时 queue，向目标 queue 发送消息后，不能直接 consumer 目标 queue
                        // 直接 consumer 会自己消费自己，而且得不到结果
                        // 通过建立临时 queue，接收返回值
                        IDestination TempQueue = session.CreateTemporaryQueue();
                        // 生产者
                        IMessageProducer producer = session.CreateProducer(destination);
                        // 消费者（使用临时 queue）
                        IMessageConsumer consumer = session.CreateConsumer(TempQueue);
                        // 生产者所要发送的消息
                        ITextMessage tmsg = producer.CreateTextMessage();
                        tmsg.Text = content;
                        // 将临时 queue 作为返回目标赋予消息
                        tmsg.NMSReplyTo = TempQueue;
                        // 消息不持久化（Active MQ 重启后消息消失）
                        producer.DeliveryMode = MsgDeliveryMode.Persistent;
                        // 超时时间为 1s
                        TimeSpan receiveTimeout = new TimeSpan(0, 0, 1);
                        producer.RequestTimeout = receiveTimeout;
                        // 发送消息
                        producer.Send(tmsg);
                        // 延时 0.1s
                        System.Threading.Thread.Sleep(100);
                        ITextMessage message;
                        string msg = null;
                        try
                        {
                            while (true)
                            {
                                message = consumer.Receive(receiveTimeout) as ITextMessage;
                                if (message == null) { throw new Exception("向 SAP ME 发送放行，未能收到其反馈，请排查 ORT 与 SAP ME 接口状态."); }
                                if (SaveResultInterceptORT(content).Result == "fail")
                                {
                                    throw new Exception("ORT::PostQueues => ORT 系统记录放行回复失败.");
                                }
                                result = JsonConvert.DeserializeObject<ResultReleaseORT>(message.Text);
                                msg = message.Text;
                            }
                        }
                        catch (Exception ex)
                        {
                            result.RESULT = "2";
                            result.MESSAGE = ex.Message;
                            SysLog log = new SysLog(ex.Message);
                        }
                        producer.Close();
                        consumer.Close();
                        connection.Close();
                        //return result;
                        return msg;
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                result.RESULT = "2";
                result.MESSAGE = "连接 SAP ME 失败.";
                //return result;
                return null;
            }
        }
        public static ResultORT SaveReleaseORT(string content)
        {
            ReleaseORT entityRelease = new ReleaseORT();
            ResultORT result = new ResultORT();
            try
            {
                entityRelease = JsonConvert.DeserializeObject<ReleaseORT>(content);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("ORT::SaveReleaseORT => " + ex.Message);
                result.Result = "fail";
                return result;
            }
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::SaveReleaseORT => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("insert into release_mq_send_log (handle,message_id,req_id,send_date_time,site,intercept_task,remarks,contents) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", UuidHelper.NewUuidString(), entityRelease.MESSAGE_ID, entityRelease.REQ_ID, entityRelease.SEND_DATE_TIME, entityRelease.SITE, entityRelease.INTERCEPT_TASK, entityRelease.REMARKS, content), conn);
                    if (comm.ExecuteNonQuery() > 0)
                    {
                        result.Result = "success";
                        return result;
                    }
                    else
                    {
                        result.Result = "fail";
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog("ORT::SaveInterceptORT => " + ex.Message);
                    result.Result = "fail";
                    return result;
                }
            }
        }
        public static List<TemplateInfo> TemplateInfo(List<TemplateSearch> templateList)
        {
            StringBuilder sql = new StringBuilder();
            foreach (TemplateSearch template in templateList)
            {
                if (string.IsNullOrEmpty(template.NAME) && !string.IsNullOrEmpty(template.HANDLE))
                {
                    sql.Append(string.Format("select * from test_template where handle = '{0}' and is_current = 'Y';", template.HANDLE));
                }
                else if (string.IsNullOrEmpty(template.HANDLE) && !string.IsNullOrEmpty(template.NAME))
                {
                    sql.Append(string.Format("select * from test_template where template_name = '{0}' and is_current = 'Y';", template.NAME));
                }
                else
                {
                    sql.Append("select * from test_template where is_current = 'Y';");
                }
            }
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::TemplateInfo => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(sql.ToString(), conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    List<TemplateInfo> resultList = new List<TemplateInfo>();
                    while (reader.Read())
                    {
                        TemplateInfo result = new TemplateInfo();
                        result.HANDLE = reader["handle"].ToString();
                        result.TEMPLATE_NAME = reader["template_name"].ToString();
                        result.VERSION = reader["z_version"].ToString();
                        result.CREATOR = reader["creator"].ToString();
                        result.IS_CURRENT = reader["is_current"].ToString();
                        result.CREATED_DATE_TIME = reader["created_date_time"].ToString();
                        resultList.Add(result);
                    }
                    if (reader.HasRows)
                    {
                        reader.Close();
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 创建模板（数据文件列与存储列对应关系）
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static ResultORT TemplateSubmit(Templates template)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::TemplateSubmit => Unable to connect to ort platform. ");
                    }
                    // 新建 test_template 记录
                    string sql = string.Format("update test_template set is_current = 'N' where template_name = '{0}'; insert into test_template (template_name,z_version,is_current,creator) values ('{0}','{1}','{2}','{3}') returning handle;", template.TEMPLATE_INFO.TEMPLATE_NAME, template.TEMPLATE_INFO.VERSION, template.TEMPLATE_INFO.IS_CURRENT, template.TEMPLATE_INFO.CREATOR);
                    NpgsqlTransaction tran = conn.BeginTransaction();
                    NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                    comm.Transaction = tran;
                    string handle_template = comm.ExecuteScalar().ToString();
                    // 判断 test_template 如果创建成功，开始创建 template_parameter
                    if (string.IsNullOrEmpty(handle_template))
                    {
                        throw new Exception("ORT::TemplateSubmit => fail to create new data of test_template.");
                    }
                    int position = 0;
                    List<string> handle_details = new List<string>();
                    foreach (TemplateDetails entity in template.TEMPLATE_DETAILS)
                    {
                        sql = string.Format("insert into template_parameter (column_calculated,column_storage,types,description,creator) values ('{0}','{1}','{2}','{3}','{4}') returning handle;", entity.COLUMN, "A" + position.ToString(), entity.TYPE, entity.DESCRIPTION, template.TEMPLATE_INFO.CREATOR);
                        comm.CommandText = sql;
                        handle_details.Add(comm.ExecuteScalar().ToString());
                        ++position;
                    }
                    // 判断 template_parameter 如果创建成功，开始创建 template_group
                    if (handle_details.Count == 0)
                    {
                        throw new Exception("ORT::TemplateSubmit => fail to create new data of template_parameter.");
                    }
                    foreach (string handle in handle_details)
                    {
                        sql = string.Format("insert into template_group (handle_template,handle_parameter,creator) values ('{0}','{1}','{2}');", handle_template, handle, template.TEMPLATE_INFO.CREATOR);
                        comm.CommandText = sql;
                        int rowAffected = comm.ExecuteNonQuery();
                        if (rowAffected == 0)
                        {
                            throw new Exception("ORT::TemplateSubmit => fail to create new data of template_group.");
                        }
                    }
                    tran.Commit();
                    ResultORT result = new ResultORT();
                    result.Result = "success";
                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    ResultORT result = new ResultORT();
                    result.Result = "fail";
                    return result;
                }
            }
        }
        public static List<TemplateDetails> GetTemplateDetails(TemplateSearch template)
        {
            List<TemplateDetails> TemplateDetailsList = new List<TemplateDetails>();
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::GetTemplateDetails => Unable to connect to ort platform. ");
                    }
                    string sql = string.Format("select tt.handle,tt.template_name,tt.z_version,tt.creator,tt.created_date_time,tp.column_calculated,tp.column_storage,tp.types,tp.description from test_template tt inner join template_group tg on tt.handle = tg.handle_template inner join template_parameter tp on tg.handle_parameter = tp.handle where tt.handle = '{0}' and tt.is_current = 'Y'; ", template.HANDLE);
                    NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        TemplateDetails result = new TemplateDetails();
                        result.COLUMN = reader["column_calculated"].ToString();
                        result.DESCRIPTION = reader["description"].ToString();
                        result.TYPE = reader["types"].ToString();
                        TemplateDetailsList.Add(result);
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
            return TemplateDetailsList;
        }
        public static ResultORT AddCycleData(DataTable mDt, string barcode, string creator)
        {
            ResultORT result = new ResultORT();
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::AddCycleData => Unable to connect to ort platform. ");
                    }
                    if (mDt.Rows.Count == 0)
                    {
                        throw new Exception("ORT::AddCycleData => DataTable be empty. ");
                    }
                    if (string.IsNullOrEmpty(barcode))
                    {
                        throw new Exception("ORT::AddCycleData => Barcode be empty. ");
                    }
                    string handle_task_details = GetHandleTaskDetails(barcode);
                    if (string.IsNullOrEmpty(handle_task_details))
                    {
                        throw new Exception("ORT::AddCycleData => handle_task_details be empty. ");
                    }
                    string handle_template = GetHandleTemplate(barcode);
                    Hashtable hashCalculated = new Hashtable();
                    hashCalculated = GetColumnCalculated(handle_template);
                    if (hashCalculated == null)
                    {
                        throw new Exception("ORT::AddCycleData => ColumnCalculated be empty. ");
                    }
                    if (hashCalculated.Count == 0)
                    {
                        throw new Exception("ORT::AddCycleData => ColumnCalculated be empty. ");
                    }
                    StringBuilder sql = new StringBuilder();
                    StringBuilder items = new StringBuilder();
                    StringBuilder values = new StringBuilder();
                    sql.Append(string.Format("update cycle_data set state = 'N' where barcode = '{0}';", barcode));
                    foreach (DataRow row in mDt.Rows)
                    {
                        sql.Append("insert into cycle_data (handle_task_details,handle_template,barcode,creator,state,");
                        foreach (DictionaryEntry entry in hashCalculated)
                        {
                            try
                            {
                                items.Append(entry.Value.ToString());
                                items.Append(",");
                                values.Append("'");
                                values.Append(row[entry.Key.ToString()].ToString());
                                values.Append("',");
                            }
                            catch (Exception ex)
                            {
                                SysLog log = new SysLog(ex.Message);
                                result.Result = "fail";
                            }
                        }
                        items = items.Replace(",", ")", items.ToString().LastIndexOf(","), 1);
                        values = values.Replace("',", "');", values.ToString().LastIndexOf("',"), 2);
                        sql.Append(items);
                        sql.Append(" values (");
                        sql.Append("'");
                        sql.Append(handle_task_details);
                        sql.Append("',");
                        sql.Append("'");
                        sql.Append(handle_template);
                        sql.Append("',");
                        sql.Append("'");
                        sql.Append(barcode);
                        sql.Append("',");
                        sql.Append("'");
                        sql.Append(creator);
                        sql.Append("','Y',");
                        sql.Append(values);
                        items.Clear();
                        values.Clear();
                    }
                    NpgsqlTransaction tran = conn.BeginTransaction();
                    NpgsqlCommand comm = new NpgsqlCommand(sql.ToString(), conn);
                    if (comm.ExecuteNonQuery() > 0)
                    {
                        result.Result = "success";
                    }
                    else
                    {
                        result.Result = "fail";
                    }
                    tran.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.Result = "fail";
                    if (ex.Message == "ORT::AddCycleData => handle_task_details be empty. ")
                    {
                        result.Informations = "该任务无此样本.";
                    }
                    return result;
                }
            }
        }
        public static string GetHandleTaskDetails(string sfc)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::GetHandleTaskDetails => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("select handle_task_details from task_sample where sfc = '{0}';", sfc), conn);
                    return comm.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static string GetHandleTemplate(string sfc)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::GetHandleTemplate => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("select distinct tt.handle from task_sample ts inner join task_details td on ts.handle_task_details = td.handle inner join inspection_item ii on td.handle_inspection_item = ii.handle inner join item_template_group itg on ii.inspection_name = itg.inspection_item inner join test_template tt on itg.template_name = tt.template_name where sfc = '{0}' and itg.state = 'Y'", sfc), conn);
                    return comm.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static Hashtable GetColumnCalculated(string handle_template)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::GetColumnCalculated => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("select tp.column_storage, tp.column_calculated from test_template tt inner join template_group tg on tt.handle = tg.handle_template inner join template_parameter tp on tg.handle_parameter = tp.handle where tt.is_current = 'Y' and tt.handle = '{0}';", handle_template), conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    Hashtable hashCalculated = new Hashtable();
                    while (reader.Read())
                    {
                        hashCalculated.Add(reader["column_calculated"].ToString(), reader["column_storage"].ToString());
                    }
                    reader.Close();
                    return hashCalculated;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static DataTable Bom_Template(string bomno, string item)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::GetColumnCalculated => Unable to connect to ort platform. ");
                    }
                    string sql = null;
                    if (!string.IsNullOrEmpty(bomno) && string.IsNullOrEmpty(item)) // 查询型号下所有项目的测试模板
                    {
                        sql = string.Format("select bom_no 型号,inspection_item 测试项目,template_name 模板名称,creator 创建人,created_date_time 创建时间 from item_template_group itg where state = 'Y' and bom_no = '{0}';", bomno);
                    }
                    else if (!string.IsNullOrEmpty(bomno) && !string.IsNullOrEmpty(item))   // 查询型号下指定项目的测试模板
                    {
                        sql = string.Format("select bom_no 型号,inspection_item 测试项目,template_name 模板名称,creator 创建人,created_date_time 创建时间 from item_template_group itg where state = 'Y' and bom_no = '{0}' and inspection_item = '{1}';", bomno, item);
                    }
                    else  // 不查询
                    {

                    }
                    NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
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
        public static ResultORT BomTemplateSubmit(List<BomTemplate> templates)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                ResultORT result = new ResultORT();
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::GetColumnCalculated => Unable to connect to ort platform. ");
                    }
                    StringBuilder sql = new StringBuilder();
                    sql.Append(string.Format("update item_template_group set state = 'N' where state = 'Y' and bom_no = '{0}';", templates[0].BOMNO));
                    foreach (BomTemplate template in templates)
                    {
                        sql.Append(string.Format("insert into item_template_group (bom_no,inspection_item,template_name,creator,state) values ('{0}','{1}','{2}','{3}','Y');", template.BOMNO, template.ITEM, template.TEMPLATE, template.CREATOR));
                    }
                    NpgsqlTransaction tran = conn.BeginTransaction();
                    NpgsqlCommand comm = new NpgsqlCommand(sql.ToString(), conn);
                    comm.ExecuteNonQuery();
                    tran.Commit();
                    result.Result = "success";
                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.Result = "fail";
                    return result;
                }
            }
        }
        public static ParameterList GetColumnCalculated(BomTemplate template)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::GetColumnCalculated => Unable to connect to ort platform. ");
                    }
                    ParameterList paraList = new ParameterList();
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("select tp.handle,tp.column_calculated,tp.column_storage,tp.types,tp.description,tps.operator1,tps.standard1,tps.operator2,tps.standard2,tps.operator3,tps.standard3 from item_template_group itg inner join test_template tt on itg.template_name = tt.template_name and itg.state = 'Y' and tt.is_current = 'Y' inner join template_group tg on tt.handle = tg.handle_template inner join template_parameter tp on tg.handle_parameter = tp.handle left join template_parameter_standard tps on tp.handle = tps.handle_parameter where (tps.state = 'Y' or tps.state is null) and itg.bom_no = '{0}' and itg.inspection_item = '{1}' and itg.template_name = '{2}';", template.BOMNO, template.ITEM, template.TEMPLATE), conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    paraList.bomno = template.BOMNO;
                    paraList.item = template.ITEM;
                    while (reader.Read())
                    {
                        if (reader["types"].ToString() == "数值")
                        {
                            NumericParameter entity = new NumericParameter();
                            entity.type = reader["types"].ToString();
                            switch (reader["operator1"].ToString())
                            {
                                case "ge":
                                    entity.ge = reader["standard1"].ToString();
                                    break;
                                case "gt":
                                    entity.gt = reader["standard1"].ToString();
                                    break;
                            }
                            switch (reader["operator2"].ToString())
                            {
                                case "le":
                                    entity.le = reader["standard2"].ToString();
                                    break;
                                case "lt":
                                    entity.lt = reader["standard2"].ToString();
                                    break;
                            }
                            entity.item = reader["column_calculated"].ToString();
                            entity.handle = reader["handle"].ToString();
                            paraList.numeric_para.Add(entity);
                        }
                        else if (reader["types"].ToString() == "字符")
                        {
                            StringParameter entity = new StringParameter();
                            entity.item = reader["column_calculated"].ToString();
                            entity.value = reader["standard3"].ToString();
                            entity.handle = reader["handle"].ToString();
                            paraList.string_para.Add(entity);
                        }
                    }
                    return paraList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static ResultORT CalculatedSubmit(ParameterList entityList)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                NpgsqlTransaction tran;
                ResultORT result = new ResultORT();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::CalculatedSubmit => Unable to connect to ort platform. ");
                    }
                    tran = conn.BeginTransaction();
                    StringBuilder sql = new StringBuilder();
                    if (entityList.string_para.Count > 0)
                    {
                        foreach (StringParameter entity in entityList.string_para)
                        {
                            sql.Append(string.Format("update template_parameter_standard set state = 'N' where handle_parameter = '{0}';insert into template_parameter_standard (handle_parameter,standard3,creator,state) values ('{0}',{1},'{2}','Y');", entity.handle, string.IsNullOrEmpty(entity.value) ? "null" : "'" + entity.value + "'", entityList.creator));
                        }
                    }
                    if (entityList.numeric_para.Count > 0)
                    {
                        foreach (NumericParameter entity in entityList.numeric_para)
                        {
                            string _value = null, _operator = null;
                            if (!string.IsNullOrEmpty(entity.gt))
                            {
                                _operator = "gt";
                                _value = entity.gt;
                            }
                            else if (!string.IsNullOrEmpty(entity.ge))
                            {
                                _operator = "ge";
                                _value = entity.ge;
                            }
                            string _value2 = null, _operator2 = null;
                            if (!string.IsNullOrEmpty(entity.lt))
                            {
                                _operator2 = "lt";
                                _value2 = entity.lt;
                            }
                            else if (!string.IsNullOrEmpty(entity.le))
                            {
                                _operator2 = "le";
                                _value2 = entity.le;
                            }
                            sql.Append(string.Format("update template_parameter_standard set state = 'N' where handle_parameter = '{0}';insert into template_parameter_standard (handle_parameter,operator1,standard1,operator2,standard2,creator,state) values ('{0}',{1},{2},{3},{4},'{5}','Y');", entity.handle, string.IsNullOrEmpty(_operator) ? "null" : "'" + _operator + "'", string.IsNullOrEmpty(_value) ? "null" : "'" + _value + "'", string.IsNullOrEmpty(_operator2) ? "null" : "'" + _operator2 + "'", string.IsNullOrEmpty(_value2) ? "null" : "'" + _value2 + "'", entityList.creator));
                        }
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(sql.ToString(), conn);
                    if (comm.ExecuteNonQuery() > 0)
                    {
                        result.Result = "success";
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.Result = "fail";
                }
                return result;
            }
        }
        public static DataTable ReportTask(ReportTask task)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                ResultORT result = new ResultORT();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::ReportTask => Unable to connect to ort platform. ");
                    }
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select distinct t.handle 任务编号 ,wa.area 区域 ,tt.types_task 任务类型 ,ii.inspection_name 项目 ,o.operation_name 工序 ,b.bom_no 型号,b.item_no 料号,ts.product_batch 批次,ts.sfc 样本码号,tss.description 状态,t.created_date_time 启动时间 from task_details td inner join work_area wa on td.handle_work_area = wa.handle inner join task_types tt on td.handle_types_task = tt.handle inner join operations o on td.handle_operation = o.handle inner join bom b on td.handle_bom = b.handle inner join inspection_item ii on td.handle_inspection_item = ii.handle inner join task_sample ts on td.handle = ts.handle_task_details inner join task t on td.handle_task = t.handle inner join task_state tss on t.state = tss.state where 1 = 1");
                    if (!string.IsNullOrEmpty(task.AREA))
                    {
                        sql.Append(string.Format(" and wa.area = '{0}'", task.AREA));
                    }
                    //if (!string.IsNullOrEmpty(task.BOMNO))
                    //{
                    //    sql.Append(string.Format(" and b.bom_no = '{0}'", task.BOMNO));
                    //}
                    if (!string.IsNullOrEmpty(task.ITEMNO))
                    {
                        sql.Append(string.Format(" and b.item_no = '{0}'", task.ITEMNO));
                    }
                    if (!string.IsNullOrEmpty(task.OPERATION))
                    {
                        sql.Append(string.Format(" and o.operation_name = '{0}'", task.OPERATION));
                    }
                    if (!string.IsNullOrEmpty(task.TYPE))
                    {
                        sql.Append(string.Format(" and tt.types_task = '{0}'", task.TYPE));
                    }
                    if (!string.IsNullOrEmpty(task.BATCH))
                    {
                        sql.Append(string.Format(" and ts.product_batch = '{0}'", task.BATCH));
                    }
                    if (!string.IsNullOrEmpty(task.STATE))
                    {
                        sql.Append(string.Format(" and tss.state = '{0}'", task.STATE));
                    }
                    if (!string.IsNullOrEmpty(task.BARCODE))
                    {
                        sql.Append(string.Format(" and ts.sfc in ({0})", task.BARCODE));
                    }
                    if (!string.IsNullOrEmpty(task.DATE_TIME))
                    {
                        sql.Append(string.Format(" and t.created_date_time between {0}", task.DATE_TIME));
                    }
                    sql.Append(" order by t.created_date_time desc;");
                    NpgsqlCommand comm = new NpgsqlCommand(sql.ToString(), conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
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
        public static DataTable GetSampleListByTaskID(string id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                ResultORT result = new ResultORT();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::GetSampleListByTaskID => Unable to connect to ort platform. ");
                    }
                    string sql = null;
                    // 判断是否为 uuid
                    string pattern = @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$";
                    Regex regex = new Regex(pattern);
                    if (regex.IsMatch(id))
                    {
                        sql = string.Format("select sfc,handle_task from task_sample ts where handle_task = '{0}';", id);
                    }
                    else
                    {
                        sql = string.Format("select distinct ts2.sfc,ts2.handle_task from task_sample ts inner join task_sample ts2 on ts.handle_task = ts2.handle_task where ts.sfc = '{0}';", id);
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
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
        public static DataTable GetCycleDataBySFC(string sfc, string handle_task)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                ResultORT result = new ResultORT();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::GetCycleDataBySFC => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("select distinct handle_template FROM cycle_data cd inner join TASK_SAMPLE ts on cd.barcode = ts.SFC WHERE cd.barcode = '{0}' and cd.state = 'Y' and ts.handle_task = '{1}';", sfc, handle_task), conn);
                    string handle_template = comm.ExecuteScalar().ToString();
                    comm.CommandText = string.Format("select tp.column_storage ,tp.column_calculated from template_group tg inner join test_template tt on tg.handle_template = tt.handle inner join template_parameter tp on tg.handle_parameter = tp.handle where tt.handle = '{0}';", handle_template);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    DataTable mDt = new DataTable();
                    mDt.Load(reader);
                    reader.Close();
                    StringBuilder sql = new StringBuilder();
                    for (int i = mDt.Rows.Count - 1; i >= 0; --i)
                    {
                        sql.Append(mDt.Rows[i]["column_storage"].ToString().ToLower());
                        sql.Append(" \"");
                        sql.Append(mDt.Rows[i]["column_calculated"]);
                        sql.Append("\",");
                    }
                    comm.CommandText = string.Format("select {0} from cycle_data cd inner join task_sample ts on cd.barcode = ts.sfc where cd.barcode = '{1}' and ts.handle_task = '{2}' and cd.state = 'Y';", sql.ToString().Substring(0, sql.Length - 1), sfc, handle_task);
                    reader = comm.ExecuteReader();
                    mDt.Reset();
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
        public static TemplateStandardSFC GetTemplateStandardBySFC(string sfc)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                ResultORT result = new ResultORT();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::GetTemplateStandardBySFC => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("select itg.bom_no ,itg.inspection_item ,itg.template_name,ts.handle handle_task,ts.sfc from task_sample ts inner join task_details td on ts.handle_task_details = td.handle inner join inspection_item ii on td.handle_inspection_item = ii.handle inner join item_template_group itg on ii.inspection_name = itg.inspection_item where sfc = '{0}' and itg.state = 'Y';", sfc), conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    TemplateStandardSFC entity = new TemplateStandardSFC();
                    reader.Read();
                    entity.TASK_ID = reader["handle_task"].ToString();
                    entity.ITEM_NO = reader["bom_no"].ToString();
                    entity.TEMPLATE_NAME = reader["template_name"].ToString();
                    entity.INSPECTION_ITEM = reader["inspection_item"].ToString();
                    entity.SFC = reader["sfc"].ToString();
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
        public static DataTable GetTemplateStandard(TemplateStandardSFC entity)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                ResultORT result = new ResultORT();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::GetTemplateStandard => Unable to connect to ort platform. ");
                    }
                    NpgsqlCommand comm = new NpgsqlCommand(string.Format("SELECT tp.handle,tp.column_calculated,tp.column_storage,tp.types,tp.description,tps.operator1,tps.standard1,tps.operator2,tps.standard2,tps.operator3,tps.standard3 FROM item_template_group itg INNER JOIN test_template tt ON itg.template_name = tt.template_name AND itg.STATE = 'Y' AND tt.is_current = 'Y' INNER JOIN template_group tg ON tt.handle = tg.handle_template INNER JOIN template_parameter tp ON tg.handle_parameter = tp.handle LEFT JOIN template_parameter_standard tps ON tp.handle = tps.handle_parameter WHERE (tps.STATE = 'Y' OR tps.STATE IS NULL) AND itg.bom_no = '{0}' AND itg.inspection_item = '{1}' AND itg.template_name = '{2}' and ((tp.types = '数值' and (tps.operator1 is not null and tps.standard1 is not null) or (operator2 is not null and tps.standard2 is not null)) or (tp.types = '字符' and tps.standard3 is not null));", entity.ITEM_NO, entity.INSPECTION_ITEM, entity.TEMPLATE_NAME), conn);
                    NpgsqlDataReader reader = comm.ExecuteReader();
                    DataTable mDt = new DataTable();
                    mDt.Load(reader);
                    reader.Close();
                    return mDt;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public static bool CycleDataJudge(string sfc, DataTable dtStandard)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ORT"].ConnectionString))
            {
                conn.Open();
                ResultORT result = new ResultORT();
                bool state = true;
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::CycleDataJudge => Unable to connect to ort platform. ");
                    }
                    StringBuilder sql = new StringBuilder();
                    for (int i = 0; i < dtStandard.Rows.Count; i++)
                    {
                        sql.Clear();
                        switch (dtStandard.Rows[i]["types"].ToString())
                        {
                            case "数值":
                                string column = dtStandard.Rows[i]["column_storage"].ToString();
                                string operators = dtStandard.Rows[i]["operator1"].ToString() == "ge" ? ">=" : dtStandard.Rows[i]["operator1"].ToString() == "gt" ? ">" : null;
                                string values = !string.IsNullOrEmpty(dtStandard.Rows[i]["standard1"].ToString()) ? dtStandard.Rows[i]["standard1"].ToString() : null;
                                string operators2 = dtStandard.Rows[i]["operator2"].ToString() == "le" ? "<=" : dtStandard.Rows[i]["operator2"].ToString() == "lt" ? "<" : null;
                                string values2 = !string.IsNullOrEmpty(dtStandard.Rows[i]["standard2"].ToString()) ? dtStandard.Rows[i]["standard2"].ToString() : null;
                                sql.Append(string.Format("select handle from cycle_data where barcode = '{0}' and state = 'Y' and handle not in (select handle from cycle_data cd where barcode = '{0}' and state = 'Y' ", sfc));
                                if (!string.IsNullOrEmpty(operators) && !string.IsNullOrEmpty(values))
                                {
                                    sql.Append(string.Format(" and cast({0} as numeric) {1} {2}", column, operators, values));
                                    state = true;
                                }
                                else
                                {
                                    state = false;
                                }
                                if (!string.IsNullOrEmpty(operators2) && !string.IsNullOrEmpty(values2))
                                {
                                    sql.Append(string.Format(" and cast({0} as numeric) {1} {2}", column, operators2, values2));
                                    state = true;
                                }
                                else
                                {
                                    state = false;
                                }
                                sql.Append(")");
                                break;
                            case "字符":
                                string column2 = dtStandard.Rows[i]["column_storage"].ToString();
                                string values3 = !string.IsNullOrEmpty(dtStandard.Rows[i]["standard3"].ToString()) ? dtStandard.Rows[i]["standard3"].ToString() : null;
                                sql.Append(string.Format("select handle from cycle_data where barcode = '{0}' and state = 'Y' and handle not in (select handle from cycle_data cd where barcode = '{0}' and state = 'Y' ", sfc));
                                if (!string.IsNullOrEmpty(values3))
                                {
                                    sql.Append(string.Format(" and {0} = {1})", column2, values3));
                                    state = true;
                                }
                                else
                                {
                                    state = false;
                                }
                                break;
                        }
                        if (state)
                        {
                            NpgsqlCommand comm = new NpgsqlCommand(sql.ToString(), conn);
                            if (comm.ExecuteNonQuery() > 0)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return false;
                }
            }
        }
        public static SampleState GetSampleState(string sfc)
        {
            using (OdbcConnection conn = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SAP_PACK"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("ORT::SampleState => Unable to connect to ort platform. ");
                    }
                    SampleState sample = new SampleState();

                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT ZOS.SN,ZSNDD.QTY,ZOS.SAMPLING_TYPE,ZOS.ITEM_NO FROM Z_SFC_NC_DATA_DETAIL ZSNDD RIGHT JOIN Z_ORT_SAMPLING ZOS ON ZOS.SN = ZSNDD.CELL_SN WHERE ZOS.SN = '{0}';", sfc.Trim()), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("ORT::SampleState => No sample.");
                    sample.BARCODE = sfc;
                    sample.STATE = "合格品";
                    while (reader.Read())
                    {
                        sample.TYPE = reader["SAMPLING_TYPE"].ToString();
                        sample.ITEM_NO = reader["ITEM_NO"].ToString();
                        if (!string.IsNullOrEmpty(reader["QTY"].ToString()))
                        {
                            sample.STATE = "不合格品";
                        }
                    }
                    return sample;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    SampleState sample = new SampleState();
                    sample.BARCODE = sfc;
                    if (ex.Message.Contains("No sample"))
                    {
                        sample.STATE = "非抽样样本";
                    }
                    else
                    {
                        sample.STATE = "不合格品";
                    }
                    return sample;
                }
            }
        }
    }
}