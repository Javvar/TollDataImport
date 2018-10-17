-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ufGetIncidentTypeGUID] (@ir_type varchar(5),@ir_subtype varchar(5))
RETURNS uniqueidentifier
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result uniqueidentifier
	
	SELECT @Result = IncidentTypeGuid
	FROM MappingIncidentTypes
	WHERE ForeignType = REPLACE(@ir_type + @ir_subtype,' ','')
	
	IF(@Result IS NULL)
	BEGIN
		SET @Result = 'B6AB99E8-5DA4-4C33-9EDD-08298A1A2104'
	END	

	RETURN @Result
END