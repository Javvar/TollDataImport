CREATE TABLE [dbo].[Incidents] (
    [IncidentID]         VARCHAR (50)     NOT NULL,
    [LaneCode]           VARCHAR (10)     NOT NULL,
    [IncidentSeqNr]      INT              NULL,
    [IncidentCode]       INT              NULL,
    [IncidentSetDate]    DATETIME         NOT NULL,
    [LaneSeqNr]          INT              NOT NULL,
    [TransactionID]      VARCHAR (50)     NULL,
    [StaffID]            VARCHAR (50)     NULL,
    [IncidentGUID]       UNIQUEIDENTIFIER NOT NULL,
    [StaffLoginGUID]     UNIQUEIDENTIFIER NULL,
    [IncidentTypeGUID]   UNIQUEIDENTIFIER NULL,
    [IncidentStatusGUID] UNIQUEIDENTIFIER NOT NULL,
    [Description]        NVARCHAR (4000)  NULL,
    [TransactionGUID]    UNIQUEIDENTIFIER NULL,
    [LastIncidentSeqNr]  INT              NULL,
    [IsSent]             BIT              NOT NULL,
    [TimeStamp]          DATETIME         NOT NULL,
    CONSTRAINT [PK_Incidents] PRIMARY KEY CLUSTERED ([IncidentID] ASC)
);



