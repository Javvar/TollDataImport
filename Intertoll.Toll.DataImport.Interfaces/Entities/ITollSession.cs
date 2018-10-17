using System;

namespace Intertoll.Toll.DataImport.Interfaces.Entities
{
    public interface ITollSession : ITollEntity
    {
        Guid SessionGUID { get; set; }
        Guid StaffLoginGUID { get; set; }
        int SessionSeq { get; set; }
        Guid LaneGUID { get; set; }
        DateTime StartDate { get; set; }
        DateTime? EndDate { get; set; }

        //ITollStaffLogin StaffLogin { get; set; }
    }
}
