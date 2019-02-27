using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Mail;

namespace CommonSpider.Common.Sender
{
    public class FtqqSender
    {
        public static void Send(string subject, string content = null, Model.FtqqConfig config = null)
        {
            if (config == null)
            {
                var senderConfig = JsonConvert.DeserializeObject<Model.SenderConfig>(File.ReadAllText(@"Configs/SenderConfig.json"));
                config = senderConfig.Ftqq;
                if (config == null)
                {
                    throw new ArgumentNullException("FtqqConfig", "Could not found config for Ftqq from SenderConfig");
                }
            }

            string key = config.Key;
            string url = string.Format(config.Url, key);

            var client = new EasyHttp.Http.HttpClient();
            var response = client.Get(url, new { text = subject, desp = content });
        }
    }
}
