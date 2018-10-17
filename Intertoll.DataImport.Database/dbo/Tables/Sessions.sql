CREATE TABLE [dbo].[Sessions] (
    [SessionGUID]    UNIQUEIDENTIFIER CONSTRAINT [DF_StaffSessions_SessionGUID] DEFAULT (newsequentialid()) ROWGUIDCOL NOT NULL,
    [SessionSeq]     INT              NOT NULL,
    [LaneCode]       VARCHAR (10)     NOT NULL,
    [StaffLoginGUID] UNIQUEIDENTIFIER NOT NULL,
    [LaneGUID]       UNIQUEIDENTIFIER NOT NULL,
    [StartDate]      DATETIME         NOT NULL,
    [EndDate]        DATETIME         NULL,
    [TimeStamp]      DATETIME         NOT NULL,
    [IsSent]         BIT              CONSTRAINT [DF_Sessions_IsSent] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED ([SessionGUID] ASC),
    CONSTRAINT [FK_Sessions_StaffLogins] FOREIGN KEY ([StaffLoginGUID]) REFERENCES [dbo].[StaffLogins] ([StaffLoginGUID])
);



