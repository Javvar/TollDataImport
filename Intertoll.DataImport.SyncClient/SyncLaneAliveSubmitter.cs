using System;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;

namespace Intertoll.DataImport.SyncClient
{
    public class SyncLaneAliveSubmitter : ILaneAliveSubmitter
    {
        public ITollDataProvider DataProvider { get; set; }

        public void Submit(Guid LaneGuid)
        {
            try
            {
                Sync.Client.SyncClient.SubmitAlive(LaneGuid);
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }
        }
    }
}
