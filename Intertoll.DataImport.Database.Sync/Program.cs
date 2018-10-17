using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Intertoll.DataImport.Database.Sync
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
