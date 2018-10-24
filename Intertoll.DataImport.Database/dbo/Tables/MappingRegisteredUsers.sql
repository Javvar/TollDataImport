CREATE TABLE [dbo].[MappingRegisteredUsers] (
    [Identifier]      VARCHAR (50)     NOT NULL,
    [AccountGuid]     UNIQUEIDENTIFIER NOT NULL,
    [AccountUserGuid] UNIQUEIDENTIFIER NOT NULL,
    [Reported]        BIT              CONSTRAINT [DF_MappingRegisteredUsers_Reported] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_MappingRegisteredUsers] PRIMARY KEY CLUSTERED ([Identifier] ASC)
);





