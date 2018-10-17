using System;
using System.Data.Entity;
using System.Linq;
using Intertoll.TollDataImport.DataRequest.Data.Entities;
using Intertoll.TollDataImport.DataRequest.Data.Model;


namespace Intertoll.TollDataImport.DataRequest.Data
{
    public class PCSStagingDataContext : BaseDataContext
    {
        private DbContext _Context;
        protected override DbContext Context
        {
            get { return _Context ?? (_Context = new PCSSVStagingEntities()); }
        }

        #region Repositories

        private GenericRepository<Transaction> _Transactions;
        public GenericRepository<Transaction> Transactions
        {
            get { return _Transactions ?? (_Transactions = new GenericRepository<Transaction>(Context)); }
        }

        private GenericRepository<StaffIdToGuidMapping> _StaffMappings;
        public GenericRepository<StaffIdToGuidMapping> StaffMappings
        {
            get { return _StaffMappings ?? (_StaffMappings = new GenericRepository<StaffIdToGuidMapping>(Context)); }
        }

        private GenericRepository<Session> _Sessions;
        public GenericRepository<Session> Sessions
        {
            get { return _Sessions ?? (_Sessions = new GenericRepository<Session>(Context)); }
        }

        private GenericRepository<StaffLogin> _StaffLogins;
        public GenericRepository<StaffLogin> StaffLogins
        {
            get { return _StaffLogins ?? (_StaffLogins = new GenericRepository<StaffLogin>(Context)); }
        }

        private GenericRepository<PaymentGroupsMapping> _PaymentGroupsMappings;
        public GenericRepository<PaymentGroupsMapping> PaymentGroupsMappings
        {
            get { return _PaymentGroupsMappings ?? (_PaymentGroupsMappings = new GenericRepository<PaymentGroupsMapping>(Context)); }
        }

        private GenericRepository<PaymentMechMapping> _PaymentMechMappings;
        public GenericRepository<PaymentMechMapping> PaymentMechMappings
        {
            get { return _PaymentMechMappings ?? (_PaymentMechMappings = new GenericRepository<PaymentMechMapping>(Context)); }
        }

        private GenericRepository<PaymentMethodsMapping> _PaymentMethodsMappings;
        public GenericRepository<PaymentMethodsMapping> PaymentMethodsMappings
        {
            get { return _PaymentMethodsMappings ?? (_PaymentMethodsMappings = new GenericRepository<PaymentMethodsMapping>(Context)); }
        }

        private GenericRepository<PaymentTypesMapping> _PaymentTypesMappings;
        public GenericRepository<PaymentTypesMapping> PaymentTypesMappings
        {
            get { return _PaymentTypesMappings ?? (_PaymentTypesMappings = new GenericRepository<PaymentTypesMapping>(Context)); }
        }

        private GenericRepository<TariffMapping> _TariffMappings;
        public GenericRepository<TariffMapping> TariffMappings
        {
            get { return _TariffMappings ?? (_TariffMappings = new GenericRepository<TariffMapping>(Context)); }
        }

        private GenericRepository<StaffIdToGuidMapping> _StaffIdToGuidMappings;
        public GenericRepository<StaffIdToGuidMapping> StaffIdToGuidMappings
        {
            get { return _StaffIdToGuidMappings ?? (_StaffIdToGuidMappings = new GenericRepository<StaffIdToGuidMapping>(Context)); }
        }
        
        private GenericRepository<Incident> _Incidents;
        public GenericRepository<Incident> Incidents
        {
            get { return _Incidents ?? (_Incidents = new GenericRepository<Incident>(Context)); }
        }


        #endregion

        #region Methods

        public uspGetTransactionByLaneAndSeq_Result GetTransactionByTransactionSeqNumber(int LaneID, int TransactionSeqNr)
        {
            return ((PCSSVStagingEntities)Context).uspGetTransactionByLaneAndSeq(LaneID, TransactionSeqNr).FirstOrDefault();
        }

        public uspGetIncidentByLaneAndSeq_Result GetIncidentByIncidentSeqNumber(int LaneID, int IncidentSeqNr)
        {
            return ((PCSSVStagingEntities)Context).uspGetIncidentByLaneAndSeq(LaneID, IncidentSeqNr).FirstOrDefault();
        }

        public uspGetAuditByLaneAndHour_Result GetLaneAudit(int LaneID, int Hour, DateTime? Date = null)
        {
            return ((PCSSVStagingEntities)Context).uspGetAuditByLaneAndHour(LaneID, Date, Hour).FirstOrDefault();
        }

        public uspAccountUserDetailsGet_Result GetAccountUserDetails(string PAN)
        {
            return ((PCSSVStagingEntities)Context).uspAccountUserDetailsGet(PAN).FirstOrDefault();
        }

        public System.Collections.Generic.List<uspIncidentTypesGet_Result> GeIncidentTypes()
        {
            return ((PCSSVStagingEntities)Context).uspIncidentTypesGet().ToList();
        }

        #endregion
    }
}
