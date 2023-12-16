using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.Business;
using Newtonsoft.Json;


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
    }
}
