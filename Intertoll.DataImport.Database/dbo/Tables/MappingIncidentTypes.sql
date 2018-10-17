CREATE TABLE [dbo].[MappingIncidentTypes] (
    [IncidentTypeGuid] UNIQUEIDENTIFIER NOT NULL,
    [ForeignType]      VARCHAR (50)     NOT NULL,
    [IncidentTypeCode] INT              NOT NULL,
    CONSTRAINT [PK_MappingIncidents] PRIMARY KEY CLUSTERED ([IncidentTypeGuid] ASC)
);

