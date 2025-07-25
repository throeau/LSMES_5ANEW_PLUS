using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Data.Odbc;
using LSMES_5ANEW_PLUS.App_Base;
using Microsoft.VisualBasic;
using System.IO;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;


namespace LSMES_5ANEW_PLUS.Business
{
    abstract public class SyncService
    {
        /// <summary>
        /// 将数据写入数据库
        /// </summary>
        /// <param name="batteryList"></param>
        /// <returns></returns>
        abstract public int CheckIn(string batteryInfo, string taskID);
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <returns></returns>
        abstract public SyncTask CreateTask();
        /// <summary>
        /// 将任务添加到任务列表中
        /// </summary>
        /// <param name="task">CreateTask创建出的任务对象</param>
        /// <returns></returns>
        abstract public int AddTask(SyncTask task);
        /// <summary>
        /// 启动数据发送任务
        /// </summary>
        /// <returns></returns>
        abstract public int SyncData();
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <returns></returns>
        abstract public int SendData(ref SyncTask task);
        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="task"></param>
        abstract public void UpdateTask(ref SyncTask task);
        /// <summary>
        /// 判断当前任务是否为有效任务，即是否存在并且状态为“未开始”
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        abstract public bool IsValid(string taskID);
        /// <summary>
        /// 任务准备就绪
        /// </summary>
        /// <param name="taskID">任务编号</param>
        /// <returns></returns>
        abstract public int Ready(string taskID);
        /// <summary>
        /// 数据回传完成将任务状态更新成“已完成”
        /// </summary>
        /// <param name="taskID">任务编号</param>
        abstract public void Complete(ref SyncTask task);
        /// <summary>
        /// 接收回复信息并记录日志中
        /// </summary>
        /// <param name="result">回复结果</param>
        /// <returns>True：成功；False：失败</returns>
        abstract public bool RecievedMsgLog(ResponseResult result);
        /// <summary>
        /// 记录发送JSON字符串
        /// </summary>
        /// <param name="jsonStr">发送字符串</param>
        abstract public void SendInfoLog(string jsonStr);
    }
    /// <summary>
    /// SMP 数据回传
    /// </summary>
    public class SyncSMP : SyncService
    {
        /// <summary>
        /// 将车间数据写入待发送数据表中
        /// </summary>
        /// <param name="batteryInfo">车间上传的发货数据</param>
        /// <param name="taskID">对应的发送任务</param>
        /// <returns>成功上传的电池数量</returns>
        override public int CheckIn(string batteryInfo, string taskID)
        {
            int affectRow = 0;
            SMP_Shipment ship = new SMP_Shipment();
            try
            {
                //整理数据
                ship = JsonConvert.DeserializeObject<SMP_Shipment>(batteryInfo);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
            //写入数据库
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                //打开连接
                conn.Open();

                //创建事务
                SqlTransaction newTrans = conn.BeginTransaction();
                //创建命令
                SqlCommand command = new SqlCommand();
                try
                {
                    command.Connection = conn;
                    command.Transaction = newTrans;
                    StringBuilder sqlStr = new StringBuilder();
                    //遍历数据
                    for (int i = 0; i < ship.data.Count; ++i)
                    {
                        sqlStr.Append("INSERT INTO SMP_SYNCBATTERY (HANDLE_TASK,CELLNAME,LOTNO,[GROUP],QALOT,CAPACITY,OCV,IMP,KVALUE,TESTTIME,BOXNUMBER,PALLETNUMBER,SUPPLIES,SMPPN,ETA,QTY,PO,CELLNAME2ND,SEGMENT1,SEGMENT2,SEGMENT3,SEGMENT4,SEGMENT5) VALUES ('");
                        sqlStr.Append(taskID);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].cellname);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].lotno);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].group);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].qalot);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].capacity);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].ocv);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].imp);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].kvalue);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].testtime);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].boxnumber);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].palletnumber);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].supplies);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].smppn);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].eta);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].qty);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].po);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].cellname2nd);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].segment1);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].segment2);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].segment3);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].segment4);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.data[i].segment5);
                        sqlStr.Append("');");
                    }
                    command.CommandText = sqlStr.ToString();
                    affectRow = command.ExecuteNonQuery();
                    newTrans.Commit();
                    return affectRow;
                }
                catch (Exception ex)
                {
                    newTrans.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 创建任务对象
        /// </summary>
        /// <returns>返回任务对象</returns>
        public SMPTask CreateSMPTask()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                //初始化Config对象
                SMPTask SmpConfig = new SMPTask();
                SqlCommand command = new SqlCommand();
                try
                {
                    //打开连接
                    conn.Open();
                    command.Connection = conn;
                    //查询当前配置
                    command.CommandText = "SELECT HANDLE,CODE,APPID,SECRET,URI,VERSION FROM SMP_CONFIG WHERE IS_CURRENT = 1;";
                    SqlDataReader SDR_Config = command.ExecuteReader();
                    while (SDR_Config.Read())
                    {
                        SmpConfig.Handle = SDR_Config[0].ToString();
                        SmpConfig.CODE = SDR_Config[1].ToString();
                        SmpConfig.APPID = SDR_Config[2].ToString();
                        SmpConfig.SECRET = SDR_Config[3].ToString();
                        SmpConfig.URI = SDR_Config[4].ToString();
                        SmpConfig.VERSION = SDR_Config[5].ToString();
                    }
                    SDR_Config.Close();
                    return SmpConfig;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 将任务添加至任务列表中
        /// </summary>
        /// <param name="task">CreateSMPTask创建的任务对象</param>
        /// <returns>成功添加的任务数量</returns>
        public int AddSMPTask(SMPTask task)
        {
            try
            {
                if (task == null)
                {
                    throw new Exception("没有获得任务对象.");
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = "INSERT INTO SMP_TASK (HANDLE_CONFIG,CODE,APPID,SECRET,TIMESTAMP,SIGN,URI,STATE,CREATED_DATE_TIME) OUTPUT INSERTED.HANDLE VALUES ('";
                    command.CommandText += task.Handle;
                    command.CommandText += "','";
                    command.CommandText += task.CODE;
                    command.CommandText += "','";
                    command.CommandText += task.APPID;
                    command.CommandText += "','";
                    command.CommandText += task.SECRET;
                    command.CommandText += "','";
                    command.CommandText += task.TIMESTAMP;
                    command.CommandText += "','";
                    command.CommandText += task.SIGN;
                    command.CommandText += "','";
                    command.CommandText += task.URI;
                    command.CommandText += "','";
                    command.CommandText += "未开始";
                    command.CommandText += "','";
                    command.CommandText += task.CREATED_DATE_TIME;
                    command.CommandText += "');";
                    int rowAffected = Convert.ToInt32(command.ExecuteScalar());
                    return rowAffected;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 创建任务对象
        /// </summary>
        /// <returns>任务对象</returns>
        override public SyncTask CreateTask()
        {
            return CreateSMPTask();
        }
        /// <summary>
        /// 获取任务对象
        /// </summary>
        /// <param name="id">任务编号</param>
        /// <returns></returns>
        public SMPTask GetTask(string id)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand(string.Format("SELECT Handle,CODE,APPID,SECRET,URI,STATE FROM SMP_TASK WHERE HANDLE = '{0}';", id), conn);
                    SqlDataReader SDR_Config = comm.ExecuteReader();
                    SMPTask task = new SMPTask();
                    while (SDR_Config.Read())
                    {
                        task.Handle = SDR_Config[0].ToString();
                        task.CODE = SDR_Config[1].ToString();
                        task.APPID = SDR_Config[2].ToString();
                        task.SECRET = SDR_Config[3].ToString();
                        task.URI = SDR_Config[4].ToString();
                        task.STATE = SDR_Config[5].ToString();
                    }
                    SDR_Config.Close();
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 将任务添加至任务列表中
        /// </summary>
        /// <param name="task">CreateSMPTask创建的任务对象</param>
        /// <returns>成功添加的任务数量</returns>
        public override int AddTask(SyncTask task)
        {
            return AddSMPTask((SMPTask)task);
        }
        public override int SyncData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //发送合计
                    int mDataCount = 0;
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = "SELECT HANDLE,CODE,APPID,SECRET,TIMESTAMP,SIGN,URI FROM SMP_TASK WHERE STATE = '准备就绪';";
                    SqlDataReader SDR_Task = command.ExecuteReader();
                    //逐任务执行发送
                    while (SDR_Task.Read())
                    {
                        //创建任务
                        SyncTask task = new SMPTask();
                        task.Handle = SDR_Task[0].ToString();
                        task.CODE = SDR_Task[1].ToString();
                        task.APPID = SDR_Task[2].ToString();
                        task.SECRET = SDR_Task[3].ToString();
                        task.TIMESTAMP = SDR_Task[4].ToString();
                        task.SIGN = SDR_Task[5].ToString();
                        task.URI = SDR_Task[6].ToString();
                        //执行发送
                        mDataCount += SendData(ref task);
                    }
                    return mDataCount;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 遍历发送任务，发送数据
        /// </summary>
        /// <returns>所有任务执行完毕总共发送电池数量</returns>
        public int SyncData2(string handle, string taskno)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //发送合计
                    int mDataCount = 0;
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("SELECT HANDLE,CODE,APPID,SECRET,TIMESTAMP,SIGN,URI FROM SMP_TASK WHERE STATE = '准备就绪' AND HANDLE = '{0}';", handle);
                    SqlDataReader SDR_Task = command.ExecuteReader();
                    //逐任务执行发送
                    while (SDR_Task.Read())
                    {
                        //创建任务
                        SyncTask task = new SMPTask();
                        task.Handle = SDR_Task[0].ToString();
                        task.CODE = SDR_Task[1].ToString();
                        task.APPID = SDR_Task[2].ToString();
                        task.SECRET = SDR_Task[3].ToString();
                        task.TIMESTAMP = SDR_Task[4].ToString();
                        task.SIGN = SDR_Task[5].ToString();
                        task.URI = SDR_Task[6].ToString();
                        //执行发送
                        mDataCount += SendData(ref task);
                    }
                    return mDataCount;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 获取该任务实际回传数量
        /// </summary>
        /// <param name="handle">任务 handle</param>
        /// <returns></returns>
        public int GetQtyOfSend(string handle)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSMP::GetQtyOfSend => fail to open database.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT COUNT(SSBL.CELLNAME) QTY FROM SMP_TASK T INNER JOIN SMP_SyncBattery_Log SSBL ON T.HANDLE = SSBL.HANDLE_TASK WHERE T.HANDLE = {0};", handle), conn);
                    object result = comm.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 执行发送任务（单任务）
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <returns>单任务对象成功发送的电池数量</returns>
        public override int SendData(ref SyncTask task)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //回传记数
                    int mCount = 0;
                    StringBuilder mJsonString = new StringBuilder();
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("UPDATE SMP_TASK SET STATE = '开始发送' WHERE HANDLE = {0};", task.Handle);
                    command.ExecuteNonQuery();
                    command.CommandText = string.Format("SELECT TOP 100 CELLNAME,LOTNO,[GROUP],QALOT,CAPACITY,OCV,IMP,KVALUE,TESTTIME,BOXNUMBER,PALLETNUMBER,SUPPLIES,SMPPN,ETA,QTY,PO,CELLNAME2ND,SEGMENT1,SEGMENT2,SEGMENT3,SEGMENT4,SEGMENT5 FROM SMP_SYNCBATTERY WHERE HANDLE_TASK = {0};", task.Handle);
                    SqlDataReader SDR_data = command.ExecuteReader();
                    //定义发送数据
                    SMP_Shipment ship = new SMP_Shipment();
                    ship.appid = task.APPID;
                    ship.code = task.CODE;
                    ship.secret = task.SECRET;
                    ship.sign = task.SIGN;
                    ship.timestamp = task.TIMESTAMP;
                    //定义web service对象
                    WebSOAP soap = new WebSOAP();
                    //是否重复一次
                    bool state = true;
                    while (SDR_data.HasRows)
                    {
                        while (SDR_data.Read())
                        {
                            //定义单体电池对象
                            SMP_Battery battery = new SMP_Battery();
                            battery.cellname = SDR_data[0].ToString();
                            battery.lotno = SDR_data[1].ToString();
                            battery.group = SDR_data[2].ToString();
                            battery.qalot = SDR_data[3].ToString();
                            battery.capacity = SDR_data[4].ToString();
                            battery.ocv = SDR_data[5].ToString();
                            battery.imp = SDR_data[6].ToString();
                            battery.kvalue = SDR_data[7].ToString();
                            battery.testtime = SDR_data[8].ToString();
                            battery.boxnumber = SDR_data[9].ToString();
                            battery.palletnumber = SDR_data[10].ToString();
                            battery.supplies = SDR_data[11].ToString();
                            battery.smppn = SDR_data[12].ToString();
                            battery.eta = SDR_data[13].ToString();
                            battery.qty = SDR_data[14].ToString();
                            battery.po = SDR_data[15].ToString();
                            battery.cellname2nd = SDR_data[16].ToString();
                            battery.segment1 = SDR_data[17].ToString();
                            battery.segment2 = SDR_data[18].ToString();
                            battery.segment3 = SDR_data[19].ToString();
                            battery.segment4 = SDR_data[20].ToString();
                            battery.segment5 = SDR_data[21].ToString();
                            ship.data.Add(battery);
                        }
                        mJsonString.Append(JsonConvert.SerializeObject(ship));
                        ResponseResult result = soap.SMP_QuerySoapWebService(task.URI, mJsonString.ToString());
                        SendInfoLog(mJsonString.ToString());
                        if (RecievedMsgLog(result))
                        {
                            UpdateSendLog(ship, task);
                            mCount += ship.data.Count;
                            state = true;
                        }
                        else if (!state)
                        {
                            task.STATE = "发生异常";
                            break;
                        }
                        else
                        {
                            state = false;
                            Thread.Sleep(60000);
                        }
                        mJsonString.Clear();
                        ship.data.Clear();
                        SDR_data.Close();
                        command.CommandText = string.Format("SELECT TOP 100 CELLNAME,LOTNO,[GROUP],QALOT,CAPACITY,OCV,IMP,KVALUE,TESTTIME,BOXNUMBER,PALLETNUMBER,SUPPLIES,SMPPN,ETA,QTY,PO,CELLNAME2ND,SEGMENT1,SEGMENT2,SEGMENT3,SEGMENT4,SEGMENT5 FROM SMP_SYNCBATTERY WHERE HANDLE_TASK = {0};", task.Handle);
                        SDR_data = command.ExecuteReader();
                    }
                    task.COUNT = mCount;
                    Complete(ref task);
                    return task.COUNT;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 更新单任务状态
        /// </summary>
        /// <param name="task">单任务对象</param>
        public override void UpdateTask(ref SyncTask task)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("UPDATE SMP_TASK SET STATE = '完成' WHERE HANDLE = '{0}';", task.Handle);
                    if (command.ExecuteNonQuery() == 0)
                    {
                        throw new Exception("更新 SMP 数据发送任务状态失败.");
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        /// <summary>
        /// 判断当前任务是否为有效任务，即是否存在并且状态为“未开始”
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public override bool IsValid(string taskID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("SELECT COUNT(1) FROM SMP_TASK WHERE STATE = '未开始' AND HANDLE = '{0}';", taskID);
                    int result = Convert.ToInt32(command.ExecuteScalar());
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 完成上传任务时将该任务设置为“准备就绪”
        /// </summary>
        /// <param name="taskID">任务编号</param>
        /// <returns></returns>
        public override int Ready(string taskID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("UPDATE SMP_TASK SET STATE = '准备就绪' WHERE HANDLE = '{0}';", taskID);
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 记录数据回传日志，将回传成功的电池从待回传表（SMP_SYNCBATTERY）中删除，并写入回传完毕的日志表（SMP_SYNCBATTERY_LOG）中
        /// </summary>
        /// <param name="battery">完成回传的电池</param>
        /// <param name="task">当前回传任务</param>
        /// <returns></returns>
        public int UpdateSendLog(SMP_Shipment ship, SyncTask task)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                SqlCommand command = new SqlCommand();
                SqlTransaction tran;
                StringBuilder sqlStr = new StringBuilder();
                //打开连接
                conn.Open();
                command.Connection = conn;
                tran = conn.BeginTransaction();
                command.Transaction = tran;
                try
                {
                    for (int i = 0; i < ship.data.Count; ++i)
                    {
                        sqlStr.Append(string.Format("INSERT INTO SMP_SYNCBATTERY_LOG SELECT HANDLE_TASK,CELLNAME,LOTNO,[GROUP],QALOT,CAPACITY,OCV,IMP,KVALUE,TESTTIME,BOXNUMBER,PALLETNUMBER,SUPPLIES,SMPPN,ETA,QTY,PO,CELLNAME2ND,SEGMENT1,SEGMENT2,SEGMENT3,SEGMENT4,SEGMENT5,CREATED_DATE_TIME,GETDATE() FROM SMP_SYNCBATTERY WHERE HANDLE_TASK = {0} AND CELLNAME = '{1}';DELETE FROM SMP_SYNCBATTERY WHERE HANDLE_TASK = {0} AND CELLNAME = '{1}';", task.Handle, ship.data[i].cellname));
                    }
                    command.CommandText = sqlStr.ToString();
                    int AffactedRow = command.ExecuteNonQuery();
                    tran.Commit();
                    return AffactedRow;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }

            }
        }
        /// <summary>
        /// 数据回传完成将任务状态更新成“已完成”
        /// </summary>
        /// <param name="taskID">任务编号</param>
        public override void Complete(ref SyncTask task)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    string taskInfo;
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    //更新任务状态
                    if (string.IsNullOrEmpty(task.STATE))
                    {
                        command.CommandText = string.Format("UPDATE SMP_TASK SET STATE = '已完成' WHERE HANDLE = {0}", task.Handle);
                        taskInfo = "任务编号：" + task.Handle + " 的发送任务已成功完成，共向客户端发送 " + task.COUNT + " 条出货数据.";
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE SMP_TASK SET STATE = '{1}' WHERE HANDLE = {0}", task.Handle, task.STATE);
                        taskInfo = "任务编号：" + task.Handle + " 的发送任务" + task.STATE + "，请反馈.";
                    }
                    command.ExecuteNonQuery();
                    //生成收件人列表
                    //command.CommandText = "SELECT USERNAME,EMAIL FROM EMAIL_CONFIG;";
                    command.CommandText = "SELECT E.USERNAME,E.EMAIL FROM CUSTOMER C INNER JOIN EMAIL_GROUP G ON C.HANDLE = G.HANDLE_CUSTOMER INNER JOIN EMAIL_CONFIG E ON G.HANDLE_EMAIL = E.HANDLE WHERE CUSTOMER = 'SMP';";

                    SqlDataReader SDR_Email = command.ExecuteReader();
                    StringBuilder emailList = new StringBuilder();
                    while (SDR_Email.Read())
                    {
                        emailList.Append(SDR_Email[1].ToString());
                        emailList.Append(",");
                    }
                    SDR_Email.Close();
                    //生成邮件内容
                    //command.CommandText = "SELECT COUNT(1) NUM,HANDLE_TASK,PALLETNUMBER FROM SMP_SYNCBATTERY_LOG WHERE HANDLE_TASK = " + task.Handle + " GROUP BY HANDLE_TASK,PALLETNUMBER;";
                    command.CommandText = "WITH A AS (SELECT HANDLE_TASK,COUNT(1) NUM,PALLETNUMBER FROM SMP_SYNCBATTERY_LOG WHERE HANDLE_TASK = " + task.Handle + "	GROUP BY HANDLE_TASK,PALLETNUMBER),B AS (SELECT DISTINCT HANDLE_TASK,ISNULL(CONVERT(VARCHAR(20), PALLETNUMBER), 'TOTAL') AS 'PALLETNUMBER',SUM(NUM) AS 'QTY_TOTAL' FROM A GROUP BY HANDLE_TASK,PALLETNUMBER WITH ROLLUP) SELECT * FROM B WHERE HANDLE_TASK IS NOT NULL ORDER BY PALLETNUMBER ASC;";

                    SqlDataReader SDR_SyncLog = command.ExecuteReader();
                    TableWeb tWeb = new TableWeb();
                    tWeb.addThead("任务编号");
                    tWeb.addThead("栈板编号");
                    tWeb.addThead("成功发送数量");
                    while (SDR_SyncLog.Read())
                    {
                        tWeb.addContext(SDR_SyncLog[0].ToString());
                        tWeb.addContext(SDR_SyncLog[1].ToString());
                        tWeb.addContext(SDR_SyncLog[2].ToString());
                    }
                    SDR_SyncLog.Close();
                    //发送邮件
                    Mail.SendMail(emailList.ToString(), System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], "SMP 数据发送任务", tWeb.TableHtml());

                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        /// <summary>
        /// 接收回复信息并记录日志中
        /// </summary>
        /// <param name="result">回复结果</param>
        /// <returns>True：成功；False：失败</returns>
        public override bool RecievedMsgLog(ResponseResult result)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    StringBuilder sqlStr = new StringBuilder();
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    bool state = true;
                    //sqlStr.Append(string.Format("INSERT INTO SMP_RECIEVED_LOG (CODE,MESSAGE,BATCHID,DATA,HANDLE_SMP_RECIEVED_INFO) VALUES ('{0}','{1}','{2}','{3}','{4}');", ((SMP_ResponseResult)result).code, ((SMP_ResponseResult)result).message, ((SMP_ResponseResult)result).batchid, ((SMP_ResponseResult)result).data, ((SMP_ResponseResult)result).ret));
                    //command.CommandText = sqlStr.ToString();
                    //int rlt = command.ExecuteNonQuery();
                    //int rlt = 0;
                    SMP_ResponseResultDetails detail = (SMP_ResponseResultDetails)JsonConvert.DeserializeObject(((SMP_ResponseResult)result).data, typeof(SMP_ResponseResultDetails));
                    //sqlStr.Append(string.Format("INSERT INTO SMP_RECIEVED_RESULT (LOGID,TOTAL,SUCCESS,NOSUCCESS,HANDLE_SMP_RECIEVED_LOG) VALUES ('{0}','{1}','{2}','{3}','{4}');", detail.logid, detail.total, detail.success, detail.nosuccess, rlt, detail.data));
                    //command.CommandText = sqlStr.ToString();
                    //rlt = command.ExecuteNonQuery();
                    //List<SMP_ResponseResultData> cellList = (List<SMP_ResponseResultData>)JsonConvert.DeserializeObject(detail.data, typeof(List<SMP_ResponseResultData>));

                    foreach (SMP_ResponseResultData cell in detail.data)
                    {
                        sqlStr.Append(string.Format("INSERT INTO SMP_RECIEVED_LOG (CODE,MESSAGE,BATCHID,LOGID,TOTAL,SUCCESS,NOSUCCESS,CELLNAME,CHECKFLAG,CHECKMESSAGE,HANDLE_SMP_RECIEVED_INFO) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');", ((SMP_ResponseResult)result).code, ((SMP_ResponseResult)result).message, ((SMP_ResponseResult)result).batchid, detail.logid, detail.total, detail.success, detail.nosuccess, cell.cellname, cell.checkflag, cell.checkmessage, ((SMP_ResponseResult)result).ret));

                        //sqlStr.Append(string.Format("INSERT INTO SMP_RECIEVED_LOG (LOGID,TOTAL,SUCCESS,NOSUCCESS,HANDLE_SMP_RECIEVED_INFO) VALUES ('{0}','{1}','{2}','{3}','{4}');", ((SMP_ResponseResultDetails)detail), ((SMP_ResponseResultDetails)detail).total, ((SMP_ResponseResultDetails)detail).success, ((SMP_ResponseResultDetails)detail).nosuccess, ((SMP_ResponseResult)result).ret));
                        //if (((SMP_ResponseResult)result).code != "1")   //SMP返回“失败”
                        //{  
                        //    if (((SMP_ResponseResult)result).message.IndexOf("end-of-file on communication channel") != -1)  //SMP系统返回网络中断异常
                        //    {
                        //        state = false;
                        //    }
                        //    //else if (((SMP_ResponseResultDetails)detail).checkmessage.IndexOf("與歷史資料重複") == -1 && ((SMP_ResponseResultDetails)detail).checkflag == "0")//发现非“與歷史資料重複”的异常信息
                        //    {
                        //        state = false;
                        //    }
                        //}
                        if (((SMP_ResponseResult)result).code == "800")   //SMP返回“失败”
                        {
                            state = false;
                        }
                    }
                    command.CommandText = sqlStr.ToString();
                    command.ExecuteNonQuery();
                    return state;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 完整记录客户端返回的信息
        /// </summary>
        /// <param name="jsonStr"></param>
        public override void SendInfoLog(string jsonStr)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("INSERT INTO SMP_SEND_INFO (INFO_JSON) VALUES ('{0}');", jsonStr);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="dt">时间范围</param>
        /// <param name="task">任务号</param>
        /// <param name="barcode">码号</param>
        /// <returns></returns>
        public DataTable Search(string dt,string task,string barcode)
        {
            string sql = null;
            if (string.IsNullOrEmpty(task) && string.IsNullOrEmpty(barcode))
            {
                sql = "WITH A AS(SELECT T.HANDLE,STATE,T.CREATED_DATE_TIME,COUNT(CELLNAME) C1 FROM SMP_TASK T LEFT JOIN SMP_SYNCBATTERY S ON T.HANDLE = S.HANDLE_TASK GROUP BY T.HANDLE,T.STATE,T.CREATED_DATE_TIME),B AS(	SELECT T.HANDLE,COUNT(CELLNAME) C2 FROM SMP_TASK T LEFT JOIN SMP_SYNCBATTERY_LOG SL ON T.HANDLE = SL.HANDLE_TASK GROUP BY T.HANDLE) SELECT A.HANDLE '任务编号',A.STATE '任务状态',A.CREATED_DATE_TIME '任务创建时间',A.C1 '未发送数量',B.C2 '已发送数量' FROM A INNER JOIN B ON A.HANDLE = B.HANDLE ORDER BY A.HANDLE DESC;";
            }
            else if (!string.IsNullOrEmpty(task) && string.IsNullOrEmpty(barcode))
            {
                sql = string.Format("WITH A AS(SELECT T.HANDLE,STATE,T.CREATED_DATE_TIME,COUNT(CELLNAME) C1 FROM SMP_TASK T LEFT JOIN SMP_SYNCBATTERY S ON T.HANDLE = S.HANDLE_TASK GROUP BY T.HANDLE,T.STATE,T.CREATED_DATE_TIME),B AS(	SELECT T.HANDLE,COUNT(CELLNAME) C2 FROM SMP_TASK T LEFT JOIN SMP_SYNCBATTERY_LOG SL ON T.HANDLE = SL.HANDLE_TASK GROUP BY T.HANDLE) SELECT A.HANDLE '任务编号',A.STATE '任务状态',A.CREATED_DATE_TIME '任务创建时间',A.C1 '未发送数量',B.C2 '已发送数量' FROM A INNER JOIN B ON A.HANDLE = B.HANDLE WHERE A.HANDLE = '{0}' ORDER BY A.HANDLE DESC;", task);
            }
            else if (string.IsNullOrEmpty(task) && !string.IsNullOrEmpty(barcode))
            {
                sql = string.Format("SELECT T.HANDLE 任务编号,T.STATE 任务状态,T.CREATED_DATE_TIME 任务创建时间,L.CELLNAME,L.LOTNO,L.[GROUP],L.QALOT,L.CAPACITY,L.OCV,L.IMP,L.KVALUE,L.TESTTIME,L.BOXNUMBER,L.PALLETNUMBER,L.SUPPLIES,L.SMPPN,L.ETA,L.PO FROM SMP_SYNCBATTERY_LOG L INNER JOIN SMP_TASK T ON L.HANDLE_TASK = T.HANDLE WHERE L.CELLNAME = '{0}';", barcode);
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    DataTable mDt = new DataTable();
                    mDt.Load(reader);
                    return mDt;
                }
                catch(Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
    }
    /// <summary>
    /// Zebra 数据回传
    /// </summary>
    public class SyncZebra : SyncService
    {
        /// <summary>
        /// 将车间数据写入待发送数据表中
        /// </summary>
        /// <param name="batteryInfo">车间上传的发货数据</param>
        /// <param name="taskID">对应的发送任务</param>
        /// <returns>成功上传的电池数量</returns>
        override public int CheckIn(string batteryInfo, string taskID)
        {
            int affectRow = 0;
            Zebra_Shipment ship = new Zebra_Shipment();
            try
            {
                //整理数据
                ship = JsonConvert.DeserializeObject<Zebra_Shipment>(batteryInfo);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
            //写入数据库
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                //打开连接
                conn.Open();

                //创建事务
                SqlTransaction newTrans = conn.BeginTransaction();
                //创建命令
                SqlCommand command = new SqlCommand();
                try
                {
                    command.Connection = conn;
                    command.Transaction = newTrans;
                    StringBuilder sqlStr = new StringBuilder();
                    //遍历数据
                    for (int i = 0; i < ship.testItemList.Count; ++i)
                    {
                        sqlStr.Append("INSERT INTO ZEBRA_SYNCBATTERY (handle_task,supplierName,cellPartNumber,cellSN,cellType,cellDateCode,cellLotNumber ,cellFactoryID,cellOCVoltage,cellACImpedance,cellCapacity,cellKvalue,cellProLineNO,cellWorkVoltage) VALUES ('");
                        sqlStr.Append(taskID);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].supplierName);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellPartNumber);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellSN);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellType);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellDateCode);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellLotNumber);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellFactoryID);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellOCVoltage);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellACImpedance);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellCapacity);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellKvalue);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellProLineNO);
                        sqlStr.Append("','");
                        sqlStr.Append(ship.testItemList[i].cellWorkVoltage);
                        sqlStr.Append("');");
                    }
                    command.CommandText = sqlStr.ToString();
                    affectRow = command.ExecuteNonQuery();
                    newTrans.Commit();
                    return affectRow;
                }
                catch (Exception ex)
                {
                    newTrans.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 创建任务对象
        /// </summary>
        /// <returns>返回任务对象</returns>
        public ZebraTask CreateZebraTask()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                //初始化Config对象
                ZebraTask ZebraConfig = new ZebraTask();
                SqlCommand command = new SqlCommand();
                try
                {
                    //打开连接
                    conn.Open();
                    command.Connection = conn;
                    //查询当前配置
                    command.CommandText = "SELECT HANDLE,ID,TOKEN,URI,VERSION FROM ZEBRA_CONFIG WHERE IS_CURRENT = 1;";
                    SqlDataReader SDR_Config = command.ExecuteReader();
                    while (SDR_Config.Read())
                    {
                        ZebraConfig.Handle = SDR_Config[0].ToString();
                        ZebraConfig.ID = SDR_Config[1].ToString();
                        ZebraConfig.TOKEN = SDR_Config[2].ToString();
                        ZebraConfig.URI = SDR_Config[3].ToString();
                        ZebraConfig.VERSION = SDR_Config[4].ToString();
                    }
                    SDR_Config.Close();
                    return ZebraConfig;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 将任务添加至任务列表中
        /// </summary>
        /// <param name="task">CreateZebraTask创建的任务对象</param>
        /// <returns>成功添加的任务数量</returns>
        public int AddZebraTask(ZebraTask task)
        {
            try
            {
                if (task == null)
                {
                    throw new Exception("没有获得任务对象.");
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = "INSERT INTO ZEBRA_TASK (HANDLE_CONFIG,ID,TOKEN,URI,STATE,CREATED_DATE_TIME) OUTPUT INSERTED.HANDLE VALUES ('";
                    command.CommandText += task.Handle;
                    command.CommandText += "','";
                    command.CommandText += task.ID;
                    command.CommandText += "','";
                    command.CommandText += task.TOKEN;
                    command.CommandText += "','";
                    command.CommandText += task.URI;
                    command.CommandText += "','";
                    command.CommandText += "未开始";
                    command.CommandText += "','";
                    command.CommandText += task.CREATED_DATE_TIME;
                    command.CommandText += "');";
                    int rowAffected = Convert.ToInt32(command.ExecuteScalar());
                    return rowAffected;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 创建任务对象
        /// </summary>
        /// <returns>任务对象</returns>
        override public SyncTask CreateTask()
        {
            return CreateZebraTask();
        }
        /// <summary>
        /// 将任务添加至任务列表中
        /// </summary>
        /// <param name="task">CreateZebraTask创建的任务对象</param>
        /// <returns>成功添加的任务数量</returns>
        public override int AddTask(SyncTask task)
        {
            return AddZebraTask((ZebraTask)task);
        }
        /// <summary>
        /// 遍历发送任务，发送数据
        /// </summary>
        /// <returns>所有任务执行完毕总共发送电池数量</returns>
        public override int SyncData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //发送合计
                    int mDataCount = 0;
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = "SELECT HANDLE,ID,TOKEN,URI FROM ZEBRA_TASK WHERE STATE = '准备就绪';";
                    SqlDataReader SDR_Task = command.ExecuteReader();
                    //逐任务执行发送
                    while (SDR_Task.Read())
                    {
                        //创建任务
                        SyncTask task = new ZebraTask();
                        task.Handle = SDR_Task[0].ToString();
                        task.ID = SDR_Task[1].ToString();
                        task.TOKEN = SDR_Task[2].ToString();
                        task.URI = SDR_Task[3].ToString();
                        //执行发送
                        mDataCount += SendData(ref task);
                    }
                    return mDataCount;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 执行发送任务（单任务）
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <returns>单任务对象成功发送的电池数量</returns>
        public override int SendData(ref SyncTask task)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //回传记数
                    int mCount = 0;
                    StringBuilder mJsonString = new StringBuilder();
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("UPDATE ZEBRA_TASK SET STATE = '开始发送' WHERE HANDLE = {0};", task.Handle);
                    command.ExecuteNonQuery();
                    // 明美老版本
                    //command.CommandText = string.Format("SELECT TOP 100 CELLSN,CONVERT(NVARCHAR(50),TESTDATE,120) TESTDATE,VOLTAGEVALUE/1000 VOLTAGEVALUE,CAPACITY,CELLBATCH,IMPEDANCE FROM ZEBRA_SYNCBATTERY WHERE HANDLE_TASK = {0};", task.Handle);
                    command.CommandText = string.Format("SELECT TOP 20 supplierName,cellPartNumber,cellSN,cellType,cellDateCode,cellLotNumber ,cellFactoryID,cellOCVoltage,cellACImpedance,cellCapacity,cellKvalue,cellProLineNO,cellWorkVoltage FROM ZEBRA_SYNCBATTERY WHERE HANDLE_TASK = {0};", task.Handle);

                    SqlDataReader SDR_data = command.ExecuteReader();
                    //定义发送数据
                    //Zebra_Shipment ship = new Zebra_Shipment();
                    List<Zebra_testItems> testItems = new List<Zebra_testItems>();
                    //定义web service对象
                    WebSOAP soap = new WebSOAP();
                    //是否重复一次
                    bool state = true;
                    while (SDR_data.HasRows)
                    {
                        while (SDR_data.Read())
                        {
                            //定义单体电池对象，老版本
                            //Zebra_testItems battery = new Zebra_testItems();
                            //battery.cellSn = SDR_data[0].ToString();
                            //battery.testDate = SDR_data[1].ToString();
                            //battery.voltageValue = SDR_data[2].ToString();
                            //battery.capacity = SDR_data[3].ToString();
                            //battery.cellBatch = SDR_data[4].ToString();
                            //battery.impedance = SDR_data[5].ToString();

                            //定义单体电池对象，新版本
                            Zebra_testItems battery = new Zebra_testItems();
                            battery.cellACImpedance = SDR_data["cellACImpedance"].ToString();
                            battery.cellCapacity = SDR_data["cellCapacity"].ToString();
                            battery.cellDateCode = SDR_data["cellDateCode"].ToString();
                            battery.cellFactoryID = SDR_data["cellFactoryID"].ToString();
                            battery.cellKvalue = SDR_data["cellKvalue"].ToString();
                            battery.cellLotNumber = SDR_data["cellLotNumber"].ToString();
                            battery.cellOCVoltage = SDR_data["cellOCVoltage"].ToString();
                            battery.cellPartNumber = SDR_data["cellPartNumber"].ToString();
                            battery.cellProLineNO = SDR_data["cellProLineNO"].ToString();
                            battery.cellSN = SDR_data["cellSN"].ToString();
                            battery.cellType = SDR_data["cellType"].ToString();
                            battery.cellWorkVoltage = SDR_data["cellWorkVoltage"].ToString();
                            battery.supplierName = SDR_data["supplierName"].ToString();
                            testItems.Add(battery);
                        }
                        SysLog log = new SysLog("0.JsonConvert");
                        mJsonString.Append(JsonConvert.SerializeObject(testItems));
                        log.AddLog("1." + mJsonString);
                        Zebra_ResponseResult result = soap.Zebra_QuerySoapWebService(task.URI, mJsonString.ToString());
                        log.AddLog("3.");
                        SendInfoLog(mJsonString.ToString());
                        if (RecievedMsgLog(result))
                        {
                            UpdateSendLog(testItems, task);
                            mCount += testItems.Count;
                            state = true;
                        }
                        else if (!state)
                        {
                            task.STATE = "发生异常";
                            break;
                        }
                        else
                        {
                            state = false;
                            Thread.Sleep(60000);
                        }
                        mJsonString.Clear();
                        testItems.Clear();
                        SDR_data.Close();
                        command.CommandText = string.Format("SELECT TOP 20 supplierName,cellPartNumber,cellSN,cellType,cellDateCode,cellLotNumber ,cellFactoryID,cellOCVoltage,cellACImpedance,cellCapacity,cellKvalue,cellProLineNO,cellWorkVoltage FROM ZEBRA_SYNCBATTERY WHERE HANDLE_TASK = {0};", task.Handle);
                        SDR_data = command.ExecuteReader();
                    }
                    task.COUNT = mCount;
                    Complete(ref task);
                    return task.COUNT;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 更新单任务状态
        /// </summary>
        /// <param name="task">单任务对象</param>
        public override void UpdateTask(ref SyncTask task)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("UPDATE ZEBRA_TASK SET STATE = '完成' WHERE HANDLE = '{0}';", task.Handle);
                    if (command.ExecuteNonQuery() == 0)
                    {
                        throw new Exception("更新 Zebra 数据发送任务状态失败.");
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        /// <summary>
        /// 判断当前任务是否为有效任务，即是否存在并且状态为“未开始”
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public override bool IsValid(string taskID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("SELECT COUNT(1) FROM ZEBRA_TASK WHERE STATE = '未开始' AND HANDLE = '{0}';", taskID);
                    int result = Convert.ToInt32(command.ExecuteScalar());
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 完成上传任务时将该任务设置为“准备就绪”
        /// </summary>
        /// <param name="taskID">任务编号</param>
        /// <returns></returns>
        public override int Ready(string taskID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("UPDATE ZEBRA_TASK SET STATE = '准备就绪' WHERE HANDLE = '{0}';", taskID);
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 记录数据回传日志，将回传成功的电池从待回传表（Zebra_SYNCBATTERY）中删除，并写入回传完毕的日志表（Zebra_SYNCBATTERY_LOG）中
        /// </summary>
        /// <param name="battery">完成回传的电池</param>
        /// <param name="task">当前回传任务</param>
        /// <returns></returns>
        public int UpdateSendLog(List<Zebra_testItems> ship, SyncTask task)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                SqlCommand command = new SqlCommand();
                SqlTransaction tran;
                StringBuilder sqlStr = new StringBuilder();
                //打开连接
                conn.Open();
                command.Connection = conn;
                tran = conn.BeginTransaction();
                command.Transaction = tran;
                try
                {
                    for (int i = 0; i < ship.Count; ++i)
                    {
                        sqlStr.Append(string.Format("INSERT INTO ZEBRA_SYNCBATTERY_LOG SELECT HANDLE, HANDLE_TASK,supplierName,cellPartNumber,cellSN,cellType,cellDateCode,cellLotNumber ,cellFactoryID,cellOCVoltage,cellACImpedance,cellCapacity,cellKvalue,cellProLineNO,cellWorkVoltage,created_date_time,GETDATE() FROM ZEBRA_SYNCBATTERY WHERE HANDLE_TASK = {0} AND CELLSN = '{1}';DELETE FROM ZEBRA_SYNCBATTERY WHERE HANDLE_TASK = {0} AND CELLSN = '{1}';", task.Handle, ship[i].cellSN));
                    }
                    command.CommandText = sqlStr.ToString();
                    int AffactedRow = command.ExecuteNonQuery();
                    tran.Commit();
                    return AffactedRow;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }

            }
        }
        /// <summary>
        /// 数据回传完成将任务状态更新成“已完成”
        /// </summary>
        /// <param name="taskID">任务编号</param>
        public override void Complete(ref SyncTask task)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    string taskInfo;
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    //更新任务状态
                    if (string.IsNullOrEmpty(task.STATE))
                    {
                        command.CommandText = string.Format("UPDATE ZEBRA_TASK SET STATE = '已完成' WHERE HANDLE = {0}", task.Handle);
                        taskInfo = "任务编号：" + task.Handle + " 的发送任务已成功完成，共向客户端发送 " + task.COUNT + " 条出货数据.";
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE ZEBRA_TASK SET STATE = '{1}' WHERE HANDLE = {0}", task.Handle, task.STATE);
                        taskInfo = "任务编号：" + task.Handle + " 的发送任务" + task.STATE + "，请反馈.";
                    }
                    command.ExecuteNonQuery();
                    //生成收件人列表
                    //command.CommandText = "SELECT USERNAME,EMAIL FROM EMAIL_CONFIG;";
                    command.CommandText = "SELECT E.USERNAME,E.EMAIL FROM CUSTOMER C INNER JOIN EMAIL_GROUP G ON C.HANDLE = G.HANDLE_CUSTOMER INNER JOIN EMAIL_CONFIG E ON G.HANDLE_EMAIL = E.HANDLE WHERE CUSTOMER = 'ZEBRA';";

                    SqlDataReader SDR_Email = command.ExecuteReader();
                    StringBuilder emailList = new StringBuilder();
                    while (SDR_Email.Read())
                    {
                        emailList.Append(SDR_Email[1].ToString());
                        emailList.Append(",");
                    }
                    SDR_Email.Close();
                    //生成邮件内容
                    //command.CommandText = "SELECT COUNT(1) NUM,HANDLE_TASK,CELLBATCH FROM ZEBRA_SYNCBATTERY_LOG WHERE HANDLE_TASK = " + task.Handle + " GROUP BY HANDLE_TASK,CELLBATCH;";
                    command.CommandText = "WITH A AS (SELECT HANDLE_TASK,COUNT(1) NUM,CELLBATCH FROM ZEBRA_SYNCBATTERY_LOG WHERE HANDLE_TASK = " + task.Handle + " GROUP BY HANDLE_TASK,CELLBATCH),B AS (SELECT DISTINCT HANDLE_TASK,ISNULL(CONVERT(VARCHAR(20), CELLBATCH), 'TOTAL') AS 'CELLBATCH',SUM(NUM) AS 'QTY_TOTAL' FROM A GROUP BY HANDLE_TASK,CELLBATCH WITH ROLLUP) SELECT * FROM B WHERE HANDLE_TASK IS NOT NULL ORDER BY CELLBATCH ASC;";
                    SqlDataReader SDR_SyncLog = command.ExecuteReader();
                    TableWeb tWeb = new TableWeb();
                    tWeb.addThead("任务编号");
                    tWeb.addThead("批次编号");
                    tWeb.addThead("成功发送数量");
                    while (SDR_SyncLog.Read())
                    {
                        tWeb.addContext(SDR_SyncLog[0].ToString());
                        tWeb.addContext(SDR_SyncLog[1].ToString());
                        tWeb.addContext(SDR_SyncLog[2].ToString());
                    }
                    SDR_SyncLog.Close();
                    //发送邮件
                    Mail.SendMail(emailList.ToString(), System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], "Zebra 数据发送任务", tWeb.TableHtml());

                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        /// <summary>
        /// 接收回复信息并记录日志中
        /// </summary>
        /// <param name="result">回复结果</param>
        /// <returns>True：成功；False：失败</returns>
        public override bool RecievedMsgLog(ResponseResult result)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    StringBuilder sqlStr = new StringBuilder();
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    bool state = true;
                    string sql = null;
                    if (((Zebra_ResponseResult)result).data == null)
                    {
                        sql = "INSERT INTO ZEBRA_RECIEVED_LOG (COUNT,CODE,MSG,HANDLE_ZEBRA_RECIEVED_INFO) VALUES ('{0}','{1}','{2}','{3}');";
                        sql = string.Format(sql, ((Zebra_ResponseResult)result).count.ToString(), ((Zebra_ResponseResult)result).code.ToString(), ((Zebra_ResponseResult)result).msg.ToString(), ((Zebra_ResponseResult)result).ret.ToString());
                    }
                    else
                    {
                        sql = "INSERT INTO ZEBRA_RECIEVED_LOG (COUNT,CODE,MSG,DATA,HANDLE_ZEBRA_RECIEVED_INFO) VALUES ('{0}','{1}','{2}','{3}','{4}');";
                        sql = string.Format(sql, ((Zebra_ResponseResult)result).count.ToString(), ((Zebra_ResponseResult)result).code.ToString(), ((Zebra_ResponseResult)result).msg.ToString(), ((Zebra_ResponseResult)result).data.ToString(), ((Zebra_ResponseResult)result).ret.ToString());
                    }
                    sqlStr.Append(sql);
                    if (((Zebra_ResponseResult)result).code != "0000")   // Zebra 返回“失败”
                    {
                        state = false;
                    }
                    command.CommandText = sqlStr.ToString();
                    command.ExecuteNonQuery();
                    return state;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 完整记录客户端返回的信息
        /// </summary>
        /// <param name="jsonStr"></param>
        public override void SendInfoLog(string jsonStr)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("INSERT INTO ZEBRA_SEND_INFO (INFO_JSON) VALUES ('{0}');", jsonStr);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
        }

    }
    /// <summary>
    /// HW 数据回传
    /// </summary>
    public class SyncHW : SyncService
    {
        public override bool RecievedMsgLog(ResponseResult result)
        {
            return true;
        }
        public override bool IsValid(string taskID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("SELECT COUNT(1) FROM HW_TASK WHERE STATE = '未开始' AND HANDLE = '{0}';", taskID);
                    int result = Convert.ToInt32(command.ExecuteScalar());
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return false;
            }
        }
        public override void SendInfoLog(string jsonStr)
        {

        }
        public override void Complete(ref SyncTask task)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    string taskInfo;
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    //更新任务状态
                    if (string.IsNullOrEmpty(task.STATE))
                    {
                        command.CommandText = string.Format("UPDATE HW_TASK SET STATE = '已完成' WHERE HANDLE = '{0}'", task.Handle);
                        taskInfo = "任务编号：" + task.Handle + " 的发送任务已成功完成，共向客户端发送 " + task.COUNT + " 条出货数据.";
                    }
                    else
                    {
                        command.CommandText = string.Format("UPDATE HW_TASK SET STATE = '{1}' WHERE HANDLE = {0}", task.Handle, task.STATE);
                        taskInfo = "任务编号：" + task.Handle + " 的发送任务" + task.STATE + "，请反馈.";
                    }
                    command.ExecuteNonQuery();
                    //生成收件人列表
                    //command.CommandText = "SELECT USERNAME,EMAIL FROM EMAIL_CONFIG;";
                    command.CommandText = "SELECT E.USERNAME,E.EMAIL FROM CUSTOMER C INNER JOIN EMAIL_GROUP G ON C.HANDLE = G.HANDLE_CUSTOMER INNER JOIN EMAIL_CONFIG E ON G.HANDLE_EMAIL = E.HANDLE WHERE CUSTOMER = 'HW';";

                    SqlDataReader SDR_Email = command.ExecuteReader();
                    StringBuilder emailList = new StringBuilder();
                    while (SDR_Email.Read())
                    {
                        emailList.Append(SDR_Email[1].ToString());
                        emailList.Append(",");
                    }
                    SDR_Email.Close();
                    //生成邮件内容
                    //command.CommandText = "SELECT COUNT(1) NUM,HANDLE_TASK,CELLBATCH FROM ZEBRA_SYNCBATTERY_LOG WHERE HANDLE_TASK = " + task.Handle + " GROUP BY HANDLE_TASK,CELLBATCH;";
                    command.CommandText = "WITH A AS (SELECT HANDLE_TASK,COUNT(1) NUM,CELLBATCH FROM ZEBRA_SYNCBATTERY_LOG WHERE HANDLE_TASK = " + task.Handle + " GROUP BY HANDLE_TASK,CELLBATCH),B AS (SELECT DISTINCT HANDLE_TASK,ISNULL(CONVERT(VARCHAR(20), CELLBATCH), 'TOTAL') AS 'CELLBATCH',SUM(NUM) AS 'QTY_TOTAL' FROM A GROUP BY HANDLE_TASK,CELLBATCH WITH ROLLUP) SELECT * FROM B WHERE HANDLE_TASK IS NOT NULL ORDER BY CELLBATCH ASC;";
                    SqlDataReader SDR_SyncLog = command.ExecuteReader();
                    TableWeb tWeb = new TableWeb();
                    tWeb.addThead("任务编号");
                    tWeb.addThead("批次编号");
                    tWeb.addThead("成功发送数量");
                    while (SDR_SyncLog.Read())
                    {
                        tWeb.addContext(SDR_SyncLog[0].ToString());
                        tWeb.addContext(SDR_SyncLog[1].ToString());
                        tWeb.addContext(SDR_SyncLog[2].ToString());
                    }
                    SDR_SyncLog.Close();
                    //发送邮件
                    Mail.SendMail(emailList.ToString(), System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], "Zebra 数据发送任务", tWeb.TableHtml());

                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public SyncTask GetTask(string HandleTask)
        {
            HWTask task = new HWTask();
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(string.Format("SELECT * FROM HW_TASK WHERE HANDLE = '{0}'", HandleTask), conn);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        task.Handle = reader["HANDLE"].ToString();
                        task.APPID = reader["APPID"].ToString();
                        task.SIGN = reader["APPKEY"].ToString();
                        task.URI = reader["URI"].ToString();
                        task.ID = reader["EXPRESS_QTY"].ToString();
                        task.STATE = reader["STATE"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
            return task;
        }
        public override int SyncData()
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM HW_TASK WHERE STATE = '准备就绪';", conn);
                    SqlDataReader reader = command.ExecuteReader();
                    HWTask task = new HWTask();
                    while (reader.Read())
                    {
                        task.APPID = reader["APPID"].ToString();
                        task.SIGN = reader["APPKEY"].ToString();
                        task.URI = reader["URI"].ToString();
                        task.Handle = reader["HANDLE"].ToString();
                        task.ID = reader["EXPRESS_QTY"].ToString();
                    }
                    reader.Close();
                    command.CommandText = string.Format("SELECT DISTINCT TOP {0} S.MATERIALSERIALNUMBER FROM HW_SYNCBATTERY S INNER JOIN HW_TASK T ON S.HANDLE_TASK = T.HANDLE AND T.STATE = '准备就绪';", task.ID);
                    reader = command.ExecuteReader();
                    while (reader.HasRows)
                    {
                        List<string> batterys = new List<string>();
                        while (reader.Read())
                        {
                            batterys.Add(reader["MATERIALSERIALNUMBER"].ToString());
                        }
                        count += CreateData(batterys, task.Handle);
                        reader.Close();
                        reader = command.ExecuteReader();
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
            return count;
        }
        public int CreateData(List<string> batterys, string task)
        {
            if (batterys.Count == 0)
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                int _count = 1;

                try
                {
                    conn.Open();
                    StringBuilder sql = new StringBuilder("SELECT * FROM HW_SYNCBATTERY WHERE MATERIALSERIALNUMBER IN ('");
                    for (int i = 0; i < batterys.Count; ++i)
                    {
                        sql.Append(batterys[i]);
                        sql.Append("','");
                    }
                    SqlCommand command = new SqlCommand(sql.ToString().Substring(0, sql.ToString().Length - 2) + ") ORDER BY MATERIALSERIALNUMBER ASC;", conn);
                    SqlDataReader reader = command.ExecuteReader();
                    string batteryNo = null;
                    UutInfo uutInfo;
                    UutResult uutResult;
                    Products product = new Products();
                    List<Products> productList = new List<Products>();
                    while (reader.Read())
                    {
                        if (batteryNo != reader["MATERIALSERIALNUMBER"].ToString().Trim())
                        {
                            if (batteryNo != null)
                            {
                                productList.Add(product);
                            }
                            product = new Products();
                            product.uploadType = null;
                            product.factoryCode = reader["FACTORYCODE"].ToString();
                            product.workshop = null;
                            product.group = null;
                            product.productLine = reader["PRODUCTLINE"].ToString();
                            //////////////////////////////////////////////////////////////////////////////////////////////////
                            uutInfo = new UutInfo();
                            uutInfo.materialCode = reader["MATERIALCODE"].ToString();
                            uutInfo.materialSerialNumber = reader["MATERIALSERIALNUMBER"].ToString();
                            uutInfo.supplierCode = reader["SUPPLIERCODE"].ToString();
                            uutInfo.testMethod = reader["TESTMETHOD"].ToString();
                            uutInfo.orderNumber = null;
                            uutInfo.testOjectType = null;
                            uutInfo.lotCode = null;
                            uutInfo.materialMould = null;
                            uutInfo.materialMouldCavity = null;
                            uutInfo.description = null;
                            uutInfo.dateCode = null;
                            uutInfo.materialColor = null;
                            //////////////////////////////////////////////////////////////////////////////////////////////////
                            uutResult = new UutResult();
                            uutResult.testSequence = reader["TESTSEQUENCE"].ToString();
                            uutResult.testStartTime = long.Parse(TimeStamp.GetTimeStampByString(reader["TESTSTARTTIME"].ToString()));
                            uutResult.testEndTime = long.Parse(TimeStamp.GetTimeStampByString(reader["TESTENDTIME"].ToString()));
                            uutResult.testResult = reader["TESTRESULT"].ToString();
                            uutResult.Operator = null;
                            uutResult.testStation = null;
                            //////////////////////////////////////////////////////////////////////////////////////////////////
                            product.uutInfo = uutInfo;
                            product.uutResult = uutResult;
                            batteryNo = reader["MATERIALSERIALNUMBER"].ToString().Trim();
                        }
                            //////////////////////////////////////////////////////////////////////////////////////////////////
                        TestItem item = new TestItem();
                        item.testItemName = reader["TESTITEMNAME"].ToString();
                        item.testStartTime = long.Parse(TimeStamp.GetTimeStampByString(reader["TESTSTARTTIME"].ToString()));
                        item.testEndTime = long.Parse(TimeStamp.GetTimeStampByString(reader["TESTENDTIME"].ToString()));
                        item.testResult = reader["TESTRESULT"].ToString();
                        item.isValueFlag = reader["ISVALUEFLAG"].ToString();
                        item.testValue = double.Parse(string.IsNullOrEmpty(reader["TESTVALUE"].ToString()) ? "0" : reader["TESTVALUE"].ToString());
                        item.testUpperLimit = double.Parse(reader["TESTUPPERLIMIT"].ToString());
                        item.testLowerLimit = double.Parse(reader["TESTLOWERLIMIT"].ToString());
                        product.testItemList.Add(item);
                        ++_count;
                    }
                    // 补充最后一条
                    productList.Add(product);

                    // 转换成 json 字符串
                    string json = JsonConvert.SerializeObject(productList);
                    reader.Close();

                    // 将准备好的的 JSON 存入待发送数据表中，同时将 JSON HANDLE 返回
                    command.CommandText = string.Format("INSERT INTO HW_SEND_INFO (HANDLE_TASK,CONTENTS) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}');", task, json);
                    string handle = command.ExecuteScalar().ToString();

                    // 使用 JSON HANDLE 更新 HW_SYNCBATTERY_LOG
                    int count = UpdateHW_SYNCBATTERY_LOG(batterys, task, handle);
                    //if (count == productList.Count)
                    //{
                    return count;
                    //}
                    //else
                    //{
                    //    throw new Exception("Exception，public int CreateData (List<string> batterys,string task)：The qty of HW_SYNCBATTERY_LOG is different from that of HW_SEND_INFO.");
                    //}
                }
                catch (Exception ex)
                {
                    StackFrame sf = new StackFrame(true);
                    int lineNumber = sf.GetFileLineNumber();
                    int colNumber = sf.GetFileColumnNumber();
                    string fileName = sf.GetFileName();
                    string methodName = sf.GetMethod().Name;
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public int UpdateHW_SYNCBATTERY_LOG(List<string> batterys, string task, string handle)
        {
            if (batterys.Count == 0)
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    string sqlStr = "INSERT HW_SYNCBATTERY_LOG (HANDLE_SEND_INFO,HANDLE_TASK,UPLOADTYPE,FACTORYCODE,WORKSHOP,[GROUP],PRODUCTLINE,ORDERNUMBER,TESTOJECTTYPE,TESTMETHOD,MATERIALCODE,MATERIALSERIALNUMBER,SUPPLIERCODE,LOTCODE,MATERIALMOULD,MATERIALMOULDCAVITY,DESCRIPTION,DATECODE,MATERIALCOLOR,OPERATOR,TESTSEQUENCE,TESTSTATION,TESTSTARTTIME,TESTENDTIME,TESTRESULT,TEESTITEEMNUMBER,TESTITEMNAME,TESTITEMSTARTTIME,TESTITEMENDTIME,TESTITEMRESULT,TESTRESULTDESCRIPTION,ISVALUEFLAG,TESTVALUE,TESTUPPERLIMIT,TESTLOWERLIMIT,TESTDEVICENAME) SELECT '{0}','{1}' ,UPLOADTYPE,FACTORYCODE,WORKSHOP,[GROUP],PRODUCTLINE,ORDERNUMBER,TESTOJECTTYPE,TESTMETHOD,MATERIALCODE,MATERIALSERIALNUMBER,SUPPLIERCODE,LOTCODE,MATERIALMOULD,MATERIALMOULDCAVITY,DESCRIPTION,DATECODE,MATERIALCOLOR,OPERATOR,TESTSEQUENCE,TESTSTATION,TESTSTARTTIME,TESTENDTIME,TESTRESULT,TEESTITEEMNUMBER,TESTITEMNAME,TESTITEMSTARTTIME,TESTITEMENDTIME,TESTITEMRESULT,TESTRESULTDESCRIPTION,ISVALUEFLAG,TESTVALUE,TESTUPPERLIMIT,TESTLOWERLIMIT,TESTDEVICENAME FROM HW_SYNCBATTERY WHERE MATERIALSERIALNUMBER IN (";
                    StringBuilder sql = new StringBuilder();
                    for (int i = 0; i < batterys.Count; ++i)
                    {
                        sql.Append("'");
                        sql.Append(batterys[i]);
                        sql.Append("',");
                    }
                    SqlCommand command = new SqlCommand(string.Format(sqlStr.ToString(), handle, task) + sql.ToString().Substring(0, sql.ToString().Length - 1) + ");", conn);
                    int count = command.ExecuteNonQuery();
                    command.CommandText = "DELETE FROM HW_SYNCBATTERY WHERE MATERIALSERIALNUMBER IN (" + sql.ToString().Substring(0, sql.ToString().Length - 1) + ");";
                    if (count == command.ExecuteNonQuery())
                    {
                        return count;
                    }
                    else
                    {
                        throw new Exception("Exception，UpdateHW_SYNCBATTERY_LOG (List<string> batterys,string task,string handle)：The qty of HW_SYNCBATTERY_LOG is different from that of HW_SYNCBATTERY.");
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public override void UpdateTask(ref SyncTask task)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(string.Format("UPDATE HW_TASK SET STATE = '{0}' WHERE HANDLE = '{1}';", task.STATE, task.Handle), conn);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
        }
        public override int SendData(ref SyncTask task)
        {
            //定义web service对象
            WebSOAP soap = new WebSOAP();
            HW_Result result = new HW_Result();
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    string sql = string.Format("SELECT S.*,'{1}' AS APPID,'{2}' AS APPKEY,'{3}' AS URI FROM HW_SEND_INFO S WHERE STATE IS NULL AND HANDLE_TASK = '{0}';", task.Handle, task.APPID, task.SIGN, task.URI);
                    SqlCommand command = new SqlCommand(sql, conn);
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    reader.Close();
                    task.STATE = "开始回传.";
                    UpdateTask(ref task);
                    foreach (DataRow row in dt.Rows)
                    {
                        result = soap.HW_QuerySoapWebService(row["URI"].ToString(), row["CONTENTS"].ToString(), row["APPID"].ToString(), row["APPKEY"].ToString());
                        if (result.code == 10000)
                        {
                            SqlTransaction tran = conn.BeginTransaction();
                            command.Transaction = tran;
                            command.CommandText = string.Format("INSERT INTO HW_SEND_LOG (HANDLE,HANDLE_TASK,CONTENTS) VALUES ('{0}','{1}','{2}'); DELETE FROM HW_SEND_INFO WHERE HANDLE = '{0}';", row["HANDLE"].ToString(), row["HANDLE_TASK"].ToString(), row["CONTENTS"].ToString());
                            command.ExecuteNonQuery();
                            tran.Commit();
                            command.CommandText = string.Format("INSERT INTO HW_RECIEVED_LOG (HANDLE_SEND_INFO,HANDLE_TASK,CODE,MESSAGE,DATA) VALUES ('{0}','{1}','{2}','{3}','{4}');", row["HANDLE"].ToString(), row["HANDLE_TASK"].ToString(), result.code, result.message, result.data);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.CommandText = string.Format("INSERT INTO HW_RECIEVED_ERROR (HANDLE_SEND_INFO,HANDLE_TASK,RECIEVED_INFO) VALUES ('{0}','{1}','{2}');", row["HANDLE"].ToString(), row["HANDLE_TASK"].ToString(), result.message);
                            command.ExecuteNonQuery();
                            task.STATE = "失败";
                        }
                    }
                    if (task.STATE != "失败")
                    {
                        command.CommandText = string.Format("SELECT COUNT(1) FROM HW_SYNCBATTERY_LOG WHERE HANDLE_TASK = '{0}'", task.Handle);
                        task.COUNT = int.Parse(command.ExecuteScalar().ToString());
                        task.STATE = null;
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    task.STATE = "失败";
                    UpdateTask(ref task);
                    return 0;
                }
            }
            //UpdateTask(ref task);
            Complete(ref task);
            return 1;
        }
        public string SyncType { get; set; }
        public HW_Result SendData(string json)
        {
            WebSOAP soap = new WebSOAP();
            HW_Result result = soap.HW_QuerySoapWebService("https://apigw-scs-beta.huawei.com/api/service/esupplier/receivetestdata/1.0.0", json, "APP_Z0001K_B2B", "BuynrEsDjM/ZyDOBcvkgUQ==");
            return result;
        }
        public override SyncTask CreateTask()
        {
            return CreateHWTask();
        }
        public HWTask CreateHWTask()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    HWTask task = new HWTask();
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    //查询当前配置
                    command.CommandText = string.Format("SELECT HANDLE,APPID,APPKEY,PUSHURL,EXPRESS_QTY FROM HW_CONFIG WHERE INTERFACE = '{0}' AND IS_CURRENT = 1;", SyncType);
                    SqlDataReader SDR_Config = command.ExecuteReader();
                    while (SDR_Config.Read())
                    {
                        task.Handle = SDR_Config[0].ToString();
                        task.APPID = SDR_Config[1].ToString();
                        task.SIGN = SDR_Config[2].ToString();
                        task.URI = SDR_Config[3].ToString();
                        task.ID = SDR_Config[4].ToString();
                    }
                    SDR_Config.Close();
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public string AddHWTask(SyncTask task)
        {
            try
            {
                if (task == null)
                {
                    throw new Exception("没有获得任务对象.");
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(string.Format("INSERT INTO HW_TASK (HANDLE_CONFIG,APPID,APPKEY,URI,EXPRESS_QTY) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}','{2}','{3}','{4}');", task.Handle, task.APPID, task.SIGN, task.URI, task.ID), conn);
                    string handle = command.ExecuteScalar().ToString();
                    conn.Close();
                    return handle;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public override int AddTask(SyncTask task)
        {
            return 0;
        }
        public override int Ready(string taskID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("UPDATE HW_TASK SET STATE = '准备就绪' WHERE HANDLE = '{0}';", taskID);
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        public override int CheckIn(string batteryInfo, string taskID)
        {
            try
            {
                if (string.IsNullOrEmpty(batteryInfo) || string.IsNullOrEmpty(taskID))
                {
                    throw new Exception("Exception，CheckIn(string batteryInfo, string taskID)：batteryInfo or taskID is empty.");
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncRemote database can not be open.");
                    }
                    string sql = null;
                    int count = 0;
                    // 生产数据
                    if (batteryInfo.Trim() == "RECEIVETESTDATA")
                    {
                        SqlCommand comm = new SqlCommand("SELECT BOX_NO FROM HW_RECIEVE_BOX WHERE STATE = '0';", conn);
                        SqlDataReader m_reader = comm.ExecuteReader();
                        List<string> boxList = new List<string>();
                        while (m_reader.Read())
                        {
                            boxList.Add(m_reader["BOX_NO"].ToString());
                        }
                        m_reader.Close();
                        foreach (string box_no in boxList)
                        {
                            sql = string.Format("SELECT CON.FACTORY_CODE AS FACTORYCODE,BAT.PRODUCT_LINE AS PRODUCTLINE,'0' AS TESTOBJECTTYPE,'全检' AS TESTMETHOD,CON.CUSTOMER_ITEM AS MATERIALCODE,SN AS MATERIALSERIALNUMBER,CON.SUPPLIES_NO AS SUPPLIERCODE,TEST_START_TIME AS TESTSTARTTIME,TEST_END_TIME AS TESTENDTIME,TEST_RESULT AS TESTRESULT,CUSTOMER_OPERATION AS TESTSEQUENCE,CUSTOMER_PARAM_DESC AS TESTITEMNAME,'Y' AS ISVALUEFLAG,PARAM_VALUE AS TESTVALUE,CUSTOMER_LOWER_LIMIT AS TESTLOWERLIMIT,CUSTOMER_UPPER_LIMIT AS TESTUPPERLIMIT,TEST_EQUIP AS TESTDEVICENAME FROM HW_RECIEVE_BATTERY BAT INNER JOIN HW_RECIEVE_BOX BOX ON BAT.STATE = '0' AND BAT.REF_BOX_NO = BOX.BOX_NO AND BOX.BOX_NO = '{0}' INNER JOIN HW_PRODUCT_CONFIG CON ON BOX.ITEM = CON.ITEM_NO;", box_no);
                            SqlCommand command = new SqlCommand(sql, conn);
                            SqlDataReader reader = command.ExecuteReader();
                            if (!reader.HasRows)
                            {
                                reader.Close();
                                SyncTask task = new HWTask();
                                task.Handle = taskID;
                                task.STATE = "关闭，无可用数据.";
                                UpdateTask(ref task);
                                throw new Exception("Exception，no data available.");
                            }
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            reader.Close();
                            SqlTransaction tran = conn.BeginTransaction();
                            command.Transaction = tran;
                            for (int i = 0; i < dt.Rows.Count; ++i)
                            {
                                command.CommandText = string.Format("INSERT INTO HW_SYNCBATTERY (HANDLE_TASK,UPLOADTYPE,FACTORYCODE,WORKSHOP,[GROUP],PRODUCTLINE,ORDERNUMBER,TESTOJECTTYPE,TESTMETHOD,MATERIALCODE,MATERIALSERIALNUMBER,SUPPLIERCODE,LOTCODE,MATERIALMOULD,MATERIALMOULDCAVITY,DESCRIPTION,DATECODE,MATERIALCOLOR,OPERATOR,TESTSEQUENCE,TESTSTATION,TESTSTARTTIME,TESTENDTIME,TESTRESULT,TEESTITEEMNUMBER,TESTITEMNAME,TESTITEMSTARTTIME,TESTITEMENDTIME,TESTITEMRESULT,TESTRESULTDESCRIPTION,ISVALUEFLAG,TESTVALUE,TESTUPPERLIMIT,TESTLOWERLIMIT,TESTDEVICENAME) VALUES ('{0}',NULL,'{1}',NULL,NULL,'{2}',NULL,NULL,'{3}','{4}','{5}','{16}',NULL,NULL,NULL,NULL,NULL,NULL,NULL,'{6}',NULL,'{7}','{8}','{9}',NULL,'{10}','{7}','{8}','{9}',NULL,'{11}','{12}','{13}','{14}','{15}');", taskID, dt.Rows[i]["FACTORYCODE"].ToString(), dt.Rows[i]["PRODUCTLINE"].ToString(), dt.Rows[i]["TESTMETHOD"].ToString(), dt.Rows[i]["MATERIALCODE"].ToString(), dt.Rows[i]["MATERIALSERIALNUMBER"].ToString(), dt.Rows[i]["TESTSEQUENCE"].ToString(), dt.Rows[i]["TESTSTARTTIME"].ToString(), dt.Rows[i]["TESTENDTIME"].ToString(), dt.Rows[i]["TESTRESULT"].ToString(), dt.Rows[i]["TESTITEMNAME"].ToString(), dt.Rows[i]["ISVALUEFLAG"].ToString(), dt.Rows[i]["TESTVALUE"].ToString(), dt.Rows[i]["TESTUPPERLIMIT"].ToString(), dt.Rows[i]["TESTLOWERLIMIT"].ToString(), dt.Rows[i]["TESTDEVICENAME"].ToString(),dt.Rows[i]["SUPPLIERCODE"]);
                                count += command.ExecuteNonQuery();
                            }
                            tran.Commit();
                            UpdateRecieveBattery(box_no);
                        }
                        m_reader.Close();
                    }
                    // 质量检验数据
                    else if (batteryInfo.Trim() == "RECEIVETESTQUALITYDATA")
                    {

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
        public bool UpdateRecieveBattery(string box_no)
        {
            try
            {
                if (string.IsNullOrEmpty(box_no))
                {
                    throw new Exception("box_no is empty.");
                }
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncRemote database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("UPDATE HW_RECIEVE_BOX SET STATE = '1' WHERE BOX_NO = '{0}';UPDATE HW_RECIEVE_BATTERY SET STATE = '1' WHERE REF_BOX_NO = '{0}';", box_no), conn);
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
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
    ///  Desay 数据回传
    /// </summary>
    public class SyncDesay
    {
        private string customer;
        public string Customer
        {
            set
            {
                customer = value;
            }
            get
            {
                return customer;
            }
        }
        public List<BomDesay> GetBom()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                //打开连接
                conn.Open();
                if (conn.State != ConnectionState.Open) return null;
                SqlCommand comm = new SqlCommand("SELECT BOMNO,REMARKS,VERSION FROM DESAY_PRODUCT_CONFIG WHERE IS_CURRENT = 'Y' GROUP BY BOMNO,REMARKS,VERSION;", conn);
                SqlDataReader reader = comm.ExecuteReader();
                if (!reader.HasRows) return null;
                List<BomDesay> lstBom = new List<BomDesay>();
                while (reader.Read())
                {
                    BomDesay entity = new BomDesay();
                    entity.BOMNO = reader["BOMNO"].ToString();
                    entity.REMARK = reader["REMARKS"].ToString();
                    entity.VERSION = reader["VERSION"].ToString();
                    lstBom.Add(entity);
                }
                return lstBom;
            }
        }
        public DesayTask CreateDesayTask(string customer)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                //初始化Config对象
                DesayTask DesayConfig = new DesayTask();
                SqlCommand command = new SqlCommand();
                try
                {
                    //打开连接
                    conn.Open();
                    command.Connection = conn;
                    //查询当前配置
                    command.CommandText = string.Format("INSERT INTO DESAY_TASK (HANDLE_CONFIG,HANDLE_PRODUCT_CONFIG,APPID,APPKEY,URI_LOGIN,URI_LOGOUT,URI_POST,MODULE,EXPRESS_QTY,STATE) OUTPUT INSERTED.HANDLE SELECT C.HANDLE,PC.HANDLE,C.APP_ID,C.APP_KEY,C.URL_LOGIN,C.URL_LOGOUT,C.URL_POST,PC.MODULE,100,'未开始' FROM DESAY_CONFIG C INNER JOIN DESAY_PRODUCT_CONFIG PC ON C.IS_CURRENT = PC.IS_CURRENT AND PC.REMARKS = '{0}' AND C.REMARKS = PC.REMARKS; ", customer);
                    string handle = command.ExecuteScalar().ToString();
                    command.CommandText = string.Format("SELECT T.HANDLE,HANDLE_CONFIG,HANDLE_PRODUCT_CONFIG,APPID,APPKEY,TICKET,URI_LOGIN,URI_LOGOUT,URI_POST,MODULE,EXPRESS_QTY,STATE,T.CREATED_DATE_TIME,VERSION FROM DESAY_TASK T INNER JOIN DESAY_CONFIG C ON C.HANDLE = T.HANDLE_CONFIG WHERE T.HANDLE = '{0}';", handle);
                    SqlDataReader SDR_Config = command.ExecuteReader();
                    while (SDR_Config.Read())
                    {
                        DesayConfig.HANDLE = SDR_Config[0].ToString();
                        DesayConfig.HANDLE_CONFIG = SDR_Config[1].ToString();
                        DesayConfig.HANDLE_PRODUCT_CONFIG = SDR_Config[2].ToString();
                        DesayConfig.APPID = SDR_Config[3].ToString();
                        DesayConfig.APPKEY = SDR_Config[4].ToString();
                        DesayConfig.URI_LOGIN = SDR_Config[6].ToString();
                        DesayConfig.URI_LOGOUT = SDR_Config[7].ToString();
                        DesayConfig.URI_POST = SDR_Config[8].ToString();
                        DesayConfig.MODULE = SDR_Config[9].ToString();
                        DesayConfig.EXPRESS = int.Parse(SDR_Config[10].ToString());
                        DesayConfig.VERSION= SDR_Config["VERSION"].ToString();
                    }
                    SDR_Config.Close();
                    return DesayConfig;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public DesayTask CreateDesayTaskByBomno(string bomno)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                //初始化Config对象
                DesayTask DesayConfig = new DesayTask();
                SqlCommand command = new SqlCommand();
                try
                {
                    //打开连接
                    conn.Open();
                    command.Connection = conn;
                    //查询当前配置
                    command.CommandText = string.Format("INSERT INTO DESAY_TASK (HANDLE_CONFIG,HANDLE_PRODUCT_CONFIG,APPID,APPKEY,URI_LOGIN,URI_LOGOUT,URI_POST,MODULE,EXPRESS_QTY,STATE) OUTPUT INSERTED.HANDLE SELECT DC.HANDLE,DPC.HANDLE,DC.APP_ID,DC.APP_KEY,DC.URL_LOGIN,DC.URL_LOGOUT,DC.URL_POST,DPC.MODULE,100,'未开始' FROM DESAY_PRODUCT_CONFIG dpc INNER JOIN DESAY_CONFIG dc ON DC.REMARKS = DPC.REMARKS AND DC.VERSION = DPC.VERSION WHERE DPC.BOMNO = '{0}';", bomno);
                    string handle = command.ExecuteScalar().ToString();
                    command.CommandText = string.Format("SELECT T.HANDLE,HANDLE_CONFIG,HANDLE_PRODUCT_CONFIG,APPID,APPKEY,TICKET,URI_LOGIN,URI_LOGOUT,URI_POST,MODULE,EXPRESS_QTY,STATE,T.CREATED_DATE_TIME,VERSION FROM DESAY_TASK T INNER JOIN DESAY_CONFIG C ON C.HANDLE = T.HANDLE_CONFIG WHERE T.HANDLE = '{0}';", handle);
                    SqlDataReader SDR_Config = command.ExecuteReader();
                    while (SDR_Config.Read())
                    {
                        DesayConfig.HANDLE = SDR_Config[0].ToString();
                        DesayConfig.HANDLE_CONFIG = SDR_Config[1].ToString();
                        DesayConfig.HANDLE_PRODUCT_CONFIG = SDR_Config[2].ToString();
                        DesayConfig.APPID = SDR_Config[3].ToString();
                        DesayConfig.APPKEY = SDR_Config[4].ToString();
                        DesayConfig.URI_LOGIN = SDR_Config[6].ToString();
                        DesayConfig.URI_LOGOUT = SDR_Config[7].ToString();
                        DesayConfig.URI_POST = SDR_Config[8].ToString();
                        DesayConfig.MODULE = SDR_Config[9].ToString();
                        DesayConfig.EXPRESS = int.Parse(SDR_Config[10].ToString());
                        DesayConfig.VERSION = SDR_Config["VERSION"].ToString();
                    }
                    SDR_Config.Close();
                    return DesayConfig;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public int CheckIn(string batteryInfo, DesayTask task)
        {
            int affectRow = 0;
            //BatteryDesay ship = new BatteryDesay();

            //List<BatteryDesay> ship = new List<BatteryDesay>();
            //List<BatteryDesay2> ship2 = new List<BatteryDesay2>();

            BatteryDesayList bl = new BatteryDesayList();
            // 2023-11-20 注销
            BatteryDesayList2 bl2 = new BatteryDesayList2();
            // 2023-11-20 新增
            BatteryDesayList3 bl3 = new BatteryDesayList3();
            try
            {
                //整理数据
                if (task.CUSTOMER == "HW" && task.VERSION == "1.0")
                {
                    bl = JsonConvert.DeserializeObject<BatteryDesayList>(batteryInfo);
                }
                else if (task.CUSTOMER == "COMMON" && task.VERSION == "1.0")
                {
                    bl2 = JsonConvert.DeserializeObject<BatteryDesayList2>(batteryInfo);
                }
                // 2023-11-20 新增
                else if (task.CUSTOMER == "COMMON" && task.VERSION == "2.6")
                {
                    bl3 = JsonConvert.DeserializeObject<BatteryDesayList3>(batteryInfo);
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
            //写入数据库
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                //打开连接
                conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    throw new Exception("SyncDesay::CheckIn => Desay database can not be connection.");
                }
                //创建事务
                SqlTransaction newTrans = conn.BeginTransaction();
                //创建命令
                SqlCommand command = new SqlCommand();
                try
                {
                    command.Connection = conn;
                    command.Transaction = newTrans;
                    StringBuilder sqlStr = new StringBuilder();
                    //遍历数据
                    if (task.CUSTOMER == "HW" && task.VERSION == "1.0")
                    {
                        for (int i = 0; i < bl.BatteryList.Count; ++i)
                        {
                            sqlStr.Append("INSERT INTO DESAY_SYNCBATTERY (HANDLE_TASK,BARCODE,OCV0,OCV,IR,DATE0,DATE,CAPACITY,KDATA) VALUES ('");
                            sqlStr.Append(task.HANDLE);
                            sqlStr.Append("','");
                            sqlStr.Append(bl.BatteryList[i].barcode);
                            sqlStr.Append("','");
                            sqlStr.Append(bl.BatteryList[i].ocv0);
                            sqlStr.Append("','");
                            sqlStr.Append(bl.BatteryList[i].ocv);
                            sqlStr.Append("','");
                            sqlStr.Append(bl.BatteryList[i].ir);
                            sqlStr.Append("','");
                            sqlStr.Append(bl.BatteryList[i].date0);
                            sqlStr.Append("','");
                            sqlStr.Append(bl.BatteryList[i].date);
                            if (string.IsNullOrEmpty(bl.BatteryList[i].Capacity.ToString()))
                            {
                                sqlStr.Append("',NULL,");
                            }
                            else
                            {
                                sqlStr.Append("','");
                                sqlStr.Append(bl.BatteryList[i].Capacity);
                                sqlStr.Append("',");
                            }
                            if (string.IsNullOrEmpty(bl.BatteryList[i].KData.ToString()))
                            {
                                sqlStr.Append("NULL);");
                            }
                            else
                            {
                                sqlStr.Append("'");
                                sqlStr.Append(bl.BatteryList[i].KData);
                                sqlStr.Append("');");
                            }
                            //sqlStr.Append("','");
                            //sqlStr.Append(ship[i].Result);
                        }
                    }
                    else if (task.CUSTOMER == "COMMON" && task.VERSION == "1.0")
                    {
                        for (int i = 0; i < bl2.BatteryList.Count; ++i)
                        {

                            sqlStr.Append("INSERT INTO DESAY_SYNCBATTERY (HANDLE_TASK,BARCODE,OCV,IR,DATE,RESULT,KDATA) VALUES ('");
                            sqlStr.Append(task.HANDLE);
                            sqlStr.Append("','");
                            sqlStr.Append(bl2.BatteryList[i].barcode);
                            sqlStr.Append("','");
                            sqlStr.Append(bl2.BatteryList[i].ocv);
                            sqlStr.Append("','");
                            sqlStr.Append(bl2.BatteryList[i].ir);
                            sqlStr.Append("','");
                            sqlStr.Append(bl2.BatteryList[i].date);
                            sqlStr.Append("','");
                            sqlStr.Append(bl2.BatteryList[i].result);
                            sqlStr.Append("','");
                            sqlStr.Append(bl2.BatteryList[i].KData);
                            sqlStr.Append("');");
                        }
                    }
                    else if (task.CUSTOMER == "COMMON" && task.VERSION == "2.6")
                    {
                        // 2023-11-20 新增
                        for (int i = 0; i < bl3.data.Count; ++i)
                        {
                            sqlStr.Append("INSERT INTO DESAY_SYNCBATTERY (HANDLE_TASK,PRODUCTCODE,CELL_BARCODE,CAPACITY_BARCODE,MODEL_NUMBER,CARTON_NUMBER,BATCH_NUM,CAPACITY,OCV,IR,K_VALUE,FDATE,BIN,REMARK) VALUES ('");
                            sqlStr.Append(task.HANDLE);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].PRODUCTCODE);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].CELL_BARCODE);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].CAPACITY_BARCODE);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].MODEL_NUMBER);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].CARTON_NUMBER);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].BATCH_NUM);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].CAPACITY);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].OCV);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].IR);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].K_VALUE);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].FDATE);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].BIN);
                            sqlStr.Append("','");
                            sqlStr.Append(bl3.data[i].REMARK);
                            sqlStr.Append("');");
                        }
                    }
                    command.CommandText = sqlStr.ToString();
                    affectRow = command.ExecuteNonQuery();
                    newTrans.Commit();
                    return affectRow;
                }
                catch (Exception ex)
                {
                    newTrans.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public int Ready(string taskID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("UPDATE DESAY_TASK SET STATE = '准备就绪' WHERE HANDLE = '{0}';", taskID);
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        public DesayTask GetDesayTask(string taskID)
        {
            try
            {
                if (string.IsNullOrEmpty(taskID))
                {
                    throw new Exception("SyncDesay::GetDesayTask => Task is not exist.");
                }
                DesayTask DesayConfig = new DesayTask();
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncDesay::GetDesayTask => SyncDesay database can not be connection.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT T.HANDLE,T.HANDLE_CONFIG,T.HANDLE_PRODUCT_CONFIG,T.APPID,T.APPKEY,T.TICKET,T.URI_LOGIN,T.URI_LOGOUT,T.URI_POST,T.MODULE,T.EXPRESS_QTY,T.STATE,T.CREATED_DATE_TIME,C.REMARKS,C.VERSION FROM DESAY_TASK T INNER JOIN DESAY_CONFIG C ON T.HANDLE = '{0}' AND T.HANDLE_CONFIG = C.HANDLE;", taskID), conn);
                    SqlDataReader SDR_Config = comm.ExecuteReader();
                    while (SDR_Config.Read())
                    {
                        DesayConfig.HANDLE = SDR_Config["HANDLE"].ToString();
                        DesayConfig.HANDLE_CONFIG = SDR_Config["HANDLE_CONFIG"].ToString();
                        DesayConfig.HANDLE_PRODUCT_CONFIG = SDR_Config["HANDLE_PRODUCT_CONFIG"].ToString();
                        DesayConfig.APPID = SDR_Config["APPID"].ToString();
                        DesayConfig.APPKEY = SDR_Config["APPKEY"].ToString();
                        DesayConfig.URI_LOGIN = SDR_Config["URI_LOGIN"].ToString();
                        DesayConfig.URI_LOGOUT = SDR_Config["URI_LOGOUT"].ToString();
                        DesayConfig.URI_POST = SDR_Config["URI_POST"].ToString();
                        DesayConfig.MODULE = SDR_Config["MODULE"].ToString();
                        DesayConfig.EXPRESS = int.Parse(SDR_Config["EXPRESS_QTY"].ToString());
                        DesayConfig.STATE = SDR_Config["STATE"].ToString();
                        DesayConfig.CUSTOMER = SDR_Config["REMARKS"].ToString();
                        DesayConfig.VERSION = SDR_Config["VERSION"].ToString();
                    }
                    SDR_Config.Close();
                    return DesayConfig;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        public int SyncData(string taskID)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(string.Format("SELECT COUNT(1) FROM DESAY_TASK WHERE HANDLE = '{0}';", taskID), conn);
                    if (int.Parse(command.ExecuteScalar().ToString()) == 0)
                    {
                        throw new Exception(string.Format("SyncDesay::SyncData => {0} mission not ready", taskID));
                    }
                    command.CommandText = string.Format("UPDATE DESAY_TASK SET STATE = '数据准备' WHERE HANDLE = '{0}';", taskID);
                    DesayTask task = GetDesayTask(taskID);
                    command.CommandText = string.Format("SELECT TOP 100 * FROM DESAY_SYNCBATTERY WHERE HANDLE_TASK = '{0}';", task.HANDLE);
                    SqlDataReader reader = command.ExecuteReader();
                    List<BatteryDesay> batterys = new List<BatteryDesay>();
                    // 2023-11-20 注销
                    List<BatteryDesay2> batterys2 = new List<BatteryDesay2>();
                    // 2023-11-20 新增
                    BatteryDesayList3 batteryList3 = new BatteryDesayList3();
                    StringBuilder barcodes = new StringBuilder();
                    barcodes.Clear();
                    while (reader.Read())
                    {
                        if (task.CUSTOMER == "HW" && task.VERSION == "1.0")
                        {
                            BatteryDesay battery = new BatteryDesay();
                            battery.barcode = reader["BARCODE"].ToString();
                            battery.Capacity = int.Parse(reader["CAPACITY"].ToString());
                            battery.ocv0 = double.Parse(reader["OCV0"].ToString());
                            battery.ocv = double.Parse(reader["OCV"].ToString());
                            battery.ir = double.Parse(reader["IR"].ToString());
                            battery.date0 = reader["DATE0"].ToString();
                            battery.date = reader["DATE"].ToString();
                            //battery.KData = reader["KDATA"].ToString();
                            batterys.Add(battery);
                            //  统计已完成码号
                            barcodes.Append("'");
                            barcodes.Append(battery.barcode);
                            barcodes.Append("',");

                        }
                        else if (task.CUSTOMER == "COMMON" && task.VERSION == "1.0")
                        {
                            // 2023-11-20 注销
                            BatteryDesay2 battery = new BatteryDesay2();
                            battery.barcode = reader["BARCODE"].ToString();
                            battery.ocv = double.Parse(reader["OCV"].ToString());
                            battery.ir = double.Parse(reader["IR"].ToString());
                            battery.date = reader["DATE"].ToString();
                            battery.result = int.Parse(reader["RESULT"].ToString());
                            battery.KData = double.Parse(reader["KData"].ToString());
                            batterys2.Add(battery);
                            //  统计已完成码号
                            barcodes.Append("'");
                            barcodes.Append(battery.barcode);
                            barcodes.Append("',");
                        }
                        else if (task.CUSTOMER == "COMMON" && task.VERSION == "2.6")
                        {
                            // 2023-11-20 新增
                            BatteryDesay3 battery = new BatteryDesay3();
                            battery.PRODUCTCODE = reader["PRODUCTCODE"].ToString();
                            battery.CELL_BARCODE = reader["CELL_BARCODE"].ToString();
                            battery.CAPACITY_BARCODE = reader["CAPACITY_BARCODE"].ToString();
                            battery.MODEL_NUMBER = reader["MODEL_NUMBER"].ToString();
                            battery.CARTON_NUMBER = reader["CARTON_NUMBER"].ToString();
                            battery.BATCH_NUM = reader["BATCH_NUM"].ToString();
                            battery.K_VALUE = reader["K_VALUE"].ToString();
                            battery.CAPACITY = reader["CAPACITY"].ToString();
                            battery.OCV = reader["OCV"].ToString();
                            battery.IR = reader["IR"].ToString();
                            battery.FDATE = reader["FDATE"].ToString();
                            battery.BIN = reader["BIN"].ToString();
                            battery.REMARK = reader["REMARK"].ToString();
                            batteryList3.data.Add(battery);
                            batteryList3.productcode = battery.PRODUCTCODE;
                            //  统计已完成码号
                            barcodes.Append("'");
                            // 2.6 版本
                            barcodes.Append(battery.CELL_BARCODE);
                            barcodes.Append("',");

                        }
                    }
                    reader.Close();
                    string str = null;
                    if (task.CUSTOMER == "HW" && task.VERSION == "1.0")
                    {
                        str = JsonConvert.SerializeObject(batterys);
                    }
                    else if (task.CUSTOMER == "COMMON" && task.VERSION == "1.0")
                    {
                        str = JsonConvert.SerializeObject(batterys2);
                    }
                    else if (task.CUSTOMER == "COMMON" && task.VERSION == "2.6")
                    {
                        // 2023-11-20 新增
                        str = JsonConvert.SerializeObject(batteryList3);
                    }
                    batterys.Clear();
                    // 2023-11-20 注销
                    //batterys2.Clear();
                    string str64 = Base64Helper.Base64Encode(str);
                    task.HANDLE_SEND_INFO = CreateSendInfo(task.HANDLE, str64, str);
                    if (string.IsNullOrEmpty(task.HANDLE_SEND_INFO))
                    {
                        throw new Exception("Fail to CreateSendInfo");
                    }
                    int count = UpdateSyncbattery(task, barcodes.ToString());
                    command.CommandText = string.Format("UPDATE DESAY_TASK SET STATE = '准备就绪' WHERE HANDLE = '{0}'", taskID);
                    command.ExecuteNonQuery();
                    conn.Close();
                    return count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public int UpdateSyncbattery(DesayTask task, string barcodes)
        {
            string handle_send_info = task.HANDLE_SEND_INFO;
            if (string.IsNullOrEmpty(barcodes))
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncDesay::UpdateSyncbattery => Desay database can not be open.");
                    }
                    string sql = null;
                    if (task.CUSTOMER == "HW")
                    {
                        sql = string.Format("INSERT INTO DESAY_SYNCBATTERY_LOG (HANDLE_TASK,HANDLE_SEND_INFO,PRODUCTCODE,CELL_BARCODE,CAPACITY_BARCODE,MODEL_NUMBER,CARTON_NUMBER,BATCH_NUM,CAPACITY,OCV,IR,K_VALUE,FDATE,BIN,REMARK) SELECT HANDLE_TASK,'{0}',PRODUCTCODE,CELL_BARCODE,CAPACITY_BARCODE,MODEL_NUMBER,CARTON_NUMBER,BATCH_NUM,CAPACITY,OCV,IR,K_VALUE,FDATE,BIN,REMARK FROM DESAY_SYNCBATTERY WHERE CELL_BARCODE IN ({1});DELETE FROM DESAY_SYNCBATTERY WHERE CELL_BARCODE IN ({1});", handle_send_info, barcodes.ToString().Substring(0, barcodes.ToString().Length - 1));
                    }
                    else if (task.CUSTOMER == "COMMON")
                    {
                        sql = string.Format("INSERT INTO DESAY_SYNCBATTERY_LOG (HANDLE_TASK,HANDLE_SEND_INFO,PRODUCTCODE,BARCODE,CAPACITY_BARCODE,MODEL_NUMBER,CARTON_NUMBER,BATCH_NUM,CAPACITY,OCV,IR,K_VALUE,FDATE,BIN,REMARK) SELECT HANDLE_TASK,'{0}',PRODUCTCODE,BARCODE,CAPACITY_BARCODE,MODEL_NUMBER,CARTON_NUMBER,BATCH_NUM,CAPACITY,OCV,IR,K_VALUE,FDATE,BIN,REMARK FROM DESAY_SYNCBATTERY WHERE BARCODE IN ({1});DELETE FROM DESAY_SYNCBATTERY WHERE BARCODE IN ({1});", handle_send_info, barcodes.ToString().Substring(0, barcodes.ToString().Length - 1));
                    }
                    SqlCommand comm = new SqlCommand(sql,conn);
                    SysLog log = new SysLog(comm.CommandText);
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
        public string CreateSendInfo(string taskHandle, string str64,string str)
        {
            if (string.IsNullOrEmpty(taskHandle) || string.IsNullOrEmpty(str))
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncDesay::CreateSendInfo => Desay database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO DESAY_SEND_INFO (HANDLE_TASK,SEND_INFO,JSON_INFO) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}','{2}') ;", taskHandle, str64,str), conn);
                    string handle = comm.ExecuteScalar().ToString();
                    return handle;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public ResultLogin Login(string app_id,string app_key,DesayTask task)
        {
            if (string.IsNullOrEmpty(app_id) || string.IsNullOrEmpty(app_key))
            {
                return null;
            }
            EntityLogin login = new EntityLogin();
            login.app_id = app_id;
            login.app_key = app_key;
            WebSOAP soap = new WebSOAP();
            ResultLogin result = soap.Login(task.URI_LOGIN, login);
            return result;
        }
        public TokenDesay Login_new(DesayTask task)
        {
            if (string.IsNullOrEmpty(task.APPID) || string.IsNullOrEmpty(task.APPKEY))
            {
                return null;
            }
            EntityLogin2 login = new EntityLogin2();
            login.username = task.APPID;
            login.password = task.APPKEY;
            WebSOAP soap = new WebSOAP();
            TokenDesay result = soap.Login_new(task.URI_LOGIN, login);
            return result;
        }
        public void SendData(DesayTask task)
        {
            try
            {
                if (task.STATE != "准备就绪")
                {
                    throw new Exception("SyncDesay::SendData => Task state is not 准备就绪");
                }
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncDesay::SendData => database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT S.HANDLE,SEND_INFO,JSON_INFO FROM DESAY_SEND_INFO S INNER JOIN DESAY_TASK T ON T.HANDLE = '{0}' AND T.STATE = '准备就绪' AND S.HANDLE_TASK = T.HANDLE AND S.STATE IS NULL;", task.HANDLE), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    WebSOAP soap = new WebSOAP();
                    if (task.CUSTOMER == "COMMON" && task.VERSION == "2.6")
                    {
                        EntityLogin2 app = new EntityLogin2();
                        app.username = task.APPID;
                        app.password = task.APPKEY;

                        TokenDesay login = soap.Login_new(task.URI_LOGIN, app);
                        if (string.IsNullOrEmpty(login.result.token)) throw new Exception("SyncDesay::SendData => faild to common get token.");
                        task.TICKET = login.result.token;
                        //UpdateTask(task.HANDLE, "开始回传");
                        while (reader.Read())
                        {
                            string ret = null;
                            ResultPost2 result = soap.Post2(task.URI_POST, reader["JSON_INFO"].ToString(), task.TICKET, ref ret);
                            UpdateRecievedResult2(task.HANDLE, reader[0].ToString(), result, ret);
                            UpdateSendInfo(reader[0].ToString());
                        }
                    }
                    else
                    {
                        EntityLogin app = new EntityLogin();
                        app.app_id = task.APPID;
                        app.app_key = task.APPKEY;
                        ResultLogin login = soap.Login(task.URI_LOGIN, app);
                        task.TICKET = login.Ticket;
                        UpdateTask(task.HANDLE, "开始回传");
                        while (reader.Read())
                        {
                            StringBuilder contents = new StringBuilder();
                            contents.Append("{Module:");
                            contents.Append("\"");
                            contents.Append(task.MODULE);
                            contents.Append("\",Data:");
                            contents.Append("\"");
                            contents.Append(reader[1].ToString());
                            contents.Append("\",VerifyCode:\"");
                            contents.Append(Encryption.MD5_Encode(reader[1].ToString() + task.APPKEY));
                            contents.Append("\"}");
                            string ret = null;
                            ResultPost result = soap.Post(task.URI_POST, contents.ToString(), task.TICKET, ref ret);
                            UpdateRecievedResult(task.HANDLE, reader[0].ToString(), result, ret);
                            UpdateSendInfo(reader[0].ToString());
                        }
                    }
                    UpdateTask(task.HANDLE, "成功");
                    //Complete(ref task);
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                UpdateTask(task.HANDLE, "失败");
            }

        }
        public void UpdateTask(string taskID,string state)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncDesay::UpdateTask => Database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("UPDATE DESAY_TASK SET STATE = '{1}' WHERE HANDLE = '{0}'", taskID, state), conn);
                    comm.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
        }
        public void UpdateSendInfo(string handle)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncDesay::UpdateSendInfo => Database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO DESAY_SEND_LOG (HANDLE_SEND_INFO,HANDLE_TASK,SEND_INFO,JSON_INFO) SELECT HANDLE,HANDLE_TASK,SEND_INFO,JSON_INFO FROM DESAY_SEND_INFO WHERE HANDLE = '{0}';DELETE FROM DESAY_SEND_INFO WHERE HANDLE = '{0}'", handle), conn);
                    comm.ExecuteNonQuery();

                }
                catch(Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
        }
        public void UpdateRecievedResult(string taskID,string handle,ResultPost result,string ret)
        {
            try
            {
                if (string.IsNullOrEmpty(ret))
                {
                    throw new Exception("SyncDesay::UpdateRecievedResult => Result is null.");
                }
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncDesay::UpdateRecievedResult => SyncDesay database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO DESAY_RECIEVED_INFO (HANDLE_TASK,HANDLE_SEND_INFO,RECIEVED_INFO) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}','{2}') ;", taskID, handle, ret), conn);
                    string r_handle = comm.ExecuteScalar().ToString();
                    if (!result.Success)
                    {
                        SqlTransaction tran = conn.BeginTransaction();
                        comm.Transaction = tran;
                        foreach (ResultBattery battery in result.Data)
                        {
                            comm.CommandText = string.Format("INSERT INTO DESAY_RECIEVED_LOG (HANDLE_TASK,HANDLE_RECIEVED_INFO,SUCCESS,ERRORCODE,ERRORMESSAGE,BARCODE,RESULT,ERROR) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", taskID, r_handle, result.Success, result.ErrorCode, result.ErrorMessage, battery.barcode, battery.result, battery.error);
                            comm.ExecuteNonQuery();
                        }
                        tran.Commit();
                        UpdateTask(taskID, "失败");
                        throw new Exception("SyncDesay::UpdateRecievedResult => Received a failure message from the Desay system.");
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        /// <summary>
        /// 德赛新增，2023-11-20
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="handle"></param>
        /// <param name="result"></param>
        /// <param name="ret"></param>
        public void UpdateRecievedResult2(string taskID, string handle, ResultPost2 result, string ret)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncDesay::UpdateRecievedResult => SyncDesay database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO DESAY_RECIEVED_INFO (HANDLE_TASK,HANDLE_SEND_INFO,RECIEVED_INFO) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}','{2}') ;", taskID, handle, ret), conn);
                    string r_handle = comm.ExecuteScalar().ToString();
                    if (!result.success)
                    {
                        //SqlTransaction tran = conn.BeginTransaction();
                        //comm.Transaction = tran;
                        //foreach (ResultBattery battery in result.Data)
                        //{
                        //    comm.CommandText = string.Format("INSERT INTO DESAY_RECIEVED_LOG (HANDLE_TASK,HANDLE_RECIEVED_INFO,SUCCESS,ERRORCODE,ERRORMESSAGE,BARCODE,RESULT,ERROR) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", taskID, r_handle, result.Success, result.ErrorCode, result.ErrorMessage, battery.barcode, battery.result, battery.error);
                        //    comm.ExecuteNonQuery();
                        //}
                        //tran.Commit();
                        UpdateTask(taskID, "失败");
                        throw new Exception("SyncDesay::UpdateRecievedResult => Received a failure message from the Desay system.");
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }

        public void Complete(ref DesayTask task)
        {
            if (task.HANDLE == null)
            {
                return;
            }
            UpdateTask(task.HANDLE, "成功");
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    // 数据库未打开
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncDesay::CreateSendInfo => Desay database can not be open.");
                    }
                    // 任务未成功
                    SqlCommand comm = new SqlCommand(string.Format("SELECT COUNT(1) FROM DESAY_TASK WHERE STATE = '成功' AND HANDLE = '{0}'", task.HANDLE), conn);
                    if (comm.ExecuteScalar().ToString() != "1")
                    {
                        return;
                    }
                    comm.CommandText = string.Format("WITH A AS(SELECT T.HANDLE,T.STATE,T.CREATED_DATE_TIME,COUNT(L.HANDLE) C1 FROM DESAY_TASK T INNER JOIN DESAY_SEND_INFO I ON T.HANDLE = I.HANDLE_TASK INNER JOIN DESAY_SYNCBATTERY_LOG L ON L.HANDLE_SEND_INFO = I.HANDLE AND L.HANDLE_TASK = T.HANDLE GROUP BY  T.HANDLE,T.STATE,T.CREATED_DATE_TIME),B AS (SELECT T.HANDLE,T.STATE,T.CREATED_DATE_TIME,COUNT(L.HANDLE) C2 FROM DESAY_TASK T INNER JOIN DESAY_SEND_LOG L ON T.HANDLE = L.HANDLE_TASK INNER JOIN DESAY_SYNCBATTERY_LOG LL ON LL.HANDLE_SEND_INFO = L.HANDLE_SEND_INFO AND T.HANDLE = LL.HANDLE_TASK GROUP BY T.HANDLE,T.STATE,T.CREATED_DATE_TIME) SELECT B.HANDLE 任务编号,B.STATE 任务状态,B.CREATED_DATE_TIME 开始回传时间,A.C1 未发送数量,B.C2 已发送数量 FROM B LEFT JOIN A ON A.HANDLE = B.HANDLE WHERE B.HANDLE = '{0}' ORDER BY B.CREATED_DATE_TIME DESC", task.HANDLE);
                    SqlDataReader reader = comm.ExecuteReader();
                    TableWeb tWeb = new TableWeb();
                    tWeb.addThead("任务编号");
                    tWeb.addThead("任务状态");
                    tWeb.addThead("开始回传时间");
                    tWeb.addThead("失败数量");
                    tWeb.addThead("成功数量");
                    while (reader.Read())
                    {
                        tWeb.addContext(reader["任务编号"].ToString());
                        tWeb.addContext(reader["任务状态"].ToString());
                        tWeb.addContext(reader["开始回传时间"].ToString());
                        tWeb.addContext(reader["未发送数量"].ToString());
                        tWeb.addContext(reader["已发送数量"].ToString());
                    }
                    reader.Close();
                    comm.CommandText = string.Format("SELECT E.EMAIL FROM CUSTOMER C INNER JOIN EMAIL_GROUP G ON C.HANDLE = G.HANDLE_CUSTOMER AND C.CUSTOMER = 'DESAY-{0}' INNER JOIN EMAIL_CONFIG E ON G.HANDLE_EMAIL = E.HANDLE;", task.CUSTOMER);
                    reader = comm.ExecuteReader();
                    StringBuilder emailList = new StringBuilder();
                    while (reader.Read())
                    {
                        emailList.Append(reader[0].ToString());
                        emailList.Append(",");
                    }
                    reader.Close();
                    //发送邮件
                    Mail.SendMail(emailList.ToString(), System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], "DESAY 数据发送任务", tWeb.TableHtml());

                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
        }
    }
    /// <summary>
    /// Sunwoda 数据回传
    /// </summary>
    public class SyncSunwoda
    {
        /// <summary>
        /// 获取 SUNWODA BOM
        /// </summary>
        /// <param name="bomno"></param>
        /// <returns></returns>
        public Hashtable GetBom(string bomno = null)
        {
            Hashtable hashBom = new Hashtable();
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                if (conn.State != ConnectionState.Open) return null;
                string sql = null;
                if (!string.IsNullOrEmpty(bomno)) sql = $"SELECT BOMNO, HANDLE FROM SUNWODA_BOM WHERE STATE = 'Y' AND BOMNO = '{bomno}';";
                else sql = "SELECT BOMNO, HANDLE FROM SUNWODA_BOM WHERE STATE = 'Y';";
                SqlCommand comm = new SqlCommand(sql, conn);
                using (SqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hashBom.Add(reader["BOMNO"].ToString(), reader["HANDLE"].ToString());
                    }
                }
                return hashBom; 
            }
        }
        /// <summary>
        /// 创建回传任务
        /// </summary>
        /// <param name="productline">线体</param>
        /// <returns></returns>
        public SunwodaTask CreateTask(string handle)
        {
            if (string.IsNullOrEmpty(handle))
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncRemote database can not be open.");
                    }
                    //SqlCommand comm = new SqlCommand(string.Format("SELECT HANDLE FROM SUNWODA_CONFIG WHERE STATE = 'Y' AND PRODUCTLINE = '{0}' AND TYPE = '{1}';", productline, type), conn);
                    SqlCommand comm = new SqlCommand($"SELECT SC.HANDLE FROM SUNWODA_CONFIG SC INNER JOIN SUNWODA_BOM SB ON SC.HANDLE = SB.HANDLE_CONFIG AND SB.STATE = 'Y' AND SC.STATE = 'Y' WHERE SB.HANDLE = '{handle}'", conn);

                    string handle_config = comm.ExecuteScalar().ToString();
                    if (string.IsNullOrEmpty(handle_config))
                    {
                        throw new Exception("SyncSunwoda::CreateTask => No matching items found in the SUNWODA_CONFIG.");
                    }
                    comm.CommandText = string.Format(string.Format("INSERT INTO SUNWODA_TASK(HANDLE_CONFIG,EXPRESS_QTY,STATE) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}','未开始');", handle_config, "100"));
                    string _handle = comm.ExecuteScalar().ToString();
                    SunwodaTask task = new SunwodaTask();
                    task = GetTask(_handle);
                    if (!string.IsNullOrEmpty(task.URI_TOKEN))
                    {
                        string info = GetToken(task.URI_TOKEN, task.TOKEN_PARA);
                        TokenSunwoda result = JsonConvert.DeserializeObject<TokenSunwoda>(info);
                        if (result.statusCode == 200)
                        {
                            task.TOKEN = result.message;
                            comm.CommandText = $"UPDATE SUNWODA_CONFIG SET TOKEN = '{task.TOKEN}' WHERE HANDLE = '{handle_config}'";
                            comm.ExecuteNonQuery();
                        }
                    }
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取 Token
        /// </summary>
        /// <param name="uri">Token 地址</param>
        /// <param name="info">参数</param>
        /// <returns></returns>
        public string GetToken(string uri, string info)
        {
            if (string.IsNullOrEmpty(uri)) return null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncRemote database can not be open.");
                    }

                    WebSOAP soap = new WebSOAP();
                    string result = soap.Sunwoda_TokenQuerySoapWebService(uri, info);
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
        /// 获取任务对象
        /// </summary>
        /// <param name="handle">任务编号</param>
        /// <returns></returns>
        public SunwodaTask GetTask(string handle)
        {
            if (string.IsNullOrEmpty(handle))
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncRemote database can not be open.");
                    }
                    SunwodaTask task = new SunwodaTask();
                    SqlCommand comm = new SqlCommand(string.Format("SELECT T.HANDLE,T.EXPRESS_QTY,S.HANDLE AS HANDLE_CONFIG,DATATYPE,S.SUPPLIERID,S.APPKEY,S.SUPPLIERNAME,S.SUPPLIERCODE,S.PASSWORD,S.PRODUCTLINE,S.URI,S.TOKEN,URI_TOKEN,TOKEN_PARA,T.STATE,T.CREATED_DATE_TIME,T.EXPRESS_QTY FROM SUNWODA_TASK T INNER JOIN SUNWODA_CONFIG S ON T.HANDLE_CONFIG = S.HANDLE AND T.HANDLE = {0}; ", handle), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        task.HANDLE = reader["HANDLE"].ToString();
                        task.HANDLE_CONFIG = reader["HANDLE_CONFIG"].ToString();
                        task.APPKEY = reader["APPKEY"].ToString();
                        task.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                        task.DATATYPE = reader["DATATYPE"].ToString();
                        task.PASSWORD = reader["PASSWORD"].ToString();
                        task.PRODUCTLINE = reader["PRODUCTLINE"].ToString();
                        task.STATE = reader["STATE"].ToString();
                        task.SUPPLIERCODE = reader["SUPPLIERCODE"].ToString();
                        task.SUPPLIERID = reader["SUPPLIERID"].ToString();
                        task.SUPPLIERNAME = reader["SUPPLIERNAME"].ToString();
                        task.URI = reader["URI"].ToString();
                        task.EXPRESS_QTY = reader["EXPRESS_QTY"].ToString();
                        task.TOKEN = reader["TOKEN"].ToString();
                        task.URI_TOKEN = reader["URI_TOKEN"].ToString();
                        task.TOKEN_PARA = reader["TOKEN_PARA"].ToString();
                    }
                    reader.Close();
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 客户端上传至服务器
        /// </summary>
        /// <param name="batteryInfo">上传的json串</param>
        /// <param name="task">任务对象</param>
        /// <returns>成功数量</returns>
        public int CheckIn(string batteryInfo, SunwodaTask task)
        {
            if (string.IsNullOrEmpty(batteryInfo) || task == null)
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSunwoda::CheckIn => SyncRemote db can not be open.");
                    }
                    StringBuilder sql = new StringBuilder();
                    BatteryListSunwoda batteryList = new BatteryListSunwoda();
                    batteryList = JsonConvert.DeserializeObject<BatteryListSunwoda>(batteryInfo);
                    foreach (BatterySunwoda battery in batteryList.batterys)
                    {
                        sql.Append(string.Format("INSERT INTO SUNWODA_SYNCBATTERY (HANDLE_TASK,BARNAME,BARCODE,CELLSN,CAPACITY,CAPACITY_LOWERLIMIT,CAPACITY_UPPERLIMIT,CAPACITY_UNIT,KVALUE,KVALUE_LOWERLIMIT,KVALUE_UPPERLIMIT,KVALUE_UNIT,RESISTANCE,RESISTANCE_LOWERLIMIT,RESISTANCE_UPPERLIMIT,RESISTANCE_UNIT,VOLTAGE,VOLTAGE_LOWERLIMIT,VOLTAGE_UPPERLIMIT,VOLTAGE_UNIT,RESULT,REMARK,PRODUCTLINE,TESTTIME,LOTNO,SWDPN,PALLETSN,ASNSN) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}');", task.HANDLE, battery.projectName, battery.projectCode, battery.barcode, battery.capacity, battery.capacityLowerLimit, battery.capacityUpperLimit, battery.capacityUnit, battery.kvalue, battery.kvalueLowerLimit, battery.kvalueUpperLimit, battery.kvalueUnit, battery.resistance, battery.resistanceLowerLimit, battery.resistanceUpperLimit, battery.resistanceUnit, battery.voltage, battery.voltageLowerLimit, battery.voltageUpperLimit, battery.voltageUnit, battery.result, battery.remark, battery.productLine, battery.testtime, battery.lotno, battery.swdpn, battery.palletsn, battery.asnsn));
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    SqlTransaction tran = conn.BeginTransaction();
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
        /// <summary>
        /// 创建 Sunwoda_Send_Info
        /// </summary>
        /// <param name="task">任务名称</param>
        /// <returns></returns>
        public int SyncData(SunwodaTask task)
        {
            if (task == null)
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSunwoda::SyncData => SyncRemote db can not be open.");
                    }
                    StringBuilder barcodes = new StringBuilder();
                    SqlCommand comm = new SqlCommand(string.Format("SELECT DISTINCT TOP {1} CELLSN FROM SUNWODA_SYNCBATTERY S INNER JOIN SUNWODA_TASK T ON S.HANDLE_TASK = T.HANDLE AND T.HANDLE = '{0}' AND S.STATE IS NULL AND T.STATE = '开始回传';", task.HANDLE, task.EXPRESS_QTY), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    int count = 0;
                    while (reader.Read())
                    {
                        if (barcodes.Length != 0)
                        {
                            barcodes.Append(",");
                        }
                        barcodes.Append("'");
                        barcodes.Append(reader["CELLSN"]);
                        barcodes.Append("'");
                        ++count;
                    }
                    reader.Close();
                    // send_info 的 handle
                    string handle = null;
                    //UpdateTask(task.HANDLE, "数据准备");
                    // 创建send_info
                    CreateSendInfo(barcodes.ToString(), task, ref handle);
                    // 创建SyncBattery_Log
                    CreateSnycBatteryLog(barcodes.ToString(), handle);
                    return count; 
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    UpdateTask(task.HANDLE, "失败");
                    return 0;
                }
            }
        }
        /// <summary>
        /// 创建发送 json
        /// </summary>
        /// <param name="barcodes">码号</param>
        /// <param name="handle">send_info 的 handle</param>
        /// <returns></returns>
        public string CreateSendInfo(string barcodes, SunwodaTask task, ref string handle)
        {
            if (string.IsNullOrEmpty(barcodes) || string.IsNullOrEmpty(task.HANDLE))
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSunwoda::CreateSendInfo => SyncSunwoda db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT HANDLE,HANDLE_TASK,BARNAME,BARCODE,CELLSN,PRODUCTLINE,CONVERT(NVARCHAR(50),TESTTIME,120) TESTTIME,CAPACITY,CAPACITY_LOWERLIMIT,CAPACITY_UPPERLIMIT,CAPACITY_UNIT,KVALUE,KVALUE_LOWERLIMIT,KVALUE_UPPERLIMIT,KVALUE_UNIT,RESISTANCE,RESISTANCE_LOWERLIMIT,RESISTANCE_UPPERLIMIT,RESISTANCE_UNIT,VOLTAGE,VOLTAGE_LOWERLIMIT,VOLTAGE_UPPERLIMIT,VOLTAGE_UNIT,RESULT,REMARK,STATE,CREATED_DATE_TIME,LOTNO,SWDPN,PALLETSN,ASNSN FROM SUNWODA_SYNCBATTERY WHERE HANDLE_TASK = '{0}' AND CELLSN IN ({1});", task.HANDLE, barcodes), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    SCUD_OcvData SunwodaData = new SCUD_OcvData();
                    SunwodaData.appKey = task.APPKEY;
                    SunwodaData.dataType = task.DATATYPE;
                    SunwodaData.passWord = task.PASSWORD;
                    SunwodaData.supplierCode = task.SUPPLIERCODE;
                    SunwodaData.supplierId = task.SUPPLIERID;
                    SunwodaData.supplierName = task.SUPPLIERNAME;

                    while (reader.Read())
                    {
                        SCUD_cellOcvData CellData = new SCUD_cellOcvData();
                        CellData.barName = reader["BARNAME"].ToString();
                        CellData.barCode = reader["BARCODE"].ToString();
                        CellData.cellSn = reader["CELLSN"].ToString();
                        CellData.productLine = reader["PRODUCTLINE"].ToString();
                        CellData.asnSn = reader["ASNSN"].ToString();
                        CellData.testTime = reader["TESTTIME"].ToString();
                        // OCV 测试项目
                        SCUD_testItems TestItems = new SCUD_testItems();
                        TestItems.testItem = "OCV";
                        TestItems.lowerLimit = double.Parse(!string.IsNullOrEmpty(reader["VOLTAGE_LOWERLIMIT"].ToString()) ? reader["VOLTAGE_LOWERLIMIT"].ToString() : "0");
                        TestItems.upperLimit = double.Parse(!string.IsNullOrEmpty(reader["VOLTAGE_UPPERLIMIT"].ToString()) ? reader["VOLTAGE_UPPERLIMIT"].ToString() : "0");
                        TestItems.unit = reader["VOLTAGE_UNIT"].ToString();
                        TestItems.testValue = double.Parse(!string.IsNullOrEmpty(reader["VOLTAGE"].ToString()) ? reader["VOLTAGE"].ToString() : "0");
                        if (!string.IsNullOrEmpty(task.TOKEN)) TestItems.testResult = "OK";
                        else TestItems.testResult = reader["RESULT"].ToString();
                        TestItems.remark = reader["REMARK"].ToString();
                        TestItems.swdPn = reader["SWDPN"].ToString();
                        TestItems.palletSn = reader["PALLETSN"].ToString();
                        // 测试项目加入电池
                        CellData.testItemList.Add(TestItems);

                        // IMP 测试项目（消费类产品内阻称为：IMP）
                        SCUD_testItems TestItems2 = new SCUD_testItems();
                        TestItems2.testItem = "IMP";
                        TestItems2.lowerLimit = double.Parse(!string.IsNullOrEmpty(reader["RESISTANCE_LOWERLIMIT"].ToString()) ? reader["RESISTANCE_LOWERLIMIT"].ToString() : "0");
                        TestItems2.upperLimit = double.Parse(!string.IsNullOrEmpty(reader["RESISTANCE_UPPERLIMIT"].ToString()) ? reader["RESISTANCE_UPPERLIMIT"].ToString() : "0");
                        TestItems2.unit = reader["RESISTANCE_UNIT"].ToString();
                        TestItems2.testValue = double.Parse(!string.IsNullOrEmpty(reader["RESISTANCE"].ToString()) ? reader["RESISTANCE"].ToString() : "0");
                        if (!string.IsNullOrEmpty(task.TOKEN)) TestItems2.testResult = "OK";
                        else TestItems2.testResult = reader["RESULT"].ToString();
                        TestItems2.remark = reader["REMARK"].ToString();
                        TestItems2.swdPn = reader["SWDPN"].ToString();
                        TestItems2.palletSn = reader["PALLETSN"].ToString();

                        // 测试项目加入电池
                        CellData.testItemList.Add(TestItems2);
                        // IR 测试项目（动力类产品内阻称为：IR）
                        SCUD_testItems TestItems22 = new SCUD_testItems();
                        TestItems22.testItem = "IR";
                        TestItems22.lowerLimit = double.Parse(!string.IsNullOrEmpty(reader["RESISTANCE_LOWERLIMIT"].ToString()) ? reader["RESISTANCE_LOWERLIMIT"].ToString() : "0");
                        TestItems22.upperLimit = double.Parse(reader["RESISTANCE_UPPERLIMIT"].ToString());
                        TestItems22.unit = reader["RESISTANCE_UNIT"].ToString();
                        TestItems22.testValue = double.Parse(!string.IsNullOrEmpty(reader["RESISTANCE"].ToString()) ? reader["RESISTANCE"].ToString() : "0");
                        if (!string.IsNullOrEmpty(task.TOKEN)) TestItems22.testResult = "OK";
                        else TestItems22.testResult = reader["RESULT"].ToString();
                        TestItems22.remark = reader["REMARK"].ToString();
                        TestItems22.swdPn = reader["SWDPN"].ToString();
                        TestItems22.palletSn = reader["PALLETSN"].ToString();

                        // 测试项目加入电池
                        CellData.testItemList.Add(TestItems22);


                        // KValue 测试项目
                        SCUD_testItems TestItems3 = new SCUD_testItems();
                        TestItems3.testItem = "K";
                        TestItems3.lowerLimit = double.Parse(!string.IsNullOrEmpty(reader["KVALUE_LOWERLIMIT"].ToString()) ? reader["KVALUE_LOWERLIMIT"].ToString() : "0");
                        TestItems3.upperLimit = double.Parse(!string.IsNullOrEmpty(reader["KVALUE_UPPERLIMIT"].ToString()) ? reader["KVALUE_UPPERLIMIT"].ToString() : "0");
                        TestItems3.unit = reader["KVALUE_UNIT"].ToString();
                        TestItems3.testValue = double.Parse(!string.IsNullOrEmpty(reader["KVALUE"].ToString()) ? reader["KVALUE"].ToString() : "0");
                        if (!string.IsNullOrEmpty(task.TOKEN)) TestItems3.testResult = "OK";
                        else TestItems3.testResult = reader["RESULT"].ToString();
                        TestItems3.remark = reader["REMARK"].ToString();
                        TestItems3.swdPn = reader["SWDPN"].ToString();
                        TestItems3.palletSn = reader["PALLETSN"].ToString();

                        // 测试项目加入电池
                        CellData.testItemList.Add(TestItems3);

                        // CAPACITY 测试项目（消费类容量称为：CAPAC）
                        SCUD_testItems TestItems4 = new SCUD_testItems();
                        TestItems4.testItem = "CAPAC";
                        TestItems4.lowerLimit = double.Parse(!string.IsNullOrEmpty(reader["CAPACITY_LOWERLIMIT"].ToString()) ? reader["CAPACITY_LOWERLIMIT"].ToString() : "0");
                        TestItems4.upperLimit = double.Parse(!string.IsNullOrEmpty(reader["CAPACITY_UPPERLIMIT"].ToString()) ? reader["CAPACITY_UPPERLIMIT"].ToString() : "0");
                        TestItems4.unit = reader["CAPACITY_UNIT"].ToString();
                        TestItems4.testValue = double.Parse(!string.IsNullOrEmpty(reader["CAPACITY"].ToString()) ? reader["CAPACITY"].ToString() : "0");
                        if (!string.IsNullOrEmpty(task.TOKEN)) TestItems4.testResult = "OK";
                        else TestItems4.testResult = reader["RESULT"].ToString();
                        TestItems4.remark = reader["REMARK"].ToString();
                        TestItems4.swdPn = reader["SWDPN"].ToString();
                        TestItems4.palletSn = reader["PALLETSN"].ToString();

                        // 测试项目加入电池
                        CellData.testItemList.Add(TestItems4);
                        // CAPACITY 测试项目（动力类容量称为：Capacity）
                        SCUD_testItems TestItems44 = new SCUD_testItems();
                        TestItems44.testItem = "Capacity";
                        TestItems44.lowerLimit = double.Parse(!string.IsNullOrEmpty(reader["CAPACITY_LOWERLIMIT"].ToString()) ? reader["CAPACITY_LOWERLIMIT"].ToString() : "0");
                        TestItems44.upperLimit = double.Parse(!string.IsNullOrEmpty(reader["CAPACITY_UPPERLIMIT"].ToString()) ? reader["CAPACITY_UPPERLIMIT"].ToString() : "0");
                        TestItems44.unit = reader["CAPACITY_UNIT"].ToString();
                        TestItems44.testValue = double.Parse(!string.IsNullOrEmpty(reader["CAPACITY"].ToString()) ? reader["CAPACITY"].ToString() : "0");
                        if (!string.IsNullOrEmpty(task.TOKEN)) TestItems44.testResult = "OK";
                        else TestItems44.testResult = reader["RESULT"].ToString();
                        TestItems44.remark = reader["REMARK"].ToString();
                        TestItems44.swdPn = reader["SWDPN"].ToString();
                        TestItems44.palletSn = reader["PALLETSN"].ToString();

                        // 测试项目加入电池
                        CellData.testItemList.Add(TestItems44);

                        // LOTNO 测试项目
                        SCUD_testItems TestItems5 = new SCUD_testItems();
                        TestItems5.testItem = "LOTNO";
                        TestItems5.lowerLimit = 0;
                        TestItems5.upperLimit = 0;
                        TestItems5.unit = "0";
                        TestItems5.testValue = 0;
                        if (!string.IsNullOrEmpty(task.TOKEN)) TestItems5.testResult = "OK";
                        else TestItems5.testResult = "0";
                        TestItems5.remark = reader["LOTNO"].ToString();
                        TestItems5.swdPn = reader["SWDPN"].ToString();
                        TestItems5.palletSn = reader["PALLETSN"].ToString();

                        // 测试项目加入电池
                        CellData.testItemList.Add(TestItems5);

                        // CELL_GROUP 项目
                        SCUD_testItems TestItems6 = new SCUD_testItems();
                        if (!string.IsNullOrEmpty(task.TOKEN))
                        {
                            TestItems6.testItem = "CELL_GROUP";
                            TestItems6.testResult = "OK";
                        }
                        else
                        {
                            TestItems6.testItem = "CELLGROUP";
                            TestItems6.testResult = "0";
                        }
                        TestItems6.lowerLimit = 0;
                        TestItems6.upperLimit = 0;
                        TestItems6.unit = "0";
                        TestItems6.testValue = 0;
                        TestItems6.remark = reader["RESULT"].ToString();
                        TestItems6.swdPn = reader["SWDPN"].ToString();
                        TestItems6.palletSn = reader["PALLETSN"].ToString();

                        // 测试项目加入电池
                        CellData.testItemList.Add(TestItems6);

                        // 电池加入回传数组
                        SunwodaData.cellOcvDataList.Add(CellData);

                    }
                    // 生成json字符串
                    string json = JsonConvert.SerializeObject(SunwodaData);
                    reader.Close();
                    comm.CommandText = string.Format("INSERT INTO SUNWODA_SEND_INFO (HANDLE_TASK,SEND_INFO) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}');", task.HANDLE, json);
                    handle = comm.ExecuteScalar().ToString();
                    if (!string.IsNullOrEmpty(handle))
                    {
                        return handle;
                    }
                    else
                    {
                        throw new Exception("SyncSunwoda::CreateSendInfo => Fail to create Sunwoda_send_info.");
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog("SyncSunwoda::CreateSendInfo => " + ex.Message);
                    handle = null;
                    return handle;
                }
            }
        }
        
        /// <summary>
        /// 根据码号、send_info的handl更新syncbattery
        /// </summary>
        /// <param name="barcodes">码号</param>
        /// <param name="handle">send_info的handl</param>
        /// <returns></returns>
        public int CreateSnycBatteryLog(string barcodes, string handle)
        {
            if (string.IsNullOrEmpty(barcodes) || string.IsNullOrEmpty(handle))
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSunwoda::CreateSnycBatteryLog => SyncSunwoda db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO SUNWODA_SYNCBATTERY_LOG (HANDLE_SEND_INFO,HANDLE_TASK,BARNAME,BARCODE,CELLSN,CAPACITY,CAPACITY_LOWERLIMIT,CAPACITY_UPPERLIMIT,CAPACITY_UNIT,KVALUE,KVALUE_LOWERLIMIT,KVALUE_UPPERLIMIT,KVALUE_UNIT,RESISTANCE,RESISTANCE_LOWERLIMIT,RESISTANCE_UPPERLIMIT,RESISTANCE_UNIT,VOLTAGE,VOLTAGE_LOWERLIMIT,VOLTAGE_UPPERLIMIT,VOLTAGE_UNIT,RESULT,REMARK,STATE,PRODUCTLINE,TESTTIME,LOTNO,SWDPN,ASNSN) SELECT '{0}',HANDLE_TASK,BARNAME,BARCODE,CELLSN,CAPACITY,CAPACITY_LOWERLIMIT,CAPACITY_UPPERLIMIT,CAPACITY_UNIT,KVALUE,KVALUE_LOWERLIMIT,KVALUE_UPPERLIMIT,KVALUE_UNIT,RESISTANCE,RESISTANCE_LOWERLIMIT,RESISTANCE_UPPERLIMIT,RESISTANCE_UNIT,VOLTAGE,VOLTAGE_LOWERLIMIT,VOLTAGE_UPPERLIMIT,VOLTAGE_UNIT,RESULT,REMARK,STATE,PRODUCTLINE,TESTTIME,LOTNO,SWDPN,ASNSN FROM SUNWODA_SYNCBATTERY WHERE CELLSN IN ({1});DELETE FROM SUNWODA_SYNCBATTERY WHERE CELLSN IN ({1});", handle, barcodes), conn);
                    return comm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog("SyncSunwoda::CreateSnycBatteryLog => " + ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="handle">任务 handle</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public bool UpdateTask(string handle, string state)
        {
            if (string.IsNullOrEmpty(handle) || string.IsNullOrEmpty(state))
            {
                return false;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSunwoda::UpdateTask => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("UPDATE SUNWODA_TASK SET STATE = '{1}' WHERE HANDLE = '{0}';", handle, state), conn);
                    if (comm.ExecuteNonQuery() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("SyncSunwoda::UpdateTask => Fail to update Sunwoda_Task.");
                    }
                }
                catch (Exception ex)
                {
                    SysLog _log = new SysLog(ex.Message);
                    return false;
                }
            }
        }
        /// <summary>
        /// 数据发送
        /// </summary>
        /// <param name="task">任务</param>
        /// <returns></returns>
        public int SendData(SunwodaTask task)
        {
            if (task == null)
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                int count = 0;
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSunwoda::SendData => database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT S.HANDLE,S.SEND_INFO FROM SUNWODA_SEND_INFO S INNER JOIN SUNWODA_TASK T ON S.HANDLE_TASK = T.HANDLE AND S.STATE IS NULL AND T.STATE = '开始回传' AND T.HANDLE = '{0}'", task.HANDLE), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    WebSOAP soap = new WebSOAP();
                    string result = null;
                    while (reader.Read())
                    {
                        result = soap.Sunwoda_QuerySoapWebService(task.URI, reader["SEND_INFO"].ToString(), 0, task.TOKEN);
                        if (Recieved(task, reader["HANDLE"].ToString(),result))
                        {
                            CreateSendDataLog(task, reader["HANDLE"].ToString());
                            ++count;
                        }
                        else
                        {
                            throw new Exception("SyncSunwoda::SendData => Fail to data upload.");
                        }
                    }
                    return count;
                }
                catch (Exception ex)
                {
                    UpdateTask(task.HANDLE, "失败");
                    SysLog _log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 创建SendDataLog，并清理SendDataInfo
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="handleSendInfo">SendDataInfo HANDLE</param>
        /// <returns></returns>
        public int CreateSendDataLog(SunwodaTask task,string handleSendInfo)
        {
            if (task == null || string.IsNullOrEmpty(handleSendInfo))
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSunwoda::CreateSendDataLog => database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO SUNWODA_SEND_LOG (HANDLE_SEND_INFO,HANDLE_TASK,SEND_INFO) SELECT HANDLE,HANDLE_TASK,SEND_INFO FROM SUNWODA_SEND_INFO WHERE HANDLE = '{0}';DELETE FROM SUNWODA_SEND_INFO WHERE HANDLE = '{0}';", handleSendInfo), conn);
                    return comm.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 接收返回结果
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="handleSendInfo">发送 JSON 的 HANDLE</param>
        /// <param name="str">对方系统返回的信息</param>
        /// <returns></returns>
        public bool Recieved(SunwodaTask task,string handleSendInfo,string str)
        {
            string sql = null, sql2 = null;
            string handle = null;
            SCUD_responseResult result = null;
            SUNWODA_responseResult result2 = null;
            try
            {
                if (string.IsNullOrEmpty(task.TOKEN)) result = JsonConvert.DeserializeObject<SCUD_responseResult>(str);
                else result2 = JsonConvert.DeserializeObject<SUNWODA_responseResult>(str);
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSunwoda::Recieved => database can not be open.");
                    }
                    sql = string.Format("INSERT INTO SUNWODA_RECIEVED_INFO (HANDLE_TASK,HANDLE_SEND_INFO,RECIEVED_INFO) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}','{2}');", task.HANDLE, handleSendInfo, str);
                    SqlCommand comm = new SqlCommand(sql, conn);
                    handle = comm.ExecuteScalar().ToString();
                    if (string.IsNullOrEmpty(handle))
                    {
                        throw new Exception("SyncSunwoda::Recieved => Failed to Insert SUNWODA_RECIEVED_Info information");
                    }
                    if (string.IsNullOrEmpty(task.TOKEN))
                        sql2 = string.Format("INSERT INTO SUNWODA_RECIEVED_LOG (HANDLE_TASK,HANDLE_RECIEVED_INFO,errorMsg,successMsg,status,remark,data,date,year) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", task.HANDLE, handleSendInfo, result.errorMsg, result.successMsg, result.status, result.remark, result.data, result.date, result.year);
                    else
                        sql2 = $"INSERT INTO SUNWODA_RECIEVED_LOG(HANDLE_TASK,HANDLE_RECIEVED_INFO) VALUES ('{task.HANDLE}','{handleSendInfo}');";
                    comm.CommandText = sql2;
                    if (comm.ExecuteNonQuery() == 1)
                    {
                        if (string.IsNullOrEmpty(task.TOKEN))
                        {
                            if (result.status == "true") return true;
                            else return false;
                        }
                        else
                        {
                            if (result2.statusCode == 200) return true;
                            else return false;
                        }
                    }
                    else
                    {
                        throw new Exception("SyncSunwoda::Recieved => Failed to Insert SUNWODA_RECIEVED_Log information");
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 任务准备就绪
        /// </summary>
        /// <param name="taskID">任务编号</param>
        /// <returns></returns>
        public int Ready(string taskID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    //打开连接
                    conn.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("UPDATE SUNWODA_TASK SET STATE = '准备就绪' WHERE HANDLE = '{0}';", taskID);
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 更新任务状态并邮件推送
        /// </summary>
        /// <param name="task">任务</param>
        public void Complete(SunwodaTask task)
        {
            if (task == null)
            {
                return;
            }
            UpdateTask(task.HANDLE, "完成");
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSunwoda::Complete => database can not be open.");
                    }
                    //SqlCommand comm = new SqlCommand(string.Format("WITH A AS (SELECT T.HANDLE,T.STATE,T.CREATED_DATE_TIME,COUNT(L.HANDLE) C1	FROM SUNWODA_TASK T	INNER JOIN SUNWODA_SEND_INFO I ON T.HANDLE = I.HANDLE_TASK INNER JOIN SUNWODA_SYNCBATTERY_LOG L ON L.HANDLE_SEND_INFO = I.HANDLE AND L.HANDLE_TASK = T.HANDLE GROUP BY T.HANDLE,T.STATE,T.CREATED_DATE_TIME),B AS (SELECT T.HANDLE,T.STATE,T.CREATED_DATE_TIME,COUNT(L.HANDLE) C2 FROM SUNWODA_TASK T INNER JOIN SUNWODA_SEND_LOG L ON T.HANDLE = L.HANDLE_TASK INNER JOIN SUNWODA_SYNCBATTERY_LOG LL ON LL.HANDLE_SEND_INFO = L.HANDLE_SEND_INFO AND T.HANDLE = LL.HANDLE_TASK GROUP BY T.HANDLE,T.STATE,T.CREATED_DATE_TIME) SELECT T.HANDLE 任务编号,B.HANDLE 回传序号,T.ITEM_NO 物料编号,I.BOMNO 型号,B.STATE 任务状态,B.CREATED_DATE_TIME 开始回传时间,A.C1 未发送量,B.C2 已发送数量 FROM B LEFT JOIN A ON A.HANDLE = B.HANDLE LEFT JOIN TASK T ON B.HANDLE = T.HANDLE_TASK INNER JOIN ITEM I ON T.ITEM_NO = I.ITEM WHERE B.HANDLE = '{0}' ORDER BY B.CREATED_DATE_TIME DESC;", task.HANDLE), conn);
                    SqlCommand comm = new SqlCommand(string.Format("WITH A AS(SELECT T.HANDLE,T.STATE,T.CREATED_DATE_TIME,COUNT(L.HANDLE) C1 FROM SUNWODA_TASK T INNER JOIN SUNWODA_SEND_INFO I ON T.HANDLE = I.HANDLE_TASK INNER JOIN SUNWODA_SYNCBATTERY_LOG L ON L.HANDLE_SEND_INFO = I.HANDLE AND L.HANDLE_TASK = T.HANDLE GROUP BY  T.HANDLE,T.STATE,T.CREATED_DATE_TIME),B AS (SELECT T.HANDLE,T.STATE,T.CREATED_DATE_TIME,COUNT(L.HANDLE) C2 FROM SUNWODA_TASK T INNER JOIN SUNWODA_SEND_LOG L ON T.HANDLE = L.HANDLE_TASK INNER JOIN SUNWODA_SYNCBATTERY_LOG LL ON LL.HANDLE_SEND_INFO = L.HANDLE_SEND_INFO AND T.HANDLE = LL.HANDLE_TASK GROUP BY T.HANDLE,T.STATE,T.CREATED_DATE_TIME) SELECT B.HANDLE 任务编号,B.STATE 任务状态,B.CREATED_DATE_TIME 开始回传时间,A.C1 未发送数量,B.C2 已发送数量 FROM B LEFT JOIN A ON A.HANDLE = B.HANDLE WHERE B.HANDLE = '{0}' ORDER BY B.CREATED_DATE_TIME DESC;", task.HANDLE), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    TableWeb tWeb = new TableWeb();
                    tWeb.addThead("任务编号");
                    //tWeb.addThead("执行顺序");
                    //tWeb.addThead("物料编号");
                    //tWeb.addThead("产品型号");
                    tWeb.addThead("任务状态");
                    tWeb.addThead("开始回传时间");
                    tWeb.addThead("失败数量");
                    tWeb.addThead("成功数量");
                    while (reader.Read())
                    {
                        tWeb.addContext(reader["任务编号"].ToString());
                        //tWeb.addContext(reader["执行顺序"].ToString());
                        //tWeb.addContext(reader["物料编号"].ToString());
                        //tWeb.addContext(reader["产品型号"].ToString());
                        tWeb.addContext(reader["任务状态"].ToString());
                        tWeb.addContext(reader["开始回传时间"].ToString());
                        tWeb.addContext(reader["未发送数量"].ToString());
                        tWeb.addContext(reader["已发送数量"].ToString());
                    }
                    reader.Close();
                    comm.CommandText = string.Format("SELECT E.EMAIL FROM CUSTOMER C INNER JOIN EMAIL_GROUP G ON C.HANDLE = G.HANDLE_CUSTOMER AND C.CUSTOMER = 'SUNWODA' INNER JOIN EMAIL_CONFIG E ON G.HANDLE_EMAIL = E.HANDLE;");
                    reader = comm.ExecuteReader();
                    StringBuilder emailList = new StringBuilder();
                    while (reader.Read())
                    {
                        emailList.Append(reader[0].ToString());
                        emailList.Append(",");
                    }
                    reader.Close();
                    //发送邮件
                    Mail.SendMail(emailList.ToString(), System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], "SUNWODA 数据发送任务", tWeb.TableHtml());
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return;
                }                
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="dt">日期范围</param>
        /// <param name="taskID">栈板号</param>
        /// <param name="barcode">码号</param>
        /// <returns></returns>
        public DataTable Search(string dt, string taskID, string barcode)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSunwoda::Search => database can not be open.");
                    }
                    string sql = null;
                    if (!string.IsNullOrEmpty(barcode))
                    {
                        sql = string.Format("SELECT T.HANDLE,T.STATE,T.CREATED_DATE_TIME,L.CELLSN,I.RECIEVED_INFO,I.RECIEVED_DATE_TIME FROM SUNWODA_SYNCBATTERY_LOG L INNER JOIN SUNWODA_RECIEVED_INFO I ON L.HANDLE_SEND_INFO = I.HANDLE_SEND_INFO INNER JOIN SUNWODA_TASK T ON L.HANDLE_TASK = T.HANDLE WHERE L.CELLSN = '{0}';", barcode);
                    }
                    else if (!string.IsNullOrEmpty(taskID))
                    {
                        sql = string.Format("WITH A AS (SELECT T.HANDLE,COUNT(RI.HANDLE) C FROM SUNWODA_TASK T INNER JOIN SUNWODA_RECIEVED_INFO RI ON RI.HANDLE_TASK = T.HANDLE INNER JOIN SUNWODA_SYNCBATTERY_LOG SL ON RI.HANDLE_SEND_INFO = SL.HANDLE_SEND_INFO GROUP BY T.HANDLE,RI.HANDLE),B AS (SELECT T.HANDLE,COUNT(S.HANDLE) C FROM SUNWODA_TASK T INNER JOIN SUNWODA_SYNCBATTERY S ON T.HANDLE = S.HANDLE_TASK	GROUP BY T.HANDLE,S.HANDLE),C AS (SELECT A.HANDLE,SUM(A.C) C2 FROM A GROUP BY A.HANDLE),D AS (SELECT B.HANDLE,SUM(B.C) C2 FROM B GROUP BY B.HANDLE) SELECT T.HANDLE 任务编号,T.STATE 任务状态,T.CREATED_DATE_TIME 任务启动时间,D.C2 未完成数量,C.C2 完成数量 FROM SUNWODA_TASK T INNER JOIN C ON T.HANDLE = C.HANDLE LEFT JOIN D ON T.HANDLE = D.HANDLE WHERE T.HANDLE = '{0}' ORDER BY T.CREATED_DATE_TIME DESC;", taskID);
                    }
                    else if (string.IsNullOrEmpty(taskID) && string.IsNullOrEmpty(barcode))
                    {
                        sql = string.Format("WITH A AS (SELECT T.HANDLE,COUNT(RI.HANDLE) C FROM SUNWODA_TASK T INNER JOIN SUNWODA_RECIEVED_INFO RI ON RI.HANDLE_TASK = T.HANDLE INNER JOIN SUNWODA_SYNCBATTERY_LOG SL ON RI.HANDLE_SEND_INFO = SL.HANDLE_SEND_INFO GROUP BY T.HANDLE,RI.HANDLE),B AS (SELECT T.HANDLE,COUNT(S.HANDLE) C FROM SUNWODA_TASK T INNER JOIN SUNWODA_SYNCBATTERY S ON T.HANDLE = S.HANDLE_TASK	GROUP BY T.HANDLE,S.HANDLE),C AS (SELECT A.HANDLE,SUM(A.C) C2 FROM A GROUP BY A.HANDLE),D AS (SELECT B.HANDLE,SUM(B.C) C2 FROM B GROUP BY B.HANDLE) SELECT T.HANDLE 任务编号,T.STATE 任务状态,T.CREATED_DATE_TIME 任务启动时间,D.C2 未完成数量,C.C2 完成数量 FROM SUNWODA_TASK T INNER JOIN C ON T.HANDLE = C.HANDLE LEFT JOIN D ON T.HANDLE = D.HANDLE ORDER BY T.CREATED_DATE_TIME DESC;");
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
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
    }
    /// <summary>
    /// 飞毛腿（SCUD）数据回传
    /// </summary>
    public class SyncSCUD
    {
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="bomno">产品型号</param>
        /// <param name="item_no">产品料号</param>
        /// <returns></returns>
        public SCUDTask CreateTask(string bomno, string item_no)
        {
            if (string.IsNullOrEmpty(bomno) && string.IsNullOrEmpty(item_no))
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::CreateTask => SyncRemote database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT * FROM SCUD_CONFIG WHERE STATE = 'Y' AND BOMNO = '{0}' AND ITEM_NO = '{1}';", bomno, item_no), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception("SCUD_CONFIG data does not exist.");
                    }
                    SCUDTask task = new SCUDTask();
                    reader.Read();
                    task.HANDLE_CONFIG = reader["HANDLE"].ToString();
                    task.APPID = reader["APPID"].ToString();
                    task.PWD = reader["PWD"].ToString();
                    task.DBTYPE = reader["DBTYPE"].ToString();
                    task.TYPE = reader["TYPE"].ToString();
                    task.URI = reader["URI"].ToString();
                    task.BOMNO = reader["BOMNO"].ToString();
                    task.ITEM_NO = reader["ITEM_NO"].ToString();
                    task.DEPT = reader["DEPT"].ToString();
                    task.CO_ITEM_CODE = reader["CO_ITEM_CODE"].ToString();
                    task.CO_ITEM_NAME = reader["CO_ITEM_NAME"].ToString();
                    task.CO_ITEM_SPEC = reader["CO_ITEM_SPEC"].ToString();
                    task.STATE = "已创建";
                    reader.Close();
                    comm.CommandText = string.Format(string.Format("INSERT INTO SCUD_TASK(HANDLE_CONFIG,STATE) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}');", task.HANDLE_CONFIG, task.STATE));
                    string handle = comm.ExecuteScalar().ToString();
                    task.HANDLE = handle;
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取任务对象
        /// </summary>
        /// <param name="handle">任务 handle</param>
        /// <returns></returns>
        public SCUDTask GetTask(string handle)
        {
            if (string.IsNullOrEmpty(handle))
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::GetTask => SyncRemote database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT T.HANDLE, C.HANDLE HANDLE_CONFIG,C.APPID,C.PWD,C.DBTYPE,C.TYPE,C.URI,C.BOMNO,C.ITEM_NO,C.DEPT,C.CO_ITEM_CODE,C.CO_ITEM_NAME,C.CO_ITEM_SPEC,T.COMPLETED,T.TOTAL,T.STATE FROM SCUD_TASK T INNER JOIN SCUD_CONFIG C ON T.HANDLE_CONFIG = C.HANDLE WHERE T.HANDLE = '{0}';", handle), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    SCUDTask task = new SCUDTask();
                    reader.Read();
                    task.HANDLE = reader["HANDLE"].ToString();
                    task.HANDLE_CONFIG = reader["HANDLE_CONFIG"].ToString();
                    task.APPID = reader["APPID"].ToString();
                    task.PWD = reader["PWD"].ToString();
                    task.DBTYPE = reader["DBTYPE"].ToString();
                    task.TYPE = reader["TYPE"].ToString();
                    task.URI = reader["URI"].ToString();
                    task.BOMNO = reader["BOMNO"].ToString();
                    task.ITEM_NO = reader["ITEM_NO"].ToString();
                    task.DEPT = reader["DEPT"].ToString();
                    task.CO_ITEM_CODE = reader["CO_ITEM_CODE"].ToString();
                    task.CO_ITEM_NAME = reader["CO_ITEM_NAME"].ToString();
                    task.CO_ITEM_SPEC = reader["CO_ITEM_SPEC"].ToString();
                    task.COMPLETED = string.IsNullOrEmpty(reader["COMPLETED"].ToString()) ? 0 : Convert.ToInt32(reader["COMPLETED"].ToString());
                    task.TOTAL = string.IsNullOrEmpty(reader["COMPLETED"].ToString()) ? 0 : Convert.ToInt32(reader["TOTAL"].ToString());
                    task.STATE = reader["STATE"].ToString();
                    reader.Close();
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 导入充电宝数据
        /// </summary>
        /// <param name="batteryInfo">充电宝对象</param>
        /// <param name="task">任务对象</param>
        /// <returns>成功导入数量</returns>
        public int CheckInByPowerBank(PowerBankSCUD batteryInfo, SCUDTask task)
        {
            try
            {
                if (batteryInfo.data.Count == 0 || task == null)
                {
                    throw new Exception("SyncSCUD::CheckInByPowerBank => No data is available for import or the task does not exist");
                }
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::CheckInByPowerBank => SyncRemote database can not be open.");
                    }
                    StringBuilder sql = new StringBuilder();
                    for (int i = 0; i < batteryInfo.data.Count; ++i)
                    {
                        sql.Append(string.Format("INSERT INTO SCUD_SYNCBATTERY (HANDLE_TASK,cellSn,voltageValue,testDate,PB_SN,CO_ITEM_CODE,CO_ITEM_NAME,CO_ITEM_SPEC,PN_SN,GEAR,IRValue) VALUES ('{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');", task.HANDLE, batteryInfo.data[i].cellSn, batteryInfo.data[i].voltageValue, batteryInfo.data[i].testDate, batteryInfo.data[i].PB_SN, batteryInfo.data[i].CO_ITEM_CODE, batteryInfo.data[i].CO_ITEM_NAME, batteryInfo.data[i].CO_ITEM_SPEC, batteryInfo.data[i].PN_SN, batteryInfo.data[i].GEAR, batteryInfo.data[i].IRValue));
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    int rowAffect = 0;
                    try
                    {
                        rowAffect = comm.ExecuteNonQuery();
                        tran.Commit();
                        comm.CommandText = string.Format("UPDATE SCUD_TASK SET TOTAL = '{1}' WHERE HANDLE = '{0}';", task.HANDLE, rowAffect.ToString());
                        comm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        SysLog log = new SysLog(ex.Message);
                    }
                    return rowAffect;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 导入笔记本数据
        /// </summary>
        /// <param name="batteryInfo">笔记本对象</param>
        /// <param name="task">任务对象</param>
        /// <returns>成功导入数量</returns>
        public int CheckInByLaptop(List<BatteryLaptopSCUD> batteryInfo, SCUDTask task)
        {
            try
            {
                if (batteryInfo.Count == 0 || task == null)
                {
                    throw new Exception("SyncSCUD::CheckInByLaptop => No data is available for import or the task does not exist");
                }
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::CheckInByPowerBank => SyncRemote database can not be open.");
                    }
                    StringBuilder sql = new StringBuilder();
                    for (int i = 0; i < batteryInfo.Count; ++i)
                    {
                        sql.Append(string.Format("INSERT INTO SCUD_SYNCBATTERY (HANDLE_TASK,E_ELECTRON_SN,E_LOTSN,E_OCV,E_TEST_TIME,E_IR,E_DCR,E_UPLOAD_TIME,Pallet_ID) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", task.HANDLE, batteryInfo[i].E_ELECTRON_SN, batteryInfo[i].E_LOTSN, batteryInfo[i].E_OCV, batteryInfo[i].E_TEST_TIME, batteryInfo[i].E_IR, batteryInfo[i].E_DCR, batteryInfo[i].E_UPLOAD_TIME, batteryInfo[i].Pallet_ID));
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    int rowAffect = 0;
                    try
                    {
                        rowAffect = comm.ExecuteNonQuery();
                        tran.Commit();
                        comm.CommandText = string.Format("UPDATE SCUD_TASK SET TOTAL = '{1}' WHERE HANDLE = '{0}';", task.HANDLE, rowAffect.ToString());
                        comm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        SysLog log = new SysLog(ex.Message);
                    }
                    return rowAffect;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 导入智能家具数据
        /// </summary>
        /// <param name="batteryInfo">智能家具对象</param>
        /// <param name="task">任务对象</param>
        /// <returns>成功导入数量</returns>
        public int CheckInBySmartFurniture(List<SmartFurnitureSCUD> batteryInfo,SCUDTask task)
        {
            try
            {
                if (batteryInfo.Count == 0 || task == null)
                {
                    throw new Exception("SyncSCUD::CheckInBySmartFurniture => No data is available for import or the task does not exist");
                }
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::CheckInBySmartFurniture => SyncRemote database can not be open.");
                    }
                    StringBuilder sql = new StringBuilder();
                    for (int i = 0; i < batteryInfo.Count; ++i)
                    {
                        sql.Append($"INSERT INTO SCUD_SYNCBATTERY (HANDLE_TASK,sn,ocvb_voltage,ocvb_test_time,ocvb_inter_resi,capacity,grade,k_value,thickness,supplier_no,ocv1,ocv1_test_time,ir1,ocv2,ocv2_test_time,ir2,test_result,lot) VALUES ('{task.HANDLE}','{batteryInfo[i].sn}','{batteryInfo[i].ocvb_voltage}','{batteryInfo[i].ocvb_test_time}','{batteryInfo[i].ocvb_inter_resi}','{batteryInfo[i].capacity}','{batteryInfo[i].grade}','{batteryInfo[i].k_value}','{batteryInfo[i].thickness}','{batteryInfo[i].supplier_no}','{batteryInfo[i].ocv1}','{batteryInfo[i].ocv1_test_time}','{batteryInfo[i].ir1}','{batteryInfo[i].ocv2}','{batteryInfo[i].ocv2_test_time}','{batteryInfo[i].ir2}','{batteryInfo[i].test_result}','{batteryInfo[i].lot}');");
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    int rowAffect = comm.ExecuteNonQuery();
                    if (rowAffect == batteryInfo.Count)
                    {
                        tran.Commit();
                        comm.CommandText = $"UPDATE SCUD_TASK SET TOTAL = '{rowAffect}' WHERE HANDLE = '{task.HANDLE}';";
                        comm.ExecuteNonQuery();
                        return rowAffect;
                    }
                    else
                    {
                        tran.Rollback();
                        throw new Exception("SyncSCUD::CheckInBySmartFurniture => Fail to checkin data");
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <param name="state">状态</param>
        /// <returns>成功：true；失败：false</returns>
        public bool UpdateTask(ref SCUDTask task,string state)
        {
            if (task == null || string.IsNullOrEmpty(task.HANDLE) || string.IsNullOrEmpty(state))
            {
                return false;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::UpdateTask => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    if (state == "准备就绪")
                    {
                        comm.CommandText = string.Format("UPDATE SCUD_TASK SET STATE = '{1}',TOTAL = '{2}' WHERE HANDLE = '{0}';", task.HANDLE, state, task.TOTAL);
                    }
                    else
                    {
                        comm.CommandText = string.Format("UPDATE SCUD_TASK SET STATE = '{1}' WHERE HANDLE = '{0}';", task.HANDLE, state);
                    }
                    if (comm.ExecuteNonQuery() == 1)
                    {
                        task.STATE = state;
                        return true;
                    }
                    else
                    {
                        throw new Exception("SyncSCUD::UpdateTask => Fail to update SCUD_Task.");
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return false;
                }
            }
        }
        /// <summary>
        /// 备份导入数据（充电宝）
        /// </summary>
        /// <param name="batteryList">导入数据列表</param>
        /// <param name="task">任务</param>
        /// <param name="handle">Handle_Send_info</param>
        /// <returns>成功数量</returns>
        public int BackupSyncBatteryByPowerBank(PowerBankSCUD batteryList,SCUDTask task,string handle)
        {
            try
            {
                if (batteryList.data.Count==0 || batteryList==null || task==null || string.IsNullOrEmpty(task.HANDLE))
                {
                    throw new Exception("SyncSCUD::BackupSyncBattery => data or task does not exist.");
                }
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::BackupSyncBatteryByPowerBank => SyncRemote database can not be open.");
                    }
                    StringBuilder sql = new StringBuilder();
                    StringBuilder barcodes = new StringBuilder();
                    for (int i = 0; i < batteryList.data.Count; ++i)
                    {
                        sql.Append(string.Format("INSERT INTO SCUD_SYNCBATTERY_LOG (HANDLE_TASK,HANDLE_SEND_INFO,cellSn,voltageValue,testDate,PB_SN,CO_ITEM_CODE,CO_ITEM_NAME,CO_ITEM_SPEC,PN_SN,GEAR) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');", task.HANDLE, handle, batteryList.data[i].cellSn, batteryList.data[i].voltageValue, batteryList.data[i].testDate, batteryList.data[i].PB_SN, batteryList.data[i].CO_ITEM_CODE, batteryList.data[i].CO_ITEM_NAME, batteryList.data[i].CO_ITEM_SPEC, batteryList.data[i].PN_SN, batteryList.data[i].GEAR));
                        barcodes.Append(string.Format("'{0}',",batteryList.data[i].cellSn));
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    comm.CommandTimeout = 120;
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    int rowAffect = comm.ExecuteNonQuery();
                    if (rowAffect != batteryList.data.Count)
                    {
                        tran.Rollback();
                        throw new Exception("SyncSCUD::BackupSyncBatteryByPowerBank => fail to backup SCUD_SYNCBATTERY_LOG.");
                    }
                    tran.Commit();
                    sql.Clear();
                    sql.Append("DELETE FROM SCUD_SYNCBATTERY WHERE CELLSN IN (");
                    sql.Append(barcodes.ToString().Substring(0, barcodes.Length - 1));
                    sql.Append(");");
                    comm.CommandText = sql.ToString();
                    comm.ExecuteNonQuery();
                    return rowAffect;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                UpdateTask(ref task, "备份充电宝导入数据失败，Failed to backup SyncBattery");
                return 0;
            }
        }
        /// <summary>
        /// 备份导入数据（笔记本）
        /// </summary>
        /// <param name="batteryList">导入数据列表</param>
        /// <param name="task">任务</param>
        /// <param name="handle">Handle_Send_info</param>
        /// <returns>成功数量</returns>
        public int BackupSyncBatteryByLaptop(List<BatteryLaptopSCUD> batteryList, SCUDTask task, string handle)
        {
            try
            {
                if (batteryList.Count == 0 || batteryList == null || task == null || string.IsNullOrEmpty(task.HANDLE))
                {
                    throw new Exception("SyncSCUD::BackupSyncBatteryByLaptop => data or task does not exist.");
                }
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::BackupSyncBatteryByLaptop => SyncRemote database can not be open.");
                    }
                    StringBuilder sql = new StringBuilder();
                    StringBuilder barcodes = new StringBuilder();
                    for (int i = 0; i < batteryList.Count; ++i)
                    {
                        sql.Append(string.Format("INSERT INTO SCUD_SYNCBATTERY_LOG (HANDLE_TASK,HANDLE_SEND_INFO,E_ELECTRON_SN,E_LOTSN,E_OCV,E_TEST_TIME,E_IR,E_DCR,E_UPLOAD_TIME,Pallet_ID) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');", task.HANDLE, handle, batteryList[i].E_ELECTRON_SN, batteryList[i].E_LOTSN, batteryList[i].E_OCV, batteryList[i].E_TEST_TIME, batteryList[i].E_IR, batteryList[i].E_DCR, batteryList[i].E_UPLOAD_TIME, batteryList[i].Pallet_ID));
                        barcodes.Append(string.Format("'{0}',", batteryList[i].E_ELECTRON_SN));
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    int rowAffect = comm.ExecuteNonQuery();
                    if (rowAffect != batteryList.Count)
                    {
                        tran.Rollback();
                        throw new Exception("SyncSCUD::BackupSyncBatteryByLaptop => fail to backup SCUD_SYNCBATTERY_LOG.");
                    }
                    tran.Commit();
                    sql.Clear();
                    sql.Append("DELETE FROM SCUD_SYNCBATTERY WHERE E_ELECTRON_SN IN (");
                    sql.Append(barcodes.ToString().Substring(0, barcodes.Length - 1));
                    sql.Append(");");
                    comm.CommandText = sql.ToString();
                    comm.ExecuteNonQuery();
                    return rowAffect;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                UpdateTask(ref task, "备份笔记本导入数据失败，Failed to backup SyncBattery");
                return 0;
            }
        }
        public int BackupSyncBatteryBySmartFurniture(SmartFurnitureSCUD battery, SCUDTask task, string handle)
        {
            if (battery==null || task==null || string.IsNullOrEmpty(handle))
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    throw new Exception("SyncSCUD::BackupSyncBatteryBySmartFurniture => SyncRemote database can not be open.");
                }
                SqlCommand comm = new SqlCommand($"INSERT INTO SCUD_SYNCBATTERY (HANDLE_TASK,sn,ocvb_voltage,ocvb_test_time,ocvb_inter_resi,capacity,grade,k_value,thickness,supplier_no,ocv1,ocv1_test_time,ir1,ocv2,ocv2_test_time,ir2,test_result,lot) VALUES ('{task.HANDLE}','{battery.sn}','{battery.ocvb_voltage}','{battery.ocvb_test_time}','{battery.ocvb_inter_resi}','{battery.capacity}','{battery.grade}','{battery.k_value}','{battery.thickness}','{battery.supplier_no}','{battery.ocv1}','{battery.ocv1_test_time}','{battery.ir1}','{battery.ocv2}','{battery.ocv2_test_time}','{battery.ir2}','{battery.test_result}','{battery.lot}');DELETE FROM SCUD_SYNCBATTERY WHERE SN = '{battery.sn}';", conn);
                try
                {
                    comm.ExecuteNonQuery();
                    return 1;
                }
                catch(Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 写入 SendInfo
        /// </summary>
        /// <param name="json">准备发送的 json</param>
        /// <param name="task">任务对象</param>
        /// <returns></returns>
        public string CreateSendInfo(string json,SCUDTask task)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::CreateSendInfo => SyncRemote db can not be open.");
                    }
                    StringBuilder sql = new StringBuilder();
                    sql.Append("INSERT INTO SCUD_SEND_INFO (HANDLE_TASK,CONTENTS,QTY) OUTPUT INSERTED.HANDLE VALUES ('");
                    sql.Append(task.HANDLE);
                    sql.Append("','");
                    sql.Append(json);
                    sql.Append("','");
                    sql.Append(task.COMPLETED);
                    sql.Append("');");
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    string handle = comm.ExecuteScalar().ToString();
                    return handle;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    UpdateTask(ref task, "写入Json数据失败，Failed to write SendInfo.");
                    return null;
                }
            }
        }
        /// <summary>
        /// 创建发送数据 json
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <returns>成功数量</returns>
        public int SyncData(SCUDTask task)
        {
            int pos = 0;
            try
            {
                if (task == null || string.IsNullOrEmpty(task.HANDLE))
                {
                    throw new Exception("SyncSCUD::SyncData => task does not exist.");
                }
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::SyncData => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT S.* FROM SCUD_SYNCBATTERY S INNER JOIN SCUD_TASK T ON S.HANDLE_TASK = T.HANDLE INNER JOIN SCUD_CONFIG C ON T.HANDLE_CONFIG = C.HANDLE WHERE T.STATE = '已创建' AND C.STATE = 'Y' AND T.HANDLE = {0};", task.HANDLE), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception("Task data does not exist.");
                    }
                    int sum = 0;
                    // 充电宝
                    if (task.DEPT == "POWER_BANK")
                    {
                        PowerBankSCUD batteryList = new PowerBankSCUD();
                        batteryList.appid = task.APPID;
                        batteryList.pwd = task.PWD;

                        StringBuilder sql = new StringBuilder();
                        while (reader.Read())
                        {
                            ++pos;
                            BatteryPowerBankSCUD battery = new BatteryPowerBankSCUD();
                            battery.cellSn = reader["cellSn"].ToString();
                            battery.voltageValue = double.Parse(reader["voltageValue"].ToString());
                            battery.testDate = reader["testDate"].ToString();
                            battery.PB_SN = reader["PB_SN"].ToString();
                            battery.CO_ITEM_CODE = reader["CO_ITEM_CODE"].ToString();
                            battery.CO_ITEM_NAME = reader["CO_ITEM_NAME"].ToString();
                            battery.CO_ITEM_SPEC = reader["CO_ITEM_SPEC"].ToString();
                            battery.PN_SN = reader["PN_SN"].ToString();
                            battery.GEAR = reader["GEAR"].ToString();
                            battery.IRValue = reader["IRValue"].ToString();
                            batteryList.data.Add(battery);
                            if (batteryList.data.Count % 100 == 0)
                            {
                                string json = JsonConvert.SerializeObject(batteryList);
                                task.COMPLETED = batteryList.data.Count;
                                string handle = CreateSendInfo(json, task);
                                int rowsAffect = BackupSyncBatteryByPowerBank(batteryList, task, handle);
                                if (rowsAffect != batteryList.data.Count)
                                {
                                    throw new Exception("");
                                }
                                else
                                {
                                    task.TOTAL += rowsAffect;
                                    batteryList.data.Clear();
                                }
                            }
                        }
                        if (batteryList.data.Count > 0)
                        {
                            string json = JsonConvert.SerializeObject(batteryList);
                            task.COMPLETED = batteryList.data.Count;
                            string handle = CreateSendInfo(json, task);
                            int rowsAffect = BackupSyncBatteryByPowerBank(batteryList, task, handle);
                            if (rowsAffect != batteryList.data.Count)
                            {
                                throw new Exception("");
                            }
                            task.TOTAL += rowsAffect;
                        }
                        reader.Close();
                    }
                    // 笔记本
                    else if (task.DEPT.ToUpper() == "LAPTOP")
                    {
                        ++pos;
                        List<BatteryLaptopSCUD> batteryList = new List<BatteryLaptopSCUD>();
                        StringBuilder sql = new StringBuilder();
                        while (reader.Read())
                        {
                            BatteryLaptopSCUD battery = new BatteryLaptopSCUD();
                            battery.E_ELECTRON_SN = reader["E_ELECTRON_SN"].ToString();
                            battery.E_LOTSN = reader["E_LOTSN"].ToString();
                            battery.E_OCV = reader["E_OCV"].ToString();
                            battery.E_TEST_TIME = reader["E_TEST_TIME"].ToString();
                            battery.E_IR = reader["E_IR"].ToString();
                            battery.E_DCR = reader["E_DCR"].ToString();
                            battery.Pallet_ID = reader["Pallet_ID"].ToString();
                            battery.E_UPLOAD_TIME = reader["E_UPLOAD_TIME"].ToString();
                            batteryList.Add(battery);
                            if (batteryList.Count % 100 == 0)
                            {
                                string json = JsonConvert.SerializeObject(batteryList);
                                task.COMPLETED = batteryList.Count;
                                string handle = CreateSendInfo(json, task);
                                int rowsAffect = BackupSyncBatteryByLaptop(batteryList, task, handle);
                                if (rowsAffect != batteryList.Count)
                                {
                                    throw new Exception("");
                                }
                                else
                                {
                                    task.TOTAL += rowsAffect;
                                    batteryList.Clear();
                                }
                            }
                        }
                        if (batteryList.Count > 0)
                        {
                            string json = JsonConvert.SerializeObject(batteryList);
                            task.COMPLETED = batteryList.Count;
                            string handle = CreateSendInfo(json, task);
                            int rowsAffect = BackupSyncBatteryByLaptop(batteryList, task, handle);
                            if (rowsAffect != batteryList.Count)
                            {
                                throw new Exception("");
                            }
                            task.TOTAL += rowsAffect;
                        }
                        reader.Close();
                    }
                    else if (task.DEPT.ToUpper() == "SMART FURNITURE")
                    {
                        task.TOTAL = 0;
                        SmartFurnitureSCUD battery = new SmartFurnitureSCUD();
                        while (reader.Read())
                        {
                            battery.sn = reader["sn"].ToString();
                            battery.ocvb_voltage = reader["ocvb_voltage"].ToString();
                            battery.ocvb_test_time = Convert.ToDateTime(reader["ocvb_test_time"]).ToString("yyyy-MM-dd hh:mm:ss");
                            battery.ocvb_inter_resi = reader["ocvb_inter_resi"].ToString();
                            battery.capacity = reader["capacity"].ToString();
                            battery.grade = reader["grade"].ToString();
                            battery.k_value = reader["k_value"].ToString();
                            battery.thickness = reader["thickness"].ToString();
                            battery.supplier_no = reader["supplier_no"].ToString();
                            battery.ocv1 = reader["ocv1"].ToString();
                            battery.ocv1_test_time = reader["ocv1_test_time"].ToString();
                            battery.ir1 = reader["ir1"].ToString();
                            battery.ocv2 = reader["ocv2"].ToString();
                            battery.ocv2_test_time = reader["ocv2_test_time"].ToString();
                            battery.ir2 = reader["ir2"].ToString();
                            battery.test_result = reader["test_result"].ToString();
                            battery.lot = reader["lot"].ToString();
                            task.COMPLETED = 1;
                            string json = JsonConvert.SerializeObject(battery);
                            string handle = CreateSendInfo(json, task);
                            task.TOTAL += BackupSyncBatteryBySmartFurniture(battery, task, handle);
                        }
                    }
                    return task.TOTAL;
                }
            }
            catch (Exception ex)
            {
                StackFrame sf = new StackFrame(true);
                int lineNumber = sf.GetFileLineNumber();
                int colNumber = sf.GetFileColumnNumber();
                string fileName = sf.GetFileName();
                string methodName = sf.GetMethod().Name;
                SysLog log = new SysLog(ex.Message);
                UpdateTask(ref task, "创建发送数据失败，Failed to generate json");
                return 0;
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <returns>成功发送数量</returns>
        public int SendData(SCUDTask task)
        {
            try
            {
                if (task == null || string.IsNullOrEmpty(task.HANDLE))
                {
                    throw new Exception("SyncSCUD::SendData => task does not exist.");
                }
                if (task.STATE != "准备就绪")
                {
                    throw new Exception("SyncSCUD::SendData => Task state is not 准备就绪");
                }
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::SendData => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("UPDATE SCUD_TASK SET STATE = '{1}' WHERE HANDLE = '{0}';", task.HANDLE, "开始回传"), conn);
                    comm.ExecuteNonQuery();
                    comm.CommandText = string.Format("SELECT S.* FROM SCUD_SEND_INFO S INNER JOIN SCUD_TASK T ON S.HANDLE_TASK = T.HANDLE WHERE T.STATE = '开始回传' AND T.HANDLE = '{0}';", task.HANDLE);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        WebSOAP soap = new WebSOAP();
                        if (task.DEPT == "SMART FURNITURE")
                        {
                            ResultSmartFurnitureSCUD result = soap.SCUD_QuerySoapWebApi(task.URI, reader["CONTENTS"].ToString());
                            if (result.M_RESULT_FLAG== "OK")
                            {
                                BackupSendInfo(task, reader["HANDLE"].ToString(), result.HANDLE_RECIEVED);
                            }
                            else
                            {
                                UpdateTask(ref task, "失败");
                            }
                        }
                        else
                        {
                            SCUD_ResponseResult result = new SCUD_ResponseResult();
                            result = soap.SCUD_QuerySoapWebService(task.URI, string.Format("dbtype={0}&type={1}&json={2}", task.DBTYPE, task.TYPE, reader["CONTENTS"].ToString()));
                            if (result.msg.Contains("传输成功"))
                            {
                                BackupSendInfo(task, reader["HANDLE"].ToString(), result.data.ToString());
                            }
                            else
                            {
                                UpdateTask(ref task, "失败");
                            }
                        }
                    }
                    UpdateTask(ref task, "结束");
                    reader.Close();
                    // 邮件通知
                    Complete(task.HANDLE);
                    comm.CommandText = string.Format("SELECT SUM(QTY) QTY FROM SCUD_SEND_LOG WHERE HANDLE_TASK = '{0}'; ", task.HANDLE);
                    reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        return 0;
                    }
                    reader.Read();
                    int qty = int.Parse(reader["QTY"].ToString());
                    reader.Close();
                    return qty;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                UpdateTask(ref task, "发送数据失败，Failed to send data");
                return 0;
            }
        }
        /// <summary>
        /// 发送成功后备份发送 json
        /// </summary>
        /// <param name="task"></param>
        /// <param name="handle_send"></param>
        /// <param name="handle_recieved"></param>
        /// <returns>成功：1；失败：0</returns>
        public int BackupSendInfo(SCUDTask task,string handle_send,string handle_recieved)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::BackupSendInfo => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO SCUD_SEND_LOG (HANDLE_TASK,HANDLE_SEND_INFO,HANDLE_RECIEVED,CONTENTS,QTY,CREATED_DATE_TIME) SELECT HANDLE_TASK,'{0}','{1}',CONTENTS,QTY,CREATED_DATE_TIME FROM SCUD_SEND_INFO WHERE HANDLE = '{0}';DELETE FROM SCUD_SEND_INFO WHERE HANDLE = '{0}';", handle_send, handle_recieved), conn);
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    if (comm.ExecuteNonQuery() == 2)
                    {
                        tran.Commit();
                    }
                    else
                    {
                        tran.Rollback();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 更新任务状态并邮件推送
        /// </summary>
        /// <param name="handle">任务 HANDLE</param>
        public void Complete(string handle)
        {
            //if (task == null)
            //{
            //    return;
            //}
            //UpdateTask(ref task, "成功");
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SCUD::Complete => database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT T.HANDLE,C.BOMNO,C.DEPT,T.STATE,T.CREATED_DATE_TIME,SUM(L.QTY) COMPLETED,T.TOTAL FROM SCUD_TASK T INNER JOIN SCUD_CONFIG C ON T.HANDLE_CONFIG = C.HANDLE INNER JOIN SCUD_SEND_LOG L ON T.HANDLE = L.HANDLE_TASK WHERE T.HANDLE = '{0}' GROUP BY T.HANDLE,T.STATE,T.CREATED_DATE_TIME,T.TOTAL,C.BOMNO,C.DEPT;", handle), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    TableWeb tWeb = new TableWeb();
                    tWeb.addThead("任务编号");
                    tWeb.addThead("任务状态");
                    tWeb.addThead("型号");
                    tWeb.addThead("类型");
                    tWeb.addThead("开始回传时间");
                    tWeb.addThead("成功完成数量");
                    tWeb.addThead("计划完成数量");
                    while (reader.Read())
                    {
                        tWeb.addContext(reader["HANDLE"].ToString());
                        tWeb.addContext(reader["STATE"].ToString());
                        tWeb.addContext(reader["BOMNO"].ToString());
                        tWeb.addContext(reader["DEPT"].ToString());
                        tWeb.addContext(reader["CREATED_DATE_TIME"].ToString());
                        tWeb.addContext(reader["COMPLETED"].ToString());
                        tWeb.addContext(reader["TOTAL"].ToString());
                    }
                    reader.Close();
                    comm.CommandText = string.Format("SELECT E.EMAIL FROM CUSTOMER C INNER JOIN EMAIL_GROUP G ON C.HANDLE = G.HANDLE_CUSTOMER AND C.CUSTOMER = 'SCUD' INNER JOIN EMAIL_CONFIG E ON G.HANDLE_EMAIL = E.HANDLE;");
                    reader = comm.ExecuteReader();
                    StringBuilder emailList = new StringBuilder();
                    while (reader.Read())
                    {
                        emailList.Append(reader[0].ToString());
                        emailList.Append(",");
                    }
                    reader.Close();
                    //发送邮件
                    Mail.SendMail(emailList.ToString(), System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], "SCUD 数据发送任务", tWeb.TableHtml());
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return;
                }
            }
        }
        /// <summary>
        /// 获取型号/料号
        /// </summary>
        /// <returns></returns>
        public List<Item_no> Item_no()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::Item_no => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT BOMNO,ITEM_NO FROM SCUD_CONFIG GROUP BY BOMNO,ITEM_NO;", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception("The item information does not exist.");
                    }
                    List<Item_no> items = new List<global::Item_no>();
                    while (reader.Read())
                    {
                        Item_no item = new Item_no();
                        item.BOMNO = reader["BOMNO"].ToString();
                        item.ITEM_NO = reader["ITEM_NO"].ToString();
                        items.Add(item);
                    }
                    return items;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 回传任务查询
        /// </summary>
        /// <param name="date">日期范围</param>
        /// <param name="task">任务编号</param>
        /// <param name="barcode">电池码号</param>
        /// <returns></returns>
        public DataTable Query(string date,string task,string barcode)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    string sql = null;
                    if (!string.IsNullOrEmpty(barcode))
                    {
                        sql = string.Format("SELECT '{0}' 码号, T.HANDLE 任务号,T.STATE 任务状态,T.CREATED_DATE_TIME 任务启动时间,L2.CREATED_DATE_TIME 该只产品数据发送时间,R.RECEIVED_DATE_TIME 接收SCUD回复时间,R.RECEIVED_INFO 接收SCUD回复内容 FROM SCUD_SYNCBATTERY_LOG L INNER JOIN SCUD_SEND_LOG L2 ON L.HANDLE_SEND_INFO = L2.HANDLE_SEND_INFO INNER JOIN SCUD_RECIEVED_INFO R ON L2.HANDLE_RECIEVED = R.HANDLE INNER JOIN SCUD_TASK T ON L.HANDLE_TASK = T.HANDLE WHERE L.CELLSN = '{0}' OR L.E_ELECTRON_SN = '{0}';", barcode);
                    }
                    else if (!string.IsNullOrEmpty(task))
                    {
                        sql = string.Format("SELECT T.HANDLE 任务号,T.STATE 任务状态,T.TOTAL 计划发送数量,SUM(L.QTY) 完成发送数量,T.CREATED_DATE_TIME 任务启动时间 FROM SCUD_TASK T INNER JOIN  SCUD_SEND_LOG L ON T.HANDLE = L.HANDLE_TASK WHERE T.HANDLE = '{0}' GROUP BY T.HANDLE,T.STATE,T.TOTAL,T.CREATED_DATE_TIME ORDER BY T.CREATED_DATE_TIME DESC;", task);
                    }
                    else if (!string.IsNullOrEmpty(date))
                    {
                        sql = string.Format("SELECT T.HANDLE 任务号,T.STATE 任务状态,T.TOTAL 计划发送数量,SUM(L.QTY) 完成发送数量,T.CREATED_DATE_TIME 任务启动时间 FROM SCUD_TASK T INNER JOIN  SCUD_SEND_LOG L ON T.HANDLE = L.HANDLE_TASK WHERE T.CREATED_DATE_TIME {O} GROUP BY T.HANDLE,T.STATE,T.TOTAL,T.CREATED_DATE_TIME ORDER BY T.CREATED_DATE_TIME DESC;", date);
                    }
                    else
                    {
                        sql = "SELECT T.HANDLE 任务号,T.STATE 任务状态,T.TOTAL 计划发送数量,SUM(L.QTY) 完成发送数量,T.CREATED_DATE_TIME 任务启动时间 FROM SCUD_TASK T LEFT JOIN  SCUD_SEND_LOG L ON T.HANDLE = L.HANDLE_TASK GROUP BY T.HANDLE,T.STATE,T.TOTAL,T.CREATED_DATE_TIME ORDER BY T.CREATED_DATE_TIME DESC;";
                    }
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SCUD::Query => database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader reader = comm.ExecuteReader();
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
    }
    public class SyncTWS
    {
        /// <summary>
        /// 获取型号信息
        /// </summary>
        /// <returns></returns>
        public List<Item_no> Item_no()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncTWS::Item_no => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT BOM,ITEM FROM TWS_CONFIG WHERE IS_CURRENT = 'Y' GROUP BY BOM,ITEM;", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception("The item information does not exist.");
                    }
                    List<Item_no> items = new List<global::Item_no>();
                    while (reader.Read())
                    {
                        Item_no item = new Item_no();
                        item.BOMNO = reader["BOM"].ToString();
                        item.ITEM_NO = reader["ITEM"].ToString();
                        items.Add(item);
                    }
                    return items;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="item">料号</param>
        /// <returns></returns>
        public TWSTask CreateTask(string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncTWS::CreateTask => SyncRemote database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT * FROM TWS_CONFIG WHERE IS_CURRENT = 'Y' AND ITEM = '{0}';", item), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception("TWS_CONFIG data does not exist.");
                    }
                    TWSTask task = new TWSTask();
                    reader.Read();
                    task.HANDLE_CONFIG = reader["HANDLE"].ToString();
                    task.ITEM = reader["ITEM"].ToString();
                    task.BOM = reader["BOM"].ToString();
                    task.TOKEN = reader["TOKEN"].ToString();
                    task.URI = reader["URI"].ToString();
                    task.STATE = "已创建";
                    reader.Close();
                    comm.CommandText = string.Format(string.Format("INSERT INTO TWS_TASK(HANDLE_CONFIG,ITEM,BOM,TOKEN,URI,STATE) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}','{2}','{3}','{4}','{5}');", task.HANDLE_CONFIG, task.ITEM, task.BOM, task.TOKEN, task.URI, task.STATE));
                    string handle = comm.ExecuteScalar().ToString();
                    task.HANDLE = handle;
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="handle">任务 handle</param>
        /// <returns></returns>
        public TWSTask GetTask(string handle)
        {
            if (string.IsNullOrEmpty(handle))
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncTWS::GetTask => SyncRemote database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT * FROM TWS_TASK WHERE HANDLE = '{0}';", handle), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    TWSTask task = new TWSTask();
                    reader.Read();
                    task.BOM = reader["BOM"].ToString();
                    task.HANDLE = reader["HANDLE"].ToString();
                    task.HANDLE_CONFIG = reader["HANDLE_CONFIG"].ToString();
                    task.ITEM = reader["ITEM"].ToString();
                    task.TOKEN = reader["TOKEN"].ToString();
                    task.URI = reader["URI"].ToString();
                    task.STATE = reader["STATE"].ToString();
                    reader.Close();
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public TWSTask UpdateTask(TWSTask task, string state)
        {
            if (task == null || string.IsNullOrEmpty(state))
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncSCUD::UpdateTask => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("UPDATE TWS_TASK SET STATE = '{1}' WHERE HANDLE = '{0}';", task.HANDLE, state), conn);
                    comm.ExecuteNonQuery();
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="batteryList">数据列表</param>
        /// <param name="task">任务对象</param>
        /// <returns></returns>
        public int CheckIn(List<BatteryTWS> batteryList, TWSTask task)
        {
            if (batteryList.Count == 0 || task == null)
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncTWS::CheckIn => SyncRemote database can not be open.");
                    }
                    StringBuilder sql = new StringBuilder();
                    foreach (BatteryTWS battery in batteryList)
                    {
                        sql.Append(string.Format("INSERT INTO TWS_SYNCBATTERY (HANDLE_TASK,supplierName,cellPartNumber,cellSN,cellType,cellDateCode,cellLotNumber,cellFactoryID,cellOCVoltage,cellACImpedance,cellCapacity,cellKvalue,cellProLineNO,cellWorkVoltage,cellTestTime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}');", task.HANDLE, battery.supplierName, battery.cellPartNumber, battery.cellSN, battery.cellType, battery.cellDateCode, battery.cellLotNumber, battery.cellFactoryID, battery.cellOCVoltage, battery.cellACImpedance, battery.cellCapacity, battery.cellKvalue, battery.cellProLineNO, battery.cellWorkVoltage, battery.cellTestTime));
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    comm.Transaction = tran;
                    comm.ExecuteNonQuery();
                    tran.Commit();
                    return batteryList.Count;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 创建发送 json
        /// </summary>
        /// <param name="batteryList">电池列表</param>
        /// <returns>创建成功的 handle</returns>
        public string CreateSendInfo(List<BatteryTWS> batteryList, TWSTask task)
        {
            if (batteryList.Count == 0)
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    throw new Exception("SyncTWS::CreateSendInfo => SyncRemote db can not be open.");
                }
                try
                {
                    string json = JsonConvert.SerializeObject(batteryList);
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO TWS_SEND_INFO (HANDLE_TASK,INFO,QTY) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}','{2}');", task.HANDLE, json, batteryList.Count), conn);
                    string handle = comm.ExecuteScalar().ToString();
                    return handle;
                }
                catch (Exception ex)
                {
                    SysLog log=new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 备份电池信息
        /// </summary>
        /// <param name="batteryList">电池清单</param>
        /// <param name="handle_sendinfo">已生成上传 json 的 handle</param>
        /// <param name="handle_task">任务 handle</param>
        /// <returns></returns>
        public int BackupSyncbattery(List<BatteryTWS> batteryList, string handle_sendinfo, string handle_task)
        {
            if (batteryList.Count == 0 || string.IsNullOrEmpty(handle_sendinfo) || string.IsNullOrEmpty(handle_task))
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    throw new Exception("SyncTWS::BackupSyncbattery => SyncRemote db can not be open.");
                }
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    StringBuilder sn = new StringBuilder();
                    for(int i = 0; i < batteryList.Count; ++i)
                    {
                        sn.Append("'");
                        sn.Append(batteryList[i].cellSN);
                        sn.Append("',");
                    }
                    string barcodes = sn.ToString().Substring(0, sn.Length - 1);
                    string sql = string.Format("INSERT INTO TWS_SYNCBATTERY_LOG (HANDLE_TASK,HANDLE_SEND_INFO,supplierName,cellPartNumber,cellSN,cellType,cellDateCode,cellLotNumber,cellFactoryID,cellOCVoltage,cellACImpedance,cellCapacity,cellKvalue,cellProLineNO,cellWorkVoltage,cellTestTime,CREATED_DATE_TIME) SELECT '{0}','{1}',supplierName,cellPartNumber,cellSN,cellType,cellDateCode,cellLotNumber,cellFactoryID,cellOCVoltage,cellACImpedance,cellCapacity,cellKvalue,cellProLineNO,cellWorkVoltage,cellTestTime,CREATED_DATE_TIME FROM TWS_SYNCBATTERY WHERE CELLSN IN ({2});DELETE FROM TWS_SYNCBATTERY WHERE CELLSN IN ({2});", handle_task, handle_sendinfo, barcodes);
                    SqlCommand comm = new SqlCommand(sql, conn);
                    comm.Transaction = tran;
                    comm.ExecuteNonQuery();
                    tran.Commit();
                    return batteryList.Count;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 将待发送电池完成发送前所有准备（生成 json，备份）
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <returns>成功数量</returns>
        public int SyncData(TWSTask task)
        {
            if (task == null)
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    throw new Exception("SyncTWS::SyncData => SyncRemote db can not be open.");
                }
                try
                {
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    bool state = true;
                    int count = 0;
                    while (state)
                    {
                        comm.CommandText = string.Format("SELECT TOP 10 T.HANDLE_TASK,supplierName,cellPartNumber,cellSN,cellType,cellDateCode,cellLotNumber,cellFactoryID,cellOCVoltage,cellACImpedance,cellCapacity,cellKvalue,cellProLineNO,cellWorkVoltage,cellTestTime FROM TWS_SYNCBATTERY S INNER JOIN TWS_TASK T ON S.HANDLE_TASK = T.HANDLE AND T.HANDLE = '{0}';", task.HANDLE);
                        SqlDataReader reader = comm.ExecuteReader();
                        if (!reader.HasRows) state = false;
                        List<BatteryTWS> batteryList = new List<BatteryTWS>();
                        while (reader.Read())
                        {
                            BatteryTWS battery = new BatteryTWS();
                            battery.supplierName = reader["supplierName"].ToString();
                            battery.cellPartNumber = reader["cellPartNumber"].ToString();
                            battery.cellSN = reader["cellSN"].ToString();
                            battery.cellType = reader["cellType"].ToString();
                            battery.cellDateCode = reader["cellDateCode"].ToString();
                            battery.cellLotNumber = reader["cellLotNumber"].ToString();
                            battery.cellFactoryID = reader["cellFactoryID"].ToString();
                            battery.cellOCVoltage = reader["cellOCVoltage"].ToString();
                            battery.cellACImpedance = reader["cellACImpedance"].ToString();
                            battery.cellCapacity = reader["cellCapacity"].ToString();
                            battery.cellKvalue = reader["cellKvalue"].ToString();
                            battery.cellProLineNO = reader["cellProLineNO"].ToString();
                            battery.cellWorkVoltage = reader["cellWorkVoltage"].ToString();
                            battery.cellTestTime = reader["cellTestTime"].ToString();
                            batteryList.Add(battery);
                        }
                        reader.Close();
                        string handle = CreateSendInfo(batteryList, task);
                        count += BackupSyncbattery(batteryList, handle, task.HANDLE);
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
        /// 备份发送字符串
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <param name="handle_send_info">发送</param>
        /// <returns></returns>
        public int BackupSendInfo(TWSTask task,string handle_send_info)
        {
            if (task == null)
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncTWS::BackupSendInfo => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO TWS_SEND_LOG (HANDLE_SEND_INFO,HANDLE_TASK,INFO,QTY,CREATED_DATE_TIME) SELECT HANDLE,'{0}',INFO,QTY,CREATED_DATE_TIME FROM TWS_SEND_INFO WHERE HANDLE = '{1}';DELETE FROM TWS_SEND_INFO WHERE HANDLE = '{1}';", task.HANDLE, handle_send_info), conn);
                    comm.Transaction = tran;
                    comm.ExecuteNonQuery();
                    tran.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    SysLog log = new SysLog("SyncTWS::BackupSendInfo => " + ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 发磅数据
        /// </summary>
        /// <param name="task">待发送任务</param>
        /// <returns>成功数量</returns>
        public int SendData(TWSTask task)
        {
            if (task==null || task.STATE != "准备就绪")
            {
                return 0;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncTWS::SendData => SyncRemote db can not be open.");
                    }
                    int count = 0;
                    Hashtable hashReceived = new Hashtable();
                    SqlCommand comm = new SqlCommand(string.Format("SELECT S.HANDLE,INFO FROM TWS_SEND_INFO S INNER JOIN TWS_TASK T ON S.HANDLE_TASK = T.HANDLE AND T.STATE = '准备就绪' WHERE S.STATE IS NULL AND S.HANDLE_TASK = '{0}';", task.HANDLE), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    WebSOAP soap = new WebSOAP();
                    while (reader.Read())
                    {
                        string result = soap.TWS_QuerySoapWebService(task.URI,reader["INFO"].ToString());
                        if (TWS_Recieved(reader["HANDLE"].ToString(), result))
                        {
                            BackupSendInfo(task, reader["HANDLE"].ToString());
                            ++count;
                        }
                        else
                            throw new Exception("SyncTWS::SendData => return value indicates a failure.");
                    }
                    return count;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("SyncTWS::SendData => " + ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 写入返回接收表
        /// </summary>
        /// <param name="handle">发送 json 的 handle</param>
        /// <param name="result">返回内容</param>
        /// <returns>true：成功；false：失败</returns>
        public bool TWS_Recieved(string handle,string result)
        {
            if (string.IsNullOrEmpty(handle) || string.IsNullOrEmpty(result))
            {
                return false;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncTWS::TWS_Recieved => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO TWS_RECIEVED_INFO(HANDLE_SEND_INFO,INFO) VALUES ('{0}','{1}');", handle, result), conn);
                    if (comm.ExecuteNonQuery() != 1) throw new Exception("SyncTWS::TWS_Recieved => fail to write TWS_RECIEVED_INFO data table.");
                    TWS_ResponseResult responseResult = JsonConvert.DeserializeObject<TWS_ResponseResult>(result);
                    if (responseResult.msg == "操作成功")
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("SyncTWS::TWS_Recieved => " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 回传完成，发送邮件通知
        /// </summary>
        /// <param name="handle">任务 HANDLE</param>
        public void TWS_Complete(string handle)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TWS::Complete => database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("WITH A AS (SELECT HANDLE_TASK,SUM(CONVERT(NUMERIC, QTY)) QTY FROM TWS_SEND_INFO WHERE HANDLE_TASK = '{0}' GROUP BY HANDLE_TASK ),B AS (SELECT HANDLE_TASK ,SUM(CONVERT(NUMERIC, QTY)) QTY FROM TWS_SEND_LOG WHERE HANDLE_TASK = '{0}' GROUP BY HANDLE_TASK ) SELECT TT.HANDLE ,TT.CREATED_DATE_TIME ,TT.BOM ,TT.ITEM ,A.QTY OUTSTANDING_QTY,B.QTY COMPLETED_QTY FROM TWS_TASK tt LEFT JOIN A ON A.HANDLE_TASK = TT.HANDLE LEFT JOIN B ON B.HANDLE_TASK = TT.HANDLE WHERE TT.HANDLE = '{0}';", handle), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    TableWeb tWeb = new TableWeb();
                    tWeb.addThead("任务编号");
                    tWeb.addThead("开始时间");
                    tWeb.addThead("型号");
                    tWeb.addThead("料号");
                    tWeb.addThead("未完成数量");
                    tWeb.addThead("已完成数量");
                    while (reader.Read())
                    {
                        tWeb.addContext(reader["HANDLE"].ToString());
                        tWeb.addContext(reader["CREATED_DATE_TIME"].ToString());
                        tWeb.addContext(reader["BOM"].ToString());
                        tWeb.addContext(reader["ITEM"].ToString());
                        tWeb.addContext(reader["OUTSTANDING_QTY"].ToString());
                        tWeb.addContext(reader["COMPLETED_QTY"].ToString());
                    }
                    reader.Close();
                    comm.CommandText = string.Format("SELECT E.EMAIL FROM CUSTOMER C INNER JOIN EMAIL_GROUP G ON C.HANDLE = G.HANDLE_CUSTOMER AND C.CUSTOMER = 'TWS' INNER JOIN EMAIL_CONFIG E ON G.HANDLE_EMAIL = E.HANDLE;");
                    reader = comm.ExecuteReader();
                    StringBuilder emailList = new StringBuilder();
                    while (reader.Read())
                    {
                        emailList.Append(reader[0].ToString());
                        emailList.Append(",");
                    }
                    reader.Close();
                    //发送邮件
                    Mail.SendMail(emailList.ToString(), System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], "TWS 数据发送任务", tWeb.TableHtml());
                }
                catch (Exception ex)
                {

                }
            }
        }
        public DataTable TWS_Query(string date, string task, string barcode)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TWS::Query => database can not be open.");
                    }
                    string sql = null;
                    if (!string.IsNullOrEmpty(barcode))
                    {
                        sql = string.Format("SELECT '{0}' 码号,T.HANDLE 任务编号,T.STATE 任务状态,T.CREATED_DATE_TIME 任务启动时间,L2.CREATED_DATE_TIME 该只产品数据发送时间,R.CREATED_DATE_TIME 接收TWS回复时间,R.INFO 接收TWS回复内容 FROM TWS_SYNCBATTERY_LOG L INNER JOIN TWS_SEND_LOG L2 ON L.HANDLE_SEND_INFO = L2.HANDLE_SEND_INFO INNER JOIN TWS_RECIEVED_INFO R ON L2.HANDLE_SEND_INFO = R.HANDLE_SEND_INFO INNER JOIN TWS_TASK T ON L.HANDLE_TASK = T.HANDLE WHERE L.CELLSN = '{0}';", barcode);
                    }
                    else if (!string.IsNullOrEmpty(task))
                    {
                        sql = string.Format("WITH A AS (SELECT HANDLE_TASK,SUM(CONVERT(NUMERIC, QTY)) QTY FROM TWS_SEND_INFO GROUP BY HANDLE_TASK),B AS (SELECT HANDLE_TASK,SUM(CONVERT(NUMERIC, QTY)) QTY FROM TWS_SEND_LOG GROUP BY HANDLE_TASK) SELECT TT.HANDLE 任务编号,TT.CREATED_DATE_TIME 任务启动时间,TT.BOM 型号,TT.ITEM 料号,A.QTY 未完成数量,B.QTY 已完成数量 FROM TWS_TASK tt LEFT JOIN A ON A.HANDLE_TASK = TT.HANDLE LEFT JOIN B ON B.HANDLE_TASK = TT.HANDLE WHERE TT.HANDLE = '{0}' ORDER BY TT.HANDLE DESC;", task);
                    }
                    else if (!string.IsNullOrEmpty(date))
                    {
                        sql = string.Format("WITH A AS (SELECT HANDLE_TASK,SUM(CONVERT(NUMERIC, QTY)) QTY FROM TWS_SEND_INFO GROUP BY HANDLE_TASK),B AS (SELECT HANDLE_TASK,SUM(CONVERT(NUMERIC, QTY)) QTY FROM TWS_SEND_LOG GROUP BY HANDLE_TASK) SELECT TT.HANDLE 任务编号,TT.CREATED_DATE_TIME 任务启动时间,TT.BOM 型号,TT.ITEM 料号,A.QTY 未完成数量,B.QTY 已完成数量 FROM TWS_TASK tt LEFT JOIN A ON A.HANDLE_TASK = TT.HANDLE LEFT JOIN B ON B.HANDLE_TASK = TT.HANDLE WHERE TT.CREATED_DATE_TIME {0} ORDER BY TT.HANDLE DESC;", date);
                    }
                    else if (string.IsNullOrEmpty(barcode) && string.IsNullOrEmpty(task) && string.IsNullOrEmpty(date))
                    {
                        sql = "WITH A AS (SELECT HANDLE_TASK,SUM(CONVERT(NUMERIC, QTY)) QTY FROM TWS_SEND_INFO GROUP BY HANDLE_TASK),B AS (SELECT HANDLE_TASK,SUM(CONVERT(NUMERIC, QTY)) QTY FROM TWS_SEND_LOG GROUP BY HANDLE_TASK) SELECT TT.HANDLE 任务编号,TT.CREATED_DATE_TIME 任务启动时间,TT.BOM 型号,TT.ITEM 料号,A.QTY 未完成数量,B.QTY 已完成数量 FROM TWS_TASK tt LEFT JOIN A ON A.HANDLE_TASK = TT.HANDLE LEFT JOIN B ON B.HANDLE_TASK = TT.HANDLE ORDER BY TT.HANDLE DESC;";
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader reader = comm.ExecuteReader();
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
    }
    public class SyncKPK
    {
        private Hashtable hashColumns = new Hashtable();
        private List<string> sqlList = new List<string>();
        /// <summary>
        /// 初始化待处理数据表字段对应关系
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int initColumns(string table)
        {
            if (string.IsNullOrEmpty(table))
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["KPK"].ConnectionString))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncKPK::initColumns => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT C.NAME,P.VALUE FROM SYS.EXTENDED_PROPERTIES P ,SYS.COLUMNS C WHERE P.MAJOR_ID=OBJECT_ID('{0}') AND P.MAJOR_ID=C.OBJECT_ID AND P.MINOR_ID=C.COLUMN_ID;", table), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception(string.Format("SyncKPK::{0} => comments from table {0} were not queried.", table));
                    }
                    hashColumns.Clear();
                    sqlList.Clear();
                    while (reader.Read())
                    {
                        hashColumns.Add(reader["VALUE"].ToString().Trim(), reader["NAME"].ToString().Trim());
                    }
                    reader.Close();
                    return hashColumns.Count;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }

            }
        }
        /// <summary>
        /// 通过事务的方式添加至数据库
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            if (sqlList.Count == 0)
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["KPK"].ConnectionString))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncKPK::Save => Database can not be opened.");
                    }
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    comm.Transaction = tran;
                    int rowsAffect = 0;
                    for (int i = 0; i < sqlList.Count; ++i)
                    {
                        comm.CommandText = sqlList[i];
                        rowsAffect += comm.ExecuteNonQuery();
                    }
                    tran.Commit();
                    return rowsAffect;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 导入性能数据
        /// </summary>
        /// <param name="rows"></param>
        /// <returns>成功数量</returns>
        public int Load(List<RowPerformance> rows, string type)
        {
            if (rows == null && string.IsNullOrEmpty(type))
            {
                return 0;
            }
            try
            {
                if (initColumns(type) == 0)
                {
                    return 0;
                }
                for (int i = 0; i < rows.Count; ++i)
                {
                    StringBuilder key = new StringBuilder();
                    StringBuilder value = new StringBuilder();
                    for (int j = 0; j < rows[i].Columns.Count; ++j)
                    {
                        try
                        {
                            key.Append(hashColumns[rows[i].Columns[j].Key.Trim()].ToString());
                            key.Append(",");
                            value.Append("'");
                            value.Append(rows[i].Columns[j].Value.Trim());
                            value.Append("',");
                        }
                        catch (Exception)
                        {

                        }
                    }
                    //sql.Append(string.Format("INSERT INTO {2} (HANDLE,{0},CREATED_USER,CREATED_DATE_TIME) VALUES (STRTOBIN(SYSUUID,'UTF-8'),{1},'PoleClient',CURRENT_TIMESTAMP);", key.ToString().Substring(0, key.ToString().Length - 1), value.ToString().Substring(0, value.ToString().Length - 1), type));
                    sqlList.Add(string.Format("INSERT INTO {2} ({0},CREATED_USER) VALUES ({1},'SyncManagement');", key.ToString().Substring(0, key.ToString().Length - 1), value.ToString().Substring(0, value.ToString().Length - 1), type));
                }
                return sqlList.Count == rows.Count ? sqlList.Count : 0;
            }
            catch (Exception ex)
            {
                StackFrame sf = new StackFrame(true);
                int lineNumber = sf.GetFileLineNumber();
                int colNumber = sf.GetFileColumnNumber();
                string fileName = sf.GetFileName();
                string methodName = sf.GetMethod().Name;
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
    }
    /// <summary>
    /// 回传任务管理
    /// </summary>
    public class TaskManagement
    {
        private Tasks mTask;
        protected bool IsValidUuid(string uuid)
        {
            string pattern = @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$";
            return Regex.IsMatch(uuid, pattern);
        }
        /// <summary>
        /// 创建回传管理任务
        /// </summary>
        /// <param name="sfc">唯一标识，若为多个需用逗号隔开</param>
        /// <param name="category">类型，如：TRAY</param>
        /// <param name="item_no">料号</param>
        /// <param name="customer">客户，如：SMP</param>
        /// <param name="created_user">创建人</param>
        /// <param name="customer2">SAP ME中客户</param>
        /// <param name="customer2_id">SAP ME中客户代码</param>
        /// <returns></returns>
        public Tasks CreateTask(List<string> sfc, string category, string item_no, string customer, string created_user, string customer2, string customer2_id)
        {
            if (sfc == null) return null;
            if (sfc.Count == 0) return null;
            List<TaskSFC> SFCList = InitTaskSFC(sfc, category);
            if (SFCList == null) throw new Exception("");
            if (SFCList.Count == 0) throw new Exception("");
            mTask = new Tasks();
            for (int i = 0; i < SFCList.Count; ++i)
            {
                mTask.QTY_TOTAL += SFCList[i].QTY;
                SFCList[i].ITEM_NO = item_no;
                SFCList[i].CATEGORY = category;
                SFCList[i].CREATED_USER = created_user;
                SFCList[i].CUSTOMER = customer2;
                SFCList[i].CUSTOMER__ID = customer2_id;
            }
            mTask.HANDLE_CUSTOMER = HandleCustomer(customer);
            mTask.ITEM_NO = item_no;
            mTask.CREATED_USER = created_user;
            mTask.STATE = "启动";
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::CreateTask => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("INSERT INTO TASK (HANDLE_CUSTOMER,ITEM_NO,QTY_SFC,QTY_TOTAL,STATE,CREATED_USER,CREATED_DATE_TIME) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", mTask.HANDLE_CUSTOMER, mTask.ITEM_NO, SFCList.Count, mTask.QTY_TOTAL, mTask.STATE, mTask.CREATED_USER, mTask.CREATED_DATE_TIME), conn);
                    string handle_task = comm.ExecuteScalar().ToString();
                    if (string.IsNullOrEmpty(handle_task)) throw new Exception("TaskManagement::CreateTask => fail to create task.");
                    comm.CommandText = string.Format("INSERT INTO TASK_LOG (HANDLE_TASK,STATE) VALUES ('{0}','{1}');", handle_task, mTask.STATE);
                    comm.ExecuteNonQuery();
                    mTask.HANDLE = handle_task;
                    StringBuilder sql = new StringBuilder();
                    for (int i = 0; i < SFCList.Count; ++i)
                    {
                        sql.Append(string.Format("INSERT INTO TASK_DETAILS (HANDLE_TASK,ITEM_NO,SFC,CATEGORY,QTY,CREATED_USER,CREATED_DATE_TIME,CUSTOMER,CUSTOMER_ID) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", handle_task, SFCList[i].ITEM_NO, SFCList[i].SFC, SFCList[i].CATEGORY, SFCList[i].QTY, SFCList[i].CREATED_USER, SFCList[i].CREATED_DATE_TIME, SFCList[i].CUSTOMER, SFCList[i].CUSTOMER__ID));
                    }
                    SqlTransaction tran = conn.BeginTransaction();
                    comm.Transaction = tran;
                    comm.CommandText = sql.ToString();
                    if (comm.ExecuteNonQuery() != SFCList.Count)
                    {
                        tran.Rollback();
                        throw new Exception("TaskManagement::CreateTask => fail to create task details.");
                    }
                    tran.Commit();
                    return mTask;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 初始化唯一标识（确定栈板数量）
        /// </summary>
        /// <param name="sfcs">唯一标识（栈板编号）</param>
        /// <returns></returns>
        private List<TaskSFC> InitTaskSFC(List<string> sfcs, string category)
        {
            if (sfcs == null) return null;
            if (sfcs.Count == 0) return null;
            using (OdbcConnection conn = new OdbcConnection(Configuer.ConnectionStringBySAP))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::CreateTask => SyncRemote db can not be open.");
                    }
                    StringBuilder sfc_str = new StringBuilder();
                    for (int i = 0; i < sfcs.Count; ++i)
                    {
                        sfc_str.Append("'");
                        sfc_str.Append(sfcs[i]);
                        sfc_str.Append("',");
                    }
                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT NUM,TO_CHAR(QTY,0) QTY,ITEM FROM Z_CELL_PACK WHERE NUM IN ({0}) AND CATEGORY = '{1}';", sfc_str.ToString().Substring(0, sfc_str.ToString().Length - 1), category), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("No data is in the SAP ME database.");
                    List<TaskSFC> SFCList = new List<TaskSFC>();
                    while (reader.Read())
                    {
                        TaskSFC sfc = new TaskSFC();
                        sfc.SFC = reader["NUM"].ToString();
                        sfc.QTY = Convert.ToInt32(reader["QTY"].ToString());
                        sfc.ITEM_NO = reader["ITEM"].ToString();
                        SFCList.Add(sfc);
                    }
                    reader.Close();
                    return SFCList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 返回客户 HANDLE
        /// </summary>
        /// <param name="customer">客户名称</param>
        /// <returns></returns>
        private string HandleCustomer(string customer)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::HandleCustomer => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT HANDLE FROM CUSTOMER WHERE CUSTOMER = '{0}';", customer.ToUpper()), conn);
                    return comm.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="handle_task">任务 HANDLE 或者 SN</param>
        /// <param name="status">状态</param>
        /// <param name="taskno">实际各客户回传任务编号</param>
        /// <returns></returns>
        public Tasks UpdateTask(string handle_task, string status, string updated_user, string taskno)
        {
            try
            {
                if (string.IsNullOrEmpty(handle_task) || string.IsNullOrEmpty(status)) throw new Exception("TaskManagement::UpdateTask => Task handle or status is empty.");
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::UpdateTask => SyncRemote db can not be open.");
                    }
                    Tasks task = new Tasks();
                    string sql = null;
                    if (IsValidUuid(handle_task))
                        sql = string.Format("UPDATE TASK SET STATE = '{0}',UPDATED_USER = '{2}',UPDATED_DATE_TIME = '{3}',HANDLE_TASK = '{4}' WHERE HANDLE = '{1}';", status, handle_task, task.UPDATED_USER, task.UPDATED_DATE_TIME, taskno);
                    else
                        sql = string.Format("UPDATE TASK SET STATE = '{0}',UPDATED_USER = '{2}',UPDATED_DATE_TIME = '{3}',HANDLE_TASK = '{4}' WHERE SN = '{1}';", status, handle_task, task.UPDATED_USER, task.UPDATED_DATE_TIME, taskno);
                    SqlCommand comm = new SqlCommand(sql, conn);
                    if (comm.ExecuteNonQuery() != 1)
                    {
                        throw new Exception("TaskManagement::UpdateTask => fail to update task.");
                    }
                    comm.CommandText = string.Format("INSERT INTO TASK_LOG (HANDLE_TASK,STATE,CREATED_USER) VALUES ('{0}','{1}','{2}');", handle_task, status, updated_user);
                    comm.ExecuteNonQuery();
                    comm.CommandText = string.Format("SELECT * FROM TASK WHERE HANDLE = '{0}';", handle_task);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception("TaskManagement::UpdateTask => the task is not exist.");
                    }
                    while (reader.Read())
                    {
                        task.HANDLE = reader["HANDLE"].ToString();
                        task.HANDLE_CUSTOMER = reader["HANDLE_CUSTOMER"].ToString();
                        task.ITEM_NO = reader["ITEM_NO"].ToString();
                        task.QTY_TOTAL = Convert.ToInt32(reader["QTY_TOTAL"].ToString());
                        task.STATE = reader["STATE"].ToString();
                        task.CREATED_USER = reader["CREATED_USER"].ToString();
                        task.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                    }
                    reader.Close();
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取指定用户 ID 的邮件地址
        /// </summary>
        /// <param name="customerid"></param>
        /// <returns></returns>
        public List<EmailInfo> GetEmails(string handle_customer)
        {
            if (string.IsNullOrEmpty(handle_customer)) return null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::EmailAddress => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT C.HANDLE,C.CUSTOMER,EC.USERNAME,EMAIL FROM EMAIL_GROUP eg INNER JOIN EMAIL_CONFIG ec ON EG.HANDLE_EMAIL = EC.HANDLE INNER JOIN CUSTOMER c ON EG.HANDLE_CUSTOMER = C.HANDLE WHERE C.HANDLE = '{0}';", handle_customer), conn);
                    List<EmailInfo> emailList = new List<EmailInfo>();
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        EmailInfo info = new EmailInfo();
                        info.CUSTOMER_ID = reader["HANDLE"].ToString();
                        info.CUSTOMER = reader["CUSTOMER"].ToString();
                        info.USERNAME = reader["USERNAME"].ToString();
                        info.EMAIL = reader["EMAIL"].ToString();
                        emailList.Add(info);
                    }
                    return emailList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 创建 Schedule 组
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <param name="Category_schedule">Schedule类型</param>
        /// <returns></returns>
        public Tasks CreateSchedule(Tasks task, string Category_schedule)
        {
            try
            {
                if (task == null || string.IsNullOrEmpty(Category_schedule)) throw new Exception("TaskManagement::CreateSchedule => Task or Category_schedule is empty.");
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::CreateSchedule => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT HANDLE FROM TASK_SCHEDULE WHERE STATE IS NULL AND CATEGORY = '{0}';", Category_schedule), conn);
                    string handle_schedule = comm.ExecuteScalar().ToString();
                    if (string.IsNullOrEmpty(handle_schedule)) throw new Exception("TaskManagement::CreateSchedule => handle_schedule is empty.");
                    comm.CommandText = string.Format("INSERT INTO SCHEDULE_GROUP (HANDLE_SCHEDULE,HANDLE_TASK,CREATED_USER,CREATED_DATE_TIME) OUTPUT INSERTED.HANDLE VALUES ('{0}','{1}','{2}','{3}');", handle_schedule, task.HANDLE, task.CREATED_USER, task.CREATED_DATE_TIME);
                    string handle_schedule_group = comm.ExecuteScalar().ToString();
                    if (string.IsNullOrEmpty(handle_schedule_group)) throw new Exception("TaskManagement::CreateSchedule => fail to create schedule_group.");
                    task.HANDLE_SCHEDULE_GROUP = handle_schedule_group;
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取任务通过任务 HANDLE
        /// </summary>
        /// <param name="handle_task">任务 HANDLE 或者 SN</param>
        /// <returns></returns>
        public Tasks GetTask(string handle_task)
        {
            try
            {
                if (string.IsNullOrEmpty(handle_task)) throw new Exception("TaskManagement::GetTask => Handle task is empty.");
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::CreateSchedule => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT tk.HANDLE HANDLE_TASK,sg.HANDLE HANDLE_SCHEDULE_GROUP,c.HANDLE HANDLE_CUSTOMER,tk.STATE STATE_TASK,tk.ITEM_NO,tk.QTY_TOTAL,tk.CREATED_USER,tk.CREATED_DATE_TIME,tk.UPDATED_USER,tk.UPDATED_DATE_TIME,C.CUSTOMER,TK.SN FROM TASK tk INNER JOIN SCHEDULE_GROUP sg ON tk.HANDLE = sg.HANDLE_TASK AND sg.STATE IS NULL INNER JOIN CUSTOMER c ON tk.HANDLE_CUSTOMER = c.HANDLE WHERE tk.HANDLE = '{0}';", handle_task), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("TaskManagement::GetTask => Task is not exist.");
                    Tasks task = new Tasks();
                    while (reader.Read())
                    {
                        task.HANDLE = reader["HANDLE_TASK"].ToString();
                        task.HANDLE_SCHEDULE_GROUP = reader["HANDLE_SCHEDULE_GROUP"].ToString();
                        task.HANDLE_CUSTOMER = reader["HANDLE_CUSTOMER"].ToString();
                        task.STATE = reader["STATE_TASK"].ToString();
                        task.ITEM_NO = reader["ITEM_NO"].ToString();
                        task.QTY_TOTAL = Convert.ToInt32(reader["QTY_TOTAL"]);
                        task.CREATED_USER = reader["CREATED_USER"].ToString();
                        task.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                        task.UPDATED_USER = reader["UPDATED_USER"].ToString();
                        task.UPDATED_DATE_TIME = reader["UPDATED_DATE_TIME"].ToString();
                        task.CUSTOMER = reader["CUSTOMER"].ToString();
                        task.SN = reader["SN"].ToString();
                    }
                    reader.Close();
                    comm.CommandText = string.Format("SELECT * FROM TASK_DETAILS td WHERE STATE IS NULL AND HANDLE_TASK = '{0}';", task.HANDLE);
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        TaskSFC sfc = new TaskSFC();
                        sfc.HANDLE = reader["HANDLE"].ToString();
                        sfc.HANDLE_TASK = reader["HANDLE_TASK"].ToString();
                        sfc.ITEM_NO = reader["ITEM_NO"].ToString();
                        sfc.QTY = Convert.ToInt32(reader["QTY"]);
                        sfc.STATE = reader["STATE"].ToString();
                        sfc.CATEGORY = reader["CATEGORY"].ToString();
                        sfc.CREATED_USER = reader["CREATED_USER"].ToString();
                        sfc.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                        task.SFC_LIST.Add(sfc);
                    }
                    reader.Close();
                    return task;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 回传任务管理中已启动但未确认的回传任务
        /// </summary>
        /// <returns></returns>
        public Hashtable GetTaskList(string customerid)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetTask => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT SN,T.HANDLE,C.CUSTOMER,ITEM_NO,QTY_SFC,QTY_TOTAL,CREATED_USER,T.CREATED_DATE_TIME FROM TASK T INNER JOIN CUSTOMER C ON T.HANDLE_CUSTOMER = C.HANDLE WHERE STATE = '启动' AND C.HANDLE = '{0}' ORDER BY CREATED_DATE_TIME DESC;", customerid), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("TaskManagement::GetTask => Task is empty.");
                    Hashtable hashTasks = new Hashtable();
                    while (reader.Read())
                    {
                        TaskSimple task = new TaskSimple();
                        task.sn = reader["SN"].ToString();
                        task.handle = reader["HANDLE"].ToString();
                        task.created_user = reader["CREATED_USER"].ToString();
                        task.created_date_time = reader["CREATED_DATE_TIME"].ToString();
                        task.customer = reader["CUSTOMER"].ToString();
                        task.item_no = reader["ITEM_NO"].ToString();
                        task.qty_sfc = reader["QTY_SFC"].ToString();
                        task.qty = reader["QTY_TOTAL"].ToString();
                        hashTasks.Add(task.sn, task);
                    }
                    return hashTasks;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 返回客户信息（回传系统中的客户，非 SAP ME 中的客户）
        /// </summary>
        /// <returns></returns>
        public Hashtable GetCustomer()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetCustomer => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT HANDLE,CUSTOMER FROM CUSTOMER ORDER BY HANDLE DESC;", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("TaskManagement::GetCustomer => Customer is empty.");
                    Hashtable hashCustomer = new Hashtable();
                    while (reader.Read())
                    {
                        hashCustomer.Add(reader["CUSTOMER"], reader["HANDLE"]);
                    }
                    return hashCustomer;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取栈板信息
        /// </summary>
        /// <param name="trayno">栈板编号</param>
        /// <returns></returns>
        public Tray GetTray(string itemno, string trayno)
        {
            if (string.IsNullOrEmpty(trayno)) throw new Exception("TaskManagement::GetTray => Tray no is empty.");
            using (OdbcConnection conn = new OdbcConnection(Configuer.ConnectionStringBySAP))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetTray => SyncRemote db can not be open.");
                    }
                    OdbcCommand comm = new OdbcCommand(string.Format("SELECT ITEM,ZCP.CUSTOMER,C.CUSTOMER_NAME,NUM,TO_CHAR(QTY,0) QTY,ZCP.CREATE_USER||'/'||ZUE.USER_NAME CREATE_USER,ZCP.CREATED_DATE_TIME FROM Z_CELL_PACK ZCP INNER JOIN CUSTOMER C ON ZCP.CUSTOMER = C.CUSTOMER INNER JOIN Z_USER_EXTEND ZUE ON ZUE.USER_NO = ZCP.CREATE_USER WHERE CATEGORY = 'TRAY' AND STATUS = 'CLOSE' AND NUM = '{0}' AND ZCP.ITEM = '{1}';", trayno, itemno), conn);
                    OdbcDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("TaskManagement::GetTray => Tray no is empty.");
                    reader.Read();
                    Tray tray = new Tray();
                    tray.ITEM_NO = reader["ITEM"].ToString();
                    tray.CUSTOMER_ID = reader["CUSTOMER"].ToString();
                    tray.CUSTOMER = reader["CUSTOMER_NAME"].ToString();
                    tray.TRAY_NO = reader["NUM"].ToString();
                    tray.QTY = reader["QTY"].ToString();
                    tray.CREATED_USER = reader["CREATE_USER"].ToString();
                    tray.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                    reader.Close();
                    return tray;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取用户数量
        /// </summary>
        /// <returns></returns>
        public string GetUsers()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetUsers => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT COUNT(1) FROM EMAIL_CONFIG;", conn);
                    string result = comm.ExecuteScalar().ToString();
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
        /// 获取客户数量
        /// </summary>
        /// <returns></returns>
        public string GetCustomers()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetUsers => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT COUNT(1) FROM CUSTOMER;", conn);
                    string result = comm.ExecuteScalar().ToString();
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
        /// 拼接用户数据表
        /// </summary>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        protected Hashtable GetCustomerTable(string tablename)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetCustomerTable => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand("SELECT CUSTOMER FROM CUSTOMER;", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("TaskManagement::GetCustomerTable => Customers is empty.");
                    Hashtable Customers = new Hashtable();
                    Hashtable newCustomers = new Hashtable();
                    while (reader.Read())
                    {
                        try
                        {
                            if (reader["CUSTOMER"].ToString().Split('-')[0] == "测试客户" || reader["CUSTOMER"].ToString().Split('-')[0] == "POLE") continue;
                            Customers.Add(reader["CUSTOMER"].ToString().Split('-')[0], tablename);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    reader.Close();
                    foreach (DictionaryEntry entry in Customers)
                    {
                        try
                        {
                            comm.CommandText = string.Format("SELECT NAME FROM SYS.TABLES WHERE NAME = '{0}_{1}';", entry.Key.ToString(), entry.Value.ToString());
                            string table = comm.ExecuteScalar().ToString();
                            if (!string.IsNullOrEmpty(table))
                            {
                                newCustomers.Add(entry.Key, table);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    return newCustomers;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取已回传总数量
        /// </summary>
        /// <returns></returns>
        public string GetSum(DateTime dt)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetSum => SyncRemote db can not be open.");
                    }

                    Hashtable table = GetCustomerTable("SYNCBATTERY_LOG");
                    int sum = 0;
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    foreach (DictionaryEntry entry in table)
                    {
                        if (entry.Value == null) continue;
                        if (dt == DateTime.MinValue)
                            comm.CommandText = string.Format("SELECT COUNT(1) FROM {0};", entry.Value.ToString());
                        else
                            comm.CommandText = string.Format("SELECT COUNT(1) FROM {0} WHERE CONVERT(NVARCHAR(10),CREATED_DATE_TIME,23) = '{1}';", entry.Value.ToString(), dt.Date.ToString("yyyy-MM-dd"));
                        sum += Convert.ToInt32(comm.ExecuteScalar());
                    }
                    return sum.ToString();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取最新回传任务的客户
        /// </summary>
        /// <returns></returns>
        public string GetCurrentCustomer()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetCurrentCustomer => SyncRemote db can not be open.");
                    }
                    Hashtable table = GetCustomerTable("TASK");
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    DateTime dt = DateTime.MinValue;
                    string customer = null;
                    foreach (DictionaryEntry entry in table)
                    {
                        if (entry.Value == null) continue;
                        comm.CommandText = string.Format("SELECT TOP 1 CREATED_DATE_TIME FROM {0} ORDER BY CREATED_DATE_TIME DESC;", entry.Value.ToString());
                        DateTime dtTemp = Convert.ToDateTime(comm.ExecuteScalar());
                        if (dt < dtTemp)
                        {
                            dt = dtTemp;
                            customer = entry.Key.ToString();
                        }
                    }
                    return customer;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 统计每日（24h）回传数量
        /// </summary>
        /// <returns></returns>
        public string GetQtyOfDay()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetQtyOfDay => SyncRemote db can not be open.");
                    }
                    Hashtable table = GetCustomerTable("SYNCBATTERY_LOG");
                    int sum = 0;
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    foreach (DictionaryEntry entry in table)
                    {
                        if (entry.Value == null) continue;
                        comm.CommandText = string.Format("SELECT COUNT(1) FROM {0} WHERE CREATED_DATE_TIME > CONVERT(NVARCHAR(10),GETDATE(),23);", entry.Value.ToString());
                        sum += Convert.ToInt32(comm.ExecuteScalar());
                    }
                    return sum.ToString();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取不同客户的回传数据量
        /// </summary>
        /// <returns></returns>
        public Hashtable GetQtyOfCustomers()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetQtyOfCustomers => SyncRemote db can not be open.");
                    }
                    Hashtable table = GetCustomerTable("SYNCBATTERY_LOG");
                    Hashtable hashQty = new Hashtable();
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    foreach (DictionaryEntry entry in table)
                    {
                        comm.CommandText = string.Format("SELECT COUNT(1) FROM {0};", entry.Value.ToString());
                        hashQty.Add(entry.Key, comm.ExecuteScalar());
                    }
                    return hashQty;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="datetime">任务创建日期</param>
        /// <param name="tray">栈板编号</param>
        /// <param name="barcode">电池码号</param>
        /// <returns></returns>
        public List<Tasks> GetTasks(string datetime, string tray, string barcode, string customer, string taskno)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetTasks => SyncRemote db can not be open.");
                    }

                    //barcode = AdapterCondition(barcode, "string");
                    string _sql = "SELECT T.HANDLE,T.HANDLE_SYNC,T.SN,C.CUSTOMER,T.ITEM_NO,QTY_SFC,QTY_TOTAL,QTY_SEND,TD.HANDLE HANDLE2,TD.SFC,TD.QTY,T.STATE,TD.CUSTOMER_ID CUSTOMER2_ID,TD.CUSTOMER CUSTOMER2,T.CREATED_USER,T.CREATED_DATE_TIME,TD.CREATED_USER CREATED_USER2,TD.CREATED_DATE_TIME CREATED_DATE_TIME2 FROM TASK T INNER JOIN TASK_DETAILS TD ON T.HANDLE = TD.HANDLE_TASK INNER JOIN CUSTOMER C ON T.HANDLE_CUSTOMER = C.HANDLE WHERE {0} T.STATE <> '结束' ";
                    if (string.IsNullOrEmpty(customer))
                        _sql = string.Format(_sql, "");
                    else
                        _sql = string.Format(_sql, "C.CUSTOMER = '" + customer + "' AND ");
                    StringBuilder sql = new StringBuilder(_sql);
                    datetime = AdapterCondition("T.CREATED_DATE_TIME", datetime, "datetime");
                    tray = AdapterCondition("SFC", tray, "string");

                    if (!string.IsNullOrEmpty(datetime))
                    {
                        sql.Append(" AND ");
                        sql.Append(datetime);
                    }
                    if (!string.IsNullOrEmpty(tray))
                    {
                        sql.Append(" AND ");
                        sql.Append(tray);
                    }
                    if (!string.IsNullOrEmpty(taskno))
                    {
                        sql.Append(" AND T.SN = ");
                        sql.Append(taskno);
                    }
                    sql.Append(" ORDER BY SN DESC;");
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("TaskManagement::GetTasks => Task is empty.");
                    List<Tasks> taskList = new List<Tasks>();
                    Tasks task = null;
                    Hashtable hashTask = new Hashtable();
                    while (reader.Read())
                    {
                        if (!hashTask.ContainsKey(reader["SN"]))
                        {
                            task = new Tasks();
                            task.SN = reader["SN"].ToString();
                            task.CUSTOMER = reader["CUSTOMER"].ToString();
                            task.ITEM_NO = reader["ITEM_NO"].ToString();
                            task.QTY_TOTAL = Convert.ToInt32(reader["QTY_TOTAL"]);
                            task.HANDLE = reader["HANDLE"].ToString();
                            task.HANDLE_SYNC = reader["HANDLE_SYNC"].ToString();
                            task.STATE = reader["STATE"].ToString();
                            task.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                            task.CREATED_USER = reader["CREATED_USER"].ToString();
                            hashTask.Add(reader["SN"], null);
                            taskList.Add(task);
                        }
                        TaskSFC sfc = new TaskSFC();
                        sfc.HANDLE = reader["HANDLE2"].ToString();
                        sfc.SFC = reader["SFC"].ToString();
                        sfc.HANDLE_TASK = reader["HANDLE"].ToString();
                        sfc.ITEM_NO = reader["ITEM_NO"].ToString();
                        sfc.QTY = Convert.ToInt32(reader["QTY"]);
                        sfc.CUSTOMER = reader["CUSTOMER2"].ToString();
                        sfc.CUSTOMER__ID = reader["CUSTOMER2_ID"].ToString();
                        sfc.CREATED_USER = reader["CREATED_USER"].ToString();
                        sfc.CREATED_DATE_TIME = reader["CREATED_DATE_TIME2"].ToString();
                        sfc.CREATED_USER = reader["CREATED_USER2"].ToString();
                        task.SFC_LIST.Add(sfc);
                    }
                    return taskList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 适配参数
        /// </summary>
        /// <param name="field">表中字段名</param>
        /// <param name="para">参数必须是字符串，且中间间隔使用逗号（,）</param>
        /// <param name="type">只分日期类型（datetime）与字符串类型（string）</param>
        /// <returns></returns>
        private string AdapterCondition(string field, string para, string type)
        {
            if (string.IsNullOrEmpty(para) || string.IsNullOrEmpty(type))
            {
                return null;
            }
            string result = null;
            switch (type)
            {
                case "datetime":
                    string[] datetime = para.Split(',');
                    if (datetime.Length != 2) return null;
                    result = " " + field + " BETWEEN '" + datetime[0] + "' AND '" + datetime[1] + "' ";
                    break;
                case "string":
                    StringBuilder condition = new StringBuilder();
                    string[] paras = para.Split(',');
                    foreach (string t in paras)
                    {
                        condition.Append("'");
                        condition.Append(t);
                        condition.Append("',");
                    }
                    result += " " + field + " IN (" + condition.ToString().Substring(0, condition.Length - 1) + ") ";
                    condition.Clear();
                    break;
            }
            return result;
        }
        /// <summary>
        /// 返回物料信息，如果不存在则添加
        /// </summary>
        /// <param name="itemno">料号</param>
        /// <param name="bomno">型号</param>
        /// <returns></returns>
        public List<Items> ItemsInfo(string itemno, string bomno, string customer)
        {
            string sql = "SELECT DISTINCT HANDLE,ITEM,BOMNO,CUSTOMER,CREATED_USER,CREATED_DATE_TIME FROM ITEM WHERE STATE = '1' ";
            sql = !string.IsNullOrEmpty(itemno) ? sql + string.Format(" AND ITEM = '{0}' ", itemno) : sql;
            sql = !string.IsNullOrEmpty(bomno) ? sql + string.Format(" AND BOMNO = '{0}'", bomno) : sql;
            sql = !string.IsNullOrEmpty(customer) ? sql + string.Format(" AND CUSTOMER = '{0}'", customer) : sql;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetItems => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    List<Items> itemList = new List<Items>();
                    if (!reader.HasRows && !string.IsNullOrEmpty(itemno) && !string.IsNullOrEmpty(bomno) && !string.IsNullOrEmpty(customer))
                    {
                        reader.Close();
                        comm.CommandText = string.Format("INSERT INTO ITEM (ITEM,BOMNO,CUSTOMER,CREATED_USER,STATE) VALUES ('{0}','{1}','{2}','ADMIN_Z','1');", itemno, bomno, customer);
                        comm.ExecuteNonQuery();
                        comm.CommandText = "SELECT HANDLE,ITEM,BOMNO,CUSTOMER,CREATED_USER,CREATED_DATE_TIME FROM ITEM;";
                        reader = comm.ExecuteReader();
                    }
                    while (reader.Read())
                    {
                        Items item = new Items();
                        item.HANDLE = reader["HANDLE"].ToString();
                        item.BOMNO = reader["BOMNO"].ToString();
                        item.ITEM = reader["ITEM"].ToString();
                        item.CUSTOMER = reader["CUSTOMER"].ToString();
                        item.CREATED_USER = reader["CREATED_USER"].ToString();
                        item.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                        itemList.Add(item);
                    }
                    reader.Close();
                    return itemList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="itemno">料号</param>
        /// <param name="bomno">型号</param>
        /// <returns></returns>
        public List<DataSources> DataSources(string itemno, string bomno)
        {
            if (string.IsNullOrEmpty(itemno) && string.IsNullOrEmpty(bomno)) return null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::DataSources => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT DISTINCT DS.HANDLE,HANDLE_ITEM,I.ITEM,I.BOMNO,DATATABLE,FIELD,DATA_PROPERTY_NAME,PARAMETER,OPERATORS,PRECISION,LSL,USL,UNIT,DS.REMARKS,PIPELINE,DS.CREATED_USER,DS.CREATED_DATE_TIME FROM DATA_SOURCES DS INNER JOIN ITEM i ON DS.HANDLE_ITEM = I.HANDLE WHERE (I.ITEM = '{0}' OR I.BOMNO = '{1}') AND I.STATE = '1' AND DS.STATE = '1';", itemno, bomno), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    List<DataSources> dsList = new List<global::DataSources>();
                    while (reader.Read())
                    {
                        DataSources ds = new DataSources();
                        ds.HANDLE = reader["HANDLE"].ToString();
                        ds.HANDLE_ITEM = reader["HANDLE_ITEM"].ToString();
                        ds.DATATABLE = reader["DATATABLE"].ToString();
                        ds.FIELD = reader["FIELD"].ToString();
                        ds.DATA_PROPERTY_NAME = reader["DATA_PROPERTY_NAME"].ToString();
                        ds.PARAMETER = reader["PARAMETER"].ToString();
                        ds.OPERATORS = reader["OPERATORS"].ToString();
                        ds.PRECISION = reader["PRECISION"].ToString();
                        ds.LSL = reader["LSL"].ToString();
                        ds.USL = reader["USL"].ToString();
                        ds.UNIT = reader["UNIT"].ToString();
                        ds.REMARKS = reader["REMARKS"].ToString();
                        ds.PIPELINE = reader["PIPELINE"].ToString();
                        ds.CREATED_USER = reader["CREATED_USER"].ToString();
                        ds.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                        ds.ITEM = reader["ITEM"].ToString();
                        ds.BOMNO = reader["BOMNO"].ToString();
                        dsList.Add(ds);
                    }
                    reader.Close();
                    return dsList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取操作编号及说明
        /// </summary>
        /// <param name="pipeline">WJ1：甲一系统；WJ2：甲二甲三系统</param>
        /// <returns></returns>
        public Hashtable Operations(string pipeline)
        {
            if (string.IsNullOrEmpty(pipeline)) return null;
            Hashtable hashOperations = new Hashtable();
            string sql = "SELECT zccc.OPERATION,O.DESCRIPTION FROM Z_CO_CALCULATE_COLUMN zccc INNER JOIN OPERATION o ON ZCCC.OPERATION = O.OPERATION WHERE zccc.OPERATION LIKE 'OP%' GROUP BY zccc.OPERATION,O.DESCRIPTION ORDER BY zccc.OPERATION ASC;";
            OdbcDataReader reader;
            if (pipeline == "WJ1")
            {
                using (OdbcConnection conn = new OdbcConnection(Configuer.ConnectionStringBySAP_ME))
                {
                    try
                    {
                        conn.Open();
                        if (conn.State != ConnectionState.Open)
                        {
                            throw new Exception("TaskManagement::Operations => SyncRemote db can not be open.");
                        }
                        OdbcCommand comm = new OdbcCommand(sql, conn);
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            hashOperations.Add(reader["OPERATION"] + " / " + reader["DESCRIPTION"], reader["OPERATION"]);
                        }
                        reader.Close();

                    }
                    catch (Exception ex)
                    {
                        SysLog log = new SysLog(ex.Message);
                        return null;
                    }
                }
            }
            else
            {
                using (OdbcConnection conn = new OdbcConnection(Configuer.ConnectionStringBySAP))
                {
                    try
                    {
                        conn.Open();
                        if (conn.State != ConnectionState.Open)
                        {
                            throw new Exception("TaskManagement::Operations => SyncRemote db can not be open.");
                        }
                        OdbcCommand comm = new OdbcCommand(sql, conn);
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            hashOperations.Add(reader["OPERATION"] + " / " + reader["DESCRIPTION"], reader["OPERATION"]);
                        }
                        reader.Close();
                        reader = comm.ExecuteReader();

                    }
                    catch (Exception ex)
                    {
                        SysLog log = new SysLog(ex.Message);
                        return null;
                    }
                }
            }
            hashOperations.Add("包装", "A");
            return hashOperations;
        }
        /// <summary>
        /// 获取指定工序的数据字段
        /// </summary>
        /// <param name="pipeline">WJ1：甲一系统；WJ2：甲二甲三系统</param>
        /// <param name="operation">工序</param>
        /// <returns></returns>
        public Hashtable Field(string pipeline, string operation)
        {
            if (string.IsNullOrEmpty(pipeline)) return null;
            Hashtable hashField = new Hashtable();
            if (operation == "A")
            {
                hashField.Add("包装箱号", "BOXID");
                hashField.Add("栈板号", "PALLETNO");
                return hashField;
            }
            string sql = string.Format("SELECT COLUMN_NAME,COLUMN_DESC FROM Z_CO_CALCULATE_COLUMN WHERE OPERATION = '{0}' AND CATEGORY = 'SFC' ORDER BY SEQ ASC;", operation);
            OdbcDataReader reader;
            if (pipeline == "WJ1")
            {
                using (OdbcConnection conn = new OdbcConnection(Configuer.ConnectionStringBySAP_ME))
                {
                    try
                    {
                        conn.Open();
                        if (conn.State != ConnectionState.Open)
                        {
                            throw new Exception("TaskManagement::Operations => SyncRemote db can not be open.");
                        }
                        OdbcCommand comm = new OdbcCommand(sql, conn);
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            hashField.Add(reader["COLUMN_NAME"] + " / " + reader["COLUMN_DESC"], reader["COLUMN_NAME"]);
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        SysLog log = new SysLog(ex.Message);
                        return null;
                    }
                }
            }
            else
            {
                using (OdbcConnection conn = new OdbcConnection(Configuer.ConnectionStringBySAP))
                {
                    try
                    {
                        conn.Open();
                        if (conn.State != ConnectionState.Open)
                        {
                            throw new Exception("TaskManagement::Operations => SyncRemote db can not be open.");
                        }
                        OdbcCommand comm = new OdbcCommand(sql, conn);
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            hashField.Add(reader["COLUMN_NAME"] + " / " + reader["COLUMN_DESC"], reader["COLUMN_NAME"]);
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        SysLog log = new SysLog(ex.Message);
                        return null;
                    }
                }
            }
            return hashField;
        }
        /// <summary>
        /// 获取补偿操作符
        /// </summary>
        /// <returns></returns>
        public Hashtable Operators()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::Operators => SyncRemote db can not be open.");
                    }
                    Hashtable hashOperator = new Hashtable();
                    SqlCommand comm = new SqlCommand("SELECT DESCRIPTION,HANDLE FROM OPERATORS;", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        hashOperator.Add(reader["DESCRIPTION"], reader["HANDLE"]);
                    }
                    reader.Close();
                    return hashOperator;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 创建DataSource
        /// </summary>
        /// <param name="ds">DataSource对象</param>
        /// <returns></returns>
        public ResultTask CreateDataSource(DataSources ds)
        {
            if (ds == null) throw new Exception("\nNon-retrieval by ITEM");
            if (string.IsNullOrEmpty(ds.HANDLE))
            {
                ds.HANDLE_ITEM = GetHandleItem(ds.ITEM);
            }
            ds.STATE = "1";
            ResultTask result = new ResultTask();
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::CreateDataSource => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT COUNT(1) FROM DATA_SOURCES WHERE HANDLE_ITEM = '{0}' AND DATA_PROPERTY_NAME = '{1}' AND STATE = '1';", ds.HANDLE_ITEM, ds.DATA_PROPERTY_NAME), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("\nfail to query datasources.");
                    reader.Read();
                    if (reader[0].ToString() != "0")    //  已存在数据，将 STATE 更新为 0
                    {
                        if (!reader.IsClosed) reader.Close();
                        comm.CommandText = string.Format("UPDATE DATA_SOURCES SET STATE = '0' WHERE HANDLE_ITEM = '{0}' AND DATA_PROPERTY_NAME = '{1}';", ds.HANDLE_ITEM, ds.DATA_PROPERTY_NAME);
                        comm.ExecuteNonQuery();
                    }
                    StringBuilder fields = new StringBuilder();
                    StringBuilder values = new StringBuilder();
                    string pipeline = null;
                    foreach (PropertyInfo item in ds.GetType().GetProperties())
                    {
                        if (item.Name == "HANDLE" || item.Name == "ITEM" || item.Name == "BOMNO" || item.Name == "CREATED_DATE_TIME")
                        {
                            continue;
                        }
                        else
                        {
                            fields.Append(item.Name);
                            fields.Append(",");
                            if (item.GetValue(ds) == null)
                            {
                                values.Append("NULL");
                            }
                            else
                            {
                                if (item.Name == "DATATABLE")
                                {
                                    if (item.GetValue(ds).ToString() != "")
                                    {
                                        if (!item.GetValue(ds).ToString().Contains("_SFC_PARAM") && !item.GetValue(ds).ToString().Contains("A"))
                                        {
                                            values.Append("'Z_");
                                            values.Append(item.GetValue(ds));
                                            values.Append("_SFC_PARAM'");
                                        }
                                        else
                                        {
                                            values.Append("'" + item.GetValue(ds) + "'");
                                        }
                                    }
                                    else
                                    {
                                        values.Append("NULL");
                                    }
                                }
                                else if (item.Name.ToUpper() == "REMARKS" && item.GetValue(ds).ToString() == "INDEX")
                                {
                                    values.Append("'INDEX',");
                                    fields.Append("LOGIC");
                                    fields.Append(",");
                                    values.Append("'");
                                    values.Append(GetIndexHandle(ds.PIPELINE));
                                    values.Append("'");
                                }
                                else
                                {
                                    values.Append("'");
                                    values.Append(item.GetValue(ds));
                                    values.Append("'");
                                }
                            }
                            values.Append(",");
                        }
                    }
                    if (!reader.IsClosed) reader.Close();
                    comm.CommandText = "INSERT INTO DATA_SOURCES (" + fields.ToString().Substring(0, fields.Length - 1) + ") VALUES (" + values.ToString().Substring(0, values.Length - 1) + ");";
                    if (comm.ExecuteNonQuery() == 1)
                    {
                        result.RESULT = "success";
                        result.MSG = "Succeeded in creating datasources record.";
                    }
                    else
                    {
                        throw new Exception(string.Format("\nFailed to write the data table.\n{0}", comm.CommandText));
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.RESULT = "fail";
                    result.MSG = string.Format("TaskManagement::CreateDataSource => fail to create datasource record.{0}", ex.Message);
                    return result;
                }
            }
        }
        /// <summary>
        /// 获取物料HANDLE
        /// </summary>
        /// <param name="item">料号</param>
        /// <returns></returns>
        protected string GetHandleItem(string item)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetHandleItem => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT CONVERT(NVARCHAR(50),HANDLE) FROM ITEM WHERE STATE = '1' AND ITEM = '{0}'", item), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    reader.Read();
                    return reader[0].ToString();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="item">料号</param>
        /// <param name="bomno">型号</param>
        /// <returns></returns>
        public List<DataTemplate> GetTemplate(string item, string bomno)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetTemplate => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT DT.HANDLE,HANDLE_ITEM,HEADLINE,SN,DT.STATE,DT.REMARKS,DT.CREATED_USER,DT.CREATED_DATE_TIME FROM DATA_TEMPLATE DT INNER JOIN ITEM I ON DT.HANDLE_ITEM = I.HANDLE WHERE (I.ITEM = '{0}' OR I.BOMNO = '{1}') AND I.STATE = '1' AND DT.STATE = '1' ORDER BY SN DESC;", item, bomno), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    List<DataTemplate> dtList = new List<DataTemplate>();
                    while (reader.Read())
                    {
                        DataTemplate template = new DataTemplate();
                        template.HANDLE = reader["HANDLE"].ToString();
                        template.HANDLE_ITEM = reader["HANDLE_ITEM"].ToString();
                        template.HEADLINE = reader["HEADLINE"].ToString();
                        template.SN = reader["SN"].ToString();
                        template.STATE = reader["STATE"].ToString();
                        template.REMARKS = reader["REMARKS"].ToString();
                        template.CREATED_USER = reader["CREATED_USER"].ToString();
                        template.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                        dtList.Add(template);
                    }
                    return dtList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="dt">模板对象</param>
        /// <returns></returns>
        public ResultTask CreateDataTemplate(DataTemplate dt)
        {
            ResultTask result = new ResultTask();
            try
            {
                if (dt == null) new Exception("TaskManagement::CreateDataTemplate => DataTemplate object is empty.");
                dt.HANDLE_ITEM = GetHandleItem(dt.HANDLE_ITEM);
                if (string.IsNullOrEmpty(dt.HANDLE_ITEM)) new Exception("TaskManagement::CreateDataTemplate => Item is not exist.");
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::CreateDataTemplate => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT COUNT(1) FROM DATA_TEMPLATE WHERE STATE = '1' AND HANDLE_ITEM = '{0}' AND HEADLINE = '{1}';", dt.HANDLE_ITEM, dt.HEADLINE), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    reader.Read();
                    if (reader[0].ToString() != "0")
                    {
                        if (!reader.IsClosed) reader.Close();
                        comm.CommandText = string.Format("UPDATE DATA_TEMPLATE SET STATE = '0' WHERE HANDLE_ITEM = '{0}' AND HEADLINE = '{1}';", dt.HANDLE_ITEM, dt.HEADLINE);
                        comm.ExecuteNonQuery();
                    }
                    StringBuilder fields = new StringBuilder();
                    StringBuilder values = new StringBuilder();
                    foreach (PropertyInfo item in dt.GetType().GetProperties())
                    {
                        if (item.Name == "HANDLE" || item.Name == "CREATED_DATE_TIME")
                        {
                            continue;
                        }
                        else
                        {
                            fields.Append(item.Name);
                            fields.Append(",");
                            if (item.GetValue(dt) == null)
                            {
                                values.Append("NULL");
                            }
                            else
                            {
                                values.Append("'");
                                values.Append(item.GetValue(dt));
                                values.Append("'");
                            }
                            values.Append(",");
                        }
                    }
                    if (!reader.IsClosed) reader.Close();
                    comm.CommandText = "INSERT INTO DATA_TEMPLATE (" + fields.ToString().Substring(0, fields.Length - 1) + ") VALUES (" + values.ToString().Substring(0, values.Length - 1) + ");";
                    if (comm.ExecuteNonQuery() == 1)
                    {
                        result.RESULT = "success";
                        result.MSG = "Succeeded in creating datatemplate record.";
                    }
                    else
                    {
                        throw new Exception(string.Format("\nFailed to write the data table.\n{0}", comm.CommandText));
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                result.RESULT = "fail";
                result.MSG = string.Format("TaskManagement::CreateDataTemplate => fail to create datatemplate record.{0}", ex.Message);
                return result;
            }
        }
        /// <summary>
        /// 获取索引
        /// </summary>
        /// <returns></returns>
        protected string GetIndexHandle(string pipeline)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    throw new Exception("TaskManagement::GetIndex => SyncRemote db can not be open.");
                }
                SqlCommand comm = new SqlCommand(string.Format("SELECT CONVERT(NVARCHAR(50),HANDLE) FROM LOGIC WHERE PIPELINE = '{0}' AND STATE = '1';", pipeline), conn);
                SqlDataReader reader = comm.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    return reader[0].ToString();
                }
                return null;
            }
        }
        /// <summary>
        /// 只进行统计与通知动作，不负责状态更新
        /// </summary>
        /// <param name="taskno">任务编号或handle</param>
        public void Completed(string taskno)
        {
            if (string.IsNullOrEmpty(taskno))
            {
                SysLog log = new SysLog("TaskManagement::Complted => taskno is empty.");
                return;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::Complted => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    comm.CommandText = string.Format("SELECT T.SN,C.CUSTOMER,I.ITEM,I.BOMNO,TD.SFC,TD.QTY,T.CREATED_DATE_TIME FROM TASK T INNER JOIN ITEM I ON T.ITEM_NO = I.ITEM INNER JOIN TASK_DETAILS TD ON T.HANDLE  = TD.HANDLE_TASK INNER JOIN CUSTOMER C ON T.HANDLE_CUSTOMER = C.HANDLE WHERE T.SN = {0};", taskno);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception(string.Format("TaskManagement::Complted => TaskNo(0#) is not exist.", taskno));
                    string customer = null;
                    HtmlTable tWeb = new HtmlTable();
                    tWeb.Title = "任务明细：";
                    tWeb.TH = "任务编号";
                    tWeb.TH = "客户";
                    tWeb.TH = "料号";
                    tWeb.TH = "型号";
                    tWeb.TH = "栈板编号";
                    tWeb.TH = "数量";
                    tWeb.TH = "任务启动时间";
                    while (reader.Read())
                    {
                        tWeb.TD = reader["SN"].ToString();
                        tWeb.TD = reader["CUSTOMER"].ToString();
                        customer = reader["CUSTOMER"].ToString();
                        tWeb.TD = reader["ITEM"].ToString();
                        tWeb.TD = reader["BOMNO"].ToString();
                        tWeb.TD = reader["SFC"].ToString();
                        tWeb.TD = reader["QTY"].ToString();
                        tWeb.TD = reader["CREATED_DATE_TIME"].ToString();
                        tWeb.TR = tWeb.TD;
                    }
                    tWeb.TABLE = tWeb.TR;
                    reader.Close();
                    comm.CommandText = string.Format("SELECT ROW_NUMBER() OVER(ORDER BY TL.CREATED_DATE_TIME) 提醒次数, T.SN 任务编号,TL.STATE 任务阶段,CASE TL.STATE WHEN '启动' THEN '回传系统提醒：通知产品负责人有新回传任务，提示尽快查看回传数据！' WHEN '确认中' THEN '回传系统提醒：产品负责人已查询回传数据但并未完成确认，提示尽快完成确认！' END 回传系统自动提醒内容,TL.CREATED_DATE_TIME FROM TASK T INNER JOIN TASK_LOG TL ON T.HANDLE = TL.HANDLE_TASK WHERE T.SN = '{0}' ORDER BY TL.CREATED_DATE_TIME ASC;", taskno);
                    reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception(string.Format("TaskManagement::Complted => TaskNo(0#) is not exist.", taskno));
                    tWeb.Title = "任务执行情况：";
                    tWeb.TH = "提醒次数";
                    tWeb.TH = "任务编号";
                    tWeb.TH = "阶段";
                    tWeb.TH = "提醒内容	";
                    tWeb.TH = "提醒时间";
                    while (reader.Read())
                    {
                        tWeb.TD = reader["提醒次数"].ToString();
                        tWeb.TD = reader["任务编号"].ToString();
                        tWeb.TD = reader["任务阶段"].ToString();
                        tWeb.TD = reader["回传系统自动提醒内容"].ToString();
                        tWeb.TD = reader["CREATED_DATE_TIME"].ToString();
                        tWeb.TR = tWeb.TD;
                    }
                    tWeb.TABLE = tWeb.TR;
                    reader.Close();
                    comm.CommandText = string.Format("SELECT E.USERNAME,E.EMAIL FROM CUSTOMER C INNER JOIN EMAIL_GROUP G ON C.HANDLE = G.HANDLE_CUSTOMER INNER JOIN EMAIL_CONFIG E ON G.HANDLE_EMAIL = E.HANDLE WHERE CUSTOMER = '{0}';", customer);
                    reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception(string.Format("TaskManagement::Complted => user email is not exist.", taskno));
                    StringBuilder emailList = new StringBuilder();
                    while (reader.Read())
                    {
                        emailList.Append(reader[1].ToString());
                        emailList.Append(",");
                    }
                    reader.Close();
                    //发送邮件
                    Mail.SendMail(emailList.ToString(), System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], customer + " 数据发送任务", tWeb.Html);
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return;
                }
            }
        }
        /// <summary>
        /// 获取邮件通知的用户清单，无用户名及邮件时，取全部
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public List<UsersInfos> GetUsers(string username, string email)
        {
            StringBuilder sql = new StringBuilder("SELECT * FROM EMAIL_CONFIG WHERE 1 = 1 ");
            if (!string.IsNullOrEmpty(username))
            {
                sql.Append($" AND USERNAME = '{username}' ");
            }
            if (!string.IsNullOrEmpty(email))
            {
                sql.Append($" AND EMAIL = '{email}' ");
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetUsers => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("No data was queried from data table EMAIL_CONFIG");
                    List<UsersInfos> userList = new List<UsersInfos>();
                    while (reader.Read())
                    {
                        UsersInfos user = new UsersInfos();
                        user.HANDLE = reader["HANDLE"].ToString();
                        user.USERNAME = reader["USERNAME"].ToString();
                        user.PASSWORD = reader["PASSWORD"].ToString();
                        user.STATE = reader["STATUS"].ToString();
                        user.EMAIL = reader["EMAIL"].ToString();
                        user.DEPT = reader["DEPARTMENT"].ToString();
                        user.REMARKS = reader["REMARKS"].ToString();
                        user.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                        userList.Add(user);
                    }
                    reader.Close();
                    return userList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取客户
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public List<CUSTOMERS> GetCustomers(string customer)
        {
            StringBuilder sql = new StringBuilder("SELECT * FROM CUSTOMER WHERE 1 = 1 ");
            if (!string.IsNullOrEmpty(customer))
            {
                sql.Append($" AND CUSTOMER = '{customer}'");
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetUsers => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("No data was queried from data table CUSTOMER");
                    List<CUSTOMERS> customerList = new List<CUSTOMERS>();
                    while (reader.Read())
                    {
                        CUSTOMERS _customer = new CUSTOMERS();
                        _customer.HANDLE = reader["HANDLE"].ToString();
                        _customer.CUSTOMER = reader["CUSTOMER"].ToString();
                        _customer.CREATED_DATE_TIME = reader["CREATED_DATE_TIME"].ToString();
                        customerList.Add(_customer);
                    }
                    reader.Close();
                    return customerList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取用户邮件与客户绑定关系
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<EMAIL_GROUP> GetEmailGroup(string customer, string user)
        {
            StringBuilder sql = new StringBuilder("SELECT EG.HANDLE HANDLE_GROUP, EG.HANDLE_CUSTOMER, EG.HANDLE_EMAIL, C.CUSTOMER, EC.USERNAME FROM EMAIL_GROUP EG INNER JOIN CUSTOMER C ON EG.HANDLE_CUSTOMER = C.HANDLE INNER JOIN EMAIL_CONFIG EC ON EG.HANDLE_EMAIL = EC.HANDLE WHERE 1 = 1 ");
            if (!string.IsNullOrEmpty(user))
            {
                sql.Append($" AND USERNAME = '{user}' ");
            }
            if (!string.IsNullOrEmpty(customer))
            {
                sql.Append($" AND CUSTOMER = '{customer}' ");
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("UsersInfos::GetUsers => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("TaskManagement::GetEmailGroup => SyncRemote db can not be open.");
                    List<EMAIL_GROUP> groupList = new List<EMAIL_GROUP>();
                    while (reader.Read())
                    {
                        EMAIL_GROUP group = new EMAIL_GROUP();
                        group.HANDLE_CUSTOMER = reader["HANDLE_CUSTOMER"].ToString();
                        group.HANDLE_EMAIL = reader["HANDLE_EMAIL"].ToString();
                        group.HANDLE_GROUP = reader["HANDLE_GROUP"].ToString();
                        group.CUSTOMER = reader["CUSTOMER"].ToString();
                        group.USERNAME = reader["USERNAME"].ToString();
                        groupList.Add(group);
                    }
                    reader.Close();
                    return groupList;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 新建客户
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public ResultTask AddCustomer(string customer)
        {
            ResultTask result = new ResultTask();
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    if (string.IsNullOrEmpty(customer)) new Exception("customer is empty.");
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::AddCustomer => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand($"BEGIN IF NOT EXISTS (SELECT * FROM CUSTOMER WHERE CUSTOMER = '{customer}') BEGIN INSERT INTO CUSTOMER (CUSTOMER) VALUES ('{customer}') END END", conn);
                    if (comm.ExecuteNonQuery() == 1)
                        result.RESULT = "success";
                    else
                    {
                        result.RESULT = "fail";
                        result.MSG = "客户已存在！";
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.RESULT = "fail";
                    result.MSG = ex.Message;
                    return result;
                }
            }
        }
        /// <summary>
        /// 新建客户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultTask AddUser(UsersInfos user)
        {
            ResultTask result = new ResultTask();
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    if (user == null) new Exception("user is empty.");
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::AddUser => SyncRemote db can not be open.");
                    }
                    string pwd = string.IsNullOrEmpty(user.PASSWORD) ? ConfigurationManager.AppSettings["KPK_Performance"].Trim() : user.PASSWORD;
                    string status = string.IsNullOrEmpty(user.STATE) ? "NULL" : "'" + user.STATE + "'";
                    string remarks = string.IsNullOrEmpty(user.REMARKS) ? "NULL" : "'" + user.REMARKS + "'";
                    string dept = string.IsNullOrEmpty(user.DEPT) ? "NULL" : "'" + user.DEPT + "'";
                    SqlCommand comm = new SqlCommand($"BEGIN IF NOT EXISTS (SELECT * FROM EMAIL_CONFIG WHERE USERNAME = '{user.USERNAME}') BEGIN INSERT INTO EMAIL_CONFIG (USERNAME,PASSWORD,STATUS,REMARKS,DEPARTMENT,EMAIL) VALUES ('{user.USERNAME}','{pwd}',{status},{remarks},{dept},'{user.EMAIL}') END END", conn);
                    if (comm.ExecuteNonQuery() == 1)
                        result.RESULT = "success";
                    else
                    {
                        result.RESULT = "fail";
                        result.MSG = "用户已存在！";
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.RESULT = "fail";
                    result.MSG = ex.Message;
                    return result;
                }
            }
        }
        /// <summary>
        /// 新建绑定关系
        /// </summary>
        /// <param name="handle_customer"></param>
        /// <param name="handle_user"></param>
        /// <returns></returns>
        public ResultTask AddGroup(string handle_customer, string handle_user)
        {
            ResultTask result = new ResultTask();
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    if (string.IsNullOrEmpty(handle_customer) || string.IsNullOrEmpty(handle_user)) throw new Exception("handle_customer or handle_user is empty.");
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::AddGroup => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand($"BEGIN IF NOT EXISTS (SELECT * FROM EMAIL_GROUP WHERE HANDLE_CUSTOMER = {handle_customer} AND HANDLE_EMAIL = {handle_user}) BEGIN INSERT INTO EMAIL_GROUP (HANDLE_CUSTOMER,HANDLE_EMAIL) VALUES ({handle_customer},{handle_user}) END END", conn);
                    if (comm.ExecuteNonQuery() == 1)
                        result.RESULT = "success";
                    else
                    {
                        result.RESULT = "fail";
                        result.MSG = "绑定已存在！";
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.RESULT = "fail";
                    result.MSG = ex.Message;
                    return result;
                }
            }
        }
        /// <summary>
        /// 解除客户与用户间的绑定关系
        /// </summary>
        /// <param name="handle_customer"></param>
        /// <param name="handle_user"></param>
        /// <returns></returns>
        public ResultTask DelGroup(string handle_customer, string handle_user)
        {
            ResultTask result = new ResultTask();
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    if (string.IsNullOrEmpty(handle_customer) || string.IsNullOrEmpty(handle_user)) throw new Exception("handle_customer or handle_user is empty.");
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::DelGroup => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand($"DELETE FROM EMAIL_GROUP WHERE HANDLE_CUSTOMER = '{handle_customer}' AND HANDLE_EMAIL = '{handle_user}'", conn);
                    if (comm.ExecuteNonQuery() == 1)
                    {
                        result.RESULT = "success";

                    }
                    else
                    {
                        result.RESULT = "fail";
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    result.RESULT = "fail";
                    result.MSG = ex.Message;
                    return result;
                }
            }
        }
        /// <summary>
        /// 根据任务号获取电池数量及待补充的数据内容
        /// </summary>
        /// <param name="taskno"></param>
        /// <returns></returns>
        public DATA_SUPPLEMENT GetQtyByTaskNo(string taskno)
        {
            if (string.IsNullOrEmpty(taskno)) return null;
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetQtyByTaskNo => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT DISTINCT T.ITEM_NO,DS.DATA_TYPE,T.QTY_TOTAL FROM TASK T INNER JOIN ITEM I ON T.ITEM_NO = I.ITEM INNER JOIN DATA_SUPPLEMENT DS ON I.HANDLE = DS.HANDLE_ITEM WHERE T.SN = '{taskno}';", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("TaskManagement::GetQtyByTaskNo => There is no additional data to add.");
                    DATA_SUPPLEMENT result = new DATA_SUPPLEMENT();
                    while (reader.Read())
                    {
                        result.ITEM_NO = reader["ITEM_NO"].ToString();
                        result.QTY = reader["QTY_TOTAL"].ToString();
                        result.DATA_TYPE = reader["DATA_TYPE"].ToString();
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
        /// 随机获取补充值
        /// </summary>
        /// <param name="taskno"></param>
        /// <returns></returns>
        public DataTable GetDataSupplement(string taskno)
        {
            if (string.IsNullOrEmpty(taskno)) return null;
            DATA_SUPPLEMENT result = GetQtyByTaskNo(taskno);
            if (result == null) return null;
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("TaskManagement::GetDataSupplement => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT TOP {result.QTY} DATA_VALUE FROM DATA_SUPPLEMENT DS INNER JOIN ITEM I ON DS.HANDLE_ITEM = I.HANDLE WHERE I.ITEM = '{result.ITEM_NO}' AND DS.DATA_TYPE = '{result.DATA_TYPE}' AND DS.STATE IS NULL ORDER BY NEWID();", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("TaskManagement::GetDataSupplement => There is no additional data to add.");
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }               
        }

    }
    public class SyncAnker
    {
        public AnkerTask CreateTask(string file)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncTWS::CreateTask => SyncRemote database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT * FROM ANKER_CONFIG WHERE STATE = 'Y';"), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        throw new Exception("ANKER_CONFIG data does not exist.");
                    }
                    AnkerTask task = new AnkerTask();
                    reader.Read();
                    task.APPID = reader["APPID"].ToString();
                    task.SECRET = reader["SECRET"].ToString();
                    task.TYPE = reader["TYPE"].ToString();
                    task.URI = reader["URI"].ToString();
                    task.HANDLE_CONFIG = reader["HANDLE"].ToString();
                    task.URI_TOKEN = reader["URI_TOKEN"].ToString();
                    reader.Close();
                    comm.CommandText = string.Format(string.Format("INSERT INTO ANKER_TASK (HANDLE_TASK,HANDLE_CONFIG,STATE,COMMENTS) OUTPUT INSERTED.HANDLE VALUES (null,'{0}','{1}','{2}');", task.HANDLE_CONFIG, "未开始", file));
                    string handle = comm.ExecuteScalar().ToString();
                    task.HANDLE = handle;
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog("SyncAnker::CreateTask ==> " + ex.Message);
                    return null;
                }
            }
        }
        public TokenAnker InitToken(AnkerTask task)
        {
            try
            {
                WebSOAP soap = new WebSOAP();
                string result = soap.Anker__QuerySoapWebApiToken(task);
                TokenAnker token = JsonConvert.DeserializeObject<TokenAnker>(result);
                return token;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("SyncAnker::InitToken ==> " + ex.Message);
                return null;
            }
        }
        public void UpdateToken(string token)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncAnker::UpdateToken => SyncRemote database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand($"UPDATE ANKER_CONFIG SET TOKEN = '{token}' WHERE STATE = 'Y';", conn);
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
        }
        public string GetToken()
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncAnker::GetToken => SyncRemote database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT TOKEN FROM ANKER_CONFIG WHERE STATE = 'Y';", conn);
                    string token = comm.ExecuteScalar().ToString();
                    return token; 
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public AnkerTask GetTask(string handle)
        {
            if (string.IsNullOrEmpty(handle))
            {
                return null;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncAnker::GetTask => SyncRemote database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand(string.Format("SELECT T.HANDLE,T.HANDLE_CONFIG,T.HANDLE_TASK, C.APPID ,C.SECRET,T.TOKEN,C.URI,C.URI_TOKEN,T.STATE,T.TOTAL FROM ANKER_TASK T INNER JOIN ANKER_CONFIG C ON T.HANDLE_CONFIG = C.HANDLE WHERE T.HANDLE = '{0}';", handle), conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    AnkerTask task = new AnkerTask();
                    reader.Read();
                    task.HANDLE = reader["HANDLE"].ToString();
                    task.HANDLE_CONFIG = reader["HANDLE_CONFIG"].ToString();
                    task.HANDLE_TASK = reader["HANDLE_TASK"].ToString();
                    task.APPID = reader["APPID"].ToString();
                    task.SECRET = reader["SECRET"].ToString();
                    task.URI = reader["URI"].ToString();
                    task.TOKEN = reader["TOKEN"].ToString();
                    task.STATE = reader["STATE"].ToString();
                    task.TOTAL = !string.IsNullOrEmpty(reader["TOTAL"].ToString()) ? Convert.ToInt32(reader["TOTAL"].ToString()) : 0;
                    task.URI_TOKEN = reader["URI_TOKEN"].ToString();
                    reader.Close();
                    return task;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        public void UpdateTaskComplete(AnkerTask task)
        {
            if (task == null) return ;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncAnker::UpdateTaskComplete => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand($"UPDATE ANKER_TASK SET COMPLETED = {task.COMPLETED} WHERE HANDLE = '{task.HANDLE}';", conn);
                    comm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return;
                }
            }
        }
        public void UpdateTask(AnkerTask task, string state, int complete)
        {
            if (task == null || string.IsNullOrEmpty(state))
            {
                return;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncAnker::UpdateTask => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    comm.CommandText = $"UPDATE ANKER_TASK SET STATE = '{state}',COMPLETED = {complete} WHERE HANDLE = '{task.HANDLE}';";
                    comm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return ;
                }
            }
        }
        public int CheckIn(List<BatteryAnker> batteryList, AnkerTask task)
        {
            if (batteryList.Count == 0 || task == null)
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncAnker::CheckIn => SyncRemote database can not be open.");
                    }
                    StringBuilder sql = new StringBuilder();
                    foreach (BatteryAnker battery in batteryList)
                    {
                        //sql.Append($"INSERT INTO ANKER_SYNCBATTERY (HANDLE_TASK,BARCODE,OCV1,OCV1_TESTTIME,IR1,OCV2,OCV2_TESTTIME,IR2,KVALUE,CAPACITY,VOLTAGE,RESISTANCE,GRADE,TEST_RESULT,TEST_TIME,LOT,MATERIALS_NO,STATION_ID) VALUES ('{task.HANDLE}','{battery.BARCODE}','{battery.OCV1}','{battery.OCV1_TESTTIME}','{battery.IR1}','{battery.OCV2}','{battery.OCV2_TESTTIME}','{battery.IR2}','{battery.KVALUE}','{battery.CAPACITY}','{battery.VOLTAGE}','{battery.RESISTANCE}','{battery.GRADE}','{battery.TEST_RESULT}','{battery.TEST_TIME}','{battery.LOT}','{battery.MATERIALS_NO}','{battery.STATION_ID}');");
                        //sql.Append($"INSERT INTO ANKER_SYNCBATTERY (HANDLE_TASK,BARCODE,OCV1,OCV1_TESTTIME,IR1,OCV2,OCV2_TESTTIME,IR2,KVALUE,CAPACITY,VOLTAGE,RESISTANCE,GRADE,TEST_RESULT,TEST_TIME,LOT,MATERIALS_NO,STATION_ID,JYL,VOLTAGE_OCV1,RESISTANCE_OCV1,TESTTIME_OCV1,VOLTAGE_OCV2,RESISTANCE_OCV2,TESTTIME_OCV2,VOLTAGE_SHELL,THICKNESS,WIDTH) VALUES ('{task.HANDLE}','{battery.BARCODE}','{battery.OCV1}','{battery.OCV1_TESTTIME}','{battery.IR1}','{battery.OCV2}','{battery.OCV2_TESTTIME}','{battery.IR2}','{battery.KVALUE}','{battery.CAPACITY}','{battery.VOLTAGE}','{battery.RESISTANCE}','{battery.GRADE}','{battery.TEST_RESULT}','{battery.TEST_TIME}','{battery.LOT}','{battery.MATERIALS_NO}','{battery.STATION_ID}','{battery.JYL}','{battery.VOLTAGE_OCV1}','{battery.RESISTANCE_OCV1}','{battery.TESTTIME_OCV1}','{battery.VOLTAGE_OCV2}','{battery.RESISTANCE_OCV2}','{battery.TESTTIME_OCV2}','{battery.VOLTAGE_SHELL}','{battery.THICKNESS}','{battery.WIDTH}');");
                        sql.Append($"INSERT INTO ANKER_SYNCBATTERY (HANDLE_TASK,BARCODE,OCV1,OCV1_TESTTIME,IR1,OCV2,OCV2_TESTTIME,IR2,KVALUE,CAPACITY,VOLTAGE,RESISTANCE,GRADE,TEST_RESULT,TEST_TIME,LOT,MATERIALS_NO,STATION_ID,JYL,VOLTAGE_SHELL) VALUES ('{task.HANDLE}','{battery.BARCODE}','{battery.OCV1}','{battery.OCV1_TESTTIME}','{battery.IR1}','{battery.OCV2}','{battery.OCV2_TESTTIME}','{battery.IR2}','{battery.KVALUE}','{battery.CAPACITY}','{battery.VOLTAGE}','{battery.RESISTANCE}','{battery.GRADE}','{battery.TEST_RESULT}','{battery.TEST_TIME}','{battery.LOT}','{battery.MATERIALS_NO}','{battery.STATION_ID}','{battery.JYL}','{battery.VOLTAGE_SHELL}');");
                    }
                    SqlCommand comm = new SqlCommand(sql.ToString(), conn);
                    comm.Transaction = tran;
                    int count = comm.ExecuteNonQuery();
                    tran.Commit();
                    comm.CommandText = $"UPDATE ANKER_TASK SET TOTAL = TOTAL + {count} WHERE HANDLE = '{task.HANDLE}';";
                    comm.ExecuteNonQuery();
                    return count;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public int CreateSendInfo(AnkerTask task)
        {
            if (task == null) return 0;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                int seed = 10, _count = 0, __count = 0;
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncAnker::CreateSendInfo => SyncRemote database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT * FROM ANKER_SyncBattery WHERE HANDLE_TASK = '{task.HANDLE}'", conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (!reader.HasRows) throw new Exception("No data ANKER_SyncBattery.");
                    StringBuilder sb = new StringBuilder();
                    SysDataAnker request = new SysDataAnker();
                    string station_id = null, material_no = null;
                    while (reader.Read())
                    {
                        ++_count;
                        station_id = reader["STATION_ID"].ToString();
                        material_no = reader["MATERIALS_NO"].ToString();
                        ItemsAnker items = new ItemsAnker();
                        items.bar_code = reader["BARCODE"].ToString();
                        items.materials_po = reader["LOT"].ToString();
                        items.test_time = reader["TEST_TIME"].ToString();
                        for (int i = 0; i < reader.FieldCount; ++i)
                        {
                            SysItemAnker data = new SysItemAnker();

                            if (reader.GetName(i) == "VOLTAGE")
                            {
                                data.name = "出货电压";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "RESISTANCE")
                            {
                                data.name = "出货内阻";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "TEST_TIME")
                            {
                                data.name = "出货测试时间";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "KVALUE")
                            {
                                data.name = "K值";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "CAPACITY")
                            {
                                data.name = "容量";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "GRADE")
                            {
                                data.name = "档位";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "JYL")
                            {
                                data.name = "保液量";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "OCV1")
                            {
                                data.name = "OCV1测试电压";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "IR1")
                            {
                                data.name = "OCV1测试内阻";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "OCV1_TESTTIME")
                            {
                                data.name = "OCV1测试时间";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "OCV2")
                            {
                                data.name = "OCV2测试电压";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "IR2")
                            {
                                data.name = "OCV2测试内阻";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "OCV2_TESTTIME")
                            {
                                data.name = "OCV2测试时间";
                                data.val = reader[i].ToString();
                                data.result = 1;
                            }
                            if (reader.GetName(i) == "VOLTAGE_SHELL")
                            {
                                data.name = "边电压";
                                data.val = reader[i].ToString();
                                data.result = 1;
                                items.test_time = reader[i].ToString();
                            }
                            if (string.IsNullOrEmpty(data.name)) continue;
                            items.data.Add(data);
                        }
                        request.items.Add(items);
                        if (_count % seed == 0)
                        {
                            if (string.IsNullOrEmpty(request.station_id)) request.station_id = station_id;
                            if (string.IsNullOrEmpty(request.material_no)) request.material_no = material_no;
                            sb.Append($"INSERT INTO ANKER_SEND_INFO (HANDLE_TASK,CONTENTS,QTY) VALUES ('{task.HANDLE}','{JsonConvert.SerializeObject(request)}','1');");
                            request = new SysDataAnker();
                        }
                    }
                    if (request.items.Count != 0)
                    {
                        if (string.IsNullOrEmpty(request.station_id)) request.station_id = station_id;
                        if (string.IsNullOrEmpty(request.material_no)) request.material_no = material_no;
                        sb.Append($"INSERT INTO ANKER_SEND_INFO (HANDLE_TASK,CONTENTS,QTY) VALUES ('{task.HANDLE}','{JsonConvert.SerializeObject(request)}','1');");
                    }
                    reader.Close();
                    SqlTransaction tran = conn.BeginTransaction();
                    try
                    {
                        comm.Transaction = tran;
                        comm.CommandText = sb.ToString();
                        int count = comm.ExecuteNonQuery();
                        tran.Commit();
                        return _count;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        SysLog log = new SysLog("SyncAnker::CreateSendInfo=>" + ex.Message);
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog("SyncAnker::CreateSendInfo=>" + ex.Message);
                    return 0;
                }
            }
        }
        public int BackupSyncbattery(AnkerTask task)
        {
            if (task == null) return 0;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncAnker::BackupSyncbattery => SyncRemote database can not be open.");
                    }
                    SqlCommand comm = new SqlCommand($"INSERT ANKER_SyncBattery_LOG (HANDLE_TASK,BARCODE,OCV1,OCV1_TESTTIME,IR1,OCV2,OCV2_TESTTIME,IR2,KVALUE,CAPACITY,VOLTAGE,RESISTANCE,GRADE,TEST_RESULT,TEST_TIME,LOT,MATERIALS_NO,STATION_ID,JYL,VOLTAGE_SHELL,THICKNESS,WIDTH,CREATED_USER) SELECT HANDLE_TASK,BARCODE,OCV1,OCV1_TESTTIME,IR1,OCV2,OCV2_TESTTIME,IR2,KVALUE,CAPACITY,VOLTAGE,RESISTANCE,GRADE,TEST_RESULT,TEST_TIME,LOT,MATERIALS_NO,STATION_ID,JYL,VOLTAGE_SHELL,THICKNESS,WIDTH,CREATED_USER FROM ANKER_SyncBattery WHERE HANDLE_TASK = '{task.HANDLE}';", conn);
                    if (comm.ExecuteNonQuery() == task.TOTAL)
                    {
                        comm.CommandText = $"DELETE FROM ANKER_SyncBattery WHERE HANDLE_TASK = '{task.HANDLE}';";
                        return comm.ExecuteNonQuery();
                    }
                    return 0;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
        public void SendData(ref AnkerTask task)
        {
            if (task == null || task.STATE != "准备就绪")
            {
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncAnker::SendData => SyncRemote db can not be open.");
                    }
                    SqlCommand comm = new SqlCommand($"SELECT HANDLE,CONTENTS FROM ANKER_SEND_INFO WHERE HANDLE_TASK = '{task.HANDLE}';", conn);
                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        if (!reader.HasRows) throw new Exception("SyncAnker::SendData => ANKER_SEND_INFO is empty.");
                        WebSOAP soap = new WebSOAP();
                        while (reader.Read())
                        {
                            string token = GetToken();
                            task.TOKEN = token;
                            string info = soap.Anker__QuerySoapWebApi(task, reader["CONTENTS"].ToString());
                            ResultAnker result = JsonConvert.DeserializeObject<ResultAnker>(info);
                            string handle = RecievedLog(reader["HANDLE"].ToString(), info, task);
                            if (result.res_code == 1)
                            {
                                BackupSendInfo(task, reader["HANDLE"].ToString(), handle);
                                //if (BackupSendInfo(task, reader["HANDLE"].ToString(), handle) == 1)
                                //{
                                //task.COMPLETED++;
                                //}
                            }
                            else
                            {
                                throw new Exception("The peer server returns the message \"Failed\".");
                                //break;
                            }
                        }
                    }
                    //if (task.COMPLETED == task.TOTAL)
                    //{
                    UpdateTask(task, "成功", task.TOTAL);
                    //}
                    //else
                    //{
                    //}
                    //UpdateTaskComplete(task);
                    //Complete(ref task);
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                UpdateTask(task, "失败", 0);
            }
            //UpdateTaskComplete(task);
            Complete(ref task);

        }
        private string RecievedLog(string handle_send, string contents, AnkerTask task)
        {
            if (string.IsNullOrEmpty(contents) || task == null) return null;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    throw new Exception("WebSOAP::RecievedLog => SyncRemote db can not be open.");
                }
                try
                {
                    SqlCommand comm = new SqlCommand($"INSERT INTO ANKER_RECIEVED_INFO (HANDLE_TASK,HANDLE_SEND_INFO,RECIEVED_INFO) OUTPUT INSERTED.HANDLE VALUES ('{task.HANDLE}','{handle_send}','{contents}');", conn);
                    string handle = comm.ExecuteScalar().ToString();
                    return handle;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        private int BackupSendInfo(AnkerTask task, string handle_send_info, string handle_recieved)
        {
            if (task == null || string.IsNullOrEmpty(handle_send_info)) return 0;
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    throw new Exception("WebSOAP::BackupSendInfo => SyncRemote db can not be open.");
                }
                try
                {
                    int result = 0;
                    SqlCommand comm = new SqlCommand($"INSERT INTO ANKER_SNED_LOG (HANDLE_TASK,HANDLE_SEND_INFO,HANDLE_RECIEVED,CONTENTS,QTY,CREATED_DATE_TIME) SELECT HANDLE_TASK,HANDLE,'{handle_recieved}',CONTENTS,'1',CREATED_DATE_TIME FROM ANKER_SEND_INFO WHERE HANDLE = '{handle_send_info}';", conn);
                    if (comm.ExecuteNonQuery() == 1)
                    {
                        comm.CommandText = $"DELETE FROM ANKER_SEND_INFO WHERE HANDLE = '{handle_send_info}'";
                        result = comm.ExecuteNonQuery();
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }

            }
        }
        public void Complete(ref AnkerTask task)
        {
            if (task.HANDLE == null)
            {
                return;
            }
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    // 数据库未打开
                    if (conn.State != ConnectionState.Open)
                    {
                        throw new Exception("SyncAnker::Complete => Anker database can not be open.");
                    }
                    // 任务未成功
                    SqlCommand comm = new SqlCommand(string.Format("SELECT COUNT(1) FROM ANKER_TASK WHERE STATE = '成功' AND HANDLE = '{0}'", task.HANDLE), conn);
                    if (comm.ExecuteScalar().ToString() != "1")
                    {
                        return;
                    }
                    comm.CommandText = $"SELECT HANDLE,TOTAL,COMPLETED,STATE,CREATED_DATE_TIME,COMMENTS FROM ANKER_TASK WHERE HANDLE = '{task.HANDLE}'";
                    SqlDataReader reader = comm.ExecuteReader();
                    TableWeb tWeb = new TableWeb();
                    tWeb.addThead("任务编号");
                    tWeb.addThead("数据文件");
                    tWeb.addThead("任务状态");
                    tWeb.addThead("开始回传时间");
                    tWeb.addThead("计划发送数量");
                    tWeb.addThead("完成发送数量");
                    while (reader.Read())
                    {
                        tWeb.addContext(reader["HANDLE"].ToString());
                        tWeb.addContext(reader["COMMENTS"].ToString());
                        //if (Convert.ToInt32(reader["TOTAL"]) != Convert.ToInt32(reader["COMPLETED"]))
                        //{
                        //    tWeb.State = false;
                        //    tWeb.addContext("失败");
                        //}
                        if (reader["STATE"].ToString() != "成功")
                        {
                            tWeb.State = false;
                            tWeb.addContext("失败");
                        }
                        else
                        {
                            tWeb.addContext("成功");
                        }
                        tWeb.addContext(reader["CREATED_DATE_TIME"].ToString());
                        tWeb.addContext(reader["TOTAL"].ToString());
                        tWeb.addContext(reader["COMPLETED"].ToString());
                    }
                    reader.Close();
                    comm.CommandText = "SELECT EC.EMAIL FROM CUSTOMER G INNER JOIN EMAIL_GROUP EG ON G.HANDLE = EG.HANDLE_CUSTOMER INNER JOIN EMAIL_CONFIG EC ON EG.HANDLE_EMAIL = EC.HANDLE WHERE G.CUSTOMER = 'ANKER';";
                    reader = comm.ExecuteReader();
                    StringBuilder emailList = new StringBuilder();
                    while (reader.Read())
                    {
                        emailList.Append(reader[0].ToString());
                        emailList.Append(",");
                    }
                    reader.Close();
                    //发送邮件
                    Mail.SendMail(emailList.ToString(), System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], "Anker 数据发送任务", tWeb.TableHtml());

                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
        }

    }
}