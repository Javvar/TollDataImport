using System;

namespace Intertoll.Toll.DataImport.Interfaces.Entities
{
    public interface IMISHotlistUpdate
    {
        int Id { get; set; }
        string CardNr { get; set; }
        string Change { get; set; }
        DateTime? DateSentToMIS { get; set; }
    }
}
