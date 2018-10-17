using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Intertoll.Toll.DataImport.DataRequest
{
    [ServiceContract(Namespace = "http://Intertoll.TollDataImport.DataRequest")]
    public interface ITollDataImportDataRequestService
    {
        [OperationContract(IsOneWay = true)]
        void RequestData(int _LaneID, DataTypeRequest _DataType, List<int> _SequenceNumbers);
        
    }

    public enum DataTypeRequest
    {
        Incident,
        Transaction,
        Session,
        Audit
    }
}
