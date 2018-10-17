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
    
    public partial class PCSSession
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PCSSession()
        {
            this.Transactions = new HashSet<PCSTransaction>();
        }
    
        public System.Guid SessionGUID { get; set; }
        public int SessionSeq { get; set; }
        public System.Guid StaffLoginGUID { get; set; }
        public System.Guid LaneGUID { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.Guid> COLDeclarationGUID { get; set; }
        public Nullable<System.Guid> DayClosingGUID { get; set; }
        public int TransCount { get; set; }
        public int IncompleteTransCount { get; set; }
        public int IncidentCount { get; set; }
        public int LaneTransSeqStart { get; set; }
        public int LaneTransSeqEnd { get; set; }
        public int AvcStartSeq { get; set; }
        public int AvcEndSeq { get; set; }
        public int StartIncidentSeq { get; set; }
        public int EndIncidentSeq { get; set; }
        public int LaneRechargeCount { get; set; }
        public decimal CollectedRevenue { get; set; }
        public decimal RechargeRevenue { get; set; }
        public decimal ViolPaidRevenue { get; set; }
        public int SystemTransCount { get; set; }
        public Nullable<int> SystemPreviousTransNr { get; set; }
        public int SystemTransFirst { get; set; }
        public int SystemTransLast { get; set; }
        public int SystemTransNext { get; set; }
        public Nullable<int> SystemPreviousAVCnr { get; set; }
        public int SystemFirstAVCnr { get; set; }
        public int SystemLastAVCnr { get; set; }
        public int SystemNextAVCnr { get; set; }
        public int SystemIncidentCount { get; set; }
        public Nullable<int> SystemPreviousIncidentNr { get; set; }
        public int SystemIncidentFirst { get; set; }
        public int SystemIncidentLast { get; set; }
        public decimal SystemRevenue { get; set; }
        public bool IsComplete { get; set; }
        public bool IsValidationComplete { get; set; }
        public bool WasForced { get; set; }
    
        public virtual PCSLane Lane { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PCSTransaction> Transactions { get; set; }
        public virtual PCSStaffLogin StaffLogin { get; set; }
    }
}
