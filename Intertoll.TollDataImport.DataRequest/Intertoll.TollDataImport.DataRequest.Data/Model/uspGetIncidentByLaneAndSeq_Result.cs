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
    
    public partial class uspGetIncidentByLaneAndSeq_Result
    {
        public long Incident_Identifier { get; set; }
        public Nullable<long> IncidentSeqNr { get; set; }
        public int IncidentCode { get; set; }
        public System.DateTime IncidentSetDate { get; set; }
        public Nullable<long> LaneSeqNr { get; set; }
        public Nullable<long> Transaction_Identifier { get; set; }
        public Nullable<int> CollectorID { get; set; }
        public string LaneCode { get; set; }
        public string Description { get; set; }
    }
}
