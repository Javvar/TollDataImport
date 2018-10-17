using IBM.Data.Informix;
using Intertoll.DataImport.Database.Sync.Data;
using Intertoll.DataImport.Database.Sync.Data.DataContext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
                    try
                    {
                        using (IfxConnection connection = EstablishConnection())
                        {
                            using (var dataContext = new DatabaseSyncDataContext())
                            {
                                IfxCommand command = connection.CreateCommand();
                                command.CommandText = "SELECT FIRST 100 * FROM p_trans ";

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
                                            if (/*todo: setting to toggle*/ !dataContext.ImportedTransactions.Any(x => x.ln_id == trans.ln_id && x.tx_seq_nr == trans.tx_seq_nr))
                                                dataContext.ImportedTransactions.Insert(trans);
                                        }

                                        processedTrans.Add(trans.ln_id + trans.tx_seq_nr);
                                    }
                                    catch (Exception ex)
                                    {
                                        //todo: log
                                    }
                                }

                                dataContext.Save();
                                dataReader.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //todo: log
                    }

                    Thread.Sleep(3000);
                }
            });
        }

        private static void StartIncidentSyncProcess()
        {
            Task.Factory.StartNew(() =>
            {
                while (run)
                {
                    try
                    {
                        using (IfxConnection connection = EstablishConnection())
                        {
                            using (var dataContext = new DatabaseSyncDataContext())
                            {
                                IfxCommand command = connection.CreateCommand();
                                command.CommandText = "SELECT FIRST 100 * FROM p_inc ";

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
                                            if (/*todo: setting to toggle*/ !dataContext.ImportedIncidents.Any(x => x.ln_id == inc.ln_id && x.tx_seq_nr == inc.in_seq_nr))
                                                dataContext.ImportedIncidents.Insert(inc);
                                        }

                                        processedIncs.Add(inc.ln_id + inc.in_seq_nr);

                                    }
                                    catch (Exception ex)
                                    {
                                        //todo: log
                                    }
                                }

                                dataContext.Save();
                                dataReader.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //todo: log
                    }

                    Thread.Sleep(3000);
                }
            });
        }

        private static void StartETCTransactionSyncProcess()
        {
            Task.Factory.StartNew(() =>
            {
                while (run)
                {

                    try
                    {
                        using (IfxConnection connection = EstablishConnection())
                        {
                            using (var dataContext = new DatabaseSyncDataContext())
                            {
                                IfxCommand command = connection.CreateCommand();
                                command.CommandText = "SELECT FIRST 100 * FROM c_tch ";

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
                                            if (!dataContext.ImportedETCTransactions.Any(x => x.ln_id == trans.ln_id && x.tx_seq_nr == trans.tx_seq_nr))
                                                dataContext.ImportedETCTransactions.Insert(trans);
                                        }

                                        processedTrans.Add(trans.ln_id + trans.tx_seq_nr); 
                                    }
                                    catch (Exception ex)
                                    {
                                        //todo: log
                                    }
                                }

                                dataContext.Save();
                                dataReader.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //todo: log
                    }

                    Thread.Sleep(3000);
                }
            });
        }

        private static void StartTimesliceSyncProcess()
        {

        }

        #endregion

        private static DateTime? ExtractDatetimeValue(object value)
        {
            if (value is DateTime)
                return (DateTime?)value;

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
