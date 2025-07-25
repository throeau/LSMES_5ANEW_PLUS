using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace LSMES_5ANEW_PLUS.App_Base
{
    public class Entity
    {
    }
    public abstract class ResponseResult { }
    public abstract class ResponseResultDetails { }
    public abstract class ResponseResultData { }

    /****************************************************************************************/
    /************************************** 新普 ********************************************/
    /****************************************************************************************/
    /// <summary>
    /// 新普发货数据
    /// </summary>
    public class SMP_Shipment
    {
        private List<SMP_Battery> _data = new List<SMP_Battery>();
        public string code { set; get; }
        public string appid { set; get; }
        public string secret { set; get; }
        public string timestamp { set; get; }
        public string sign { set; get; }
        public List<SMP_Battery> data 
        {
            set
            {
                _data = value;
            }
            get
            {
                return _data;
            }
        }
    }
    /// <summary>
    /// 新普单只电池数据
    /// </summary>
    public class SMP_Battery
    {
        public string cellname { set; get; }
        public string lotno { set; get; }
        public string group { set; get; }
        public string qalot { set; get; }
        public string capacity { set; get; }
        public string ocv { set; get; }
        public string imp { set; get; }
        public string kvalue { set; get; }
        public string testtime {set;get;}
        public string boxnumber { set; get; }
        public string palletnumber { set; get; }
        public string supplies { set; get; }
        public string smppn { set; get; }
        public string eta { set; get; }
        public string qty { set; get; }
        public string po { set; get; }
        public string cellname2nd { set; get; }
        public string segment1 { set; get; }
        public string segment2 { set; get; }
        public string segment3 { set; get; }
        public string segment4 { set; get; }
        public string segment5 { set; get; }
    }
    public class SMP_ResponseResult:ResponseResult
    {
        public string data { get; set; }
        //public string data { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string batchid { get; set; }
        public string ret { get; set; }
    }
    public class SMP_ResponseResultDetails: ResponseResultDetails
    {
        public string logid { get; set; }
        public string total { get; set; }
        public string success { get; set; }
        public string nosuccess { get; set; }
        public List<SMP_ResponseResultData> data = new List<SMP_ResponseResultData>();
        //public string data { get; set; }
    }

    public class SMP_ResponseResultData: ResponseResultData
    {
        public string cellname { get; set; }
        public string checkflag { get; set; }
        public string checkmessage { get; set; }
    }
    /******************************************************************************************/
    /************************************** 欣旺达 ********************************************/
    /******************************************************************************************/

    /// <summary>
    /// 欣旺达发货数据
    /// </summary>
    class SCUD_OcvData
    {
        public List<SCUD_cellOcvData> cellOcvDataList = new List<SCUD_cellOcvData>();

        public string dataType { get; set; }
        public string supplierId { get; set; }
        public string appKey { get; set; }
        public string supplierName { get; set; }
        public string supplierCode { get; set; }
        public string passWord { get; set; }
        //public List<SCUD_cellOcvData> cellOcvDataList
        //{
        //    get
        //    {
        //        return cellOcvDataLists;
        //    }
        //    set
        //    {
        //        cellOcvDataLists = value;
        //    }
        //}
    }
    /// <summary>
    /// 欣旺达单只电池数据
    /// </summary>
    class SCUD_cellOcvData
    {
        public List<SCUD_testItems> testItemList = new List<SCUD_testItems>();
        public string barName { get; set; }
        public string barCode { get; set; }
        public string cellSn { get; set; }
        public string productLine { get; set; }
        public string asnSn { get; set; }
        public string testTime { get; set; }
        //public List<SCUD_testItems> testItemList
        //{
        //    get
        //    {
        //        return testItems;
        //    }
        //    set
        //    {
        //        testItems = value;
        //    }
        //}
    }
    /// <summary>
    /// 欣旺达单项测试池数据
    /// </summary>
    class SCUD_testItems
    {
        public string testItem { get; set; }
        public double lowerLimit { get; set; }
        public double upperLimit { get; set; }
        public string unit { get; set; }
        public double testValue { get; set; }
        public string testResult { get; set; }
        public string remark { get; set; }
        public string swdPn { get; set; }
        public string palletSn { get; set; }
    }
    class SCUD_responseResult
    {
        public string errorMsg { get; set; }
        public string successMsg { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public string date { get; set; }
        public string data { get; set; }
        public string year { get; set; }
    }
    class SUNWODA_responseResult
    {
        public int statusCode { get; set; }
        public string message { get; set; }
    }
    /// <summary>
    /// 客户端上传数据（单体）
    /// </summary>
    class BatterySunwoda
    {
        public string projectName { get; set; }
        public string projectCode { get; set; }
        public string productLine { get; set; }
        public string barcode { get; set; }
        public string capacity { get; set; }
        public string capacityUnit { get; set; }
        public string capacityLowerLimit { get; set; }
        public string capacityUpperLimit { get; set; }
        public string kvalue { get; set; }
        public string kvalueUnit { get; set; }
        public string kvalueLowerLimit { get; set; }
        public string kvalueUpperLimit { get; set; }
        public string voltage { get; set; }
        public string voltageUnit { get; set; }
        public string voltageLowerLimit { get; set; }
        public string voltageUpperLimit { get; set; }
        public string resistance { get; set; }
        public string resistanceUnit { get; set; }
        public string resistanceLowerLimit { get; set; }
        public string resistanceUpperLimit { get; set; }
        public string testtime { get; set; }
        public string result { get; set; }
        public string remark { get; set; }
        public string lotno { get; set; }
        public string swdpn { get; set; }
        public string palletsn { get; set; }
        public string asnsn { get; set; }

    }
    class BatteryListSunwoda
    {
        public List<BatterySunwoda> batterys = new List<BatterySunwoda>();
    }
    class TokenSunwoda
    {
        public int statusCode { get; set; }
        public string message { get; set; }
    }

    /******************************************************************************************/
    /************************************** Zebra *********************************************/
    /******************************************************************************************/

    /// <summary>
    /// Zebra数据回传，老版本
    /// </summary>
    //public class Zebra_testItems
    //{
    //    public string cellSn { get; set; }
    //    public string testDate { get; set; }
    //    public string voltageValue { get; set; }
    //    public string capacity { get; set; }
    //    public string cellBatch { get; set; }
    //    public string impedance { get; set; }
    //}
    /// <summary>
    /// Zebra数据回传，新版本
    /// </summary>
    public class Zebra_testItems
    {
        public string supplierName { get; set; }
        public string cellPartNumber { get; set; }
        public string cellSN { get; set; }
        public string cellType { get; set; }
        public string cellDateCode { get; set; }
        public string cellLotNumber { get; set; }
        public string cellFactoryID { get; set; }
        public string cellOCVoltage { get; set; }
        public string cellACImpedance { get; set; }
        public string cellCapacity { get; set; }
        public string cellKvalue { get; set; }
        public string cellProLineNO { get; set; }
        public string cellWorkVoltage { get; set; }
    }

    public class Zebra_Shipment
    {
        private List<Zebra_testItems> testItems = new List<Zebra_testItems>();
        public List<Zebra_testItems> testItemList
        {
            get
            {
                return testItems;
            }
            set
            {
                testItems = value;
            }
        }
    }
    public class Zebra_ResponseResult: ResponseResult
    {
        public int count { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
        public string ret { get; set; }

    }
}

/******************************************************************************************/
/**************************************** OCV *********************************************/
/******************************************************************************************/
public class BatteryPerformance
{
    public string DTYPE { get; set; }
    public string PTYPE { get; set; }
    public List<OCV> Data = new List<OCV>();
}
//public class OCV1
//{
//    public string DTYPE { get; set; }
//    public string CREATED_DATE_TIME { get; set; }
//    public string BOMNO { get; set; }
//    public string SIZENO { get; set; }
//    public string BATTERYNO { get; set; }
//    public string EQUIPMENTNO { get; set; }
//    public string CHANNEL { get; set; }
//    public string VOLTAGE { get; set; }
//    public string RESISTANCE { get; set; }
//    public string TESTDATE { get; set; }
//    public string LEVEL { get; set; }
//    public string CARRIOR { get; set; }
//    public string POSITION { get; set; }
//    public string VOLTAGE_UPPER { get; set; }
//    public string VOLTAGE_LOWER { get; set; }
//    public string RESISTANCE_UPPER { get; set; }
//    public string RESISTANCE_LOWER { get; set; }
//}

public class OCV
{
    public string CREATED_DATE_TIME { get; set; }
    public string BOMNO { get; set; }
    public string SIZENO { get; set; }
    public string BATTERYNO { get; set; }
    public string EQUIPMENTNO { get; set; }
    public string CHANNEL { get; set; }
    public string VOLTAGE { get; set; }
    public string RESISTANCE { get; set; }
    public string DELTAV { get; set; }
    public string DELTAT { get; set; }
    public string KVALUE { get; set; }
    public string TESTDATE { get; set; }
    public string LEVEL { get; set; }
    public string CARRIOR { get; set; }
    public string POSITION { get; set; }
    public string VOLTAGE_UPPER { get; set; }
    public string VOLTAGE_LOWER { get; set; }
    public string RESISTANCE_UPPER { get; set; }
    public string RESISTANCE_LOWER { get; set; }
    public string DELTAV_UPPER { get; set; }
    public string DELTAV_LOWER { get; set; }
    public string DELTAV_AVG { get; set; }
    public string KVALUE_UPPER { get; set; }
    public string KVALUE_LOWER { get; set; }
    public string KVALUE_AVG { get; set; }
    public string SIGMA { get; set; }
}
//public class OCV3
//{
//    public string DTYPE { get; set; }
//    public string CREATED_DATE_TIME { get; set; }
//    public string BOMNO { get; set; }
//    public string SIZENO { get; set; }
//    public string BATTERYNO { get; set; }
//    public string EQUIPMENTNO { get; set; }
//    public string CHANNEL { get; set; }
//    public string VOLTAGE { get; set; }
//    public string RESISTANCE { get; set; }
//    public string DELTAV { get; set; }
//    public string KVALUE { get; set; }
//    public string TESTDATE { get; set; }
//    public string LEVEL { get; set; }
//    public string CARRIOR { get; set; }
//    public string POSITION { get; set; }
//    public string VOLTAGE_UPPER { get; set; }
//    public string VOLTAGE_LOWER { get; set; }
//    public string RESISTANCE_UPPER { get; set; }
//    public string RESISTANCE_LOWER { get; set; }
//    public string DELTAV_UPPER { get; set; }
//    public string DELTAV_LOWER { get; set; }
//    public string DELTAV_AVG { get; set; }
//    public string KVALUE_UPPER { get; set; }
//    public string KVALUE_LOWER { get; set; }
//    public string KVALUE_AVG { get; set; }
//    public string SIGMA { get; set; }
//}
//public class OCV4
//{
//    public string DTYPE { get; set; }
//    public string CREATED_DATE_TIME { get; set; }
//    public string BOMNO { get; set; }
//    public string SIZENO { get; set; }
//    public string BATTERYNO { get; set; }
//    public string EQUIPMENTNO { get; set; }
//    public string CHANNEL { get; set; }
//    public string VOLTAGE { get; set; }
//    public string RESISTANCE { get; set; }
//    public string TESTDATE { get; set; }
//    public string LEVEL { get; set; }
//    public string CARRIOR { get; set; }
//    public string POSITION { get; set; }
//    public string VOLTAGE_UPPER { get; set; }
//    public string VOLTAGE_LOWER { get; set; }
//    public string RESISTANCE_UPPER { get; set; }
//    public string RESISTANCE_LOWER { get; set; }
//}
public class CustomerDS
{
    public string MODEL { get; set; }
    public string PN { get; set; }
    public string BOMNO { get; set; }
    public string PO { get; set; }
    public string LOT { get; set; }
    public string SHIPDATE { get; set; }
    public string BOX_QTY { get; set; }
    public string BATTERY_QTY { get; set; }
    public string WEEK { get; set; }
    public string MODEL2 { get; set; }
    public string PN2 { get; set; }
}
/*************************************************************************************************************/
/**************************************** BatteryList 2021/07/15 *********************************************/
/*************************************************************************************************************/
/// <summary>
/// Cell_SN_List 供PACK线下载 
/// </summary>
public class EntityBattery
{
    public string RESULT { get; set; }
    public string MESSAGE { get; set; }
    public string REQ_ID { get; set; }
    public string CARRIER_BATCH_NO { get; set; }
    public List<CellSn> SN_LIST = new List<CellSn>();
}
public class CellSn
{
    public string SN { get; set; }
}
/******************************************************************************************************************/
/**************************************** 客户信息（通用） 2021/07/15 *********************************************/
/******************************************************************************************************************/
public class CustomerCommon
{
    public string BOMNO { get; set; }
    public string PN { get; set; }
    public string SUPPLIER { get; set; }
    public string VERSION { get; set; }
    public string CREATEDATE { get; set; }
    public string VENDOR { get; set; }
}
/*************************************************************************************************************************************/
/**************************************** 电芯系统包装箱请求信息（力神LSMES） 2022/01/13 *********************************************/
/*************************************************************************************************************************************/
public class CellBoxInfo
{
    public string ITEM_NO { get; set; }
    public string ORDER_NO { get; set; }
    public string BOMNO { get; set; }
    public string BOXID { get; set; }
}
public class CellBoxList
{
    public List<CellBoxInfo> BOX_LIST = new List<CellBoxInfo>();
}
/*************************************************************************************************************************************/
/**************************************** 电芯系统包装箱信息返回（力神LSMES） 2022/01/13 *********************************************/
/*************************************************************************************************************************************/
public class CellInfo
{
    public string ITEM_NO { get; set; }
    public string ORDER_NO { get; set; }
    public string BOMNO { get; set; }
    public string BATTERYNO { get; set; }
    public string E5VOLTAGE1 { get; set; }
    public string E5TESTTIME { get; set; }
    public string E6VOLTAGE2 { get; set; }
    public string E6TESTTIME { get; set; }
    public string STATUS { get; set; }
}
public class CellInfoList
{
    public string BOXIID { get; set; }
    public List<CellInfo> CELL_LIST = new List<CellInfo>();
}
/**************************************************************************************************************************************/
/**************************************** 电芯（LSMES）导入PACK（SAP）基本信息 2022/01/23 *********************************************/
/**************************************************************************************************************************************/
public class EntitySynCellPack
{
    public string WEBSITE { get; set; }
    public string EQUIPMENT { get; set; }
    public string LINENO { get; set; }
    public string BOMNO { get; set; }
    public string ITEMNO { get; set; }
    public string CUSTOMERNO { get; set; }
    public string LABELCODE { get; set; }
    public string BOXID { get; set; }
    public string OPERATORS { get; set; }
    public string ORDERNO { get; set; }
}
/**************************************************************************************************************************************/
/*************************************************** 系统返回异常的基本信息 2022/01/23 ************************************************/
/**************************************************************************************************************************************/
public class EntityException
{
    public bool IsException { get; set; }
    public string ExpMessage { get; set; }
}
/**************************************************************************************************************************************/
/*************************************************** SAP ME 系统返回工作中心 2022/02/24 ***********************************************/
/**************************************************************************************************************************************/
public class EntityWorkCenter
{
    public string WORK_CENTER { get; set; }
    public string DESCRIPTION { get; set; }
}
public class WorkCenter
{
    public List<EntityWorkCenter> WORK_CENTER_LIST = new List<EntityWorkCenter>();
}
/**************************************************************************************************************************************/
/*************************************************** SAP ME 系统返回设备编号 2022/02/24 ***********************************************/
/**************************************************************************************************************************************/
public class EntityResource
{
    public string RESOURCE { get; set; }
    public string DESCRIPTION { get; set; }
}
public class Resource
{
    public List<EntityResource> Resource_LIST = new List<EntityResource>();
}

/**************************************************************************************************************************************/
/*************************************************** 接收 SAP ME 华为数据回传 2022/02/24 ***********************************************/
/**************************************************************************************************************************************/
/// <summary>
/// HW 电池数据类
/// </summary>
public class BatteryParameter
{
    public string OPERATION { get; set; }
    public string CUSTOMER_OPERATION { get; set; }
    public string PARAM_NO { get; set; }
    public string PARAM_VALUE { get; set; }
    public string LS_PARAM_DESC { get; set; }
    public string CUSTOMER_PARAM_DESC { get; set; }
    public string LS_LOWER_LIMIT { get; set; }
    public string LS_UPPER_LIMIT { get; set; }
    public string CUSTOMER_LOWER_LIMIT { get; set; }
    public string CUSTOMER_UPPER_LIMIT { get; set; }
    public string UNIT { get; set; }
    public string TEST_START_TIME { get; set; }
    public string TEST_END_TIME { get; set; }
    public string TEST_EQUIP { get; set; }
    public string TEST_RESULT { get; set; }
}
/// <summary>
/// HW 电池类
/// </summary>
public class Battery
{
    public string SN { get; set; }
    public string PRODUCT_LINE { get; set; }
    public string ORDER_NO { get; set; }

    public List<BatteryParameter> DC_INFO = new List<BatteryParameter>();
}
public class HW_Recieved
{
    public string MESSAGE_ID { get; set; }
    public string REQ_ID { get; set; }
    public string SITE { get; set; }
    public string SEND_TIME { get; set; }
    public string ITEM { get; set; }
    public string ITEM_TYPE { get; set; }
    public string CUSTOMER_NAME { get; set; }
    public string CUSTOMER_ITEM { get; set; }
    public string FACTORY_CODE { get; set; }
    public string SUPPLIES_NO { get; set; }
    public string BOX_NO { get; set; }

    public List<Battery> SN_LIST = new List<Battery>();
}
/*****************************************************************************************************************************************/
/***************************************************** 接收 SAP 华为电池数据 2022/04/28 **************************************************/
/*****************************************************************************************************************************************/
/// <summary>
/// 华为数据格式（JSON）中 uutInfo 类
/// </summary>
public class UutInfo
{
    public string orderNumber { get; set; }
    public string testOjectType { get; set; }
    public string testMethod { get; set; }
    public string materialCode { get; set; }
    public string materialSerialNumber { get; set; }
    public string supplierCode { get; set; }
    public string lotCode { get; set; }
    public string materialMould { get; set; }
    public string materialMouldCavity { get; set; }
    public string description { get; set; }
    public string dateCode { get; set; }
    public string materialColor { get; set; }
}
/// <summary>
/// 华为数据格式（JSON）中 uutResult 类
/// </summary>
public class UutResult
{
    private string mResult;
    public string Operator { get; set; }
    public string testSequence { get; set; }
    public string testStation { get; set; }
    public long testStartTime { get; set; }
    public long testEndTime { get; set; }
    public string testResult {
        get
        {
            return mResult;
        }
        set
        {
            switch (value)
            {
                case "OK":
                    mResult = "passed";
                    break;
            }
        }
    }
}
/// <summary>
/// 华为数据格式（JSON）中 testItem 类
/// </summary>
public class TestItem
{
    private string mResult;
    public string testItemNumber { get; set; }
    public string testItemName { get; set; }
    public long testStartTime { get; set; }
    public long testEndTime { get; set; }
    public string testResult
    {
        get
        {
            return mResult;
        }
        set
        {
            switch (value)
            {
                case "OK":
                    mResult = "passed";
                    break;
            }
        }
    }
    public string testResultDescription { get; set; }
    public string isValueFlag { get; set; }
    public double testValue { get; set; }
    public double testUpperLimit { get; set; }
    public double testLowerLimit { get; set; }
}
/// <summary>
/// 华为数据回传最外围结构
/// </summary>
public class Products
{
    public string uploadType { get; set; }
    public string factoryCode { get; set; }
    public string workshop { get; set; }
    public string group { get; set; }
    public string productLine { get; set; }
    public UutInfo uutInfo { get; set; }
    public UutResult uutResult { get; set; }

    public List<TestItem> testItemList = new List<TestItem>();
}
/// <summary>
/// 华为数据回传返回结果
/// </summary>
public class HW_Result
{
    public int code { get; set; }
    public string message { get; set; }
    public object data { get; set; }
}
/*************************************************************************************************************************************/
/***************************************************** 飞毛腿回传电池数据 2022/05/16 **************************************************/
/*************************************************************************************************************************************/

/*************************************************************************************************************************************/
/******************************************************* 产线基础配置 2022/05/27 *****************************************************/
/*************************************************************************************************************************************/
public class CapacityBase
{
    public string OPERATION { get; set; }
    public string DESCRIPTION { get; set; }
    public string EQUIPMENT_NO { get; set; }
    public string ITEM_NO { get; set; }
    public string CAPACITY { get; set; }
}
/*************************************************************************************************************************************/
/******************************************************* 物料（ITEM） 2022/06/07 *****************************************************/
/*************************************************************************************************************************************/
public class ItemNo
{
    public string ITEM { get; set; }
    public string DESCRIPTION { get; set; }
}
public class ItemList
{
    public List<ItemNo> ItemLists = new List<ItemNo>();
}
/***************************************************************************************************************************************/
/******************************************************* Desay 电池信息 2022/06/15 *****************************************************/
/***************************************************************************************************************************************/
public class BatteryDesay
{
    public string barcode { get; set; }
    public int Capacity { get; set; }
    public string date0 { get; set; }
    public string date { get; set; }
    public double ir { get; set; }
    public double ocv { get; set; }
    public double ocv0 { get; set; }
    public double KData { get; set; }
}
public class BatteryDesayList
{
    public List<BatteryDesay> BatteryList = new List<BatteryDesay>();
}
public class BatteryDesay2
{
    public string barcode { get; set; }
    public string date { get; set; }
    public double ir { get; set; }
    public double ocv { get; set; }
    public int result { get; set; }
    public double KData { get;set;}
}
public class BatteryDesayList2
{
    public List<BatteryDesay2> BatteryList = new List<BatteryDesay2>();
}
public class ResultLogin
{
    public string IsLogin { get; set; }
    public string App_id { get; set; }
    public string App_key { get; set; }
    public string LoginDate { get; set; }
    public string TimeOutDate { get; set; }
    public string Ticket { get; set; }
    public string Error { get; set; }
}
public class ResultPost
{
    public bool Success { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }

    public List<ResultBattery> Data = new List<ResultBattery>();
}
/// <summary>
/// 德赛更新，2023-11-20
/// </summary>
public class ResultPost2
{
    public bool success { get; set; }
    public string message { get; set; }
    public int code { get; set; }
    public string result { get; set; }
    public object timestamp { get; set; } 
}
public class ResultBattery
{
    public string barcode { get; set;}
    public string result { get; set; }
    public string error { get; set; }
}
public class EntityLogin
{
    public string app_id { get; set; }
    public string app_key { get; set; }
}
public class EntityLogin2
{
    public string username { get; set; }
    public string password { get; set; }
    public string captcha { get; set; }
    public string checkKey { get; set; }
}
public class License
{
    public string license { get; set; }
    public string customer { get; set; }
}
public class Licenses
{
    public List<License> Data = new List<License>();
}
/// <summary>
/// Desay 2023-11-1 系统更新
/// </summary>
public class BatteryDesay3
{
    public string PRODUCTCODE { get; set; } // DSY项目代号
    public string CELL_BARCODE { get; set; }    // 电芯条码
    public string CAPACITY_BARCODE { get; set; }    // 电芯容量码
    public string MODEL_NUMBER { get; set; }    // ATL型号
    public string CARTON_NUMBER { get; set; }   // ATL箱号
    public string BATCH_NUM { get; set; }   // ATL lot no.
    public string CAPACITY { get; set; }    // 容量
    public string OCV { get; set; } // 电压
    public string IR { get; set; }  // 电阻
    public string K_VALUE { get; set; } // K值
    public string FDATE { get; set; }   // OCV测试时间
    public string BIN { get; set; } // 分bin(如无分bin则空着)
    public string DELIVERY_TIME // 数据抛送日期
    {
        get
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
    public string REMARK { get; set; }  // 备注
}
/// <summary>
/// Desay 2023-11-1 系统更新
/// </summary>
public class BatteryDesayList3
{
    public string type  // 固定为FA_CELL_SUPPLIER_DATA_UPLOAD
    {
        get
        {
            return "FA_CELL_SUPPLIER_DATA_UPLOAD";
        }
    }
    public string vendor  // 供应商名
    {
        get
        {
            return "LISHEN";
        }
    }
    public string productcode { get; set; } // DSY项目代号
    public List<BatteryDesay3> data = new List<BatteryDesay3>();
}
public class TokenDesay
{
    public string success { set; get; }
    public string message { set; get; }
    public string code { get; set; }
    public TokenResultDesay result { set; get; }
    public string timestamp { set; get; }
}
public class TokenResultDesay
{
    public object userInfo { set; get; }
    public string token { set; get; }
}
public class BomDesay
{
    public string BOMNO { set; get; }
    public string REMARK { set; get; }
    public string VERSION { set; get; }
}
/***************************************************************************************************************************************/
/****************************************************** 闻泰 电池包装信息 2022/10/09 ***************************************************/
/***************************************************************************************************************************************/
public class BoxWT
{
    public string BOXID { set; get; }
    public string QTY { set; get; }
    public string BOMNO { set; get; }
    public string VENDOR { set; get; }
    public string PO { set; get; }
    public string PACKAGEDATE { set; get; }
    public string Description { set; get; }
    public string LOTDATE { set; get; }
}
/***************************************************************************************************************************************/
/****************************************************** Powergene 电池包装信息 2024/7/1 ************************************************/
/***************************************************************************************************************************************/
public class BoxPowergene
{
    public string PO_NUMBER { set; get; }
    public string CUSTOMER_PN { set; get; }
    public string PART_NAME { set; get; }
    public string TYPE { set; get; }
    public string CAPACITY_ENERGY { set; get; }
    public string VERDOR_PN { set; get; }
    public string VENDOR_CODE { set; get; }
    public string V_QTY { set; get; }
    public string ROHS { set; get; }
    public string VENDOR_NAME { set; get; }
    public string DATE_CODE { set; get; }
}
/***************************************************************************************************************************************/
/********************************************************* Anker 电池包装信息 2024/7/1 *************************************************/
/***************************************************************************************************************************************/
public class BoxAnker
{
    public string SUPPLIER_NUMBER { set; get; }
    public string PO_CUSTOMER { set; get; }
    public string CUSTOMER_ITEM { set; get; }
    public string CUSTOMER_ITEM_DESCRIPTION { set; get; }
    public string QTY { set; get; }
    public string PRODUCT_DATE { set; get; }
    public string GROUP_INFO { set; get; }
    public string NOTE { set; get; }
}
/***************************************************************************************************************************************/
/********************************************************** Hipot 信息 2022/12/09 ******************************************************/
/***************************************************************************************************************************************/
public class HiPot
{
    public string barcode { set; get; }
    public string hipot { set; get; }
    public string energy { set; get; }
    public string energy2 { set; get; }
    public string datetime { set; get; }
}
public class ResultHiPot
{
    public string Result { set; get; }
    public string Information { set; get; }
}
/******************************************************************************************************************************************/
/********************************************************** 飞毛腿电池信息 2023/5/22 ******************************************************/
/******************************************************************************************************************************************/
public class BatteryPowerBankSCUD
{
    public string cellSn { get; set; }
    public double voltageValue { get; set; }
    public string testDate { get; set; }
    public string PB_SN { get; set; }
    public string CO_ITEM_CODE { get; set; }
    public string CO_ITEM_NAME { get; set; }
    public string CO_ITEM_SPEC { get; set; }
    public string PN_SN { get; set; }
    public string GEAR { get; set; }
    public string IRValue { get; set; }
}
public class BatteryLaptopSCUD
{
    public string E_ELECTRON_SN { get; set; }
    public string E_LOTSN { get; set; }
    public string E_OCV { get; set; }
    public string E_TEST_TIME { get; set; }
    public string E_IR { get; set; }
    public string E_DCR { get; set; }
    public string Pallet_ID { get; set; }
    public string E_UPLOAD_TIME { get; set; }
}
public class PowerBankSCUD
{
    public string appid { get; set; }
    public string pwd { get; set; }
    public List<BatteryPowerBankSCUD> data = new List<BatteryPowerBankSCUD>();
}
public class SCUD_ResponseResult
{
    public object data { get; set; }
    public string msg { get; set; }
    public object status { get; set; }
}
public  class SCUD_Result
{
    public string RESULT { set; get; }
    public string MSG { set; get; }
    public string CREATED_DATE_TIME
    {
        get
        {
            return DateTime.Now.ToString();
        }
    }
}
public class Item_no
{
    public string BOMNO { set; get; }
    public string ITEM_NO { set; get; }
}
/*****************************************************************************************************************************************/
/****************************************************** TWS(明美) 电池信息 2023/06/12 ****************************************************/
/*****************************************************************************************************************************************/
public class BatteryTWS
{
    public string supplierName { get; set; }
    public string cellPartNumber { get; set; }
    public string cellSN { get; set; }
    public string cellType { get; set; }
    public string cellDateCode { get; set; }
    public string cellLotNumber { get; set; }
    public string cellFactoryID { get; set; }
    public string cellOCVoltage { get; set; }
    public string cellACImpedance { get; set; }
    public string cellCapacity { get; set; }
    public string cellKvalue { get; set; }
    public string cellProLineNO { get; set; }
    public string cellWorkVoltage { get; set; }
    public string cellTestTime { get; set; }
}
public class TWS_ResponseResult
{
    public int count { get; set; }
    public string code { get; set; }
    public string msg { get; set; }
    public object data { get; set; }
}

/*****************************************************************************************************************************************/
/********************************************************* 电极追溯数据 2023/09/08 *******************************************************/
/*****************************************************************************************************************************************/
public class ColumnPole
{
    public string Key { get; set; }
    public string Value { get; set; }
}
public class RowPole
{
    public List<ColumnPole> Columns { get; set; }
}
public class ReslutPole
{
    public int count { get; set; }
    public string result { get; set; }
    public string msg { get; set; }
}
/*****************************************************************************************************************************************/
/********************************************************* 性能追溯数据 2023/09/12 *******************************************************/
/*****************************************************************************************************************************************/
public class ColumnPerformance
{
    public string Key { get; set; }
    public string Value { get; set; }
}
public class RowPerformance
{
    public List<ColumnPerformance> Columns { get; set; }
}
public class ReslutPerformance
{
    public int count { get; set; }
    public string result { get; set; }
    public string msg { get; set; }
}
/*****************************************************************************************************************************************/
/********************************************************* 回传任务管理 2023/10/17 *******************************************************/
/*****************************************************************************************************************************************/
public class TaskSimple
{
    public string sn { get; set; }
    public string handle { get; set; }
    public string customer { get; set; }
    public string item_no { get; set; }
    public string qty_sfc { get; set; }
    public string qty { get; set; }
    public string created_user { get; set; }
    public string created_date_time { get; set; }
}

public class Items
{
    public string HANDLE { get; set; }
    public string ITEM { get; set; }
    public string BOMNO { get; set; }
    public string CUSTOMER { get; set; }
    public string CREATED_USER { get; set; }
    public string CREATED_DATE_TIME { get; set; }
}
public class DataSources
{
    private string handle_item;
    public string HANDLE { get; set; }
    public string HANDLE_ITEM { get; set; }
    public string ITEM { get; set; }
    public string BOMNO { get; set; }
    public string DATATABLE { get; set; }
    public string FIELD { get; set; }
    public string DATA_PROPERTY_NAME { get; set; }
    public string PARAMETER { get; set; }
    public string OPERATORS { get; set; }
    public string PRECISION { get; set; }
    public string LSL { get; set; }
    public string USL { get; set; }
    public string UNIT { get; set; }
    public string STATE { get; set; }
    public string REMARKS { get; set; }
    public string PIPELINE { get; set; }
    public string CREATED_USER { get; set; }
    public string CREATED_DATE_TIME { get; set; }
}
public class ResultTask
{
    public string RESULT { get; set; }
    public string MSG { get; set; }
}
public class DataTemplate
{
    public string HANDLE { set; get; }
    public string HANDLE_ITEM { set; get; }
    public string HEADLINE { set; get; }
    public string SN { set; get; }
    public string STATE { set; get; }
    public string REMARKS { set; get; }
    public string CREATED_USER { set; get; }
    public string CREATED_DATE_TIME { set; get; }
}
public class StatementsLog
{
    private string statement;
    public string HANDLE { set; get; }
    public string ITEM { set; get; }
    public string BOMNO { set; get; }
    public string STATEMENTS
    {
        set
        {
            statement = value;
        }
        get
        {
            return statement.Replace("'", "''");
        }
    }
    public string PIPELINE { set; get; }
    public string TYPE { set; get; }
    public string TASKNO { set; get; }
    public string REMARKS { set; get; }
    public string CREATED_DATE_TIME { set; get; }
}
public class EmailInfo
{
    public string CUSTOMER_ID { set; get; }
    public string CUSTOMER { set; get; }
    public string USERNAME { set; get; }
    public string EMAIL { set; get; }
}
/// <summary>
/// 任务管理中的用户信息
/// </summary>
public class UsersInfos
{
    public string HANDLE { set; get; }
    public string USERNAME { set; get; }
    public string PASSWORD { set; get; }
    public string EMAIL { set; get; }
    public string DEPT { set; get; }
    public string STATE { set; get; }
    public string REMARKS { set; get; }
    public string CREATED_DATE_TIME { set; get; }
}
/// <summary>
/// 任务管理中的客户信息
/// </summary>
public class CUSTOMERS
{
    public string HANDLE { set; get; }
    public string CUSTOMER { set; get; }
    public string CREATED_DATE_TIME { set; get; }
}
/// <summary>
/// 
/// </summary>
public class DATA_SUPPLEMENT
{
    public string ITEM_NO { set; get; }
    public string DATA_TYPE { set; get; }
    public string QTY { set; get; }
}
/// <summary>
/// 任务管理中的用户与客户绑定关系
/// </summary>
public class EMAIL_GROUP
{
    public string HANDLE_GROUP;
    public string HANDLE_CUSTOMER;
    public string HANDLE_EMAIL;
    public string CUSTOMER;
    public string USERNAME;
}
public class HtmlTable
{
    StringBuilder mTD = new StringBuilder();
    StringBuilder mTR = new StringBuilder();
    StringBuilder mTH = new StringBuilder();
    StringBuilder mTable = new StringBuilder();
    /// <summary>
    /// 添加 TD，
    /// </summary>
    public string TD
    {
        set
        {
            mTD.Append(string.Format("<td>{0}</td>", value));
        }
        get
        {
            return mTD.ToString();
        }
    }
    /// <summary>
    /// 将若干 TD 添加至 TR 中
    /// </summary>
    public string TR
    {
        set
        {
            mTR.Append(string.Format("<tr>{0}</tr>", value));
            mTD.Clear();
        }
        get
        {
            mTD.Clear();
            return mTR.ToString();
        }
    }
    /// <summary>
    /// 设置 Title
    /// </summary>
    public string Title { set; get; }
    /// <summary>
    /// 设置 文本内容
    /// </summary>
    public string Context { set; get; }
    /// <summary>
    /// 设置 TH
    /// </summary>
    public string TH
    {
        set
        {
            mTH.Append(string.Format("<th>{0}</th>", value));
        }
        get
        {
            return mTH.ToString();
        }
    }
    /// <summary>
    /// 导出 Table
    /// </summary>
    public string TABLE
    {
        set
        {
            mTable.Append(string.Format("<p>{0}</p><p>{1}</p><table id=\"table-1\"><thead>{2}</thead><tbody>{3}</tbody></table>", Title, Context, TH, value));
            mTR.Clear();
            mTH.Clear();
        }
        get
        {
            return mTable.ToString();
        }
    }
    /// <summary>
    /// 导出最终 Html
    /// </summary>
    public string Html
    {
        get
        {
            string table = "<!DOCTYPE html><html><head><meta charset=\"UTF - 8\"><style>#table-1 thead,#table-1 tr {border-top-width: 1px;border-top-style: solid;border-top-color: #98c5e7;}#table-1 {border-bottom-width: 1px;border-bottom-style: solid;border-bottom-color: #98c5e7;}#table-1 th {border-top-width: 1px;border-top-style: solid;border-bottom-width: 1px;border-bottom-style: solid;border-color: #5c729e;padding: 5px 10px;font-size: 12px;font-family: Verdana;}#table-1 td {padding: 5px 10px;font-size: 12px;font-family: Verdana;color: #5c729e;}#table-1 tr:nth-child(even) {background: #d0dae8}#table-1 tr:nth-child(odd) {background: #FFF}</style></head><body>" + TABLE + "</body></html>";
            mTable.Clear();
            return table;
        }
    }
}
/*****************************************************************************************************************************************/
/********************************************************** Anker 电池信息 2024/04/09 ****************************************************/
/*****************************************************************************************************************************************/
public class BatteryAnker
{
    public string BARCODE { set; get; }
    public string OCV1 { set; get; }
    public string OCV1_TESTTIME { set; get; }
    public string IR1 { set; get; }
    public string OCV2 { set; get; }
    public string OCV2_TESTTIME { set; get; }
    public string IR2 { set; get; }
    public string KVALUE { set; get; }
    public string CAPACITY { set; get; }
    public string VOLTAGE { set; get; }
    public string RESISTANCE { set; get; }
    public string GRADE { set; get; }
    public string TEST_RESULT { set; get; }
    public string TEST_TIME { set; get; }
    public string LOT { set; get; }
    public string MATERIALS_PO { set; get; }
    public string MATERIALS_NO { set; get; }
    public string STATION_ID { set; get; }
    public string JYL { set; get; }
    public string VOLTAGE_OCV1 { set; get; }
    public string RESISTANCE_OCV1 { set; get; }
    public string TESTTIME_OCV1 { set; get; }
    public string VOLTAGE_OCV2 { set; get; }
    public string RESISTANCE_OCV2 { set; get; }
    public string TESTTIME_OCV2 { set; get; }
    public string VOLTAGE_SHELL { set; get; }
    //public string THICKNESS { set; get; }
    //public string WIDTH { set; get; }
}
public class SysItemAnker
{
    public string name { set; get; }
    public string val { set; get; }
    public int result { set; get; }
}
public class ItemsAnker
{
    public string bar_code { set; get; }
    public string type
    {
        get
        {
            return "电芯";
        }
    }
    public int result
    {
        get
        {
            return 1;
        }
    }
    public string materials_po { set; get; }

    public string test_time { set; get; }
    public List<SysItemAnker> data = new List<SysItemAnker>();
}
public class SysDataAnker
{
    public string request_id
    {
        get
        {
            return Guid.NewGuid().ToString();
        }
    }
    public string station_id { set; get; }
    public string material_no { set; get; }

    public List<ItemsAnker> items = new List<ItemsAnker>();
}
public class ResultAnker
{
    public int res_code { set; get; }
    public string message { get; set; }
}
public class ResultTransfer
{
    public string RESULT { set; get; }
    public string MSG { set; get; }
    public string CREATED_DATE_TIME
    {
        get
        {
            return DateTime.Now.ToString();
        }
    }
}
public class TokenDataAnker
{
    public string token { set; get; }
}
public class TokenAnker
{
    public int res_code { set; get; }
    public string message { set; get; }
    public bool need_login { set; get; }
    public TokenDataAnker data { set; get; }
}
public class LoginAnker
{
    public string account { set; get; }
    public string password { set; get; }
    public string mac 
    {
        get
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces)
            {
                // 获取物理地址（即MAC地址）
                PhysicalAddress physicalAddress = networkInterface.GetPhysicalAddress();
                string macAddress = physicalAddress.ToString();
                return macAddress;
            }
            return null;
        }
    }
    public string ip
    {
        get
        {
            string localIpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            return localIpAddress;
        }
    }
}
/*****************************************************************************************************************************************/
/************************************************* 飞毛腿动力 （智能家具）电池信息 2024/04/15 ********************************************/
/*****************************************************************************************************************************************/
public class SmartFurnitureSCUD
{
    public string sn { set; get; }
    public string ocvb_voltage { set; get; }
    public string ocvb_test_time { set; get; }
    public string ocvb_inter_resi { set; get; }
    public string capacity { set; get; }
    public string grade { set; get; }
    public string k_value { set; get; }
    public string thickness { set; get; }
    public string supplier_no { set; get; }
    public string ocv1 { set; get; }
    public string ocv1_test_time { set; get; }
    public string ir1 { set; get; }
    public string ocv2 { set; get; }
    public string ocv2_test_time { set; get; }
    public string ir2 { set; get; }
    public string test_result { set; get; }
    public string lot { set; get; }
}
public class ResultSmartFurnitureSCUD
{
    public string M_RESULT_FLAG { set; get; }
    public string M_RESULT_MSG { set; get; }
    public string HANDLE_RECIEVED { set; get; }
}
/******************************************************************************************************************************************/
/********************************************************** 海陆通 电池信息 2024/08/01 ****************************************************/
/******************************************************************************************************************************************/
public class BoxHiLT
{
    /// <summary>
    /// 名称
    /// </summary>
    public string CORE_TYPE_CODE { set; get; }
    /// <summary>
    /// 物料编码
    /// </summary>
    public string CUSTOMER_ITEM { set; get; }
    /// <summary>
    /// 数量
    /// </summary>
    public string QTY { set; get; }
    /// <summary>
    /// 日期
    /// </summary>
    public string CREATED_DATE_TIME { set; get; }
}

