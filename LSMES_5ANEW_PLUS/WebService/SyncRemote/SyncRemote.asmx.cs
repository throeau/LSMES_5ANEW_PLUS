using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.Business;
using LSMES_5ANEW_PLUS.App_Base;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;

namespace LSMES_5ANEW_PLUS.WebService.SyncRemote
{
    /// <summary>
    /// SyncRemote 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class SyncRemote : System.Web.Services.WebService
    {
        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// 查询许可证 //////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public void Token(string info)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    conn.Open();
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        throw new Exception("Database open failed.");
                    }
                    StringBuilder str = new StringBuilder();
                    Licenses tokens = JsonConvert.DeserializeObject<Licenses>(info);
                    foreach(License token in tokens.Data)
                    {
                        str.Append("'");
                        str.Append(token.license);
                        str.Append("',");
                    }
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format("SELECT CUSTOMER,PWD FROM AUTHORIZE WHERE PWD IN ({0});", str.ToString().Substring(0, str.Length - 1));
                    SqlDataReader reader = command.ExecuteReader();

                    tokens.Data.Clear();
                    while (reader.Read())
                    {
                        License token = new License();
                        token.customer = reader["CUSTOMER"].ToString();
                        token.license = reader["PWD"].ToString();
                        tokens.Data.Add(token);
                    }

                    reader.Close();
                    conn.Close();
                    string jsonResult = JsonConvert.SerializeObject(tokens);
                    Context.Response.Charset = "UTF-8";
                    Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(jsonResult);
                    Context.Response.End();

                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    Context.Response.Charset = "UTF-8";
                    Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                    Context.Response.Write(null);
                    Context.Response.End();
                }
            }

        }
        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// 查询各型号所需上传数据 //////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public void IndexSync(string info)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
            {
                try
                {
                    string sql = null;
                    if (info.Contains("SCUD"))
                    {
                        sql = string.Format("SELECT REMARKS VALUE FROM SCUD_CONFIG WHERE ITEM_NO = '{0}'", info.Split(',')[1]);
                    }
                    else
                    {
                        sql = string.Format("SELECT C.NAME,P.VALUE FROM SYS.EXTENDED_PROPERTIES P ,SYS.COLUMNS C WHERE P.MAJOR_ID=OBJECT_ID('DESAY_SYNCBATTERY') AND P.MAJOR_ID=C.OBJECT_ID AND P.MINOR_ID=C.COLUMN_ID AND C.NAME <> '' AND P.VALUE <> '';", info);
                    }
                    StringBuilder title = new StringBuilder();
                    conn.Open();                    
                    SqlCommand comm = new SqlCommand(sql, conn);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            title.Append(reader["VALUE"].ToString());
                            title.Append(",");
                        }
                    }
                    else
                    {
                        reader.Close();
                        comm.CommandText = string.Format("SELECT NAME FROM SYSCOLUMNS WHERE ID = OBJECT_ID('{0}_SYNCBATTERY') AND NAME NOT IN ('HANDLE','HANDLE_TASK','CREATED_DATE_TIME'); ", info);
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            if (!string.IsNullOrEmpty(reader["NAME"].ToString()))
                            {
                                title.Append(reader["NAME"].ToString());
                                title.Append(",");
                            }
                        }
                    }
                    reader.Close();
                    if (title.Length > 0)
                    {
                        string result = title.ToString().Substring(0, title.Length - 1);
                        Context.Response.Charset = "UTF-8";
                        Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                        Context.Response.Write(result);
                        Context.Response.End();
                    }
                }
                catch(Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////// SMP 数据回传 //////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public int SMP_CheckIn_Auto(string info)
        {
            SyncService service = new SyncSMP();
            try
            {
                SyncTask task = service.CreateTask();
                int taskID=service.AddTask(task);
                if (taskID == 0) {
                    throw new Exception("生成任务失败.");
                }
                int result = service.CheckIn(info,taskID.ToString());
                return result;
            }
            catch(Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        [WebMethod]
        public void SMP_CheckIn(string info,string taskID)
        {
            //try
            //{
            //    SyncService service = new SyncSMP();
            //    if (service.IsValid(taskID))
            //    {
            //        int result = service.CheckIn(info, taskID.ToString());
            //        return result;
            //    }
            //    else
            //    {
            //        throw new Exception("任务不存在或者其不是未开始状态");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    SysLog log = new SysLog(ex.Message);
            //    return 0;
            //}
            SyncService service = new SyncSMP();
            if (service.IsValid(taskID))
            {
                string result = service.CheckIn(info, taskID.ToString()).ToString();
                Context.Response.Charset = "UTF-8";
                Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Context.Response.Write(result);
                Context.Response.End();
            }
        }
        [WebMethod]
        public void SMP_CreateTask()
        {
            //try
            //{
            //    SyncService service = new SyncSMP();
            //    SyncTask task = service.CreateTask();
            //    return service.AddTask(task);
            //}
            //catch (Exception ex)
            //{
            //    SysLog log = new SysLog(ex.Message);
            //    return 0;
            //}

            SyncService service = new SyncSMP();
            SyncTask task = service.CreateTask();
            string result = service.AddTask(task).ToString();
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void SMP_GetTask(string id)
        {
            SyncSMP service = new SyncSMP();
            SMPTask task = service.GetTask(id);
            string json = JsonConvert.SerializeObject(task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void SMP_Ready(string taskID)
        {
            try
            {
                SyncService service = new SyncSMP();
                service.Ready(taskID);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        [WebMethod]
        public void SMP_SyncData()
        {
            SyncService service = new SyncSMP();
            service.SyncData();
        }
        [WebMethod]
        public void SMP_Search(string dt,string task,string barcode)
        {
            SyncSMP service = new SyncSMP();
            DataTable mDt = new DataTable();
            mDt = service.Search(dt, task, barcode);
            string json = JsonConvert.SerializeObject(mDt);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////// Zebra 数据回传 /////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public int Zebra_CheckIn_Auto(string info)
        {
            SyncService service = new SyncZebra();
            try
            {
                SyncTask task = service.CreateTask();
                int taskID = service.AddTask(task);
                if (taskID == 0)
                {
                    throw new Exception("生成任务失败.");
                }
                int result = service.CheckIn(info, taskID.ToString());
                return result;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        [WebMethod]
        public int Zebra_CheckIn(string info, string taskID)
        {
            try
            {
                SyncService service = new SyncZebra();
                if (service.IsValid(taskID))
                {
                    int result = service.CheckIn(info, taskID.ToString());
                    return result;
                }
                else
                {
                    throw new Exception("任务不存在或者其不是未开始状态");
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        [WebMethod]
        public int Zebra_CreateTask()
        {
            try
            {
                SyncService service = new SyncZebra();
                SyncTask task = service.CreateTask();
                return service.AddTask(task);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 0;
            }
        }
        [WebMethod]
        public void Zebra_Ready(string taskID)
        {
            try
            {
                SyncService service = new SyncZebra();
                service.Ready(taskID);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        [WebMethod]
        public void Zebra_SyncData()
        {
            SyncService service = new SyncZebra();
            service.SyncData();
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// HW 数据回传 /////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public int HW_Send_Test(string json)
        {
            try
            {
                SyncHW hw = new SyncHW();
                hw.SendData(json);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        /// <summary>
        /// Step 5. HW_Send
        /// 正式发送数据
        /// </summary>
        /// <param name="HandleTask">任务 HANDLE</param>
        /// <returns></returns>
        [WebMethod]
        public int HW_Send(string HandleTask)
        {
            SyncHW hw = new SyncHW();
            SyncTask task = hw.GetTask(HandleTask);
            task.STATE = "开始发送";
            int count = hw.SendData(ref task);
            hw.Complete(ref task);
            return count;
        }
        /// <summary>
        /// Step 2. HW_CheckIn
        /// 将已经解析为 box、battery 的数据与任务绑定，并写入 HW_SYNCBATTERY
        /// </summary>
        /// <param name="batteryInfo">RECEIVETESTDATA：生产数据；RECEIVETESTQUALITYDATA：质检数据</param>
        /// <param name="taskID">任务编号，HW_TASK 中 HANDLE</param>
        /// <returns></returns>
        [WebMethod]
        public int HW_CheckIn(string batteryInfo, string taskID)
        {
            SyncHW hw = new SyncHW();
            return hw.CheckIn(batteryInfo, taskID);
        }
        /// <summary>
        /// Step 1. HW_CreateTask_TestData
        /// 创建生产数据任务
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string HW_CreateTask_TestData()
        {
            SyncHW hw = new SyncHW();
            hw.SyncType = "RECEIVETESTDATA";
            return hw.AddHWTask(hw.CreateTask());
        }
        /// <summary>
        /// Step 3. HW_Ready
        /// 更新任务状态为准备就绪
        /// </summary>
        /// <param name="taskID">任务 HANDLE</param>
        /// <returns></returns>
        [WebMethod]
        public int HW_Ready(string taskID)
        {
            SyncHW hw = new SyncHW();
            return hw.Ready(taskID);
        }
        /// <summary>
        /// Step 4. HW_SyncData
        /// 将数据转为待发送的 json
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public int HW_SyncData()
        {
            SyncHW hw = new SyncHW();
            return hw.SyncData();
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// Desay 数据回传 //////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="customer">目前只有两种客户：HW / COMMON </param>
        /// <returns></returns>
        public string Desay_CreateTask(string customer)
        {
            SyncDesay desay = new SyncDesay();
            DesayTask task = desay.CreateDesayTask(customer);
            return task.HANDLE;
        }
        [WebMethod]

        /// <summary>
        /// 上传原始数据
        /// </summary>
        /// <param name="info">客户端将数据进行 JSON 打包后的内容</param>
        /// <param name="taskID">任务编号</param>
        /// <returns></returns>
        public void Desay_CheckIn(string info,string taskID)
        {
            SyncDesay desay = new SyncDesay();
            DesayTask task = new DesayTask();
            task = desay.GetDesayTask(taskID);
            task.COUNT = desay.CheckIn(info, task);
            string json = JsonConvert.SerializeObject(task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]

        /// <summary>
        /// 将数据同步成待发送状态（DESAY_SEND_INFO）
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public void Desay_SyncBattery(string taskID)
        {
            SyncDesay desay = new SyncDesay();
            DesayTask task = new DesayTask();
            task = desay.GetDesayTask(taskID);
            while (desay.SyncData(taskID)>0)
            {
                task = desay.GetDesayTask(taskID);
                desay.SendData(task);
            }
            desay.Complete(ref task);
        }
        [WebMethod]

        public void Desay_SendData(string taskID)
        {
            try
            {
                SyncDesay desay = new SyncDesay();
                DesayTask task = new DesayTask();
                task = desay.GetDesayTask(taskID);
                desay.SendData(task);
                //return 1;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                //return 0;
            }
        }
        [WebMethod]

        public int Desay_Ready(string taskID)
        {
            SyncDesay desay = new SyncDesay();
            return desay.Ready(taskID);
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////// Sunwoda 数据回传 //////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public void Sunwoda_CreateTask(string productLine)
        {
            SyncSunwoda sunwoda = new SyncSunwoda();
            SunwodaTask task = sunwoda.CreateTask(productLine);
            string result = JsonConvert.SerializeObject(task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        public void Sunwoda_GetTask(string taskID)
        {
            SunwodaTask task = new SunwodaTask();
            SyncSunwoda sunwoda = new SyncSunwoda();
            task = sunwoda.GetTask(taskID);
            string result = JsonConvert.SerializeObject(task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void Sunwoda_CheckIn(string Info,string taskID)
        {
            SyncSunwoda sunwoda = new SyncSunwoda();
            SunwodaTask task = new SunwodaTask();
            task = sunwoda.GetTask(taskID);
            string result = sunwoda.CheckIn(Info, task).ToString();
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void Sunwoda_SyncData(string taskID)
        {
            if (string.IsNullOrEmpty(taskID))
            {
                return;
            }
            SyncSunwoda sunwoda = new SyncSunwoda();
            SunwodaTask task = new SunwodaTask();
            task = sunwoda.GetTask(taskID);
            sunwoda.UpdateTask(task.HANDLE, "开始回传");
            while (sunwoda.SyncData(task) > 0)
            {
                task = sunwoda.GetTask(taskID);
                int result = sunwoda.SendData(task);
            }
            sunwoda.Complete(task);
        }
        [WebMethod]
        public void Sunwoda_Ready(string taskID)
        {
            SyncSunwoda sunwoda = new SyncSunwoda();
            bool result = sunwoda.UpdateTask(taskID, "准备就绪");
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void Sunwoda_SendData(string taskID)
        {
            SyncSunwoda sunwoda = new SyncSunwoda();
            SunwodaTask task = new SunwodaTask();
            task = sunwoda.GetTask(taskID);
            string result = sunwoda.SendData(task).ToString();
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void Sunwoda_Search(string dt,string taskID,string barcode)
        {
            SyncSunwoda sunwoda = new SyncSunwoda();
            DataTable mDt = sunwoda.Search(dt, taskID, barcode);
            string result = JsonConvert.SerializeObject(mDt);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////// 飞毛腿（SCUD） 数据回传 //////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public void SCUD_CreateTask(string bomno,string item_no)
        {
            SCUDTask task = new SCUDTask();
            SyncSCUD scud = new SyncSCUD();
            task = scud.CreateTask(bomno, item_no);
            string json = JsonConvert.SerializeObject(task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void SCUD_CheckIn(string info,string task_info)
        {
            SCUDTask task = new SCUDTask();
            task = JsonConvert.DeserializeObject<SCUDTask>(task_info);
            SyncSCUD scud = new SyncSCUD();
            SCUD_Result result = new SCUD_Result();
            if (task.DEPT.ToUpper() == "POWER_BANK")
            {
                PowerBankSCUD pbs = new PowerBankSCUD();
                pbs = JsonConvert.DeserializeObject<PowerBankSCUD>(info);
                int count = scud.CheckInByPowerBank(pbs, task);
                if (count > 0)
                {
                    result.RESULT = "success";
                }
                result.MSG = count.ToString();
            }
            if (task.DEPT.ToUpper() == "LAPTOP")
            {
                List<BatteryLaptopSCUD> bls = new List<BatteryLaptopSCUD>();
                bls = JsonConvert.DeserializeObject<List<BatteryLaptopSCUD>>(info);
                int count = scud.CheckInByLaptop(bls, task);
                if (count > 0)
                {
                    result.RESULT = "success";
                }
                result.MSG = count.ToString();
            }
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void SCUD_GetTask(string handle)
        {
            SCUDTask task = new SCUDTask();
            SyncSCUD scud = new SyncSCUD();
            task = scud.GetTask(handle);
            string json = JsonConvert.SerializeObject(task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void SCUD_SEND(string handle)
        {
            SCUDTask task = new SCUDTask();
            SyncSCUD scud = new SyncSCUD();
            task = scud.GetTask(handle);
            int total = scud.SyncData(ref task);
            SCUD_Result result = new SCUD_Result();
            if (total == task.TOTAL)
            {
                scud.UpdateTask(ref task, "准备就绪");
                total = scud.SendData(task);
                if ("成功" == task.STATE)
                {
                    result.RESULT = "success";
                }
            }
            else
            {
                result.RESULT = "fail";
            }
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void SCUD_ITEM()
        {
            SyncSCUD scud = new SyncSCUD();
            List<Item_no> result = scud.Item_no();
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void SCUD_Complete(string handle)
        {
            SyncSCUD scud = new SyncSCUD();
            scud.Complete(handle);
        }
        [WebMethod]
        public void SCUD_Query(string date,string task,string barcode)
        {
            SyncSCUD scud = new SyncSCUD();
            DataTable mDt = new DataTable();
            mDt = scud.Query(date, task, barcode);
            string json = JsonConvert.SerializeObject(mDt);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// 明美（TWS） 数据回传 ///////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public void TWS_CreateTask(string item)
        {
            TWSTask task = new TWSTask();
            SyncTWS tws = new SyncTWS();
            task = tws.CreateTask(item);
            string json = JsonConvert.SerializeObject(task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void TWS_CheckIn(string info, string handle_task)
        {
            TWSTask task = new TWSTask();
            SyncTWS tws = new SyncTWS();
            task = tws.GetTask(handle_task);
            List<BatteryTWS> batterys = new List<BatteryTWS>();
            batterys = JsonConvert.DeserializeObject<List<BatteryTWS>>(info);
            int count = tws.CheckIn(batterys, task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(count);
            Context.Response.End();
        }
        [WebMethod]
        public void TWS_GetTask(string handle)
        {
            TWSTask task = new TWSTask();
            SyncTWS tws = new SyncTWS();
            task = tws.GetTask(handle);
            string json = JsonConvert.SerializeObject(task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void TWS_Ready(string handle)
        {
            TWSTask task = new TWSTask();
            SyncTWS tws = new SyncTWS();
            task = tws.GetTask(handle);
            tws.UpdateTask(task, "准备就绪");
        }
        [WebMethod]
        public void TWS_SendData(string handle)
        {
            TWSTask task = new TWSTask();
            SyncTWS tws = new SyncTWS();
            task = tws.GetTask(handle);
            int count = tws.SendData(task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(count);
            Context.Response.End();
        }
        [WebMethod]
        public void TWS_SyncBattery(string handle)
        {
            TWSTask task = new TWSTask();
            SyncTWS tws = new SyncTWS();
            task = tws.GetTask(handle);
            int count = tws.SyncData(task);
            if (count > 0)
            {
                tws.UpdateTask(task, "准备就绪");
                count = tws.SendData(task);
            }
            task = tws.UpdateTask(task, "完成");
            tws.TWS_Complete(task.HANDLE);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(count);
            Context.Response.End();
        }
        [WebMethod]
        public void TWS_Query(string date, string task, string barcode)
        {
            SyncTWS tws = new SyncTWS();
            DataTable mDt = new DataTable();
            mDt = tws.TWS_Query(date, task, barcode);
            string json = JsonConvert.SerializeObject(mDt);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////// 科普克（KPK） 数据发送 //////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public void KPK_SendData(string info)
        {
            List<RowPerformance> rows = new List<RowPerformance>();
            ReslutPerformance result = new ReslutPerformance();
            rows = JsonConvert.DeserializeObject<List<RowPerformance>>(info);
            if (rows.Count == 0)
            {
                result.result = "fail";
                result.count = 0;
                result.msg = "no data.";
            }
            else
            {
                SyncKPK kpk = new SyncKPK();
                int qty = kpk.Load(rows, ConfigurationManager.AppSettings["KPK_Performance"].Trim());
                if (qty == kpk.Save())
                {
                    result.result = "success";
                    result.count = qty;
                }
                else
                {
                    result.result = "fail";
                    result.count = 0;
                }
            }
            info = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(info);
            Context.Response.End();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////// 任务管理 数据发送 ////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////
        [WebMethod]
        public void CreateSyncTask(string sfc, string category, string item_no, string customer, string created_user,string customer2,string customer2_id)
        {
            TaskManagement management = new TaskManagement();
            List<string> sfcList = new List<string>();
            string[] sfcs = sfc.Split(',');
            foreach (string str in sfcs)
            {
                sfcList.Add(str);
            }
            Tasks task = management.CreateTask(sfcList, category, item_no, customer, created_user, customer2, customer2_id);
            task = management.CreateSchedule(task, "DAILY_CHECK");
            string json = JsonConvert.SerializeObject(task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void UpdateSyncTask(string handle_task,string status,string updated_user)
        {
            TaskManagement management = new TaskManagement();
            Tasks task = management.UpdateTask(handle_task, status, updated_user);
            string json = JsonConvert.SerializeObject(task);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void InitTask(string customerid)
        {
            TaskManagement man = new TaskManagement();
            Hashtable hashTasks = new Hashtable();
            hashTasks = man.GetTaskList(customerid);
            string json = JsonConvert.SerializeObject(hashTasks);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void InitCustomer()
        {
            TaskManagement man = new TaskManagement();
            Hashtable hashCustomer = new Hashtable();
            hashCustomer = man.GetCustomer();
            string json = JsonConvert.SerializeObject(hashCustomer);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void GetTray(string itemno, string trayno)
        {
            TaskManagement man = new TaskManagement();
            Tray tray = new Tray();
            tray = man.GetTray(itemno, trayno);
            string json = JsonConvert.SerializeObject(tray);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void GetSum(int day)
        {
            TaskManagement man = new TaskManagement();
            Hashtable hashQty = new Hashtable();
            DateTime dt = DateTime.Now;
            if (day > 0)
            {
                for (int i = 0; i < day; ++i)
                {
                    dt = dt.AddDays(-1);
                    hashQty.Add(dt.ToString("yyyy-MM-dd"), man.GetSum(dt));
                }
            }
            else
            {
                hashQty.Add(dt.ToString("yyyy-MM-dd"), man.GetSum(DateTime.MinValue));
            }
            string json = JsonConvert.SerializeObject(hashQty);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void GetCurrentCustomer()
        {
            TaskManagement man = new TaskManagement();
            string result = man.GetCurrentCustomer();
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void GetQtyOfDay()
        {
            TaskManagement man = new TaskManagement();
            string result = man.GetQtyOfDay();
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void GetUsers()
        {
            TaskManagement man = new TaskManagement();
            string result = man.GetUsers();
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void GetQtyOfCustomer()
        {
            TaskManagement man = new TaskManagement();
            Hashtable result = man.GetQtyOfCustomers();
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        /// <summary>
        /// 获取客户信息（回传系统中客户）
        /// </summary>
        [WebMethod]
        public void GetCustomer()
        {
            TaskManagement man = new TaskManagement();
            Hashtable result = man.GetCustomer();
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void GetTasks(string datetime, string tray, string barcode)
        {
            TaskManagement man = new TaskManagement();
            List<Tasks> result = man.GetTasks(datetime, tray, barcode);
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void CreateData(string handle_task)
        {
            TaskManagement man = new TaskManagement();
            Tasks task = man.GetTask(handle_task);
            string pallet = null;
            for (int i = 0; i < task.SFC_LIST.Count; ++i)
            {
                pallet += task.SFC_LIST[i].SFC;
                pallet += ",";
            }
            pallet = pallet.Substring(0, pallet.Length - 1);
            DataIntegration data = new DataIntegration();
            DataTable mDt = data.CreateData(task.ITEM_NO, pallet);
            string json = JsonConvert.SerializeObject(mDt);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void ItemsInfo(string item,string bomno,string customer)
        {
            TaskManagement man = new TaskManagement();
            List<Items> itemList = new List<Items>();
            itemList = man.ItemsInfo(item, bomno, customer);
            string json = JsonConvert.SerializeObject(itemList);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void DataSources(string item,string bomno)
        {
            TaskManagement man = new TaskManagement();
            List<DataSources> dsList = man.DataSources(item, bomno);
            string json = JsonConvert.SerializeObject(dsList);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void Operations(string pipeline)
        {
            TaskManagement man = new TaskManagement();
            Hashtable hashOperations = man.Operations(pipeline);
            string json = JsonConvert.SerializeObject(hashOperations);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void Fields(string pipeline,string operation)
        {
            TaskManagement man = new TaskManagement();
            Hashtable hashOperations = man.Field(pipeline, operation);
            string json = JsonConvert.SerializeObject(hashOperations);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void Operators()
        {
            TaskManagement man = new TaskManagement();
            Hashtable hashOperations = man.Operators();
            string json = JsonConvert.SerializeObject(hashOperations);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void CreateDataSource(string info)
        {
            DataSources ds = JsonConvert.DeserializeObject<DataSources>(info);
            TaskManagement man = new TaskManagement();
            ResultTask result = man.CreateDataSource(ds);
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void GetTemplate(string item, string bomno)
        {
            TaskManagement man = new TaskManagement();
            List<DataTemplate> dtList = man.GetTemplate(item, bomno);
            string json = JsonConvert.SerializeObject(dtList);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void CreateDataTemplate(string info)
        {
            DataTemplate dt = JsonConvert.DeserializeObject<DataTemplate>(info);
            TaskManagement man = new TaskManagement();
            ResultTask result = man.CreateDataTemplate(dt);
            string json = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod]
        public void GetSyncData(string item, string pallet)
        {
            DataIntegration man = new DataIntegration();
            DataTable dt = man.CreateData(item, pallet);
            string json = JsonConvert.SerializeObject(dt);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(json);
            Context.Response.End();
        }
    }
}
