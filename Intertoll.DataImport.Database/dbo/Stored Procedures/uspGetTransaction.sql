-- =============================================
-- Author:		SJ
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[uspGetTransaction] @LaneCode VARCHAR(10),@SequenceNumber INT
AS
BEGIN
	SET NOCOUNT ON;

	EXEC uspCheckMappingsExistence

    IF(NOT EXISTS(SELECT * FROM Transactions WHERE LaneCode = @LaneCode AND LaneTransSeqNr = @SequenceNumber))
    BEGIN	

		DECLARE @TransBatch udtTransactions
		
		IF(NOT EXISTS(SELECT * FROM StagingTransactions WHERE ln_id = @LaneCode AND tx_seq_nr = @SequenceNumber))
		BEGIN
			SET @SequenceNumber = @SequenceNumber -- dummy	
			-- TODO: cover case where transaction was not copied to staging table
		END	
		
		INSERT INTO @TransBatch
		SELECT TOP 5000  CASE WHEN NOT EXISTS(SELECT * FROM Transactions WHERE TransactionID = T.ln_id + CAST(T.tx_seq_nr AS VARCHAR(20))) 
							  THEN T.ln_id + CAST(T.tx_seq_nr AS VARCHAR(20)) 
							  ELSE T.ln_id + CAST(T.tx_seq_nr AS VARCHAR(20))  + 'D'
						 END --<TransactionID>
			   ,T.ln_id --<LaneCode,>
			   ,T.us_id -- <StaffID>
			   ,NEWID() --<TransGUID>
			   ,T.dt_started --<TransStartDate>
			   ,T.dt_concluded --<TransDate>
			   ,[dbo].[ufGetClassGuid]([dbo].ufRemoveNonNumericCharacters(T.mvc))	--<ColClassGUID>
			   ,[dbo].[ufGetClassGuid]([dbo].ufRemoveNonNumericCharacters(T.avc))	--<AVCClassGUID>
			   , NULL	--<RealClassGUID>
			   ,[dbo].[ufGetClassGuid]([dbo].ufRemoveNonNumericCharacters(T.mvc))	--<AppliedClassGUID>
			   ,[dbo].[ufGetTariffGuid](T.ft_id,T.pl_id,[dbo].ufRemoveNonNumericCharacters(T.mvc)) --<TariffGUID>
			   ,T.loc_value	--<TariffAmount>
			   ,[dbo].[ufCalculateTariffVAT](T.loc_value)	--<TariffVat>
			   ,0 --<ChangeAmount>
			   ,0 --<TenderedAmount>
			   ,'942190CD-5053-41B4-9A08-CD472C99ECAF' --<CurrencyGUID>
			   ,'D2710190-E72F-41FC-9ED4-C7A80E1BED30' --<LaneStatusGUID>
			   ,NULL --<SessionGUID> (STEP 2)
			   ,T.tx_seq_nr	--<LaneTransSeqNr>
			   ,ISNULL(T.avc_seq_nr,-1)	--<AVCSeqNr>
			   ,'A6C093A3-AEB1-447D-AB45-875956BB5AA4'	--<AvcStatusGUID>
			   ,[dbo].[ufGetTransactionPaymentFieldTypeGuid](pm_id,1)	--<PaymentMethodGUID>
			   ,[dbo].[ufGetTransactionPaymentFieldTypeGuid](pm_id,4)	--<PaymentGroupGUID>
			   ,[dbo].[ufGetTransactionPaymentFieldTypeGuid](pm_id,2)	--<PaymentMechGUID>
			   ,[dbo].[ufGetTransactionPaymentFieldTypeGuid](pm_id,3)	--<PaymentTypeGUID>
			   ,NULL	--<PaymentDetail> (STEP 2)
			   ,''	--<LicensePlate>
			   ,T.iv_nr	--<TaxInvNr, nvarchar(50),>
			   ,0	--<ReceiptCount>
			   ,NULL	--<AdditionalTransactionDetail>
			   ,NULL	--<AccountUserGUID>
			   ,NULL	--<SupervisorLoginGUID>
			   ,0	--<IsKeyed, bit,>
			   ,NULL	--<ImageID>
			   ,''	--<AVCDetail>
			   ,NULL	--<AccountGUID>
			   ,'5A5AE37F-AABC-428D-B4DC-2B4C87B8646C'	--<ExchangeRate>
			   ,T.loc_value	--<TotalInLocalCurrency>
			   ,NULL	--<PreviousLicensePlate>
			   ,NULL	--<PreviousPaymentMethodGUID>
			   ,(CASE WHEN T.iv_nr IS NULL THEN NULL ELSE T.dt_concluded END)	--<ReceiptTaxInvoiceDate>
			   ,''	--<ANPRLicensePlate>
			   ,(CASE WHEN ET.tx_seq_nr IS NOT NULL THEN [dbo].[ufGetETCTransactionGuid](T.tx_seq_nr,T.dt_concluded,T.ln_id) ELSE NULL END)	--<ETCTransactionGuid>
			   ,T.mask_nr	-- <CardNumber, varchar(25),>
			   ,1	--<BCCTransferStatus>
			   ,''	--<AccountIdentifier>
			   ,(CASE WHEN ET.etc_contect_mrk = '8E0080800D00' THEN 1 ELSE NULL END)	--<ContextMarkId> ,
			   ,ET.card_nr --<PAN>,
			   ,NULL --<CONV>,
			   ,ET.id_vl -- <IDVL> ,
			   ,NULL -- <VehichleState>,
			   ,0 -- <IsSent>,
			   ,GETDATE() -- [TimeStamp]
		FROM [dbo].[StagingTransactions] T
		LEFT JOIN [dbo].[StagingETCTransactions] ET ON T.ln_id = ET.ln_id and T.tx_seq_nr = ET.tx_seq_nr
		WHERE T.ln_id = @LaneCode AND T.tx_seq_nr = @SequenceNumber
		
		DECLARE @TransactionID VARCHAR(100)
		
		DECLARE @StaffGuid uniqueidentifier
		DECLARE @StaffID VARCHAR(20)
		DECLARE @timestamp DATETIME
		DECLARE @TranSeq INT
		
		SELECT TOP 1 @TransactionID = TransactionID,@StaffID = StaffID
					 ,@timestamp = TransDate,@TranSeq = LaneTransSeqNr
		FROM @TransBatch
		
		DECLARE @SessionGuid uniqueidentifier
		DECLARE @StaffLoginGuid uniqueidentifier
		DECLARE @AccountIdentifier VARCHAR(50)
		
		EXEC uspGetSessionGuidAndStaffLoginGuid @LaneCode = @LaneCode,@StaffID = @StaffID,@timestamp = @timestamp,
												@SessionGuid = @SessionGuid output, @StaffLoginGuid = @StaffLoginGuid output,
												@IsIncident = 0
		
		UPDATE @TransBatch
		SET SessionGUID = @SessionGuid
		WHERE LaneCode = @LaneCode AND LaneTransSeqNr = @TranSeq
		
		DECLARE @AccountGuid uniqueidentifier,@AccountUserGuid uniqueidentifier
		SET @AccountGuid = NULL
		SET @AccountUserGuid = NULL
		
		EXEC [uspGetAccountDetails]@AccountUserIdentifier = @AccountIdentifier, 
								   @AccountUserGUID = @AccountUserGuid output,
								   @AccountGUID = @AccountGuid output
								   
		UPDATE @TransBatch
		SET AccountGUID = @AccountGuid,AccountUserGUID = @AccountUserGuid
		WHERE LaneCode = @LaneCode AND LaneTransSeqNr = @TranSeq
		
		INSERT INTO Transactions
		SELECT [TransactionID],[LaneCode],[TransGUID],[TransStartDate],[TransDate]
				  ,[ColClassGUID],[AVCClassGUID],[RealClassGUID],[AppliedClassGUID]
				  ,[TariffGUID],[TariffAmount],[TariffVat],[ChangeAmount],[TenderedAmount]
				  ,[CurrencyGUID],[LaneStatusGUID],[SessionGUID],[LaneTransSeqNr],[AVCSeqNr]
				  ,[AvcStatusGUID],[PaymentMethodGUID],[PaymentGroupGUID],[PaymentMechGUID]
				  ,[PaymentTypeGUID],[PaymentDetail],[LicensePlate],[TaxInvNr],[ReceiptCount]
				  ,[AdditionalTransactionDetail],[AccountUserGUID],[SupervisorLoginGUID],[IsKeyed]
				  ,[ImageID],[AVCDetail],[AccountGUID],[ExchangeRate],[TotalInLocalCurrency]
				  ,[PreviousLicensePlate],[PreviousPaymentMethodGUID] ,[ReceiptTaxInvoiceDate]
				  ,[ANPRLicensePlate] ,[ETCTransactionGuid],[CardNumber],[BCCTransferStatus]
				  ,ContextMarkId,PAN,CONV,IDVL,VehichleState,[IsSent],[TimeStamp],NULL
		FROM @TransBatch
		
    END
    
    SELECT * 
	FROM Transactions 
	WHERE LaneCode = @LaneCode AND LaneTransSeqNr = @SequenceNumber 
END