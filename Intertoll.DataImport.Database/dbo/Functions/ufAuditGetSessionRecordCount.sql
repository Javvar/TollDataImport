-- =============================================
-- Author:		SJ
-- Create date: 18-04-2018
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[ufAuditGetSessionRecordCount](@LaneCode VARCHAR(10),@Date DATE,@Hour INT)
RETURNS INT
AS
BEGIN
	DECLARE @Result int = 0	

	RETURN @Result

END