using Quartz;
using Quartz.Spi;
using Unity;

namespace Intertoll.DataImport.WinService.Scheduler
{
    public class UnityJobFactory : IJobFactory
    {
        private readonly IUnityContainer _container;

        public UnityJobFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _container.Resolve(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
        }
    }

}
