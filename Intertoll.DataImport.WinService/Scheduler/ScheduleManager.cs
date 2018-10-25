using Quartz;

namespace Intertoll.DataImport.WinService.Scheduler
{
    internal class ScheduleManager
    {
        private IScheduler Scheduler { get; set; }

        public ScheduleManager(IScheduler _scheduler)
        {
            Scheduler = _scheduler;
        }

        public void RegisterJobs()
        {
            var transactionsJob = new TransactionsJob.TransactionsJob(null, null, null, null);
            transactionsJob.Schedule(Scheduler);

            var incidentsJob = new IncidentsJob.IncidentsJob(null, null);
            incidentsJob.Schedule(Scheduler);

            var sessionsJob = new SessionsJob.SessionsJob(null, null);
            sessionsJob.Schedule(Scheduler);

            var hourlyJob = new HourlyJob.HourlyJob(null, null);
            hourlyJob.Schedule(Scheduler);

            var registeredAccJob = new RegisteredAccountsJob.RegisteredUsersJob(null, null, null);
            registeredAccJob.Schedule(Scheduler);

            var balUpdateJob = new AccountBalanceUpdateJob.AccountBalanceUpdateJob(null, null);
            balUpdateJob.Schedule(Scheduler);

            //var wdJob = new WatchdogJob.WatchdogJob(null, null);
            //wdJob.Schedule(Scheduler);
        }
    }
}
