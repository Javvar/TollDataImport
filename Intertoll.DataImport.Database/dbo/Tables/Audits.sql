CREATE TABLE [dbo].[Audits] (
    [Id]                         INT              NOT NULL,
    [AuditID]                    VARCHAR (50)     NOT NULL,
    [LaneCode]                   VARCHAR (10)     NOT NULL,
    [LaneGuid]                   UNIQUEIDENTIFIER NOT NULL,
    [AuditDate]                  DATE             NOT NULL,
    [AuditHour]                  INT              NOT NULL,
    [TransStartSeqNumber]        INT              NOT NULL,
    [TransEndSeqNumber]          INT              NOT NULL,
    [TransRecordCount]           INT              NOT NULL,
    [TransDifferenceNumber]      INT              NOT NULL,
    [IncidentStartSeqNumber]     INT              NOT NULL,
    [IncidentEndSeqNumber]       INT              NOT NULL,
    [IncidentRecordCount]        INT              NOT NULL,
    [IncidentDifferenceNumber]   INT              NOT NULL,
    [SessionStartSeqNumber]      INT              NOT NULL,
    [SessionEndSeqNumber]        INT              NOT NULL,
    [SessionRecordCount]         INT              NOT NULL,
    [SessionDifferenceNumber]    INT              NOT NULL,
    [StaffLoginRecordCount]      INT              NOT NULL,
    [StaffLoginDifferenceNumber] INT              NOT NULL,
    [TransAuditStatus]           INT              NOT NULL,
    [IncidentAuditStatus]        INT              NOT NULL,
    [SessionAuditStatus]         INT              NOT NULL,
    [StaffLoginAuditStatus]      INT              NOT NULL,
    [TimeStamp]                  DATETIME         NOT NULL,
    [LaneMode]                   INT              NOT NULL,
    [IsSent]                     BIT              CONSTRAINT [DF_Audits_IsSent] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Audits_1] PRIMARY KEY CLUSTERED ([AuditID] ASC)
);



