CREATE TABLE [dbo].[MappingTarrifs] (
    [TariffGuid]    UNIQUEIDENTIFIER NOT NULL,
    [VirtualPlaza]  INT              NOT NULL,
    [Class]         INT              NOT NULL,
    [Amount]        MONEY            NOT NULL,
    [EffectiveDate] DATETIME         NOT NULL,
    CONSTRAINT [PK_TariffMappings] PRIMARY KEY CLUSTERED ([TariffGuid] ASC)
);

