using System;

namespace Intertoll.Toll.DataImport.Interfaces.Entities
{
    interface IMISValidationListRecord
    {
        long IDVL { get; set; }
        long EFCMark { get; set; }
        long PAN { get; set; }
        int VehicleClass { get; set; }
        int ProductType { get; set; }
        string VLNPlateNo { get; set; }
        DateTime VehicleStateDate { get; set; }
        string VehicleState { get; set; }
    }
}
