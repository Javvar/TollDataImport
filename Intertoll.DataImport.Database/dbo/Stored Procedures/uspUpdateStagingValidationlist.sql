-- =============================================
-- Author:		SJ
-- Create date: 29/10
-- Description:	
-- =============================================
CREATE PROCEDURE uspUpdateStagingValidationlist
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @AddedTags TABLE([IDVL] [bigint] NOT NULL,
								[EFCMark] [bigint] NOT NULL,
								[PAN] [bigint] NOT NULL,
								[VehicleClass] [int] NOT NULL,
								[ProductType] [int] NOT NULL,
								[VLNPlateNo] [varchar](50) NOT NULL,
								[VehicleStateDate] [datetime] NOT NULL,
								[VehicleState] [varchar](50) NOT NULL)

    DECLARE @DeletedTags TABLE(IDVL INT)
    DECLARE @UpdatedTags TABLE(PAN INT)

	INSERT INTO @AddedTags
	SELECT VL.IDVL,0,VL.PAN,VL.VehicleClass,VL.ProductType
			,VL.VLNPlateNo,VL.VehicleStateDate,VL.VehicleState
	FROM [PCS].[dbo].ValidationList VL
	LEFT JOIN StagingMISValidationList SVL ON VL.PAN = SVL.PAN
	WHERE SVL.PAN IS NULL

	INSERT INTO @DeletedTags
	SELECT SVL.IDVL
	FROM StagingMISValidationList SVL 
	LEFT JOIN [PCS].[dbo].ValidationList VL ON SVL.PAN = VL.PAN
	WHERE VL.PAN IS NULL

	INSERT INTO @UpdatedTags
	SELECT VL.PAN
	FROM [PCS].[dbo].ValidationList VL
	JOIN StagingMISValidationList SVL ON VL.PAN = SVL.PAN
	WHERE VL.ProductType <> SVL.ProductType OR
	      VL.VehicleClass <> SVL.VehicleClass OR
		  VL.VehicleState <> SVL.VehicleState OR
		  VL.VLNPlateNo <> SVL.VLNPlateNo

	-- added tags
	INSERT INTO StagingMISValidationListUpdate
	SELECT IDVL,PAN,'Add',NULL 
	FROM @AddedTags

	INSERT INTO StagingMISValidationList
	SELECT IDVL,0,PAN,VehicleClass,ProductType
		   ,VLNPlateNo,VehicleStateDate,VehicleState
	FROM @AddedTags

	-- updated tags
	INSERT INTO StagingMISValidationListUpdate
	SELECT NULL,PAN,'Update',NULL 
	FROM @UpdatedTags

	UPDATE SVL
	SET SVL.IDVL = VL.IDVL,
		SVL.ProductType = VL.ProductType,
		SVL.VehicleClass = VL.VehicleClass,
		SVL.VehicleState = VL.VehicleState,
		SVL.VehicleStateDate = VL.VehicleStateDate,
		SVL.VLNPlateNo = VL.VLNPlateNo
	FROM StagingMISValidationList SVL	
	JOIN @UpdatedTags UT ON UT.PAN = SVL.PAN
	JOIN [PCS].[dbo].ValidationList VL ON VL.PAN = SVL.PAN
	
	-- deleted tags
	INSERT INTO StagingMISValidationListUpdate
	SELECT IDVL,NULL,'Delete',NULL 
	FROM @DeletedTags

	DELETE StagingMISValidationList
	WHERE IDVL IN (SELECT IDVL FROM @DeletedTags)

END