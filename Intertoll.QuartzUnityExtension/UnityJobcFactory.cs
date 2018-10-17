using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Core;
using Quartz.Impl;
using Quartz.Spi;
using Unity;
using Unity.Extension;

namespace Intertoll.QuartzUnityExtension
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

    public class QuartzUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            this.Container.RegisterType<IJobFactory, UnityJobFactory>();

            this.Container.RegisterType<ISchedulerFactory, UnitySchedulerFactory>();
        }
    }
}
