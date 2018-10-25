using IBM.Data.Informix;
using Intertoll.DataImport.Schedulable;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
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
                    UpdateMISBalance(update);
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

        private void UpdateMISBalance(IMISAccountBalanceUpdate update)
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            //try
            //{
            //    using (IfxConnection connection = EstablishConnection())
            //    {
            //        IfxCommand command = connection.CreateCommand();
            //        command.CommandText = $"UPDATE AccBalances SET Balance = {update.NewBalance} WHERE ac_nr = {update.MISAccountNr} ";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Log.LogException(ex);
            //}

            Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        private IfxConnection EstablishConnection()
        {
            try
            {
                var connection = new IfxConnection(Settings.MISDBConnectionString);
                connection.DatabaseLocale = "en_US.CP1252";
                connection.ClientLocale = "en_US.CP1252";
                connection.Open();

                return connection;
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");

                return null;
            }
        }
    }
}
