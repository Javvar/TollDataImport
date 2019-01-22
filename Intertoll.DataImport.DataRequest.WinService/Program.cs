using System.ServiceProcess;

namespace Intertoll.DataImport.DataRequest.WinService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			#if DEBUG

            WinService svc = new WinService();
            svc.DebugStart();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

			#else

			ServiceBase[] ServicesToRun = { new WinService() };
			ServiceBase.Run(ServicesToRun);

			#endif
		}
	}
}
