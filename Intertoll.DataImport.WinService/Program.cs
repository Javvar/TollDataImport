using System;
using System.ServiceProcess;

namespace Intertoll.DataImport.WinService
{
    static class Program
    {
        static void Main()
        {
            #if DEBUG

            WinService svc = new WinService();
            svc.DebugStart();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            #else

            ServiceBase[] ServicesToRun = {new WinService()};
                ServiceBase.Run(ServicesToRun);

            #endif
        }
    }
}
