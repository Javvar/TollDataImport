using System;

namespace Intertoll.Toll.DataImport.Interfaces.Entities
{
    public interface ITollStaffLogin : ITollEntity
    {
        Guid StaffLoginGUID { get; set; }
        Guid StaffGUID { get; set; }
        DateTime StartDate { get; set; }
        DateTime? EndDate { get; set; }
        Guid RoleGUID { get; set; }
        Guid LocationGUID { get; set; }
        Guid LocationTypeGUID { get; set; }
    }
}
