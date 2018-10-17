-- =============================================
-- Author:		SJ
-- Create date: 18-04-2018
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[ufAuditGetTransactionDifferenceNumber]
(
	@LaneCode VARCHAR(10),
	@Date DATE,
	@Hour INT
)
RETURNS int
AS
BEGIN
	DECLARE @Result int

	SET @Result = [dbo].[ufAuditGetTransactionEndSequenceNumber](@LaneCode,@Date,@Hour) - 
				  [dbo].[ufAuditGetTransactionStartSequenceNumber](@LaneCode,@Date,@Hour)
	
	SET @Result = CASE WHEN ISNULL(@Result,0) = -1 THEN 0 ELSE ISNULL(@Result,0) END

	RETURN @Result

END