CREATE TABLE [dbo].[StagingAccounts] (
    [ac_nr]        VARCHAR (31) NOT NULL,
    [ag_id]        INT          NULL,
    [rs_id]        VARCHAR (2)  NULL,
    [ac_holder]    VARCHAR (51) NULL,
    [ac_reg_dt]    DATETIME     NULL,
    [ac_min_bal]   VARCHAR (20) NULL,
    [ac_term_dt]   DATETIME     NULL,
    [ac_term_rule] VARCHAR (2)  NULL,
    [ac_link_rule] VARCHAR (2)  NULL,
    [la_id]        VARCHAR (5)  NULL,
    CONSTRAINT [PK_StagingAccounts] PRIMARY KEY CLUSTERED ([ac_nr] ASC)
);

