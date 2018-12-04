using System;
using System.Collections.Generic;
using System.Text;

namespace CommonSpider.Interfaces
{
    interface IService
    {
        void Init(string targetUrl, Dictionary<string, object> data);
    }
}
