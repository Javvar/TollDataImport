CREATE TABLE [dbo].[StaffLogins] (
    [StaffLoginGUID]   UNIQUEIDENTIFIER NOT NULL,
    [StaffGUID]        UNIQUEIDENTIFIER NOT NULL,
    [StartDate]        DATETIME         NOT NULL,
    [EndDate]          DATETIME         NULL,
    [RoleGUID]         UNIQUEIDENTIFIER NOT NULL,
    [LocationGUID]     UNIQUEIDENTIFIER NOT NULL,
    [LocationTypeGUID] UNIQUEIDENTIFIER NOT NULL,
    [TimeStamp]        DATETIME         NOT NULL,
    [IsSent]           BIT              CONSTRAINT [DF_StaffLogins_IsSent] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_StaffLogin] PRIMARY KEY CLUSTERED ([StaffLoginGUID] ASC)
);



