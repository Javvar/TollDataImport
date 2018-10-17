using System;
using System.Collections.Generic;
using System.ServiceModel;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Unity;
using Unity.Resolution;

namespace Intertoll.DataImport.DataRequest
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class TollDataRequest : ITollDataRequest
    {
        object lock_ = new object();

        private ITollDataProvider DataProvider { get; set; }
        IUnityContainer Container;

        public TollDataRequest()
        {
            
        }

        public TollDataRequest(ITollDataProvider _dataProvider, IUnityContainer _container)
        {
            DataProvider = _dataProvider;
            Container = _container;
        }

        public void RequestData(string laneCode, DataTypeRequest dataType, List<int> sequenceNumbers)
        {
            Log.LogTrace(string.Format("[Enter] {3} Data Request,Lane:{0} Type:{1} Count:{2}", laneCode, dataType, sequenceNumbers.Count,DateTime.Now.ToShortTimeString()));

            try
            {
                lock (lock_)
                {
                    switch (dataType)
                    {
                        case DataTypeRequest.Transaction:
                            RequestTransactions(laneCode, sequenceNumbers);
                            break;
                        case DataTypeRequest.Incident:
                            RequestIncidents(laneCode, sequenceNumbers);
                            break;
                        case DataTypeRequest.Audit:
                            RequestAudits(laneCode, sequenceNumbers);
                            break;
                        default:
                            throw new NotImplementedException(dataType.ToString());
                    }
                }

                Log.LogTrace(string.Format("[Exit] {3} Data Request,Lane:{0} Type:{1} Count:{2}", laneCode, dataType, sequenceNumbers.Count, DateTime.Now.ToShortTimeString()));
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }

            Log.LogTrace("[Exit] Data Request");
        }
       
        void RequestTransactions(string laneCode,List<int> sequenceNumbers)
        {
            List<ITollTransaction> SentTransactions = new List<ITollTransaction>();

            foreach (var seqNr in sequenceNumbers)
            {
                Log.LogTrace("Requesting Transaction: " + laneCode + " " + seqNr);

                var tollTransaction = DataProvider.GetTransaction(laneCode, seqNr);

                if (tollTransaction != null)
                {
                    Log.LogTrace("Found Transaction: " + laneCode + " " + seqNr);

                    var submitter = Container.Resolve<ITransactionSubmitter>(new ParameterOverride("_dataProvider", DataProvider));
                    var submittedEntity = submitter.Submit(tollTransaction);

                    if (submittedEntity.IsSent)
                        SentTransactions.Add(submittedEntity); 
                }
            }

            DataProvider.SaveTransactions(SentTransactions);
        }

        private void RequestIncidents(string laneCode, List<int> sequenceNumbers)
        {
            List<ITollIncident> SentIncidents = new List<ITollIncident>();

            foreach (var seqNr in sequenceNumbers)
            {
                Log.LogTrace("Requesting Incident: " + laneCode + " " + seqNr);

                var tollIncident = DataProvider.GetIncident(laneCode, seqNr);

                if (tollIncident != null)
                {
                    Log.LogTrace("Found Incident: " + laneCode + " " + seqNr);

                    var submitter = Container.Resolve<IIncidentSubmitter>(new ParameterOverride("_dataProvider", DataProvider));
                    var submittedEntity = submitter.Submit(tollIncident);

                    if (submittedEntity.IsSent)
                        SentIncidents.Add(submittedEntity);
                }
            }

            DataProvider.SaveIncidents(SentIncidents);
        }

        private void RequestAudits(string laneCode, List<int> sequenceNumbers)
        {
            // TODO: implement
        }
    }
}
