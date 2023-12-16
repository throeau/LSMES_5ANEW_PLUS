using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Web;

namespace Listener
{
    /// <summary>
    /// author:zhaozichao
    /// date:2020/02/11
    /// </summary>
    class WebService
    {
        /// <summary>
        /// 使用post方法向web service发送数据
        /// </summary>
        /// <param name="url">web service地址</param>
        /// <param name="op">web service的方法</param>
        /// <param name="pars">发送的内容集</param>
        /// <returns>web service返回内容（XML）</returns>
        public XmlDocument QuerySoapWebService(string url,string op, Hashtable pars)
        {
            //定义调用Web Service的HttpWebRequest对象
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url + "/" + op);
            try
            {
                //设置Content-Type
                request.ContentType = "application/x-www-form-urlencoded";
                //设置发送方式POST/GET
                request.Method = "POST";
                //设置身份授权为默认
                request.Credentials = CredentialCache.DefaultCredentials;
                //设置超时为3000ms
                request.Timeout = 3000;
                //将待发送内容转换为byte[]
                byte[] data = EncodePars(pars);
                //发送内容至web service
                WriteRequestData(request,data);
                //返回web service XML对象
                return ReadXmlResponse(request.GetResponse());
            }
            catch (WebException ex)
            {
                /*----------------------------------获取服务器端返回的异常信息--------------------------------------*/
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream ErrorResponseStream = res.GetResponseStream();
                StreamReader ErrorStreamReader = new StreamReader(ErrorResponseStream, Encoding.UTF8);
                string remoteMessage = ErrorStreamReader.ReadToEnd();
                ErrorStreamReader.Close();
                SysLog log = new SysLog(ex.Message+" $ 服务器端返回异常信息："+remoteMessage);
                return null;
            }
        }
        /// <summary>
        /// 对web service参数及发送内容进行UTF8编码
        /// </summary>
        /// <param name="Pars">参数项及待发送内容</param>
        /// <returns>返回byte[]</returns>
        private static byte[] EncodePars(Hashtable Pars)
        {
            return Encoding.UTF8.GetBytes(ParsToString(Pars));
        }
        /// <summary>
        /// 将参数项与内容编成string对象
        /// </summary>
        /// <param name="Pars">参数项目及对应内容</param>
        /// <returns>返回string</returns>
        private static string ParsToString(Hashtable Pars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string k in Pars.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.Append(HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(Pars[k].ToString()));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 向web service发送数据
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="data">byte[]化的参数串</param>
        private static void WriteRequestData(HttpWebRequest request, byte[] data)
        {
            request.ContentLength = data.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();
        }
        /// <summary>
        /// 接收与web serivce交互后所得结果
        /// </summary>
        /// <param name="response">HttpWebRequest创建的WebResponse对象</param>
        /// <returns>web service返回的XML对象</returns>
        private static XmlDocument ReadXmlResponse(WebResponse response)
        {
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            String retXml = sr.ReadToEnd();
            sr.Close();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(retXml);
            return doc;
        }
    }
}
