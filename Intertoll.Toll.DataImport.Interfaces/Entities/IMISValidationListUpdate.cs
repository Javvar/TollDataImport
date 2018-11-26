using System;

namespace Intertoll.Toll.DataImport.Interfaces.Entities
{
    public interface IMISValidationListUpdate
    {
        int Id { get; set; }
        int? IDVL { get; set; }
        string PAN { get; set; }
        string Action { get; set; }
        DateTime? DateSentToMIS { get; set; }
    }
}
