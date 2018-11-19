using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Dapper;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace RentSpider
{
    public class RentSummaryUtilities
    {
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

        private static readonly string listUrl = "https://www.douban.com/group/512844";
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
                    var keys = new string[] { "两房", "2房", "两室", "2室" };
                    if (keys.Any(p => title.Contains(p)))
                    {
                        var author = tds[1].InnerText.Replace("\\", "");
                        var url = titleNode?.Attributes["href"]?.Value.TrimEnd('/');
                        var id = Convert.ToInt32(url.Substring(url.LastIndexOf('/') + 1));
                        if (!CheckExist(id))
                        {
                            var rentSummary = new RentSummary()
                            {
                                Id = id,
                                Title = title,
                                Url = url,
                                Author = author,
                            };
                            items.Add(rentSummary);
                            Insert(rentSummary);
                        }
                    }
                }
            }

            return items;
        }

        private static bool CheckExist(int id)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "SELECT * from RentSummary WHERE Id = @id";
                var rentSummary = conn.Query<RentSummary>(query, new { id = id }).FirstOrDefault();
                return (rentSummary != null);
            }
        }

        private static bool Insert(RentSummary rentSummary)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "INSERT INTO rentsummary(Id, Title, Author, Url) VALUES(@id, @title, @author, @url)";
                int row = conn.Execute(query, rentSummary);
                return row > 0;
            }
        }

        private static MySqlConnection _connection;
        private static MySqlConnection OpenConnection()
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection($"Server=127.0.0.1;Port=3306;Uid=root;Pwd=123asD;DataBase=RentSpider;Convert Zero Datetime=True; allow zero datetime=true;Max Pool Size=10000;Allow User Variables=True");
            }
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            return _connection;
        }
    }

    public class RentSummary
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
    }
}
