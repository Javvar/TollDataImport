CREATE TABLE [dbo].[MappingLanes] (
    [LaneCode]         VARCHAR (10)     NOT NULL,
    [LaneGuid]         UNIQUEIDENTIFIER NOT NULL,
    [VPCode]           VARCHAR (10)     NOT NULL,
    [ETCDirectionCode] VARCHAR (10)     NOT NULL,
    CONSTRAINT [PK_LaneMapping] PRIMARY KEY CLUSTERED ([LaneCode] ASC)
);





