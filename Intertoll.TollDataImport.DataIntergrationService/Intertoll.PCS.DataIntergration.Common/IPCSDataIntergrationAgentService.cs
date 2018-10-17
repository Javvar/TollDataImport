using System;
using System.ServiceModel;

namespace Intertoll.PCS.DataIntergration.Common
{
	/// <summary>
    /// Service contract
	/// </summary>
	[ServiceContract()]
    public interface IPCSDataIntergrationAgentService
	{
		[OperationContract(IsOneWay = true)]
        void RequestMissingTransactions(DateTime _transactionDate, int _transactionHour);

        [OperationContract(IsOneWay = true)]
        void RequestMissingIncidents(DateTime _incidentDate, int _incidentHour);

        [OperationContract(IsOneWay = true)]
        void RequestMissingHourlyAudits(DateTime _hourlyAuditDate, int _hourlyAuditHour);
	}
}