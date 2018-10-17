-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ufGetIncideCode] (@ir_type varchar(5),@ir_subtype varchar(5))
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result INT
	
	SET @Result = 0

	RETURN @Result
END