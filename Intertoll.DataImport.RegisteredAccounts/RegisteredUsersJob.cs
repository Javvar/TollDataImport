using Intertoll.DataImport.Mail;
using Intertoll.DataImport.Schedulable;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Quartz;
using System;
using System.Linq;
using Unity;

namespace Intertoll.DataImport.RegisteredAccountsJob
{
    [DisallowConcurrentExecution]
    public class RegisteredUsersJob : BaseSchedulable<RegisteredUsersJob>
    {
        ITollDataProvider DataProvider;
        IMailClient MailClient;
        ISettingsProvider Settings { get; set; }

        public RegisteredUsersJob(ITollDataProvider _dataProvider, IMailClient _mailClient, ISettingsProvider _settings)
        {
            DataProvider = _dataProvider;
            MailClient = _mailClient;
            Settings = _settings;
        }

        public override void Execute(IJobExecutionContext context)
        {
            Log.LogTrace("[Enter]" + JobName);

            try
            {
                CheckForUnImportedRegisteredUsers();
                SendImportedRegisteredAccounts();
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }

            Log.LogTrace("[Exit]" + JobName);
        }

        public void CheckForUnImportedRegisteredUsers()
        {
            DataProvider.ImportNewRegisteredUsers();            
        }

        public void SendImportedRegisteredAccounts()
        {
            var newIdentifierMappings = DataProvider.GetNewFrequentUsersCreated();

            if (newIdentifierMappings.Any())
                MailClient.SendMessage<NewStaffMailFormatter>("New registered users imported from old system", newIdentifierMappings.Aggregate((x, y) => x + "|" + y));

            DataProvider.SetFrequentUserMappingAsReported(newIdentifierMappings);
        }
    }
}
