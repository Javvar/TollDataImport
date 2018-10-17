namespace Intertoll.Toll.DataImport.Interfaces
{
    public interface ISettingsProvider
    {
        #region Email

        string ServiceNotificationsMailServer { get;  }
        string ServiceServiceSender { get;  }
        string ServiceNotificationList { get; }
        string CardDecryptionUtilityLocation { get; }
        string EncryptionKey { get; }

        #endregion
    }
}
