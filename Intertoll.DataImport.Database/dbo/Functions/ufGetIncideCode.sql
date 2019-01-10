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
	
	SELECT @Result = IncidentTypeCode
	FROM MappingIncidentTypes
	WHERE ForeignType = REPLACE(@ir_type + @ir_subtype,' ','')
	
	IF(@Result IS NULL OR @Result = '')
	BEGIN
		SET @Result = 0
	END	

	RETURN @Result
END