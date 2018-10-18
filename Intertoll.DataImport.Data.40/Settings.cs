using System.Configuration;

namespace Intertoll.DataImport.Database.Sync.Data
{
    public class AppSettings 
    {
        public static bool CheckDuplicatesOnExistingData
        {
            get { return bool.TryParse(ConfigurationManager.AppSettings["CheckDuplicatesOnExistingData"],out var outVar) ? outVar : false; }
        }

        public static int TransactionSelectBatchSize
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["TransactionSelectBatchSize"], out var outVar) ? outVar : 100; }
        }

        public static int ETCTransactionSelectBatchSize
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["ETCTransactionSelectBatchSize"], out var outVar) ? outVar : 100; }
        }

        public static int IncidentSelectBatchSize
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["IncidentSelectBatchSize"], out var outVar) ? outVar : 100; }
        }

        public static int TimesliceSelectBatchSize
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["TimesliceSelectBatchSize"], out var outVar) ? outVar : 100; }
        }

        public static int TransactionsIntervalInSeconds
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["TransactionsIntervalInSeconds"], out var outVar) ? outVar : 10; }
        }

        public static int IncidentsIntervalInSeconds
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["IncidentsIntervalInSeconds"], out var outVar) ? outVar : 10; }
        }

        public static int ETCTransactionsIntervalInSeconds
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["ETCTransactionsIntervalInSeconds"], out var outVar) ? outVar : 10; }
        }

        public static int TimeslicesIntervalInSeconds
        {
            get { return int.TryParse(ConfigurationManager.AppSettings["TimeslicesIntervalInSeconds"], out var outVar) ? outVar : 10; }
        }
    }
}
