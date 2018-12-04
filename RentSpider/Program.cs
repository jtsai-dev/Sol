using Quartz;
using Quartz.Impl;
using RentSpider.Jobs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using Topshelf;

namespace RentSpider
{
    class Program
    {
        private static int _intervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings.Get("IntervalInMinutes"));
        static void Main(string[] args)
        {
            HostFactory.Run(host =>
            {
                host.SetServiceName("RentSummary");
                host.SetDisplayName("RentSummary");
                host.SetDescription("自动抓取深圳宝安租房信息");

                host.Service<RentSummaryService>();
                host.StartAutomaticallyDelayed();
            });
        }

        public class RentSummaryService : ServiceControl
        {
            public bool Start(HostControl hostControl)
            {
                try
                {
                    InitSchedulerJob();
                }
                catch (Exception ex)
                {

                }
                return true;
            }

            public bool Stop(HostControl hostControl)
            {
                return true;
            }

            private async void InitSchedulerJob()
            {
                var _scheduler = await new StdSchedulerFactory().GetScheduler();

                IJobDetail job = JobBuilder.Create<SyncRentSummaryJob>()
                    .WithIdentity("job1", "syncGroup")
                    .Build();

                var tirggers = new List<ITrigger>() {
                GenerateEveryMinTrigger(_intervalInMinutes),
            };
                await _scheduler.ScheduleJob(
                    job,
                    new ReadOnlyCollection<ITrigger>(tirggers),
                    false);

                await _scheduler.Start();
            }
            private ITrigger GenerateEveryMinTrigger(int min)
            {
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"SyncNoticesTrigger_{min}", "syncGroup")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(min)
                        .RepeatForever())
                    .Build();

                return trigger;
            }
        }
    }
}
