using System;
using System.Collections.Generic;
using Intertoll.DataImport.WinService.Unity;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using Unity;

namespace Intertoll.DataImport.WinService.Scheduler
{
    internal class GlobalScheduler : IScheduler
    {
        private static IScheduler _instance;

        static GlobalScheduler()
        {
            _instance = ContainerManager.Container.Resolve<ISchedulerFactory>().GetScheduler();

            //ISchedulerFactory schedFact = new StdSchedulerFactory();
            //_instance = schedFact.GetScheduler();
            _instance.Start();
        }

        #region IScheduler

        public bool IsJobGroupPaused(string groupName)
        {
            return _instance.IsJobGroupPaused(groupName);
        }

        public bool IsTriggerGroupPaused(string groupName)
        {
            return _instance.IsTriggerGroupPaused(groupName);
        }

        public SchedulerMetaData GetMetaData()
        {
            return _instance.GetMetaData();
        }

        public IList<IJobExecutionContext> GetCurrentlyExecutingJobs()
        {
            return _instance.GetCurrentlyExecutingJobs();
        }

        public IList<string> GetJobGroupNames()
        {
            return _instance.GetJobGroupNames();
        }

        public IList<string> GetTriggerGroupNames()
        {
            return _instance.GetTriggerGroupNames();
        }

        public Quartz.Collection.ISet<string> GetPausedTriggerGroups()
        {
            return _instance.GetPausedTriggerGroups();
        }

        public void Start()
        {
            _instance.Start();
        }

        public void StartDelayed(TimeSpan delay)
        {
            _instance.StartDelayed(delay);
        }

        public void Standby()
        {
            _instance.Standby();
        }

        public void Shutdown()
        {
            _instance.Shutdown();
        }

        public void Shutdown(bool waitForJobsToComplete)
        {
            _instance.Shutdown(waitForJobsToComplete);
        }

        public DateTimeOffset ScheduleJob(IJobDetail jobDetail, ITrigger trigger)
        {
            return _instance.ScheduleJob(jobDetail, trigger);
        }

        public DateTimeOffset ScheduleJob(ITrigger trigger)
        {
            return _instance.ScheduleJob(trigger);
        }

        public void ScheduleJobs(IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> triggersAndJobs, bool replace)
        {
            _instance.ScheduleJobs(triggersAndJobs, replace);
        }

        public void ScheduleJob(IJobDetail jobDetail, Quartz.Collection.ISet<ITrigger> triggersForJob, bool replace)
        {
            _instance.ScheduleJob(jobDetail, triggersForJob, replace);
        }

        public bool UnscheduleJob(TriggerKey triggerKey)
        {
            return _instance.UnscheduleJob(triggerKey);
        }

        public bool UnscheduleJobs(IList<TriggerKey> triggerKeys)
        {
            return _instance.UnscheduleJobs(triggerKeys);
        }

        public DateTimeOffset? RescheduleJob(TriggerKey triggerKey, ITrigger newTrigger)
        {
            return _instance.RescheduleJob(triggerKey, newTrigger);
        }

        public void AddJob(IJobDetail jobDetail, bool replace)
        {
            _instance.AddJob(jobDetail, replace);
        }

        public void AddJob(IJobDetail jobDetail, bool replace, bool storeNonDurableWhileAwaitingScheduling)
        {
            _instance.AddJob(jobDetail, replace, storeNonDurableWhileAwaitingScheduling);
        }

        public bool DeleteJob(JobKey jobKey)
        {
            return _instance.DeleteJob(jobKey);
        }

        public bool DeleteJobs(IList<JobKey> jobKeys)
        {
            return _instance.DeleteJobs(jobKeys);
        }

        public void TriggerJob(JobKey jobKey)
        {
            _instance.TriggerJob(jobKey);
        }

        public void TriggerJob(JobKey jobKey, JobDataMap data)
        {
            _instance.TriggerJob(jobKey, data);
        }

        public void PauseJob(JobKey jobKey)
        {
            _instance.PauseJob(jobKey);
        }

        public void PauseJobs(GroupMatcher<JobKey> matcher)
        {
            _instance.PauseJobs(matcher);
        }

        public void PauseTrigger(TriggerKey triggerKey)
        {
            _instance.PauseTrigger(triggerKey);
        }

        public void PauseTriggers(GroupMatcher<TriggerKey> matcher)
        {
            _instance.PauseTriggers(matcher);
        }

        public void ResumeJob(JobKey jobKey)
        {
            _instance.ResumeJob(jobKey);
        }

        public void ResumeJobs(GroupMatcher<JobKey> matcher)
        {
            _instance.ResumeJobs(matcher);
        }

        public void ResumeTrigger(TriggerKey triggerKey)
        {
            _instance.ResumeTrigger(triggerKey);
        }

        public void ResumeTriggers(GroupMatcher<TriggerKey> matcher)
        {
            _instance.ResumeTriggers(matcher);
        }

        public void PauseAll()
        {
            _instance.PauseAll();
        }

        public void ResumeAll()
        {
            _instance.ResumeAll();
        }

        public Quartz.Collection.ISet<JobKey> GetJobKeys(GroupMatcher<JobKey> matcher)
        {
            return _instance.GetJobKeys(matcher);
        }

        public IList<ITrigger> GetTriggersOfJob(JobKey jobKey)
        {
            return _instance.GetTriggersOfJob(jobKey);
        }

        public Quartz.Collection.ISet<TriggerKey> GetTriggerKeys(GroupMatcher<TriggerKey> matcher)
        {
            return _instance.GetTriggerKeys(matcher);
        }

        public IJobDetail GetJobDetail(JobKey jobKey)
        {
            return _instance.GetJobDetail(jobKey);
        }

        public ITrigger GetTrigger(TriggerKey triggerKey)
        {
            return _instance.GetTrigger(triggerKey);
        }

        public TriggerState GetTriggerState(TriggerKey triggerKey)
        {
            return _instance.GetTriggerState(triggerKey);
        }

        public void AddCalendar(string calName, ICalendar calendar, bool replace, bool updateTriggers)
        {
            _instance.AddCalendar(calName, calendar, replace, updateTriggers);
        }

        public bool DeleteCalendar(string calName)
        {
            return _instance.DeleteCalendar(calName);
        }

        public ICalendar GetCalendar(string calName)
        {
            return _instance.GetCalendar(calName);
        }

        public IList<string> GetCalendarNames()
        {
            return _instance.GetCalendarNames();
        }

        public bool Interrupt(JobKey jobKey)
        {
            return _instance.Interrupt(jobKey);
        }

        public bool Interrupt(string fireInstanceId)
        {
            return _instance.Interrupt(fireInstanceId);
        }

        public bool CheckExists(JobKey jobKey)
        {
            return _instance.CheckExists(jobKey);
        }

        public bool CheckExists(TriggerKey triggerKey)
        {
            return _instance.CheckExists(triggerKey);
        }

        public void Clear()
        {
            _instance.Clear();
        }

        public string SchedulerName
        {
            get { return _instance.SchedulerName; }
        }

        public string SchedulerInstanceId
        {
            get { return _instance.SchedulerInstanceId; }
        }

        public SchedulerContext Context
        {
            get { return _instance.Context; }
        }

        public bool InStandbyMode
        {
            get { return _instance.InStandbyMode; }
        }

        public bool IsShutdown
        {
            get { return _instance.IsShutdown; }
        }

        public IJobFactory JobFactory
        {
            set { _instance.JobFactory = value; }
        }

        public IListenerManager ListenerManager
        {
            get { return _instance.ListenerManager; }
        }

        public bool IsStarted
        {
            get { return _instance.IsStarted; }
        }

        #endregion
    }
}
