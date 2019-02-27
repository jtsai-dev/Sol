using CommonSpider.Common.Sender;
using CommonSpider.Interfaces;
using CommonSpider.Jobs.SZWater.Entities;
using CommonSpider.Jobs.SZWater.Repositories;
using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonSpider.Jobs.SZWater.Services
{
    public class SZWaterService : IService
    {
        private static ILog _logger;
        private Dictionary<string, object> _data;

        private static string _detailUrl;
        private static SZWaterRepository _szWaterRepository = new SZWaterRepository();

        public async Task ExcuteAsync(string name, string targetUrl, Dictionary<string, object> data)
        {
            _logger = LogManager.GetLogger(typeof(SZWaterService));
            _data = data;
            _detailUrl = _data["detailUrl"].ToString();

            List<Notice> notices = new List<Notice>();
            var ids = GetList(targetUrl);
            ids.ForEach(async id =>
            {
                var exist = await _szWaterRepository.QueryAsync(id) == null;
                if (!exist)
                {
                    var notice = GetDetail(id);
                    await _szWaterRepository.InsertAsync(notice);
                    notices.Add(notice);
                }
            });

            if (notices.Count > 0)
            {
                string subject = $"{name}-{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
                string message = GenerateMessage(notices);
                try
                {
                    EmailSender.Send(subject, message);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
        }

        private string GenerateMessage(List<Notice> notices)
        {
            StringBuilder sb = new StringBuilder();
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
            return sb.ToString();
        }

        private List<string> GetList(string targetUrl)
        {
            List<string> ids = new List<string>();

            HtmlDocument doc = new HtmlWeb().Load(targetUrl);
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

        private Notice GetDetail(string id)
        {
            Notice notice = new Notice();

            string url = string.Format(_detailUrl, id);
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
    }
}
