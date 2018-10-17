using System.Collections.Generic;
using System.ServiceModel;

namespace Intertoll.DataImport.DataRequest
{
    [ServiceContract(Namespace = "http://Intertoll.TollDataImport.DataRequest")]
    public interface ITollDataRequest
    {
        [OperationContract(IsOneWay = true)]
        void RequestData(string laneCode, DataTypeRequest dataType, List<int> _sequenceNumbers);
    }

    public enum DataTypeRequest
    {
        Incident,
        Transaction,
        Session,
        Audit
    }
}
