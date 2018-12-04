using CommonSpider.Jobs;
using log4net;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using Topshelf;

namespace CommonSpider
{
    class TopService : ServiceControl
    {
        private static ILog _logger;
        private static List<Model.JobDescription> _jobs { get; set; }
        public TopService(List<Model.JobDescription> jobs)
        {
            _logger = LogManager.GetLogger(typeof(TopService));
            _jobs = jobs;
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                _logger.Info("service start");
                InitSchedulerJob();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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

            var triggersAndJobs = QueryJobs();
            await _scheduler.ScheduleJobs(triggersAndJobs, false);

            await _scheduler.Start();
        }

        private IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> QueryJobs()
        {
            var result = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>();
            _jobs.ForEach(p =>
            {
                var dataMap = new JobDataMap();
                dataMap.Add("TargetUrl", p.TargetUrl);
                dataMap.Add("Service", p.Service);
                dataMap.Add("Data", p.Data);

                IJobDetail job = JobBuilder.Create<BaseJob>()
                                .WithIdentity(p.Name, p.Group)
                                .UsingJobData(dataMap)
                                .Build();

                var triggers = new List<ITrigger>();
                p.Triggers.ForEach(t =>
                {
                    triggers.Add(GenerateTrigger(t, p.Name, p.Group));
                });
                result.TryAdd(job, triggers);
            });
            return result;
        }

        private ITrigger GenerateTrigger(string cronStr, string name, string group)
        {
            ITrigger trigger = TriggerBuilder.Create()
                   .WithIdentity($"trigger_{name}_{cronStr}", group)
                   .StartNow()
                   .WithCronSchedule(cronStr)
                   .Build();

            return trigger;
        }
    }
}
