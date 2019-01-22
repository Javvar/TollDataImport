using System;
using System.ServiceModel;
using System.ServiceProcess;

namespace Intertoll.DataImport.DataRequest.WinService
{
	public partial class WinService : ServiceBase
	{
		ServiceHost serviceHost;

		public WinService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				DataRequestJob FirstExecution = new DataRequestJob();
				FirstExecution.Execute(null);
				DataRequestScheduler.ScheduleDataRequest();
			}
			catch (Exception ex)
			{
				//Log.LogException(ex);
			}
		}

		protected override void OnStop()
		{
			//Log.LogInfoMessage("Stopping service");

			try
			{
				DataRequestScheduler.ClearScheduler();
			}
			catch (Exception ex)
			{
				//Log.LogException(ex);
			}

			if (serviceHost != null)
				serviceHost.Close();
		}

		void SetupWCFService()
		{
			try
			{
				if (serviceHost != null)
					serviceHost.Close();

				serviceHost = new ServiceHost(typeof(TollDataRequest));
				serviceHost.Faulted += ServiceHost_Faulted;
				serviceHost.Open();
				Console.WriteLine("serviceHost Opened");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.ReadLine();
				//Log.LogException(ex);
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		void ServiceHost_Faulted(object sender, EventArgs e)
		{
			Console.WriteLine("ServiceHost_Faulted");
			//Log.LogFatal(new Exception("ServiceHost_Faulted"), "ServiceHost_Faulted");
			serviceHost.Abort();
			SetupWCFService();
		}

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
