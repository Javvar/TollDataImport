CREATE TABLE [dbo].[StagingAccountDetails] (
    [ac_nr]       VARCHAR (31) NOT NULL,
    [ac_title]    VARCHAR (20) NULL,
    [ac_initial]  VARCHAR (20) NULL,
    [ac_surname]  VARCHAR (31) NULL,
    [ac_home_ph]  VARCHAR (31) NULL,
    [ac_work_ph]  VARCHAR (31) NULL,
    [ac_cell_ph]  VARCHAR (31) NULL,
    [ac_fax_ph]   VARCHAR (31) NULL,
    [ac_email]    VARCHAR (51) NULL,
    [at_id]       VARCHAR (20) NULL,
    [ac_password] VARCHAR (20) NULL,
    CONSTRAINT [PK_StagingAccountDetails] PRIMARY KEY CLUSTERED ([ac_nr] ASC)
);

