using System;
using System.Configuration;
using System.Diagnostics;
using Intertoll.NLogger;
using Quartz;

namespace Intertoll.DataImport.Schedulable
{
    public abstract class BaseSchedulable<T> : ISchedulable where T : IJob
    {
        public string CronSchedule { get; set; }
        public Action<IScheduler> Schedule { get; set; }
        public Action<IScheduler, string> ScheduleWithCron { get; set; }

        public string JobName
        {
            get { return GetType().Name; }
        }

        public string JobGroupName
        {
            get { return JobName.Replace("Job", string.Empty); }
        }

        protected BaseSchedulable()
        {
            if (ConfigurationManager.AppSettings[JobName + "CronSchedule"] != null)
            {
                CronSchedule = ConfigurationManager.AppSettings[JobName + "CronSchedule"];
            }

            Schedule = ScheduleJob;
        }

        private void ScheduleJob(IScheduler scheduler)
        {
            if (!scheduler.CheckExists(new JobKey(JobName,JobGroupName)))
            {
                Log.LogTrace("[Enter] Scheduling " + JobName);
                Log.LogDebugMessage(JobName + " schedule: " + (new CronExpression(CronSchedule)).GetExpressionSummary());

                var job = JobBuilder.Create<T>()
                                           .WithIdentity(JobName,JobGroupName)
                                           .RequestRecovery(true)
                                           .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(JobName,JobGroupName)
                    .StartAt(DateTime.Now.AddSeconds(1)) 
                    .ForJob(job)
                    .WithCronSchedule(CronSchedule)
                    .Build();

                scheduler.ScheduleJob(job, trigger);

                Log.LogTrace("[Exit] Scheduling" + JobGroupName);
            }
        }

        public abstract void Execute(IJobExecutionContext context);
    }
}
