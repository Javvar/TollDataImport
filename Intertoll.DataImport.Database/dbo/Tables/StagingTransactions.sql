﻿CREATE TABLE [dbo].[StagingTransactions] (
    [pl_id]            VARCHAR (9)   NULL,
    [ln_id]            VARCHAR (9)   NULL,
    [dt_concluded]     DATETIME      NULL,
    [tx_seq_nr]        INT           NULL,
    [ts_seq_nr]        INT           NULL,
    [us_id]            VARCHAR (31)  NULL,
    [ent_plz_id]       VARCHAR (9)   NULL,
    [ent_lane_id]      VARCHAR (9)   NULL,
    [dt_started]       DATETIME      NULL,
    [next_inc]         INT           NULL,
    [prev_inc]         INT           NULL,
    [ft_id]            INT           NULL,
    [pg_group]         INT           NULL,
    [cg_group]         INT           NULL,
    [vg_group]         VARCHAR (9)   NULL,
    [mvc]              VARCHAR (9)   NULL,
    [avc]              VARCHAR (9)   NULL,
    [svc]              VARCHAR (9)   NULL,
    [loc_curr]         VARCHAR (5)   NULL,
    [loc_value]        VARCHAR (50)  NULL,
    [ten_curr]         VARCHAR (5)   NULL,
    [ten_value]        VARCHAR (50)  NULL,
    [loc_change]       VARCHAR (50)  NULL,
    [variance]         VARCHAR (50)  NULL,
    [er_id]            INT           NULL,
    [pm_id]            VARCHAR (2)   NULL,
    [card_nr]          VARCHAR (MAX) NULL,
    [mask_nr]          VARCHAR (21)  NULL,
    [bin_nr]           VARCHAR (11)  NULL,
    [serv_code]        VARCHAR (5)   NULL,
    [ca_id]            VARCHAR (2)   NULL,
    [ct_id]            VARCHAR (2)   NULL,
    [it_id]            VARCHAR (2)   NULL,
    [sec_card_nr]      VARCHAR (76)  NULL,
    [lm_id]            VARCHAR (2)   NULL,
    [as_id]            VARCHAR (2)   NULL,
    [reg_nr]           VARCHAR (21)  NULL,
    [vouch_nr]         INT           NULL,
    [ac_nr]            VARCHAR (31)  NULL,
    [rec_nr]           VARCHAR (11)  NULL,
    [tick_nr]          VARCHAR (21)  NULL,
    [bp_id]            INT           NULL,
    [fg_id]            INT           NULL,
    [dg_id]            INT           NULL,
    [rd_id]            VARCHAR (2)   NULL,
    [rep_indic]        VARCHAR (2)   NULL,
    [maint_indic]      VARCHAR (2)   NULL,
    [req_indic]        INT           NULL,
    [iv_prt_indic]     VARCHAR (2)   NULL,
    [ts_dt_started]    DATETIME      NULL,
    [iv_nr]            INT           NULL,
    [td_id]            INT           NULL,
    [avc_seq_nr]       INT           NULL,
    [update_us_id]     VARCHAR (31)  NULL,
    [card_bank]        VARCHAR (21)  NULL,
    [card_ac_nr]       VARCHAR (31)  NULL,
    [tg_mfg_id]        VARCHAR (11)  NULL,
    [tg_post_bal]      VARCHAR (50)  NULL,
    [tg_reader]        VARCHAR (9)   NULL,
    [tg_us_cat]        VARCHAR (2)   NULL,
    [tg_card_type]     VARCHAR (2)   NULL,
    [tg_serv_prov_id]  VARCHAR (3)   NULL,
    [tg_issuer]        VARCHAR (3)   NULL,
    [tg_tx_seq_nr]     INT           NULL,
    [etc_context_mrk]  VARCHAR (13)  NULL,
    [etc_manufac_id]   INT           NULL,
    [etc_beacon_id]    INT           NULL,
    [etc_contract_pv]  VARCHAR (7)   NULL,
    [avc_dt_concluded] DATETIME      NULL,
    [avc_status]       VARCHAR (51)  NULL,
    [anpr_vln]         VARCHAR (21)  NULL,
    [anpr_conf]        INT           NULL,
    [lvc]              VARCHAR (9)   NULL,
    [inc_ind]          INT           NULL,
    [id_vl]            VARCHAR (21)  NULL,
    [vl_vln]           VARCHAR (21)  NULL,
    [anpr_seq_nr]      INT           NULL
);






GO
CREATE CLUSTERED INDEX [DateConcludedIndex]
    ON [dbo].[StagingTransactions]([dt_concluded] ASC);

