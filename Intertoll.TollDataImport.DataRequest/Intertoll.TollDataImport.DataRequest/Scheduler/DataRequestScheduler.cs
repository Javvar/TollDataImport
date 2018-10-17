using System.Configuration;
using Intertoll.NLogger;
using Quartz;
using Quartz.Impl;

namespace Intertoll.TollDataImport.DataRequest.Scheduler
{
    public class DataRequestScheduler 
    {
        public static void ScheduleDataRequest()
        {
            var schedFact = new StdSchedulerFactory();
            var Scheduler = schedFact.GetScheduler();
            Scheduler.Start();

            string JobName = "DataRequestJob";
            string GroupName = "DataRequest";

            if (!Scheduler.CheckExists(new JobKey(JobName, GroupName)))
            {
                var objJob = JobBuilder.Create<DataRequestJob>().WithIdentity(JobName, GroupName).RequestRecovery(true).Build();

                var Trigger = TriggerBuilder.Create().WithIdentity("DataRequestTrigger", "DataRequest")
                                                     .WithDailyTimeIntervalSchedule(s => s.WithIntervalInMinutes(int.Parse(ConfigurationManager.AppSettings["ScheduleIntertval"]))
                                                     .OnEveryDay()
                                                     .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))).Build();

                Scheduler.ScheduleJob(objJob, Trigger);
            }
        }

        public static void ClearScheduler()
        {
            var schedFact = new StdSchedulerFactory();
            var Scheduler = schedFact.GetScheduler();
            Scheduler.Clear();
        }
    }
}
