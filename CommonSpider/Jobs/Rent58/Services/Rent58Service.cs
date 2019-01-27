using CommonSpider.Common;
using CommonSpider.Interfaces;
using CommonSpider.Jobs.Rent58.Entities;
using CommonSpider.Jobs.Rent58.Repositories;
using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonSpider.Jobs.Rent58.Services
{
    public class Rent58Service : IService
    {
        private static ILog _logger;
        private Dictionary<string, object> _data;

        private static string _detailUrl;
        private static Rent58Repository _szWaterRepository = new Rent58Repository();

        // todo: 重新构建
        public void Init(string targetUrl, Dictionary<string, object> data)
        {
            _logger = LogManager.GetLogger(typeof(Rent58Service));
            _data = data;
            //_detailUrl = _data["detailUrl"].ToString();

            List<Rent58Item> items = new List<Rent58Item>();
            var ids = GetList(targetUrl);
            ids.ForEach(id =>
            {
                var exist = _szWaterRepository.Query(id) == null;
                if (!exist)
                {
                    var notice = GetDetail(id);
                    _szWaterRepository.Insert(notice);
                    items.Add(notice);
                }
            });

            if (items.Count > 0)
            {
                SendNotices(items);
            }
        }

        private void SendNotices(List<Rent58Item> notices)
        {
            string subject = string.Format("58租房-{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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
            sb.AppendFormat("抓取时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            try
            {
                EmailHelper.Send(subject, sb.ToString());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        // todo: 重新构建
        private static List<string> GetList(string targetUrl)
        {
            List<string> ids = new List<string>();

            HtmlDocument doc = new HtmlWeb().Load(targetUrl);
            var items = doc?.DocumentNode?.SelectNodes(".//div[contains(@class, 'des')]");
            foreach (var li in items)
            {
                var titleContainer = li.SelectSingleNode(".//h2");
                var title = titleContainer.InnerText;
                var link = $"https://{titleContainer.SelectSingleNode(".//a[@href]").Attributes["href"]}";


                //var href = li.SelectSingleNode(".//div");
                //var idStr = Regex.Replace(href?.Value, @"([\S]*)[id=]", "");
                //ids.Add(idStr);
            }

            return ids;
        }

        // todo: 重新构建
        private static Rent58Item GetDetail(string id)
        {
            Rent58Item item = new Rent58Item();

            string url = string.Format(_detailUrl, id);
            HtmlDocument doc = new HtmlWeb().Load(url);
            var rightDiv = doc.DocumentNode.SelectSingleNode(".//div[contains(@class, 'r_txt')]");
            if (rightDiv != null)
            {
                var titleDiv = rightDiv.SelectSingleNode(".//div[contains(@id, 'bt_news')]");
                var dateDiv = rightDiv.SelectSingleNode(".//div[contains(@class, 'news_time')]");

                item.Id = id;
                item.Title = titleDiv.InnerText.Trim();
                item.PublishDate = Convert.ToDateTime(dateDiv.SelectSingleNode(".//span")?.InnerText?.Replace("发表时间：", ""));
                string content = rightDiv.InnerText.Trim();
                var startIndex = (content.IndexOf("【停水时间】") - 1) < 0 ? 0 : (content.IndexOf("【停水时间】") - 1);
                item.Content = content.Substring(startIndex).Replace("<br>", "");
                item.Url = url;
            }

            return item;
        }
    }
}
