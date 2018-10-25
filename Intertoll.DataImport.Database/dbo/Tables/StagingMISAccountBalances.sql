CREATE TABLE [dbo].[StagingMISAccountBalances] (
    [MISAccountNr]   VARCHAR (50)     COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [PCSAccountGuid] UNIQUEIDENTIFIER NOT NULL,
    [MISBalance]     MONEY            NOT NULL,
    CONSTRAINT [PK_StagingMISAccountBalances] PRIMARY KEY CLUSTERED ([MISAccountNr] ASC)
);

