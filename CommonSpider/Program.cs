using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Topshelf;
using System.Linq;
using Microsoft.Extensions.Logging;
using log4net;
using CommonSpider.Common.Sender;
using System;

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
                if (jobs.Any(p => p.InDebug))
                {
                    jobs = jobs.Where(p => p.InDebug).ToList();
                }
                host.SetDescription($"CommonSpider with: {string.Join(", ", jobs.Select(p => p.Name + "-" + p.Description))}");

                host.Service(() => { return new TopService(jobs); });
                host.OnException(exception =>
                {
                    ExceptionHander(exception);
                });

                host.StartAutomaticallyDelayed();
            });
        }

        private static void ExceptionHander(Exception exception)
        {
            var logger = LogManager.GetLogger(typeof(Program));
            logger.Error("CommonSpider has been stop cause by the exception：" + exception.Message, exception);
            FtqqSender.Send("CommonSpider异常停止: ", exception.Message);
        }
    }
}
