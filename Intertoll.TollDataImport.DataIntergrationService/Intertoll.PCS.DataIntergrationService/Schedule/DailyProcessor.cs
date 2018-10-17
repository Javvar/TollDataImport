using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Log = Intertoll.NLogger.Log;
using Intertoll.PCS.DataIntergration.Data;
using Intertoll.PCS.DataIntergrationService.Managers;
using NLog;
using System.Security.Cryptography;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Data.Entity.Validation;
using System.Threading;
using System.ServiceProcess;
using Intertoll.Encryption;
using System.Text.RegularExpressions;
using System.Data.Entity;

namespace Intertoll.PCS.DataIntergrationService.Schedule
{
    public class DailyProcessor
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static List<uspIncidentTypesGet_Result> IncidentTypes = LookupManager.IncidentTypes();
        private static List<PaymentGroupsMapping> PaymentGroups = LookupManager.PaymentGroupsMappings();
        private static List<PaymentMechMapping> PaymentMechs = LookupManager.PaymentMechsMapping();
        private static List<PaymentMethodsMapping> PaymentMethods = LookupManager.PaymentMethodsMapping();
        private static List<PaymentTypesMapping> PaymentTypes = LookupManager.PaymentTypesMapping();
        private static List<TariffMapping> Tariffs = LookupManager.TariffMapping();
        private static List<Lane> Lanes = LookupManager.LaneIPAdresses();
        #region Properties

        static object lockObject = new object();

        #endregion

        public static void ExecuteUpdateComms()
        {
            lock (lockObject)
            {
                //This should be for every lane
                UpdateLaneComms();
            }
        }

        public static void ExecuteSendAliveMessage()
        {
            lock (lockObject)
            {
                SendAliveMessage();
            }
        }
                
        public static void ProcessTransactionsAndIncidents()
        {
            lock (lockObject)
            {
                TimeSpan endTime = new TimeSpan(23, 59, 59);
                if (DateTime.Now.TimeOfDay.TotalMinutes > 5 && (endTime - DateTime.Now.TimeOfDay).TotalMinutes > 5)
                {
                    ProcessTransactions();
                    ProcessIncidents();
                    ValidateSessions();
                    ProcessLinkedIncidents();
                    ProcessUnprocessedIncidentsTransactions();
                }
            }
        }

        public static void ProcessHourlyAudits()
        {
            lock (lockObject)
            {
                ProcessHourlyAuditsData();
            }
        }

        public static void ProcessEndOfDayTask()
        {
            lock (lockObject)
            {
                ProcessEndOfDayTasks();
            }
        }

        private static void UpdateLaneComms()
        {
            try
            {
                var context = new PCSDataIntergrationEntities();
                
                foreach (var lane in Lanes)
                {
                    if (NetworkUtility.CanPingHost(lane.LaneIP, Log.LogWarning))
                    { 
                        Intertoll.Sync.Client.SyncClient.SubmitAlive(lane.LaneGuid);
                    }
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        private static void SendAliveMessage()
        {
            try
            {
                Console.WriteLine("Sending Alive Message");
                Intertoll.PCS.TSSUpdater.Client.TSSUpdaterClient.UpdateTimeStatic("LatestPCSDataIntergrationTime", DateTime.Now);
                Console.WriteLine("Sent Alive Message");
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        private static void ProcessEndOfDayTasks()
        {
            try
            {

                Log.LogTrace("ProcessEndOfDayTasks - Start");
                var context = new PCSDataIntergrationEntities();
                var lanes = context.Lanes.ToList();
                foreach (var lane in lanes)
                {
                    var currentSession = context.Sessions.Where(x => x.LaneCode == lane.LaneCode && x.EndDate == null).OrderByDescending(x => x.StartDate).FirstOrDefault();
                    if (currentSession != null)
                    {
                        //var staffLoginGuid = StaffLoginCreate(lane.LaneCode, startDate, null, currentStaffLogin.StaffGUID);

                        //If last transaction is old. close session and not create a new one
                        var lastTransaction = context.Transactions.Where(x => x.SessionGuid == currentSession.SessionGUID).OrderByDescending(x => x.TransactionDate).FirstOrDefault();

                        if (lastTransaction == null)
                        {
                            Log.LogInfoMessage(string.Format("ProcessEndOfDayTasks_SessGuid: {0}_lastTransactionTime: null_ Lane: {1}", currentSession.SessionGUID, lane.LaneCode));
                            var endDate = new DateTime(currentSession.StartDate.Year, currentSession.StartDate.Month, currentSession.StartDate.Day, 23, 59, 59, 0);
                            currentSession.EndDate = endDate;
                            SessionEnd(currentSession);
                        }
                        else
                        {
                            Log.LogInfoMessage(string.Format("ProcessEndOfDayTasks_SessGuid: {0}_lastTransactionTime: {1}_ Lane: {2}", currentSession.SessionGUID, lastTransaction.TransactionStartDate, lane.LaneCode));
                            if (lastTransaction != null && lastTransaction.TransGuid != null && DateTime.Now.Subtract(lastTransaction.TransactionStartDate).Minutes > 30)
                            {
                                currentSession.EndDate = lastTransaction.TransactionStartDate.AddMilliseconds(5);
                                SessionEnd(currentSession);
                            }
                            else
                            {
                                Guid sessionGuid = new Guid();
                                Guid staffLoginGuid = new Guid();
                                DivideEndOfDaySessions(currentSession, out sessionGuid, out staffLoginGuid);
                            }
                        }

                        //TODO: create sessions for closed lanes????

                    }
                }
                Log.LogTrace("ProcessEndOfDayTasks - End");
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                ServiceAgentManager.SendReport("ProcessEndOfDayTasks: " + ex.Message, 1);
            }
        }
        
        private static void DivideEndOfDaySessions(Session currentSession, out Guid nextSessionGuid, out Guid nextStaffLoginGuid)
        {
            var currentEndDate = currentSession.EndDate;
            var context = new PCSDataIntergrationEntities();
            var endDate = new DateTime(currentSession.StartDate.Year, currentSession.StartDate.Month, currentSession.StartDate.Day, 23, 59, 59, 0);
            var nextDay = currentSession.StartDate.AddDays(1);
            var startDate = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0, 0);


            //TODO:
            var dbSession = context.Sessions.FirstOrDefault(x => x.SessionGUID == currentSession.SessionGUID);
            var dbStaffLogin = context.StaffLogins.FirstOrDefault(x => x.StaffLoginGUID == dbSession.StaffLoginGUID);
            dbSession.EndDate = endDate;
            dbStaffLogin.EndDate = endDate;

            context.SaveChanges();
            SessionEnd(dbSession);
            StaffLoginEnd(dbStaffLogin);

            var sessList = context.Sessions.Where(x => x.LaneCode == currentSession.LaneCode && x.SessionSeq > currentSession.SessionSeq).ToList();
            foreach (var sess in sessList)
            {
                sess.SessionSeq = sess.SessionSeq + 1;
            }
            context.SaveChanges();

            //Create Session and staff login
            nextStaffLoginGuid = StaffLoginCreate(currentSession.LaneCode, startDate, currentEndDate, dbStaffLogin.StaffGUID);
            nextSessionGuid = SessionCreate(currentSession.SessionSeq + 1, LookupManager.Lanes(currentSession.LaneCode), nextStaffLoginGuid, startDate, currentEndDate, currentSession.LaneCode);

            //Get all transaction that should be in new session
            var transactionList = context.Transactions.Where(x => x.SessionGuid == currentSession.SessionGUID && x.TransactionStartDate >= startDate);
            foreach (var trans in transactionList)
            {
                trans.Processed = false;
                trans.SessionGuid = nextSessionGuid;
            }

            var incidentList = context.Incidents.Where(x => x.StaffLoginGUID == currentSession.StaffLoginGUID && x.IncidentSetDate >= startDate);
            foreach (var incident in incidentList)
            {
                incident.Processed = false;
                incident.StaffLoginGUID = nextStaffLoginGuid;
            }
            context.SaveChanges();
        }

        private static void ValidateSessions()
        {
            var context = new PCSDataIntergrationEntities();
            var sessionList = context.uspGetSessionsToSlice().ToList();
            Log.LogTrace("uspGetSessionsToSlice: " + sessionList.Count());
            Console.WriteLine("uspGetSessionsToSlice: " + sessionList.Count());
            foreach (var item in sessionList)
            {
                var session = context.Sessions.Where(x => x.SessionGUID == item.SessionGUID).FirstOrDefault();
                if (session == null)
                {
                    Log.LogTrace("session is null: " + item.SessionGUID);
                    Console.WriteLine("session is null: " + sessionList.Count());
                }

                //if (session.EndDate != null)
                //{
                //    var sessList = context.Sessions.Where(x => x.LaneCode == session.LaneCode && x.SessionSeq >= session.SessionSeq).ToList();
                //    foreach (var sess in sessList)
                //    {
                //        sess.SessionSeq = sess.SessionSeq + 1;
                //    }
                //    context.SaveChanges();
                //}
                Guid sessionGuid = new Guid();
                Guid staffLoginGuid = new Guid();
                DivideEndOfDaySessions(session, out sessionGuid, out staffLoginGuid);
                Console.WriteLine("session sliced: " + item.SessionGUID);
            }
            Console.WriteLine("uspGetSessionsToSlice: End");
        }
            
        private static void ProcessTransactions()
        {
            try
            {
                Console.WriteLine("ProcessTransactions - start");
                var context = new PCSDataIntergrationEntities();
                context.Database.CommandTimeout = 180;
                var staffIdGuidList = context.StaffIdToGuidMappings.ToList(); //TODO: SP
                
                var startDate = AppSettingsManager.TransactionStartDate;
                if (startDate == null)
                {
                    var lastTransactionDate = context.Transactions.OrderByDescending(x => x.TransactionStartDate).FirstOrDefault();
                    if (lastTransactionDate != null)
                        startDate = lastTransactionDate.TransactionStartDate;
                    else
                        startDate = DateTime.Today.AddYears(-5);//TODO: clarify

                }
                var transactionsList = context.uspTransactionGet(startDate).ToList();

                Console.WriteLine("transactionsList - " + transactionsList.Count());
                Log.LogTrace("ProcessTransactions, transactionsList: " + transactionsList.Count());  
                
                int i = 1;
                foreach (var trans in transactionsList.ToList())
                {
                    //already processed - ignore
                    var transactionsExist = context.Transactions.Where(x => x.Transaction_Identifier == trans.Transaction_Identifier.ToString()
                        && x.LaneCode == trans.LaneCode
                        ).ToList();

                    if (transactionsExist.Count() > 0)
                    {
                        continue;
                    }

                    //Log.LogTrace("ProcessTransactions no - " + i);
                    Console.WriteLine("ProcessTransactions no - " + i);
                    i++;

                    var staffGuid = staffIdGuidList.Where(x => x.StaffId == trans.CollectorID).FirstOrDefault().StaffGuid;
                    Guid sessionGuid = new Guid();
                    Guid staffLoginGuid = new Guid();

                    GetStaffLoginSession(trans.LaneCode, staffGuid, trans.TransactionDate, out sessionGuid, out staffLoginGuid);

                    Data.Transaction realTransaction = new Data.Transaction();
                    if (!(string.IsNullOrWhiteSpace(trans.FrequentUserPANNumber)))
                    {
                        var accountuser = context.uspAccountUserDetailsGet(trans.FrequentUserPANNumber).FirstOrDefault();
                        if(accountuser != null)
                        {
                            realTransaction.AccountGUID = accountuser.AccountGUID;
                            realTransaction.AccountUserGUID = accountuser.AccountUserGUID;
                        }
                        else
                        {
                            Log.LogTrace(string.Format("FrequentUserPANNumber : {0}, account not found for Transaction: {1} on lane: {2}", trans.FrequentUserPANNumber, trans.Transaction_Identifier, trans.LaneCode));
                            ServiceAgentManager.SendReport(string.Format("FrequentUserPANNumber : {0}, account not found for Transaction: {1} on lane: {2}", trans.FrequentUserPANNumber, trans.Transaction_Identifier, trans.LaneCode), 1);
                        }
                    }

                    //add transaction
                    realTransaction.AppliedClassGUID = trans.CollectorClass.HasValue ? LookupManager.Classes((Int32)trans.CollectorClass) : (Guid?)null;
                    realTransaction.AVCClassGUID = trans.AVCClass.HasValue ? LookupManager.Classes((Int32)trans.AVCClass) : (Guid?)null; 
                    realTransaction.AVCSeqNr = (int)trans.AVCSeqNr;
                    realTransaction.ColClassGUID = trans.CollectorClass.HasValue ? LookupManager.Classes((Int32)trans.CollectorClass) : (Guid?)null;
                    realTransaction.CurrencyGUID = Guid.Parse("942190CD-5053-41B4-9A08-CD472C99ECAF"); 
                    realTransaction.ExchangeRate = Guid.Parse("5A5AE37F-AABC-428D-B4DC-2B4C87B8646C");
                    realTransaction.IsKeyed = trans.IsKeyed.HasValue ? (bool)trans.IsKeyed : false;
                    //realTransaction.LaneStatusGUID = trans.LaneStatus.HasValue ? LookupManager.LaneStatuses((Int32)trans.LaneStatus) : (Guid?)null; 
                    realTransaction.LaneTransSeqNr = (int)(trans.LaneTransSeqNr);
                    realTransaction.LicensePlate = trans.LicensePlate;
                    realTransaction.PaymentDetail = EncryptString(trans.LaneCode, trans.BankCardNumber, GetExpiryDateFromTrack2(trans.BankCardExpiryDate, trans.BankCardNumber));


                    realTransaction.PaymentGroupGUID = PaymentGroups.Where(x => x.PaymentGroupCode == trans.PaymentGroup).FirstOrDefault().PaymentGroupGUID;
                    realTransaction.PaymentMechGUID = PaymentMechs.Where(x => x.PaymentMechCode == trans.PaymentMech).FirstOrDefault().PaymentMechGUID;
                    realTransaction.PaymentMethodGUID = PaymentMethods.Where(x => x.PaymentMethodCode == trans.PaymentMech).FirstOrDefault().PaymentMethodGUID;
                    realTransaction.PaymentTypeGUID = PaymentTypes.Where(x => x.PaymentTypeCode == trans.PaymentType).FirstOrDefault().PaymentTypeGUID;
                    realTransaction.PreviousLicensePlate = null;
                    realTransaction.PreviousPaymentMethodGUID = null;
                    realTransaction.RealClassGUID = null;
                    realTransaction.ReceiptCount = trans.ReceiptCount.HasValue ? (int)trans.ReceiptCount : (int?)null;
                    realTransaction.ReceiptTaxInvoiceDate = trans.ReceiptTaxInvoiceDate;
                    realTransaction.SessionGUID = sessionGuid;
                    realTransaction.TariffAmount = (decimal)trans.TariffAmount;
                    realTransaction.TariffGUID = Tariffs.Where(x => x.VirtualPlazaID == trans.VirtualPlazaID && x.ClassCode == trans.CollectorClass && x.TollTariffPlanID == trans.TollTariffPlanID).FirstOrDefault().TariffGuid;
                    realTransaction.TariffVat = 0;
                    realTransaction.TaxInvNr = trans.TaxInvNr;
                    realTransaction.TotalInLocalCurrency = (decimal)trans.TariffAmount;
                    realTransaction.TransDate = trans.TransactionDate;
                    realTransaction.TransStartDate = trans.TransactionStartDate;
                    realTransaction.TransGUID = Guid.NewGuid();
                    realTransaction.AVCDetail = trans.AvcStatus;
                    
                        //TODO: remove after Testing
                    if (realTransaction.TariffGUID == new Guid() || realTransaction.TariffGUID == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                    {
                        Log.LogTrace(string.Format("VP: {0}; CC: {1}; TTPlanId: {2}; LaneCode: {3};Transaction_Id: {4}", trans.VirtualPlazaID, trans.CollectorClass, trans.TollTariffPlanID, trans.LaneCode, trans.Transaction_Identifier));
                    }
                    if (realTransaction.PaymentGroupGUID == realTransaction.PaymentMechGUID || realTransaction.PaymentMethodGUID == realTransaction.PaymentTypeGUID
                        | realTransaction.PaymentGroupGUID == realTransaction.PaymentMethodGUID || realTransaction.PaymentMechGUID == realTransaction.PaymentTypeGUID)
                    {
                        Log.LogTrace(string.Format("PaymentGroup: {0}; PaymentMech: {1}; PaymentType: {2}; LaneCode: {3};Transaction_Id: {4}", trans.PaymentGroup, trans.PaymentMech, trans.PaymentType, trans.LaneCode, trans.Transaction_Identifier));
                    }
                    
                    //End testing

                    if (!(string.IsNullOrWhiteSpace(trans.ETCTransactionGUID)))
                    {
                        var etcTransaction = new Data.ETCTransaction();
                        var IAuthenticator = new Data.ETCIAuthenticator();
                        var OAuthenticator = new Data.ETCOAuthenticator();

                        IAuthenticator.Authenticator = trans.IssuerAuthenticator_DData;
                        IAuthenticator.AuthenticatorGuid = Guid.NewGuid();
                        IAuthenticator.KeyRef = trans.IssuerAuthenticator_KRef.ToString();
                        IAuthenticator.Nonce = trans.IssuerAuthenticator_MAC; //TODO ??Confirm

                        OAuthenticator.Authenticator = trans.OperatorAuthenticator_DData;
                        OAuthenticator.AuthenticatorGuid = Guid.NewGuid();
                        OAuthenticator.KeyRef = trans.OperatorAuthenticator_KRef.ToString();
                        OAuthenticator.Nonce = trans.OperatorAuthenticator_MAC; //TODO ??Confirm

                        etcTransaction.ClassGUID = LookupManager.Classes((Int32)trans.TagClass);
                        etcTransaction.ContextMarkId = short.Parse(trans.ContextMark);
                        etcTransaction.ETCTransactionGuid = Guid.Parse(trans.ETCTransactionGUID);
                        etcTransaction.ExpiryDate = string.Empty;
                        etcTransaction.IDVL = (long)trans.IDVL;
                        etcTransaction.IssuerAuthenticatorGuid = IAuthenticator.AuthenticatorGuid;
                        etcTransaction.LaneTransSeqNr = (long)trans.LaneTransSeqNr;
                        etcTransaction.LicensePlate = trans.LicensePlate;
                        etcTransaction.OperatorAuthenticatorGuid = OAuthenticator.AuthenticatorGuid;
                        etcTransaction.PAN = long.Parse(trans.PAN);
                        etcTransaction.VehichleState = trans.TagStatus;

                        realTransaction.ETCTransactionGuid = etcTransaction.ETCTransactionGuid;

                        SaveTransaction(realTransaction, etcTransaction, IAuthenticator, OAuthenticator, trans);
                        Intertoll.Sync.Client.SyncClient.SubmitETCTransaction(realTransaction, etcTransaction, IAuthenticator, OAuthenticator);
                                            
                    }
                    else
                    {
                        SaveTransaction(realTransaction, new Data.ETCTransaction(), new Data.ETCIAuthenticator(), new Data.ETCOAuthenticator(), trans);

                        Intertoll.Sync.Client.SyncClient.SubmitTransaction(realTransaction);
                    }
                }

                Console.WriteLine("ProcessTransactions - End");
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogFatal(ex, "Error Processing Transactions" + ex.Message);
            }
        }

        private static void ProcessIncidents()
        {
            try
            {
                Console.WriteLine("ProcessIncidents - start");

                var context = new PCSDataIntergrationEntities();
                context.Database.CommandTimeout = 180;

                var startDate = AppSettingsManager.IncidentStartDate;
                if (startDate == null)
                {
                    var lastIncidentDate = context.Incidents.OrderByDescending(x => x.IncidentSetDate).FirstOrDefault();
                    if (lastIncidentDate != null)
                        startDate = lastIncidentDate.IncidentSetDate;
                    else
                        startDate = DateTime.Today.AddYears(-5);//TODO: clarify
                }

                var incidents = context.uspIncidentsGet(startDate).ToList();
                var staffIdGuidList = context.StaffIdToGuidMappings.ToList();
                //TODO: get for date and if not there user the proc
                Console.WriteLine("uspIncidentsGet count: ", incidents.Count());
                Log.LogTrace("uspIncidentsGet, incidents: " + incidents.Count());   

                int i = 1;
                foreach (var incident in incidents)
                {
                    //already processed - ignore
                    var incidentsExist = context.Incidents.Where(x => x.Incident_Identifier == incident.Incident_Identifier.ToString()
                        && x.LaneCode == incident.LaneCode
                        ).ToList();
                    if (incidentsExist.Count() > 0)
                    {
                        continue;
                    }

                    //If incident code is unknown then ignore
                    var incidentType = IncidentTypes.FirstOrDefault(x => x.Id == incident.IncidentCode);
                    if (incidentType == null)
                    {
                        Log.LogInfoMessage(string.Format("ProcessIncidents: Incident {0} on lane {1} has invalid incident code {2}", incident.Incident_Identifier, incident.LaneCode, incident.IncidentCode));
                        //ServiceAgentManager.SendReport(string.Format("ProcessIncidents: Incident {0} on lane {1} has invalid incident code {2}", incident.Incident_Identifier, incident.LaneCode, incident.IncidentCode), 1);
                        continue;
                    }
                    Console.WriteLine("Process Incidents no - "+ i);
                    i++;

                    var staffGuid = staffIdGuidList.Where(x => x.StaffId == incident.CollectorID).FirstOrDefault().StaffGuid; 

                    Guid sessionGuid = new Guid();
                    Guid staffLoginGuid = new Guid();
                    GetStaffLoginSession(incident.LaneCode, staffGuid, incident.IncidentSetDate, out sessionGuid, out staffLoginGuid);
                    
                    Data.Incident realIncident = new Data.Incident();

                    //add Incident
                    realIncident.Description = incident.Description;
                    //realIncident.IncidentAckDate = null
                    //realIncident.IncidentClearedDate = null
                    realIncident.IncidentGUID = Guid.NewGuid();
                    realIncident.IncidentSeqNr = (int)incident.IncidentSeqNr;
                    realIncident.IncidentSetDate = incident.IncidentSetDate;
                    realIncident.IncidentStatusGUID = Guid.Parse("F10FE5B2-7ED3-DE11-9533-001517C991CF");
                    realIncident.IncidentTypeGUID = (Guid)incidentType.IncidentGuid;
                    realIncident.LaneSeqNr = (int) incident.LaneSeqNr;
                    realIncident.LastIncidentSeqNr = null;
                    realIncident.StaffLoginGUID = staffLoginGuid;

                    if (incident.Transaction_Identifier > 0)
                    {
                        var transaction = context.Transactions.FirstOrDefault(x => x.Transaction_Identifier == incident.Transaction_Identifier.ToString() && x.LaneCode == incident.LaneCode);
                        if (transaction != null)
                        {
                            realIncident.TransactionGUID = transaction.TransGuid == null ? (Guid?)null : transaction.TransGuid;
                        }
                        else
                        {
                            //processed = false;
                            SaveIncident(realIncident, incident, staffLoginGuid, false);
                            continue;
                            //Log.LogTrace(string.Format("Transaction {0} for incident {1} doesnt exist on lane {2}.", incident.Transaction_Identifier, realIncident.IncidentGUID, incident.LaneCode));
                            //ServiceAgentManager.SendReport(string.Format("Transaction {0} for invident {1} doesnt exist on lane {2}.", incident.Transaction_Identifier, realIncident.IncidentGUID, incident.LaneCode), 1);
                        }

                    }
                    //IncidentTypes

                    SaveIncident(realIncident, incident, staffLoginGuid, true);
                    Intertoll.Sync.Client.SyncClient.SubmitIncident(realIncident);

                }


                Console.WriteLine("ProcessIncidents - end");
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        private static void GetStaffLoginSession(string laneCode, Guid staffGuid, DateTime eventDateTime, out Guid sessionGuidOut, out Guid staffLoginGuidOut)
        {
            Guid sessionGuid = new Guid();
            Guid staffLoginGuid = new Guid();
            try
            {
                var context = new PCSDataIntergrationEntities();
                var session = context.uspSessionsGet(laneCode).FirstOrDefault();

                if (session != null)
                {
                    if (session.StaffGUID == staffGuid)
                    {
                        //current session on this lane is for this user

                        sessionGuid = session.SessionGUID;
                        staffLoginGuid = session.StaffLoginGUID;
                            var currentSession = context.Sessions.Where(x => x.SessionGUID == session.SessionGUID).FirstOrDefault();
                            if ((eventDateTime.Date - session.StartDate.Date).Days > 0)
                            {
                                DivideEndOfDaySessions(currentSession, out sessionGuid, out staffLoginGuid);                               
                            }
                    }
                    else
                    {

                        //its in between session times on this lane, therefore it belongs to this session
                        var containingSession = context.Sessions.FirstOrDefault(x => x.LaneCode == laneCode && eventDateTime >= x.StartDate && eventDateTime <= x.EndDate);
                        if (containingSession != null)
                        {
                            sessionGuid = containingSession.SessionGUID;
                            staffLoginGuid = containingSession.StaffLoginGUID;
                            
                            if ((eventDateTime.Date - containingSession.StartDate.Date).Days > 0)
                            {
                                DivideEndOfDaySessions(containingSession, out sessionGuid, out staffLoginGuid);
                            }
                        }
                        else
                        {
                            var sessionBefore = context.Sessions.FirstOrDefault(x => x.LaneCode == laneCode && x.EndDate <= eventDateTime);
                            var sessionAfter = context.Sessions.FirstOrDefault(x => x.LaneCode == laneCode && x.StartDate >= eventDateTime);
                            //check date overlap 
                            if (sessionBefore != null)
                            {
                                DateTime endDate = (DateTime)sessionBefore.EndDate;
                                if ((endDate.Date - sessionBefore.StartDate.Date).Days > 0)
                                {
                                    DivideEndOfDaySessions(sessionBefore, out sessionGuid, out staffLoginGuid);
                                    sessionBefore = context.Sessions.FirstOrDefault(x => x.SessionGUID == sessionGuid);
                                }
                            }

                            if (sessionAfter != null)
                            {
                                if (sessionAfter.EndDate != null)
                                {
                                    DateTime endDate = (DateTime)sessionBefore.EndDate;
                                    if ((endDate.Date - sessionAfter.StartDate.Date).Days > 0)
                                    {
                                        DivideEndOfDaySessions(sessionAfter, out sessionGuid, out staffLoginGuid);
                                        sessionAfter = context.Sessions.FirstOrDefault(x => x.SessionGUID == sessionGuid);
                                    }
                                }
                                else
                                {
                                    if ((eventDateTime.Date - sessionAfter.StartDate.Date).Days > 0)
                                    {
                                        DivideEndOfDaySessions(sessionAfter, out sessionGuid, out staffLoginGuid);
                                        sessionAfter = context.Sessions.FirstOrDefault(x => x.SessionGUID == sessionGuid);
                                    }
                                }
                            }


                            //no session before it and its before start of first session
                            if (sessionBefore == null && sessionAfter != null)
                            {
                                //same staff
                                var dbStaffLogin = context.StaffLogins.Where(x => x.StaffLoginGUID == sessionAfter.StaffLoginGUID).FirstOrDefault();
                                if (sessionAfter.StaffLogin.StaffGUID == staffGuid)
                                {
                                    //change sessionAfter start to include this
                                    sessionAfter.StartDate = eventDateTime.AddMilliseconds(-5);
                                    dbStaffLogin.StartDate = eventDateTime.AddMilliseconds(-5);
                                    context.SaveChanges();
                                    SessionEnd(sessionAfter);
                                    StaffLoginEnd(dbStaffLogin);
                                    staffLoginGuid = dbStaffLogin.StaffLoginGUID;
                                    sessionGuid = sessionAfter.SessionGUID;
                                }
                                else
                                {
                                    //new session before fisrt session - start: this minus 5 ms; end: sessionAfter start minus 5 ms//Create Session and staff login

                                    //reorder the sequence numbers
                                    var sessList = context.Sessions.Where(x => x.LaneCode == laneCode).ToList();
                                    foreach (var sess in sessList)
                                    {
                                        sess.SessionSeq = sess.SessionSeq + 1;
                                    }
                                    context.SaveChanges();
                                    //start new session  before fisrt session
                                    staffLoginGuid = StaffLoginCreate(laneCode, eventDateTime.AddMilliseconds(-5), eventDateTime.AddMilliseconds(5), staffGuid);
                                    sessionGuid = SessionCreate(1, LookupManager.Lanes(laneCode), staffLoginGuid, eventDateTime.AddMilliseconds(-5), eventDateTime.AddMilliseconds(5), laneCode);
                                }
                            }
                            else
                            {
                                if (sessionBefore != null && sessionAfter != null)
                                {
                                    //same staff as session before
                                    if (sessionBefore.StaffLogin.StaffGUID == staffGuid)
                                    {
                                        //change sessionBefore end to include this
                                        //is session last transaction and incident still in session?

                                        var dbStaffLogin = context.StaffLogins.Where(x => x.StaffLoginGUID == sessionBefore.StaffLoginGUID).FirstOrDefault();
                                        sessionBefore.EndDate = eventDateTime.AddMilliseconds(5);
                                        dbStaffLogin.EndDate = eventDateTime.AddMilliseconds(5);

                                        //TODO: is session last transaction and incident still in sessionBefore?


                                        context.SaveChanges();
                                        SessionEnd(sessionBefore);
                                        StaffLoginEnd(dbStaffLogin);

                                        staffLoginGuid = dbStaffLogin.StaffLoginGUID;
                                        sessionGuid = sessionBefore.SessionGUID;
                                    }
                                    else
                                    {
                                        //same staff as session After
                                        if (sessionAfter.StaffLogin.StaffGUID == staffGuid)
                                        {
                                            //change sessionAfter start to include this
                                            var dbStaffLogin = context.StaffLogins.Where(x => x.StaffLoginGUID == sessionAfter.StaffLoginGUID).FirstOrDefault();
                                            sessionAfter.StartDate = eventDateTime.AddMilliseconds(-5);
                                            dbStaffLogin.StartDate = eventDateTime.AddMilliseconds(-5);

                                            //TODO: is session first transaction and incident still in sessionAfter?

                                            context.SaveChanges();
                                            SessionEnd(sessionAfter);
                                            StaffLoginEnd(dbStaffLogin);

                                            staffLoginGuid = dbStaffLogin.StaffLoginGUID;
                                            sessionGuid = sessionAfter.SessionGUID;
                                        }
                                        else
                                        {
                                            //Insert new session between sessionBefore and sessionAfter
                                            //new session - start: sessionBefore end this minus 5 ms; end: sessionAfter start minus 5 ms
                                            var dbStaffLogin = context.StaffLogins.Where(x => x.StaffLoginGUID == sessionBefore.StaffLoginGUID).FirstOrDefault();
                                            sessionBefore.EndDate = eventDateTime.AddMilliseconds(-5);
                                            dbStaffLogin.EndDate = eventDateTime.AddMilliseconds(-5);
                                            SessionEnd(sessionBefore);
                                            StaffLoginEnd(dbStaffLogin);

                                            var dbStaffLoginAfter = context.StaffLogins.Where(x => x.StaffLoginGUID == sessionBefore.StaffLoginGUID).FirstOrDefault();
                                            sessionAfter.StartDate = eventDateTime.AddMilliseconds(5);
                                            dbStaffLoginAfter.StartDate = eventDateTime.AddMilliseconds(5);
                                            SessionEnd(sessionAfter);
                                            StaffLoginEnd(dbStaffLoginAfter);

                                            //reorder the sequence numbers
                                            var sessList = context.Sessions.Where(x => x.LaneCode == laneCode && x.SessionSeq >= sessionAfter.SessionSeq).ToList();
                                            foreach (var sess in sessList)
                                            {
                                                sess.SessionSeq = sess.SessionSeq + 1;
                                            }
                                            context.SaveChanges();

                                            staffLoginGuid = StaffLoginCreate(laneCode, eventDateTime.AddMilliseconds(-5), eventDateTime.AddMilliseconds(5), staffGuid);
                                            sessionGuid = SessionCreate(sessionAfter.SessionSeq, LookupManager.Lanes(laneCode), staffLoginGuid, eventDateTime.AddMilliseconds(-5), eventDateTime.AddMilliseconds(5), laneCode);

                                            //TODO: is session last transaction and incident still in sessionBefore?
                                            //TODO: is session first transaction and incident still in sessionAfter?
                                        }
                                    }
                                }
                                else
                                {
                                    //session is not null and this staff is different from session staff
                                    //End Current Session and create a new session
                                    var dbSession = context.Sessions.Where(x => x.SessionGUID == session.SessionGUID).FirstOrDefault();
                                    var dbStaffLogin = context.StaffLogins.Where(x => x.StaffLoginGUID == dbSession.StaffLoginGUID).FirstOrDefault();
                                    if ((eventDateTime.Date - session.StartDate.Date).Days > 0)
                                    {
                                        DivideEndOfDaySessions(dbSession, out sessionGuid, out staffLoginGuid);
                                        dbSession = context.Sessions.Where(x => x.SessionGUID == sessionGuid).FirstOrDefault();
                                        dbStaffLogin = context.StaffLogins.Where(x => x.StaffLoginGUID == staffLoginGuid).FirstOrDefault();
                                    }

                                    dbStaffLogin.EndDate = eventDateTime.AddMilliseconds(-10);
                                    dbSession.EndDate = eventDateTime.AddMilliseconds(-10);
                                    context.SaveChanges();
                                    SessionEnd(dbSession);
                                    StaffLoginEnd(dbStaffLogin);

                                    //Create Session and staff login
                                    staffLoginGuid = StaffLoginCreate(laneCode, eventDateTime.AddMilliseconds(-5), null, staffGuid);
                                    sessionGuid = SessionCreate(dbSession.SessionSeq + 1, LookupManager.Lanes(laneCode), staffLoginGuid, eventDateTime.AddMilliseconds(-5), null, laneCode);

                                }
                            }

                            //if (sessionBefore != null && sessionAfter == null)
                            //{
                            //    //session is not null and this staff is different from session staff
                            //    //End Current Session and create a new session
                            //    var dbSession = context.Sessions.Where(x => x.SessionGUID == session.SessionGUID).FirstOrDefault();
                            //    var dbStaffLogin = context.StaffLogins.Where(x => x.StaffLoginGUID == dbSession.StaffLoginGUID).FirstOrDefault();

                            //    dbStaffLogin.EndDate = eventDateTime.AddMilliseconds(-10);
                            //    dbSession.EndDate = eventDateTime.AddMilliseconds(-10);
                            //    context.SaveChanges();
                            //    SessionEnd(dbSession);
                            //    StaffLoginEnd(dbStaffLogin);

                            //    //Create Session and staff login
                            //    staffLoginGuid = StaffLoginCreate(laneCode, eventDateTime.AddMilliseconds(-5), null, staffGuid);
                            //    sessionGuid = SessionCreate(dbSession.SessionSeq + 1, LookupManager.Lanes(laneCode), staffLoginGuid, eventDateTime.AddMilliseconds(-5), null, laneCode);
                            //}
                        }
                    }
                }
                else
                {
                    staffLoginGuid = StaffLoginCreate(laneCode, eventDateTime, null, staffGuid);
                    sessionGuid = SessionCreate(1, LookupManager.Lanes(laneCode), staffLoginGuid, eventDateTime.AddSeconds(-5), null, laneCode);
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Console.WriteLine("Session: " + ex.Message);
            }
            sessionGuidOut = sessionGuid;
            staffLoginGuidOut = staffLoginGuid;
            if (sessionGuidOut == new Guid() || staffLoginGuidOut == new Guid())
            {
                Console.WriteLine(string.Format("session is empty {0}, {1}, {2}", laneCode, staffGuid, eventDateTime));
                Log.LogTrace(string.Format("session is empty {0}, {1}, {2}", laneCode, staffGuid, eventDateTime));
            }
        }

        private static void ProcessHourlyAuditsData()
        {
            try
            {
                Log.LogTrace("ProcessHourlyAudits - start");
                Console.WriteLine("ProcessHourlyAudits - start");

                var context = new PCSDataIntergrationEntities();
                var startDate = DateTime.Now.AddHours(-1);
                var startDateHour = DateTime.Now.AddHours(-1).Hour;

                if (AppSettingsManager.GetHourlyAuditDate != null)
                {
                    startDate = (DateTime)(AppSettingsManager.GetHourlyAuditDate);
                    startDateHour = startDate.Hour;
                }
                else
                {
                    var lastHourlyAuditDate = context.HourlyAudits.OrderByDescending(x => x.Id).FirstOrDefault();
                    if (lastHourlyAuditDate != null)
                    {
                        startDate = lastHourlyAuditDate.AuditDate;
                        startDateHour = lastHourlyAuditDate.Hour;
                    }
                }
                startDate = startDate.Date;
                var hourlyAudits = context.uspHourlyAuditsGet(startDate,startDateHour).ToList();

                var sessionList = context.Sessions.Where(x => x.StartDate >= startDate).ToList();
                var staffLoginList = context.StaffLogins.Where(x => x.StartDate >= startDate).ToList();
                Log.LogTrace(string.Format("ProcessHourlyAudits count: {0}; date: {1}; hour: {2}", hourlyAudits.Count(), startDate, startDateHour));
                foreach (var hourlyAudit in hourlyAudits)
                {
                    //already processed - ignore
                    var hourlyAuditExist = context.HourlyAudits.Where(x => DbFunctions.TruncateTime(x.AuditDate) == DbFunctions.TruncateTime(hourlyAudit.AuditDate)
                        && x.Hour == hourlyAudit.Hour
                        && x.LaneCode == hourlyAudit.LaneCode
                        ).ToList();

                    if (hourlyAuditExist.Count() > 0)
                    {
                        continue;
                    }

                    var nextAuditDate = hourlyAudit.AuditDate.AddDays(1);
                    var startSession = sessionList.OrderBy(x => x.StartDate).FirstOrDefault(x => x.StartDate >= hourlyAudit.AuditDate && x.StartDate < nextAuditDate
                                                && x.StartDate.Hour == hourlyAudit.Hour
                                                && x.LaneCode == hourlyAudit.LaneCode);

                    var endSession = sessionList.OrderByDescending(x => x.StartDate).FirstOrDefault(x => x.StartDate >= hourlyAudit.AuditDate  && x.StartDate < nextAuditDate
                                                              && x.StartDate.Hour == hourlyAudit.Hour
                                                              && x.LaneCode == hourlyAudit.LaneCode);

                    var sessionStartSeqNumber = startSession == null ? 0 : startSession.SessionSeq;
                    var sessionEndSeqNumber = endSession == null ? 0 : endSession.SessionSeq;
                    var sessionDifferenceNumber = sessionEndSeqNumber - sessionStartSeqNumber;
                    var sessionRecordCount = sessionList.Where(x => x.StartDate >= hourlyAudit.AuditDate && x.StartDate < nextAuditDate
                                                              && x.StartDate.Hour == hourlyAudit.Hour
                                                              && x.LaneCode == hourlyAudit.LaneCode).Count();
                   

                    var staffLoginRecordCount = staffLoginList
                       .Join(sessionList,
                          staffLogin => staffLogin.StaffLoginGUID,
                          session => session.StaffLoginGUID,
                          (staffLogin, session) => new { StaffLogin = staffLogin, Session = session })
                       .Where(x => x.StaffLogin.StartDate >= hourlyAudit.AuditDate && x.StaffLogin.StartDate < nextAuditDate
                                                              && x.StaffLogin.StartDate.Hour == hourlyAudit.Hour
                                                              && x.Session.LaneCode == hourlyAudit.LaneCode)
                        .Count();
                                      

                    var staffLoginDifferenceNumber = 0;
                    Intertoll.Sync.Common.Audit realAudit = new Intertoll.Sync.Common.Audit();

                    realAudit.AuditDate = hourlyAudit.AuditDate;
                    realAudit.AuditHour = hourlyAudit.Hour;
                    realAudit.IncidentAuditStatus = hourlyAudit.IncidentAuditStatus;
                    realAudit.IncidentDifferenceNumber = (int)( hourlyAudit.IncidentDifferenceNumber);
                    realAudit.IncidentEndSeqNumber = (int)( hourlyAudit.IncidentEndSeqNumber);
                    realAudit.IncidentRecordCount = (int)( hourlyAudit.IncidentsCount);
                    realAudit.IncidentStartSeqNumber = (int)( hourlyAudit.IncidentsStartSeqNumber);
                    realAudit.LaneGuid = LookupManager.Lanes(hourlyAudit.LaneCode);
                    realAudit.LaneMode = hourlyAudit.LaneMode;
                    realAudit.SessionAuditStatus = hourlyAudit.SessionAuditStatus;
                    realAudit.SessionDifferenceNumber = sessionDifferenceNumber; // (int)hourlyAudit.SessionDifferenceNumber;
                    realAudit.SessionEndSeqNumber = sessionEndSeqNumber; // (int)hourlyAudit.SessionEndSeqNumber;
                    realAudit.SessionRecordCount = sessionRecordCount; // (int)hourlyAudit.SessionRecordCount;
                    realAudit.SessionStartSeqNumber = sessionStartSeqNumber; // (int)hourlyAudit.SessionStartSeqNumber;
                    realAudit.StaffLoginAuditStatus = hourlyAudit.StaffLoginAuditStatus;
                    realAudit.StaffLoginDifferenceNumber = staffLoginDifferenceNumber;// hourlyAudit.StaffLoginDifferenceNumber;
                    realAudit.StaffLoginRecordCount = staffLoginRecordCount; // hourlyAudit.StaffLoginRecordCount;
                    realAudit.TransAuditStatus = hourlyAudit.TransAuditStatus;
                    realAudit.TransDifferenceNumber = (int)hourlyAudit.TransDifferenceNumber;
                    realAudit.TransEndSeqNumber = (int)hourlyAudit.TransactionEndSeqNumber;
                    realAudit.TransRecordCount = (int)hourlyAudit.TransactionCount;
                    realAudit.TransStartSeqNumber = (int)hourlyAudit.TransactionStartSeqNumber;
                    realAudit.TimeStamp = DateTime.Now;

                    SaveAudit(realAudit, hourlyAudit);
                    Intertoll.Sync.Client.SyncClient.SubmitAudit(realAudit, hourlyAudit.Hour);
                }
                Console.WriteLine("ProcessHourlyAudits - end");
                Log.LogTrace("ProcessHourlyAudits - end");
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        private static Guid StaffLoginCreate(string laneCode, DateTime startDate, DateTime? endDate, Guid staffGuid)
        {
            var staffLogin = new Data.StaffLogin();
            staffLogin.ApplicationID = null;
            staffLogin.EndDate = endDate.HasValue ? endDate : null;
            staffLogin.IpAddress = null;
            staffLogin.LocationGUID = LookupManager.Lanes(laneCode);
            staffLogin.LocationTypeGUID = Guid.Parse("69CD1EE2-909C-44AA-9395-97A3F3D5F13A");
            staffLogin.RoleGUID = new Guid();
            staffLogin.StaffGUID = staffGuid;
            staffLogin.StaffLoginGUID = Guid.NewGuid();
            staffLogin.StartDate = startDate;
            staffLogin.SupervisorOnDuty = false;
            staffLogin.TerminalID = null;

            try
            {
                //Insert into DB
                using (var context = new PCSDataIntergrationEntities())
                {
                    context.StaffLogins.Add(
                        new StaffLogin()
                        {
                            ApplicationID = staffLogin.ApplicationID,
                            EndDate = staffLogin.EndDate,
                            IpAddress = staffLogin.IpAddress,
                            LocationGUID = staffLogin.LocationGUID,
                            LocationTypeGUID = staffLogin.LocationTypeGUID,
                            RoleGUID = staffLogin.RoleGUID,
                            StaffGUID = staffLogin.StaffGUID,
                            StaffLoginGUID = staffLogin.StaffLoginGUID,
                            StartDate = staffLogin.StartDate,
                            SupervisorOnDuty = staffLogin.SupervisorOnDuty,
                            TerminalID = staffLogin.TerminalID
                        });
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var e in ex.EntityValidationErrors)
                {
                    foreach (var ee in e.ValidationErrors)
                    {
                        Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ee.PropertyName, ee.ErrorMessage));

                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                ServiceAgentManager.SendReport("CreateStaffLogin: " + ex.Message, 1);
            }

            Intertoll.Sync.Client.SyncClient.SubmitStaffLogin(staffLogin);
            return staffLogin.StaffLoginGUID;
        }

        private static Guid SessionCreate(int sessionSeqNr, Guid laneGuid, Guid staffLoginGUID, DateTime sessionStartDate, DateTime? sessionEndDate, string laneCode)
        {
            var session = new Data.Session();
            session.AvcEndSeq = 0;
            session.AvcStartSeq = 0;
            session.COLDeclarationGUID = null;
            session.CollectedRevenue = 0;
            session.DayClosingGUID = null;
            session.EndDate = sessionEndDate.HasValue ? sessionEndDate : null;
            session.EndIncidentSeq = 0;
            session.IncidentCount = 0;
            session.IncompleteTransCount = 0;
            session.IsComplete = false;
            session.IsValidationComplete = false;
            session.LaneGUID = laneGuid;
            session.LaneRechargeCount = 0;
            session.LaneTransSeqEnd = 0;
            session.LaneTransSeqStart = 0;
            session.RechargeRevenue = 0;
            session.SessionGUID = Guid.NewGuid();
            session.SessionSeq = sessionSeqNr;
            session.StaffLoginGUID = staffLoginGUID;
            session.StartDate = sessionStartDate;
            session.StartIncidentSeq = 0;
            session.SystemFirstAVCnr = 0;
            session.SystemIncidentCount = 0;
            session.SystemIncidentFirst = 0;
            session.SystemIncidentLast = 0;
            session.SystemLastAVCnr = 0;
            session.SystemNextAVCnr = 0;
            session.SystemPreviousAVCnr = 0;
            session.SystemPreviousIncidentNr = 0;
            session.SystemPreviousTransNr = 0;
            session.SystemRevenue = 0;
            session.SystemTransCount = 0;
            session.SystemTransFirst = 0;
            session.SystemTransLast = 0;
            session.SystemTransNext = 0;
            session.TransCount = 0;
            session.ViolPaidRevenue = 0;
            session.WasForced = false;

            try
            {
                //Insert into DB
                using (var context = new PCSDataIntergrationEntities())
                {
                    context.Sessions.Add(
                        new Session()
                        {
                            AvcEndSeq = session.AvcEndSeq,
                            AvcStartSeq = session.AvcStartSeq,
                            COLDeclarationGUID = session.COLDeclarationGUID,
                            CollectedRevenue = session.CollectedRevenue,
                            DayClosingGUID = session.DayClosingGUID,
                            EndDate = session.EndDate,
                            EndIncidentSeq = session.EndIncidentSeq,
                            IncidentCount = session.IncidentCount,
                            IncompleteTransCount = session.IncompleteTransCount,
                            IsComplete = session.IsComplete,
                            IsValidationComplete = session.IsValidationComplete,
                            LaneCode = laneCode,
                            LaneGUID = session.LaneGUID,
                            LaneRechargeCount = session.LaneRechargeCount,
                            LaneTransSeqEnd = session.LaneTransSeqEnd,
                            LaneTransSeqStart = session.LaneTransSeqStart,
                            RechargeRevenue = session.RechargeRevenue,
                            SessionGUID = session.SessionGUID,
                            SessionSeq = session.SessionSeq,
                            StaffLoginGUID = session.StaffLoginGUID,
                            StartDate = session.StartDate,
                            StartIncidentSeq = session.StartIncidentSeq,
                            SystemFirstAVCnr = session.SystemFirstAVCnr,
                            SystemIncidentCount = session.SystemIncidentCount,
                            SystemIncidentFirst = session.SystemIncidentFirst,
                            SystemIncidentLast = session.SystemIncidentLast,
                            SystemLastAVCnr = session.SystemLastAVCnr,
                            SystemNextAVCnr = session.SystemNextAVCnr,
                            SystemPreviousAVCnr = session.SystemPreviousAVCnr,
                            SystemPreviousIncidentNr = session.SystemPreviousIncidentNr,
                            SystemPreviousTransNr = session.SystemPreviousTransNr,
                            SystemRevenue = session.SystemRevenue,
                            SystemTransCount = session.SystemTransCount,
                            SystemTransFirst = session.SystemTransFirst,
                            SystemTransLast = session.SystemTransLast,
                            SystemTransNext = session.SystemTransNext,
                            TransCount = session.TransCount,
                            ViolPaidRevenue = session.ViolPaidRevenue,
                            WasForced = session.WasForced
                        });
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var e in ex.EntityValidationErrors)
                {
                    foreach (var ee in e.ValidationErrors)
                    {
                        Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ee.PropertyName, ee.ErrorMessage));

                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                ServiceAgentManager.SendReport("SessionCreate: " + ex.Message, 1);
            }

            Intertoll.Sync.Client.SyncClient.SubmitSession(session);
            return session.SessionGUID;
        }

        private static void StaffLoginEnd(StaffLogin dbStaffLogin)
        {
            try
            {
                var staffLogin = new Data.StaffLogin();
                staffLogin.ApplicationID = dbStaffLogin.ApplicationID;
                staffLogin.EndDate = dbStaffLogin.EndDate;
                staffLogin.IpAddress = dbStaffLogin.IpAddress;
                staffLogin.LocationGUID = dbStaffLogin.LocationGUID;
                staffLogin.LocationTypeGUID = dbStaffLogin.LocationTypeGUID;
                staffLogin.RoleGUID = dbStaffLogin.RoleGUID;
                staffLogin.StaffGUID = dbStaffLogin.StaffGUID;
                staffLogin.StaffLoginGUID = dbStaffLogin.StaffLoginGUID;
                staffLogin.StartDate = dbStaffLogin.StartDate;
                staffLogin.SupervisorOnDuty = dbStaffLogin.SupervisorOnDuty;
                staffLogin.TerminalID = dbStaffLogin.TerminalID;

                Intertoll.Sync.Client.SyncClient.SubmitStaffLogin(staffLogin);
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                ServiceAgentManager.SendReport("StaffLoginEnd: " + ex.Message, 1);
            }
        }

        private static void SessionEnd(Session dbsession)
        {
            try
            {
                var session = new Data.Session();
                session.AvcEndSeq = dbsession.AvcEndSeq;
                session.AvcStartSeq = dbsession.AvcStartSeq;
                session.COLDeclarationGUID = dbsession.COLDeclarationGUID;
                session.CollectedRevenue = dbsession.CollectedRevenue;
                session.DayClosingGUID = dbsession.DayClosingGUID;
                session.EndDate = dbsession.EndDate;
                session.EndIncidentSeq = dbsession.EndIncidentSeq;
                session.IncidentCount = dbsession.IncidentCount;
                session.IncompleteTransCount = dbsession.IncompleteTransCount;
                session.IsComplete = dbsession.IsComplete;
                session.IsValidationComplete = dbsession.IsValidationComplete;
                session.LaneGUID = dbsession.LaneGUID;
                session.LaneRechargeCount = dbsession.LaneRechargeCount;
                session.LaneTransSeqEnd = dbsession.LaneTransSeqEnd;
                session.LaneTransSeqStart = dbsession.LaneTransSeqStart;
                session.RechargeRevenue = dbsession.RechargeRevenue;
                session.SessionGUID = dbsession.SessionGUID;
                session.SessionSeq = dbsession.SessionSeq;
                session.StaffLoginGUID = dbsession.StaffLoginGUID;
                session.StartDate = dbsession.StartDate;
                session.StartIncidentSeq = dbsession.StartIncidentSeq;
                session.SystemFirstAVCnr = dbsession.SystemFirstAVCnr;
                session.SystemIncidentCount = dbsession.SystemIncidentCount;
                session.SystemIncidentFirst = dbsession.SystemIncidentFirst;
                session.SystemIncidentLast = dbsession.SystemIncidentLast;
                session.SystemLastAVCnr = dbsession.SystemLastAVCnr;
                session.SystemNextAVCnr = dbsession.SystemNextAVCnr;
                session.SystemPreviousAVCnr = dbsession.SystemPreviousAVCnr;
                session.SystemPreviousIncidentNr = dbsession.SystemPreviousIncidentNr;
                session.SystemPreviousTransNr = dbsession.SystemPreviousTransNr;
                session.SystemRevenue = dbsession.SystemRevenue;
                session.SystemTransCount = dbsession.SystemTransCount;
                session.SystemTransFirst = dbsession.SystemTransFirst;
                session.SystemTransLast = dbsession.SystemTransLast;
                session.SystemTransNext = dbsession.SystemTransNext;
                session.TransCount = dbsession.TransCount;
                session.ViolPaidRevenue = dbsession.ViolPaidRevenue;
                session.WasForced = dbsession.WasForced;

                Intertoll.Sync.Client.SyncClient.SubmitSession(session);

            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                ServiceAgentManager.SendReport("SessionEnd: " + ex.Message, 1);
            }
        }

        private static string EncryptString(string laneCode, string cardNumber, DateTime expiryDate)
        {
            if(string.IsNullOrWhiteSpace(cardNumber))
                return null;

            const int dwKeySize = 1024;

            string keyXML = LookupManager.LaneEncryptionKeys(laneCode);

            if (string.IsNullOrEmpty(keyXML))
                throw new Exception("No RSA XML public key defined: 'PublicRSAKey' in LaneConfiguration is null or empty");

            RSAEncryption Encryptor = new RSAEncryption(dwKeySize, keyXML);

            //expiryDate = (DateTime)expiryDate;

            var inputString = cardNumber + "," + expiryDate.Year + "/" + expiryDate.Month;
            return Encryptor.Encrypt(inputString);
        }

        private static void SaveTransaction(Data.Transaction realTransaction, Data.ETCTransaction etcTransaction, Data.ETCIAuthenticator IAuthenticator, Data.ETCOAuthenticator OAuthenticator, uspTransactionGet_Result dbTransaction)
        {
            try
            {
                //Insert into DB
                using (var context = new PCSDataIntergrationEntities())
                {
                    context.Transactions.Add(
                        new Transaction()
                        {
                            ANPRLicensePlate = realTransaction.ANPRLicensePlate,
                            AVCClass = realTransaction.AVCClassGUID.HasValue ? realTransaction.AVCClassGUID : (Guid?)null,
                            AVCSeqNr = realTransaction.AVCSeqNr,
                            AvcStatus = realTransaction.AVCDetail,
                            BankCardExpiryDate = dbTransaction.BankCardExpiryDate,
                            BankCardNumber = dbTransaction.BankCardNumber,
                            CollectorClass = realTransaction.ColClassGUID.HasValue ? realTransaction.ColClassGUID : (Guid?)null,
                            CollectorID = dbTransaction.CollectorID.HasValue ? (int)dbTransaction.CollectorID : 0,
                            SessionGuid = realTransaction.SessionGUID,
                            ContextMark = dbTransaction.ContextMark,
                            ContextMarkId = etcTransaction.ContextMarkId,
                            ETCTransactionGUID = realTransaction.ETCTransactionGuid.HasValue ? realTransaction.ETCTransactionGuid : (Guid?)null,
                            FrequentUserPANNumber = dbTransaction.FrequentUserPANNumber,
                            AccountUserGUID = realTransaction.AccountUserGUID,
                            IDVL = etcTransaction.IDVL,
                            IsKeyed = realTransaction.IsKeyed,
                            IssuerAuthenticator_DData = IAuthenticator.Authenticator,
                            IssuerAuthenticator_DMAC = dbTransaction.IssuerAuthenticator_DMAC,
                            IssuerAuthenticator_KRef = IAuthenticator.KeyRef,
                            IssuerAuthenticator_KSID = dbTransaction.IssuerAuthenticator_KSID,
                            IssuerAuthenticator_KV = dbTransaction.IssuerAuthenticator_KV.ToString(),
                            IssuerAuthenticator_MAC = IAuthenticator.Nonce,
                            IssuerAuthenticatorGuid = etcTransaction.IssuerAuthenticatorGuid,
                            LaneCode = dbTransaction.LaneCode,
                            LaneStatus = realTransaction.LaneStatusGUID.ToString(),
                            LaneTransSeqNr = realTransaction.LaneTransSeqNr,
                            LicensePlate = realTransaction.LicensePlate,
                            OperatorAuthenticator_DData = OAuthenticator.Authenticator,
                            OperatorAuthenticator_DMAC = dbTransaction.OperatorAuthenticator_DMAC,
                            OperatorAuthenticator_KRef = OAuthenticator.KeyRef,
                            OperatorAuthenticator_KSID = dbTransaction.OperatorAuthenticator_KSID,
                            OperatorAuthenticator_KV = dbTransaction.OperatorAuthenticator_MAC,
                            OperatorAuthenticator_MAC = OAuthenticator.Nonce,
                            OperatorAuthenticatorGuid = etcTransaction.OperatorAuthenticatorGuid,
                            PAN = etcTransaction.PAN.ToString(),
                            PaymentGroup = realTransaction.PaymentGroupGUID.ToString(),
                            PaymentMech = realTransaction.PaymentMechGUID.ToString(),
                            PaymentMethod = realTransaction.PaymentMethodGUID.ToString(),
                            PaymentType = realTransaction.PaymentTypeGUID.ToString(),
                            ReceiptCount = realTransaction.ReceiptCount,
                            ReceiptTaxInvoiceDate = realTransaction.ReceiptTaxInvoiceDate,
                            Shift_Identifier = dbTransaction.Shift_Identifier.HasValue ? dbTransaction.Shift_Identifier.ToString() : null,
                            TagClass = dbTransaction.TagClass,
                            TagStatus = etcTransaction.VehichleState,
                            TariffAmount = realTransaction.TariffAmount,
                            TaxInvNr = realTransaction.TaxInvNr,
                            TimeStamp = DateTime.Now,
                            Transaction_Identifier = dbTransaction.Transaction_Identifier.ToString(),
                            TransactionDate = realTransaction.TransDate,
                            TransactionStartDate = realTransaction.TransStartDate,
                            TransGuid = realTransaction.TransGUID,
                            TariffGuid = (Guid)realTransaction.TariffGUID,
                            TAGEFCM = dbTransaction.TAGEFCM,
                            TAGMAID = dbTransaction.TAGMAID,
                            TAGOBUID = dbTransaction.TAGOBUID

                        });
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var e in ex.EntityValidationErrors)
                {
                    foreach (var ee in e.ValidationErrors)
                    {
                        Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ee.PropertyName, ee.ErrorMessage));

                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                //ServiceAgentManager.SendReport("CreateStaffLogin: " + ex.Message, 1);
            }

        }

        private static void SaveIncident(Data.Incident realIncident, uspIncidentsGet_Result dbIncident, Guid staffLoginGuid, Boolean transactionExist)
        {
            try
            {
                //Console.WriteLine("SaveIncident");
                //Insert into DB
                using (var context = new PCSDataIntergrationEntities())
                {
                    context.Incidents.Add(
                        new Incident()
                        {
                            CollectorID = dbIncident.CollectorID.HasValue ? (int)dbIncident.CollectorID : (int?)null,
                            Description = dbIncident.Description,
                            Incident_Identifier = dbIncident.Incident_Identifier.ToString(),
                            IncidentAckDate = realIncident.IncidentAckDate,
                            IncidentClearedDate = realIncident.IncidentClearedDate,
                            IncidentCode = dbIncident.IncidentCode,
                            IncidentGUID = realIncident.IncidentGUID,
                            IncidentSeqNr = realIncident.IncidentSeqNr,
                            IncidentSetDate = realIncident.IncidentSetDate,
                            IncidentStatusGUID = realIncident.IncidentStatusGUID,
                            IncidentTypeGUID = realIncident.IncidentTypeGUID,
                            LaneCode = dbIncident.LaneCode,
                            LaneSeqNr = realIncident.LaneSeqNr,
                            LastIncidentSeqNr = realIncident.LastIncidentSeqNr,
                            StaffLoginGUID = staffLoginGuid,
                            TimeStamp = DateTime.Now,
                            Transaction_Identifier = dbIncident.Transaction_Identifier.ToString(),
                            TransactionGUID = realIncident.TransactionGUID,
                            TransactionExist = transactionExist
                        });
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var e in ex.EntityValidationErrors)
                {
                    foreach (var ee in e.ValidationErrors)
                    {
                        Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ee.PropertyName, ee.ErrorMessage));

                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                //ServiceAgentManager.SendReport("CreateStaffLogin: " + ex.Message, 1);
            }

        }

        private static void SaveAudit(Intertoll.Sync.Common.Audit realAudit, uspHourlyAuditsGet_Result dbAudit)
        {
            try
            {
                //Insert into DB
                using (var context = new PCSDataIntergrationEntities())
                {
                    context.HourlyAudits.Add(
                        new HourlyAudit()
                        {
                            AuditDate = realAudit.AuditDate,
                            Hour = realAudit.AuditHour,
                            Id = realAudit.Id,
                            IncidentAuditStatus = realAudit.IncidentAuditStatus,
                            IncidentDifferenceNumber = realAudit.IncidentDifferenceNumber,
                            IncidentEndSeqNumber = realAudit.IncidentEndSeqNumber,
                            IncidentsCount = realAudit.IncidentRecordCount,
                            IncidentsStartSeqNumber = realAudit.IncidentStartSeqNumber,
                            LaneCode = dbAudit.LaneCode,
                            LaneGuid = realAudit.LaneGuid,
                            LaneMode = realAudit.LaneMode,
                            SessionAuditStatus = realAudit.SessionAuditStatus,
                            SessionDifferenceNumber = realAudit.SessionDifferenceNumber,
                            SessionEndSeqNumber = realAudit.SessionEndSeqNumber,
                            SessionRecordCount = realAudit.SessionRecordCount,
                            SessionStartSeqNumber = realAudit.SessionStartSeqNumber,
                            StaffLoginAuditStatus = realAudit.StaffLoginAuditStatus,
                            StaffLoginDifferenceNumber = realAudit.StaffLoginDifferenceNumber,
                            StaffLoginRecordCount = realAudit.StaffLoginRecordCount,
                            TimeStamp = realAudit.TimeStamp,
                            TransactionCount = realAudit.TransAuditStatus,
                            TransactionEndSeqNumber = realAudit.TransEndSeqNumber,
                            TransactionStartSeqNumber = realAudit.TransStartSeqNumber,
                            TransAuditStatus = realAudit.TransAuditStatus,
                            TransDifferenceNumber = realAudit.TransDifferenceNumber,
                            VGSDifferenceNumber = (Int32) dbAudit.VGSDifferenceNumber,
                            VGSIncidentCount = (Int32) dbAudit.VGSIncidentsCount,
                            VGSIncidentEndSeqNo = (Int32)dbAudit.VGSIncidentEndSeqNo,
                            VGSIncidentStartSeqNo = (Int32)dbAudit.VGSIncidentStartSeqNo
                        });
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var e in ex.EntityValidationErrors)
                {
                    foreach (var ee in e.ValidationErrors)
                    {
                        Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ee.PropertyName, ee.ErrorMessage));

                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                ServiceAgentManager.SendReport("SaveAudit: " + ex.Message, 1);
            }

        }
        
        private static void ProcessLinkedIncidents()
        {
            var context = new PCSDataIntergrationEntities();
            var incidentsList = context.Incidents.Where(x => x.TransactionExist == false).ToList();
            Log.LogTrace("ProcessLinkedIncidents Incidents List: " + incidentsList.Count());
            Console.WriteLine("ProcessLinkedIncidents Incidents List: " + incidentsList.Count());
            foreach (var incident in incidentsList)
            {
                Data.Incident realIncident = new Data.Incident();
                var transaction = context.Transactions.FirstOrDefault(x => x.Transaction_Identifier == incident.Transaction_Identifier.ToString() && x.LaneCode == incident.LaneCode);
                if (transaction != null)
                {
                    realIncident.Description = incident.Description;
                    realIncident.IncidentAckDate = incident.IncidentAckDate;
                    realIncident.IncidentClearedDate = incident.IncidentClearedDate;
                    realIncident.IncidentGUID = incident.IncidentGUID;
                    realIncident.IncidentSeqNr = incident.IncidentSeqNr;
                    realIncident.IncidentSetDate = incident.IncidentSetDate;
                    realIncident.IncidentStatusGUID = incident.IncidentStatusGUID;
                    realIncident.IncidentTypeGUID = incident.IncidentTypeGUID;
                    realIncident.LaneSeqNr = incident.LaneSeqNr;
                    realIncident.LastIncidentSeqNr = null;
                    realIncident.StaffLoginGUID = incident.StaffLoginGUID;
                    realIncident.TransactionGUID = transaction.TransGuid == null ? (Guid?)null : transaction.TransGuid;                    

                    Intertoll.Sync.Client.SyncClient.SubmitIncident(realIncident);

                    incident.TransactionExist = true;
                    context.SaveChanges();
                }
                else
                {
                    //Log.LogTrace(string.Format("Transaction {0} for incident {1} doesnt exist on lane {2}.", incident.Transaction_Identifier, incident.IncidentGUID, incident.LaneCode));
                    //ServiceAgentManager.SendReport(string.Format("Transaction {0} for invident {1} doesnt exist on lane {2}.", incident.Transaction_Identifier, realIncident.IncidentGUID, incident.LaneCode), 1);
                }
            }
            Console.WriteLine("ProcessLinkedIncidents: End");
        }

        private static void ProcessUnprocessedIncidentsTransactions()
        {
            var context = new PCSDataIntergrationEntities();
            var incidentsList = context.Incidents.Where(x => x.Processed == false).ToList();
            Log.LogTrace("ProcessUnprocessedIncidentsTransactions Incidents List: " + incidentsList.Count());
            Console.WriteLine("ProcessUnprocessedIncidentsTransactions Incidents List: " + incidentsList.Count());
            foreach (var incident in incidentsList)
            {
                try
                {
                    var results = new System.Data.Entity.Core.Objects.ObjectParameter("Result", 0);
                    context.uspUpdateIncident(incident.IncidentGUID, incident.StaffLoginGUID, results);
                    if ((int)results.Value == 2)
                    {
                        incident.Processed = true;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogException(ex);
                }
            }

            var transactionsList = context.Transactions.Where(x => x.Processed == false).ToList();
            Log.LogTrace("ProcessUnprocessedIncidentsTransactions Transactions List: " + transactionsList.Count());
            Console.WriteLine("ProcessUnprocessedIncidentsTransactions Transactions List: " + transactionsList.Count());
            foreach (var trans in transactionsList)
            {
                try
                {
                    var results = new System.Data.Entity.Core.Objects.ObjectParameter("Result", 0);
                    context.uspUpdateTransaction(trans.TransGuid, trans.SessionGuid, results);
                    if ((int)results.Value == 2)
                    {
                        trans.Processed = true;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogException(ex);
                }
            }
            context.SaveChanges();
            Console.WriteLine("ProcessUnprocessedIncidentsTransactions: End");
        }
        
        public static DateTime GetExpiryDateFromTrack2(string data, string bankCardNumber)
        {
            var expiryDate = new DateTime(); 
            try
            {
                if (string.IsNullOrWhiteSpace(bankCardNumber))
                {
                    return expiryDate;
                }

                if (string.IsNullOrWhiteSpace(data))
                {
                    //check if processed before and assign that expiry date otherwise next month as expiry date
                    var context = new PCSDataIntergrationEntities();
                    var trans = context.Transactions.FirstOrDefault(x => x.BankCardNumber == bankCardNumber && !(x.BankCardExpiryDate.Trim() == string.Empty || x.BankCardExpiryDate == null));
                    if (trans != null && !string.IsNullOrWhiteSpace(trans.BankCardExpiryDate))
                    {
                        Log.LogTrace(string.Format("bankCardNumber : {0}, has no expiry date, assigned Track2 from previous transaction Track2:{1}, transaction: {2}", bankCardNumber, trans.BankCardExpiryDate, trans.TransGuid));
                        data = trans.BankCardExpiryDate;
                        ServiceAgentManager.SendReport(string.Format("bankCardNumber : {0}, has no expiry date, assigned Track2 from previous transaction Track2:{1}, transaction: {2}", bankCardNumber, trans.BankCardExpiryDate, trans.TransGuid), 1);
                    }
                    else
                    {
                        expiryDate = GetDefaultExpiryDate();

                        Log.LogTrace(string.Format("bankCardNumber : {0}, has no expiry date, default date assigned: {1}", bankCardNumber,expiryDate));
                        ServiceAgentManager.SendReport(string.Format("bankCardNumber : {0}, has no expiry date, default date assigned: {1}", bankCardNumber, expiryDate), 1);
                        return expiryDate;
                    }
                }


                var yearMonthStr = data.Split('=')[1];

                var pos = data.IndexOf('=');
                var text = data.Substring(pos + 1);
                var yearMonthInt = new string(text.Where(c => char.IsDigit(c)).ToArray());

                if (yearMonthInt.Length < 4)
                {
                    expiryDate = GetDefaultExpiryDate();
                    Log.LogTrace(string.Format("bankCardNumber : {0}, has invalid expiry date from Track2: {1}, default date assigned: {2}", bankCardNumber, data, expiryDate));
                    ServiceAgentManager.SendReport(string.Format("bankCardNumber : {0}, has invalid expiry date from Track2: {1}, default date assigned: {2}", bankCardNumber, data, expiryDate), 1);
                    return expiryDate;
                }
                var yearMonth = yearMonthInt.Substring(0, 4);
                var year = yearMonth.Substring(0, 2);
                var month = yearMonth.Substring(2, 2);

                var dateString = "20" + year + "/" + month + "/1";

                if (!DateTime.TryParse(dateString, out expiryDate))
                {
                    expiryDate = GetDefaultExpiryDate();
                    Log.LogTrace(string.Format("bankCardNumber : {0}, has invalid expiry date from Track2: {1}, default date assigned {2}", bankCardNumber, data, expiryDate));
                    ServiceAgentManager.SendReport(string.Format("bankCardNumber : {0}, has invalid expiry date from Track2: {1}, default date assigned {2}", bankCardNumber, data, expiryDate), 1);
                    return expiryDate;
                }
            }
            catch(Exception ex)
            {
                Log.LogTrace(data);
                Log.LogException(ex);
                ServiceAgentManager.SendReport(string.Format("GetExpiryDateFromTrack2 Failed: Card: {0}; Data: {1}", bankCardNumber, data), 1);
                return GetDefaultExpiryDate();
            }
            return expiryDate;
        }

        private static DateTime GetDefaultExpiryDate()
        {
            var nowDate = DateTime.Now;
            Log.LogTrace(string.Format("new date now {0}, year:{1}, month:{2}", nowDate, nowDate.Year, nowDate.Month));
            var expiryDate = new DateTime(nowDate.Year, nowDate.Month, 1);
            expiryDate = expiryDate.AddMonths(2).AddMinutes(-1);

            return expiryDate;
        }
    }
}
