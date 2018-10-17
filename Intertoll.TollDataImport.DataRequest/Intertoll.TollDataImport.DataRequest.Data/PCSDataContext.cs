using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intertoll.TollDataImport.DataRequest.Data.Entities;
using Intertoll.TollDataImport.DataRequest.Data.Model;


namespace Intertoll.TollDataImport.DataRequest.Data
{
    public class PCSDataContext : BaseDataContext
    {
        private DbContext _Context;
        protected override DbContext Context
        {
            get { return _Context ?? (_Context = new PCSDBEntities()); }
        }

        private GenericRepository<PCSAudit> _Audits;
        public GenericRepository<PCSAudit> Audits
        {
            get { return _Audits ?? (_Audits = new GenericRepository<PCSAudit>(Context)); }
        }

        private GenericRepository<PCSTransaction> _Transactions;
        public GenericRepository<PCSTransaction> Transactions
        {
            get { return _Transactions ?? (_Transactions = new GenericRepository<PCSTransaction>(Context)); }
        }

        private GenericRepository<PCSIncident> _Incidents;
        public GenericRepository<PCSIncident> Incidents
        {
            get { return _Incidents ?? (_Incidents = new GenericRepository<PCSIncident>(Context)); }
        }
        private GenericRepository<PCSStaffLogin> _StaffLogins;
        public GenericRepository<PCSStaffLogin> StaffLogins
        {
            get { return _StaffLogins ?? (_StaffLogins = new GenericRepository<PCSStaffLogin>(Context)); }
        }
    }
}
