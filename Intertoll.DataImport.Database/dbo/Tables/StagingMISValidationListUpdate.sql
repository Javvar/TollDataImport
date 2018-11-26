CREATE TABLE [dbo].[StagingMISValidationListUpdate] (
    [Id]            INT          IDENTITY (1, 1) NOT NULL,
    [IDVL]          INT          NULL,
    [PAN]           VARCHAR (50) NULL,
    [Action]        VARCHAR (10) NOT NULL,
    [DateSentToMIS] DATETIME     NULL,
    CONSTRAINT [PK_StagingMISValidationListUpdate_1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

