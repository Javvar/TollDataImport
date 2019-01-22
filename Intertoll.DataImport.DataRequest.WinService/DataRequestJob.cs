using Intertoll.DataImport.DataRequest;
using Intertoll.DataImport.DataRequest.Client;
using Intertoll.TollDataImport.DataRequest.Data;
using Intertoll.TollDataImport.DataRequest.Data.Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Intertoll.DataImport.DataRequest
{
	[DisallowConcurrentExecution]
	public class DataRequestJob : IJob
	{
		public void Execute(IJobExecutionContext context)
		{
			var Date = DateTime.Today;
			var PreviousDay = Date.AddDays(-1 * int.Parse(ConfigurationManager.AppSettings["NumberOfDaysInThePast"]));

			using (PCSDataContext PCSContext = new PCSDataContext())
			{
				foreach (PCSAudit audit in PCSContext.Audits.Get(x => x.TransAuditStatus == (int)AuditStatus.ValidatedIncorrect)
																	  .Where(x => x.AuditDate >= PreviousDay) //todo: exclude converted lanes
																	  .OrderBy(x => x.AuditDate).ThenBy(x => x.AuditHour)
																	  .Take(int.Parse(ConfigurationManager.AppSettings["AuditChunkSize_Trans"])))
				{
					//Log.LogInfoMessage(string.Format("Auditing transactions {0} {1} {2}", audit.Lane.LaneCode, audit.AuditDate.ToShortDateString(), audit.AuditHour));

					PCSAudit audit_ = audit; // avoid closure issues
					DateTime StartDate = audit_.AuditDate.Date;
					DateTime EndDate = audit_.AuditDate.Date.AddDays(1);

					var TransactionCount = PCSContext.Transactions.Count(x => x.TransDate.Hour == audit_.AuditHour &&
																			  x.TransDate > StartDate &&
																			  x.TransDate < EndDate &&
																			  x.Session.Lane.LaneGUID == audit_.LaneGuid);

					AuditTransactions(TransactionCount, audit, PCSContext);

				}
			}
		}

		static void AuditTransactions(int TransactionCount, PCSAudit _Audit, PCSDataContext _PCSContext)
		{
			if (TransactionCount < _Audit.TransRecordCount)
			{
				List<int> MissingTransactions = new List<int>();
				for (int i = _Audit.TransStartSeqNumber; i <= _Audit.TransEndSeqNumber; i++)
				{
					int i_ = i; // avoid closure issues

					if (!_PCSContext.Transactions.Any(x => x.LaneTransSeqNr == i_ && x.Session.LaneGUID == _Audit.LaneGuid))
						MissingTransactions.Add(i);
				}

				TollDataRequestClient.RequestDataStatic(_Audit.Lane.LaneCode, DataTypeRequest.Transaction, MissingTransactions);
				_Audit.TransAuditStatus = (int)AuditStatus.ValidatedIncorrect;

				//Log.LogInfoMessage(string.Format("Transactions missing between {0}h00 and {1}h00: {2}", _Audit.AuditHour - 1, _Audit.AuditHour, MissingTransactions.Count));
			}
		}

		static void AuditIncidents(int IncidentCount, PCSAudit _Audit, PCSDataContext _PCSContext)
		{
			if (IncidentCount < _Audit.IncidentRecordCount)
			{
				List<int> MissingIncidents = new List<int>();

				for (int i = _Audit.IncidentStartSeqNumber; i <= _Audit.IncidentEndSeqNumber; i++)
				{
					int i_ = i; // avoid closure issues

					var exists = (from inc in _PCSContext.Incidents.Where(x => x.IncidentSeqNr == i_)
								  join sl in _PCSContext.StaffLogins.Where() on inc.StaffLoginGUID equals sl.StaffLoginGUID
								  where sl.LocationGUID == _Audit.LaneGuid
								  select inc.IncidentGUID).Any();

					if (!exists)
						MissingIncidents.Add(i);
				}

				TollDataRequestClient.RequestDataStatic(_Audit.Lane.LaneCode, DataTypeRequest.Incident, MissingIncidents);

				//Log.LogInfoMessage(string.Format("Incidents missing between {0}h00 and {1}h00: {2}", _Audit.AuditHour - 1, _Audit.AuditHour, MissingIncidents.Count));
			}
		}
	}

	enum AuditStatus
	{
		Unvalidated = 0,
		ValidatedCorrect = 1,
		ValidatedIncorrect = 2
	}
}
