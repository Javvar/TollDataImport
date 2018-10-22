using System;

namespace Intertoll.Toll.DataImport.Interfaces.Entities
{
    public interface ITollTransaction : ITollEntity
    {
        string TransactionID { get; set; }
        Guid TransGUID { get; set; }
        DateTime TransStartDate { get; set; }
        DateTime TransDate { get; set; }
        Guid? ColClassGUID { get; set; }
        Guid? AVCClassGUID { get; set; }
        Guid? RealClassGUID { get; set; }
        Guid? AppliedClassGUID { get; set; }
        Guid? TariffGUID { get; set; }
        decimal TariffAmount { get; set; }
        decimal TariffVat { get; set; }
        decimal? ChangeAmount { get; set; }
        decimal? TenderedAmount { get; set; }
        Guid? CurrencyGUID { get; set; }
        Guid? LaneStatusGUID { get; set; }
        Guid SessionGUID { get; set; }
        int LaneTransSeqNr { get; set; }
        int AVCSeqNr { get; set; }
        Guid? AvcStatusGUID { get; set; }
        Guid PaymentMethodGUID { get; set; }
        Guid PaymentGroupGUID { get; set; }
        Guid PaymentMechGUID { get; set; }
        Guid PaymentTypeGUID { get; set; }
        string PaymentDetail { get; set; }
        string LicensePlate { get; set; }
        string TaxInvNr { get; set; }
        int? ReceiptCount { get; set; }
        string AdditionalTransactionDetail { get; set; }
        Guid? AccountUserGUID { get; set; }
        Guid? SupervisorLoginGUID { get; set; }
        bool IsKeyed { get; set; }
        int? ImageID { get; set; }
        string AVCDetail { get; set; }
        Guid? AccountGUID { get; set; }
        Guid? ExchangeRate { get; set; }
        decimal TotalInLocalCurrency { get; set; }
        string PreviousLicensePlate { get; set; }
        Guid? PreviousPaymentMethodGUID { get; set; }
        DateTime? ReceiptTaxInvoiceDate { get; set; }
        string ANPRLicensePlate { get; set; }
        Guid? ETCTransactionGuid { get; set; }
        string CardNumber { get; set; }
        int? BCCTransferStatus { get; set; }
        string FullCardNumber { get; set; }
    }
}
