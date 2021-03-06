﻿CREATE TABLE [dbo].[StagingIncidents] (
    [pl_id]            VARCHAR (9)   NULL,
    [ln_id]            VARCHAR (9)   NOT NULL,
    [dt_generated]     DATETIME      NULL,
    [in_seq_nr]        INT           NOT NULL,
    [ir_type]          VARCHAR (3)   NULL,
    [ir_subtype]       VARCHAR (3)   NULL,
    [tx_seq_nr]        INT           NULL,
    [ts_seq_nr]        INT           NULL,
    [us_id]            VARCHAR (31)  NULL,
    [ft_id]            INT           NULL,
    [pg_group]         INT           NULL,
    [cg_group]         INT           NULL,
    [vg_group]         VARCHAR (9)   NULL,
    [mvc]              VARCHAR (9)   NULL,
    [avc]              VARCHAR (9)   NULL,
    [svc]              VARCHAR (9)   NULL,
    [er_id]            INT           NULL,
    [pm_id]            VARCHAR (2)   NULL,
    [card_nr]          VARCHAR (176) NULL,
    [mask_nr]          VARCHAR (21)  NULL,
    [ca_id]            VARCHAR (2)   NULL,
    [ct_id]            VARCHAR (2)   NULL,
    [tx_indic]         VARCHAR (2)   NULL,
    [lm_id]            VARCHAR (2)   NULL,
    [as_id]            VARCHAR (2)   NULL,
    [rep_indic]        VARCHAR (2)   NULL,
    [rd_id]            VARCHAR (2)   NULL,
    [maint_indic]      VARCHAR (2)   NULL,
    [req_indic]        INT           NULL,
    [ts_dt_started]    DATETIME      NULL,
    [in_amt]           VARCHAR (50)  NULL,
    [in_data]          VARCHAR (256) NULL,
    [tg_bl_id]         VARCHAR (3)   NULL,
    [tg_mfg_id]        VARCHAR (11)  NULL,
    [tg_card_type]     VARCHAR (2)   NULL,
    [tg_reader]        VARCHAR (9)   NULL,
    [tg_tx_seq_nr]     INT           NULL,
    [avc_in_seq_nr]    INT           NULL,
    [avc_in_type_id]   INT           NULL,
    [avc_dt_generated] DATETIME      NULL,
    CONSTRAINT [PK_StagingIncidents] PRIMARY KEY CLUSTERED ([ln_id] ASC, [in_seq_nr] ASC)
);






GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20190107-221454]
    ON [dbo].[StagingIncidents]([dt_generated] ASC);

