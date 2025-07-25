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
using System.Text.RegularExpressions;

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
            if (System.Configuration.ConfigurationManager.AppSettings["IsAliMail"].ToString().ToUpper() != "TRUE")
            {
                SendMailByLishen(email, System.Configuration.ConfigurationManager.AppSettings["ExchangeUID"], System.Configuration.ConfigurationManager.AppSettings["ExchangePWD"], System.Configuration.ConfigurationManager.AppSettings["ExchangeDomain"], title, body);
            }
            else
            {
                SendMailByAliYun(email, title, body);
            }
        }
        public static void SendMailByLishen(string email, string userId, string pwd, string domain, string title, string body)
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
        public static void SendMailByAliYun(string email, string title, string body)
        {
            if (ConfigurationManager.AppSettings["IsMail"].Trim().ToUpper() != "TRUE") return;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            SmtpClient smtpClient = new SmtpClient();
            //smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            //smtpClient.UseDefaultCredentials = false; // 必须设置为false
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Host = System.Configuration.ConfigurationManager.AppSettings["AliYunURL"];
            smtpClient.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AliPort"]);
            if (smtpClient.Port == 465) smtpClient.EnableSsl = true;
            smtpClient.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["AliUID"], System.Configuration.ConfigurationManager.AppSettings["AliPWD"]);
            if (email[email.Length - 1] == ',')
            {
                email = email.Substring(0, email.Length - 1);
            }
            MailMessage mailMessage = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["AliUID"], email);//from（发出邮箱）和to（目标邮箱）
            mailMessage.Subject = title;//邮件标题 
            mailMessage.Body = body;//邮件内容 
            mailMessage.BodyEncoding = System.Text.Encoding.Default;//正文编码  
            mailMessage.IsBodyHtml = true;//设置为HTML格式  
            mailMessage.Priority = MailPriority.High;//优先级  
            //mailMessage.To.Add("xxx@xxx.com");
            //mailMessage.To.Add("aaa@xxx.com");
            //mailMessage.To.Add("bbb@xxx.com");
            //mailMessage.To.Add("ccc@xxx.com");//可以发送给多个人
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception e)
            {
                SysLog log = new SysLog(e.Message + " $ 服务器端返回异常信息：" + email);
                // 若服务器返回某一邮箱地址存在异常，通过以下函数清理
                email = reassembleEmails(email, e.Message);
                if (!string.IsNullOrEmpty(email))
                {
                    SendMailByAliYun(email, title, body);
                }
            }
        }
        /// <summary>
        /// 检验是否存在异常邮箱地址，若存在异常地址则剔除该地址
        /// </summary>
        /// <param name="emails">邮箱列表</param>
        /// <param name="exp">异常信息</param>
        /// <returns>剔除该地址后的邮箱列表</returns>
        public static string reassembleEmails(string emails, string exp)
        {
            // 定义匹配邮箱地址的正则表达式模式
            string pattern = @"(\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b)(?=.*dosn't exist)";
            // 判断异常信息中是否有邮件信息
            Match emailMatch = Regex.Match(exp, pattern);
            // 不存在异常邮件地址
            if (!emailMatch.Success)
            {
                return null;
            }
            // 使用正则表达式替换邮箱地址为空字符串
            pattern = emailMatch.Value + ",?";
            emails = Regex.Replace(emails, pattern, "");
            return emails;
        }
    }
}