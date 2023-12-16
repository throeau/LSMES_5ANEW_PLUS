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
using Newtonsoft.Json;

namespace Listener_Pack
{
    class Program
    {
        static public Entity cellList;
        static public ISession session;
        static public IMessageConsumer consumer;
        static public IMessageProducer producer;
        static public string REQ_ID;
        static void Main(string[] args)
        {
            try
            {
                cellList = new Entity();
                Uri connectUri = new Uri(ConfigurationManager.AppSettings["uri"]);
                IConnectionFactory factory = new ConnectionFactory(connectUri);
                Console.Write("SAP ME(PACK) 向LSN MES 请求电芯数据 Ver:1.1");
                using (IConnection connection = factory.CreateConnection(ConfigurationManager.AppSettings["uid"], ConfigurationManager.AppSettings["pwd"]))
                {
                    connection.Start();
                    using (session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                    {
                        IDestination destination = session.GetQueue(ConfigurationManager.AppSettings["queue"]);
                        consumer = session.CreateConsumer(destination);
                        //producer = session.CreateProducer(destination);
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
                SysLog log = new SysLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Receive：\n\r" + ((ITextMessage)msg).Text+"\n\r");
                LotEntity lot = new LotEntity();
                lot = JsonConvert.DeserializeObject<LotEntity>(((ITextMessage)msg).Text);
                
                if (string.IsNullOrEmpty(lot.LOT_NO) || REQ_ID == lot.REQ_ID)
                {
                    return;
                }
                else
                {
                    REQ_ID = lot.REQ_ID;
                }
                cellList = PostSoap(lot.LOT_NO, 1, 2000);
                cellList.REQ_ID = lot.REQ_ID;
                if (cellList.SN_LIST.Count > 0)
                {
                    PostQueues(msg, JsonConvert.SerializeObject(cellList));
                }
                Thread.Sleep(3000);
                cellList.SN_LIST.Clear();
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
        public static Entity PostSoap(string content, int repeat, int second)
        {
            //string content = "{\"DATA\":[{\"SN\":\"N689521U052C8\",\"CORE_TYPE_CODE\":\"SP515974SG\",\"OP11_A013\":0.0238,\"OP11_A016\":\"合格\"},{\"SN\":\"N689521U052C9\",\"CORE_TYPE_CODE\":\"SP515974SG\",\"OP11_A013\":0.0244,\"OP11_A016\":\"合格\"},{\"SN\":\"N689521U051FA\",\"CORE_TYPE_CODE\":\"SP515974SG\",\"OP11_A013\":0.0161,\"OP11_A016\":\"合格\"}]}";
            Entity cellList = new Entity();
            try
            {
                Hashtable contentParameters = new Hashtable();
                contentParameters.Add(System.Configuration.ConfigurationManager.AppSettings["webservice_para"], content);

                WebService web = new WebService();
                string result = web.QuerySoapWebService(System.Configuration.ConfigurationManager.AppSettings["url"], System.Configuration.ConfigurationManager.AppSettings["op"], contentParameters);
                cellList = JsonConvert.DeserializeObject<Entity>(result);
                if (cellList.RESULT == "1")
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "批次 ["+ content + "] 数据请求成功.");
                    Console.WriteLine(msg.ToString() + "\r\n" + result);
                    SysLog log = new SysLog(msg.ToString() + result);
                }
                else
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "批次 [" + content + "] 数据请求失败.");
                    Console.WriteLine(msg.ToString() + "\r\n" + result);
                    SysLog log = new SysLog(msg.ToString() + result);
                }
                //PostQueues(JsonConvert.SerializeObject(cellList));
            }
            catch (WebServiceException ex)
            {
                /*----------------------------------获取本地运行程序的异常信息--------------------------------------*/
                StringBuilder err = new StringBuilder();
                err.Append("向web service(" + System.Configuration.ConfigurationManager.AppSettings["url"] + "/" + System.Configuration.ConfigurationManager.AppSettings["op"] + ")发送数据失败，数据内容：" + content + "\n");
                SysLog log = new SysLog(err.ToString());
                /*-------------------------------------------------------------------------------------------------*/
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "数据请求失败，具体原因请见日志.");
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog("访问web service时异常，具体信息：" + ex.Message + "\n\r发送失败内容：" + content);
                if (repeat > 0)
                {
                    Thread.Sleep(second);
                    SysLog log2 = new SysLog("间隔" + second + "秒后，数据开始尝试重新发送...");
                    PostSoap(content, --repeat, second);
                }
            }
            return cellList;
        }
        public static void PostQueues(object rmsg,string content)
        {
            try
            {

                //Uri connectUri = new Uri(ConfigurationManager.AppSettings["uri"]);
                //IConnectionFactory factory = new ConnectionFactory(connectUri);

                //using (IConnection connection = factory.CreateConnection(ConfigurationManager.AppSettings["uid"], ConfigurationManager.AppSettings["pwd"]))
                //{
                //    connection.Start();
                //    using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                //    {
                IDestination destination = session.GetQueue(ConfigurationManager.AppSettings["queue"]);
                IMessageProducer producer = session.CreateProducer(destination);
                ITextMessage tmsg = producer.CreateTextMessage(content);
                tmsg.NMSReplyTo = ((ITextMessage)rmsg).NMSDestination;
                producer.Send(tmsg);
                        StringBuilder msg = new StringBuilder();
                        msg.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "批次 [" + content + "] 数据回复成功.");
                        Console.WriteLine(msg.ToString() + "\r\n");
                        SysLog log = new SysLog(msg.ToString());
                //}
                //connection.Close();
                //}
                producer.Close();
            }
            catch (Exception ex)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "批次 [" + content + "] 数据回复失败.");
                Console.WriteLine(msg.ToString() + "\r\n");
                SysLog log = new SysLog(msg.ToString());
            }
        }
    }
}
