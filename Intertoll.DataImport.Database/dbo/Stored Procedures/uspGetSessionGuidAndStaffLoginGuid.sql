-- =============================================
-- Author:		SJ
-- Create date: 10/04/2018
-- Description:	Handles creation and management of sessions and staff logins
-- =============================================
CREATE PROCEDURE [dbo].[uspGetSessionGuidAndStaffLoginGuid] @LaneCode varchar(10),@StaffID varchar(20),@timestamp datetime,
												   @SessionGuid uniqueidentifier OUTPUT,
												   @StaffLoginGuid uniqueidentifier OUTPUT,
												   @IsIncident bit 
AS
BEGIN
	SET XACT_ABORT, NOCOUNT ON
	
	DECLARE @starttrancount int
	
	BEGIN TRY

        SELECT @starttrancount = @@TRANCOUNT

		IF @starttrancount = 0
			BEGIN TRANSACTION			
			
		-- Get StaffGuid
		DECLARE @StaffGuid uniqueidentifier
		SELECT @StaffGuid = StaffGuid
		FROM MappingStaff
		WHERE StaffID = @StaffID
		
		-- Get LaneGuid
		DECLARE @LaneGuid uniqueidentifier	
		SELECT TOP 1 @LaneGuid = LaneGuid
		FROM MappingLanes
		WHERE LaneCode = @LaneCode
			
		--DECLARE @CurrentOpenSessionGuid uniqueidentifier
		DECLARE @CurrentOpenSessionStartDate datetime
		
		-- Get current open ession
		SELECT @SessionGuid = SessionGUID,@StaffLoginGuid = SL.StaffLoginGUID,@CurrentOpenSessionStartDate = SL.StartDate
		FROM Sessions S 
		JOIN StaffLogins SL ON SL.StaffLoginGUID = S.StaffLoginGUID
		WHERE LaneCode = @LaneCode AND SL.EndDate IS NULL
		
		-- CASE 1: No open session
		IF(@SessionGuid IS NULL)
		BEGIN
			DECLARE @NewDataTimeStamp datetime = GETDATE() 
			
			IF(@IsIncident = 0)
			BEGIN
				EXEC uspStartNewSession @LaneCode = @LaneCode,@LaneGuid = @LaneGuid
										,@SessionGuidIn = NULL,@StartTime = @timestamp
										,@StaffGuid = @StaffGuid,@StaffLoginGuidOut = @StaffLoginGuid output
										,@SessionGuidOut = @SessionGuid output
			END
			
			GOTO RET
		END
		ELSE IF(@SessionGuid IS NOT NULL)
		BEGIN
			SET @SessionGuid = NULL
			-- CASE 6: Data belongs to closed session / No Staff (Incidents only)
			SELECT @SessionGuid = SessionGUID,@StaffLoginGuid = SL.StaffLoginGUID,@CurrentOpenSessionStartDate = SL.StartDate
			FROM Sessions S 
			JOIN StaffLogins SL ON SL.StaffLoginGUID = S.StaffLoginGUID
			WHERE LaneCode = @LaneCode AND (SL.StaffGUID = @StaffGuid OR (@IsIncident = 1 AND @StaffGuid IS NULL)) AND @timestamp BETWEEN S.StartDate AND S.EndDate 
			
			IF(@SessionGuid IS NOT NULL)
			BEGIN
				GOTO RET
			END
			
			-- CASE 2: Data belongs to the current open session / No Staff (Incidents only)
			SELECT @SessionGuid = SessionGUID,@StaffLoginGuid = SL.StaffLoginGUID,@CurrentOpenSessionStartDate = SL.StartDate
			FROM Sessions S 
			JOIN StaffLogins SL ON SL.StaffLoginGUID = S.StaffLoginGUID
			WHERE LaneCode = @LaneCode AND (SL.StaffGUID = @StaffGuid OR (@IsIncident = 1 AND @StaffGuid IS NULL)) AND @timestamp >= S.StartDate AND S.EndDate IS NULL 
			
			IF(@SessionGuid IS NOT NULL)
			BEGIN
				-- Data belongs to this session but this is a new day,so create a new session and close the currently open
				IF(CAST(@CurrentOpenSessionStartDate AS DATE) <> CAST(@timestamp AS DATE))
				BEGIN
					EXEC uspStartNewSession @LaneCode = @LaneCode
									,@LaneGuid = @LaneGuid
									,@SessionGuidIn = @SessionGuid,@StartTime = @timestamp
									,@StaffGuid = @StaffGuid,@StaffLoginGuidOut = @StaffLoginGuid output
									,@SessionGuidOut = @SessionGuid output	
				END
				
				GOTO RET
			END
					
			-- CASE 3: Data belongs to a different user
			DECLARE @CurrentSessionStaffGuid uniqueidentifier
			
			SELECT @CurrentSessionStaffGuid = SL.StaffGUID, @SessionGuid = SessionGUID
			FROM Sessions S 
			JOIN StaffLogins SL ON SL.StaffLoginGUID = S.StaffLoginGUID
			WHERE LaneCode = @LaneCode AND S.EndDate IS NULL
			
			IF(@CurrentSessionStaffGuid <> @StaffGuid)
			BEGIN
				-- end current session and staff login
				IF(@IsIncident = 0)
				BEGIN
					EXEC uspStartNewSession @LaneCode = @LaneCode,@LaneGuid = @LaneGuid
									,@SessionGuidIn = @SessionGuid,@StartTime = @timestamp
									,@StaffGuid = @StaffGuid,@StaffLoginGuidOut = @StaffLoginGuid output
									,@SessionGuidOut = @SessionGuid output
				END				
			
				GOTO Ret
			END
			
			-- CASE 5: Data is first entity of the day, same user (split sessions)
			-- CASE 6: Data is first entity of the day, different user (split sessions)		
		END		
			
RET:	IF @starttrancount = 0 
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
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState) WITH LOG;	
	END CATCH 		
END