using System;
using System.Configuration;
using System.Messaging;
using System.ServiceModel;
using System.ServiceProcess;
using Intertoll.NLogger;
using Intertoll.PCS.DataIntergrationService.Schedule;
using Quartz;
using Quartz.Impl;
using Intertoll.PCS.DataIntergrationService.Managers;

namespace Intertoll.PCS.DataIntergrationService
{
    public partial class DataIntergrationServiceHost : ServiceBase
    {
        private ServiceHost serviceHost;

        public DataIntergrationServiceHost()
        {
            InitializeComponent();
        }

        public void ConsoleStart()
        {
            OnStart(new string[] { });
            Console.WriteLine("Started service");
        }

        public void ConsoleStop()
        {
        }
        protected override void OnStart(string[] args)
        {
            Console.WriteLine("PCS Data Intergration Service is starting");
            Log.LogTrace("PCS Data Intergration Service is starting");
            ServiceAgentManager.SendReport("PCS Data Intergration Service is starting", 3);

             //WCF Service
            Console.WriteLine("Setup WCF Service - Start");
            SetupWCFService();

            Console.WriteLine("Setup WCF Service - End");
            DailyScheduler.ScheduleUpdateCommsTasks();
            //DailyScheduler.ScheduleSendAliveMessageTasks();
            DailyScheduler.ScheduleTransactionsAndIncidentsTasks();
            DailyScheduler.ScheduleHourlyAuditJobTasks();
            DailyScheduler.ScheduleEndOfDayTask();
        }

        protected override void OnStop()
        {
            Log.LogInfoMessage("PCS Data Intergration service is stopping");
            ServiceAgentManager.SendReport("PCS Data Intergration service is stopping", 3);
        }
        
		private void SetupWCFService()
		{
			try
			{
				// Create queue if it doesn't exist
                if (!MessageQueue.Exists(AppSettingsManager.DataAggregationQueueName))
                {
                    Console.WriteLine("queue  doesn't exist");
                    var messageQueue = MessageQueue.Create(AppSettingsManager.DataAggregationQueueName, true);
                    messageQueue.SetPermissions("Administrators", MessageQueueAccessRights.FullControl);
                    Console.WriteLine("queue  doesn't exist - created");
                }

				if (serviceHost != null)
					serviceHost.Close();

                serviceHost = new ServiceHost(typeof(IntergrationService));
                serviceHost.Faulted += ServiceHost_Faulted;
                serviceHost.Open();
                Console.WriteLine("serviceHost Opened");
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex);
                Console.ReadLine();
                Log.LogException(ex);
                Console.WriteLine(ex.Message);
				throw;
			}
		}

        void ServiceHost_Faulted(object sender, EventArgs e)
        {
            Console.WriteLine("ServiceHost_Faulted");
            Log.LogFatal(new Exception("ServiceHost_Faulted"), "ServiceHost_Faulted");
            serviceHost.Abort();
            SetupWCFService();
        }

        private void Restart()
        {
            serviceHost.Abort();
            SetupWCFService();
        }
        #region Debugging

        internal void DebugStart()
        {
            OnStart(null);
        }

        #endregion
    }
}
