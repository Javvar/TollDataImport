-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ufSkipAudit]
(
	@LaneCode VARCHAR(10),
	@Date DATE,
	@Hour INT
)
RETURNS BIT
AS
BEGIN
	DECLARE @Result BIT 

	IF(@LaneCode = '87rs' ANd @Date = '2018-10-25' AND @Hour = 0)
	BEGIN
		SET @Result = 1
	END

	RETURN @Result
END