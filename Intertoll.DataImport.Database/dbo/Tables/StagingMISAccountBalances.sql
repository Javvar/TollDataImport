CREATE TABLE [dbo].[StagingMISAccountBalances] (
    [MISAccountNr]   VARCHAR (50)     NOT NULL,
    [PCSAccountGuid] UNIQUEIDENTIFIER NOT NULL,
    [MISBalance]     MONEY            NOT NULL,
    CONSTRAINT [PK_StagingMISAccountBalances] PRIMARY KEY CLUSTERED ([MISAccountNr] ASC)
);



