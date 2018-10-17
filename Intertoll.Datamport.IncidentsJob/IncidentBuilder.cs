using System;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Intertoll.Toll.DataImport.Interfaces.EntityBuilders;

namespace Intertoll.DataImport.IncidentsJob
{
    public class IncidentEntityBuilder : IIncidentEntityBuilder
    {
        public ITollIncident Build(ITollIncident entity)
        {
            return entity.StaffLoginGUID.HasValue && entity.IncidentTypeGUID != new Guid("B6AB99E8-5DA4-4C33-9EDD-08298A1A2104") ? entity : null;
        }
    }
}
