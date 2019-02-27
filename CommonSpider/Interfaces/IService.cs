using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommonSpider.Interfaces
{
    interface IService
    {
        Task ExcuteAsync(string name, string targetUrl, Dictionary<string, object> data);
    }
}
