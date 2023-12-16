using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Configuration;
using LSMES_5ANEW_PLUS.Business;
using Newtonsoft.Json;

namespace LSMES_5ANEW_PLUS.WebService.Update
{
    /// <summary>
    /// Update 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Update : System.Web.Services.WebService
    {
        [WebMethod]
        public int UpdateCapacity(string bomno, string pipeline, string orderno)
        {
            try
            {
                if (!string.IsNullOrEmpty(orderno) && !string.IsNullOrEmpty(bomno) && !string.IsNullOrEmpty(pipeline))
                {
                    Datum mdata = new Datum();
                    mdata.UpdateCapacity(bomno, pipeline, orderno);
                    return 0;
                }
                else
                    return 1;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 1;
            }
        }
        [WebMethod]
        public int UpdateKValue(string content, string lot)
        {
            try
            {
                if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(lot))
                {
                    Datum mdata = new Datum();
                    mdata.UpdateKValue(Serialize.DeserializeDataTable(content), lot);
                    return 0;
                }
                else
                    throw new Exception("content or lot is empty.");
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return 1;
            }
        }
        [WebMethod]
        public int UpdateE6DataFromWj1Sap(string jsonStr)
        {
            try
            {
                foreach (string key in ConfigurationManager.AppSettings.Keys)
                {
                    jsonStr = jsonStr.Replace(key, ConfigurationManager.AppSettings[key]);
                }
                UpdateRemote mBussinesss = new UpdateRemote();
                return mBussinesss.UpdateE6DataFromWj1Sap(jsonStr);
            }
            catch (Exception ex)
            {
                SysLog mLog = new SysLog(ex.Message);
                return 1;
            }
        }
        [WebMethod]
        public int UpdateE5DataFromWj1Sap(string jsonStr)
        {
            try
            {
                foreach (string key in ConfigurationManager.AppSettings.Keys)
                {
                    jsonStr = jsonStr.Replace(key, ConfigurationManager.AppSettings[key]);
                }
                UpdateRemote mBussinesss = new UpdateRemote();
                return mBussinesss.UpdateE5DataFromWj1Sap(jsonStr);
            }
            catch (Exception ex)
            {
                SysLog mLog = new SysLog(ex.Message);
                return 1;
            }
        }
        [WebMethod]
        public int UpdateE3DataFromWj1Sap(string jsonStr)
        {
            try
            {
                foreach (string key in ConfigurationManager.AppSettings.Keys)
                {
                    jsonStr = jsonStr.Replace(key, ConfigurationManager.AppSettings[key]);
                }
                UpdateRemote mBussinesss = new UpdateRemote();
                return mBussinesss.UpdateE3DataFromWj1Sap(jsonStr);
            }
            catch (Exception ex)
            {
                SysLog mLog = new SysLog(ex.Message);
                return 1;
            }
        }
        [WebMethod]
        public int UpdateE16DataFromWj1Sap(string jsonStr)
        {
            try
            {
                foreach (string key in ConfigurationManager.AppSettings.Keys)
                {
                    jsonStr = jsonStr.Replace(key, ConfigurationManager.AppSettings[key]);
                }
                UpdateRemote mBussinesss = new UpdateRemote();
                return mBussinesss.UpdateE16DataFromWj1Sap(jsonStr);
            }
            catch (Exception ex)
            {
                SysLog mLog = new SysLog(ex.Message);
                return 1;
            }
        }
        /// <summary>
        /// 五期甲一上传性能数据至二三楼老系统
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        [WebMethod]
        public int UpdateEDataFromWj1Sap(string jsonStr)
        {
            try
            {
                SysLog log = new SysLog(jsonStr);

                if (jsonStr.IndexOf("OP11") > -1)
                {
                    UpdateE6DataFromWj1Sap(jsonStr);
                    return 0;
                }
                else if (jsonStr.IndexOf("OP07") > -1)
                {
                    UpdateE3DataFromWj1Sap(jsonStr);
                    return 0;
                }
                else if (jsonStr.IndexOf("OP08") > -1)
                {
                    UpdateE16DataFromWj1Sap(jsonStr);
                    return 0;
                }
                else if (jsonStr.IndexOf("OP09") > -1)
                {
                    UpdateE5DataFromWj1Sap(jsonStr);
                    return 0;
                }

                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                SysLog mLog = new SysLog(ex.Message);
                return 1;
            }
        }
        [WebMethod]
        public int UploadOCV(string jsonStr)
        {
            try
            {
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    UpdateRemote mBussinesss = new UpdateRemote();
                    if (mBussinesss.RecievedOCV(jsonStr))
                    {
                        return mBussinesss.UpdatePerformance(mBussinesss.SavePerformance(jsonStr));
                    }
                    else
                    {
                        throw new Exception("WS:UploadOCV => RecievedOCV 失败.");
                    }
                }
                else
                {
                    throw new Exception("WS:UploadOCV => 上传数据JSON为空.");
                }
            }
            catch (Exception ex)
            {
                SysLog mLog = new SysLog(ex.Message);
                return 1;
            }
        }
        [WebMethod]
        public int UpdateDSBoxID(string bomno,string boxid_lsn,string boxid_ds)
        {
            try
            {
                return Datum.UpdateDSBoxID(bomno, boxid_lsn, boxid_ds);
            }
            catch(Exception ex)
            {
                SysLog mLog = new SysLog(ex.Message);
                return 0;
            }
        }
        [WebMethod]
        public int UpdateDSCaseID(string bomno,string caseid_lsn,string caseid_ds)
        {
            try
            {
                return Datum.UpdateDSCaseID(bomno, caseid_lsn, caseid_ds);
            }
            catch(Exception ex)
            {
                SysLog mLog = new SysLog(ex.Message);
                return 0;
            }
        }
        [WebMethod]
        public void UpdateHipot(string info)
        {
            ResultHiPot result = new ResultHiPot();
            try
            {
                HiPot hipot = new HiPot();
                hipot = JsonConvert.DeserializeObject<HiPot>(info);
                UpdateRemote remote = new UpdateRemote();
                if (remote.UpdateHipot("SP27A0B8SF", hipot.barcode, hipot.hipot) == 1)
                {
                    result.Result = "success";
                }
                else
                {
                    result.Result = "fail";
                }
            }
            catch(Exception ex)
            {
                SysLog mLog = new SysLog(ex.Message);
                result.Result = "fail";
            }
            string resultStr = JsonConvert.SerializeObject(result);
            Context.Response.Charset = "UTF-8";
            Context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Context.Response.Write(result);
            Context.Response.End();
        }
    }
}
