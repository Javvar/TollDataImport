using System;
using Intertoll.DataImport.Schedulable;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Quartz;
using Unity;
using Unity.Resolution;

namespace Intertoll.DataImport.HourlyJob
{
    [DisallowConcurrentExecution]
    public class HourlyJob : BaseSchedulable<HourlyJob>
    {
        ITollDataProvider DataProvider;
        IUnityContainer Container;

        public HourlyJob(IUnityContainer _container, ITollDataProvider _dataProvider)
        {
            Container = _container;
            DataProvider = _dataProvider;
        }

        public override void Execute(IJobExecutionContext context)
        {
            Log.LogTrace("[Enter]" + JobName);

            try
            {
                var auditsBatch = DataProvider.GetNextHourlyAuditBatch(null);

                Log.LogTrace("Audit batch count: " + auditsBatch.Count);

                foreach (var hourlyAudit in auditsBatch)
                {
                    var submitter = Container.Resolve<IHourlyAuditSubmitter>(new ParameterOverride("_dataProvider", DataProvider));
                    submitter.Submit(hourlyAudit);
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
