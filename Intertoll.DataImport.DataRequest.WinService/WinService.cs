using Intertoll.NLogger;
using System;
using System.ServiceModel;
using System.ServiceProcess;

namespace Intertoll.DataImport.DataRequest.WinService
{
	public partial class WinService : ServiceBase
	{
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
		}

		#region Debug

		public void ConsoleStart()
		{
			OnStart(new string[] { });
			Console.WriteLine("Sync Started service");
		}

		public void ConsoleStop()
		{
		}

		#endregion
	}
}
