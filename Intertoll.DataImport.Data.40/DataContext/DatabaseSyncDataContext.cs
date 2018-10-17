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

        #endregion
    }
}
