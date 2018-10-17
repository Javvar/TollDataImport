using System;

namespace Intertoll.Toll.DataImport.Interfaces.Entities
{
    public interface    IETCTollTransaction : ITollTransaction
    {
        short? ContextMarkId { get; set; }
        long? PAN { get; set; }
        long? CONV { get; set; }
        long? IDVL { get; set; }
        string VehichleState { get; set; }
        Guid? ClassGUID { get; set; }
        Guid? IssuerAuthenticatorGuid { get; set; }
        Guid? OperatorAuthenticatorGuid { get; set; }
    }
}
