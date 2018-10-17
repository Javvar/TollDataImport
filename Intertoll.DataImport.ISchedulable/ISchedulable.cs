using System;
using Quartz;

namespace Intertoll.DataImport.Schedulable
{
    public interface ISchedulable : IJob
    {
        string CronSchedule { get; set; }
        Action<IScheduler> Schedule { get; set; }
    }
}
