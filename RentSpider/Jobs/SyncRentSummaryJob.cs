using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RentSpider.Jobs
{
    public class SyncRentSummaryJob : IJob
    {
        Task IJob.Execute(IJobExecutionContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                new Services.RentSummaryService().GetNotice();
            });
        }
    }
}
