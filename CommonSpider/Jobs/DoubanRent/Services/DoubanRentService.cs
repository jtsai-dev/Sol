using CommonSpider.Common.Sender;
using CommonSpider.Interfaces;
using CommonSpider.Jobs.DoubanRent.Entities;
using CommonSpider.Jobs.DoubanRent.Repositories;
using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonSpider.Jobs.DoubanRent.Services
{
    public class DoubanRentService : IService
    {
        private static ILog _logger;
        private Dictionary<string, object> _data;

        private static string[] _keys;
        private static DoubanRentRepository _rentSummaryRepository = new DoubanRentRepository();

        public async Task ExcuteAsync(string name, string targetUrl, Dictionary<string, object> data)
        {
            _logger = LogManager.GetLogger(typeof(DoubanRentService));
            _data = data;
            _keys = _data["keys"].ToString().Split('|');
            var list = await GetList(targetUrl);
            if (list.Count > 0)
            {
                string subject = $"{name}-{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
                string message = GenerateMessage(list);
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

        private string GenerateMessage(List<DoubanRentSummary> rentSummarys)
        {
            string subject = string.Format("租房列表-{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            StringBuilder sb = new StringBuilder();
            foreach (var item in rentSummarys)
            {
                sb.AppendFormat(@"{0}<a href='{1}'>详细</a><hr>", item.Title, item.Url);
                sb.AppendLine();
            }
            sb.AppendFormat("抓取时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return sb.ToString();
        }

        private async Task<List<DoubanRentSummary>> GetList(string targetUrl)
        {
            List<DoubanRentSummary> items = new List<DoubanRentSummary>();

            HtmlDocument doc = new HtmlWeb().Load(targetUrl);
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
                    var title = titleNode?.Attributes["title"]?.Value.TrimEnd('/');
                    if (_keys.Any(p => title.Contains(p)))
                    {
                        var author = tds[1].InnerText.Replace("\\", "");
                        var url = titleNode?.Attributes["href"]?.Value.TrimEnd('/');
                        var id = Convert.ToInt32(url.Substring(url.LastIndexOf('/') + 1));
                        var record = await _rentSummaryRepository.QueryAsync(id);
                        if (record == null)
                        {
                            var rentSummary = new DoubanRentSummary()
                            {
                                Id = id,
                                Title = title,
                                Url = url,
                                Author = author,
                            };
                            items.Add(rentSummary);
                            await _rentSummaryRepository.InsertAsync(rentSummary);
                        }
                    }
                }
            }

            return items;
        }
    }
}
