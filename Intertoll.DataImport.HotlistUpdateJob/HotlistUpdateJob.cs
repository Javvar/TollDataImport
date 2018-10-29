using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using IBM.Data.Informix;
using Intertoll.DataImport.Schedulable;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Quartz;


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
                var encryptedDic = EncryptIdentifiers(hotlistUpdates.Select(x => x.CardNr).ToList());

                foreach (var update in hotlistUpdates)
                {
                    if(UpdateMISHotlist(update, encryptedDic[update.CardNr]))
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

        private bool UpdateMISHotlist(IMISHotlistUpdate update, string encryptedIdentifier)
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                using (IfxConnection connection = EstablishConnection())
                {
                    IfxCommand command = connection.CreateCommand();

                    if (update.Change == "Add")
                        command.CommandText = $"INSERT INTO Hotlist VALUES ('{encryptedIdentifier}') ";
                    else if (update.Change == "Delete")
                        command.CommandText = $"DELETE FROM Hotlist WHERE ac_nr = '{encryptedIdentifier}'";
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");

                return false;
            }

            Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            return true;
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

        private Dictionary<string, string> EncryptIdentifiers(IList<string> batch)
        {
            Dictionary<string, string> retDic = new Dictionary<string, string>();

            var identifiersFileName = Path.Combine(Settings.HotListCardEncryptionUtilityLocation, "cards_" + batch.GetHashCode() + ".txt");

            if (!batch.Any())
                return retDic;

            File.WriteAllLines(identifiersFileName, batch);

            try
            {
                var process = new Process();
                string fileName = Path.Combine(Settings.HotListCardEncryptionUtilityLocation, Settings.EncryptionDecryptionApplication);
                string param = "/C" + "\" " + fileName + " " + identifiersFileName + " e\"";

                var processStartInfo = new ProcessStartInfo("cmd.exe", param);
                processStartInfo.UseShellExecute = false;
                processStartInfo.WorkingDirectory = Settings.HotListCardEncryptionUtilityLocation;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.CreateNoWindow = true;

                process.StartInfo = processStartInfo;
                process.Start();

                int RetryCount = 120;

                while (!process.HasExited && RetryCount > 0)
                {
                    RetryCount--;
                    Thread.Sleep(1000);
                }

                if (RetryCount > 0)
                {
                    foreach (var clearPAN in batch)
                    {
                        var encryptedPAN = File.ReadAllText(Path.Combine(Settings.HotListCardEncryptionUtilityLocation, "PAN_" + clearPAN + ".txt"));
                        retDic[clearPAN] = Regex.Replace(encryptedPAN, @"\t|\n|\r", "");
                    }
                }

                string filesToDelete = @"PAN_*";
                string[] fileList = Directory.GetFiles(Settings.HotListCardEncryptionUtilityLocation, filesToDelete);

                foreach (string file in fileList)
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }
            finally
            {
                if (!string.IsNullOrEmpty(identifiersFileName) && File.Exists(identifiersFileName))
                    File.Delete(identifiersFileName);
            }

            return retDic;
        }
    }
}
