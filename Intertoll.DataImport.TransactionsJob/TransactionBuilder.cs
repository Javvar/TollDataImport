using System;
using Intertoll.DataImport.Mail;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Intertoll.Toll.DataImport.Interfaces.EntityBuilders;
using Unity;

namespace Intertoll.DataImport.TransactionsJob
{
    public class TransactionEntityBuilder : ITransactionEntityBuilder
    {
        IMailClient MailClient;
        ITransactionCardDetailBuilder CardDetailBuilder;

        public TransactionEntityBuilder(IMailClient _mailClient, ITransactionCardDetailBuilder _cardDetailBuilder)
        {
            MailClient = _mailClient;
            CardDetailBuilder = _cardDetailBuilder;
        }

        public ITollTransaction Build(ITollTransaction entity)
        {
            if (!string.IsNullOrEmpty(entity.PaymentDetail))
                CardDetailBuilder.EncryptedPAN(ref entity);

            if (entity.TransactionID.EndsWith("D"))
            {
                MailClient.SendMessage<DuplicateTransactionMailFormatter>("Duplicate transaction", "Duplicate transaction detected: " + entity.TransactionID);
                entity = null;
            }

            return entity;
        }
    }

    public class ETCTransactionEntityBuilder : IETCTransactionEntityBuilder
    {
        IMailClient MailClient;

        public ETCTransactionEntityBuilder(IMailClient _mailClient)
        {
            MailClient = _mailClient;
        }

        public IETCTollTransaction Build(IETCTollTransaction entity)
        {
            if (entity.TransactionID.EndsWith("D"))
            {
                MailClient.SendMessage<DuplicateTransactionMailFormatter>("Duplicate etc transaction", "Duplicate etc transaction detected: " + entity.TransactionID);
            }

            entity.ClassGUID = entity.AppliedClassGUID;
            entity.IssuerAuthenticatorGuid = Guid.NewGuid();
            entity.OperatorAuthenticatorGuid = Guid.NewGuid();

            return !entity.TransactionID.EndsWith("D") ? entity : null;
        }
    }
}
