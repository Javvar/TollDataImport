using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Intertoll.NLogger;

namespace Intertoll.PCS.DataIntergrationService
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
                DataIntergrationServiceHost service = new DataIntergrationServiceHost();
                service.ConsoleStart();
                Console.ReadLine();

                service.ConsoleStop();
            }
            else
            {
                ServiceBase.Run(new DataIntergrationServiceHost());
            }
        }
    }
}
