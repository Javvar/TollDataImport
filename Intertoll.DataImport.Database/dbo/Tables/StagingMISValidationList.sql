CREATE TABLE [dbo].[StagingMISValidationList] (
    [IDVL]             BIGINT       NOT NULL,
    [EFCMark]          BIGINT       NOT NULL,
    [PAN]              BIGINT       NOT NULL,
    [VehicleClass]     INT          NOT NULL,
    [ProductType]      INT          NOT NULL,
    [VLNPlateNo]       VARCHAR (50) NOT NULL,
    [VehicleStateDate] DATETIME     NOT NULL,
    [VehicleState]     VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_StagingValidationList] PRIMARY KEY CLUSTERED ([IDVL] ASC)
);

