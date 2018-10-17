using System;
using AutoMapper;
using Intertoll.Data;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;

namespace Intertoll.DataImport.SyncClient
{
    public class SyncSessionSubmitter : ISessionSubmitter
    {
        public ITollDataProvider DataProvider { get; set; }

        static SyncSessionSubmitter()
        {
            Mapper.CreateMap<ITollSession, Session>();
        }

        public SyncSessionSubmitter(ITollDataProvider _dataProvider)
        {
            DataProvider = _dataProvider;
        }

        public bool Submit(ITollSession session)
        {
            try
            {
                var syncSession = Mapper.Map<ITollSession, Session>(session);
                Sync.Client.SyncClient.SubmitSession(syncSession);

                session.IsSent = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
                return false;
            }
        }
    }
}
