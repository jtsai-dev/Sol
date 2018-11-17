using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace JT.Infrastructure
{
    public class EMail
    {
        public static void SendMail(string[] to, string subject, string content, string[] attachments = null, bool isAsync = true)
        {
            System.Net.Mail.SmtpClient client = new SmtpClient("smtp.qq.com");//根据发送邮箱具体类型而定
            //client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("1396252309@qq.com", "123asD");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Timeout = 60000;

            System.Net.Mail.MailMessage message = new MailMessage();
            message.From = new MailAddress("1396252309@qq.com");
            foreach (var item in to)
            {
                if (!string.IsNullOrEmpty(item))
                    message.To.Add(item);
            }
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.Normal;
            message.Subject = subject;
            message.Body = content;
            if (attachments != null)
            {
                foreach (string item in attachments)
                {
                    message.Attachments.Add(new Attachment(item));
                }
            }

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                foreach (var item in message.Attachments)
                {
                    item.Dispose();
                }
                message.Attachments.Dispose();
            }

        }
    }
}
