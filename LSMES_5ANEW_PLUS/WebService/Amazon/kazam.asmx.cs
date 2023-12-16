using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.Business;
using LSMES_5ANEW_PLUS.App_Base;
using Newtonsoft.Json;
using System.Data;
using System.Collections;

namespace LSMES_5ANEW_PLUS.WebService.Amazon
{
    /// <summary>
    /// kazam 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class kazam : System.Web.Services.WebService
    {
        /// <summary>
        /// 同步 Amazon PACK 数据，SAP Server to LSN Server 
        /// </summary>
        /// <param name="boxid">箱号</param>
        [WebMethod]
        public void DataSyncAmazonKazam(string boxid)
        {
            ResultAmazon result = new ResultAmazon();
            result = SAP_Information.DataSyncAmazonKazam(SAP_Information.PerformanceAmazonByBox(boxid),"create");
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        /// <summary>
        /// 返回 Amazon Kazam 客户端数据请求,指定参数
        /// </summary>
        /// <param name="barcode">PACK 码号</param>
        /// <param name="para">所需返回的参数名</param>
        [WebMethod]
        public void SyncPackData(string barcode,string para)
        {
            EntityAmazonBatteryParameter result = SAP_Information.SyncPackData(barcode, para);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        /// <summary>
        /// 返回 Amazon Kazam 客户端数据请求，全部参数
        /// </summary>
        /// <param name="barcode">PACK 码号</param>
        [WebMethod]
        public void SyncPackDataAll(string barcode)
        {
            List<EntityAmazonBatteryParameter> result = SAP_Information.SyncPackData(barcode);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        /// <summary>
        /// 将数据移至备份表
        /// </summary>
        /// <param name="bracode">PACK 码号</param>
        [WebMethod]
        public void BackupPackData(string barcode)
        {
            ResultAmazon result = SAP_Information.BackupPackData(barcode);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        /// <summary>
        /// 从 LSN Server 获取已经就绪的电池码号（已完成 SAP 至 LSN 同步的整箱内电池）
        /// </summary>
        /// <param name="barcode">箱号</param>
        [WebMethod]
        public void BatteryList(string barcode)
        {
            List<string> result = SAP_Information.BatteryNoByBox(barcode);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        /// <summary>
        /// 获取指定箱号/栈板号全部数据
        /// </summary>
        /// <param name="barcode">箱号/栈板号</param>
        [WebMethod]
        public void AllParametersByBarcode(string barcode)
        {            
            string result = JsonConvert.SerializeObject(SAP_Information.AllParametersByBarcode(barcode));
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
        [WebMethod]
        public void UpdateAmazonKazamPack(string info)
        {
            DataTable mDt = new DataTable();
            mDt = JsonConvert.DeserializeObject<DataTable>(info);
            ResultAmazon result = SAP_Information.DataSyncAmazonKazam(mDt,"update");
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        /// <summary>
        /// 同步 Amazon PACK 数据，SAP Server to LSN Server 
        /// </summary>
        /// <param name="boxid"></param>
        /// <param name="pack_plt_no"></param>
        /// <param name="pack_lot_no"></param>
        /// <param name="pack_shp_no"></param>
        /// <param name="pack_shp_date"></param>
        [WebMethod]
        public void DataSyncAmazonKazam2(string boxid, string pack_plt_no, string pack_lot_no, string pack_shp_no, string pack_shp_date)
        {
            ResultAmazon result = new ResultAmazon();
            result = SAP_Information.DataSyncAmazonKazam(SAP_Information.PerformanceAmazonByBox(boxid, pack_plt_no, pack_lot_no, pack_shp_no, pack_shp_date),"create");
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        /// <summary>
        /// 获取 Amazon 主数据
        /// </summary>
        /// <param name="item_no">LSN SAP ME 产品物料编号</param>
        [WebMethod]
        public void PrimaryAmazon(string item_no)
        {
            DataTable mDt = new DataTable();
            mDt = Primary.PrimaryDataAmazon(item_no);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(mDt));
            Context.Response.End();
        }
        [WebMethod]
        public void DataSyncAmazonPerformanceCell(string id)
        {
            int affectRows = Datum.DataSyncAmazonKazamPerformanceCell(id);
            ResultAmazon result = new ResultAmazon();
            //if (affectRows > 0)
            //{
                result.RESULT = "success";
                result.MSG = string.Format("{0} records succeeded", affectRows);
            //}
            //else
            //{
            //    result.RESULT = "fail";
            //}
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        [WebMethod]
        public void GetAmazonPerformanceCell(string id)
        {
            List<EntityAmazonPerformanceCell> Cells = new List<EntityAmazonPerformanceCell>();
            Cells = Datum.GetAmazonKazamPerformanceCell(id);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(Cells));
            Context.Response.End();
        }
        [WebMethod]
        public void UpdateAmazonKazamCell(string info)
        {
            List<EntityAmazonPerformanceCell> Cells = new List<EntityAmazonPerformanceCell>();
            Cells = JsonConvert.DeserializeObject<List<EntityAmazonPerformanceCell>>(info);
            int affect_Rows = Datum.UpdateAmazonKazamPerformanceCell(Cells);
            ResultAmazon result = new ResultAmazon();
            if (affect_Rows == Cells.Count)
            {
                result.RESULT = "success";
                result.MSG = string.Format("{0} records succeeded", affect_Rows);
            }
            else
            {
                result.RESULT = "fail";
            }
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        [WebMethod]
        public void Statistics(string type)
        {
            List<AmazonKazamStatistics> result = new List<AmazonKazamStatistics>();
            result = Datum.Statistics(type);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        [WebMethod]
        public void Ready(string info,string type)
        {
            List<AmazonBarcode> barcodeList = new List<AmazonBarcode>();
            barcodeList = JsonConvert.DeserializeObject<List<AmazonBarcode>>(info);
            int affect_rows = Datum.Ready(barcodeList, type);
            ResultAmazon result = new ResultAmazon();
            if (affect_rows == barcodeList.Count)
            {
                result.RESULT = "success";
                result.MSG = string.Format("{0} rows are affected", affect_rows);
            }
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        [WebMethod]
        public void SyncCellDataAll(string barcode)
        {
            List<EntityAmazonBatteryParameter> result = SAP_Information.SyncCellData(barcode);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        /// <summary>
        /// 将数据移至备份表
        /// </summary>
        /// <param name="bracode">CELL 码号</param>
        [WebMethod]
        public void BackupCellData(string barcode)
        {
            ResultAmazon result = SAP_Information.BackupCellData(barcode);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        /// <summary>
        /// 将已回传数据还原回，未回传状态
        /// </summary>
        /// <param name="barcode">箱号或栈板号</param>
        [WebMethod]
        public void RollbackAmazon(string barcode,string type)
        {
            int affect_rows = Datum.RollBackBackupAmazon(barcode, type);
            ResultAmazon result = new ResultAmazon();
            if (affect_rows != -1)
            {
                result.RESULT = "success";
                result.MSG = string.Format("{0} rows are affected", affect_rows);
            }
            else
            {
                result.RESULT = "fail";
            }
        }
        /// <summary>
        /// 获取 Amazon 项目名称
        /// </summary>
        [WebMethod]
        public void AmazonProject()
        {
            Hashtable result = SAP_Information.AmazonProject();
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
        /// <summary>
        /// 获取项目参数
        /// </summary>
        /// <param name="item_no">项目</param>
        [WebMethod]
        public void AmazonParameters(string item_no)
        {
            List<AmazonParameter> result = SAP_Information.AmazonParameters(item_no);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(JsonConvert.SerializeObject(result));
            Context.Response.End();
        }
    }
}
