CREATE TABLE [dbo].[Transactions] (
    [TransactionID]               VARCHAR (50)     NOT NULL,
    [LaneCode]                    VARCHAR (10)     NOT NULL,
    [TransGUID]                   UNIQUEIDENTIFIER CONSTRAINT [DF_Transactions_TransGUID] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [TransStartDate]              DATETIME         NOT NULL,
    [TransDate]                   DATETIME         NOT NULL,
    [ColClassGUID]                UNIQUEIDENTIFIER NULL,
    [AVCClassGUID]                UNIQUEIDENTIFIER NULL,
    [RealClassGUID]               UNIQUEIDENTIFIER NULL,
    [AppliedClassGUID]            UNIQUEIDENTIFIER NULL,
    [TariffGUID]                  UNIQUEIDENTIFIER NOT NULL,
    [TariffAmount]                MONEY            NOT NULL,
    [TariffVat]                   MONEY            NOT NULL,
    [ChangeAmount]                MONEY            NULL,
    [TenderedAmount]              MONEY            NULL,
    [CurrencyGUID]                UNIQUEIDENTIFIER NULL,
    [LaneStatusGUID]              UNIQUEIDENTIFIER NULL,
    [SessionGUID]                 UNIQUEIDENTIFIER NOT NULL,
    [LaneTransSeqNr]              INT              NOT NULL,
    [AVCSeqNr]                    INT              NOT NULL,
    [AvcStatusGUID]               UNIQUEIDENTIFIER NULL,
    [PaymentMethodGUID]           UNIQUEIDENTIFIER NOT NULL,
    [PaymentGroupGUID]            UNIQUEIDENTIFIER NOT NULL,
    [PaymentMechGUID]             UNIQUEIDENTIFIER NOT NULL,
    [PaymentTypeGUID]             UNIQUEIDENTIFIER NOT NULL,
    [PaymentDetail]               NVARCHAR (1000)  NULL,
    [LicensePlate]                NVARCHAR (1000)  NULL,
    [TaxInvNr]                    NVARCHAR (50)    NULL,
    [ReceiptCount]                INT              NULL,
    [AdditionalTransactionDetail] NVARCHAR (1000)  NULL,
    [AccountUserGUID]             UNIQUEIDENTIFIER NULL,
    [SupervisorLoginGUID]         UNIQUEIDENTIFIER NULL,
    [IsKeyed]                     BIT              CONSTRAINT [DF_Transactions_IsKeyed] DEFAULT ((0)) NOT NULL,
    [ImageID]                     INT              NULL,
    [AVCDetail]                   NVARCHAR (1000)  NULL,
    [AccountGUID]                 UNIQUEIDENTIFIER NULL,
    [ExchangeRate]                UNIQUEIDENTIFIER NULL,
    [TotalInLocalCurrency]        MONEY            NOT NULL,
    [PreviousLicensePlate]        NVARCHAR (64)    NULL,
    [PreviousPaymentMethodGUID]   UNIQUEIDENTIFIER NULL,
    [ReceiptTaxInvoiceDate]       DATETIME         NULL,
    [ANPRLicensePlate]            VARCHAR (50)     NULL,
    [ETCTransactionGuid]          UNIQUEIDENTIFIER NULL,
    [CardNumber]                  VARCHAR (25)     NULL,
    [BCCTransferStatus]           INT              CONSTRAINT [DF__Transacti__BCCTr__0AD2A005] DEFAULT ((1)) NULL,
    [ContextMarkId]               SMALLINT         NULL,
    [PAN]                         BIGINT           NULL,
    [CONV]                        BIGINT           NULL,
    [IDVL]                        BIGINT           NULL,
    [VehichleState]               VARCHAR (50)     NULL,
    [IsSent]                      BIT              NOT NULL,
    [TimeStamp]                   DATETIME         NOT NULL,
    [FullCardNumber]              VARCHAR (50)     NULL,
    CONSTRAINT [PK_Transactions_1] PRIMARY KEY CLUSTERED ([TransactionID] ASC)
);












GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20190205-145310]
    ON [dbo].[Transactions]([LaneCode] ASC, [TransDate] ASC, [LaneTransSeqNr] ASC);

