using Intertoll.DataImport.DataRequest;
using Intertoll.DataImport.DataRequest.Client;
using Intertoll.NLogger;
using Intertoll.TollDataImport.DataRequest.Data;
using Intertoll.TollDataImport.DataRequest.Data.Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace Intertoll.DataImport.DataRequest
{
	[DisallowConcurrentExecution]
	public class DataRequestJob : IJob
	{
		public DateTime AuditDate
		{
			get
			{
				var adate = ConfigurationManager.AppSettings["AuditDate"];
				return adate == null ? DateTime.Today : DateTime.ParseExact(adate, "yyyy-MM-dd",CultureInfo.InvariantCulture);
			}
		}

		public void Execute(IJobExecutionContext context)
		{
			Log.LogInfoMessage("[Enter] auditing"); 

			var HistoricalDateLimit = AuditDate.AddDays(-1 * int.Parse(ConfigurationManager.AppSettings["NumberOfDaysInThePast"]));

			using (PCSDataContext PCSContext = new PCSDataContext())
			{
				foreach (PCSAudit audit in PCSContext.Audits.Get(x => x.TransAuditStatus == (int)AuditStatus.ValidatedIncorrect)
																	  .Where(x => x.AuditDate >= HistoricalDateLimit) 
																	  .OrderBy(x => x.AuditDate)
																	  .ThenBy(x => x.AuditHour)
																	  .Take(int.Parse(ConfigurationManager.AppSettings["AuditChunkSize_Trans"])))
				{
					PCSAudit audit_ = audit; // avoid closure issues
					DateTime StartDate = audit_.AuditDate.Date;
					DateTime EndDate = audit_.AuditDate.Date.AddDays(1);

					var TransactionCount = PCSContext.Transactions.Count(x => x.TransDate.Hour == audit_.AuditHour &&
																			  x.TransDate > StartDate &&
																			  x.TransDate < EndDate &&
																			  x.Session.Lane.LaneGUID == audit_.LaneGuid);

					AuditTransactions(TransactionCount, audit, PCSContext);
				}

				PCSContext.Save();
			}

			Log.LogInfoMessage("[Exit] auditing");
		}

		static void AuditTransactions(int TransactionCount, PCSAudit _Audit, PCSDataContext _PCSContext)
		{
			if (TransactionCount < _Audit.TransRecordCount)
			{
				Log.LogInfoMessage($"About to audit transactions for lane {_Audit.Lane.LaneCode} : {_Audit.AuditDate.ToShortDateString()} : {_Audit.AuditHour}");

				List<int> MissingTransactions = new List<int>();

				for (int i = _Audit.TransStartSeqNumber; i <= _Audit.TransEndSeqNumber; i++)
				{
					int i_ = i; // avoid closure issues

					if (!_PCSContext.Transactions.Any(x => x.LaneTransSeqNr == i_ && x.Session.LaneGUID == _Audit.LaneGuid))
					{
						Log.LogTrace($"Missing transaction: {_Audit.Lane.LaneCode} : {i}");
						MissingTransactions.Add(i);
					}
				}

				if (MissingTransactions.Count > 0)
				{
					Log.LogInfoMessage($"About to request {MissingTransactions.Count} transactions for lane {_Audit.Lane.LaneCode}");
					TollDataRequestClient.RequestDataStatic(_Audit.Lane.LaneCode, DataTypeRequest.Transaction, MissingTransactions);
				}
				else
				{
					_Audit.TransAuditStatus = (int)AuditStatus.ValidatedCorrect;
				}
			}
			else
			{
				_Audit.TransAuditStatus = (int)AuditStatus.ValidatedCorrect;
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
