using System.Configuration;
using System.IO;
using System.Reflection;

namespace Intertoll.DataImport.Database.Sync
{
    public class AppSettings 
    {
        public static string IdentifiersDecryptionUtilityLocation
        {
            get { return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "AccountIdentifiers"); }
        }

        public static bool CheckDuplicatesOnExistingData
        {
            get { return bool.TryParse(ConfigurationManager.AppSettings["CheckDuplicatesOnExistingData"],out var outVar) ? outVar : false; }
        }

        public static int TransactionSelectBatchSize
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["TransactionSelectBatchSize"], out var outVar) ? outVar : 1; }
        }

        public static int ETCTransactionSelectBatchSize
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["ETCTransactionSelectBatchSize"], out var outVar) ? outVar : 1; }
        }

        public static int IncidentSelectBatchSize
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["IncidentSelectBatchSize"], out var outVar) ? outVar : 1; }
        }

        public static int TimesliceSelectBatchSize
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["TimesliceSelectBatchSize"], out var outVar) ? outVar : 1; }
        }

        public static int TransactionsIntervalInSeconds
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["TransactionsIntervalInSeconds"], out var outVar) ? outVar : 1; }
        }

        public static int IncidentsIntervalInSeconds
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["IncidentsIntervalInSeconds"], out var outVar) ? outVar : 1; }
        }

        public static int ETCTransactionsIntervalInSeconds
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["ETCTransactionsIntervalInSeconds"], out var outVar) ? outVar : 1; }
        }

        public static int TimeslicesIntervalInSeconds
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["TimeslicesIntervalInSeconds"], out var outVar) ? outVar : 1; }
        }

        public static int RegisteredAccountsIntervalInSeconds
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["RegisteredAccountsIntervalInSeconds"], out var outVar) ? outVar : 1; }
        }

        public static int RegisteredAccountBatchSize
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["RegisteredAccountBatchSize"], out var outVar) ? outVar : 1; }
        }

        public static int RegisteredAccountDetailsBatchSize
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["RegisteredAccountDetailsBatchSize"], out var outVar) ? outVar : 1; }
        }

        public static int RegisteredAccountUsersBatchSize
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["RegisteredAccountUsersBatchSize"], out var outVar) ? outVar : 1; }
        }

        public static string MISDBConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["MISDBConnectionString"] ??
                  "Host=tongaat;Server=tongaat_tcp;Service=2000;Protocol=onsoctcp;UID=informix;Password=informix123;Database=tongaat;";
            }
        }

        public static string EncryptionDecryptionApplication
        {
            get { return ConfigurationManager.AppSettings["EncryptionDecryptionApplication"] ?? "EncryptDecrypt.exe"; }
        }
    }
}
