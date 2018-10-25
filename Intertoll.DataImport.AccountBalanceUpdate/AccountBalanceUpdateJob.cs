using Intertoll.DataImport.Schedulable;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intertoll.DataImport.AccountBalanceUpdateJob
{
    [DisallowConcurrentExecution]
    public class AccountBalanceUpdateJob : BaseSchedulable<AccountBalanceUpdateJob>
    {
        ITollDataProvider DataProvider;
        ISettingsProvider Settings { get; set; }

        public AccountBalanceUpdateJob(ITollDataProvider _dataProvider,ISettingsProvider _settings)
        {
            DataProvider = _dataProvider;
            Settings = _settings;
        }

        public override void Execute(IJobExecutionContext context)
        {
            Log.LogTrace("[Enter]" + JobName);

            try
            {
                var balanceUpdates = DataProvider.GetListOfMISAccountBalanceUpdates();

                foreach (var update in balanceUpdates)
                {
                    UpdateMISBalance();
                    DataProvider.SetSentMISAccountBalanceUpdate(update);
                }                
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }

            Log.LogTrace("[Exit]" + JobName);
        }

        private void UpdateMISBalance()
        {
            
        }
    }
}
