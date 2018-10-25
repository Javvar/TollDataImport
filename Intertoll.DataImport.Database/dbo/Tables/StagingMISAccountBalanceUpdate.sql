CREATE TABLE [dbo].[StagingMISAccountBalanceUpdate] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [MISAccountNr]   VARCHAR (50)     COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [PCSAccountGuid] UNIQUEIDENTIFIER NOT NULL,
    [OldBalance]     MONEY            NOT NULL,
    [NewBalance]     MONEY            NOT NULL,
    [DateCreated]    DATETIME         NOT NULL,
    [DateSentToMIS]  DATETIME         NULL,
    CONSTRAINT [PK_StagingMISAccountBalanceUpdate] PRIMARY KEY CLUSTERED ([Id] ASC)
);

