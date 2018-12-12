using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSpider.Model
{
    public class EmailConfig
    {
        public string Host { get; set; }
        public string UserName{get;set;}
        public string Password { get; set; }
        public string From { get; set; }
        public string[] To { get; set; }
    }
}
