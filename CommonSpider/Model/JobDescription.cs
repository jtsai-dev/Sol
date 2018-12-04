using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSpider.Model
{
    class JobDescription
    {
        public string Name { get; set; }
        public string Description{get;set;}
        public string TargetUrl { get; set; }
        public string Group { get; set; }
        public string Service { get; set; }
        public List<string> Triggers { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
