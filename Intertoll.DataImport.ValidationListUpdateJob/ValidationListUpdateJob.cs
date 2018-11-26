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

namespace Intertoll.DataImport.ValidationListUpdateJob
{
    [DisallowConcurrentExecution]
    public class ValidationListUpdateJob : BaseSchedulable<ValidationListUpdateJob>
    {
        ITollDataProvider DataProvider;
        ISettingsProvider Settings { get; set; }

        public ValidationListUpdateJob(ITollDataProvider _dataProvider, ISettingsProvider _settings)
        {
            DataProvider = _dataProvider;
            Settings = _settings;
        }

        public override void Execute(IJobExecutionContext context)
        {
            Log.LogTrace("[Enter]" + JobName);

            try
            {
                var updates = DataProvider.GetListOfMISValidationListUpdates();

                using (IfxConnection conn = EstablishConnection())
                {
                    RemoveDeletedUpdates(updates.Where(x => x.Action == "Delete"), conn);
                    InsertNewUpdates(updates.Where(x => x.Action == "Add"), conn);
                    UpdateMISValidationList(updates.Where(x => x.Action == "Update"), conn);
                }                                    

                foreach (var update in updates)
                {
                    
                    //DataProvider.SetSentMISValidationUpdate(update);
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }

            Log.LogTrace("[Exit]" + JobName);
        }

        private void RemoveDeletedUpdates(IEnumerable<IMISValidationListUpdate> deletedItems, IfxConnection conn)
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                IfxCommand command = conn.CreateCommand();

                string deleteList = "()";
                command.CommandText = $"DELETE VL WHERE IDVL IN {deleteList}";
            }
            catch (Exception ex)
            {
                Log.LogTrace(ex.Message + ". Check error log for more details.");
                Log.LogException(ex);
            }

            Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        private void InsertNewUpdates(IEnumerable<IMISValidationListUpdate> addedItems, IfxConnection conn)
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                IfxCommand command = conn.CreateCommand();

                var insertItems = "()";
                command.CommandText = $"INSERT INTO VL VALUES {insertItems}";
            }
            catch (Exception ex)
            {
                Log.LogTrace(ex.Message + ". Check error log for more details.");
                Log.LogException(ex);
            }

            Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        private void UpdateMISValidationList(IEnumerable<IMISValidationListUpdate> updatedItems, IfxConnection conn)
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                foreach (var update in updatedItems)
                {
                    IfxCommand command = conn.CreateCommand();
                    command.CommandText = $"UPDATE VL SET Balance = Balance WHERE ac_nr = ac_nr ";
                }
            }
            catch (Exception ex)
            {
                Log.LogTrace(ex.Message + ". Check error log for more details.");
                Log.LogException(ex);
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
