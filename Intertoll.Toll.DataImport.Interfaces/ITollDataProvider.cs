using System;
using System.Collections.Generic;
using Intertoll.Toll.DataImport.Interfaces.Entities;

namespace Intertoll.Toll.DataImport.Interfaces
{
    public interface ITollDataProvider
    {
        IList<ITollTransaction> GetNextTransactionBatch();
        IList<ITollTransaction> GetTransactionBatchGreaterThanTime(DateTime dateFrom);
        void SaveTransactions(IList<ITollTransaction> sentTransactions);
        ITollTransaction GetTransaction(string laneCode, int sequenceNumber);

        IList<ITollIncident> GetNextIncidentBatch();
        IList<ITollIncident> GetIncidentBatchGreaterThanTime(DateTime dateFrom);
        void SaveIncidents(IList<ITollIncident> sentIncidents);
        ITollIncident GetIncident(string laneCode, int sequenceNumber);

        IList<ITollHourlyAudit> GetNextHourlyAuditBatch(DateTime? date);

        ITollSession GetSessionFollowingSession(string laneCode, DateTime? transactionTime);
        ITollSession GetSessionPrecedingSession(string laneCode, DateTime? transactionTime);
        IList<ITollSession> GetSessionsToSlice();

        IList<ITollSession> GetUnsentSessions();
        IList<ITollStaffLogin> GetUnsentStaffLogins();

        IList<ILaneStatus> GetLaneStatuses(); 
        
        void InsertTransaction(ITollTransaction newTransaction);
        void InsertIncident(ITollIncident newIncident);
        void InsertAudit(ITollHourlyAudit newAudit);

        IList<string> ImportNewStaff();
        void ImportNewRegisteredUsers();
        IList<string> GetNewFrequentUsersCreated();
        void SetFrequentUserMappingAsReported(IList<string> freqUserMapping);

        IList<IMISAccountBalanceUpdate> GetListOfMISAccountBalanceUpdates();
        void SetSentMISAccountBalanceUpdate(IMISAccountBalanceUpdate update);

        void Save();
    }
}