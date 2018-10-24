CREATE TABLE [dbo].[StagingAccountIdentifiers] (
    [ac_nr]          VARCHAR (31)  NULL,
    [ri_id]          VARCHAR (76)  NOT NULL,
    [mask_nr]        VARCHAR (21)  NULL,
    [it_id]          VARCHAR (2)   NULL,
    [his_action]     VARCHAR (2)   NULL,
    [his_dt]         DATETIME      NULL,
    [his_us_id]      VARCHAR (31)  NULL,
    [his_change1]    VARCHAR (256) NULL,
    [FullIdentifier] VARCHAR (76)  NULL,
    CONSTRAINT [PK_StagingAccountIdentifiers] PRIMARY KEY CLUSTERED ([ri_id] ASC)
);

