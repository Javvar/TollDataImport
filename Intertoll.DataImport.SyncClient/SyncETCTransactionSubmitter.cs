using System;
using AutoMapper;
using Intertoll.Data;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Intertoll.Toll.DataImport.Interfaces.EntityBuilders;

namespace Intertoll.DataImport.SyncClient
{
    public class SyncETCTransactionSubmitter : IETCTransactionSubmitter
    {
        public ITollDataProvider DataProvider { get; set; }
        public IETCTransactionEntityBuilder EntityBuilder { get; set; }
        public IETCTransactionEntityBuilder ETCEntityBuilder { get; set; }

        static SyncETCTransactionSubmitter()
        {
            Mapper.CreateMap<ITollTransaction, Transaction>();
            Mapper.CreateMap<IETCTollTransaction, ETCTransaction>().ForMember(dest => dest.LaneTransSeqNr,opt => opt.MapFrom(src => src.LaneTransSeqNr));
        }

        public SyncETCTransactionSubmitter(ITollDataProvider _dataProvider, IETCTransactionEntityBuilder _entityBuilder, IETCTransactionEntityBuilder _etcEntityBuilder)
        {
            DataProvider = _dataProvider;
            EntityBuilder = _entityBuilder;
            ETCEntityBuilder = _etcEntityBuilder;
        }


        public IETCTollTransaction Submit(IETCTollTransaction transaction)
        {
            var builtTransaction = EntityBuilder.Build(transaction);

            if (builtTransaction != null)
            {
                try
                {
                    var builtETCTransaction = ETCEntityBuilder.Build(transaction);

                    var syncTransaction = Mapper.Map<ITollTransaction, Transaction>(builtTransaction);
                    var syncETCTransaction = Mapper.Map<IETCTollTransaction, ETCTransaction>(builtETCTransaction);

                    Sync.Client.SyncClient.SubmitETCTransaction(syncTransaction, syncETCTransaction
                                                                , GetIAuthenticator(syncETCTransaction.IssuerAuthenticatorGuid)
                                                                , GetOAuthenticator(syncETCTransaction.OperatorAuthenticatorGuid));

                    transaction.IsSent = true;
                    return builtTransaction;
                }
                catch (Exception ex)
                {
                    Log.LogException(ex);
                    Log.LogTrace(ex.Message + ". Check error log for more details.");
                }
            }

            return transaction;
        }

        private ETCIAuthenticator GetIAuthenticator(Guid guid)
        {
            var Authenticator = new ETCIAuthenticator();
            Authenticator.Authenticator = string.Empty;
            Authenticator.AuthenticatorGuid = guid;
            Authenticator.KeyRef = string.Empty;
            Authenticator.Nonce = string.Empty;

            return Authenticator;
        }

        ETCOAuthenticator GetOAuthenticator(Guid guid)
        {
            var Authenticator = new ETCOAuthenticator();
            Authenticator.Authenticator = string.Empty;
            Authenticator.AuthenticatorGuid = guid;
            Authenticator.KeyRef = string.Empty;
            Authenticator.Nonce = string.Empty;

            return Authenticator;
        }
    }
}
