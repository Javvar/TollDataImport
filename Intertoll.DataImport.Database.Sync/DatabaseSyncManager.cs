using IBM.Data.Informix;
using Intertoll.DataImport.Database.Sync.Data;
using Intertoll.DataImport.Database.Sync.Data.DataContext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Intertoll.NLogger;

namespace Intertoll.DataImport.Database.Sync
{
    public static class DatabaseSyncManager
    {
        static bool run = true;

        public static void StartSynchingProcess()
        {
            run = true;

            StartTransactionSyncProcess();
            StartIncidentSyncProcess();
            StartETCTransactionSyncProcess();
            StartTimesliceSyncProcess();
            StartRegisteredAccountsProcess();
        }        

        public static void EndSynchingProcess()
        {
            run = false;
        }

        #region Sync

        private static void StartTransactionSyncProcess()
        {          
            Task.Factory.StartNew(() =>
            {
                while (run)
                {
                    TransactionSyncProcess();
                    Thread.Sleep(1000 * AppSettings.TransactionsIntervalInSeconds);
                }
            });            
        }

        private static void StartETCTransactionSyncProcess()
        {
            Task.Factory.StartNew(() =>
            {
                while (run)
                {
                    ETCTransactionSyncProcess();
                    Thread.Sleep(1000 * AppSettings.ETCTransactionsIntervalInSeconds);
                }
            });
        }

        private static void StartIncidentSyncProcess()
        {
            Task.Factory.StartNew(() =>
            {
                while (run)
                {
                    IncidentSyncProcess();
                    Thread.Sleep(1000 * AppSettings.IncidentsIntervalInSeconds);
                }
            });
        }

        private static void StartTimesliceSyncProcess()
        {
            Task.Factory.StartNew(() =>
            {
                while (run)
                {
                    TimesliceSyncProcess();
                    Thread.Sleep(1000 * AppSettings.TimeslicesIntervalInSeconds);
                }
            });
        }

        private static void StartRegisteredAccountsProcess()
        {
            Task.Factory.StartNew(() =>
            {
                while (run)
                {
                    //RegisteredAccountsProcess();
                    //RegisteredAccountDetailsProcess();
                    RegisteredAccountsUsers();
                    Thread.Sleep(1000 * AppSettings.RegisteredAccountsIntervalInSeconds);
                }
            });
        }

        private static void TransactionSyncProcess()
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                using (IfxConnection connection = EstablishConnection())
                {
                    {
                        using (var dataContext = new DatabaseSyncDataContext())
                        {
                            IfxCommand command = connection.CreateCommand();
                            command.CommandText = $"SELECT FIRST {AppSettings.TransactionSelectBatchSize} * FROM p_trans ";

                            var lastTransaction = dataContext.ImportedTransactions.FirstOrDefault(orderBy: x => x.OrderByDescending(y => y.dt_concluded));

                            if (lastTransaction != null)
                            {
                                command.CommandText += string.Format("WHERE dt_concluded > TO_DATE('{0}','%Y-%m-%d %H:%M:%S')",
                                                                       lastTransaction.dt_concluded?.ToString("yyyy-MM-dd HH:mm:ss"));
                            }

                            command.CommandText = command.CommandText + " ORDER BY dt_concluded";

                            IfxDataReader dataReader = command.ExecuteReader();

                            var processedTrans = new List<string>();

                            while (dataReader.Read())
                            {
                                var trans = new StagingTransaction();

                                try
                                {
                                    int i = 0;

                                    trans.pl_id = dataReader[i].ToString().Trim();
                                    trans.ln_id = dataReader[++i].ToString().Trim();
                                    trans.dt_concluded = ExtractDatetimeValue(dataReader[++i]);
                                    trans.tx_seq_nr = int.TryParse(dataReader[++i].ToString(), out var outInt) ? outInt :
                                                                    throw new InvalidDataException("Invalid transaction sequence number.");

                                    trans.ts_seq_nr = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.us_id = dataReader[++i].ToString().Trim();
                                    trans.ent_plz_id = dataReader[++i].ToString().Trim();
                                    trans.ent_lane_id = dataReader[++i].ToString().Trim();
                                    trans.dt_started = ExtractDatetimeValue(dataReader[++i]);
                                    trans.next_inc = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.prev_inc = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.ft_id = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.pg_group = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.cg_group = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.vg_group = dataReader[++i].ToString().Trim();
                                    trans.mvc = dataReader[++i].ToString().Trim();
                                    trans.avc = dataReader[++i].ToString().Trim();
                                    trans.svc = dataReader[++i].ToString().Trim();
                                    trans.loc_curr = dataReader[++i].ToString().Trim();
                                    trans.loc_value = ExtractBCDValue(dataReader, ++i);
                                    trans.ten_curr = dataReader[++i].ToString().Trim();
                                    trans.ten_value = ExtractBCDValue(dataReader, ++i);
                                    trans.loc_change = ExtractBCDValue(dataReader, ++i);
                                    trans.variance = ExtractBCDValue(dataReader, ++i);
                                    trans.er_id = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.pm_id = dataReader[++i].ToString().Trim();
                                    trans.card_nr = dataReader[++i].ToString().Trim();
                                    trans.mask_nr = dataReader[++i].ToString().Trim();
                                    trans.bin_nr = dataReader[++i].ToString().Trim();
                                    trans.serv_code = dataReader[++i].ToString().Trim();
                                    trans.ca_id = dataReader[++i].ToString().Trim();
                                    trans.ct_id = dataReader[++i].ToString().Trim();
                                    trans.it_id = dataReader[++i].ToString().Trim();
                                    trans.sec_card_nr = dataReader[++i].ToString().Trim();
                                    trans.lm_id = dataReader[++i].ToString().Trim();
                                    trans.as_id = dataReader[++i].ToString().Trim();
                                    trans.reg_nr = dataReader[++i].ToString().Trim();
                                    trans.vouch_nr = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.ac_nr = dataReader[++i].ToString().Trim();
                                    trans.rec_nr = dataReader[++i].ToString().Trim();
                                    trans.tick_nr = dataReader[++i].ToString().Trim();
                                    trans.bp_id = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.fg_id = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.dg_id = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.rd_id = dataReader[++i].ToString().Trim();
                                    trans.rep_indic = dataReader[++i].ToString().Trim();
                                    trans.maint_indic = dataReader[++i].ToString().Trim();
                                    trans.req_indic = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.iv_prt_indic = dataReader[++i].ToString().Trim();
                                    trans.ts_dt_started = ExtractDatetimeValue(dataReader[++i]);
                                    trans.iv_nr = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.td_id = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.avc_seq_nr = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.update_us_id = dataReader[++i].ToString().Trim();
                                    trans.card_bank = dataReader[++i].ToString().Trim();
                                    trans.card_ac_nr = dataReader[++i].ToString().Trim();
                                    trans.tg_mfg_id = dataReader[++i].ToString().Trim();
                                    trans.tg_post_bal = ExtractBCDValue(dataReader, ++i);
                                    trans.tg_reader = dataReader[++i].ToString().Trim();
                                    trans.tg_us_cat = dataReader[++i].ToString().Trim();
                                    trans.tg_card_type = dataReader[++i].ToString().Trim();
                                    trans.tg_serv_prov_id = dataReader[++i].ToString().Trim();
                                    trans.tg_issuer = dataReader[++i].ToString().Trim();
                                    trans.tg_tx_seq_nr = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.etc_context_mrk = dataReader[++i].ToString().Trim();
                                    trans.etc_manufac_id = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.etc_beacon_id = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.etc_contract_pv = dataReader[++i].ToString().Trim();
                                    trans.avc_dt_concluded = ExtractDatetimeValue(dataReader[++i]);
                                    trans.avc_status = dataReader[++i].ToString().Trim();
                                    trans.anpr_vln = dataReader[++i].ToString().Trim();
                                    trans.anpr_conf = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.lvc = dataReader[++i].ToString().Trim();
                                    trans.inc_ind = ExtractIntegerValue(dataReader[++i].ToString());
                                    trans.id_vl = dataReader[++i].ToString().Trim();
                                    trans.vl_vln = dataReader[++i].ToString().Trim();
                                    trans.anpr_seq_nr = ExtractIntegerValue(dataReader[++i].ToString());

                                    if (!processedTrans.Any(x => x == trans.ln_id + trans.tx_seq_nr))
                                    {
                                        if ((AppSettings.CheckDuplicatesOnExistingData &&
                                             !dataContext.ImportedTransactions.Any(x => x.ln_id == trans.ln_id && x.tx_seq_nr == trans.tx_seq_nr)) ||
                                             !AppSettings.CheckDuplicatesOnExistingData)
                                        {
                                            dataContext.ImportedTransactions.Insert(trans);
                                        }
                                    }

                                    processedTrans.Add(trans.ln_id + trans.tx_seq_nr);
                                }
                                catch (Exception ex)
                                {
                                    Log.LogException(ex);
                                }
                            }

                            dataContext.Save();
                            dataReader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }

            Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        private static void ETCTransactionSyncProcess()
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                using (IfxConnection connection = EstablishConnection())
                {
                    using (var dataContext = new DatabaseSyncDataContext())
                    {
                        IfxCommand command = connection.CreateCommand();
                        command.CommandText = $"SELECT FIRST {AppSettings.ETCTransactionSelectBatchSize} * FROM c_tch ";

                        var lastTransaction = dataContext.ImportedETCTransactions.FirstOrDefault(orderBy: x => x.OrderByDescending(y => y.dt_concluded));

                        if (lastTransaction != null)
                        {
                            command.CommandText += string.Format("WHERE dt_concluded > TO_DATE('{0}','%Y-%m-%d %H:%M:%S')",
                                                                   lastTransaction.dt_concluded?.ToString("yyyy-MM-dd HH:mm:ss"));
                        }

                        command.CommandText = command.CommandText + " ORDER BY dt_concluded";

                        IfxDataReader dataReader = command.ExecuteReader();

                        var processedTrans = new List<string>();

                        while (dataReader.Read())
                        {
                            var trans = new StagingETCTransaction();

                            try
                            {
                                int i = 0;
                                trans.pl_id = dataReader[i].ToString().Trim();
                                trans.ln_id = dataReader[++i].ToString().Trim();
                                trans.dt_concluded = ExtractDatetimeValue(dataReader[++i]);
                                trans.tx_seq_nr = int.TryParse(dataReader[++i].ToString().Trim(), out var outInt) ? outInt :
                                                                throw new InvalidDataException("Invalid transaction sequence number.");
                                trans.ops_dt = ExtractDatetimeValue(dataReader[++i]);
                                trans.ops_sh = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.ts_seq_nr = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.ent_plz_id = dataReader[++i].ToString().Trim();
                                trans.ent_lane_id = dataReader[++i].ToString().Trim();
                                trans.dt_started = ExtractDatetimeValue(dataReader[++i]);
                                trans.cg_group = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.vg_group = dataReader[++i].ToString().Trim();
                                trans.mvc = dataReader[++i].ToString().Trim();
                                trans.avc = dataReader[++i].ToString().Trim();
                                trans.lvc = dataReader[++i].ToString().Trim();
                                trans.svc = dataReader[++i].ToString().Trim();
                                trans.loc_curr = dataReader[++i].ToString().Trim();
                                trans.loc_value = ExtractBCDValue(dataReader, ++i);
                                trans.variance = ExtractBCDValue(dataReader, ++i);
                                trans.er_id = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.it_id = dataReader[++i].ToString().Trim();
                                trans.sec_card_nr = dataReader[++i].ToString().Trim();
                                trans.reg_nr = dataReader[++i].ToString().Trim();
                                trans.vl_vln = dataReader[++i].ToString().Trim();
                                trans.vouch_nr = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.rec_nr = dataReader[++i].ToString().Trim();
                                trans.tick_nr = dataReader[++i].ToString().Trim();
                                trans.fg_id = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.bp_id = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.dg_id = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.disc_value = ExtractBCDValue(dataReader, ++i);
                                trans.ft_id = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.pg_group = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.ts_dt_started = ExtractDatetimeValue(dataReader[++i]);
                                trans.us_id = dataReader[++i].ToString().Trim();
                                trans.count_for_disc = dataReader[++i].ToString().Trim();
                                trans.iv_prt_indic = dataReader[++i].ToString().Trim();
                                trans.iv_nr = ExtractIntegerValue(dataReader[++i].ToString()); ;
                                trans.nom_value = ExtractBCDValue(dataReader, ++i);
                                trans.update_us_id = dataReader[++i].ToString().Trim();
                                trans.etc_contect_mrk = dataReader[++i].ToString().Trim();
                                trans.etc_manufac_id = ExtractIntegerValue(dataReader[++i].ToString()); ;
                                trans.etc_contract_pv = dataReader[++i].ToString().Trim();
                                trans.etc_beacon_id = ExtractIntegerValue(dataReader[++i].ToString()); ;
                                trans.avc_dt_concluded = ExtractDatetimeValue(dataReader[++i]); ;
                                trans.ca_id = dataReader[++i].ToString().Trim();
                                trans.ct_id = dataReader[++i].ToString().Trim();
                                trans.td_id = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.ac_nr = dataReader[++i].ToString().Trim();
                                trans.card_nr = dataReader[++i].ToString().Trim();
                                trans.inc_ind = ExtractIntegerValue(dataReader[++i].ToString());
                                trans.id_vl = dataReader[++i].ToString().Trim();
                                trans.loc_tax = ExtractBCDValue(dataReader, ++i);
                                trans.act_disc = dataReader[++i].ToString().Trim();

                                if (!processedTrans.Any(x => x == trans.ln_id + trans.tx_seq_nr))
                                {
                                    if ((AppSettings.CheckDuplicatesOnExistingData &&
                                         !dataContext.ImportedETCTransactions.Any(x => x.ln_id == trans.ln_id && x.tx_seq_nr == trans.tx_seq_nr)) ||
                                         !AppSettings.CheckDuplicatesOnExistingData)
                                    {
                                        dataContext.ImportedETCTransactions.Insert(trans);
                                    }
                                }

                                processedTrans.Add(trans.ln_id + trans.tx_seq_nr);
                            }
                            catch (Exception ex)
                            {
                                Log.LogException(ex);
                            }
                        }

                        dataContext.Save();
                        dataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }

            Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        private static void IncidentSyncProcess()
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                using (IfxConnection connection = EstablishConnection())
                {
                    using (var dataContext = new DatabaseSyncDataContext())
                    {
                        IfxCommand command = connection.CreateCommand();
                        command.CommandText = $"SELECT FIRST {AppSettings.IncidentSelectBatchSize} * FROM p_inc ";

                        var lastIncident = dataContext.ImportedIncidents.FirstOrDefault(orderBy: x => x.OrderByDescending(y => y.dt_generated));

                        if (lastIncident != null)
                        {
                            command.CommandText += string.Format("WHERE dt_generated > TO_DATE('{0}','%Y-%m-%d %H:%M:%S')",
                                                                   lastIncident.dt_generated?.ToString("yyyy-MM-dd HH:mm:ss"));
                        }

                        command.CommandText = command.CommandText + "ORDER BY dt_generated";

                        IfxDataReader dataReader = command.ExecuteReader();

                        var processedIncs = new List<string>();

                        while (dataReader.Read())
                        {
                            var inc = new StagingIncident();

                            try
                            {
                                int i = 0;
                                inc.pl_id = dataReader[i].ToString().Trim();
                                inc.ln_id = dataReader[++i].ToString().Trim();
                                inc.dt_generated = ExtractDatetimeValue(dataReader[++i]);

                                inc.in_seq_nr = int.TryParse(dataReader[++i].ToString(), out var outInt) ? outInt :
                                                                throw new InvalidDataException("Invalid incident sequence number.");

                                inc.ir_type = dataReader[++i].ToString().Trim();
                                inc.ir_subtype = dataReader[++i].ToString().Trim();
                                inc.tx_seq_nr = ExtractIntegerValue(dataReader[++i].ToString());
                                inc.ts_seq_nr = ExtractIntegerValue(dataReader[++i].ToString());
                                inc.us_id = dataReader[++i].ToString().Trim();
                                inc.ft_id = ExtractIntegerValue(dataReader[++i].ToString());
                                inc.pg_group = ExtractIntegerValue(dataReader[++i].ToString());
                                inc.cg_group = ExtractIntegerValue(dataReader[++i].ToString());
                                inc.vg_group = dataReader[++i].ToString().Trim();
                                inc.mvc = dataReader[++i].ToString().Trim();
                                inc.avc = dataReader[++i].ToString().Trim();
                                inc.svc = dataReader[++i].ToString().Trim();
                                inc.er_id = ExtractIntegerValue(dataReader[++i].ToString());
                                inc.pm_id = dataReader[++i].ToString().Trim();
                                inc.card_nr = dataReader[++i].ToString().Trim();
                                inc.mask_nr = dataReader[++i].ToString().Trim();
                                inc.ca_id = dataReader[++i].ToString().Trim();
                                inc.ct_id = dataReader[++i].ToString().Trim();
                                inc.tx_indic = dataReader[++i].ToString().Trim();
                                inc.lm_id = dataReader[++i].ToString().Trim();
                                inc.as_id = dataReader[++i].ToString().Trim();
                                inc.rep_indic = dataReader[++i].ToString().Trim();
                                inc.rd_id = dataReader[++i].ToString().Trim();
                                inc.maint_indic = dataReader[++i].ToString().Trim();
                                inc.req_indic = ExtractIntegerValue(dataReader[++i].ToString());
                                inc.ts_dt_started = ExtractDatetimeValue(dataReader[++i]);
                                inc.in_amt = ExtractBCDValue(dataReader, ++i);
                                inc.in_data = dataReader[++i].ToString().Trim();
                                inc.tg_bl_id = dataReader[++i].ToString().Trim();
                                inc.tg_mfg_id = dataReader[++i].ToString().Trim();
                                inc.tg_card_type = dataReader[++i].ToString().Trim();
                                inc.tg_reader = dataReader[++i].ToString().Trim();
                                inc.tg_tx_seq_nr = ExtractIntegerValue(dataReader[++i].ToString());
                                inc.avc_in_seq_nr = ExtractIntegerValue(dataReader[++i].ToString());
                                inc.avc_in_type_id = ExtractIntegerValue(dataReader[++i].ToString());
                                inc.avc_dt_generated = ExtractDatetimeValue(dataReader[++i]);

                                if (!processedIncs.Any(x => x == inc.ln_id + inc.in_seq_nr))
                                {

                                    if ((AppSettings.CheckDuplicatesOnExistingData &&
                                         !dataContext.ImportedIncidents.Any(x => x.ln_id == inc.ln_id && x.tx_seq_nr == inc.in_seq_nr)) ||
                                         !AppSettings.CheckDuplicatesOnExistingData)
                                    {
                                        dataContext.ImportedIncidents.Insert(inc);
                                    }
                                }

                                processedIncs.Add(inc.ln_id + inc.in_seq_nr);

                            }
                            catch (Exception ex)
                            {
                                Log.LogException(ex);
                            }
                        }

                        dataContext.Save();
                        dataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }

            Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        private static void TimesliceSyncProcess()
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                using (IfxConnection connection = EstablishConnection())
                {
                    using (var dataContext = new DatabaseSyncDataContext())
                    {
                        IfxCommand command = connection.CreateCommand();
                        command.CommandText = $"SELECT FIRST {AppSettings.TimesliceSelectBatchSize} * FROM p_ts ";

                        var lastTS = dataContext.ImportedTimeslices.FirstOrDefault(orderBy: x => x.OrderByDescending(y => y.dt_started));

                        if (lastTS != null)
                        {
                            command.CommandText += string.Format("WHERE dt_started > TO_DATE('{0}','%Y-%m-%d %H:%M:%S')",
                                                                   lastTS.dt_started?.ToString("yyyy-MM-dd HH:mm:ss"));
                        }

                        command.CommandText = command.CommandText + " ORDER BY dt_started";

                        IfxDataReader dataReader = command.ExecuteReader();

                        var processedTimeslices = new List<string>();

                        while (dataReader.Read())
                        {
                            var ts = new StagingTimeSlice();

                            try
                            {
                                int i = 0;

                                ts.pl_id = dataReader[i].ToString().Trim();
                                ts.ln_id = dataReader[++i].ToString().Trim();

                                ts.ts_seq_nr = int.TryParse(dataReader[++i].ToString().Trim(), out var outInt) ? outInt :
                                                                throw new InvalidDataException("Invalid timeslice sequence number.");

                                ts.dt_started = ExtractDatetimeValue(dataReader[++i]);
                                ts.dt_ended = ExtractDatetimeValue(dataReader[++i]);
                                ts.us_id = dataReader[++i].ToString().Trim();
                                ts.lm_id = dataReader[++i].ToString().Trim();
                                ts.next_tx = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.prev_tx = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.tx_count = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.next_inc = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.prev_inc = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.inc_count = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.next_bo = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.prev_bo = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.bo_count = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.next_sp = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.prev_sp = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.sp_count = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.next_vat = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.prev_vat = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.vat_count = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.next_avc = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.prev_avc = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.avc_count = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.vgs_count = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.ft_id = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.pg_group = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.loc_value = ExtractBCDValue(dataReader, ++i);
                                ts.cash_value = ExtractBCDValue(dataReader, ++i);
                                ts.sun_value = ExtractBCDValue(dataReader, ++i);
                                ts.stats_msg = ExtractBCDValue(dataReader, ++i);
                                ts.stats_count = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.ops_dt = ExtractDatetimeValue(dataReader[++i]);
                                ts.ops_sh = ExtractIntegerValue(dataReader[++i].ToString());
                                ts.vs_id = dataReader[++i].ToString().Trim();
                                ts.rep_indic = dataReader[++i].ToString().Trim();
                                ts.rd_id = dataReader[++i].ToString().Trim();
                                ts.maint_indic = dataReader[++i].ToString().Trim();
                                ts.req_indic = ExtractIntegerValue(dataReader[++i].ToString());

                                if (!processedTimeslices.Any(x => x == ts.ln_id + ts.ts_seq_nr))
                                {
                                    if ((AppSettings.CheckDuplicatesOnExistingData &&
                                         !dataContext.ImportedTimeslices.Any(x => x.ln_id == ts.ln_id && x.ts_seq_nr == ts.ts_seq_nr)) ||
                                         !AppSettings.CheckDuplicatesOnExistingData)
                                    {
                                        dataContext.ImportedTimeslices.Insert(ts);
                                    }
                                }

                                processedTimeslices.Add(ts.ln_id + ts.ts_seq_nr);
                            }
                            catch (Exception ex)
                            {
                                Log.LogException(ex);
                            }
                        }

                        dataContext.Save();
                        dataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }

            Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        private static void RegisteredAccountsProcess()
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                using (IfxConnection connection = EstablishConnection())
                {
                    using (var dataContext = new DatabaseSyncDataContext())
                    {
                        IfxCommand command = connection.CreateCommand();
                        command.CommandText = $"SELECT FIRST 1000 * FROM c_account ";

                        var lastAcc = dataContext.ImportedAccounts.FirstOrDefault(orderBy: x => x.OrderByDescending(y => y.ac_reg_dt)
                                                                                                 .ThenByDescending(y => y.ac_nr));

                        if (lastAcc != null)
                        {
                            command.CommandText += string.Format("WHERE ac_nr > '{0}'", lastAcc.ac_nr);
                        }

                        command.CommandText = command.CommandText + " ORDER BY ac_nr";

                        IfxDataReader dataReader = command.ExecuteReader();

                        var processedAccounts = new List<string>();

                        while (dataReader.Read())
                        {
                            var acc = new StagingAccount();

                            try
                            {
                                int i = 0;

                                acc.ac_nr = dataReader[i].ToString().Trim();
                                acc.ag_id = ExtractIntegerValue(dataReader[++i].ToString().Trim());
                                acc.rs_id = dataReader[++i].ToString().Trim();
                                acc.ac_holder = dataReader[++i].ToString().Trim();
                                acc.ac_reg_dt = ExtractDatetimeValue(dataReader[++i].ToString().Trim());
                                acc.ac_min_bal = ExtractBCDValue(dataReader, ++i);
                                acc.ac_term_dt = ExtractDatetimeValue(dataReader[++i].ToString().Trim());
                                acc.ac_term_rule = dataReader[++i].ToString().Trim();
                                acc.ac_link_rule = dataReader[++i].ToString().Trim();
                                acc.la_id = dataReader[++i].ToString().Trim();

                                if (!processedAccounts.Any(x => x == acc.ac_nr))
                                    if (!dataContext.ImportedAccounts.Any(x => x.ac_nr == acc.ac_nr))
                                        dataContext.ImportedAccounts.Insert(acc);

                                processedAccounts.Add(acc.ac_nr);
                            }
                            catch (Exception ex)
                            {
                                Log.LogException(ex);
                            }
                        }

                        dataContext.Save();
                        dataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }

            Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        private static void RegisteredAccountDetailsProcess()
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                using (IfxConnection connection = EstablishConnection())
                {
                    using (var dataContext = new DatabaseSyncDataContext())
                    {
                        IfxCommand command = connection.CreateCommand();
                        command.CommandText = $"SELECT FIRST 1000 * FROM c_acc_det ";

                        var lastAcc = dataContext.ImportedAccountDetails.FirstOrDefault(orderBy: x => x.OrderByDescending((y => y.ac_nr)));

                        if (lastAcc != null)
                        {
                            command.CommandText += string.Format("WHERE ac_nr > '{0}'", lastAcc.ac_nr);
                        }

                        command.CommandText = command.CommandText + " ORDER BY ac_nr";

                        IfxDataReader dataReader = command.ExecuteReader();

                        var processedAccountDetailss = new List<string>();

                        while (dataReader.Read())
                        {
                            var acc = new StagingAccountDetail();

                            try
                            {
                                int i = 0;

                                acc.ac_nr = dataReader[i].ToString().Trim();
                                acc.ac_title = dataReader[++i].ToString().Trim();
                                acc.ac_initial = dataReader[++i].ToString().Trim();
                                acc.ac_surname = dataReader[++i].ToString().Trim();
                                acc.ac_home_ph = dataReader[++i].ToString().Trim();
                                acc.ac_work_ph = dataReader[++i].ToString().Trim();
                                acc.ac_cell_ph = dataReader[++i].ToString().Trim();
                                acc.ac_fax_ph = dataReader[++i].ToString().Trim();
                                acc.ac_email = dataReader[++i].ToString().Trim();
                                acc.at_id = dataReader[++i].ToString().Trim();
                                acc.ac_password = dataReader[++i].ToString().Trim();

                                if (!processedAccountDetailss.Any(x => x == acc.ac_nr))
                                    if (!dataContext.ImportedAccountDetails.Any(x => x.ac_nr == acc.ac_nr))
                                        dataContext.ImportedAccountDetails.Insert(acc);

                                processedAccountDetailss.Add(acc.ac_nr);
                            }
                            catch (Exception ex)
                            {
                                Log.LogException(ex);
                            }
                        }

                        dataContext.Save();
                        dataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }

            Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        private static void RegisteredAccountsUsers()
        {
            Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            try
            {
                using (IfxConnection connection = EstablishConnection())
                {
                    using (var dataContext = new DatabaseSyncDataContext())
                    {
                        IfxCommand command = connection.CreateCommand();
                        command.CommandText = $"SELECT FIRST 1000 * FROM h_reg_id ";

                        var lastAccId = dataContext.ImportedAccountIdentifiers.FirstOrDefault(orderBy: x => x.OrderByDescending((y => y.his_dt)));

                        if (lastAccId != null)
                        {
                            command.CommandText += string.Format("WHERE his_dt >= TO_DATE('{0}','%Y-%m-%d %H:%M:%S')",
                                                                   lastAccId.his_dt?.ToString("yyyy-MM-dd HH:mm:ss"));
                        }

                        command.CommandText = command.CommandText + " ORDER BY his_dt";

                        IfxDataReader dataReader = command.ExecuteReader();

                        var processedAccountDetailss = new List<string>();

                        while (dataReader.Read())
                        {
                            var acc = new StagingAccountIdentifier(); 

                            try
                            {
                                int i = 0;

                                acc.ac_nr = dataReader[i].ToString().Trim();
                                acc.ri_id = dataReader[++i].ToString().Trim();
                                acc.mask_nr = dataReader[++i].ToString().Trim();
                                acc.it_id = dataReader[++i].ToString().Trim();
                                acc.his_action = dataReader[++i].ToString().Trim();
                                acc.his_dt = ExtractDatetimeValue(dataReader[++i].ToString().Trim());
                                acc.his_us_id = dataReader[++i].ToString().Trim();
                                acc.his_change1 = dataReader[++i].ToString().Trim();

                                if (!processedAccountDetailss.Any(x => x == acc.ri_id))
                                    if (!dataContext.ImportedAccountIdentifiers.Any(x => x.ri_id == acc.ri_id))
                                        dataContext.ImportedAccountIdentifiers.Insert(acc);

                                processedAccountDetailss.Add(acc.ri_id);
                            }
                            catch (Exception ex)
                            {
                                Log.LogException(ex);
                            }
                        }

                        dataContext.Save();
                        dataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }

            Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        #endregion

        private static DateTime? ExtractDatetimeValue(object value)
        {
            if (value is DateTime)
                return (DateTime?)value;

            if (value is string && DateTime.TryParse(value.ToString(),out var outdt))
                return outdt;

            return null;
        }

        private static int? ExtractIntegerValue(string value)
        {
            return int.TryParse(value, out var outInt) ? (int?)outInt : null;
        }

        private static string ExtractBCDValue(IfxDataReader reader,int index)
        {
            try
            {
                return reader[index].ToString().Trim();
            }
            catch (Exception)
            {
                return "-";
            }
        }        

        private static IfxConnection EstablishConnection()
        {
            try
            {
                var connection = new IfxConnection("Host=tongaat;Server=tongaat_tcp;Service=2000;Protocol=onsoctcp;UID=informix;Password=informix123;Database=tongaat;");
                connection.DatabaseLocale = "en_US.CP1252";
                connection.ClientLocale = "en_US.CP1252";
                connection.Open();

                return connection;
            }
            catch (Exception ex)
            {
                //todo:log
                return null;
            }
        }
    }
}
