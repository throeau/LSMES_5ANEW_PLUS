using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Util;
using Apache.NMS.ActiveMQ.Commands;
using System.Configuration;
using System.Threading;

namespace Listener
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Uri connectUri = new Uri(ConfigurationManager.AppSettings["uri"]);
                IConnectionFactory factory = new ConnectionFactory(connectUri);
                Console.Write("Ver:2.0");
                using (IConnection connection = factory.CreateConnection(ConfigurationManager.AppSettings["uid"], ConfigurationManager.AppSettings["pwd"]))
                {
                    connection.Start();
                    using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                    {
                        IDestination destination = session.GetQueue(ConfigurationManager.AppSettings["queue"]);
                        IMessageConsumer consumer = session.CreateConsumer(destination);
                        consumer.Listener += new MessageListener(OnMessages);
                        Console.ReadLine();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
                Console.ReadLine();
            }
        }
        static void OnMessages(object msg)
        {
            try
            {
                Console.WriteLine("Receive：{0}", ((ITextMessage)msg).Text);
                //PostSoap(((ITextMessage)msg).Text,1,2000);
            }
            catch (Exception ex)
            {
                /*----------------------------------获取本地ActiveMQ监听的异常信息--------------------------------------*/
                StringBuilder err = new StringBuilder();
                err.Append("监听ActiveMQ(" + ConfigurationManager.AppSettings["uri"] + "/" + ConfigurationManager.AppSettings["queue"] + ")失败，异常信息：" + ex.Message);
                SysLog log = new SysLog(err.ToString());
                /*------------------------------------------------------------------------------------------------------*/
                Console.ReadLine();
            }
        }
        /// <summary>
        /// 向web service发送数据
        /// </summary>
        /// <param name="content">待发送内容</param>
        /// <param name="repeat">重复次数：0｜不重复</param>
        /// <param name="second">重复时间间隔：单位毫秒</param>
        public static void PostSoap(string content,int repeat,int second)
        {
            content = "{\"DATA\":[{\"SN\":\"N689521U052C8\",\"CORE_TYPE_CODE\":\"SP515974SG\",\"OP11_A013\":0.0238,\"OP11_A016\":\"合格\"},{\"SN\":\"N689521U052C9\",\"CORE_TYPE_CODE\":\"SP515974SG\",\"OP11_A013\":0.0244,\"OP11_A016\":\"合格\"},{\"SN\":\"N689521U051FA\",\"CORE_TYPE_CODE\":\"SP515974SG\",\"OP11_A013\":0.0161,\"OP11_A016\":\"合格\"}]}";

            try
            {
                Hashtable contentParameters = new Hashtable();
                contentParameters.Add(System.Configuration.ConfigurationManager.AppSettings["webservice_para"], content);

                WebService web = new WebService();
                XmlDocument result = web.QuerySoapWebService(System.Configuration.ConfigurationManager.AppSettings["url"], System.Configuration.ConfigurationManager.AppSettings["op"], contentParameters);
                if (result.InnerText == "0")
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "数据同步成功.");
                    Console.WriteLine(msg.ToString()+"\n");
                    SysLog log = new SysLog(msg.ToString());
                }
                else
                {
                    throw new WebServiceException();
                }
            }
            catch (WebServiceException ex)
            {
                /*----------------------------------获取本地运行程序的异常信息--------------------------------------*/
                StringBuilder err = new StringBuilder();
                err.Append("向web service(" + System.Configuration.ConfigurationManager.AppSettings["url"] + "/" + System.Configuration.ConfigurationManager.AppSettings["op"] + ")发送数据失败，数据内容：" + content + "\n");
                SysLog log = new SysLog(err.ToString());
                /*-------------------------------------------------------------------------------------------------*/
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "数据同步失败，具体原因请见日志.");
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("访问web service时异常，具体信息：" + ex.Message + "\n\r发送失败内容：" + content);
                if (repeat>0)
                {
                    Thread.Sleep(second);
                    SysLog log2 = new SysLog("间隔"+second+"秒后，数据开始尝试重新发送...");
                    PostSoap(content, --repeat, second);
                }
            }
        }
    }
}
