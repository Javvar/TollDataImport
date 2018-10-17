using System;

namespace Intertoll.Toll.DataImport.Interfaces
{
    public interface ITollTransaction : ITollEntity
    {
        DateTime TransactionStartDate { get; set; }
        DateTime TransactionDate { get; set; }
        int MvcClass { get; set; }
        int AvcClass { get; set; }
        int? RealClass { get; set; }
        decimal TariffAmount { get; set; }
        string LaneStatus { get; set; }
        string CollectorID { get; set; }
        string VirtualPlazaID { get; set; }
        long AvcSequenceNumber { get; set; }
        string AvcStatus { get; set; }
        int? PaymentMethod { get; set; }
        int? PaymentGroup { get; set; }
        int? PaymentMech { get; set; }
        int? PaymentType { get; set; }
        string IsoCardPan { get; set; }
        string IsoCardExpiryDate { get; set; }
        string VehicleRegistrationPlate { get; set; }
        string TaxInvoiceNumber { get; set; }
        bool IsCardNumberKeyed { get; set; }
        Guid? Etc_etcTransactionGUID { get; set; }
        string Etc_contextMark { get; set; }
        string Etc_pan { get; set; }
        long? Etc_Idvl { get; set; }
        string Etc_TagStatus { get; set; }
        int? Etc_TagClass { get; set; }
        string Etc_OperatorAuthenticator_KSID { get; set; }
        long? Etc_OperatorAuthenticator_KV { get; set; }
        long? Etc_OperatorAuthenticator_KRef { get; set; }
        string Etc_OperatorAuthenticator_DData { get; set; }
        string Etc_OperatorAuthenticator_DMAC { get; set; }
        string Etc_OperatorAuthenticator_MAC { get; set; }
        string Etc_IssuerAuthenticator_KSID { get; set; }
        long? Etc_IssuerAuthenticator_KV { get; set; }
        long? Etc_IssuerAuthenticator_KRef { get; set; }
        string Etc_IssuerAuthenticator_DData { get; set; }
        string Etc_IssuerAuthenticator_DMAC { get; set; }
        string Etc_IssuerAuthenticator_MAC { get; set; }
        long? Etc_TagEfcm { get; set; }
        short? Etc_TagMaid { get; set; }
        int? Etc_TagObuid { get; set; }
    }
}
