using Intertoll.EFDbContext.GenericWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        #endregion        
    }
}
