using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using LSMES_5ANEW_PLUS.App_Base;
using System.Data.SqlClient;



namespace LSMES_5ANEW_PLUS.Business
{
    /// <summary>
    /// 材料库管理
    /// </summary>
    public class StorehouseManagement
    {
        /// <summary>
        /// 材料入库
        /// </summary>
        /// <param name="STOREHOUSEID">仓库ID</param>
        /// <param name="STOREHOUSENAME">仓库名称</param>
        /// <param name="MAFLAG">材料类型</param>
        /// <param name="MATYPE">材料名称（极片入库时，称为极片极性）</param>
        /// <param name="MABOMNO">电芯BOM</param>
        /// <param name="MATECHARGENM">供应商批号（极片入库时，称为极片批号）</param>
        /// <param name="SUPPLIERNM">材料批号（极片入库时，称为极片编号）</param>
        /// <param name="INPUTCOUNT">入库数量</param>
        /// <param name="INPUT_USERID">入库人ID</param>
        /// <param name="INPUT_NAME">入库人姓名</param>
        /// <param name="INPUT_TIME">入库时间</param>
        /// <param name="GLAIRNO">浆料批号</param>
        /// <param name="HOTDATE">烘干日期</param>
        /// <param name="EQUIPMENTNO">冲壳设备号</param>
        /// <param name="TOOLNO">模具号</param>
        /// <param name="TOOLNAME">模具名称</param>
        /// <param name="UNIT">单位</param>
        /// <param name="REMARKS">备注</param>
        /// <param name="STATE">状态</param>
        /// <param name="CREATER">创建人</param>
        /// <param name="CREATTIME">创建时间</param>
        /// <param name="UPDATER">更新人</param>
        /// <param name="UPDATETIME">更新时间</param>
        public static string intoMAStore(string STOREHOUSEID, string STOREHOUSENAME, string MAFLAG, string MATYPE, string MABOMNO, string MATECHARGENM, string SUPPLIERNM, string INPUTCOUNT, string INPUT_USERID, string INPUT_NAME, string INPUT_TIME, string GLAIRNO, string HOTDATE, string EQUIPMENTNO, string TOOLNO, string TOOLNAME, string UNIT, string REMARKS, string STATE, string CREATER, string CREATTIME, string UPDATER, string UPDATETIME)
        {
            try
            {
                //查询极片批次是否存在
                SQLQueryBuilder mSelect = new SQLQueryBuilder(QueryType.SELECT);
                mSelect.Bomno = MABOMNO;
                mSelect.DataTable = "T_MA_INTOSTORE";
                mSelect.Assemble("COUNT(MATECHARGENM)", "NUM", null);
                mSelect.Condition("MAFLAG", MAFLAG, true);
                mSelect.Condition("MATYPE", MATYPE, true);
                mSelect.Condition("MABOMNO", MABOMNO, true);
                mSelect.Condition("MATECHARGENM", MATECHARGENM, true);
                mSelect.Condition("SUPPLIERNM", SUPPLIERNM, true);
                mSelect.Condition("GLAIRNO", GLAIRNO, true);
                //插入极片批次信息
                SQLQueryBuilder mInsert = new SQLQueryBuilder(QueryType.INSERT);
                mInsert.Bomno = MABOMNO;
                mInsert.DataTable = "T_MA_INTOSTORE";
                mInsert.Assemble("STOREHOUSEID", null, "3");
                mInsert.Assemble("STOREHOUSENAME", null, STOREHOUSENAME);
                mInsert.Assemble("MAFLAG", null, MAFLAG);
                mInsert.Assemble("MATYPE", null, MATYPE);
                mInsert.Assemble("MABOMNO", null, MABOMNO);
                mInsert.Assemble("MATECHARGENM", null, MATECHARGENM);
                mInsert.Assemble("SUPPLIERNM",null,SUPPLIERNM);
                mInsert.Assemble("INPUTCOUNT", null, INPUTCOUNT);
                mInsert.Assemble("INPUT_USERID", null, INPUT_USERID);
                mInsert.Assemble("INPUT_NAME", null, INPUT_NAME);
                mInsert.Assemble("INPUT_TIME", null, INPUT_TIME);
                mInsert.Assemble("GLAIRNO", null, GLAIRNO);
                mInsert.Assemble("HOTDATE", null, HOTDATE);
                mInsert.Assemble("EQUIPMENTNO", null, EQUIPMENTNO);
                mInsert.Assemble("TOOLNO", null, TOOLNO);
                mInsert.Assemble("TOOLNAME", null, TOOLNAME);
                mInsert.Assemble("UNIT", null, UNIT);
                mInsert.Assemble("REMARKS", null, REMARKS);
                mInsert.Assemble("STATE", null, STATE);
                mInsert.Assemble("CREATER", null, CREATER);
                mInsert.Assemble("CREATTIME", null, CREATTIME);
                mInsert.Assemble("UPDATER", null, UPDATER);
                mInsert.Assemble("UPDATETIME", null, UPDATETIME);
                //run SQL
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW))
                {
                    string str = mSelect.Build();
                    conn.Open();
                    SqlCommand comm = new SqlCommand(str, conn);
                    if (Convert.ToInt32(comm.ExecuteScalar()) == 0)
                    {
                        comm.CommandText = mInsert.Build();
                        comm.ExecuteNonQuery();
                        conn.Close();
                        return "成功";
                    }
                    else
                    {
                        throw new Exception("极片批次重复");
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return ex.Message;
            }
        }
    }
}