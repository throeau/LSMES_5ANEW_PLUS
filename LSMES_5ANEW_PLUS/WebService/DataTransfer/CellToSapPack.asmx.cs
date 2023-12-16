using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LSMES_5ANEW_PLUS.Business;
using LSMES_5ANEW_PLUS.App_Base;
using Newtonsoft.Json;


namespace LSMES_5ANEW_PLUS.WebService.DataTransfer
{
    /// <summary>
    /// CellToSapPack 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class CellToSapPack : System.Web.Services.WebService
    {

        //从力神聚合物老系统向SapPack系统导入电芯数据
        [WebMethod]
        public string SendCellDataOfLsmesToSapPack(string info)
        {
            SyncCellPack pack = new SyncCellPack();
            pack.Initialization(JsonConvert.DeserializeObject<EntitySynCellPack>(info));
            if (pack.GetCellsInfo().CELL_LIST.Count > 0)
            {
                string HANDLE = pack.CreateShipStatement();
                if (pack.CreateShipCellStatement(HANDLE))
                {
                    if (pack.CreateOcv1Statement())
                    {
                        if (pack.CreateOcv2Statement())
                        {
                            return "Successfully sync cell to pack.";
                        }
                        else
                        {
                            return "Failed to import IR-OCV 2 data.";
                        }
                    }
                    else
                    {
                        return "Failed to import IR-OCV 1 data.";
                    }
                }
                else
                {
                    return "Failed to import package information.";
                }
            }
            else
            {
                return "Abnormal number of batteries in the box.";
            }
        }
        /// <summary>
        /// 验证包装箱是否从CELL系统导入PACK中
        /// </summary>
        /// <param name="info">基本信息</param>
        /// <returns></returns>
        [WebMethod]
        public string IsException(string info)
        {
            try
            {
                EntitySynCellPack entity = JsonConvert.DeserializeObject<EntitySynCellPack>(info);
                SyncCellPack pack = new SyncCellPack();
                pack.Initialization(entity);
                AppException exp = pack.IsValidSAP();
                AppException exp2 = pack.IsValidLSMES();
                if (exp.IsException)
                {
                    EntityException entityExp = new EntityException();
                    entityExp.IsException = exp.IsException;
                    entityExp.ExpMessage = exp.obj.ToString();
                    return JsonConvert.SerializeObject(entityExp);
                }
                else if (exp2.IsException)
                {
                    EntityException entityExp = new EntityException();
                    entityExp.IsException = exp2.IsException;
                    entityExp.ExpMessage = exp2.obj.ToString();
                    return JsonConvert.SerializeObject(entityExp);
                }
                else
                {
                    EntityException entityExp = new EntityException();
                    entityExp.IsException = false;
                    entityExp.ExpMessage = null;
                    return JsonConvert.SerializeObject(entityExp);
                }
            }
            catch (Exception ex)
            {
                EntityException entityExp = new EntityException();
                entityExp.IsException = false;
                entityExp.ExpMessage = ex.Message;
                return JsonConvert.SerializeObject(entityExp);
            }
        }
    }
}
