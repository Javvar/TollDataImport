-- =============================================
-- Author:		SJ
-- Create date: 10/04/2018
-- Description:	Ends current session and start new one
-- =============================================
CREATE PROCEDURE [dbo].[uspStartNewSession] @LaneCode varchar(10),@LaneGuid uniqueidentifier
									,@SessionGuidIn uniqueidentifier,@StartTime datetime
									,@StaffGuid uniqueidentifier,@StaffLoginGuidOut uniqueidentifier OUTPUT
									,@SessionGuidOut uniqueidentifier OUTPUT
AS
BEGIN

	SET XACT_ABORT, NOCOUNT ON
	
	DECLARE @starttrancount int
	
	BEGIN TRY

        SELECT @starttrancount = @@TRANCOUNT

		IF @starttrancount = 0
			BEGIN TRANSACTION
		
		IF(@SessionGuidIn IS NULL AND (@LaneGuid IS NULL OR @StaffGuid IS NULL OR @LaneCode IS NULL))
		BEGIN
			DECLARE @message VARCHAR(1000) = 'Supplied paramemters are not complete' + @laneCode + CAST(@StartTime AS varchar(100))
			RAISERROR(@message,20,-1) WITH LOG
		END
		
		
		DECLARE @NewDataTimeStamp datetime = GETDATE()
			
		IF(@SessionGuidIn IS NOT NULL)
		BEGIN
			DECLARE @EndDate DATETIME
			
			SELECT @EndDate = (CASE WHEN DATEPART(DAY,Sess.StartDate) = DATEPART(DAY,@StartTime) THEN DATEADD(SECOND,-1,@StartTime)		
								    ELSE DATEADD(SECOND,-1,DATEADD(DAY,1,CAST(CAST(Sess.StartDate AS DATE) AS DATETIME)))
							   END)	
			FROM Sessions Sess
			WHERE SessionGUID = @SessionGuidIn
		
			UPDATE Sess
			SET EndDate = @EndDate,IsSent = 0
			FROM Sessions Sess
			WHERE SessionGUID = @SessionGuidIn
			
			UPDATE SL
			SET EndDate = @EndDate
			FROM Sessions Sess
			JOIN StaffLogins SL ON SL.StaffLoginGUID = Sess.StaffLoginGUID
			WHERE SessionGUID = @SessionGuidIn
		END
		
		SET @StaffLoginGuidOut = NEWID()
		
		INSERT INTO StaffLogins
		VALUES (@StaffLoginGuidOut,@StaffGuid,@StartTime,NULL,
				'882ABFF5-1FF9-46DC-B130-44EFF2A2337F',@LaneGuid,
				'69CD1EE2-909C-44AA-9395-97A3F3D5F13A',@NewDataTimeStamp,0)
		
		SET @SessionGuidOut = NEWID()
		DECLARE @newsessionseq int
		
		SELECT TOP 1 @newsessionseq = SessionSeq
		FROM Sessions
		WHERE LaneCode = @LaneCode
		ORDER BY StartDate DESC 
		
		SET @newsessionseq = (CASE WHEN @newsessionseq IS NULL THEN 0 ELSE @newsessionseq + 1 END)
			
		INSERT INTO Sessions
		VALUES(@SessionGuidOut,@newsessionseq,@LaneCode,@StaffLoginGuidOut,@LaneGuid
				,@StartTime,NULL,@NewDataTimeStamp,0)	

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
		
		PRINT('Exception ' + @ErrorMessage)
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH    
END