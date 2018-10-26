using IBM.Data.Informix;
using Intertoll.DataImport.Schedulable;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Quartz;
using System;

namespace Intertoll.DataImport.HotlistUpdateJob
{
    [DisallowConcurrentExecution]
    public class HotlistUpdateJob : BaseSchedulable<HotlistUpdateJob>
    {
        ITollDataProvider DataProvider;
        ISettingsProvider Settings { get; set; }

        public HotlistUpdateJob(ITollDataProvider _dataProvider, ISettingsProvider _settings)
        {
            DataProvider = _dataProvider;
            Settings = _settings;
        }

        public override void Execute(IJobExecutionContext context)
        {
            Log.LogTrace("[Enter]" + JobName);

            try
            {
                var hotlistUpdates = DataProvider.GetListOfMISHotlistUpdates();

                foreach (var update in hotlistUpdates)
                {
                    UpdateMISHotlist(update);
                    DataProvider.SetSentMISHolistUpdate(update);
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }

            Log.LogTrace("[Exit]" + JobName);
        }

        private void UpdateMISHotlist(IMISHotlistUpdate update)
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                //using (IfxConnection connection = EstablishConnection())
                //{
                //    IfxCommand command = connection.CreateCommand();

                //    var EncryptedCarNr = update.CardNr;

                //    if (update.Change == "Add")
                //    {
                        
                //        command.CommandText = $"INSERT INTO Hotlist VALUES ('{EncryptedCarNr}') ";
                //    }
                //    else if(update.Change == "Delete")
                //    {
                //        command.CommandText = $"DELETE FROM Hotlist WHERE ac_nr = '{EncryptedCarNr}'";
                //    }                    
                //}
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }

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
