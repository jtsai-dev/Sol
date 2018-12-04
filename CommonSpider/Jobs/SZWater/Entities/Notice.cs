using System;

namespace CommonSpider.Jobs.SZWater.Entities
{
    public class Notice
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
    }
}
