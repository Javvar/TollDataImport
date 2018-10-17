﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intertoll.DataImport.Data._40
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DataImportEntities : DbContext
    {
        public DataImportEntities()
            : base("name=DataImportEntities")
        {
    		Database.CommandTimeout = 180;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<StaffLogin> StaffLogins { get; set; }
        public virtual DbSet<Incident> Incidents { get; set; }
        public virtual DbSet<Audit> Audits { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<StagingIncident> StagingIncidents { get; set; }
    
        public virtual ObjectResult<uspGetLaneSession_Result> uspGetLaneSession(string laneCode)
        {
            var laneCodeParameter = laneCode != null ?
                new ObjectParameter("laneCode", laneCode) :
                new ObjectParameter("laneCode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<uspGetLaneSession_Result>("uspGetLaneSession", laneCodeParameter);
        }
    
        public virtual int uspGetSessionFollowingSession(string laneCode, Nullable<System.DateTime> transactionTime)
        {
            var laneCodeParameter = laneCode != null ?
                new ObjectParameter("laneCode", laneCode) :
                new ObjectParameter("laneCode", typeof(string));
    
            var transactionTimeParameter = transactionTime.HasValue ?
                new ObjectParameter("transactionTime", transactionTime) :
                new ObjectParameter("transactionTime", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("uspGetSessionFollowingSession", laneCodeParameter, transactionTimeParameter);
        }
    
        public virtual int uspGetSessionPrecedingSession(string laneCode, Nullable<System.DateTime> transactionTime)
        {
            var laneCodeParameter = laneCode != null ?
                new ObjectParameter("laneCode", laneCode) :
                new ObjectParameter("laneCode", typeof(string));
    
            var transactionTimeParameter = transactionTime.HasValue ?
                new ObjectParameter("transactionTime", transactionTime) :
                new ObjectParameter("transactionTime", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("uspGetSessionPrecedingSession", laneCodeParameter, transactionTimeParameter);
        }
    
        public virtual ObjectResult<uspGetIncidentBatchGreaterThanTime_Result> uspGetIncidentBatchGreaterThanTime(Nullable<System.DateTime> dateFrom)
        {
            var dateFromParameter = dateFrom.HasValue ?
                new ObjectParameter("dateFrom", dateFrom) :
                new ObjectParameter("dateFrom", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<uspGetIncidentBatchGreaterThanTime_Result>("uspGetIncidentBatchGreaterThanTime", dateFromParameter);
        }
    
        public virtual ObjectResult<uspGetTransactionBatchGreaterThanTime_Result> uspGetTransactionBatchGreaterThanTime(Nullable<System.DateTime> dateFrom)
        {
            var dateFromParameter = dateFrom.HasValue ?
                new ObjectParameter("dateFrom", dateFrom) :
                new ObjectParameter("dateFrom", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<uspGetTransactionBatchGreaterThanTime_Result>("uspGetTransactionBatchGreaterThanTime", dateFromParameter);
        }
    
        public virtual ObjectResult<uspGetNextIncidentBatch_Result> uspGetNextIncidentBatch()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<uspGetNextIncidentBatch_Result>("uspGetNextIncidentBatch");
        }
    
        public virtual ObjectResult<uspGetNextHourlyAuditBatch_Result> uspGetNextHourlyAuditBatch(Nullable<System.DateTime> date)
        {
            var dateParameter = date.HasValue ?
                new ObjectParameter("date", date) :
                new ObjectParameter("date", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<uspGetNextHourlyAuditBatch_Result>("uspGetNextHourlyAuditBatch", dateParameter);
        }
    
        public virtual ObjectResult<uspGetLaneAliveStatus_Result> uspGetLaneAliveStatus()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<uspGetLaneAliveStatus_Result>("uspGetLaneAliveStatus");
        }
    
        public virtual ObjectResult<string> uspImportNewStaff()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("uspImportNewStaff");
        }
    
        public virtual ObjectResult<uspGetNextIncidentBatch_Result> uspGetIncident(string laneCode, Nullable<int> sequenceNumber)
        {
            var laneCodeParameter = laneCode != null ?
                new ObjectParameter("LaneCode", laneCode) :
                new ObjectParameter("LaneCode", typeof(string));
    
            var sequenceNumberParameter = sequenceNumber.HasValue ?
                new ObjectParameter("SequenceNumber", sequenceNumber) :
                new ObjectParameter("SequenceNumber", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<uspGetNextIncidentBatch_Result>("uspGetIncident", laneCodeParameter, sequenceNumberParameter);
        }
    
        public virtual ObjectResult<uspGetNextTransactionBatch_Result> uspGetTransaction(string laneCode, Nullable<int> sequenceNumber)
        {
            var laneCodeParameter = laneCode != null ?
                new ObjectParameter("LaneCode", laneCode) :
                new ObjectParameter("LaneCode", typeof(string));
    
            var sequenceNumberParameter = sequenceNumber.HasValue ?
                new ObjectParameter("SequenceNumber", sequenceNumber) :
                new ObjectParameter("SequenceNumber", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<uspGetNextTransactionBatch_Result>("uspGetTransaction", laneCodeParameter, sequenceNumberParameter);
        }
    
        public virtual ObjectResult<uspGetNextTransactionBatch_Result> uspGetNextTransactionBatch()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<uspGetNextTransactionBatch_Result>("uspGetNextTransactionBatch");
        }
    }
}
