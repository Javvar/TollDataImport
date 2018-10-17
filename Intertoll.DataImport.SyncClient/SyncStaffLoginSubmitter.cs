using System;
using AutoMapper;
using Intertoll.Data;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;

namespace Intertoll.DataImport.SyncClient
{
    public class SyncStaffLoginSubmitter : IStaffLoginSubmitter
    {
         public ITollDataProvider DataProvider { get; set; }

        static SyncStaffLoginSubmitter()
        {
            Mapper.CreateMap<ITollStaffLogin, StaffLogin>();
        }

        public SyncStaffLoginSubmitter(ITollDataProvider _dataProvider)
        {
            DataProvider = _dataProvider;
        }

        public bool Submit(ITollStaffLogin staffLogin)
        {
            try
            {
                var syncsl = Mapper.Map<ITollStaffLogin, StaffLogin>(staffLogin);
                Sync.Client.SyncClient.SubmitStaffLogin(syncsl);

                staffLogin.IsSent = true;

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
