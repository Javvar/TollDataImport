using System;

namespace Intertoll.Toll.DataImport.Interfaces.Entities
{
    public interface ILaneStatus
    {
        Guid LaneGuid { get; set; }
        bool Alive { get; }
    }
}
