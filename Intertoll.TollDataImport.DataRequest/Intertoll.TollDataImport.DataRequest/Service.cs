using Intertoll.NLogger;
using System;
using System.ServiceModel;
using System.ServiceProcess;
using Intertoll.TollDataImport.DataRequest.Scheduler;

namespace Intertoll.TollDataImport.DataRequest
{
    public partial class Service : ServiceBase
    {
        private ServiceHost serviceHost;

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.LogInfoMessage("Starting service");
            SetupWCFService();

            try
            {
                DataRequestJob FirstExecution = new DataRequestJob();
                FirstExecution.Execute(null);
                DataRequestScheduler.ScheduleDataRequest();
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        protected override void OnStop()
        {
            Log.LogInfoMessage("Stopping service");

            try
            {
                DataRequestScheduler.ClearScheduler();
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }

            if (serviceHost != null)
                serviceHost.Close();
        }

        public void ConsoleStart()
        {
            OnStart(new string[] { });
            Console.WriteLine("Started service");
        }

        public void ConsoleStop()
        {
        }

        private void SetupWCFService()
        {
            try
            {
                if (serviceHost != null)
                    serviceHost.Close();

                serviceHost = new ServiceHost(typeof(TollDataImportDataRequestService));
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
    }
}
