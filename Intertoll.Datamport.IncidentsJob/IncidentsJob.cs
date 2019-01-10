using System;
using System.Collections.Generic;
using Intertoll.DataImport.Schedulable;
using Intertoll.DataImport.SyncClient;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Intertoll.Toll.DataImport.Interfaces.EntityBuilders;
using Intertoll.DataImport.Data.DataContext;
using Quartz;
using Unity;
using Unity.Resolution;

namespace Intertoll.DataImport.IncidentsJob
{
    [DisallowConcurrentExecution]
    public class IncidentsJob : BaseSchedulable<IncidentsJob>
    {
        ITollDataProvider DataProvider;
        IUnityContainer Container;

        public IncidentsJob(IUnityContainer _container, ITollDataProvider _dataProvider)
        {
            Container = _container;
            DataProvider = _dataProvider;
        }

        public override void Execute(IJobExecutionContext context)
        {
            Log.LogTrace("[Enter]" + JobName);

            try
            {
                var incidentBatch = DataProvider.GetNextIncidentBatch();

                Log.LogTrace("Incident batch count: " + incidentBatch.Count);

                var SentIncidents = new List<ITollIncident>();

                foreach (var tollIncident in incidentBatch)
                {
                    var submitter = Container.Resolve<IIncidentSubmitter>(new ParameterOverride("_dataProvider", DataProvider));
                    var submittedEntity = submitter.Submit(tollIncident);

                    if (submittedEntity.IsSent)
                        SentIncidents.Add(submittedEntity);
                }

	            Log.LogTrace("Incident sent batch count: " + SentIncidents.Count);

				DataProvider.SaveIncidents(SentIncidents);
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }

            Log.LogTrace("[Exit]" + JobName);
        }
    }
}
