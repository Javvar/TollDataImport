using Intertoll.Toll.DataImport.Interfaces.Entities;

// ReSharper disable once CheckNamespace
namespace Intertoll.DataImport.Data
{
    public partial class Session : ITollSession
    {
        public bool WellFormed { get; private set; }
    }
}
