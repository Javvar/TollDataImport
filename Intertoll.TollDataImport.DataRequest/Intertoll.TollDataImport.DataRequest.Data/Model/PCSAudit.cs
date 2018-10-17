//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intertoll.TollDataImport.DataRequest.Data.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class PCSAudit
    {
        public int Id { get; set; }
        public System.Guid LaneGuid { get; set; }
        public System.DateTime AuditDate { get; set; }
        public int AuditHour { get; set; }
        public int TransStartSeqNumber { get; set; }
        public int TransEndSeqNumber { get; set; }
        public int TransRecordCount { get; set; }
        public int TransDifferenceNumber { get; set; }
        public int IncidentStartSeqNumber { get; set; }
        public int IncidentEndSeqNumber { get; set; }
        public int IncidentRecordCount { get; set; }
        public int IncidentDifferenceNumber { get; set; }
        public int SessionStartSeqNumber { get; set; }
        public int SessionEndSeqNumber { get; set; }
        public int SessionRecordCount { get; set; }
        public int SessionDifferenceNumber { get; set; }
        public int StaffLoginRecordCount { get; set; }
        public int StaffLoginDifferenceNumber { get; set; }
        public int TransAuditStatus { get; set; }
        public int IncidentAuditStatus { get; set; }
        public int SessionAuditStatus { get; set; }
        public int StaffLoginAuditStatus { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public int LaneMode { get; set; }
    
        public virtual PCSLane Lane { get; set; }
    }
}
