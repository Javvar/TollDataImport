-- =============================================
-- Author:		SJ
-- Create date: 14-04-2018
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[uspGetNextIncidentBatch] 
AS
BEGIN
	SET XACT_ABORT, NOCOUNT ON
	
	DECLARE @starttrancount int
	
	BEGIN TRY	
		SELECT @starttrancount = @@TRANCOUNT

		IF @starttrancount = 0
			BEGIN TRANSACTION
			
			
		-- Step 1
		DECLARE @IncBatch udtIncidents
		
		DECLARE @LaneCode VARCHAR(10)
		
		DECLARE @LaneCodeCursor as CURSOR;
		SET @LaneCodeCursor = CURSOR FOR
		SELECT LaneCode
		FROM MappingLanes
		ORDER BY LaneCode
		
		OPEN @LaneCodeCursor;
		
		FETCH NEXT FROM @LaneCodeCursor INTO @LaneCode;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			
			DECLARE @LastIncDate datetime
			SELECT @LastIncDate = MAX(IncidentSetDate)
			FROM Incidents
			WHERE LaneCode = @LaneCode
			
			DECLARE @LastTransactionDate datetime
			SELECT @LastTransactionDate = MAX(TransDate)
			FROM Transactions	
			WHERE LaneCode = @LaneCode	
		
			INSERT INTO @IncBatch
			SELECT TOP 250  
					REPLACE(ln_id,' ','') + CAST(REPLACE(in_seq_nr,' ','') AS VARCHAR(20)) --[ID] [varchar](50) NOT NULL,
					,ln_id --[LaneCode] [varchar](10) NOT NULL,
					,us_id--[CollectorID] [varchar](50) NULL,
					,in_seq_nr--[IncidentSeqNr] [int] NOT NULL,
					,[dbo].[ufGetIncideCode](ir_type,ir_subtype)--[IncidentCode] [int] NOT NULL,
					,dt_generated--[IncidentSetDate] [datetime] NOT NULL,
					,tx_seq_nr--[LaneSeqNr] [int] NOT NULL,
					,REPLACE(ln_id,' ','') + CAST(REPLACE(tx_seq_nr,' ','') AS VARCHAR(20))--[TransactionID] [varchar](50) NULL,
					,NEWID() --[IncidentGUID] [uniqueidentifier] NOT NULL,
					,NULL	--[StaffLoginGUID] [uniqueidentifier] NEXT STEP,
					,[dbo].[ufGetIncidentTypeGUID](ir_type,ir_subtype)	--[IncidentTypeGUID] ,
					,'F10FE5B2-7ED3-DE11-9533-001517C991CF' --[IncidentStatusGUID] [uniqueidentifier] NOT NULL,
					,ir_type + ' ' + ir_subtype --[Description] [nvarchar](4000) NULL,
					,NULL --[TransactionGUID] next step
					,0--[LastIncidentSeqNr] [int] NULL,
					,0--[IsSent] [bit] NOT NULL,
					,GETDATE()--[TimeStamp] [datetime] NOT NULL
			FROM [dbo].[StagingIncidents]
			WHERE ln_id = @LaneCode AND 
				  (dt_generated > @LastIncDate OR @LastIncDate IS NULL) AND 
				  dt_generated <= @LastTransactionDate -- incidents should never be ahead of transactions 
			ORDER BY dt_generated
			
			FETCH NEXT FROM @LaneCodeCursor INTO @LaneCode;
		END
	
		CLOSE @LaneCodeCursor;
		DEALLOCATE @LaneCodeCursor;	
		
		
		-- step 2		
		DECLARE @StaffGuid uniqueidentifier
		DECLARE @StaffID VARCHAR(20)
		DECLARE @timestamp DATETIME
		DECLARE @IncSeq INT
		DECLARE @TransactionID VARCHAR(50)
		
		
		DECLARE @IncCursor as CURSOR;
		SET @IncCursor = CURSOR FOR
		SELECT LaneCode, IncidentSeqNr,StaffID,IncidentSetDate,TransactionID
		FROM @IncBatch
		ORDER BY IncidentSetDate
		
		OPEN @IncCursor;
		
		FETCH NEXT FROM @IncCursor INTO @LaneCode, @IncSeq, @StaffID, @timestamp,@TransactionID;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			-- step 2.1 do basic checks of transaction data completeness
			-- TODO: decide whether to proceed with the other transactions or not????
			
		
			DECLARE @SessionGuid uniqueidentifier
			DECLARE @StaffLoginGuid uniqueidentifier
			
			SET @SessionGuid = NULL
			SET @StaffLoginGuid = NULL
			
			-- step 2.2 populate staff login guid
			EXEC uspGetSessionGuidAndStaffLoginGuid @LaneCode = @LaneCode,@StaffID = @StaffID,@timestamp = @timestamp,
													@SessionGuid = @SessionGuid output, @StaffLoginGuid = @StaffLoginGuid output,
													@IsIncident = 1
													
			UPDATE @IncBatch
			SET StaffLoginGUID = @StaffLoginGuid
			WHERE LaneCode = @LaneCode AND IncidentSeqNr = @IncSeq
			
			-- 2.3 populate trans guid
			
			DECLARE @TransGuid uniqueidentifier
			SET @TransGuid = NULL
			
			SELECT @TransGuid = TransGUID
			FROM Transactions
			WHERE TransactionID = @TransactionID
			
			UPDATE @IncBatch
			SET TransactionGUID = @TransGuid
			WHERE LaneCode = @LaneCode AND IncidentSeqNr = @IncSeq
			
		
			FETCH NEXT FROM @IncCursor INTO @LaneCode, @IncSeq, @StaffID, @timestamp,@TransactionID;
		END
	
		CLOSE @IncCursor;
		DEALLOCATE @IncCursor;	
		
		DECLARE @SessionsUnSent INT
		
		SELECT @SessionsUnSent = COUNT(*) 
		FROM @IncBatch I
		JOIN StaffLogins SL ON SL.StaffLoginGUID = I.StaffLoginGUID 
		WHERE SL.IsSent = 0
		
		IF(@SessionsUnSent = 0)
		BEGIN
			INSERT INTO Incidents
			SELECT [IncidentID],[LaneCode],[IncidentSeqNr],[IncidentCode]
				  ,[IncidentSetDate],[LaneSeqNr],[TransactionID],[StaffID]
				  ,[IncidentGUID],[StaffLoginGUID],[IncidentTypeGUID]
				  ,[IncidentStatusGUID],[Description],[TransactionGUID]
				  ,[LastIncidentSeqNr],[IsSent],[TimeStamp]
			FROM @IncBatch
		END		
		
		SELECT top 5000 *,'' StaffID
		FROM Incidents
		WHERE IsSent = 0 AND IncidentTypeGUID <> 'B6AB99E8-5DA4-4C33-9EDD-08298A1A2104' 
		ORDER BY IncidentSetDate 
		
		IF @starttrancount = 0 
			COMMIT TRANSACTION
	END TRY
	BEGIN CATCH	
		IF XACT_STATE() <> 0 AND @starttrancount = 0 
			ROLLBACK TRANSACTION;	
		
		DECLARE @ErrorNumber INT = ERROR_NUMBER();
		DECLARE @ErrorLine INT = ERROR_LINE();
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();
		
		PRINT('Exception: ' + @ErrorMessage)
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState) WITH LOG;	
	END CATCH	
END
