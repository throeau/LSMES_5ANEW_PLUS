using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using LSMES_5ANEW_PLUS.App_Base;
using LSMES_5ANEW_PLUS.Business;
using System.Data;
using System.Collections;

namespace LSMES_5ANEW_PLUS.WebService.ort
{
    /// <summary>
    /// task 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class task : System.Web.Services.WebService
    {

        [WebMethod]
        public void instask(string handle,string info)
        {
            List<TaskInspection> TaskList = new List<TaskInspection>();
            handle = Base64Helper.Base64Decode(handle);
            TaskList = JsonConvert.DeserializeObject<List<TaskInspection>>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.CreateTaskInspection(handle, TaskList)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void taskinfo(string type)
        {
            type = Base64Helper.Base64Decode(type);
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetTaskInfo(type)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void taskdetails(string handle)
        {
            handle = Base64Helper.Base64Decode(handle);
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetTaskDetails(handle)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void taskdetailsbybarcode(string barcode)
        {
            barcode = Base64Helper.Base64Decode(barcode);
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetTaskDetailsByBarcode(barcode)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void taskresult(string info)
        {
            ResultORT resultOrt = new ResultORT();
            resultOrt = ORT.SetTaskResults(JsonConvert.DeserializeObject<ResultTaskList>(Base64Helper.Base64Decode(info)));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(resultOrt));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void sampling(string info)
        {
            ResultORT resultOrt = new ResultORT();
            resultOrt = ORT.SetSampling(JsonConvert.DeserializeObject<List<SampleORT>>(Base64Helper.Base64Decode(info)));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(resultOrt));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void sampleInfo(string info)
        {
            DataTable mDt = new DataTable();
            mDt = ORT.GetSampleInfo(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(mDt));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void sampleDetails(string info)
        {
            DataTable mDt = new DataTable();
            mDt = ORT.GetSampleDetails(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(mDt));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void sampleRecieved(string info)
        {
            ResultORT resultOrt = new ResultORT();
            resultOrt = ORT.SetSampleRecieved(JsonConvert.DeserializeObject<List<SampleRecieved>>(Base64Helper.Base64Decode(info)));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(resultOrt));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void taskInfos(string info)
        {
            info = Base64Helper.Base64Decode(info);
            TaskSearch task = JsonConvert.DeserializeObject<TaskSearch>(info);
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetTaskInfo(task)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void sampleSFC(string info)
        {
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetSampleSFC(Base64Helper.Base64Decode(info))));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void Locked(string info)
        {
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.SetLocded(JsonConvert.DeserializeObject<List<TaskLocked>>(Base64Helper.Base64Decode(info)))));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void Unlock(string info)
        {
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.UnLock(JsonConvert.DeserializeObject<List<TaskLocked>>(Base64Helper.Base64Decode(info)))));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void InterceptME(string info)
        {
            //string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.PostQueues(Base64Helper.Base64Decode(info))));
            //info = Base64Helper.Base64Decode(info);
            ORT ort = new ORT();
            string result = JsonConvert.SerializeObject(ort.PostInterceptQueues(info, "intercept_queue",true));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
            //ORT.RecieveQueues();
        }
        [WebMethod]
        public void ReleaseME(string info)
        {
            string result = JsonConvert.SerializeObject(ORT.PostReleaseQueues(info, "release_queue"));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void TemplateInfo(string info)
        {
            List<TemplateSearch> templateList = new List<TemplateSearch>();
            templateList = JsonConvert.DeserializeObject<List<TemplateSearch>>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.TemplateInfo(templateList)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void TemplateSubmit(string info)
        {
            Templates template = new Templates();
            template = JsonConvert.DeserializeObject<Templates>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.TemplateSubmit(template)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void TemplateDetails(string info)
        {
            TemplateSearch template = new TemplateSearch();
            template = JsonConvert.DeserializeObject<TemplateSearch>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetTemplateDetails(template)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void UploadCycleData(string info)
        {
            CycleData data = new CycleData();
            data = JsonConvert.DeserializeObject<CycleData>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.AddCycleData(data.DATA, data.BARCODE, data.CREATOR)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void Bomno()
        {
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetBom()));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void BomTemplate(string info)
        {
            BomTemplate entity = JsonConvert.DeserializeObject<BomTemplate>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.Bom_Template(entity.BOMNO, entity.ITEM)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void BomTemplateSubmit(string info)
        {
            List<BomTemplate> entity = JsonConvert.DeserializeObject<List<BomTemplate>>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.BomTemplateSubmit(entity)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void ColumnCalculated(string info)
        {
            BomTemplate entity = JsonConvert.DeserializeObject<BomTemplate>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetColumnCalculated(entity)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void ColumnCalculatedSubmit(string info)
        {
            ParameterList paraList = JsonConvert.DeserializeObject<ParameterList>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.CalculatedSubmit(paraList)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void ReportTask(string info)
        {
            ReportTask task = JsonConvert.DeserializeObject<ReportTask>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.ReportTask(task)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void ReportSampleListByTaskID(string info)
        {
            string taskid = JsonConvert.DeserializeObject<string>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetSampleListByTaskID(taskid)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void ReportSampleDataByBarcode(string info)
        {
            string sfc = JsonConvert.DeserializeObject<string>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetSampleDataByBarcode(sfc)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void ReportSampleVoltageByBarcode(string info)
        {
            string sfc = JsonConvert.DeserializeObject<string>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetSamplePerformanceByBarcode(sfc)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void ReportCycleDataBySFC(string info)
        {
            SfcTask entity = JsonConvert.DeserializeObject<SfcTask>(Base64Helper.Base64Decode(info));
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(ORT.GetCycleDataBySFC(entity.SFC, entity.TASKID)));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void CycleDataJudge(string info)
        {
            //string sfc = Base64Helper.Base64Decode(info);
            string sfc = info;
            TemplateStandardSFC entity = ORT.GetTemplateStandardBySFC(sfc);
            DataTable mDt = ORT.GetTemplateStandard(entity);
            ResultORT resultOrt = new ResultORT();
            resultOrt.Result = ORT.CycleDataJudge(sfc, mDt) ? "success" : "fail";
            resultOrt.Informations = sfc;
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(resultOrt));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void GetSmapleState(string info)
        {
            info = Base64Helper.Base64Decode(info);
            if (string.IsNullOrEmpty(info)) return;
            SampleState sample = ORT.GetSampleState(info);
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(sample));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void GetSampleingCondition(string info)
        {
            info = Base64Helper.Base64Decode(info);
            if (string.IsNullOrEmpty(info)) return;
            Hashtable sample = ORT.SampleingCondition(info);
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(sample));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void AutoSamples(string info)
        {
            info = Base64Helper.Base64Decode(info);
            if (string.IsNullOrEmpty(info)) return;
            List<SampleInfo> samples = ORT.AutoGetSample(info);
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(samples));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void GetTestStandard (string info)
        {
            info = Base64Helper.Base64Decode(info);
            if (string.IsNullOrEmpty(info)) return;
            queryTestStandard entity = JsonConvert.DeserializeObject<queryTestStandard>(info);
            List<TestStandard> standards = ORT.GetTestStandard(entity.HANDLE_BOM, entity.HANDLE_TYPE);
            string result = Base64Helper.Base64Encode(JsonConvert.SerializeObject(standards));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void CreateTestStandards(string info)
        {
            info = Base64Helper.Base64Decode(info);

            ResultORT result = new ResultORT();
            if (string.IsNullOrEmpty(info))  result.Result = "fail";
            result = ORT.CreateTestStandards(JsonConvert.DeserializeObject<List<TestStandard>>(info));
            info = Base64Helper.Base64Encode(JsonConvert.SerializeObject(result));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(info);
            Context.Response.End();
        }
        [WebMethod]
        public void GetTestStandardBySample(string info)
        {
            info = Base64Helper.Base64Decode(info);
            queryTestStandard entity = JsonConvert.DeserializeObject<queryTestStandard>(info);
            List<TestStandard> result = ORT.GetTestStandardBySample(entity.BARCODE, entity.TYPE);
            info = Base64Helper.Base64Encode(JsonConvert.SerializeObject(result));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(info);
            Context.Response.End();
        }
        [WebMethod]
        public void CreateTestData(string info)
        {
            info = Base64Helper.Base64Decode(info);
            ResultORT result = ORT.CreateTestData(JsonConvert.DeserializeObject<List<TestData>>(info));
            info = Base64Helper.Base64Encode(JsonConvert.SerializeObject(result));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(info);
            Context.Response.End();
        }
        [WebMethod]
        public void GetThicknessBySFC(string info)
        {
            info = Base64Helper.Base64Decode(info);
            TestData result = ORT.GetThicknessBySFC(info);
            info = Base64Helper.Base64Encode(JsonConvert.SerializeObject(result));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(info);
            Context.Response.End();
        }
        [WebMethod]
        public void GetTestTypeBySFC(string info)
        {
            info = Base64Helper.Base64Decode(info);
            TestType result = ORT.GetTestTypeBySFC(info);
            info = Base64Helper.Base64Encode(JsonConvert.SerializeObject(result));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(info);
            Context.Response.End();
        }
    }
}
