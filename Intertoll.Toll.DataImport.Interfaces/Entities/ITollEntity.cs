using System;

namespace Intertoll.Toll.DataImport.Interfaces
{
    public interface ITollEntity
    {
        string LaneCode { get; set; }
        bool IsSent { get; set; }
        bool WellFormed { get; }
        DateTime TimeStamp { get; set; }
    }
}
