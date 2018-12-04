using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Topshelf;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace CommonSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            new LoggerFactory().AddLog4Net();

            HostFactory.Run(host =>
            {
                host.SetServiceName("CommonSpider");
                host.SetDisplayName("CommonSpider");
                var jobs = JsonConvert.DeserializeObject<List<Model.JobDescription>>(File.ReadAllText(@"Configs/Jobs.json"));
                host.SetDescription($"CommonSpider with: {string.Join(", ", jobs.Select(p => p.Name + "-" + p.Description))}");

                host.Service(() => { return new TopService(jobs); });
                host.StartAutomaticallyDelayed();
            });
        }
    }
}
