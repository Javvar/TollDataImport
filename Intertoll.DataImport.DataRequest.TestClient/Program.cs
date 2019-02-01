using Intertoll.DataImport.DataRequest.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intertoll.DataImport.DataRequest.TestClient
{
	class Program
	{
		static void Main(string[] args)
		{
			TollDataRequestClient.RequestDataStatic("13MS",DataTypeRequest.Transaction,new List<int>{0});
		}
	}
}
