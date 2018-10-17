using System;
using System.Collections.Generic;
using System.Linq;
using Intertoll.Encryption;
using Intertoll.NLogger;
using Intertoll.TollDataImport.DataRequest.Data;
using Intertoll.TollDataImport.DataRequest.Data.Model;
using Intertoll.TollDataImport.DataRequest.Managers;

namespace Intertoll.TollDataImport.DataRequest
{
    public class TransactionRequestProcessor
    {
        static public void SendTransactions(int _LaneID, List<int> _SequenceNumbers)
        {
            using (PCSStagingDataContext dc = new PCSStagingDataContext())
            {
                int TransactionsFound = 0;

                //foreach (var sequenceNumber in _SequenceNumbers)
                {
                    try
                    {
                        string laneCode = LookupManager.Lanes(_LaneID);
                        var foundTransactions = dc.Transactions.Where(x => x.LaneCode == laneCode && _SequenceNumbers.Contains(x.LaneTransSeqNr));

                        foreach (var foundTransaction in foundTransactions)
                        {
                            Log.LogTrace(foundTransaction.LaneTransSeqNr + " found in sent items ");

                            if (SendTransaction(dc, foundTransaction))
                                TransactionsFound++;
                        }

                        foreach (var sequenceNumber in _SequenceNumbers.Except(foundTransactions.Select(x => x.LaneTransSeqNr)))
                        {
                            Log.LogTrace(string.Format("requesting {0} : {1}", _LaneID, sequenceNumber));
                            var foundTransactionData = dc.GetTransactionByTransactionSeqNumber(_LaneID, sequenceNumber);

                            if (foundTransactionData != null)
                            {
                                Log.LogTrace(sequenceNumber + " found in SV DB ");

                                if (SendTransaction(dc, foundTransactionData))
                                    TransactionsFound++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.LogException(ex);
                    }
                }

                Log.LogInfoMessage("Found transactions" + "(" + _LaneID + ")" + TransactionsFound + "/" +  _SequenceNumbers.Count);
            }
        }

        private static bool SendTransaction(PCSStagingDataContext dc, Transaction _Transaction)
        {
            Log.LogInfoMessage(string.Format("[Enter] sending previously sent transaction Lane{0} Sequence{1}", _Transaction.LaneCode, _Transaction.LaneTransSeqNr));

            var syncTransaction = new Intertoll.Data.Transaction();

            // Account user details
            if (!(string.IsNullOrWhiteSpace(_Transaction.FrequentUserPANNumber)))
            {
                var accountuser = dc.GetAccountUserDetails(_Transaction.FrequentUserPANNumber);

                if (accountuser == null)
                {
                    Log.LogFatal("Account user details not found PAN: " + _Transaction.FrequentUserPANNumber);
                    return false;
                }
                syncTransaction.AccountGUID = accountuser.AccountGUID;
                syncTransaction.AccountUserGUID = accountuser.AccountUserGUID;
            }

            Log.LogTrace("SessioGuid: " + _Transaction.SessionGuid);

            //add transaction
            syncTransaction.AppliedClassGUID = _Transaction.CollectorClass;
            syncTransaction.AVCClassGUID = _Transaction.AVCClass;
            syncTransaction.AVCSeqNr = _Transaction.AVCSeqNr;
            syncTransaction.ColClassGUID = _Transaction.CollectorClass;
            syncTransaction.CurrencyGUID = new Guid("942190CD-5053-41B4-9A08-CD472C99ECAF");
            syncTransaction.ExchangeRate = new Guid("5A5AE37F-AABC-428D-B4DC-2B4C87B8646C");
            syncTransaction.IsKeyed = _Transaction.IsKeyed.HasValue && (bool)_Transaction.IsKeyed;

            syncTransaction.LaneTransSeqNr = (int)(_Transaction.LaneTransSeqNr);
            syncTransaction.LicensePlate = _Transaction.LicensePlate;
            syncTransaction.PaymentDetail = EncryptString(_Transaction.LaneCode, _Transaction.BankCardNumber, GetExpiryDateFromTrack2(_Transaction.BankCardExpiryDate, _Transaction.BankCardNumber));


            syncTransaction.PaymentGroupGUID = new Guid(_Transaction.PaymentGroup);
            syncTransaction.PaymentMechGUID = new Guid(_Transaction.PaymentMech);
            syncTransaction.PaymentMethodGUID = new Guid(_Transaction.PaymentMethod);
            syncTransaction.PaymentTypeGUID = new Guid(_Transaction.PaymentType);
            syncTransaction.PreviousLicensePlate = null;
            syncTransaction.PreviousPaymentMethodGUID = null;
            syncTransaction.RealClassGUID = null;
            syncTransaction.ReceiptCount = _Transaction.ReceiptCount.HasValue ? _Transaction.ReceiptCount : null;
            syncTransaction.ReceiptTaxInvoiceDate = _Transaction.ReceiptTaxInvoiceDate;
            syncTransaction.SessionGUID = _Transaction.SessionGuid;
            syncTransaction.TariffAmount = _Transaction.TariffAmount;
            syncTransaction.TariffGUID = _Transaction.TariffGuid;
            syncTransaction.TariffVat = 0;
            syncTransaction.TaxInvNr = _Transaction.TaxInvNr;
            syncTransaction.TotalInLocalCurrency = _Transaction.TariffAmount;
            syncTransaction.TransDate = _Transaction.TransactionDate;
            syncTransaction.TransStartDate = _Transaction.TransactionStartDate;
            syncTransaction.TransGUID = _Transaction.TransGuid;
            syncTransaction.AVCDetail = _Transaction.AvcStatus;

            if (_Transaction.ETCTransactionGUID.HasValue)
            {
                var etcTransaction = new Intertoll.Data.ETCTransaction();
                var IAuthenticator = new Intertoll.Data.ETCIAuthenticator();
                var OAuthenticator = new Intertoll.Data.ETCOAuthenticator();

                IAuthenticator.Authenticator = _Transaction.IssuerAuthenticator_DData;
                IAuthenticator.AuthenticatorGuid = _Transaction.IssuerAuthenticatorGuid.Value;
                IAuthenticator.KeyRef = _Transaction.IssuerAuthenticator_KRef;
                IAuthenticator.Nonce = _Transaction.IssuerAuthenticator_MAC; //TODO ??Confirm

                OAuthenticator.Authenticator = _Transaction.OperatorAuthenticator_DData;
                OAuthenticator.AuthenticatorGuid = _Transaction.OperatorAuthenticatorGuid.Value;
                OAuthenticator.KeyRef = _Transaction.OperatorAuthenticator_KRef;
                OAuthenticator.Nonce = _Transaction.OperatorAuthenticator_MAC; //TODO ??Confirm

                etcTransaction.ClassGUID = _Transaction.TagClass.HasValue ? LookupManager.Classes((Int32)_Transaction.TagClass) :
                                                                               new Guid("57C3EBAF-AD68-46CB-B048-3E9E9F1D5F74");
                etcTransaction.ContextMarkId = short.Parse(_Transaction.ContextMark);
                etcTransaction.ETCTransactionGuid = _Transaction.ETCTransactionGUID.Value;
                etcTransaction.ExpiryDate = string.Empty;
                etcTransaction.IDVL = (long)_Transaction.IDVL;
                etcTransaction.IssuerAuthenticatorGuid = IAuthenticator.AuthenticatorGuid;
                etcTransaction.LaneTransSeqNr = _Transaction.LaneTransSeqNr;
                etcTransaction.LicensePlate = _Transaction.LicensePlate;
                etcTransaction.OperatorAuthenticatorGuid = OAuthenticator.AuthenticatorGuid;
                etcTransaction.PAN = long.Parse(_Transaction.PAN);
                etcTransaction.VehichleState = _Transaction.TagStatus;

                syncTransaction.ETCTransactionGuid = etcTransaction.ETCTransactionGuid;

                Sync.Client.SyncClient.SubmitETCTransaction(syncTransaction, etcTransaction, IAuthenticator, OAuthenticator);

            }
            else
            {
                Sync.Client.SyncClient.SubmitTransaction(syncTransaction);
            }

            Log.LogInfoMessage(string.Format("[Exit] sending previously sent transaction Lane{0} Sequence{1}", _Transaction.LaneCode, _Transaction.LaneTransSeqNr));

            return true;
        }

        private static bool SendTransaction(PCSStagingDataContext dc, uspGetTransactionByLaneAndSeq_Result TransactionData)
        {
            Log.LogInfoMessage(string.Format("[Enter] sending transaction Lane{0} Sequence{1}",TransactionData.LaneCode,TransactionData.LaneTransSeqNr));

            var syncTransaction = new Intertoll.Data.Transaction();

            // Account user details
            if (!(string.IsNullOrWhiteSpace(TransactionData.FrequentUserPANNumber)))
            {
                var accountuser = dc.GetAccountUserDetails(TransactionData.FrequentUserPANNumber);

                if (accountuser == null)
                {
                    Log.LogFatal("Account user details not found PAN: " + TransactionData.FrequentUserPANNumber);
                    return false;
                }
                syncTransaction.AccountGUID = accountuser.AccountGUID;
                syncTransaction.AccountUserGUID = accountuser.AccountUserGUID;
            }

            // Session
            var session = dc.Sessions.FirstOrDefault(x => x.StartDate <= TransactionData.TransactionDate && 
                                                     (x.EndDate >= TransactionData.TransactionDate || x.EndDate == null) &&
                                                     x.LaneCode == TransactionData.LaneCode)
                                         ;

            if (session == null)
            {
                Log.LogFatal(string.Format("Session not found: time({0}),lane({1})", TransactionData.TransactionDate, TransactionData.LaneCode));
                return false;
            }

            Log.LogTrace("SessioGuid: " + session.SessionGUID);

            //add transaction
            syncTransaction.AppliedClassGUID = TransactionData.CollectorClass.MappedClass();
            syncTransaction.AVCClassGUID = TransactionData.AVCClass.MappedClass();
            syncTransaction.AVCSeqNr = (int)TransactionData.AVCSeqNr;
            syncTransaction.ColClassGUID = TransactionData.CollectorClass.MappedClass();
            syncTransaction.CurrencyGUID = new Guid("942190CD-5053-41B4-9A08-CD472C99ECAF"); 
            syncTransaction.ExchangeRate = new Guid("5A5AE37F-AABC-428D-B4DC-2B4C87B8646C"); 
            syncTransaction.IsKeyed = TransactionData.IsKeyed.HasValue && (bool)TransactionData.IsKeyed;

            syncTransaction.LaneTransSeqNr = (int)(TransactionData.LaneTransSeqNr);
            syncTransaction.LicensePlate = TransactionData.LicensePlate;
            syncTransaction.PaymentDetail = EncryptString(TransactionData.LaneCode, TransactionData.BankCardNumber, GetExpiryDateFromTrack2(TransactionData.BankCardExpiryDate, TransactionData.BankCardNumber));


            syncTransaction.PaymentGroupGUID = TransactionData.PaymentGroup.MappedPaymentGroup();
            syncTransaction.PaymentMechGUID = TransactionData.PaymentMech.MappedPaymentMech();
            syncTransaction.PaymentMethodGUID = TransactionData.PaymentMethod.MappedPaymentMethod();
            syncTransaction.PaymentTypeGUID = TransactionData.PaymentType.MappedPaymentType();
            syncTransaction.PreviousLicensePlate = null;
            syncTransaction.PreviousPaymentMethodGUID = null;
            syncTransaction.RealClassGUID = null;
            syncTransaction.ReceiptCount = TransactionData.ReceiptCount.HasValue ? (int)TransactionData.ReceiptCount : (int?)null;
            syncTransaction.ReceiptTaxInvoiceDate = TransactionData.ReceiptTaxInvoiceDate;
            syncTransaction.SessionGUID = session.SessionGUID;
            syncTransaction.TariffAmount = (decimal)TransactionData.TariffAmount;
            syncTransaction.TariffGUID = TransactionData.VirtualPlazaID.MappedTariff(TransactionData.CollectorClass, TransactionData.TollTariffPlanID); ;
            syncTransaction.TariffVat = 0;
            syncTransaction.TaxInvNr = TransactionData.TaxInvNr;
            syncTransaction.TotalInLocalCurrency = (decimal)TransactionData.TariffAmount;
            syncTransaction.TransDate = TransactionData.TransactionDate;
            syncTransaction.TransStartDate = TransactionData.TransactionStartDate;
            syncTransaction.TransGUID = Guid.NewGuid();
            syncTransaction.AVCDetail = TransactionData.AvcStatus;

            if (!(string.IsNullOrWhiteSpace(TransactionData.ETCTransactionGUID)))
            {
                var etcTransaction = new Intertoll.Data.ETCTransaction();
                var IAuthenticator = new Intertoll.Data.ETCIAuthenticator();
                var OAuthenticator = new Intertoll.Data.ETCOAuthenticator();

                IAuthenticator.Authenticator = TransactionData.IssuerAuthenticator_DData;
                IAuthenticator.AuthenticatorGuid = Guid.NewGuid();
                IAuthenticator.KeyRef = TransactionData.IssuerAuthenticator_KRef.ToString();
                IAuthenticator.Nonce = TransactionData.IssuerAuthenticator_MAC; //TODO ??Confirm

                OAuthenticator.Authenticator = TransactionData.OperatorAuthenticator_DData;
                OAuthenticator.AuthenticatorGuid = Guid.NewGuid();
                OAuthenticator.KeyRef = TransactionData.OperatorAuthenticator_KRef.ToString();
                OAuthenticator.Nonce = TransactionData.OperatorAuthenticator_MAC; //TODO ??Confirm

                etcTransaction.ClassGUID = TransactionData.TagClass.HasValue ? LookupManager.Classes((Int32)TransactionData.TagClass) : 
                                                                               new Guid("57C3EBAF-AD68-46CB-B048-3E9E9F1D5F74");
                etcTransaction.ContextMarkId = short.Parse(TransactionData.ContextMark);
                etcTransaction.ETCTransactionGuid = Guid.Parse(TransactionData.ETCTransactionGUID);
                etcTransaction.ExpiryDate = string.Empty;
                etcTransaction.IDVL = (long)TransactionData.IDVL;
                etcTransaction.IssuerAuthenticatorGuid = IAuthenticator.AuthenticatorGuid;
                etcTransaction.LaneTransSeqNr = TransactionData.LaneTransSeqNr;
                etcTransaction.LicensePlate = TransactionData.LicensePlate;
                etcTransaction.OperatorAuthenticatorGuid = OAuthenticator.AuthenticatorGuid;
                etcTransaction.PAN = long.Parse(TransactionData.PAN);
                etcTransaction.VehichleState = TransactionData.TagStatus;

                syncTransaction.ETCTransactionGuid = etcTransaction.ETCTransactionGuid;

                SaveTransaction(syncTransaction, etcTransaction, IAuthenticator, OAuthenticator, TransactionData);
                Sync.Client.SyncClient.SubmitETCTransaction(syncTransaction, etcTransaction, IAuthenticator, OAuthenticator);

            }
            else
            {
                SaveTransaction(syncTransaction, new Intertoll.Data.ETCTransaction(), new Intertoll.Data.ETCIAuthenticator(), new Intertoll.Data.ETCOAuthenticator(), TransactionData);
                Sync.Client.SyncClient.SubmitTransaction(syncTransaction);
            }

            Log.LogInfoMessage(string.Format("[Exit] sending transaction Lane{0} Sequence{1}", TransactionData.LaneCode, TransactionData.LaneTransSeqNr));

            return true;
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
                    var context = new PCSStagingDataContext();
                    var trans = context.Transactions.FirstOrDefault(x => x.BankCardNumber == bankCardNumber && !(x.BankCardExpiryDate.Trim() == string.Empty || x.BankCardExpiryDate == null));
                    if (trans != null && !string.IsNullOrWhiteSpace(trans.BankCardExpiryDate))
                    {
                        Log.LogTrace(string.Format("bankCardNumber : {0}, has no expiry date, assigned Track2 from previous transaction Track2:{1}, transaction: {2}", bankCardNumber, trans.BankCardExpiryDate, trans.TransGuid));
                        data = trans.BankCardExpiryDate;
                        //ServiceAgentManager.SendReport(string.Format("bankCardNumber : {0}, has no expiry date, assigned Track2 from previous transaction Track2:{1}, transaction: {2}", bankCardNumber, trans.BankCardExpiryDate, trans.TransGuid), 1);
                    }
                    else
                    {
                        expiryDate = GetDefaultExpiryDate();

                        Log.LogTrace(string.Format("bankCardNumber : {0}, has no expiry date, default date assigned: {1}", bankCardNumber, expiryDate));
                        //ServiceAgentManager.SendReport(string.Format("bankCardNumber : {0}, has no expiry date, default date assigned: {1}", bankCardNumber, expiryDate), 1);
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
                    //ServiceAgentManager.SendReport(string.Format("bankCardNumber : {0}, has invalid expiry date from Track2: {1}, default date assigned: {2}", bankCardNumber, data, expiryDate), 1);
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
                    //ServiceAgentManager.SendReport(string.Format("bankCardNumber : {0}, has invalid expiry date from Track2: {1}, default date assigned {2}", bankCardNumber, data, expiryDate), 1);
                    return expiryDate;
                }
            }
            catch (Exception ex)
            {
                Log.LogTrace(data);
                Log.LogException(ex);
                //ServiceAgentManager.SendReport(string.Format("GetExpiryDateFromTrack2 Failed: Card: {0}; Data: {1}", bankCardNumber, data), 1);
                return GetDefaultExpiryDate();
            }
            return expiryDate;
        }

        private static DateTime GetDefaultExpiryDate()
        {
            var nowDate = DateTime.Now;
            var expiryDate = new DateTime(nowDate.Year, nowDate.Month, 1);
            expiryDate = expiryDate.AddMonths(2).AddMinutes(-1);

            return expiryDate;
        }

        private static void SaveTransaction(Intertoll.Data.Transaction realTransaction, Intertoll.Data.ETCTransaction etcTransaction, Intertoll.Data.ETCIAuthenticator IAuthenticator, Intertoll.Data.ETCOAuthenticator OAuthenticator, uspGetTransactionByLaneAndSeq_Result dbTransaction)
        {
            try
            {
                //Insert into DB
                using (var context = new PCSStagingDataContext())
                {
                    context.Transactions.Insert(
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
                    context.Save();
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }

        }

        static string EncryptString(string laneCode, string cardNumber, DateTime expiryDate)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
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
    }
}
