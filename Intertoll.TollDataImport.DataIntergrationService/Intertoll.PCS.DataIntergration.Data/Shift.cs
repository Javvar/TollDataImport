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
    
    public partial class Shift
    {
        public string Shift_Identifier { get; set; }
        public int Shift_Number { get; set; }
        public System.DateTime Shift_StartTime { get; set; }
        public System.DateTime Shift_EndTime { get; set; }
        public string Shift_LaneCode { get; set; }
        public string Staff_Identifier { get; set; }
        public Nullable<bool> Processed { get; set; }
        public Nullable<bool> Duplicate { get; set; }
        public System.DateTime TimeStamp { get; set; }
    }
}
