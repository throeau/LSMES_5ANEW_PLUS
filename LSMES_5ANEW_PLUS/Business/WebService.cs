using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Web;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace LSMES_5ANEW_PLUS.App_Base
{
    class WebSOAP
    {
        /// <summary>
        /// 使用post方法向web service发送数据
        /// </summary>
        /// <param name = "url" > web service地址</param>
        /// <param name = "op" > web service的方法</param>
        /// <param name = "pars" > 发送的内容集 </ param >
        /// < returns > web service返回内容（XML）</returns>
        public XmlDocument QuerySoapWebService(string url, string op, string pars)
        {
            //定义调用Web Service的HttpWebRequest对象
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                //设置Content-Type
                request.ContentType = "application/x-www-form-urlencoded";
                //设置发送方式POST/GET
                request.Method = "POST";
                //设置身份授权为默认
                request.Credentials = CredentialCache.DefaultCredentials;
                //设置超时为3000ms
                request.Timeout = 6000;
                //将待发送内容转换为byte[]
                byte[] data = EncodePars(pars);
                //发送内容至web service
                WriteRequestData(request, data);
                //返回web service XML对象
                return ReadXmlResponse(request.GetResponse());
            }
            catch (WebException ex)
            {
                /*----------------------------------获取服务器端返回的异常信息--------------------------------------*/
                using (HttpWebResponse res = (HttpWebResponse)ex.Response)
                {
                    Stream ErrorResponseStream = res.GetResponseStream();
                    StreamReader ErrorStreamReader = new StreamReader(ErrorResponseStream, Encoding.UTF8);
                    string remoteMessage = ErrorStreamReader.ReadToEnd();
                    ErrorStreamReader.Close();
                    res.Close();
                    SysLog log = new SysLog(ex.Message + " $ 服务器端返回异常信息：" + remoteMessage);
                    return null;
                }
            }
        }
        public SMP_ResponseResult SMP_QuerySoapWebService(string url, string pars)
        {
            //定义调用Web Service的HttpWebRequest对象
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            try
            {
                //设置Content-Type
                request.ContentType = "application/json"; //直接发送SMP方式
                //request.ContentType = "application/x-www-form-urlencoded";//直接发送LSN方式
                //设置发送方式POST/GET
                request.Method = "POST";
                //设置身份授权为默认
                request.Credentials = CredentialCache.DefaultCredentials;
                //设置超时为300000ms
                request.Timeout = 300000;
                //将待发送内容转换为byte[]
                byte[] data = EncodePars(pars);
                //发送内容至web service
                WriteRequestData(request, data);
                //直接发送SMP后，接收SMP返回值
                return SMP_ReadJsonResponse(request.GetResponse());
            }
            catch (WebException ex)
            {
                /*----------------------------------获取服务器端返回的异常信息--------------------------------------*/
                using (HttpWebResponse res = (HttpWebResponse)ex.Response)
                {
                    Stream ErrorResponseStream = res.GetResponseStream();
                    StreamReader ErrorStreamReader = new StreamReader(ErrorResponseStream, Encoding.UTF8);
                    string remoteMessage = ErrorStreamReader.ReadToEnd();
                    ErrorStreamReader.Close();
                    res.Close();
                    SysLog log = new SysLog(ex.Message + " $ 服务器端返回异常信息：" + remoteMessage);
                    return null;
                }
            }
        }
        public string Sunwoda_QuerySoapWebService(string url, string pars,int threshold = 0)
        {
            //定义调用Web Service的HttpWebRequest对象
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                //设置Content-Type
                request.ContentType = "application/x-www-form-urlencoded";
                //设置发送方式POST/GET
                request.Method = "POST";
                //设置身份授权为默认
                request.Credentials = CredentialCache.DefaultCredentials;
                //设置超时为300000ms
                request.Timeout = 300000;
                //将待发送内容转换为byte[]
                byte[] data = EncodePars("jsonData=" + pars);
                //发送内容至web service
                WriteRequestData(request, data);
                //返回web service XML对象
                return Sunwoda_ReadJsonResponse(request.GetResponse());
            }
            catch (WebException ex)
            {
                /*----------------------------------获取服务器端返回的异常信息--------------------------------------*/
                try
                {
                    if (ex.Message == "操作超时" && threshold < 5 )
                    {
                        SysLog _log = new SysLog(string.Format("Sunwoda_QuerySoap {0}，重试第 {1} 次.", ex.Message, threshold));
                        return Sunwoda_QuerySoapWebService(url, pars, ++threshold);
                    }
                    using (HttpWebResponse res = (HttpWebResponse)ex.Response)
                    {
                        Stream ErrorResponseStream = res.GetResponseStream();
                        StreamReader ErrorStreamReader = new StreamReader(ErrorResponseStream, Encoding.UTF8);
                        string remoteMessage = ErrorStreamReader.ReadToEnd();
                        ErrorStreamReader.Close();
                        res.Close();
                        SysLog log = new SysLog(ex.Message + " $ 服务器端返回异常信息：" + remoteMessage);
                    }
                }
                catch (Exception _ex)
                {
                    SysLog log = new SysLog(_ex.Message);
                }
                return null;
            }
        }
        public SCUD_ResponseResult SCUD_QuerySoapWebService(string url, string pars)
        {
            //定义调用Web Service的HttpWebRequest对象
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                //设置Content-Type
                //request.ContentType = "application/json"; //直接发送SMP方式
                request.ContentType = "application/x-www-form-urlencoded";//直接发送LSN方式
                //设置发送方式POST/GET
                request.Method = "POST";
                //设置身份授权为默认
                request.Credentials = CredentialCache.DefaultCredentials;
                //设置超时为300000ms
                request.Timeout = 300000;
                //将待发送内容转换为byte[]
                byte[] data = EncodePars(pars);
                //发送内容至web service
                WriteRequestData(request, data);
                //返回web service XML对象(直接发送LSN方式)
                return SCUD_ReadResponse(request.GetResponse());
            }
            catch (WebException ex)
            {
                /*----------------------------------获取服务器端返回的异常信息--------------------------------------*/
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream ErrorResponseStream = res.GetResponseStream();
                StreamReader ErrorStreamReader = new StreamReader(ErrorResponseStream, Encoding.UTF8);
                string remoteMessage = ErrorStreamReader.ReadToEnd();
                ErrorStreamReader.Close();
                SysLog log = new SysLog(ex.Message + " $ 服务器端返回异常信息：" + remoteMessage);
                return null;
            }
        }
        /// <summary>
        /// Zebra 数据回传
        /// </summary>
        /// <param name="url">Zebra接收地址</param>
        /// <param name="pars">回传数据</param>
        /// <returns>Zebra 返回结果</returns>

        public Zebra_ResponseResult Zebra_QuerySoapWebService(string url, string pars)
        {
            //定义调用Web Service的HttpWebRequest对象
            SysLog log = new SysLog("2.1," + url);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            //request.Headers.Add("id", "1001");
            //request.Headers.Add("token", "54610983b6c895db0a2c603839fe983c");
            //log.AddLog("2.2，id:" + request.Headers.GetValues(0) + "，token：" + request.Headers.GetValues(1));
            try
            {
                //设置Content-Type
                request.ContentType = "application/json"; //直接发送Zebra方式
                //request.ContentType = "application/x-www-form-urlencoded";//直接发送LSN方式
                //设置发送方式POST/GET
                request.Method = "POST";
                //设置身份授权为默认
                request.Credentials = CredentialCache.DefaultCredentials;
                //设置超时为300000ms
                request.Timeout = 300000;
                //将待发送内容转换为byte[]
                byte[] data = EncodePars(pars);

                log.AddLog("2.3，WriteRequestData");
                //发送内容至web service
                WriteRequestData(request, data);

                log.AddLog("2.4，ReadJsonResponse");
                //直接发送SMP后，接收Zebra返回值
                return Zebra_ReadJsonResponse(request.GetResponse());
            }
            catch (WebException ex)
            {
                /*----------------------------------获取服务器端返回的异常信息--------------------------------------*/
                using (HttpWebResponse res = (HttpWebResponse)ex.Response)
                {
                    Stream ErrorResponseStream = res.GetResponseStream();
                    StreamReader ErrorStreamReader = new StreamReader(ErrorResponseStream, Encoding.UTF8);
                    string remoteMessage = ErrorStreamReader.ReadToEnd();
                    ErrorStreamReader.Close();
                    res.Close();
                    SysLog log2 = new SysLog(ex.Message + " $ 服务器端返回异常信息：" + remoteMessage);
                    return null;
                }
            }
        }

        /// <summary>
        /// 对web service参数及发送内容进行UTF8编码
        /// </summary>
        /// <param name="Pars">参数项及待发送内容</param>
        /// <returns>返回byte[]</returns>
        //private static byte[] EncodePars(Hashtable Pars)
        //{
        //    return Encoding.UTF8.GetBytes(ParsToString(Pars));
        //}
        private static byte[] EncodePars(string Pars)
        {
            return Encoding.UTF8.GetBytes(Pars);
        }
        /// <summary>
        /// 将参数项与内容编成string对象
        /// </summary>
        /// <param name="Pars">参数项目及对应内容</param>
        /// <returns>返回string</returns>
        public static string ParsToString(Hashtable Pars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string k in Pars.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }
                sb.Append("\"" + HttpUtility.UrlEncode(k) + "\":\"" + HttpUtility.UrlEncode(Pars[k].ToString()) + "\"");
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
            using (Stream writer = request.GetRequestStream())
            {
                writer.Write(data, 0, data.Length);
                writer.Close();
            }
        }
        /// <summary>
        /// 接收与web serivce交互后所得结果
        /// </summary>
        /// <param name="response">HttpWebRequest创建的WebResponse对象</param>
        /// <returns>web service返回的XML对象</returns>
        private static XmlDocument ReadXmlResponse(WebResponse response)
        {
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string retXml = sr.ReadToEnd();
                sr.Close();
                response.Close();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(retXml);
                return doc;
            }
        }
        /// <summary>
        /// 新普数据发送后接收返回结果
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static SMP_ResponseResult SMP_ReadJsonResponse(WebResponse response)
        {
            string retJson = null;
            SMP_ResponseResult result = new SMP_ResponseResult();
            try
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    retJson = sr.ReadToEnd();
                    string ret = SMP_ResponseLog(retJson);
                    sr.Close();
                    response.Close();
                    result = (SMP_ResponseResult)JsonConvert.DeserializeObject(retJson, typeof(SMP_ResponseResult));
                    //result.items = JsonConvert.DeserializeObject<List<SMP_ResponseResultDetails>>(result.data);
                    result.ret = ret;
                    return result;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("SMP_ReadJsonResponse：" + ex.Message);
                return null;
            }
        }
        private static string SMP_ResponseLog(string response)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT INTO SMP_RECIEVED_INFO (RET_INFO) OUTPUT INSERTED.HANDLE VALUES ('{0}')", response);
                    return cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 欣旺达数据发送后接收返回结果
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string Sunwoda_ReadJsonResponse(WebResponse response)
        {
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string retJson = sr.ReadToEnd();
                sr.Close();
                response.Close();
                return retJson;
            }
        }
        /// <summary>
        /// Zebra 数据发送后返回结果
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string Zebra_ResponseLog(string response)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT INTO ZEBRA_RECIEVED_INFO (RET_INFO) OUTPUT INSERTED.HANDLE VALUES ('{0}')", response);
                    return cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Zebra 数据发送后接收返回的结果
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static Zebra_ResponseResult Zebra_ReadJsonResponse(WebResponse response)
        {
            try
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string retJson = sr.ReadToEnd();
                    string ret = Zebra_ResponseLog(retJson);
                    sr.Close();
                    response.Close();
                    var result = (Zebra_ResponseResult)JsonConvert.DeserializeObject(retJson, typeof(Zebra_ResponseResult));
                    result.ret = ret;
                    return result;
                }

            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("Zebra_ReadJsonResponse：" + ex.Message);
                return null;
            }
        }
        private static SCUD_ResponseResult SCUD_ReadResponse(WebResponse response)
        {
            try
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string retJson = sr.ReadToEnd();
                    sr.Close();
                    response.Close();
                    if (string.IsNullOrEmpty(retJson))
                    {
                        throw new Exception("Result is empty.");
                    }
                    SCUD_ResponseResult result = new SCUD_ResponseResult();
                    retJson = retJson.Substring(retJson.IndexOf('{'), retJson.LastIndexOf('}') - retJson.IndexOf('{') + 1);
                    string handle = SCUD_ResponseLog(retJson);
                    result = JsonConvert.DeserializeObject<SCUD_ResponseResult>(retJson);
                    result.data = handle;
                    return result;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("SCUD_ReadResponse：" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// SCUD 数据发送后返回的结果
        /// </summary>
        /// <param name="json">结果</param>
        /// <returns>Handle</returns>
        private static string SCUD_ResponseLog(string json)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Configuer.ConnectionStringBySyncRemote))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT INTO SCUD_RECIEVED_INFO (RECEIVED_INFO) OUTPUT INSERTED.HANDLE VALUES ('{0}');", json);
                    return cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }

        }
        /// <summary>
        /// HW 数据回传
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pars"></param>
        /// <param name="appid"></param>
        /// <param name="appkey"></param>
        /// <returns></returns>
        public HW_Result HW_QuerySoapWebService(string url, string pars, string appid, string appkey)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            //定义调用Web Service的HttpWebRequest对象
            //SysLog log = new SysLog("2.1," + url);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Headers.Add("X-HW-ID", appid);
            request.Headers.Add("X-HW-APPKEY", appkey);
            //log.AddLog("2.2，id:" + request.Headers.GetValues(0) + "，token：" + request.Headers.GetValues(1));
            try
            {
                //设置Content-Type
                request.ContentType = "application/json"; //直接发送HW方式
                //request.ContentType = "application/x-www-form-urlencoded";//直接发送LSN方式
                //设置发送方式POST/GET
                request.Method = "POST";
                //设置身份授权为默认
                request.Credentials = CredentialCache.DefaultCredentials;
                //设置超时为300000ms
                request.Timeout = 30000;
                //将待发送内容转换为byte[]
                byte[] data = EncodePars(pars);

                //log.AddLog("2.3，WriteRequestData");

                //发送内容至 service
                WriteRequestData(request, data);

                //log.AddLog("2.4，ReadJsonResponse");
                //直接发送 HW 后，接收 HW 返回值
                return HW_ReadJsonResponse(request.GetResponse());
            }
            catch (WebException ex)
            {
                /*----------------------------------获取服务器端返回的异常信息--------------------------------------*/
                using (HttpWebResponse res = (HttpWebResponse)ex.Response)
                {
                    Stream ErrorResponseStream = res.GetResponseStream();
                    StreamReader ErrorStreamReader = new StreamReader(ErrorResponseStream, Encoding.UTF8);
                    string remoteMessage = ErrorStreamReader.ReadToEnd();
                    ErrorStreamReader.Close();
                    res.Close();
                    SysLog log2 = new SysLog(ex.Message + " $ 服务器端返回异常信息：" + remoteMessage);
                    HW_Result result = new HW_Result();
                    result.message = remoteMessage;
                    result.code = -1;
                    return result;
                }
            }
        }
        /// <summary>
        /// HW 数据发送后接收返回的结果
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static HW_Result HW_ReadJsonResponse(WebResponse response)
        {
            try
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string retJson = sr.ReadToEnd();
                    HW_Result result = JsonConvert.DeserializeObject<HW_Result>(retJson);
                    sr.Close();
                    response.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("HW_ReadJsonResponse：" + ex.Message);
                return null;
            }
        }
        /******************************************************************************************/
        /************************************ Desay 数据发送 **************************************/
        /******************************************************************************************/

        public string Desay_QuerySoapWebService(string url, string pars, string Ticket)
        {
            //定义调用Web Service的HttpWebRequest对象
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            if (!string.IsNullOrEmpty(Ticket))
            {
                request.Headers.Add("Authorization", "BasicAuth  " + Ticket);
            }
            try
            {
                //设置Content-Type
                request.ContentType = "application/json"; //直接发送 Desay 方式
                                                          //request.ContentType = "application/x-www-form-urlencoded";//直接发送LSN方式
                                                          //设置发送方式POST/GET
                request.Method = "POST";
                //设置身份授权为默认
                request.Credentials = CredentialCache.DefaultCredentials;
                //设置超时为300000ms
                request.Timeout = 300000;
                //将待发送内容转换为byte[]
                byte[] data = EncodePars(pars);
                //发送内容至web service
                WriteRequestData(request, data);
                //直接发送SMP后，接收Zebra返回值
                return ReadJsonResponse(request.GetResponse());
            }
            catch (WebException ex)
            {
                /*----------------------------------获取服务器端返回的异常信息--------------------------------------*/
                using (HttpWebResponse res = (HttpWebResponse)ex.Response)
                {
                    Stream ErrorResponseStream = res.GetResponseStream();
                    StreamReader ErrorStreamReader = new StreamReader(ErrorResponseStream, Encoding.UTF8);
                    string remoteMessage = ErrorStreamReader.ReadToEnd();
                    ErrorStreamReader.Close();
                    res.Close();
                    SysLog log2 = new SysLog(ex.Message + " $ 服务器端返回异常信息：" + remoteMessage);
                    return null;
                }
            }
        }
        public string Desay_QuerySoapWebService2(string url, string pars, string Ticket)
        {
            //定义调用Web Service的HttpWebRequest对象
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            if (!string.IsNullOrEmpty(Ticket))
            {
                request.Headers.Add("x-access-token", Ticket);
            }
            try
            {
                //设置Content-Type
                request.ContentType = "application/json"; //直接发送 Desay 方式
                                                          //request.ContentType = "application/x-www-form-urlencoded";//直接发送LSN方式
                                                          //设置发送方式POST/GET
                request.Method = "POST";
                //设置身份授权为默认
                request.Credentials = CredentialCache.DefaultCredentials;
                //设置超时为300000ms
                request.Timeout = 300000;
                //将待发送内容转换为byte[]
                byte[] data = EncodePars(pars);
                //发送内容至web service
                WriteRequestData(request, data);
                //直接发送SMP后，接收Zebra返回值
                return ReadJsonResponse(request.GetResponse());
            }
            catch (WebException ex)
            {
                /*----------------------------------获取服务器端返回的异常信息--------------------------------------*/
                using (HttpWebResponse res = (HttpWebResponse)ex.Response)
                {
                    Stream ErrorResponseStream = res.GetResponseStream();
                    StreamReader ErrorStreamReader = new StreamReader(ErrorResponseStream, Encoding.UTF8);
                    string remoteMessage = ErrorStreamReader.ReadToEnd();
                    ErrorStreamReader.Close();
                    res.Close();
                    SysLog log2 = new SysLog(ex.Message + " $ 服务器端返回异常信息：" + remoteMessage);
                    return null;
                }
            }
        }
        public string ReadJsonResponse(WebResponse response)
        {
            try
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string retJson = sr.ReadToEnd();
                    sr.Close();
                    response.Close();
                    return retJson;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("ReadJsonResponse：" + ex.Message);
                return null;
            }
        }
        public ResultLogin Login (string url,EntityLogin login)
        {
            try
            {
                string str = JsonConvert.SerializeObject(login);
                string result = Desay_QuerySoapWebService(url, str, null);
                return JsonConvert.DeserializeObject<ResultLogin>(result);
            }
            catch(Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        public TokenDesay Login_new(string url, EntityLogin2 login)
        {
            try
            {
                string str = JsonConvert.SerializeObject(login);
                string result = Desay_QuerySoapWebService(url, str, null);
                return JsonConvert.DeserializeObject<TokenDesay>(result);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 数据发送
        /// </summary>
        /// <param name="url">对方接收地址</param>
        /// <param name="para">发送的内容</param>
        /// <param name="Ticket">发送数据需要填写票据</param>
        /// <returns></returns>
        public ResultPost Post(string url,string para,string Ticket,ref string result)
        {
            try
            {
                string ret = Desay_QuerySoapWebService(url, para, Ticket);
                result = ret;
                return JsonConvert.DeserializeObject<ResultPost>(ret);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 数据发送，2023-11-20 新版本
        /// </summary>
        /// <param name="url">对方接收地址</param>
        /// <param name="para">发送的内容</param>
        /// <param name="Ticket">发送数据需要填写票据</param>
        /// <returns></returns>
        public ResultPost2 Post2(string url, string para, string Ticket, ref string result)
        {
            try
            {
                string ret = Desay_QuerySoapWebService2(url, para, Ticket);
                result = ret;
                return JsonConvert.DeserializeObject<ResultPost2>(ret);
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 向 TWS 发送数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="pars">数据</param>
        /// <returns>结果字符串</returns>
        public string TWS_QuerySoapWebService(string url,string pars)
        {
            //定义调用Web Service的HttpWebRequest对象
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                //设置Content-Type
                request.ContentType = "application/json"; //直接发送 TWS 方式
                //request.ContentType = "application/x-www-form-urlencoded";//直接发送LSN方式
                //设置发送方式POST/GET
                request.Method = "POST";
                //设置身份授权为默认
                request.Credentials = CredentialCache.DefaultCredentials;
                //设置超时为300000ms
                request.Timeout = 300000;
                //将待发送内容转换为byte[]
                byte[] data = EncodePars(pars);
                //发送内容至web service
                WriteRequestData(request, data);
                //直接发送 TWS 后，接收 TWS 返回值
                return TWS_ReadResponse(request.GetResponse());
            }
            catch (WebException ex)
            {
                /*----------------------------------获取服务器端返回的异常信息--------------------------------------*/
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream ErrorResponseStream = res.GetResponseStream();
                StreamReader ErrorStreamReader = new StreamReader(ErrorResponseStream, Encoding.UTF8);
                string remoteMessage = ErrorStreamReader.ReadToEnd();
                ErrorStreamReader.Close();
                SysLog log2 = new SysLog(ex.Message + " $ 服务器端返回异常信息：" + remoteMessage);
                return null;
            }
        }
        /// <summary>
        /// 读取 TWS 返回结果
        /// </summary>
        /// <param name="response">结果参数</param>
        /// <returns>字符串</returns>
        private static string TWS_ReadResponse(WebResponse response)
        {
            try
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string retJson = sr.ReadToEnd();
                    sr.Close();
                    response.Close();
                    if (string.IsNullOrEmpty(retJson))
                    {
                        throw new Exception("Result is empty.");
                    }
                    return retJson;
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("TWS_ReadResponse：" + ex.Message);
                return null;
            }
        }
    }
}
