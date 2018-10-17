using System;
using AutoMapper;
using Intertoll.Data;
using Intertoll.NLogger;
using Intertoll.Toll.DataImport.Interfaces;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Intertoll.Toll.DataImport.Interfaces.EntityBuilders;

namespace Intertoll.DataImport.SyncClient
{

    public class SyncTransactionSubmitter : ITransactionSubmitter
    {
        public ITollDataProvider DataProvider { get; set; }
        public ITransactionEntityBuilder EntityBuilder { get; set; }

        static SyncTransactionSubmitter()
        {
            Mapper.CreateMap<ITollTransaction, Transaction>();
        }

        public SyncTransactionSubmitter(ITollDataProvider _dataProvider, ITransactionEntityBuilder _builder)
        {
            DataProvider = _dataProvider;
            EntityBuilder = _builder;
        }


        public ITollTransaction Submit(ITollTransaction transaction)
        {
            var builtTransaction = EntityBuilder.Build(transaction);

            if (builtTransaction != null)
            {
                try
                {
                    var syncTransaction = Mapper.Map<ITollTransaction, Transaction>(builtTransaction);
                    Sync.Client.SyncClient.SubmitTransaction(syncTransaction);

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
    }
}
