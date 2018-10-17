-- =============================================
-- Author:		SJ
-- Create date: 18-04-2018
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[ufAuditGetIncidentStartSequenceNumber] 
(
	@LaneCode VARCHAR(10),
	@Date DATE,
	@Hour INT
)
RETURNS INT
AS
BEGIN
	DECLARE @Result int
	
	IF([dbo].[ufTimeSlicesComplete](@LaneCode,@Date,@Hour ) = 1)
	BEGIN
		--SELECT @Result = ISNULL(MIN(in_seq_nr),0)
		--FROM StagingIncidents
		--WHERE ln_id = @LaneCode		
		--	  AND dt_generated BETWEEN [dbo].[ufCombineDateAndTime](@Date, [dbo].[ufGetHourTime](@Hour,1)) 
		--								AND [dbo].[ufCombineDateAndTime](@Date, [dbo].[ufGetHourTime](@Hour,0))
		
		select @Result = ISNULL(MIN(next_inc),0)
		from StagingTimeSlices
		where ln_id = @LaneCode and DATEPART(HOUR,dt_started) = @Hour AND CAST(dt_started AS DATE) = @Date
	END	  

	RETURN @Result

END