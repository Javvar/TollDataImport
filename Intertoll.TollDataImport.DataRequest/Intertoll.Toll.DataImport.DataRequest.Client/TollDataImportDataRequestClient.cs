using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Intertoll.NLogger;

namespace Intertoll.Toll.DataImport.DataRequest.Client
{
    public class TollDataImportDataRequestClient : ClientBase<ITollDataImportDataRequestService>, ITollDataImportDataRequestService
    {
        private TollDataImportDataRequestClient(string configurationName): base(configurationName)
        {
        }

        public static void RequestDataStatic(int _LaneID, DataTypeRequest _DataType, List<int> _SequenceNumbers)
        {
            foreach (string EndPoint in GetEndPointList())
            {
                TollDataImportDataRequestClient proxy = new TollDataImportDataRequestClient(EndPoint);
                proxy.RequestData(_LaneID, _DataType, _SequenceNumbers);
            }
        }

        public void RequestData(int _LaneID, DataTypeRequest _DataType, List<int> _SequenceNumbers)
        {
            try
            {
                Channel.RequestData(_LaneID, _DataType, _SequenceNumbers);
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Console.WriteLine(ex.Message);
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
                            if (((ChannelEndpointElement)endPoint).Contract == "Intertoll.Toll.DataImport.DataRequest.ITollDataImportDataRequestService")
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
