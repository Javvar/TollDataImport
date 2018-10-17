using System;
using AutoMapper;
using Intertoll.NLogger;
using Intertoll.Sync.Common;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Intertoll.Toll.DataImport.Interfaces.EntityBuilders;

namespace Intertoll.DataImport.SyncClient
{
    public class SyncHourlyAuditSubmitter : IHourlyAuditSubmitter
    {
        public ITollDataProvider DataProvider { get; set; }
        public IHourlyAuditEntityBuilder EntityBuilder { get; set; }

        static SyncHourlyAuditSubmitter()
        {
            Mapper.CreateMap<ITollHourlyAudit, Audit>();
        }

        public SyncHourlyAuditSubmitter(ITollDataProvider _dataProvider, IHourlyAuditEntityBuilder _builder)
        {
            DataProvider = _dataProvider;
            EntityBuilder = _builder;
        }

        public bool Submit(ITollHourlyAudit audit)
        {
            var builtAudit = EntityBuilder.Build(audit);

            if (builtAudit != null)
            {
                try
                {
                    var syncAudit = Mapper.Map<ITollHourlyAudit, Audit>(builtAudit);
                    Sync.Client.SyncClient.SubmitAudit(syncAudit, syncAudit.AuditHour);

                    builtAudit.IsSent = true;
                    DataProvider.InsertAudit(builtAudit);

                    return true;
                }
                catch (Exception ex)
                {
                    Log.LogException(ex);
                    Log.LogTrace(ex.Message + ". Check error log for more details.");
                }
            }

            audit.TimeStamp = DateTime.Now;
            DataProvider.InsertAudit(audit);
            
            return false;
        }
    }
}
