CREATE TABLE [dbo].[StagingMISHotlistUpdates] (
    [Id]            INT          IDENTITY (1, 1) NOT NULL,
    [CardNr]        VARCHAR (50) NOT NULL,
    [Change]        VARCHAR (20) NOT NULL,
    [DateSentToMIS] DATETIME     NULL,
    CONSTRAINT [PK_StagingMISHotlistUpdates] PRIMARY KEY CLUSTERED ([Id] ASC)
);

