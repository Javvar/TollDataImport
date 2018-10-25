using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intertoll.Toll.DataImport.Interfaces.Entities
{
    public interface IMISAccountBalanceUpdate
    {
        int Id { get; set; }
        string MISAccountNr { get; set; }
        Guid PCSAccountGuid { get; set; }
        decimal OldBalance { get; set; }
        decimal NewBalance { get; set; }
        DateTime DateCreated { get; set; }
        DateTime? DateSentToMIS { get; set; }
    }
}
