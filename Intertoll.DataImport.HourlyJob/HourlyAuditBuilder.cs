using System;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Intertoll.Toll.DataImport.Interfaces.EntityBuilders;

namespace Intertoll.DataImport.HourlyJob
{
    public class HourlyAuditBuilder : IHourlyAuditEntityBuilder
    {
        public ITollHourlyAudit Build(ITollHourlyAudit entity)
        {
            return entity.WellFormed ? entity : null;
        }
    }
}
