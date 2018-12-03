using HtmlAgilityPack;
using RentSpider.Entities;
using RentSpider.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace RentSpider.Services
{
    public class RentSummaryService
    {
        private static RentSummaryRepository _rentSummaryRepository = new RentSummaryRepository();

        public void GetNotice()
        {
            var list = GetList();
            if (list.Count > 0)
            {
                SendNotice(list);
            }
        }

        private void SendNotice(List<RentSummary> rentSummarys)
        {
            SmtpClient client = new SmtpClient("smtp.qq.com");
            client.Credentials = new System.Net.NetworkCredential("1396252309@qq.com", "123asD");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Timeout = 60000;
            client.EnableSsl = true;

            MailMessage message = new MailMessage();
            message.From = new MailAddress("1396252309@qq.com");
            message.To.Add("1396252309@qq.com");
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.Normal;
            message.Subject = string.Format("租房列表-{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));


            StringBuilder sb = new StringBuilder();
            sb.Append(@"
        <meta name=""viewport"" content=""width = device - width, initial - scale = 1.0, minimum - scale = 1.0, maximum -

        scale = 1.0, user - scalable = no""/>
        <style>
        pre{
            font-family: 'Microsoft YaHei';
            white-space: pre-wrap;
            white-space: -moz-pre-wrap;
            white-space: -pre-wrap;
            white-space: -o-pre-wrap;
            word-wrap: break-word;
        }
        </style>");
            foreach (var item in rentSummarys)
            {
                sb.AppendFormat(@"{0}<a href='{1}'>详细</a><hr>", item.Title, item.Url);
                sb.AppendLine();
            }
            sb.AppendFormat("抓取时间: {0}", DateTime.Now.ToString("yyyy-MM-dd"));

            message.Body = sb.ToString();

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {

            }
        }

        private static readonly string listUrl = ConfigurationManager.AppSettings.Get("TargetUrl");
        private static readonly string[] keys = ConfigurationManager.AppSettings.Get("KeyWords").Split('|');
        private static List<RentSummary> GetList()
        {
            List<RentSummary> items = new List<RentSummary>();

            HtmlDocument doc = new HtmlWeb().Load(listUrl);
            var table = doc?.DocumentNode?.SelectNodes(".//table[contains(@class, 'olt')]")?.FirstOrDefault();
            if (table != null)
            {
                var trs = table.SelectNodes(".//tr");
                trs = trs == null ? new HtmlNodeCollection(null) : trs;
                trs.RemoveAt(0);// 移除标题行
                foreach (var tr in trs)
                {
                    var tds = tr.SelectNodes(".//td");
                    var titleNode = tds[0].SelectNodes(".//a[@href]").FirstOrDefault();
                    //var title = titleNode.InnerText.Replace("&nbsp;", "").Trim();
                    var title = titleNode?.Attributes["title"]?.Value.TrimEnd('/');
                    if (keys.Any(p => title.Contains(p)))
                    {
                        var author = tds[1].InnerText.Replace("\\", "");
                        var url = titleNode?.Attributes["href"]?.Value.TrimEnd('/');
                        var id = Convert.ToInt32(url.Substring(url.LastIndexOf('/') + 1));
                        if (_rentSummaryRepository.Query(id) == null)
                        {
                            var rentSummary = new RentSummary()
                            {
                                Id = id,
                                Title = title,
                                Url = url,
                                Author = author,
                            };
                            items.Add(rentSummary);
                            _rentSummaryRepository.Insert(rentSummary);
                        }
                    }
                }
            }

            return items;
        }
    }
}
