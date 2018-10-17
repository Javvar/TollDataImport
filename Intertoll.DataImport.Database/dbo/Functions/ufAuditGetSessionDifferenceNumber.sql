-- =============================================
-- Author:		SJ
-- Create date: 18-04-2018
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[ufAuditGetSessionDifferenceNumber]
(
	@LaneCode VARCHAR(10),
	@Date DATE,
	@Hour INT
)
RETURNS int
AS
BEGIN
	DECLARE @Result int

	SET @Result = 1

	RETURN @Result

END