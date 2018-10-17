-- =============================================
-- Author:		SJ
-- Create date: 18-04-2018
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[ufAuditGetTransactionRecordCount](@LaneCode VARCHAR(10),@Date DATE,@Hour INT)
RETURNS int
AS
BEGIN
	DECLARE @Result int
	
	IF([dbo].[ufTimeSlicesComplete](@LaneCode,@Date,@Hour) = 1)
	BEGIN
		SELECT @Result = ISNULL(SUM(tx_count),0)
		FROM StagingTimeSlices
		WHERE ln_id = @LaneCode 
			  AND CAST(dt_started AS DATE) = @Date AND CAST(dt_ended AS DATE) = @Date 
			  AND DATEPART(HOUR,dt_started) = @Hour AND DATEPART(HOUR,dt_ended) = @Hour 
	END  

	RETURN @Result

END