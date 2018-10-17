-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [ufCombineDateAndTime](@date Date,@Time Time)
RETURNS DATETIME
AS
BEGIN
	RETURN CAST(@date AS DATETIME) + CAST(@Time AS DATETIME) 

END