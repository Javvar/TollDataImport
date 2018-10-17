using Intertoll.Toll.DataImport.Interfaces.Entities;

// ReSharper disable once CheckNamespace
namespace Intertoll.DataImport.Data
{
    public partial class Incident : ITollIncident
    {
        public bool WellFormed { get; private set; }
    }
}
