CREATE TABLE [dbo].[MappingStaff] (
    [StaffID]   VARCHAR (20)     NOT NULL,
    [StaffGuid] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_MappingStaff] PRIMARY KEY CLUSTERED ([StaffID] ASC)
);

