using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LSMES_5ANEW_PLUS.Business
{
    public class Serialize
    {
        /// <summary>
        /// 将DataTable序列化（标准）
        /// </summary>
        /// <param name="pDt">待序列化的DataTable</param>
        /// <returns>序列化的XML</returns>
        public static string SerializeDataTableXml(DataTable pDt)
        {
            try
            {
                if (pDt.Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    XmlWriter writer = XmlWriter.Create(sb);
                    XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
                    serializer.Serialize(writer, pDt);
                    writer.Close();
                    return sb.ToString();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 反序列化DataTable
        /// </summary>
        /// <param name="pXml">待序列化的string</param>
        /// <returns>DataTable</returns>
        public static DataTable DeserializeDataTable(string pXml)
        {
            StringReader strReader = new StringReader(pXml);
            XmlReader xmlReader = XmlReader.Create(strReader);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            DataTable dt = serializer.Deserialize(xmlReader) as DataTable;
            return dt;
        }

    }
}