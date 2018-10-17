-- =============================================
-- Author:		SJ
-- Create date: 10/04/2018
-- Description:	Determines Tariff Guid From Amount based on date of transaction
-- =============================================
CREATE FUNCTION [dbo].[ufGetTariffGuid] (@TariffTableID int, @Vp varchar(5), @Class int)
RETURNS uniqueidentifier
AS
BEGIN
	DECLARE @Result uniqueidentifier
	
	SELECT @Result = TariffGuid
	FROM MappingTariffs
	WHERE VirtualPlaza = @Vp AND 
		  Class = @Class AND 
		  TariffTableID = @TariffTableID
	

	RETURN @Result
END