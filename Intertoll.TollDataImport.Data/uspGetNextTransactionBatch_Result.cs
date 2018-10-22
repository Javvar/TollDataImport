//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intertoll.DataImport.Data
{
    using System;
    
    public partial class uspGetNextTransactionBatch_Result
    {
        public string TransactionID { get; set; }
        public string LaneCode { get; set; }
        public System.Guid TransGUID { get; set; }
        public System.DateTime TransStartDate { get; set; }
        public System.DateTime TransDate { get; set; }
        public Nullable<System.Guid> ColClassGUID { get; set; }
        public Nullable<System.Guid> AVCClassGUID { get; set; }
        public Nullable<System.Guid> RealClassGUID { get; set; }
        public Nullable<System.Guid> AppliedClassGUID { get; set; }
        public Nullable<System.Guid> TariffGUID { get; set; }
        public decimal TariffAmount { get; set; }
        public decimal TariffVat { get; set; }
        public Nullable<decimal> ChangeAmount { get; set; }
        public Nullable<decimal> TenderedAmount { get; set; }
        public Nullable<System.Guid> CurrencyGUID { get; set; }
        public Nullable<System.Guid> LaneStatusGUID { get; set; }
        public System.Guid SessionGUID { get; set; }
        public int LaneTransSeqNr { get; set; }
        public int AVCSeqNr { get; set; }
        public Nullable<System.Guid> AvcStatusGUID { get; set; }
        public System.Guid PaymentMethodGUID { get; set; }
        public System.Guid PaymentGroupGUID { get; set; }
        public System.Guid PaymentMechGUID { get; set; }
        public System.Guid PaymentTypeGUID { get; set; }
        public string PaymentDetail { get; set; }
        public string LicensePlate { get; set; }
        public string TaxInvNr { get; set; }
        public Nullable<int> ReceiptCount { get; set; }
        public string AdditionalTransactionDetail { get; set; }
        public Nullable<System.Guid> AccountUserGUID { get; set; }
        public Nullable<System.Guid> SupervisorLoginGUID { get; set; }
        public bool IsKeyed { get; set; }
        public Nullable<int> ImageID { get; set; }
        public string AVCDetail { get; set; }
        public Nullable<System.Guid> AccountGUID { get; set; }
        public Nullable<System.Guid> ExchangeRate { get; set; }
        public decimal TotalInLocalCurrency { get; set; }
        public string PreviousLicensePlate { get; set; }
        public Nullable<System.Guid> PreviousPaymentMethodGUID { get; set; }
        public Nullable<System.DateTime> ReceiptTaxInvoiceDate { get; set; }
        public string ANPRLicensePlate { get; set; }
        public Nullable<System.Guid> ETCTransactionGuid { get; set; }
        public string CardNumber { get; set; }
        public Nullable<int> BCCTransferStatus { get; set; }
        public Nullable<short> ContextMarkId { get; set; }
        public Nullable<long> PAN { get; set; }
        public Nullable<long> CONV { get; set; }
        public Nullable<long> IDVL { get; set; }
        public string VehichleState { get; set; }
        public bool IsSent { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public string FullCardNumber { get; set; }
        public string StaffID { get; set; }
        public string AccountIdentifier { get; set; }
    }
}
