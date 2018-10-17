using System;

namespace Intertoll.Toll.DataImport.Interfaces.Entities
{
    public interface ITollIncident: ITollEntity
    {
        Guid IncidentGUID { get; set; }
        Guid? StaffLoginGUID { get; set; }
        Guid? IncidentTypeGUID { get; set; }
        Guid IncidentStatusGUID { get; set; }

        DateTime IncidentSetDate { get; set; }

        string IncidentID { get; set; }
        int? IncidentSeqNr { get; set; }
        int? IncidentCode { get; set; }
        int LaneSeqNr { get; set; }
        string TransactionID { get; set; }
        string StaffID { get; set; }
        
        string Description { get; set; }
        Guid? TransactionGUID { get; set; }
    }
}
