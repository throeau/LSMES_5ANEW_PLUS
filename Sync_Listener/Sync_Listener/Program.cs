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

namespace Sync_Listener
{
    class Program
    {
        static public ISession session;
        static public IMessageConsumer consumer;
        static public IMessageProducer producer;
        static void Main(string[] args)
        {
            try
            {
                Uri connectUri = new Uri(ConfigurationManager.AppSettings["uri"]);
                IConnectionFactory factory = new ConnectionFactory(connectUri);
                Console.Write("Ver:1.0\n");
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
                Write_HW_RECIEVE_LOG(((ITextMessage)msg).Text);
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
        static void Recieve()
        {

        }
        static void Write_HW_RECIEVE_LOG(string content)
        {
            try
            {
                Hashtable contentParameters = new Hashtable();
                contentParameters.Add(System.Configuration.ConfigurationManager.AppSettings["webservice_para"], content);

                WebService web = new WebService();
                string result = web.QuerySoapWebService(System.Configuration.ConfigurationManager.AppSettings["url"], System.Configuration.ConfigurationManager.AppSettings["op"], contentParameters);
                if (result.Contains("1"))
                {
                    Console.Write("--------------  Success，数据接收成功.  --------------\n");
                    SysLog log = new SysLog(content);
                }
                else
                {
                    Console.Write("--------------  Fail，数据接收失败.  --------------\n");
                }
            }
            catch (Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                Console.Write("Fail，" + ex.Message + ".\n");

            }
        }
    }
}
