using System;

namespace Intertoll.Toll.DataImport.Interfaces.Entities
{
    public interface ITollHourlyAudit : ITollEntity
    {
        int Id { get; set; }
        string AuditID { get; set; }
        Guid LaneGuid { get; set; }
        DateTime AuditDate { get; set; }
        int AuditHour { get; set; }
        int TransStartSeqNumber { get; set; }
        int TransEndSeqNumber { get; set; }
        int TransDifferenceNumber { get; set; }
        int TransRecordCount { get; set; }
        int IncidentStartSeqNumber { get; set; }
        int IncidentEndSeqNumber { get; set; }
        int IncidentDifferenceNumber { get; set; }
        int IncidentRecordCount { get; set; }
        int SessionStartSeqNumber { get; set; }
        int SessionEndSeqNumber { get; set; }
        int SessionDifferenceNumber { get; set; }
        int SessionRecordCount { get; set; }
        int StaffLoginRecordCount { get; set; }
        int StaffLoginDifferenceNumber { get; set; }
        int TransAuditStatus { get; set; }
        int IncidentAuditStatus { get; set; }
        int SessionAuditStatus { get; set; }
        int StaffLoginAuditStatus { get; set; }
        int LaneMode { get; set; }
    }
}
