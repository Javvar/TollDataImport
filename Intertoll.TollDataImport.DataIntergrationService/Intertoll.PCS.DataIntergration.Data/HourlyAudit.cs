//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intertoll.PCS.DataIntergration.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class HourlyAudit
    {
        public long Id { get; set; }
        public System.DateTime AuditDate { get; set; }
        public int Hour { get; set; }
        public int TransactionStartSeqNumber { get; set; }
        public int TransactionEndSeqNumber { get; set; }
        public int TransDifferenceNumber { get; set; }
        public int TransactionCount { get; set; }
        public int IncidentsStartSeqNumber { get; set; }
        public int IncidentEndSeqNumber { get; set; }
        public int IncidentDifferenceNumber { get; set; }
        public int IncidentsCount { get; set; }
        public int VGSIncidentStartSeqNo { get; set; }
        public int VGSIncidentEndSeqNo { get; set; }
        public int VGSIncidentCount { get; set; }
        public int VGSDifferenceNumber { get; set; }
        public string LaneCode { get; set; }
        public Nullable<bool> Processed { get; set; }
        public Nullable<bool> Duplicate { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public int SessionStartSeqNumber { get; set; }
        public int SessionEndSeqNumber { get; set; }
        public int SessionDifferenceNumber { get; set; }
        public int SessionRecordCount { get; set; }
        public int StaffLoginRecordCount { get; set; }
        public int StaffLoginDifferenceNumber { get; set; }
        public int TransAuditStatus { get; set; }
        public int IncidentAuditStatus { get; set; }
        public int SessionAuditStatus { get; set; }
        public int StaffLoginAuditStatus { get; set; }
        public int LaneMode { get; set; }
        public System.Guid LaneGuid { get; set; }
    }
}
