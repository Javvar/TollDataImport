using Intertoll.DataImport.DataRequest;
using Intertoll.DataImport.HourlyJob;
using Intertoll.DataImport.IncidentsJob;
using Intertoll.DataImport.Mail;
using Intertoll.DataImport.Settings;
using Intertoll.DataImport.SyncClient;
using Intertoll.DataImport.TransactionsJob;
using Intertoll.DataImport.WinService.Scheduler;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Intertoll.Toll.DataImport.Interfaces.EntityBuilders;
using Intertoll.DataImport.Data.DataContext;
using Quartz;
using Unity;
using Intertoll.DataImport.WinService.DataRequest;

namespace Intertoll.DataImport.WinService.Unity
{
    public class ContainerManager
    {
        private static IUnityContainer _container;

        public static IUnityContainer Container
        {
            get { return _container; }
        }

        static ContainerManager()
        {
            _container = new UnityContainer();
            _container.AddNewExtension<QuartzUnityExtension>();
        }

        static public void Initialise()
        {
            _container.RegisterType<IScheduler, GlobalScheduler>();
            _container.RegisterType<ITollDataProvider, TollDataContext>();
            _container.RegisterType<ITransactionSubmitter, SyncTransactionSubmitter>();
            _container.RegisterType<IETCTransactionSubmitter, SyncETCTransactionSubmitter>();
            _container.RegisterType<IIncidentSubmitter, SyncIncidentSubmitter>();
            _container.RegisterType<ISessionSubmitter, SyncSessionSubmitter>();
            _container.RegisterType<IStaffLoginSubmitter, SyncStaffLoginSubmitter>();
            _container.RegisterType<IHourlyAuditSubmitter, SyncHourlyAuditSubmitter>();
            _container.RegisterType<ILaneAliveSubmitter, SyncLaneAliveSubmitter>();
            _container.RegisterType<ITransactionEntityBuilder, TransactionEntityBuilder>();
            _container.RegisterType<ITransactionCardDetailBuilder, TransactionCardDetailBuilder>();
            _container.RegisterType<IETCTransactionEntityBuilder, ETCTransactionEntityBuilder>();
            _container.RegisterType<IIncidentEntityBuilder, IncidentEntityBuilder>();
            _container.RegisterType<IHourlyAuditEntityBuilder, HourlyAuditBuilder>();
            _container.RegisterType<ScheduleManager, ScheduleManager>();
            _container.RegisterType<ISettingsProvider, TollDataImportSettings>();
            _container.RegisterType<IMailClient, MailClient>();
            _container.RegisterType<INewStaffMailFormatter, NewStaffMailFormatter>();
            _container.RegisterType<IDuplicateTransactionMailFormatter, DuplicateTransactionMailFormatter>();
            _container.RegisterType<ITollDataRequest, TollDataRequest>();
        }
    }
}
