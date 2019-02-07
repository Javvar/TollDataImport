﻿CREATE TABLE [dbo].[StagingETCTransactions] (
    [pl_id]            VARCHAR (9)   NULL,
    [ln_id]            VARCHAR (9)   NOT NULL,
    [dt_concluded]     DATETIME      NULL,
    [tx_seq_nr]        INT           NOT NULL,
    [ops_dt]           DATE          NULL,
    [ops_sh]           INT           NULL,
    [ts_seq_nr]        INT           NULL,
    [ent_plz_id]       VARCHAR (9)   NULL,
    [ent_lane_id]      VARCHAR (9)   NULL,
    [dt_started]       DATETIME      NULL,
    [cg_group]         INT           NULL,
    [vg_group]         VARCHAR (9)   NULL,
    [mvc]              VARCHAR (9)   NULL,
    [avc]              VARCHAR (9)   NULL,
    [lvc]              VARCHAR (9)   NULL,
    [svc]              VARCHAR (9)   NULL,
    [loc_curr]         VARCHAR (5)   NULL,
    [loc_value]        VARCHAR (100) NULL,
    [variance]         VARCHAR (100) NULL,
    [er_id]            INT           NULL,
    [it_id]            VARCHAR (2)   NULL,
    [sec_card_nr]      VARCHAR (21)  NULL,
    [reg_nr]           VARCHAR (21)  NULL,
    [vl_vln]           VARCHAR (21)  NULL,
    [vouch_nr]         INT           NULL,
    [rec_nr]           VARCHAR (11)  NULL,
    [tick_nr]          VARCHAR (11)  NULL,
    [fg_id]            INT           NULL,
    [bp_id]            INT           NULL,
    [dg_id]            INT           NULL,
    [disc_value]       VARCHAR (100) NULL,
    [ft_id]            INT           NULL,
    [pg_group]         INT           NULL,
    [ts_dt_started]    DATETIME      NULL,
    [us_id]            VARCHAR (31)  NULL,
    [count_for_disc]   VARCHAR (2)   NULL,
    [iv_prt_indic]     VARCHAR (2)   NULL,
    [iv_nr]            INT           NULL,
    [nom_value]        VARCHAR (100) NULL,
    [update_us_id]     VARCHAR (31)  NULL,
    [etc_contect_mrk]  VARCHAR (21)  NULL,
    [etc_manufac_id]   INT           NULL,
    [etc_contract_pv]  VARCHAR (7)   NULL,
    [etc_beacon_id]    INT           NULL,
    [avc_dt_concluded] DATETIME      NULL,
    [ca_id]            VARCHAR (2)   NULL,
    [ct_id]            VARCHAR (2)   NULL,
    [td_id]            INT           NULL,
    [ac_nr]            VARCHAR (31)  NULL,
    [card_nr]          VARCHAR (21)  NULL,
    [inc_ind]          INT           NULL,
    [id_vl]            VARCHAR (21)  NULL,
    [loc_tax]          VARCHAR (100) NULL,
    [act_disc]         VARCHAR (100) NULL,
    CONSTRAINT [PK_StagingETCTransactions] PRIMARY KEY NONCLUSTERED ([ln_id] ASC, [tx_seq_nr] ASC)
);






GO


