-- =============================================
-- Author:		SJ
-- Create date: 17-04-2018
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[uspGetNextHourlyAuditBatch](@date date = NULL)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @AuditsBatch udtAudits

	DECLARE @LaneCode VARCHAR(5)
	DECLARE @LanesCursor as CURSOR;
	SET @LanesCursor = CURSOR FOR
	SELECT LaneCode
	FROM MappingLanes
	ORDER BY LaneCode
		
	OPEN @LanesCursor;
		
	FETCH NEXT FROM @LanesCursor INTO @LaneCode;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @LastAuditDate DATE
		DECLARE @LastAuditHour INT

		DECLARE @NextAuditDate DATE
		DECLARE @NextAuditHour INT

		SELECT TOP 1 @LastAuditDate = AuditDate, @LastAuditHour = AuditHour
		FROM Audits
		WHERE LaneCode = @LAneCode
		ORDER BY AuditDate DESC, AuditHour DESC

		IF(@LastAuditDate IS NULL)
		BEGIN
			SELECT @NextAuditDate = CAST(MIN(TransDate) AS DATE),
				   @NextAuditHour = 1
			FROM Transactions
		END
		ELSE
		BEGIN
			SET @NextAuditDate = @LastAuditDate
			SET @NextAuditHour = @LastAuditHour + 1

			IF(@LastAuditHour = 23)
			BEGIN
				SET @NextAuditDate = DATEADD(DAY,1, @NextAuditDate)
				SET @NextAuditHour = 0
			END
		END

		IF(@NextAuditDate IS NOT NULL)
		BEGIN

			IF([dbo].ufSkipAudit(@LaneCode,@NextAuditDate,@NextAuditHour) = 1)
			BEGIN
				SET @NextAuditHour = @NextAuditHour + 1
			END

			IF([dbo].[ufTimeSlicesComplete] (@LaneCode,@NextAuditDate,@NextAuditHour) = 1)
			BEGIN
				INSERT INTO @AuditsBatch
				SELECT 		0
						   ,L.LaneCode + REPLACE(CAST(@NextAuditDate AS VARCHAR(20)),'-','') + CAST(@NextAuditHour AS VARCHAR(20))
						   ,L.LaneCode
						   ,L.LaneGuid
						   ,@NextAuditDate
						   ,@NextAuditHour
						   ,ISNULL([dbo].[ufAuditGetTransactionStartSequenceNumber](L.LaneCode,@NextAuditDate,@NextAuditHour),0)
						   ,ISNULL([dbo].[ufAuditGetTransactionEndSequenceNumber](L.LaneCode,@NextAuditDate,@NextAuditHour),0)
						   ,ISNULL([dbo].[ufAuditGetTransactionRecordCount](L.LaneCode,@NextAuditDate,@NextAuditHour),0)
						   ,ISNULL([dbo].[ufAuditGetTransactionDifferenceNumber](L.LaneCode,@NextAuditDate,@NextAuditHour),0)
						   ,ISNULL([dbo].[ufAuditGetIncidentStartSequenceNumber](L.LaneCode,@NextAuditDate,@NextAuditHour),0)
						   ,ISNULL([dbo].[ufAuditGetIncidentEndSequenceNumber](L.LaneCode,@NextAuditDate,@NextAuditHour),0)
						   ,ISNULL([dbo].[ufAuditGetIncidentRecordCount](L.LaneCode,@NextAuditDate,@NextAuditHour),0)
						   ,[dbo].[ufAuditGetIncidentDifferenceNumber](L.LaneCode,@NextAuditDate,@NextAuditHour)
						   ,[dbo].[ufAuditGetSessionStartSequenceNumber](L.LaneCode,@NextAuditDate,@NextAuditHour)
						   ,[dbo].[ufAuditGetSessionEndSequenceNumber](L.LaneCode,@NextAuditDate,@NextAuditHour)
						   ,[dbo].[ufAuditGetSessionRecordCount](L.LaneCode,@NextAuditDate,@NextAuditHour)
						   ,[dbo].[ufAuditGetSessionDifferenceNumber](L.LaneCode,@NextAuditDate,@NextAuditHour)
						   ,[dbo].[ufAuditGetStaffLoginRecordCount](L.LaneCode,@NextAuditDate,@NextAuditHour)
						   ,[dbo].[ufAuditGetStaffLoginDifferenceNumber](L.LaneCode,@NextAuditDate,@NextAuditHour)
						   ,0,0,0,0
						   ,(CASE WHEN [dbo].[ufTimeSlicesComplete](L.LaneCode,@NextAuditDate,@NextAuditHour) = 1 THEN GETDATE() ELSE CONVERT( DATETIME, '01 JAN 1970', 106 ) END)
						   ,0,0
				FROM MappingLanes L
				WHERE L.LaneCode = @LaneCode
			END			
		END

		FETCH NEXT FROM @LanesCursor INTO @LaneCode;
	END        
    
    SELECT *
	FROM @AuditsBatch     
END