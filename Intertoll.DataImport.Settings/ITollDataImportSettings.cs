using System.Configuration;
using System.IO;
using System.Reflection;
using Intertoll.Toll.DataImport.Interfaces;

namespace Intertoll.DataImport.Settings
{
    public class TollDataImportSettings : ISettingsProvider
    {
        public string ServiceNotificationsMailServer
        {
            get { return ConfigurationManager.AppSettings["ServiceNotificationsMailServer"]; }
        }

        public string ServiceServiceSender
        {
            get { return ConfigurationManager.AppSettings["ServiceServiceSender"]; }
        }

        public string ServiceNotificationList
        {
            get { return ConfigurationManager.AppSettings["ServiceNotificationList"]; }
        }

        public string TransactionsCardDecryptionUtilityLocation
        {
            get { return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "CardDecryption"); }
        }

        public string HotListCardEncryptionUtilityLocation
        {
            get { return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "CardEncryption"); }
        }

        public string EncryptionKey
        {
            get { return "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"; }
        }

        public string MISDBConnectionString
        {
            get { return ConfigurationManager.AppSettings["MISDBConnectionString"] ?? "Host=tongaat;Server=tongaat_tcp;Service=2000;Protocol=onsoctcp;UID=informix;Password=informix123;Database=tongaat;"; }
        }

        public string EncryptionDecryptionApplication
        {
            get { return ConfigurationManager.AppSettings["EncryptionDecryptionApplication"] ?? "EncryptDecrypt.exe"; }
        }        
    }
}
