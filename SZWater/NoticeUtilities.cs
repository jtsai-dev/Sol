using Dapper;
using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace SZWater
{
    public class NoticeUtilities
    {
        private ILog _logger;
        public NoticeUtilities()
        {
            _logger = LogManager.GetLogger(typeof(NoticeUtilities));
        }

        public void GetNotice()
        {
            List<Notice> notices = new List<Notice>();

            var ids = GetIds();
            ids.ForEach(id =>
            {
                var exist = CheckExist(id);
                if (!exist)
                {
                    var notice = GetDetail(id);
                    Insert(notice);
                    notices.Add(notice);
                }
            });

            if (notices.Count > 0)
            {
                _logger.Info("Got new notices, send email");
                SendNotices(notices);
            }
        }

        /// <summary>
        /// 发送通知到邮箱
        /// </summary>
        /// <param name="notices"></param>
        private void SendNotices(List<Notice> notices)
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
            message.Subject = string.Format("停水通知-{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));


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
            foreach (var notice in notices)
            {
                sb.AppendFormat(@"<h4>{0}</h4>
<small>{1}</small>
<p>
<pre>{2}</pre>
</p>
<a href='{3}'>详细</a>
<hr>", notice.Title, notice.PublishDate, notice.Content, notice.Url);
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
                _logger.Error(ex);
            }
        }

        #region 获取停水通知列表 id 集合(仅获取第一页的数据)
        private static readonly string listUrl = "http://www.sz-water.com.cn/Announcement.aspx?nc=101034004";
        private static List<string> GetIds()
        {
            List<string> ids = new List<string>();

            HtmlDocument doc = new HtmlWeb().Load(listUrl);
            var ul = doc?.DocumentNode?.SelectNodes(".//div[contains(@class, 'r_txt')]")?.FirstOrDefault();
            if (ul != null)
            {
                var links = ul.SelectNodes(".//a[@href]");
                links = links == null ? new HtmlNodeCollection(null) : links;
                foreach (var link in links)
                {
                    var href = link.Attributes["href"];
                    var idStr = Regex.Replace(href?.Value, @"([\S]*)[id=]", "");
                    ids.Add(idStr);
                }
            }

            return ids;
        }
        #endregion

        #region 获取停水通知详细
        private static readonly string detailUrl = "http://www.sz-water.com.cn/newsdetails.aspx?nc=101034004&id={0}";
        private static Notice GetDetail(string id)
        {
            Notice notice = new Notice();

            string url = string.Format(detailUrl, id);
            HtmlDocument doc = new HtmlWeb().Load(url);
            var rightDiv = doc.DocumentNode.SelectSingleNode(".//div[contains(@class, 'r_txt')]");
            if (rightDiv != null)
            {
                var titleDiv = rightDiv.SelectSingleNode(".//div[contains(@id, 'bt_news')]");
                var dateDiv = rightDiv.SelectSingleNode(".//div[contains(@class, 'news_time')]");

                notice.Id = id;
                notice.Title = titleDiv.InnerText.Trim();
                notice.PublishDate = Convert.ToDateTime(dateDiv.SelectSingleNode(".//span")?.InnerText?.Replace("发表时间：", ""));
                string content = rightDiv.InnerText.Trim();
                var startIndex = (content.IndexOf("【停水时间】") - 1) < 0 ? 0 : (content.IndexOf("【停水时间】") - 1);
                notice.Content = content.Substring(startIndex).Replace("<br>", "");
                notice.Url = url;
            }

            return notice;
        }
        #endregion

        private static bool CheckExist(string id)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "SELECT * from [Notice] WHERE Id = @id";
                var notices = conn.Query<Notice>(query, new { id = id }).FirstOrDefault();
                return (notices != null);
            }
        }

        private static bool Insert(Notice notice)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "INSERT INTO Notice([Id], [Title], [PublishDate], [Content], [Url]) VALUES(@id, @title, @publishDate, @content, @url)";
                int row = conn.Execute(query, notice);
                return row > 0;
            }
        }

        //private static readonly string connStr = "Data Source=10.99.37.177\\SQL2012;Initial Catalog=SZWater;User Id=sa;Password=123asD;";
        //private static SqlConnection OpenConnection()
        //{
        //    SqlConnection connection = new SqlConnection(connStr);
        //    connection.Open();
        //    return connection;
        //}

        private static readonly string _dbDir = ConfigurationManager.AppSettings["connStr"];
        private static SQLiteConnection OpenConnection()
        {
            SQLiteConnection connection = new SQLiteConnection($"Data Source={_dbDir}");
            connection.Open();
            return connection;
        }
    }

    public class Notice
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
    }
}
