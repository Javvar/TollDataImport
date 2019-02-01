using System;
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
			if (System.Environment.UserInteractive)
			{
				WinService service = new WinService();
				service.ConsoleStart();
				Console.ReadLine();
				service.ConsoleStop();
			}
			else
			{
				ServiceBase.Run(new WinService());
			}
		}
	}
}
