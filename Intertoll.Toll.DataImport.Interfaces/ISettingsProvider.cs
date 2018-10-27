namespace Intertoll.Toll.DataImport.Interfaces
{
    public interface ISettingsProvider
    {
        #region Email

        string ServiceNotificationsMailServer { get;  }
        string ServiceServiceSender { get;  }
        string ServiceNotificationList { get; }
        string TransactionsCardDecryptionUtilityLocation { get; }
        string HotListCardEncryptionUtilityLocation { get; }
        string EncryptionKey { get; }
        string MISDBConnectionString { get; } 
        string EncryptionDecryptionApplication { get; }

        #endregion
    }
}
