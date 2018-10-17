using System;
using System.Collections.Generic;
using System.ServiceModel;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.DataRequest;
using Intertoll.TollDataImport.DataRequest.Data;
using Intertoll.TollDataImport.DataRequest.Data.Model;

namespace Intertoll.TollDataImport.DataRequest
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)] 
    public class TollDataImportDataRequestService : ITollDataImportDataRequestService
    {
        object lock_ = new object();

        public void RequestData(int _LaneID, DataTypeRequest _DataType, List<int> _SequenceNumbers)
        {
            Log.LogInfoMessage(string.Format("[Enter] Data Request,Lane:{0} Type:{1} Count:{2}", _LaneID, _DataType,_SequenceNumbers.Count));

            try
            {
                lock (lock_)
                {
                    switch (_DataType)
                    {
                        case DataTypeRequest.Transaction:
                            TransactionRequestProcessor.SendTransactions(_LaneID, _SequenceNumbers);
                            break;
                        case DataTypeRequest.Incident:
                            IncidentRequestProcessor.SendIncidents(_LaneID, _SequenceNumbers);
                            break;
                        case DataTypeRequest.Audit:
                            SendAudits(_LaneID, _SequenceNumbers);
                            break;
                        default:
                            throw new NotImplementedException(_DataType.ToString());
                    }
                }

                Log.LogInfoMessage("[Exit] Data Request");
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        private void SendAudits(int _LaneID, List<int> _Hours)
        {
            var AuditsFound = new List<uspGetAuditByLaneAndHour_Result>();

            using (PCSStagingDataContext dc = new PCSStagingDataContext())
            {
                foreach (var hour in _Hours)
                {
                    var foundAudit = dc.GetLaneAudit(_LaneID, hour);

                    if (foundAudit != null)
                    {
                        AuditsFound.Add(foundAudit);
                    }
                }

                Log.LogInfoMessage("Found entities " + AuditsFound);
            }
        }
    }
}
