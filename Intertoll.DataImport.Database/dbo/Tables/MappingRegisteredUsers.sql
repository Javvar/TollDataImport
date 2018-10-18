CREATE TABLE [dbo].[MappingRegisteredUsers] (
    [Identifier]  VARCHAR (50)     NOT NULL,
    [AccountGuid] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_MappingRegisteredUsers] PRIMARY KEY CLUSTERED ([Identifier] ASC)
);

