using System;
using System.ServiceProcess;

namespace Intertoll.TollDataImport.DataRequest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                Service service = new Service();
                service.ConsoleStart();
                Console.ReadLine();

                service.ConsoleStop();
            }
            else
            {
                ServiceBase.Run(new Service());
            }
        }
    }
}
