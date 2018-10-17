using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intertoll.DataImport.WinService.Scheduler;
using Quartz;
using Quartz.Spi;
using Unity;
using Unity.Extension;

namespace Intertoll.DataImport.WinService.Unity
{
    public class QuartzUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<IJobFactory, UnityJobFactory>();
            Container.RegisterType<ISchedulerFactory, UnitySchedulerFactory>();
        }
    }
}
