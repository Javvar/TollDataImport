using Intertoll.Toll.DataImport.Interfaces.Entities;

namespace Intertoll.Toll.DataImport.Interfaces.EntityBuilders
{
    public interface ITransactionEntityBuilder : ITollEntityBuilder<ITollTransaction>
    {
    }

    public interface IETCTransactionEntityBuilder : ITollEntityBuilder<IETCTollTransaction>
    {
    }
}
