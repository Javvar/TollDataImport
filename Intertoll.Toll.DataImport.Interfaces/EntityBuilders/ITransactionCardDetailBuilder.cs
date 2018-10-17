using Intertoll.Toll.DataImport.Interfaces.Entities;

namespace Intertoll.Toll.DataImport.Interfaces.EntityBuilders
{
    public interface ITransactionCardDetailBuilder
    {
        void EncryptedPAN(ref ITollTransaction transaction);
    }
}
