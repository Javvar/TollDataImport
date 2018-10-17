-- =============================================
-- Author:		SJ
-- Create date: 17-04-2018
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[uspGetNextHourlyAuditBatch](@date date = NULL)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @Hours table([Hour] INT)
	INSERT INTO @Hours
	VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8),(9),(10),
		   (11),(12),(13),(14),(15),(16),(17),(18),
		   (19),(20),(21),(22),(23)

    
    IF(@date IS NULL)
    BEGIN
		IF(EXISTS(SELECT * FROM Audits WHERE AuditDate < CAST(GETDATE() AS DATE)))
		BEGIN
			SELECT @date = MAX(AuditDate)
			FROM Audits
			WHERE AuditDate < CAST(GETDATE() AS DATE)
		END
		ELSE 
		IF(EXISTS(SELECT * FROM Transactions))
		BEGIN
			SELECT @date = MIN(TransDate)
			FROM Transactions
		END
		ELSE
			SET @date = CAST(GETDATE() AS DATE)
    END		
    
    IF(@date = CAST(GETDATE() AS DATE))
    BEGIN
		DELETE @Hours
		WHERE Hour < DATEPART(HOUR,GETDATE())
    END
    
    DECLARE @AuditsBatch udtAudits
    
    INSERT INTO @AuditsBatch
    SELECT TOP 100 --L.LaneCode,L.LaneGuid,Hour
    
				0
			   ,L.LaneCode + REPLACE(CAST(@date AS VARCHAR(20)),'-','') + CAST(Hour AS VARCHAR(20))
			   ,L.LaneCode
			   ,L.LaneGuid
			   ,@date
			   ,Hour
			   ,ISNULL([dbo].[ufAuditGetTransactionStartSequenceNumber](L.LaneCode,@date,Hour),0)
			   ,ISNULL([dbo].[ufAuditGetTransactionEndSequenceNumber](L.LaneCode,@date,Hour),0)
			   ,ISNULL([dbo].[ufAuditGetTransactionRecordCount](L.LaneCode,@date,Hour),0)
			   ,ISNULL([dbo].[ufAuditGetTransactionDifferenceNumber](L.LaneCode,@date,Hour),0)
			   ,ISNULL([dbo].[ufAuditGetIncidentStartSequenceNumber](L.LaneCode,@date,Hour),0)
			   ,ISNULL([dbo].[ufAuditGetIncidentEndSequenceNumber](L.LaneCode,@date,Hour),0)
			   ,ISNULL([dbo].[ufAuditGetIncidentRecordCount](L.LaneCode,@date,Hour),0)
			   ,[dbo].[ufAuditGetIncidentDifferenceNumber](L.LaneCode,@date,Hour)
			   ,[dbo].[ufAuditGetSessionStartSequenceNumber](L.LaneCode,@date,Hour)
			   ,[dbo].[ufAuditGetSessionEndSequenceNumber](L.LaneCode,@date,Hour)
			   ,[dbo].[ufAuditGetSessionRecordCount](L.LaneCode,@date,Hour)
			   ,[dbo].[ufAuditGetSessionDifferenceNumber](L.LaneCode,@date,Hour)
			   ,[dbo].[ufAuditGetStaffLoginRecordCount](L.LaneCode,@date,Hour)
			   ,[dbo].[ufAuditGetStaffLoginDifferenceNumber](L.LaneCode,@date,Hour)
			   ,0,0,0,0
			   ,(CASE WHEN [dbo].[ufTimeSlicesComplete](L.LaneCode,@date,Hour) = 1 THEN GETDATE() ELSE CONVERT( DATETIME, '01 JAN 1970', 106 ) END)
			   ,0,0
	FROM MappingLanes L
	CROSS JOIN @Hours
	LEFT JOIN Audits A ON A.LaneGuid = L.LaneGuid 
						  AND Hour = A.AuditHour
						  AND A.AuditDate = @date
	WHERE A.Id IS NULL	
	ORDER BY L.LaneCode
    
    

	--DECLARE @LaneCode VARCHAR(10)	
	--DECLARE @LaneGuid uniqueidentifier	
	--DECLARE @Hour int	
	
	--DECLARE @AudCurs as CURSOR;
	--SET @AudCurs = CURSOR FOR
	--SELECT TOP 100 L.LaneCode,L.LaneGuid,Hour
	--FROM MappingLanes L
	--CROSS JOIN @Hours
	--LEFT JOIN Audits A ON A.LaneGuid = L.LaneGuid 
	--					  AND Hour = A.AuditHour
	--					  AND A.AuditDate = @date
	--WHERE A.Id IS NULL	
	--ORDER BY LaneCode
	
	
	
	--OPEN @AudCurs;
	
	--FETCH NEXT FROM @AudCurs INTO @LaneCode, @LaneGuid,@Hour;
	--WHILE @@FETCH_STATUS = 0
	--BEGIN
		
	--	IF(NOT EXISTS(SELECT * FROM Audits WHERE LaneCode=@LaneCode AND AuditDate = @date AND AuditHour = @Hour))
	--		BEGIN
	--			IF([dbo].[ufTimeSlicesComplete](@LaneCode,@date,@Hour) = 1)
	--			BEGIN
	--				INSERT INTO @AuditsBatch
	--				VALUES(0
	--					   ,@LaneCode + REPLACE(CAST(@date AS VARCHAR(20)),'-','') + CAST(@Hour AS VARCHAR(20))
	--					   ,@LaneCode
	--					   ,@LaneGuid
	--					   ,@date
	--					   ,@Hour
	--					   ,[dbo].[ufAuditGetTransactionStartSequenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetTransactionEndSequenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetTransactionRecordCount](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetTransactionDifferenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetIncidentStartSequenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetIncidentEndSequenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetIncidentRecordCount](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetIncidentDifferenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetSessionStartSequenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetSessionEndSequenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetSessionRecordCount](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetSessionDifferenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetStaffLoginRecordCount](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetStaffLoginDifferenceNumber](@LaneCode,@date,@Hour)
	--					   ,0,0,0,0,GETDATE(),0,0)
	--			END	
	--			ELSE
	--			BEGIN
	--				INSERT INTO @AuditsBatch
	--				VALUES(0
	--					   ,@LaneCode + REPLACE(CAST(@date AS VARCHAR(20)),'-','') + CAST(@Hour AS VARCHAR(20))
	--					   ,@LaneCode
	--					   ,@LaneGuid
	--					   ,@date
	--					   ,@Hour
	--					   ,ISNULL([dbo].[ufAuditGetTransactionStartSequenceNumber](@LaneCode,@date,@Hour),0)
	--					   ,ISNULL([dbo].[ufAuditGetTransactionEndSequenceNumber](@LaneCode,@date,@Hour),0)
	--					   ,ISNULL([dbo].[ufAuditGetTransactionRecordCount](@LaneCode,@date,@Hour),0)
	--					   ,ISNULL([dbo].[ufAuditGetTransactionDifferenceNumber](@LaneCode,@date,@Hour),0)
	--					   ,ISNULL([dbo].[ufAuditGetIncidentStartSequenceNumber](@LaneCode,@date,@Hour),0)
	--					   ,ISNULL([dbo].[ufAuditGetIncidentEndSequenceNumber](@LaneCode,@date,@Hour),0)
	--					   ,ISNULL([dbo].[ufAuditGetIncidentRecordCount](@LaneCode,@date,@Hour),0)
	--					   ,[dbo].[ufAuditGetIncidentDifferenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetSessionStartSequenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetSessionEndSequenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetSessionRecordCount](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetSessionDifferenceNumber](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetStaffLoginRecordCount](@LaneCode,@date,@Hour)
	--					   ,[dbo].[ufAuditGetStaffLoginDifferenceNumber](@LaneCode,@date,@Hour)
	--					   ,0,0,0,0,CONVERT( DATETIME, '01 JAN 1970', 106 ),0,0)
	--			END
	--		END				
	
	--	FETCH NEXT FROM @AudCurs INTO @LaneCode, @LaneGuid,@Hour;
	--END

	--CLOSE @AudCurs;
	--DEALLOCATE @AudCurs;
	
	SELECT *
	FROM @AuditsBatch
   
    
END