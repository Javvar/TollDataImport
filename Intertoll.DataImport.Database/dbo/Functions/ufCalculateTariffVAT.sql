-- =============================================
-- Author:		SJ
-- Create date: 10/04/2018
-- Description:	Calculates VAT from tariff amount
-- =============================================
CREATE FUNCTION [dbo].[ufCalculateTariffVAT](@Amount money)
RETURNS money
AS
BEGIN

	DECLARE @Result money
	
	SET @Result = 0.13043 * @Amount

	RETURN @Result

END