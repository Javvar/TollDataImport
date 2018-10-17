using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intertoll.Toll.DataImport.DataRequest;
using Intertoll.Toll.DataImport.DataRequest.Client;

namespace Intertoll.TollDataImport.DataRequest.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting...");
            while (true)
            {
                try
                {
                    int intd = int.Parse(Console.ReadLine());
                    TollDataImportDataRequestClient.RequestDataStatic(1, DataTypeRequest.Transaction, new List<int> { intd });
                }
                catch (Exception)
                {
                }
                
            }
        }
    }
}
