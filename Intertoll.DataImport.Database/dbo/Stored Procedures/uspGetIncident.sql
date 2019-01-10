-- =============================================
-- Author:		SJ
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspGetIncident] @LaneCode VARCHAR(10),@SequenceNumber INT
AS
BEGIN
	SET NOCOUNT ON;

    IF(NOT EXISTS(SELECT * FROM Incidents WHERE LaneCode = @LaneCode AND IncidentSeqNr = @SequenceNumber))
    BEGIN
		DECLARE @IncBatch udtIncidents
		
		IF(NOT EXISTS(SELECT * FROM StagingIncidents WHERE ln_id = @LaneCode AND in_seq_nr = @SequenceNumber))
		BEGIN
			SET @SequenceNumber = @SequenceNumber -- dummy
			-- TODO: cover case where incident was not copied to staging table
		END		
		
		INSERT INTO @IncBatch
		SELECT TOP 100  
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
		WHERE ln_id = @LaneCode AND in_seq_nr = @SequenceNumber
		
		IF(EXISTS(SELECT * FROM @IncBatch))
		BEGIN
			DECLARE @StaffGuid uniqueidentifier
			DECLARE @StaffID VARCHAR(20)
			DECLARE @timestamp DATETIME
			DECLARE @IncSeq INT
			DECLARE @SessionGuid uniqueidentifier
			DECLARE @StaffLoginGuid uniqueidentifier
			DECLARE @TransactionID VARCHAR(50)
			
			SET @SessionGuid = NULL
			SET @StaffLoginGuid = NULL
			
			SELECT TOP 1 @LaneCode = LaneCode,@StaffID = StaffID,@timestamp = IncidentSetDate
					   ,@IncSeq = IncidentSeqNr,@TransactionID = TransactionID
			FROM @IncBatch
			
			-- 
			EXEC uspGetSessionGuidAndStaffLoginGuid @LaneCode = @LaneCode,@StaffID = @StaffID,@timestamp = @timestamp,
													@SessionGuid = @SessionGuid output, @StaffLoginGuid = @StaffLoginGuid output,
													@IsIncident = 1
													
			UPDATE @IncBatch
			SET StaffLoginGUID = @StaffLoginGuid
			WHERE LaneCode = @LaneCode AND IncidentSeqNr = @IncSeq
			
			-- 
			
			DECLARE @TransGuid uniqueidentifier
			SET @TransGuid = NULL
			
			SELECT @TransGuid = TransGUID
			FROM Transactions
			WHERE TransactionID = @TransactionID
			
			UPDATE @IncBatch
			SET TransactionGUID = @TransGuid
			WHERE LaneCode = @LaneCode AND IncidentSeqNr = @IncSeq
			
			-- make sure incident never precedes transactions
			DECLARE @LastTransactionDate datetime
			SELECT @LastTransactionDate = MAX(TransDate)
			FROM Transactions	
			WHERE LaneCode = @LaneCode
			
			IF(@LastTransactionDate > @timestamp)
			BEGIN
				INSERT INTO Incidents
				SELECT TOP 1 [IncidentID],[LaneCode],[IncidentSeqNr]
							  ,[IncidentCode],[IncidentSetDate],[LaneSeqNr]
							  ,[TransactionID],[StaffID],[IncidentGUID]
							  ,[StaffLoginGUID],[IncidentTypeGUID]
							  ,[IncidentStatusGUID],[Description],[TransactionGUID]
							  ,[LastIncidentSeqNr],[IsSent],[TimeStamp]
				FROM @IncBatch	
			END
		END		
	END    
		
	SELECT * 
	FROM Incidents 
	WHERE LaneCode = @LaneCode AND IncidentSeqNr = @SequenceNumber 
END