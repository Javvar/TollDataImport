using Intertoll.NLogger;
using Intertoll.PCS.DataIntergration.Common;
using Intertoll.PCS.DataIntergrationService.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Log = Intertoll.NLogger.Log;

namespace Intertoll.PCS.DataIntergrationService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, ReleaseServiceInstanceOnTransactionComplete = false)]
    public class IntergrationService : IPCSDataIntergrationAgentService
    {
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void RequestMissingTransactions(DateTime _transactionDate, int _transactionHour)
        {
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void RequestMissingIncidents(DateTime _incidentDate, int _incidentHour)
        {
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void  RequestMissingHourlyAudits(DateTime _hourlyAuditDate, int _hourlyAuditHour)
        {
        }

    }
}
