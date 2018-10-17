using System;
using System.Configuration;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Intertoll.PCS.DataIntergrationService.Managers
{
    public class AppSettingsManager
    {
        public static DateTime GetEndOfDayTaskTime
        {
            get
            {
                return DateTime.Parse(ConfigurationManager.AppSettings["EndOfDayTaskTime"]);
            }
        }

        public static int GetHourlyAuditInterval
        {
            get
            {
                var AppSetting = Int32.Parse(ConfigurationManager.AppSettings["HourlyAuditInterval"]);
                return AppSetting;
            }
        }

        public static DateTime? GetHourlyAuditDate
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["HourlyAuditDate"]))
                    return null;

                var AppSetting = DateTime.Parse(ConfigurationManager.AppSettings["HourlyAuditDate"]);
                return AppSetting;
            }
        }
        
        public static string DataAggregationQueueName
        {
            get
            {
                return ConfigurationManager.AppSettings["DataAggregationQueueName"];
            }
        }       

        public static Int32 CommsUpdateInterval
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["CommsUpdateInterval"]);
            }
        }

        public static Int32 SendAliveMessageInterval
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["SendAliveMessageInterval"]);
            }
        }

        public static Int32 GetShiftsAndLoginsInterval
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["GetShiftsAndLoginsInterval"]);
            }
        }

        public static Int32 GetProcessTransactionsAndIncidentsInterval
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["ProcessTransactionsAndIncidentsInterval"]);
            }
        }

        public static DateTime HourlyAuditTime
        {
            get
            {
                return DateTime.Parse(ConfigurationManager.AppSettings["HourlyAuditTime"]);
            }
        }

        public static DateTime? TransactionStartDate
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["TransactionStartDate"]))
                    return null;

                return DateTime.Parse(ConfigurationManager.AppSettings["TransactionStartDate"]);
            }
        }

        public static DateTime? IncidentStartDate
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IncidentStartDate"]))
                    return null;

                return DateTime.Parse(ConfigurationManager.AppSettings["IncidentStartDate"]);
            }
        }

        public static string NotificationsMailServer
        {
            get
            {
                return ConfigurationManager.AppSettings["NotificationsMailServer"];
            }
        }
        public static string EmailNotificationFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailNotificationFormat"];
            }
        }
        public static string ServiceSender
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceSender"];
            }
        }
        public static string NotificationList
        {
            get
            {
                return ConfigurationManager.AppSettings["NotificationList"];
            }
        }

        public static Int32 CommsRetryInterval
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["CommsRetryInterval"]);
            }
        }

        public static string ServiceIP
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceIP"];
            }
        }
    }
}
