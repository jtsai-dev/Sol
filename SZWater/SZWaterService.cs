using log4net;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Topshelf;

namespace SZWater
{
    public class SZWaterService : ServiceControl
    {
        private static ILog _logger;
        public SZWaterService()
        {
            _logger = LogManager.GetLogger(typeof(SZWaterService));
        }

        public bool Start(HostControl hostControl)
        {
            _logger.Info("Service has been started");
            try
            {
                //_logger.Info("Loading notices in starting up...");
                new NoticeUtilities().GetNotice();
                //_logger.Info("Ended loading");

                InitSchedulerJob();
            }
            catch (Exception ex)
            {
                _logger.Error("Service has been crash: ", ex);
            }
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _logger.Info("Service has been stopped");
            return true;
        }


        private async void InitSchedulerJob()
        {
            var _scheduler = await new StdSchedulerFactory().GetScheduler();

            IJobDetail job = JobBuilder.Create<SyncNoticesJob>()
                .WithIdentity("job1", "syncGroup")
                .Build();

            var tirggers = new List<ITrigger>() {
                GenerateEveryDayTrigger(7, 30),
                GenerateEveryDayTrigger(18, 20),
            };
            await _scheduler.ScheduleJob(
                job,
                new ReadOnlyCollection<ITrigger>(tirggers),
                false);

            await _scheduler.Start();
        }

        private ITrigger GenerateEveryDayTrigger(int hour, int min)
        {
            //创建并定义触发器的规则（每天执行一次时间为：时：分）
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity($"SyncNoticesTrigger_{hour}_{min}", "syncGroup")
                .StartNow()
                .WithDailyTimeIntervalSchedule(x => x
                    .WithIntervalInHours(24).OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, min)))
                .Build();

            return trigger;
        }

        //private ITrigger GenerateTestTrigger(int interval)
        //{
        //    ITrigger trigger = TriggerBuilder.Create()
        //        .WithIdentity($"SyncNoticesTrigger_{interval}")
        //        .StartNow()
        //        .WithSimpleSchedule(x => x
        //            .WithIntervalInSeconds(interval)
        //            .RepeatForever())
        //        .Build();

        //    return trigger;
        //}
    }

    public class SyncNoticesJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Factory.StartNew(() =>
            {
                new NoticeUtilities().GetNotice();
            });
        }
    }
}
