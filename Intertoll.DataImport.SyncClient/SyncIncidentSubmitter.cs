using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Intertoll.Toll.DataImport.Interfaces.EntityBuilders;

namespace Intertoll.DataImport.SyncClient
{
    public class SyncIncidentSubmitter : IIncidentSubmitter
    {
        public ITollDataProvider DataProvider { get; set; }
        public IIncidentEntityBuilder EntityBuilder { get; set; }

        static SyncIncidentSubmitter()
        {
            Mapper.CreateMap<ITollIncident, Data.Incident>();
        }

        public SyncIncidentSubmitter(ITollDataProvider _dataProvider, IIncidentEntityBuilder _builder)
        {
            DataProvider = _dataProvider;
            EntityBuilder = _builder;
        }

        public ITollIncident Submit(ITollIncident incident)
        {
            var builtIncident = EntityBuilder.Build(incident);

            if (builtIncident != null)
            {
                try
                {
                    var syncIncident = Mapper.Map<ITollIncident, Data.Incident>(builtIncident);
                    Sync.Client.SyncClient.SubmitIncident(syncIncident);

	                incident.IsSent = true;
                    return builtIncident;
                }
                catch (Exception ex)
                {
                    Log.LogException(ex);
                    Log.LogTrace(ex.Message + ". Check error log for more details.");
                }
            }

            return incident;
        }
    }
}
