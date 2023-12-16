using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Security;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.WebServices.Data;
using System.Configuration;
using System.IO;
using System.Text;

namespace LSMES_5ANEW_PLUS.App_Base
{
    public class Mail
    {
        /// <summary>
        /// exchange服务对象
        /// </summary>
        private static ExchangeService _exchangeService = new ExchangeService(ExchangeVersion.Exchange2010_SP1);

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static void SendMail(string email, string userId, string pwd, string domain,string title,string body)
        {
            try
            {
                if (ConfigurationManager.AppSettings["IsMail"].Trim().ToUpper() != "TRUE") return;

                _exchangeService.Credentials = new NetworkCredential(userId, pwd);
                _exchangeService.Url = new Uri(System.Configuration.ConfigurationManager.AppSettings["ExchangeServerURL"]);

                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { return true; };

                //发送人
                //Mailbox mail = new Mailbox(userId);
                
                //邮件内容
                EmailMessage message = new EmailMessage(_exchangeService);
                string[] strTos = email.Split(';');
                //string[] strTos = { "zhao_zichao@lishen.com" };

                //接收人
                foreach (string item in strTos)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        message.ToRecipients.Add(item);
                    }
                }
                //抄送人
                //foreach (string item in email.Mail_cc.Split(';'))
                //{
                //    if (!string.IsNullOrEmpty(item))
                //    {
                //        message.CcRecipients.Add(item);
                //    }

                //}
                //邮件标题
                message.Subject = title;
                //邮件内容
                message.Body = new MessageBody(body);
                message.Body.BodyType = BodyType.HTML;
                //发送并且保存
                message.SendAndSaveCopy();
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                Stream ErrorResponseStream = res.GetResponseStream();
                StreamReader ErrorStreamReader = new StreamReader(ErrorResponseStream, Encoding.UTF8);
                string remoteMessage = ErrorStreamReader.ReadToEnd();
                ErrorStreamReader.Close();
                res.Close();
                SysLog log = new SysLog(ex.Message + " $ 服务器端返回异常信息：" + remoteMessage);
            }
            //catch (Exception ex)
            //{
            //    throw new Exception("发送邮件出错，" + ex.Message + "\r\n" + ex.StackTrace);
            //}
        }
    }
}