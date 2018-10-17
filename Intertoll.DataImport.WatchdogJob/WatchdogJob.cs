using Intertoll.DataImport.Schedulable;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Quartz;
using Unity;

namespace Intertoll.DataImport.WatchdogJob
{
    [DisallowConcurrentExecution]
    public class WatchdogJob : BaseSchedulable<WatchdogJob>
    {
        ITollDataProvider DataProvider;
        IUnityContainer Container;

        public WatchdogJob(IUnityContainer _container, ITollDataProvider _dataProvider)
        {
            Container = _container;
            DataProvider = _dataProvider;
        }

        public override void Execute(IJobExecutionContext context)
        {
            Log.LogTrace("[Enter]" + JobName);

            foreach (var status in DataProvider.GetLaneStatuses())
            {
                if (status.Alive)
                {
                    var submitter = Container.Resolve<ILaneAliveSubmitter>();
                    submitter.Submit(status.LaneGuid);
                }
            }

            Log.LogTrace("[Exit]" + JobName);
        }
    }
}
