using Intertoll.Mail;
using Intertoll.NLogger;
using MailKit.Security;
using System;
using System.Configuration;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Intertoll.PCS.DataIntergrationService.Managers
{
    public static class ServiceAgentManager
    {
        public static void SendReport(string message, int severity)
        {
            try
            {
                var mailSubject = "PCS Data Intergration Service Message";
                switch (severity)
                {
                    case 1:
                        mailSubject = string.Format("PCS Data Intergration Service reported a critical Error");
                        break;
                    case 2:
                        mailSubject = string.Format("PCS Data Intergration Service reported a major Error");
                        break;
                    case 3:
                        mailSubject = string.Format("PCS Data Intergration Service Info Message");
                        break;
                    case 4:
                        mailSubject = string.Format("PCS Data Intergration Service Message");
                        break;
                }
                var messageStr = string.Format(AppSettingsManager.EmailNotificationFormat, message);
                using (var client = new SMTPMailClient(AppSettingsManager.NotificationsMailServer, 25, SecureSocketOptions.None))
                {
                    client.SendHtmlMessage(AppSettingsManager.ServiceSender, AppSettingsManager.NotificationList, mailSubject, messageStr);
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorMessage(ex, "");
            }
        }
    }
}
