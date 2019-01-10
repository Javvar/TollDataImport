-- =============================================
-- Author:		SJ
-- Create date: 18-04-2018
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[ufGetHourTime](@Hour INT,@Start BIT)
RETURNS TIME
AS
BEGIN
	DECLARE @Result TIME

	IF(@Start = 1)
	BEGIN
		SET @Result = RIGHT('0' + CAST(@Hour AS VARCHAR), 2) + ':' + '00:00'
	END
	ELSE
	BEGIN
		SET @Result = RIGHT('0' + CAST(@Hour AS VARCHAR), 2) + ':' + '59:59'
	END
	RETURN @Result

END