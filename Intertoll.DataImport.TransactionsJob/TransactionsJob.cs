﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Intertoll.DataImport.Mail;
using Intertoll.DataImport.Schedulable;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Quartz;
using Unity;
using Unity.Resolution;

namespace Intertoll.DataImport.TransactionsJob
{
    [DisallowConcurrentExecution]
    public class TransactionsJob : BaseSchedulable<TransactionsJob>
    {
        ITollDataProvider DataProvider;
        IUnityContainer Container;
        IMailClient MailClient;
        ISettingsProvider Settings { get; set; }

        public TransactionsJob(IUnityContainer _container, ITollDataProvider _dataProvider, IMailClient _mailClient, ISettingsProvider _settings)
        {
            Container = _container;
            DataProvider = _dataProvider;
            MailClient = _mailClient;
            Settings = _settings;
        }

        public override void Execute(IJobExecutionContext context)
        {
            Log.LogTrace("[Enter]" + JobName);

            string CardsFileName = null;

            try
            {
                CheckForUnImportedStaff();

                var transactionBatch = DataProvider.GetNextTransactionBatch();
                CardsFileName = Path.Combine(Settings.CardDecryptionUtilityLocation, "cards_" + transactionBatch.GetHashCode() + ".txt");

                DecryptCardNumbers(transactionBatch, CardsFileName);

                Log.LogTrace("Transaction batch count: " + transactionBatch.Count);

                var SentTransactions = new List<ITollTransaction>();

                foreach (var tollTransaction in transactionBatch)
                {
                    ITollTransaction submittedEntity;

                    if (tollTransaction.ETCTransactionGuid == null)
                    {
                        var submitter = Container.Resolve<ITransactionSubmitter>(new ParameterOverride("_dataProvider", DataProvider));
                        submittedEntity = submitter.Submit(tollTransaction);
                    }
                    else
                    {
                        var submitter = Container.Resolve<IETCTransactionSubmitter>(new ParameterOverride("_dataProvider", DataProvider));
                        submittedEntity = submitter.Submit(tollTransaction);
                    }

                    if (submittedEntity.IsSent)
                        SentTransactions.Add(submittedEntity);
                }

                DataProvider.SaveTransactions(SentTransactions);
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }
            finally
            {
                if (!string.IsNullOrEmpty(CardsFileName) && File.Exists(CardsFileName))
                    File.Delete(CardsFileName); 
            }

            Log.LogTrace("[Exit]" + JobName);
        }

        private void DecryptCardNumbers(IList<ITollTransaction> TransactionBatch, string CardsFileName)
        {
            if(!TransactionBatch.Any())
                return;

            List<string> cardNumbers = TransactionBatch.Where(x => !string.IsNullOrEmpty(x.PaymentDetail))
                                                       .Select(transaction => transaction.PaymentDetail)
                                                       .Distinct()
                                                       .ToList();

            File.WriteAllLines(CardsFileName, cardNumbers);

            try
            {
                var p = new Process();
                string fileName = Path.Combine(Settings.CardDecryptionUtilityLocation, "DecryptUtil.exe");
                string param = "/C" + "\"" + fileName + " " + CardsFileName + "\"";

                var process = new ProcessStartInfo("cmd.exe", param);
                process.UseShellExecute = false;
                process.WorkingDirectory = Settings.CardDecryptionUtilityLocation;
                process.RedirectStandardOutput = true;
                process.RedirectStandardError = true;
                process.CreateNoWindow = true;

                p.StartInfo = process;
                p.Start();

                int RetryCount = 120;

                while (!p.HasExited && RetryCount > 0)
                {
                    RetryCount--;
                    Thread.Sleep(1000);
                }

                if (RetryCount > 0)
                {
                    foreach (var cardNumber in cardNumbers)
                    {
                        var DecryptedPAN = File.ReadAllText(Path.Combine(Settings.CardDecryptionUtilityLocation, "PAN_" + cardNumber + ".txt"));

                        foreach (var tranx in TransactionBatch.Where(x => x.PaymentDetail == cardNumber))
                        {
                            tranx.PaymentDetail  = Regex.Replace(DecryptedPAN, @"\t|\n|\r", "");
                        }
                    }
                }

                string filesToDelete = @"PAN_*";
                string[] fileList = Directory.GetFiles(Settings.CardDecryptionUtilityLocation, filesToDelete);

                foreach (string file in fileList)
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
                Log.LogTrace(ex.Message + ". Check error log for more details.");
            }
        }

        public void CheckForUnImportedStaff()
        {
            var Newstaff = DataProvider.ImportNewStaff();

            if (Newstaff.Any())
                MailClient.SendMessage<NewStaffMailFormatter>("New staff imported from old system", Newstaff.Aggregate((x, y) => x + "|" + y));
        }
    }
}


