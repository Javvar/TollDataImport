using System;
using System.Collections.Generic;
using System.Linq;
using Intertoll.EFDbContext.GenericWrapper;

namespace Intertoll.DataImport.Database.Sync.Data.DataContext
{
    public class DatabaseSyncDataContext : BaseDataContext<DatabaseSyncEntities>
    {
        static DatabaseSyncDataContext()
        {
        }

        #region Repositories

        private IGenericRepository<StagingIncident> importedIncidents;
        public IGenericRepository<StagingIncident> ImportedIncidents
        {
            get { return importedIncidents ?? (importedIncidents = new GenericRepository<StagingIncident>(context)); }
        }

        private IGenericRepository<StagingTransaction> importedTransactions;
        public IGenericRepository<StagingTransaction> ImportedTransactions
        {
            get { return importedTransactions ?? (importedTransactions = new GenericRepository<StagingTransaction>(context)); }
        }

        private IGenericRepository<StagingETCTransaction> importedETCTransactions;
        public IGenericRepository<StagingETCTransaction> ImportedETCTransactions
        {
            get { return importedETCTransactions ?? (importedETCTransactions = new GenericRepository<StagingETCTransaction>(context)); }
        }

        private IGenericRepository<StagingTimeSlice> importedTimeslices;
        public IGenericRepository<StagingTimeSlice> ImportedTimeslices
        {
            get { return importedTimeslices ?? (importedTimeslices = new GenericRepository<StagingTimeSlice>(context)); }
        }

        private IGenericRepository<StagingAccount> importedAccounts;
        public IGenericRepository<StagingAccount> ImportedAccounts
        {
            get { return importedAccounts ?? (importedAccounts = new GenericRepository<StagingAccount>(context)); }
        }

        private IGenericRepository<StagingAccountDetail> importedAccountDetails;
        public IGenericRepository<StagingAccountDetail> ImportedAccountDetails
        {
            get { return importedAccountDetails ?? (importedAccountDetails = new GenericRepository<StagingAccountDetail>(context)); }
        }

        private IGenericRepository<StagingAccountIdentifier> importedAccountIdentifiers;
        public IGenericRepository<StagingAccountIdentifier> ImportedAccountIdentifiers
        {
            get { return importedAccountIdentifiers ?? (importedAccountIdentifiers = new GenericRepository<StagingAccountIdentifier>(context)); }
        }

        #endregion

	    public List<uspGetListOfSequenceNrGaps_Result> GetTransactionSequenceNrGaps(DateTime from,DateTime to)
	    {
		    return context.uspGetListOfSequenceNrGaps(from,to).ToList();
	    }
    }
}
