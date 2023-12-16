using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

namespace LSMES_5ANEW_PLUS.Business
{
    public class ToolsInfo
    {
        /// <summary>
        /// 返回工装治具使用寿命数据
        /// </summary>
        /// <param name="equipmentno">设备编号</param>
        /// <param name="toolno">治具编号</param>
        /// <param name="postion">安装位置号</param>
        /// <returns>成功：返回结果DataTable；失败：返回 null</returns>
        public XmlReader LoadToolsInfo(string equipmentno, string toolno, string postion)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT TOOLNO,M.LINE EQUIPMENTNO,TOOLNAME,TOOLMODEL,T.REMARKS REMARKS,SETDATE FROM T_TOOL T LEFT JOIN M_EQUIPMENT M ON T.EQUIPMENTNO = M.EQUIPMENTNO WHERE TOOLNO = '{0}' FOR XML RAW";
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format(sql, toolno);
                    XmlReader reader = command.ExecuteXmlReader();
                    //SqlDataReader mReader = command.ExecuteReader();
                    //DataTable dt = new DataTable("ToolsInfo");
                    //dt.Load(mReader);
                    //mReader.Close();
                    //conn.Close();
                    //return dt;
                    return reader;
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 更新工装治具使用次数
        /// </summary>
        /// <param name="equipmentno">设备编号</param>
        /// <param name="toolno">治具编号</param>
        /// <param name="sl">剩余次数</param>
        /// <returns>成功：1；失败：0</returns>
        public int updateServiceLife(string equipmentno, string toolno, string sl)
        {
            using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringByLSMES_5ANEW))
            {
                try
                {
                    conn.Open();
                    string sql = "UPDATE T_TOOL SET REMARKS = '{1}' WHERE TOOLNO = '{0}';";
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = string.Format(sql, toolno, sl);
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    SysLog log = new SysLog(ex.Message);
                    return 0;
                }
            }
        }
    }
}