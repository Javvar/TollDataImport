-- =============================================
-- Author:		SJ
-- Create date: 18-04-2018
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[ufTimeSlicesComplete] (@LaneCode VARCHAR(10),@Date DATE,	@Hour INT)
RETURNS BIT
AS
BEGIN
	DECLARE @Result BIT = 0
	
	DECLARE @HourStart TIME 
	DECLARE @HourEnd TIME 

	SELECT @HourStart = CAST(MIN(dt_started) AS TIME)
	FROM StagingTimeSlices
	WHERE ln_id = @LaneCode 
		  AND CAST(dt_started AS DATE) = @Date AND CAST(dt_ended AS DATE) = @Date 
		  AND DATEPART(HOUR,dt_started) = @Hour AND DATEPART(HOUR,dt_ended) = @Hour
			  
	SELECT @HourEnd = CAST(MAX(dt_ended) AS TIME)
	FROM StagingTimeSlices
	WHERE ln_id = @LaneCode 
		  AND CAST(dt_started AS DATE) = @Date AND CAST(dt_ended AS DATE) = @Date 
		  AND DATEPART(HOUR,dt_started) = @Hour AND DATEPART(HOUR,dt_ended) = @Hour
		  
	
	IF(@HourStart = [dbo].[ufGetHourTime](@Hour,1) AND @HourEnd = [dbo].[ufGetHourTime](@Hour,0))
	BEGIN
		SET @Result = 1
	END	
	
	RETURN @Result

END