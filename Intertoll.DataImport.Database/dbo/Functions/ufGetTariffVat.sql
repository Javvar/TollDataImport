-- =============================================
-- Author:		SJ
-- Create date: 10/04/2018
-- Description:	Determines Tariff Guid From Amount based on date of transaction
-- =============================================
CREATE FUNCTION [dbo].[ufGetTariffVat] (@ln varchar(5), @Class int,@date datetime)
RETURNS money
AS
BEGIN
	DECLARE @Result money
	
	--SELECT @Result = TariffGuid
	--FROM MappingTariffs
	--WHERE VirtualPlaza = @Vp AND 
	--	  Class = @Class AND 
	--	  TariffTableID = @TariffTableID

	DECLARE @VP uniqueidentifier

	SELECT @VP = VirtualPlazaGUID
	FRom PCS.dbo.Lanes
	WHERE LaneCode = @ln

	SELECT @Result = TariffVAT
	FROM PCS.dbo.Tariffs T
	JOIN PCS.dbo.TariffSchedules TS ON TS.TariffScheduleGUID = T.TariffScheduleGUID 
													   AND TS.TariffScheduleGUID = (SELECT TOP 1 TS1.TariffScheduleGUID 
																					FROM PCS.dbo.TariffSchedules TS1
																					WHERE TS1.EffectiveDate < @date
																					ORDER BY TS1.EffectiveDate DESC)
	JOIN PCS.dbo.Classes C ON C.ClassGUID = T.ClassGUID
	WHERE T.VirtualPlazaGUID = @Vp AND C.ClassCode = @Class
	

	RETURN @Result
END