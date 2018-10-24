using Intertoll.Mail;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using MailKit.Security;
using Unity;

namespace Intertoll.DataImport.Mail
{
    public class MailClient : IMailClient
    {
        IUnityContainer Container;
        ISettingsProvider Settings { get; set; }

        public MailClient(IUnityContainer _container, ISettingsProvider _settings)
        {
            Container = _container;
            Settings = _settings;
        }

        public void SendMessage<T>(string subject, string message) where T : IMailFormatter
        {
            //Log.LogInfoMessage(message);

            //var Formatter = Container.Resolve<T>();
            //var FormattedMessage = Formatter.Format(message, '|');

            //using (var client = new SMTPMailClient(Settings.ServiceNotificationsMailServer, 25, SecureSocketOptions.None))
            //{
            //    client.SendHtmlMessage(Settings.ServiceServiceSender, Settings.ServiceNotificationList,subject, FormattedMessage);
            //}
        }
    }
}
