using Intertoll.Toll.DataImport.Interfaces.Entities;

// ReSharper disable once CheckNamespace
namespace Intertoll.DataImport.Data
{
    public partial class StaffLogin : ITollStaffLogin
    {
        public string LaneCode { get; set; }
        public bool WellFormed { get; private set; }
    }
}
