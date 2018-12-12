using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Mail;

namespace CommonSpider.Common
{
    public class EmailHelper
    {
        public static void Send(string subject, string content, Model.EmailConfig config = null)
        {
            config = config ?? JsonConvert.DeserializeObject<Model.EmailConfig>(File.ReadAllText(@"Configs/EmailConfig.json"));

            SmtpClient client = new SmtpClient(config.Host);
            client.Credentials = new System.Net.NetworkCredential(config.UserName, config.Password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Timeout = 60000;
            client.EnableSsl = true;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(config.From);
            foreach (var item in config.To)
            {
                message.To.Add(item);
            }
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.Normal;
            message.Subject = subject;
            message.Body = content;

            client.Send(message);
        }
    }
}
