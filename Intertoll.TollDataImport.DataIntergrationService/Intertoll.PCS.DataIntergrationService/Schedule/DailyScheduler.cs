using Intertoll.PCS.DataIntergrationService.Managers;
using Quartz;
using Quartz.Impl;
using System;

namespace Intertoll.PCS.DataIntergrationService.Schedule
{
    internal class DailyScheduler
    {
        public static void ScheduleUpdateCommsTasks()
        {
            var schedFact = new StdSchedulerFactory();
            var Scheduler = schedFact.GetScheduler();
            Scheduler.Start();

            string JobName = "UpdateCommsJob";
            string GroupName = "UpdateCommsGroup";

            if (!Scheduler.CheckExists(new JobKey(JobName, GroupName)))
            {
                var objJob = JobBuilder.Create<UpdateCommsJob>().WithIdentity(JobName).RequestRecovery(true).Build();

                var Trigger = TriggerBuilder.Create()
                                .WithIdentity("UpdateCommsTrigger")
                                .StartAt(DateTime.Now.AddSeconds(1)) // if a start time is not given (if this line were omitted), "now" is implied
                                .WithSimpleSchedule(x => x
                                    .WithIntervalInSeconds(AppSettingsManager.CommsUpdateInterval)
                                    .RepeatForever())
                                .ForJob(JobName) // identify job with handle to its JobDetail itself                   
                                .Build();

                Scheduler.ScheduleJob(objJob, Trigger);
            }
        }

        public static void ScheduleSendAliveMessageTasks()
        {
            var schedFact = new StdSchedulerFactory();
            var Scheduler = schedFact.GetScheduler();
            Scheduler.Start();

            string JobName = "SendAliveMessageJob";
            string GroupName = "SendAliveMessageGroup";

            if (!Scheduler.CheckExists(new JobKey(JobName, GroupName)))
            {
                var objJob = JobBuilder.Create<SendAliveMessageJob>().WithIdentity(JobName).RequestRecovery(true).Build();

                var Trigger = TriggerBuilder.Create()
                                .WithIdentity("SendAliveMessageTrigger")
                                .StartAt(DateTime.Now.AddSeconds(5)) // if a start time is not given (if this line were omitted), "now" is implied
                                .WithSimpleSchedule(x => x
                                    .WithIntervalInSeconds(AppSettingsManager.SendAliveMessageInterval)
                                    .RepeatForever())
                                .ForJob(JobName) // identify job with handle to its JobDetail itself                   
                                .Build();

                Scheduler.ScheduleJob(objJob, Trigger);
            }
        }
            
        public static void ScheduleTransactionsAndIncidentsTasks()
        {
            var schedFact = new StdSchedulerFactory();
            var Scheduler = schedFact.GetScheduler();
            Scheduler.Start();

            string JobName = "ProcessTransactionsAndIncidentsJob";
            string GroupName = "ProcessTransactionsAndIncidentsJroup";

            if (!Scheduler.CheckExists(new JobKey(JobName, GroupName)))
            {
                var objJob = JobBuilder.Create<ProcessTransactionsAndIncidentsJob>().WithIdentity(JobName).RequestRecovery(true).Build();

                var Trigger = TriggerBuilder.Create()
                                .WithIdentity("ProcessTransactionsAndIncidentsTrigger")
                                //.StartAt(AppSettingsManager.GetTAStatementTime) // if a start time is not given (if this line were omitted), "now" is implied
                                .WithSimpleSchedule(x => x
                                    .WithIntervalInMinutes(AppSettingsManager.GetProcessTransactionsAndIncidentsInterval)
                                    .RepeatForever())
                                .ForJob(JobName) // identify job with handle to its JobDetail itself                   
                                .Build();

                Scheduler.ScheduleJob(objJob, Trigger);
            }
        }

        public static void ScheduleHourlyAuditJobTasks()
        {
            var schedFact = new StdSchedulerFactory();
            var Scheduler = schedFact.GetScheduler();
            Scheduler.Start();

            string JobName = "ProcessHourlyAuditJob";
            string GroupName = "ProcessHourlyAuditJobGroup";

            if (!Scheduler.CheckExists(new JobKey(JobName, GroupName)))
            {
                var objJob = JobBuilder.Create<ProcessHourlyAuditJob>().WithIdentity(JobName).RequestRecovery(true).Build();

                var Trigger = TriggerBuilder.Create()
                                .WithIdentity("ProcessHourlyAuditTrigger")
                                .StartAt(AppSettingsManager.HourlyAuditTime) // if a start time is not given (if this line were omitted), "now" is implied
                                .WithSimpleSchedule(x => x
                                    .WithIntervalInMinutes(AppSettingsManager.GetHourlyAuditInterval)
                                    .RepeatForever())
                                .ForJob(JobName) // identify job with handle to its JobDetail itself                   
                                .Build();

                Scheduler.ScheduleJob(objJob, Trigger);
            }
        }

        public static void ScheduleEndOfDayTask()
        {
            var schedFact = new StdSchedulerFactory();
            var Scheduler = schedFact.GetScheduler();
            Scheduler.Start();

            string JobName = "ProcessEndOfDayTaskJob";
            string GroupName = "ProcessEndOfDayTaskJobGroup";

            if (!Scheduler.CheckExists(new JobKey(JobName, GroupName)))
            {
                var objJob = JobBuilder.Create<ProcessEndOfDayTaskJob>().WithIdentity(JobName).RequestRecovery(true).Build();

                var Trigger = TriggerBuilder.Create()
                                .WithIdentity("ProcessEndOfDayTaskTrigger")
                                .StartAt(AppSettingsManager.GetEndOfDayTaskTime) // if a start time is not given (if this line were omitted), "now" is implied
                                .WithSimpleSchedule(x => x
                                    .WithIntervalInHours(24)
                                    .RepeatForever())
                                .ForJob(JobName) // identify job with handle to its JobDetail itself                   
                                .Build();

                Scheduler.ScheduleJob(objJob, Trigger);
            }
        }
    }
}
