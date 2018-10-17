using System;
using System.Linq;
using Intertoll.DataImport.Schedulable;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Quartz;
using Unity;
using Unity.Resolution;

namespace Intertoll.DataImport.SessionsJob
{
    [DisallowConcurrentExecution]
    public class SessionsJob : BaseSchedulable<SessionsJob>
    {
        ITollDataProvider DataProvider;
        IUnityContainer Container;

        public SessionsJob(IUnityContainer _container, ITollDataProvider _dataProvider)
        {
            Container = _container;
            DataProvider = _dataProvider;
        }

        public override void Execute(IJobExecutionContext context)
        {
            Log.LogTrace("[Enter]" + JobName);

            try
            {
                var paramOverride = new ParameterOverride("_dataProvider", DataProvider);

                var unSentStaffLogins = DataProvider.GetUnsentStaffLogins();

                if (unSentStaffLogins.Any())
                    Log.LogInfoMessage("Unsent staff login batch count: " + unSentStaffLogins.Count);

                foreach (var unsent in unSentStaffLogins)
                {
                    var submitter = Container.Resolve<IStaffLoginSubmitter>(paramOverride);
                    submitter.Submit(unsent);
                }

                var unSentSessions = DataProvider.GetUnsentSessions();

                if (unSentSessions.Any())
                    Log.LogInfoMessage("Unsent session batch count: " + unSentSessions.Count);

                foreach (var session in unSentSessions)
                {
                    var submitter = Container.Resolve<ISessionSubmitter>(paramOverride);
                    submitter.Submit(session);
                }

                DataProvider.Save();
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }

            Log.LogTrace("[Exit]" + JobName);
        }
    }
}
