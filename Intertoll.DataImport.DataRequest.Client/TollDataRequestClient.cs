using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Intertoll.DataImport.DataRequest.Client
{
	public class TollDataRequestClient : ClientBase<ITollDataRequest>, ITollDataRequest
	{
		TollDataRequestClient(string configurationName) : base(configurationName)
		{
		}

		public void RequestData(string laneCode, DataTypeRequest dataType, List<int> sequenceNumbers)
		{
			Channel.RequestData(laneCode, dataType, sequenceNumbers);
		}

		public static void RequestDataStatic(string laneCode, DataTypeRequest dataType, List<int> sequenceNumbers)
		{
			foreach (string EndPoint in GetEndPointList())
			{
				var proxy = new TollDataRequestClient(EndPoint);
				proxy.RequestData(laneCode, dataType, sequenceNumbers);
			}
		}

		private static List<string> GetEndPointList()
		{
			List<string> endpointNames = new List<string>();
			ClientSection clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;

			if (clientSection != null)
			{
				var propertyInformation = clientSection.ElementInformation.Properties[string.Empty];

				if (propertyInformation != null)
				{
					ChannelEndpointElementCollection endpointCollection = propertyInformation.Value as ChannelEndpointElementCollection;

					if (endpointCollection != null)
					{
						foreach (var endPoint in endpointCollection)
						{
							if (((ChannelEndpointElement)endPoint).Contract == "Intertoll.DataImport.DataRequest.ITollDataRequest")
							{
								{
									Console.WriteLine(((ChannelEndpointElement)endPoint).Name);
									endpointNames.Add(((ChannelEndpointElement)endPoint).Name);
								}
							}
						}
					}
				}
			}

			return endpointNames;
		}
	}
}
