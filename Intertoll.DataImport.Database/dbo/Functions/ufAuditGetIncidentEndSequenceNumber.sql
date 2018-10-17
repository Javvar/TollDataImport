-- =============================================
-- Author:		SJ
-- Create date: 18-04-2018
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[ufAuditGetIncidentEndSequenceNumber] 
(
	@LaneCode VARCHAR(10),
	@Date DATE,
	@Hour INT
)
RETURNS int
AS
BEGIN
	DECLARE @Result int

	IF([dbo].[ufTimeSlicesComplete](@LaneCode,@Date,@Hour ) = 1)
	BEGIN
		--SELECT @Result = ISNULL(MAX(in_seq_nr),0)
		--FROM StagingIncidents
		--WHERE ln_id = @LaneCode		
		--	  AND dt_generated BETWEEN CAST(@Date AS DATETIME) + CAST([dbo].[ufGetHourTime](@Hour,1) AS DATETIME)  
		--							   AND CAST(@Date AS DATETIME) + CAST([dbo].[ufGetHourTime](@Hour,0) AS DATETIME)
		
		select @Result = ISNULL(MAX(prev_inc),0)
		from StagingTimeSlices
		where ln_id = @LaneCode and DATEPART(HOUR,dt_started) = @Hour AND CAST(dt_started AS DATE) = @Date
	END	  

	RETURN @Result

END