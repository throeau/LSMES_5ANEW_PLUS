using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.Business;
using Newtonsoft.Json;
using LSMES_5ANEW_PLUS.App_Base;
using System.Collections;


namespace LSMES_5ANEW_PLUS.WebService.LoadData
{
    /// <summary>
    /// LoadCapacity 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class BatteryData : System.Web.Services.WebService
    {

        [WebMethod]
        public string LoadCapacity(string bomno,string pipeline,string orderno)
        {
            try
            {
                if (!string.IsNullOrEmpty(bomno) && !string.IsNullOrEmpty(pipeline) && !string.IsNullOrEmpty(orderno))
                {
                    Datum mdata = new Datum();
                    return Serialize.SerializeDataTableXml(mdata.LoadCapacity(bomno, pipeline, orderno));
                }
                else
                {
                    throw new Exception("BOMNO or PIPELINE or ORDERNO is empty");
                }
            }
            catch (Exception ex){
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        [WebMethod]
        public string LoadDeltaV(string lot)
        {
            try
            {
                if (!string.IsNullOrEmpty(lot))
                {
                    Datum mdata = new Datum();
                    return Serialize.SerializeDataTableXml(mdata.LoadDeltaV(lot));
                }
                else
                {
                    throw new Exception("lot is empty.");
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        [WebMethod]
        public string LoadVoltage(string sql)
        {
            try
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    Datum mdata = new Datum();
                    return Serialize.SerializeDataTableXml(mdata.LoadVoltage(sql));
                }
                else
                {
                    throw new Exception("parameters are empty.");
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        [WebMethod]
        public string LoadCustomer(string sql)
        {
            try
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    Datum mdata = new Datum();
                    return Serialize.SerializeDataTableXml(mdata.LoadCustomer(sql));
                }
                else
                {
                    throw new Exception("parameters are empty.");
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        [WebMethod]
        public string LoadPackBomno()
        {
            try
            {
                Datum mdata = new Datum();
                return Serialize.SerializeDataTableXml(mdata.LoadPackBomno());
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        [WebMethod]
        public string LoadPackBarcode(string bomno, string boxid)
        {
            try
            {
                Datum mdata = new Datum();
                return Serialize.SerializeDataTableXml(mdata.LoadPackBarcode(bomno,boxid));
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        [WebMethod]
        public string LoadBC(string bomno,string boxid)
        {
            try
            {
                Datum mdata = new Datum();
                return Serialize.SerializeDataTableXml(mdata.LoadBC(bomno, boxid));
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        [WebMethod]
        public void LoadCellList(string batch)
        {
            try
            {
                Datum mdata = new Datum();
                Context.Response.Charset= "UTF-8";
                Context.Response.Write(JsonConvert.SerializeObject(Datum.GetBatterynoByBatch(batch)));
                Context.Response.End();
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        [WebMethod]
        public void AvailabilityBarcode(string info)
        {
            ResultAvailability result = new ResultAvailability();
            try
            {
                BarcodeAvailable entity = JsonConvert.DeserializeObject<BarcodeAvailable>(info);

                result.Result = "OK";
                info = entity.Barcode;
                entity.Barcode = Datum.Analysis(info);
                if (string.IsNullOrEmpty(entity.Barcode))
                {
                    result.Result = "NG";
                    result.Msg = string.Format("喷码系统所发信息不符合规则，请确认", entity.Barcode);
                }
                if (Datum.QtyBatteryNoByAvailabilityLog(entity.Barcode) > 0)
                {
                    result.Result = "NG";
                    result.Msg = string.Format("喷码系统已存在{0}校验记录，请检查实物是否已喷码号{0}", entity.Barcode);
                }
                else if (Datum.QtyBatteryNoByLSMES(entity.Barcode) > 0)
                {
                    result.Result = "NG";
                    result.Msg = "追溯系统中存在该码号！";
                }
                else if (Datum.QtyBatteryNoBySapPACK(entity.Barcode) > 0)
                {
                    result.Result = "NG";
                    result.Msg = "五期甲二系统中存在该码号！";
                }
                else if (Datum.QtyBatteryNoBySapME(entity.Barcode) > 0)
                {
                    result.Result = "NG";
                    result.Msg = "五期甲一系统中存在该码号！";
                }
                Datum.AvailabilityLog(entity.Barcode, result.Msg, entity.Created_user, info);

            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                result.Result = "NG";
                result.Msg = "接口所接收参数错误，请检查数据格式";
            }
            info = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(info);
            Context.Response.End();
        }
        [WebMethod]
        public void ProductionModel(string resrce_no)
        {
            if (string.IsNullOrEmpty(resrce_no)) return;
            try
            {
                ProductionModel result = Datum.ProductionModel(resrce_no);
                string info = JsonConvert.SerializeObject(result);
                Context.Response.Charset = "UTF-8";
                Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Context.Response.Write(info);
                Context.Response.End();
            }
            catch(Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
        [WebMethod]
        public void LoadCalculateColumn(string line,string op)
        {
            if (string.IsNullOrEmpty(op)) return;
            try
            {
                Hashtable result = SAP_Information.CalculateColumn(line, op);
                string info = JsonConvert.SerializeObject(result);
                Context.Response.Charset = "UTF-8";
                Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Context.Response.Write(info);
                Context.Response.End();
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
            }
        }
    }
}
