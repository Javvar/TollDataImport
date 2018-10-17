using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Intertoll.Encryption;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Intertoll.Toll.DataImport.Interfaces.EntityBuilders;
using System.Text.RegularExpressions;

namespace Intertoll.DataImport.TransactionsJob
{
    public class TransactionCardDetailBuilder : ITransactionCardDetailBuilder
    {
        ISettingsProvider Settings { get; set; }

        public TransactionCardDetailBuilder(ISettingsProvider _settings)
        {
            Settings = _settings;
        }

        public void EncryptedPAN(ref ITollTransaction transaction)
        {
            RSAEncryption Encryptor = new RSAEncryption(1024, Settings.EncryptionKey);
            transaction.PaymentDetail = Encryptor.Encrypt(transaction.PaymentDetail);
        }
    }
}
