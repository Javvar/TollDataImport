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
        #region ctor

        static TollDataContext()
        {
            Mapper.CreateMap<ITollTransaction, Transaction>();
            Mapper.CreateMap<ITollIncident, Incident>();
            Mapper.CreateMap<ITollSession, Session>();
            Mapper.CreateMap<ITollStaffLogin, StaffLogin>();
            Mapper.CreateMap<ITollHourlyAudit, Audit>();
            Mapper.CreateMap<IMISAccountBalanceUpdate, StagingMISAccountBalanceUpdate>();
        }

        #endregion

        #region Transactions

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
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
	            foreach (var eve in e.EntityValidationErrors)
	            {
		            Log.LogErrorMessage(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));

		            foreach (var ve in eve.ValidationErrors)
		            {
			            Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
		            }
	            }

	            throw;
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
	                    try
	                    {
		                    ctx.context.Configuration.AutoDetectChangesEnabled = false;

		                    Transaction mappedtransaction = Mapper.Map<ITollTransaction, Transaction>(sentTransaction);
		                    ctx.context.Entry(mappedtransaction).State = EntityState.Modified;
		                    ctx.Save();
	                    }
	                    catch (System.Data.Entity.Validation.DbEntityValidationException e)
	                    {
		                    foreach (var eve in e.EntityValidationErrors)
		                    {
			                    Log.LogErrorMessage(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));

			                    foreach (var ve in eve.ValidationErrors)
			                    {
				                    Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
			                    }
		                    }
	                    }
						catch (Exception e)
	                    {
							Log.LogException(ex);
		                    Log.LogTrace(ex.Message + ". Check error log for more details.");
		                    Log.LogInfoMessage("About to start saving transactions individually.");
						}
                    }
                }
            }
        }

        public ITollTransaction GetTransaction(string laneCode, int sequenceNumber)
        {
            return context.uspGetTransaction(laneCode, sequenceNumber).FirstOrDefault();
        }

        public void InsertTransaction(ITollTransaction newtransactions)
        {
            context.Transactions.Add(Mapper.Map<ITollTransaction, Transaction>(newtransactions));
        }

        #endregion

        #region Incidents

        public IList<ITollIncident> GetNextIncidentBatch()
        {
            var returnList = new List<ITollIncident>();
            var incidentBatch = context.uspGetNextIncidentBatch().ToList();

            foreach (var batchResult in incidentBatch)
                returnList.Add(batchResult);

            return returnList; 
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
                        var mappedIncident = Mapper.Map<ITollIncident, Incident>(sentIncident);
                        ctx.context.Entry(mappedIncident).State = EntityState.Modified;
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

        public void InsertIncident(ITollIncident newIncident)
        {
            context.Incidents.Add(Mapper.Map<ITollIncident, Incident>(newIncident));
        }

        #endregion

        #region Sessions

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
			return Mapper.Map<IList<ITollSession>>(context.Sessions.Where(x => !x.IsSent));
		}

        #endregion

        #region Staff and Users

        public IList<string> ImportNewStaff()
        {
            return context.uspImportNewStaff().ToList();
        }

        public void ImportNewRegisteredUsers()
        {
            context.uspImportNewRegisteredUsers();
        }

        public IList<string> GetNewFrequentUsersCreated()
        {
            return context.MappingRegisteredUsers.Where(x => !x.Reported).Select(x => x.Identifier).ToList();
        }

        public void SetFrequentUserMappingAsReported(IList<string> freqUserMapping)
        {
            foreach (var identifier in freqUserMapping)
            {
                context.uspSetRegisteredMappingReported(identifier);
            }
        }

        #endregion

        #region Audit

        public IList<ITollHourlyAudit> GetNextHourlyAuditBatch(DateTime? date = null)
        {
            var returnList = new List<ITollHourlyAudit>();
            var batch = context.uspGetNextHourlyAuditBatch(date).ToList();

            foreach (var batchResult in batch)
                returnList.Add(batchResult);

            return returnList;
        }

        public void InsertAudit(ITollHourlyAudit newAudit)
        {
            context.Audits.Add(Mapper.Map<ITollHourlyAudit, Audit>(newAudit));
        }

        #endregion

        #region Other

        public IList<ITollStaffLogin> GetUnsentStaffLogins()
        {
            var sls = context.StaffLogins.Where(x => !x.IsSent);
            return Mapper.Map<IList<ITollStaffLogin>>(sls);
        }

        public IList<ILaneStatus> GetLaneStatuses()
        {
            var laneStatuses = context.uspGetLaneAliveStatus();
            return Mapper.Map<IList<ILaneStatus>>(laneStatuses);
        }

        #endregion

        #region MIS Update

        public IList<IMISAccountBalanceUpdate> GetListOfMISAccountBalanceUpdates()
        {
            context.uspUpdateStagingHotlist();

            var balanceUpdates = context.StagingMISAccountBalanceUpdates.Where(x => !x.DateSentToMIS.HasValue).OrderBy(x => x.DateCreated);
            return Mapper.Map<IList<IMISAccountBalanceUpdate>>(balanceUpdates);
        }

        public void SetSentMISAccountBalanceUpdate(IMISAccountBalanceUpdate update)
        {
            using (var ctx = new DataImportEntities())
            {
                var balanceRecord = ctx.StagingMISAccountBalances.FirstOrDefault(x => x.MISAccountNr == update.MISAccountNr);

                if (balanceRecord != null)
                {
                    balanceRecord.MISBalance = update.NewBalance;
                    ctx.SaveChanges();
                }

                var updateRecord = ctx.StagingMISAccountBalanceUpdates.FirstOrDefault(x => x.Id == update.Id);

                if (updateRecord != null)
                {
                    updateRecord.DateSentToMIS = DateTime.Now;
                    ctx.SaveChanges();
                }
            }                
        }

        public IList<IMISHotlistUpdate> GetListOfMISHotlistUpdates()
        {
            context.uspUpdateStagingHotlist();

            var balanceUpdates = context.StagingMISHotlistUpdates.Where(x => !x.DateSentToMIS.HasValue);
            return Mapper.Map<IList<IMISHotlistUpdate>>(balanceUpdates);
        }

        public void SetSentMISHolistUpdate(IMISHotlistUpdate update)
        {
            using (var ctx = new DataImportEntities())
            {
                var updateRecord = ctx.StagingMISHotlistUpdates.FirstOrDefault(x => x.Id == update.Id);

                if (updateRecord != null)
                {
                    updateRecord.DateSentToMIS = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
        }

        public IList<IMISValidationListUpdate> GetListOfMISValidationListUpdates()
        {
            context.uspUpdateStagingValidationlist();

            var vlUpdates = context.StagingMISValidationListUpdates.Where(x => !x.DateSentToMIS.HasValue);
            return Mapper.Map<IList<IMISValidationListUpdate>>(vlUpdates);
        }

        public void SetSentMISValidationUpdate(IMISValidationListUpdate update)
        {
            using (var ctx = new DataImportEntities())
            {
                var updateRecord = ctx.StagingMISValidationListUpdates.FirstOrDefault(x => x.Id == update.Id);

                if (updateRecord != null)
                {
                    updateRecord.DateSentToMIS = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
        }

		public void QueueRequestedData(RequestedDataType type, string laneCode, List<string> sequenceNumbers)
		{
			using (var ctx = new DataImportEntities())
			{
				foreach (var sequenceNumber in sequenceNumbers)
				{
					if(!ctx.DataRequests.Any(x => x.LaneCode == laneCode && x.SequenceNumber == sequenceNumber && x.Type == (int)type))
						ctx.DataRequests.Add(new DataRequest { LaneCode = laneCode, SequenceNumber = sequenceNumber, Type = (int)type });
				}

				ctx.SaveChanges();
			}
		}

		#endregion
	}
}
