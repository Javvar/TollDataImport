using Intertoll.NLogger;
using System.ServiceProcess;

namespace Intertoll.DataImport.Database.Sync
{
    public partial class WinService : ServiceBase
    {
        public WinService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.LogInfoMessage("[Enter] Starting Service");

            DatabaseSyncManager.StartSynchingProcess();

            Log.LogInfoMessage("[Exit] Starting Service");
        }

        protected override void OnStop()
        {
            Log.LogInfoMessage("[Enter] Stopping Service");


            Log.LogInfoMessage("[Exit] Stopping Service");
        }

        #region Debug

        internal void DebugStart()
        {
            OnStart(new string[] { });
        }

        internal void DebugStop()
        {
            OnStop();
        }

        #endregion
    }
}
