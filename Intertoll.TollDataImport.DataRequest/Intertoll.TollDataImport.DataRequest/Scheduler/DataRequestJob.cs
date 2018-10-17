using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.DataRequest;
using Intertoll.Toll.DataImport.DataRequest.Client;
using Intertoll.TollDataImport.DataRequest.Data;
using Quartz;
using Intertoll.TollDataImport.DataRequest.Data.Model;

namespace Intertoll.TollDataImport.DataRequest.Scheduler
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
                                                                      .Where(x => x.AuditDate >= PreviousDay && (x.Lane.LaneCode == "09MS" || x.Lane.LaneCode == "12MN"))
                                                                      .OrderBy(x => x.AuditDate).ThenBy(x => x.AuditHour)
                                                                      .Take(int.Parse(ConfigurationManager.AppSettings["AuditChunkSize_Trans"])))
                {
                    Log.LogInfoMessage(string.Format("Auditing transactions {0} {1} {2}", audit.Lane.LaneCode, audit.AuditDate.ToShortDateString(), audit.AuditHour));

                    PCSAudit audit_ = audit; // avoid closure issues
                    DateTime StartDate = audit_.AuditDate.Date;
                    DateTime EndDate = audit_.AuditDate.Date.AddDays(1);

                    var TransactionCount = PCSContext.Transactions.Count(x => x.TransDate.Hour == audit_.AuditHour && 
                                                                              x.TransDate > StartDate && 
                                                                              x.TransDate < EndDate && 
                                                                              x.Session.Lane.LaneGUID == audit_.LaneGuid);

                    AuditTransactions(TransactionCount, audit, PCSContext);

                }

                /*foreach (PCSAudit audit in PCSContext.Audits.Get(x => x.IncidentAuditStatus == (int)AuditStatus.ValidatedIncorrect)
                                                                      .Where(x => x.AuditDate >= PreviousDay)
                                                                      .OrderBy(x => x.AuditDate).ThenBy(x => x.AuditHour)
                                                                      .Take(int.Parse(ConfigurationManager.AppSettings["AuditChunkSize_Inc"])))
                {
                    Log.LogInfoMessage(string.Format("Auditing incidents {0} {1} {2}", audit.Lane.LaneCode, audit.AuditDate.ToShortDateString(), audit.AuditHour));

                    PCSAudit audit_ = audit; // avoid closure issues
                    DateTime StartDate = audit_.AuditDate.Date;
                    DateTime EndDate = audit_.AuditDate.Date.AddDays(1);

                    var IncidentCount = (from i in PCSContext.Incidents.Where(x => x.IncidentSetDate.Hour == audit_.AuditHour && x.IncidentSetDate > StartDate && x.IncidentSetDate < EndDate)
                                         join sl in PCSContext.StaffLogins.Where() on i.StaffLoginGUID equals sl.StaffLoginGUID
                                         where sl.LocationGUID == audit_.LaneGuid
                                         select i.IncidentGUID).Count();

                    AuditIncidents(IncidentCount, audit, PCSContext);
                }*/
            }
        }

        /*private void AuditDateFiles(DateTime Date)
        {
            var PreviousDay = Date.PreviousDay();

            using (PCSAgentDataContext PCSContext = new PCSAgentDataContext())
            {
                foreach (Audit audit in PCSContext.AuditRepository.Get(x => x.TransAuditStatus != (int)AuditStatus.ValidatedCorrect || x.IncidentAuditStatus != (int)AuditStatus.ValidatedCorrect ||
                                                                                x.SessionAuditStatus != (int)AuditStatus.ValidatedCorrect || x.StaffLoginAuditStatus != (int)AuditStatus.ValidatedCorrect)
                                                                      .Where(x => x.AuditDate >= PreviousDay)
                                                                      .Take(20))
                {
                    Log.LogInfoMessage(string.Format("Auditing {0} {1} {2}", audit.Lane.LaneCode, audit.AuditDate.ToShortDateString(), audit.AuditHour));

                    Audit audit_ = audit; // avoid closure issues
                    DateTime StartDate = audit_.AuditDate.Date;
                    DateTime EndDate = audit_.AuditDate.Date.AddDays(1);

                    var TransactionCount = PCSContext.TransactionRepository.Count(x => x.TransDate.Hour == audit_.AuditHour && x.TransDate > StartDate && x.TransDate < EndDate);
                    var IncidentCount = PCSContext.IncidentRepository.Count(x => x.IncidentSetDate.Hour == audit_.AuditHour && x.IncidentSetDate > StartDate && x.IncidentSetDate < EndDate);
                    var LoginsCount = PCSContext.StaffLoginRepository.Count(x => x.StartDate.Hour == audit_.AuditHour && x.StartDate > StartDate && x.StartDate < EndDate);
                    var SessionCount = PCSContext.SessionRepository.Count(x => x.StartDate.Hour == audit_.AuditHour && x.StartDate > StartDate && x.StartDate < EndDate);

                    AuditTransactions(TransactionCount, audit, PCSContext);
                    AuditIncidents(IncidentCount, audit, PCSContext);
                    AuditStaffLogins(LoginsCount, audit);
                    AuditSessions(SessionCount, audit, PCSContext);
                }

                PCSContext.Save();
            }
        }*/

        private static void AuditTransactions(int TransactionCount, PCSAudit _Audit, PCSDataContext _PCSContext)
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

                TollDataImportDataRequestClient.RequestDataStatic(int.Parse(_Audit.Lane.LaneCode.Substring(0,2)), DataTypeRequest.Transaction, MissingTransactions);
                _Audit.TransAuditStatus = (int)AuditStatus.ValidatedIncorrect;

                Log.LogInfoMessage(string.Format("Transactions missing between {0}h00 and {1}h00: {2}", _Audit.AuditHour - 1, _Audit.AuditHour, MissingTransactions.Count));
            }
        }

        private static void AuditIncidents(int IncidentCount, PCSAudit _Audit, PCSDataContext _PCSContext)
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

                TollDataImportDataRequestClient.RequestDataStatic(int.Parse(_Audit.Lane.LaneCode.Substring(0, 2)), DataTypeRequest.Incident, MissingIncidents);

                Log.LogInfoMessage(string.Format("Incidents missing between {0}h00 and {1}h00: {2}", _Audit.AuditHour - 1, _Audit.AuditHour, MissingIncidents.Count));
            }
        }

        /*private static void AuditSessions(int _SessionCount, Audit _Audit, PCSDataContext _PCSContext)
        {
            if (_SessionCount < _Audit.SessionRecordCount)
            {
                List<int> MissingSessions = new List<int>();

                for (int i = _Audit.SessionStartSeqNumber; i <= _Audit.SessionEndSeqNumber; i++)
                {
                    int i_ = i; // avoid closure issues

                    if (!_PCSContext.SessionRepository.Any(x => x.SessionSeq == i_))
                        MissingSessions.Add(i);
                }

                DataRequestClient.RequestDataStatic(_Audit.LaneGuid, DataTypeRequest.Session, MissingSessions);
                _Audit.SessionAuditStatus = (int)AuditStatus.ValidatedIncorrect;

                Log.LogInfoMessage(string.Format("Sessions missing between {0}h00 and {1}h00: {2}", _Audit.AuditHour - 1, _Audit.AuditHour, MissingSessions.Count));
            }
            else
            {
                _Audit.SessionAuditStatus = (int)AuditStatus.ValidatedCorrect;
            }
        }*/

        /*private static void AuditStaffLogins(int _LoginsCount, PCSAudit _Audit)
        {
            if (_LoginsCount < _Audit.StaffLoginRecordCount)
            {
                TollDataImportDataRequestClient.RequestDataStatic(_Audit.LaneGuid, _Audit.AuditHour);
                _Audit.StaffLoginAuditStatus = (int)AuditStatus.ValidatedIncorrect;

                Log.LogInfoMessage(string.Format("Staff Login(s) missing between {0}h00 and {1}h00", _Audit.AuditHour - 1, _Audit.AuditHour));

            }
            else
            {
                _Audit.StaffLoginAuditStatus = (int)AuditStatus.ValidatedCorrect;
            }
        }*/
    }

    enum AuditStatus
    {
        Unvalidated = 0,
        ValidatedCorrect = 1,
        ValidatedIncorrect = 2
    }
}
