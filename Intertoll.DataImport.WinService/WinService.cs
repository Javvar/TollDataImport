using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceProcess;
using Intertoll.DataImport.DataRequest;
using Intertoll.DataImport.WinService.Scheduler;
using Intertoll.DataImport.WinService.Unity;
using Intertoll.NLogger;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Impl;
using Unity;



namespace Intertoll.DataImport.WinService
{
    public partial class WinService : ServiceBase
    {
        private ServiceHost ServHost;

        public WinService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.LogInfoMessage("[Enter] Starting Service");

	        try
	        {
		        ContainerManager.Initialise();
				SetupRequestService();
		        ContainerManager.Container.Resolve<ScheduleManager>().RegisterJobs();
	        }
	        catch (Exception ex)
	        {
				Log.LogException(ex);
		        Log.LogTrace(ex.Message + ". Check error log for more details.");

				throw;
	        }

			Log.LogInfoMessage("[Exit] Starting Service");
        }

        protected override void OnStop()
        {
            Log.LogInfoMessage("[Enter] Stopping Service");


            Log.LogInfoMessage("[Exit] Stopping Service");
        }

        #region Data Request Service

        void SetupRequestService()
        {
            try
            {
				if(ServHost != null)
					ServHost.Close();

				ServHost = new ServiceHost(ContainerManager.Container.Resolve<ITollDataRequest>());
                ServHost.Faulted += ServiceHost_Faulted;
                ServHost.Open();
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }
        }

        void ServiceHost_Faulted(object sender, EventArgs e)
        {
            Log.LogFatal(new Exception("ServiceHost_Faulted"), "ServiceHost_Faulted");
            ServHost.Abort();
            SetupRequestService();
        }

        #endregion

        #region Debug

        internal void DebugStart()
        {
            OnStart(new string[] { });
        }

        internal void DebugStop()
        {
            OnStop();
        }

        #endregion
    }
}
