using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intertoll.Toll.DataImport.Interfaces.Entities;

// ReSharper disable once CheckNamespace
namespace Intertoll.DataImport.Data
{
    public partial class uspGetNextTransactionBatch_Result : ITollTransaction, IETCTollTransaction
    {
        public bool WellFormed { get; private set; }
        public Guid? ClassGUID { get; set; }
        public Guid? IssuerAuthenticatorGuid { get; set; }
        public Guid? OperatorAuthenticatorGuid { get; set; }
    }

    public partial class uspGetNextIncidentBatch_Result : ITollIncident
    {
        public bool WellFormed { get; private set; }
    }

    public partial class uspGetNextHourlyAuditBatch_Result : ITollHourlyAudit
    {
        public bool WellFormed
        {
            get { return TimeStamp != new DateTime(1970, 1, 1); } 
        }
    }

    public partial class uspGetLaneAliveStatus_Result : ILaneStatus
    {
        public bool Alive
        {
            get { return CNT != 0; }
        }
    }

    
}
