CREATE TABLE [dbo].[MappingTariffs] (
    [TariffGuid]    UNIQUEIDENTIFIER NOT NULL,
    [VirtualPlaza]  VARCHAR (5)      NOT NULL,
    [Class]         INT              NOT NULL,
    [TariffTableID] INT              NOT NULL,
    CONSTRAINT [PK_TariffMappings] PRIMARY KEY CLUSTERED ([TariffGuid] ASC)
);



