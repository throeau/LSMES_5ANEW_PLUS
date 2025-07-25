using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

namespace LSMES_5ANEW_PLUS.App_Base
{
    /// <summary>
    /// 主数据
    /// </summary>
    public class EntityORT
    {
    }
    public class WorkArea
    {
        public string handle;
        public string area;
    }
    public class Operations
    {
        public string handle;
        public string operations;
    }
    public class TypeArea
    {
        public string handle;
        public string type_area;
        public string comments;
    }
    public class Bom
    {
        public string handle;
        public string bomno;
        public string itemno;
        public string types;
    }
    public class TypeTask
    {
        public string handle;
        public string types;
    }
    public class ORTTestItem
    {
        public string handle;
        public string inspection_name;
    }
    public class TaskState
    {
        public string state;
        public string description;
    }
    public class MasterOrt
    {
        public List<WorkArea> WorkAreaList { set; get; }
        public List<Operations> OperationsList { set; get; }
        public List<TypeArea> TypeAreaList { set; get; }
        public List<Bom> BomList { set; get; }
        public List<TypeTask> TypeTaskList { set; get; }
        public List<TaskState> TaskStateList { set; get; }
    }
    /// <summary>
    /// 返回结果
    /// </summary>
    public class ResultORT
    {
        public string Result { set; get; }
        public string Informations { set; get; }
    }
    /// <summary>
    /// 任务
    /// </summary>
    public class TaskInspection
    {
        public string handle { set; get; }
        public string handle_work_area { set; get; }
        public string handle_operation { set; get; }
        public string handle_types_area { set; get; }
        public string handle_bom { set; get; }
        public string handle_types_task { set; get; }
        public string handle_task_item { set; get; }
        public string state { set; get; }
        public string creator { set; get; }
    }
    /// <summary>
    /// 任务结果
    /// </summary>
    public class TaskNo
    {
        public string handle { set; get; }
    }
    public class ResultItem
    {
        public string handle { set; get; }
        public string result { set; get; }
    }
    public class ResultTaskList
    {
        public TaskNo task { set; get; }
        public List<ResultItem> ItemList { set; get; }
    }
    /// <summary>
    /// 取样样本
    /// </summary>
    public class SampleORT
    {
        public string handle_task { set; get; }
        public string handle_item { set; get; }
        public string barcode { set; get; }
    }
    /// <summary>
    /// 接收样本
    /// </summary>
    public class SampleRecieved
    {
        public string handle_task { set; get; }
        public string handle_task_details { set; get; }
        public string handle_sample { set; get; }
        public string creator { set; get; }
    }
    /// <summary>
    /// 任务查询
    /// </summary>
    public class TaskSearch
    {
        public string area { set; get; }
        public string type_area { set; get; }
        public string operation_name { set; get; }
        public string product_batch { set; get; }
        public string item_no { set; get; }
        public string state { set; get; }
    }
    /// <summary>
    /// 拦截任务
    /// </summary>
    public class TaskLocked
    {
        public string handle_task { set; get; }
        public string handle_sample { set; get; }
        public string comments { set; get; }
        public string state { set; get; }
        public string creator { set; get; }
        public string created_date_time
        {
            get
            {
                return DateTime.Now.ToString("s");
            }
        }
    }
    /// <summary>
    /// 拦截实例
    /// </summary>
    public class InterceptORT
    {
        public string MESSAGE_ID { set; get; }
        public string REQ_ID { set; get; }
        public string SEND_DATE_TIME { set; get; }
        public string ORT_NO { set; get; }
        public string SITE { set; get; }
        public string OPERATION { set; get; }
        public string INTERCEPT_OPERATION { set; get; }
        public List<InterceptSN> SN_LIST { set; get; }
    }
    /// <summary>
    /// 拦截码号
    /// </summary>
    public class InterceptSN
    {
        public string SN { set; get; }
    }
    /// <summary>
    /// 拦截结果
    /// </summary>
    public class ResultInterceptORT
    {
        public string RESULT { set; get; }
        public string MESSAGE { set; get; }
        public string INTERCEPT_TASK { set; get; }
        public string REQ_ID { set; get; }
    }
    /// <summary>
    /// 拦截放行
    /// </summary>
    public class ReleaseORT
    {
        public string MESSAGE_ID { set; get; }
        public string REQ_ID { set; get; }
        public string SEND_DATE_TIME { set; get; }
        public string SITE { set; get; }
        public string INTERCEPT_TASK { set; get; }
        public string REMARKS { set; get; }
    }
    /// <summary>
    /// 放行结果
    /// </summary>
    public class ResultReleaseORT
    {
        public string RESULT { set; get; }
        public string MESSAGE { set; get; }
        public string REQ_ID { set; get; }
    }
    /// <summary>
    /// 测试模板信息
    /// </summary>
    public class TemplateInfo
    {
        public string BOMNO { set; get; }
        public string INSPECTION_ITEM { set; get; }
        public string HANDLE { set; get; }
        public string TEMPLATE_NAME { set; get; }
        public string VERSION { set; get; }
        public string CREATOR { set; get; }
        public string IS_CURRENT { set; get; }
        public string CREATED_DATE_TIME { set; get; }
    }
    /// <summary>
    /// 查询测试模板
    /// </summary>
    public class TemplateSearch
    {
        public string HANDLE { set; get; }
        public string NAME { set; get; }
    }
    /// <summary>
    /// 模板明细
    /// </summary>
    public class TemplateDetails
    {
        public string COLUMN { set; get; }
        public string DESCRIPTION { set; get; }
        public string TYPE { set; get; }
    }
    /// <summary>
    /// 提交的 Template
    /// </summary>
    public class Templates
    {
        public TemplateInfo TEMPLATE_INFO { set; get; }
        public List<TemplateDetails> TEMPLATE_DETAILS = new List<TemplateDetails>();
    }
    /// <summary>
    /// 循环数据
    /// </summary>
    public class CycleData
    {
        public DataTable DATA { set; get; }
        public string HANDLE_TASK_DETAILS { set; get; }
        public string HANDLE_TEMPLATE { set; get; }
        public string BARCODE { set; get; }
        public string CREATOR { set; get; }
    }
    /// <summary>
    /// 型号BOM与模板
    /// </summary>
    public class BomTemplate
    {
        public string HANDLE { set; get; }
        public string BOMNO { set; get; }
        public string ITEM { set; get; }
        public string TEMPLATE { set; get; }
        public string VERSION { set; get; }
        public string CREATOR { set; get; }
    }
    /// <summary>
    /// 模板参数及参数标准
    /// </summary>
    public class Calculated
    {
        public string COLUMN_CALCULATED { set; get; }
        public string COLUMN_STORAGE { set; get; }
        public string TYPES { set; get; }
        public string DESCRIPTION { set; get; }
        public string OPERATOR1 { set; get; }
        public string STANDARD1 { set; get; }
        public string OPERATOR2 { set; get; }
        public string STANDARD2 { set; get; }
        public string OPERATOR3 { set; get; }
        public string STANDARD3 { set; get; }
    }
    /// <summary>
    /// 计算列参数标准基类
    /// </summary>
    abstract public class EntityParameter
    {
        abstract public string handle { get; set; }
        abstract public string item { get; set; }
        abstract public string parameter { get; set; }
        abstract public string type { get; set; }
    }
    /// <summary>
    /// 计算列参数标准字符类
    /// </summary>
    public class StringParameter : EntityParameter
    {
        string mHandle;
        string mParameter;
        string mType;
        string mItem;
        public override string handle
        {
            get
            {
                return mHandle;
            }

            set
            {
                mHandle = value;
            }
        }
        public override string item
        {
            set { mItem = value; }
            get { return mItem; }
        }
        public override string parameter
        {
            set { mParameter = value; }
            get { return mParameter; }
        }
        public override string type
        {
            get
            { return mType; }
            set
            { mType = value; }
        }
        public string value { get; set; }
    }
    /// <summary>
    /// 计算列参数标准数字类
    /// </summary>
    public class NumericParameter : EntityParameter
    {
        string mHandle;
        string mParameter;
        string mType;
        string mItem;
        public override string handle
        {
            get
            {
                return mHandle;
            }

            set
            {
                mHandle = value;
            }
        }
        public override string item
        {
            set { mItem = value; }
            get { return mItem; }
        }

        public override string parameter
        {
            set { mParameter = value; }
            get { return mParameter; }
        }
        public override string type
        {
            get { return mType; }
            set { mType = value; }
        }
        public string ge { get; set; }
        public string le { get; set; }
        public string gt { get; set; }
        public string lt { get; set; }
    }
    /// <summary>
    /// 计算列参数标准（一个模板内全部参数）
    /// </summary>
    public class ParameterList
    {
        public string item;
        public string bomno;
        public string version;
        public string creator;
        public List<StringParameter> string_para = new List<StringParameter>();
        public List<NumericParameter> numeric_para = new List<NumericParameter>();
    }
    /// <summary>
    /// 报表 —— 任务明细
    /// </summary>
    public class ReportTask
    {
        public string AREA { set; get; }
        public string TYPE { set; get; }
        public string OPERATION { set; get; }
        public string BOMNO { set; get; }
        public string ITEMNO { set; get; }
        public string BATCH { set; get; }
        public string STATE { set; get; }
        public string BARCODE { set; get; }
        public string DATE_TIME { set; get; }

    }
    public class SfcTask
    {
        public string TASKID { set; get; }
        public string SFC { set; get; }
    }
    /// <summary>
    /// 样本模板标准
    /// </summary>
    public class TemplateStandardSFC
    {
        public string TASK_ID { set; get; }
        public string ITEM_NO { set; get; }
        public string INSPECTION_ITEM { set; get; }
        public string TEMPLATE_NAME { set; get; }
        public string SFC { set; get; }
    }
    /// <summary>
    /// 样本状态（SAP ME中状态）
    /// </summary>
    public class SampleState
    {
        public string BARCODE { set; get; }
        public string STATE { set; get; }
        public string TYPE { set; get; }
        public string ITEM_NO { set; get; }
    }
    /// <summary>
    /// ORT 测试数据上传实例
    /// </summary>
    public class SampleData
    {
        public string HANDLE { set; get; }
        public string HANDLE_TASK { set; get; }
        public string HANDLE_TASK_DETAIL { set; get; }
        public string SN { set; get; }
        public string KEY_COLUMN { set; get; }
        public string VALUE_COLUMN { set; get; }
        public string INDEX_COLUMN { set; get; }
        public string STATUS { set; get; }
        public string CREATED_USER { set; get; }
        public string CREATED_DATE_TIME { set; get; }
    }
    public class SamapleDataColumns
    {
        public Hashtable SampleDataColumn { set; get; }
    }
    public class SamapleDataRows
    {
        public Hashtable SampleDataRow { set; get; }
    }
    /// <summary>
    /// 系统自动分配的样本
    /// </summary>
    public class AutoSamaples
    {
        private Hashtable hashSampleList = new Hashtable();
        private List<SampleInfo> listSample;

        public void AddSampleList(SampleInfo sample)
        {
            if (hashSampleList.ContainsKey(sample.CATALOG))
            {
                listSample = (List<SampleInfo>)hashSampleList[sample.CATALOG];
                listSample.Add(sample);
                hashSampleList[sample.CATALOG] = listSample;
            }
            else
            {
                hashSampleList.Add(sample.CATALOG, sample);
            }
        }
        public List<SampleInfo> GetSample(string catalog)
        {
            return (List<SampleInfo>)hashSampleList[catalog];
        }

        public string TASK_CREATED_DATE_TIME { set; get; }
    }
    public class SampleInfo
    {
        public string CATALOG { set; get; }
        public string SN { set; get; }
        public string PRODUCT_BATCH { set; get; }
        public string SHOP_ORDER { set; get; }
        public string CREATED_DATE_TIME { set; get; }
        public string STATE { set; get; }
    }
    public class SampleStrategy
    {
        public Hashtable CATALOG = new Hashtable();
        public string CREATED_DATE_TIME { set; get; }
    }
    public class TestType
    {
        public string HANDLE { set; get; }
        public string TEST_NAME { set; get; }
        public string STATE { set; get; }
        public string CREATED_USER { set; get; }
        public string CREATED_DATE_TIME { set; get; }
    }
    public class TestStandard
    {
        public string HANDLE { set; get; }
        public string HANDLE_TYPE { set; get; }
        public string HANDLE_BOM { set; get; }
        public string BOM { set; get; }
        public string ITEM_NO { set; get; }
        public string TEST_NAME { set; get; }
        public string LSL { set; get; }
        public string USL { set; get; }
        public string MID { set; get; }
        public string L_OFFSET { set; get; }
        public string U_OFFSET { set; get; }
        public string VERSION { set; get; }
        public string STATE { set; get; }
        public string CREATED_USER { set; get; }
        public string CREATED_DATE_TIME { set; get; }
        public string TEST_ITEM { set; get; }
    }
    public class queryTestStandard
    {
        public string HANDLE_BOM { set; get; }
        public string HANDLE_TYPE { set; get; }
        public string BARCODE { set; get; }
        public string TYPE { set; get; }
    }
    public class TestData
    {
        public string HANDLE { set; get; }
        public string HANDLE_SFC { set; get; }
        public string SFC { set; get; }
        public string THICKNESS { set; get; }
        public string RESULT_THICKNESS { set; get; }
        public string VERSION_THICKNESS { set; get; }
        public string VOLTAGE { set; get; }
        public string RESULT_VOLTAGE { set; get; }
        public string VERSION_VOLTAGE { set; get; }
        public string RESISTANCE { set; get; }
        public string RESULT_RESISTANCE { set; get; }
        public string VERSION_RESISTANCE { set; get; }
        public string CREATED_USER { set; get; }
        public string CREATED_DATE_TIME { set; get; }
        public string TEST_TYPE { set; get; }
    }
}