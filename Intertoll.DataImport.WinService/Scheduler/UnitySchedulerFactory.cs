using Quartz;
using Quartz.Core;
using Quartz.Impl;
using Quartz.Spi;

namespace Intertoll.DataImport.WinService.Scheduler
{
    public class UnitySchedulerFactory : StdSchedulerFactory
    {
        private readonly IJobFactory _jobFactory;

        public UnitySchedulerFactory(IJobFactory jobFactory)
        {
            _jobFactory = jobFactory;
        }

        protected override IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
        {
            qs.JobFactory = _jobFactory;

            return base.Instantiate(rsrcs, qs);
        }
    }
}
