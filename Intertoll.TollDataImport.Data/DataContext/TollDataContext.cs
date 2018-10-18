using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using Intertoll.EFDbContext.GenericWrapper;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;

namespace Intertoll.DataImport.Data.DataContext
{
    public class TollDataContext : BaseDataContext<DataImportEntities>, ITollDataProvider
    {
        static TollDataContext()
        {
            Mapper.CreateMap<ITollTransaction, Transaction>();
            Mapper.CreateMap<ITollIncident, Incident>();
            Mapper.CreateMap<ITollSession, Session>();
            Mapper.CreateMap<ITollStaffLogin, StaffLogin>();
            Mapper.CreateMap<ITollHourlyAudit, Audit>();
        }

        #region Repositories

        private IGenericRepository<ITollTransaction> transactions;
        public IGenericRepository<ITollTransaction> Transactions
        {
            get { return transactions ?? (transactions = new GenericRepository<ITollTransaction>(context)); }
        }

        private IGenericRepository<ITollIncident> incidents;
        public IGenericRepository<ITollIncident> Incidents
        {
            get { return incidents ?? (incidents = new GenericRepository<ITollIncident>(context)); }
        }

        private IGenericRepository<StaffLogin> staffLogins;
        public IGenericRepository<StaffLogin> StaffLogins
        {
            get { return staffLogins ?? (staffLogins = new GenericRepository<StaffLogin>(context)); }
        }

        private IGenericRepository<ITollHourlyAudit> audits;
        public IGenericRepository<ITollHourlyAudit> Audits
        {
            get { return audits ?? (audits = new GenericRepository<ITollHourlyAudit>(context)); }
        }

        private IGenericRepository<Session> sessions;
        public IGenericRepository<Session> Sessions
        {
            get { return sessions ?? (sessions = new GenericRepository<Session>(context)); }
        }

        #endregion

        #region ITollDataProvider

        public IList<string> ImportNewStaff()
        {
            return context.uspImportNewStaff().ToList();
        }

        public IList<string> ImportNewRegisteredUsers()
        {
            return context.uspImportNewRegisteredUsers().ToList();
        }

        public IList<ITollTransaction> GetNextTransactionBatch()
        {
            var returnList = new List<ITollTransaction>();
            var transactionBatch = context.uspGetNextTransactionBatch().ToList();

            foreach (var batchResult in transactionBatch)
                returnList.Add(batchResult);

            return returnList;
        }

        public IList<ITollTransaction> GetTransactionBatchGreaterThanTime(DateTime dateFrom)
        {
            var returnedTransactions = context.uspGetTransactionBatchGreaterThanTime(dateFrom).ToList();
            return Mapper.Map<IList<uspGetTransactionBatchGreaterThanTime_Result>, IList<ITollTransaction>>(returnedTransactions);
        }

        public void SaveTransactions(IList<ITollTransaction> sentTransactions)
        {
            try
            {
                using (var ctx = new TollDataContext())
                {
                    ctx.context.Configuration.AutoDetectChangesEnabled = false;

                    foreach (var sentTransaction in sentTransactions)
                    {
                        var mappedtransaction = Mapper.Map<ITollTransaction, Transaction>(sentTransaction);
                        ctx.context.Entry(mappedtransaction).State = EntityState.Modified;
                    }

                    ctx.Save();
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
                Log.LogInfoMessage("About to start saving transactions individually.");

                foreach (ITollTransaction sentTransaction in sentTransactions)
                {
                    using (var ctx = new TollDataContext())
                    {
                        ctx.context.Configuration.AutoDetectChangesEnabled = false;

                        Transaction mappedtransaction = Mapper.Map<ITollTransaction, Transaction>(sentTransaction);
                        ctx.context.Entry(mappedtransaction).State = EntityState.Modified;
                        ctx.Save();
                    }
                }
            }
        }

        public ITollTransaction GetTransaction(string laneCode, int sequenceNumber)
        {
            return context.uspGetTransaction(laneCode, sequenceNumber).FirstOrDefault();
        }

        public IList<ITollIncident> GetNextIncidentBatch()
        {
            var returnList = new List<ITollIncident>();
            var incidentBatch = context.uspGetNextIncidentBatch().ToList();

            foreach (var batchResult in incidentBatch)
                returnList.Add(batchResult);

            return returnList;;
        }

        public IList<ITollIncident> GetIncidentBatchGreaterThanTime(DateTime dateFrom)
        {
            var returnedIncidents = context.uspGetIncidentBatchGreaterThanTime(dateFrom).ToList();
            return Mapper.Map<IList<uspGetIncidentBatchGreaterThanTime_Result>, IList<ITollIncident>>(returnedIncidents);
        }

        public void SaveIncidents(IList<ITollIncident> sentIncidents)
        {
            try
            {
                using (var ctx = new TollDataContext())
                {
                    ctx.context.Configuration.AutoDetectChangesEnabled = false;

                    foreach (var sentIncident in sentIncidents)
                    {
                        var mappedtransaction = Mapper.Map<ITollIncident, Incident>(sentIncident);
                        ctx.context.Entry(mappedtransaction).State = EntityState.Modified;
                    }

                    ctx.Save();
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
                Log.LogInfoMessage("About to start saving transactions individually.");

                foreach (var sentIncident in sentIncidents)
                {
                    using (var ctx = new TollDataContext())
                    {
                        ctx.context.Configuration.AutoDetectChangesEnabled = false;

                        var mappedIncident = Mapper.Map<ITollIncident, Incident>(sentIncident);
                        ctx.context.Entry(mappedIncident).State = EntityState.Modified;
                        ctx.Save();
                    }
                }
            }
        }

        public ITollIncident GetIncident(string laneCode, int sequenceNumber)
        {
            return context.uspGetIncident(laneCode, sequenceNumber).FirstOrDefault();
        }

        public IList<ITollHourlyAudit> GetNextHourlyAuditBatch(DateTime? date=null)
        {
            var returnList = new List<ITollHourlyAudit>();
            var batch = context.uspGetNextHourlyAuditBatch(date).ToList();

            foreach (var batchResult in batch)
                returnList.Add(batchResult);

            return returnList;
        }

        public ITollSession GetLaneSession(string laneCode)
        {
            throw new NotImplementedException();
        }

        public ITollSession GetSessionFollowingSession(string laneCode, DateTime? transactionTime)
        {
            throw new NotImplementedException();
            //return context.uspGetSessionFollowingSession(laneCode, transactionTime).;
        }

        public ITollSession GetSessionPrecedingSession(string laneCode, DateTime? transactionTime)
        {
            throw new NotImplementedException();
        }

        public IList<ITollSession> GetSessionsToSlice()
        {
            throw new NotImplementedException();
        }

        public IList<ITollSession> GetUnsentSessions()
        {
            var sess = Sessions.Where(x => !x.IsSent);
            return Mapper.Map<IList<ITollSession>>(sess);
        }

        public IList<ITollStaffLogin> GetUnsentStaffLogins()
        {
            var sls = StaffLogins.Where(x => !x.IsSent);
            return Mapper.Map<IList<ITollStaffLogin>>(sls);
        }

        public IList<ILaneStatus> GetLaneStatuses()
        {
            var laneStatuses = context.uspGetLaneAliveStatus();
            return Mapper.Map<IList<ILaneStatus>>(laneStatuses); 
        }

        public void InsertTransaction(ITollTransaction newtransactions)
        {
            context.Transactions.Add(Mapper.Map<ITollTransaction, Transaction>(newtransactions));
        }

        public void InsertIncident(ITollIncident newIncident)
        {
            context.Incidents.Add(Mapper.Map<ITollIncident, Incident>(newIncident));
        }

        public void InsertAudit(ITollHourlyAudit newAudit)
        {
            context.Audits.Add(Mapper.Map<ITollHourlyAudit, Audit>(newAudit));
        }        

        #endregion
    }
}
