CREATE TABLE [dbo].[MappingIncidentTypes] (
    [IncidentTypeGuid] UNIQUEIDENTIFIER NOT NULL,
    [ForeignType]      VARCHAR (50)     NOT NULL,
    [IncidentTypeCode] INT              NOT NULL
);



