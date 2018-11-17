using Topshelf;
using System.IO;
using System;

namespace SZWater
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
            HostFactory.Run(host =>
            {
                host.SetServiceName("SZWater");
                host.SetDisplayName("SZWater");
                host.SetDescription("自动抓取深圳水务集团停水通知");

                host.Service<SZWaterService>();
                host.StartAutomaticallyDelayed();
            });
        }
    }
}
