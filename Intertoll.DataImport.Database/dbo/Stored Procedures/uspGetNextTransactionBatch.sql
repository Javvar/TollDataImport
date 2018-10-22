-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspGetNextTransactionBatch] 
AS
BEGIN	
	
	SET XACT_ABORT, NOCOUNT ON
	
	DECLARE @starttrancount int
	
	BEGIN TRY	
		SELECT @starttrancount = @@TRANCOUNT

		IF @starttrancount = 0
			BEGIN TRANSACTION
		
		DECLARE @TransBatch udtTransactions
		
		-- Step 1
		DECLARE @LastTransactionDate datetime
		SELECT @LastTransactionDate = MAX(TransDate) --ISNULL(MAX(TransDate),'2017-01-24 19:00:00')
		FROM Transactions
		
		INSERT INTO @TransBatch
		SELECT TOP 5000 CASE WHEN NOT EXISTS(SELECT * FROM Transactions WHERE TransactionID = T.ln_id + CAST(T.tx_seq_nr AS VARCHAR(20))) 
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
			   ,T.card_nr	--<PaymentDetail> (STEP 2)
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
			   ,'' -- <VehichleState>,
			   ,0 -- <IsSent>,
			   ,GETDATE() -- [TimeStamp]
		FROM [dbo].[StagingTransactions] T
		LEFT JOIN [dbo].[StagingETCTransactions] ET ON T.ln_id = ET.ln_id and T.tx_seq_nr = ET.tx_seq_nr
		WHERE T.dt_concluded > @LastTransactionDate OR @LastTransactionDate IS NULL 
		ORDER BY T.dt_concluded
		
		-- step 2
		DECLARE @TransactionID VARCHAR(100)
		DECLARE @TransactionIDs TABLE(ID VARCHAR(100))
		
		DECLARE @StaffGuid uniqueidentifier
		DECLARE @LaneCode VARCHAR(10)
		DECLARE @StaffID VARCHAR(20)
		DECLARE @AccountIdentifier VARCHAR(50)
		DECLARE @timestamp DATETIME
		DECLARE @TranSeq INT	
		DECLARE @TransactionGuid uniqueidentifier
		
		DECLARE @TransCursor as CURSOR;
		SET @TransCursor = CURSOR FOR
		SELECT LaneCode, LaneTransSeqNr,StaffID,TransDate,AccountIdentifier,TransGUID
		FROM @TransBatch
		ORDER BY TransDate
		
		OPEN @TransCursor;
		
		FETCH NEXT FROM @TransCursor INTO @LaneCode, @TranSeq, @StaffID, @timestamp,@AccountIdentifier,@TransactionGuid;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			-- step 2.1 do basic checks of transaction data completeness
			-- TODO: decide whether to proceed with the other transactions or not????
			--IF(NOT EXISTS(SELECT * FROM MappingStaff WHERE StaffID = @StaffID))
			--BEGIN
			--	DECLARE @ErrorMsg VARCHAR(100) 
			--	SET @ErrorMsg = 'Transaction: ' + @LaneCode + ' ' + @TranSeq + ' failed a data compleness test!'
			--	RAISERROR(@ErrorMsg,20,-1) 
			--END
		
			DECLARE @SessionGuid uniqueidentifier
			DECLARE @StaffLoginGuid uniqueidentifier
			
			SET @SessionGuid = NULL
			SET @StaffLoginGuid = NULL
			
			-- step 2.2 populate session guid
			EXEC uspGetSessionGuidAndStaffLoginGuid @LaneCode = @LaneCode,@StaffID = @StaffID,@timestamp = @timestamp,
													@SessionGuid = @SessionGuid output, @StaffLoginGuid = @StaffLoginGuid output,
													@IsIncident = 0
													
			UPDATE @TransBatch
			SET SessionGUID = @SessionGuid
			WHERE LaneCode = @LaneCode AND LaneTransSeqNr = @TranSeq	 
			
			-- step 2.3 populate accountuserguid and accountguid
			DECLARE @AccountGuid uniqueidentifier,@AccountUserGuid uniqueidentifier
			SET @AccountGuid = NULL
			SET @AccountUserGuid = NULL
			
			EXEC [uspGetAccountDetails]@AccountUserIdentifier = @AccountIdentifier, 
									   @AccountUserGUID = @AccountUserGuid output,
									   @AccountGUID = @AccountGuid output
									   
			UPDATE @TransBatch
			SET AccountGUID = @AccountGuid,AccountUserGUID = @AccountUserGuid
			WHERE LaneCode = @LaneCode AND LaneTransSeqNr = @TranSeq	
			
			-- Check for transaction duplication
			SELECT @TransactionID = TransactionID
			FROM @TransBatch				
			WHERE LaneCode = @LaneCode AND LaneTransSeqNr = @TranSeq				
			
			IF(NOT EXISTS(SELECT * FROM @TransactionIDs WHERE ID = @TransactionID))
			BEGIN
				INSERT INTO @TransactionIDs
				VALUES(@TransactionID)
			END
			ELSE
			BEGIN
				/**/
				SET @TransactionID = @TransactionID + 'D'
				
				UPDATE @TransBatch
				SET AccountGUID = @AccountGuid,AccountUserGUID = @AccountUserGuid,TransactionID = @TransactionID
				WHERE TransGUID = @TransactionGuid
				
				INSERT INTO @TransactionIDs
				VALUES(@TransactionID + 'D')
				
				/*DELETE @TransBatch
				WHERE TransGUID = (SELECT TOP 1 TransGUID FROM @TransBatch WHERE LaneCode = @LaneCode AND LaneTransSeqNr = @TranSeq)*/
				
			END				
		 
			FETCH NEXT FROM @TransCursor INTO @LaneCode, @TranSeq, @StaffID, @timestamp,@AccountIdentifier,@TransactionGuid;
		END
		
		CLOSE @TransCursor;
		DEALLOCATE @TransCursor;
		
		IF @starttrancount = 0 
			COMMIT TRANSACTION			
			
		DECLARE @SessionsUnSent INT
		
		SELECT @SessionsUnSent = COUNT(*) 
		FROM @TransBatch T
		JOIN Sessions S ON S.SessionGUID = T.SessionGUID
		JOIN StaffLogins SL ON SL.StaffLoginGUID = S.StaffLoginGUID 
		WHERE S.IsSent = 0 OR SL.IsSent = 0
		
		IF(@SessionsUnSent = 0)
		BEGIN
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
				  ,ContextMarkId,PAN,CONV,IDVL,VehichleState,[IsSent],[TimeStamp],''
			FROM @TransBatch
			ORDER BY TransactionID			
		END	
		
		SELECT top 5000 *,'' StaffID,'' AccountIdentifier
		FROM Transactions
		WHERE IsSent = 0 AND TransactionID NOT IN (SELECT TransactionID -- exclude duplicate transactions already detected
												   FROM Transactions 
												   WHERE TransactionID LIKE '%D' 
														 AND TimeStamp > DATEADD(DAY,-1,GETDATE()))
		ORDER BY TransDate	
	END TRY
	BEGIN CATCH	
		IF XACT_STATE() <> 0 AND @starttrancount = 0 
			ROLLBACK TRANSACTION;	
		
		DECLARE @ErrorNumber INT = ERROR_NUMBER();
		DECLARE @ErrorLine INT = ERROR_LINE();
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		PRINT('Exception ' + @ErrorMessage)
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState) WITH LOG;	
	END CATCH 	
END
