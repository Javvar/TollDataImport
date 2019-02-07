using IBM.Data.Informix;
using Intertoll.DataImport.Database.Sync.Data;
using Intertoll.DataImport.Database.Sync.Data.DataContext;
using Intertoll.NLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Intertoll.DataImport.Database.Sync
{
	public static class MissingDataSyncManager
	{
		static DateTime LastRunDuplicateCheck { get; set; }
	

		static DateTime CurrentMonth
		{
			get
			{
				return new DateTime(DateTime.Today.AddMonths(-AppSettings.MissingDatacheckMonthInThePast).Year, 
									DateTime.Today.AddMonths(-AppSettings.MissingDatacheckMonthInThePast).Month, 1);
			}
		}

		static DateTime FollowingMonth
		{
			get
			{
				return CurrentMonth.AddMonths(1);
			}
		}

		static bool run = true;

		public static void StartProcess()
		{
			run = true;

			StartCheckSequenceGapsProcess();
			StartCheckDuplicateSequenceGapsProcess();
		}

		public static void EndProcess()
		{
			run = false;
		}

		static void StartCheckSequenceGapsProcess()
		{
			Task.Factory.StartNew(() =>
			{
				while (run)
				{
					CheckSequenceGaps();

					Log.LogInfoMessage($"Sequence number gap check will run again at {DateTime.Now.AddSeconds(3600).ToShortTimeString()}");
					Thread.Sleep(1000 * 3600);
				}
			});
		}

		static void StartCheckDuplicateSequenceGapsProcess()
		{
			Task.Factory.StartNew(() =>
			{
				while (run)
				{
					if (LastRunDuplicateCheck < DateTime.Today)
					{
						CheckDuplicateSequenceGaps();
						LastRunDuplicateCheck = DateTime.Now;
					}
					
					Log.LogInfoMessage($"MIS duplicate check will run again at {DateTime.Now.AddSeconds(1800).ToShortTimeString()}");
					Thread.Sleep(1000 * 18000);
				}
			});
		}

		static void CheckSequenceGaps()
		{
			Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

			try
			{
				using (IfxConnection connection = EstablishConnection())
				using (var dataContext = new DatabaseSyncDataContext())
				{
					var missingTransactions = dataContext.GetTransactionSequenceNrGaps(CurrentMonth, FollowingMonth);

					foreach (var missingtrans in missingTransactions)
					{
						try
						{
							Log.LogTrace($"Requesting {missingtrans.Lane}:{missingtrans.Sequencenr}");

							IfxCommand command = connection.CreateCommand();
							command.CommandText = $"SELECT * FROM p_trans WHERE ln_id='{missingtrans.Lane}' AND tx_seq_nr='{missingtrans.Sequencenr}'";
							
							IfxDataReader dataReader = command.ExecuteReader();

							while (dataReader.Read())
							{
								Log.LogTrace($"Found {missingtrans.Lane}:{missingtrans.Sequencenr}");

								var trans = new StagingTransaction();

								try
								{
									int i = 0;

									trans.pl_id = dataReader[i].ToString().Trim();
									trans.ln_id = dataReader[++i].ToString().Trim();
									trans.dt_concluded = ExtractDatetimeValue(dataReader[++i]).Value;
									trans.tx_seq_nr = int.TryParse(dataReader[++i].ToString(), out var outInt)
										? outInt
										: throw new InvalidDataException("Invalid transaction sequence number.");

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
									trans.loc_value = dataReader.GetIfxDecimal(++i).ToString();
									trans.ten_curr = dataReader[++i].ToString().Trim();
									trans.ten_value = dataReader.GetIfxDecimal(++i).ToString();
									trans.loc_change = dataReader.GetIfxDecimal(++i).ToString();
									trans.variance = dataReader.GetIfxDecimal(++i).ToString();
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
									trans.tg_post_bal = dataReader.GetIfxDecimal(++i).ToString();
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

									if (!dataContext.ImportedTransactions.Any(x => x.ln_id == trans.ln_id
									                                               && x.tx_seq_nr == trans.tx_seq_nr
									                                               && x.dt_concluded == trans.dt_concluded))
									{
										dataContext.ImportedTransactions.Insert(trans);
									}
									else
									{
										Log.LogTrace($"Trans already exists {trans.ln_id} : {trans.tx_seq_nr} : {trans.dt_concluded}");
									}
								}
								catch (Exception ex)
								{
									Log.LogException(ex);
								}
							}

							dataContext.Save();
							dataReader.Close();
						}
						catch (Exception e)
						{
							Console.WriteLine(e);
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

		static void CheckDuplicateSequenceGaps()
		{
			Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

			try
			{
				using (IfxConnection connection = EstablishConnection())
				using (var dataContext = new DatabaseSyncDataContext())
				{
					var missingTransactions = GetDuplicateTransactions(connection);

					foreach (var missingtrans in missingTransactions)
					{
						try
						{
							Log.LogInfoMessage($"Requesting {missingtrans.Lane}:{missingtrans.Sequencenr}");

							IfxCommand command = connection.CreateCommand();
							command.CommandText = $"SELECT * FROM p_trans WHERE ln_id='{missingtrans.Lane}' AND tx_seq_nr='{missingtrans.Sequencenr}'";

							IfxDataReader dataReader = command.ExecuteReader();

							while (dataReader.Read())
							{
								Log.LogTrace($"Found {missingtrans.Lane}:{missingtrans.Sequencenr}");

								var trans = new StagingTransaction();

								try
								{
									int i = 0;

									trans.pl_id = dataReader[i].ToString().Trim();
									trans.ln_id = dataReader[++i].ToString().Trim();
									trans.dt_concluded = ExtractDatetimeValue(dataReader[++i]).Value;
									trans.tx_seq_nr = int.TryParse(dataReader[++i].ToString(), out var outInt)
										? outInt
										: throw new InvalidDataException("Invalid transaction sequence number.");

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
									trans.loc_value = dataReader.GetIfxDecimal(++i).ToString();
									trans.ten_curr = dataReader[++i].ToString().Trim();
									trans.ten_value = dataReader.GetIfxDecimal(++i).ToString();
									trans.loc_change = dataReader.GetIfxDecimal(++i).ToString();
									trans.variance = dataReader.GetIfxDecimal(++i).ToString();
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
									trans.tg_post_bal = dataReader.GetIfxDecimal(++i).ToString();
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

									if (!dataContext.ImportedTransactions.Any(x => x.ln_id == trans.ln_id
																				   && x.tx_seq_nr == trans.tx_seq_nr
																				   && x.dt_concluded == trans.dt_concluded))
									{
										dataContext.ImportedTransactions.Insert(trans);
									}
									else
									{
										Log.LogTrace($"Trans already exists {trans.ln_id} : {trans.tx_seq_nr} : {trans.dt_concluded}");
									}
								}
								catch (Exception ex)
								{
									Log.LogException(ex);
								}
							}

							dataContext.Save();
							dataReader.Close();
						}
						catch (Exception e)
						{
							Console.WriteLine(e);
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

		static List<uspGetListOfSequenceNrGaps_Result> GetDuplicateTransactions(IfxConnection connection)
		{
			Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

			var missing = new List<uspGetListOfSequenceNrGaps_Result>();

			using (var dataContext = new DatabaseSyncDataContext())
			{
				IfxCommand command = connection.CreateCommand();

				Log.LogInfoMessage($"Querying duplicate transactions for month : {CurrentMonth.ToString("yyyy-MM-dd 00:00:00")}");

				command.CommandText =
					$"select count(*),ln_id, tx_seq_nr " +
					$"from informix.p_trans " +
					$"where dt_concluded >= TO_DATE('{CurrentMonth.ToString("yyyy-MM-dd 00:00:00")}', '%Y-%m-%d %H:%M:%S') " +
					$"and dt_concluded < TO_DATE('{FollowingMonth.ToString("yyyy-MM-dd 00:00:00")}','%Y-%m-%d %H:%M:%S') " +
					$"Group by ln_id,tx_seq_nr " ;

				Log.LogInfoMessage(command.CommandText);

				IfxDataReader dataReader = command.ExecuteReader();

				List<string> s = new List<string>();

				while (dataReader.Read())
				{
					if (dataReader[0].ToString().Trim() == "1")
						continue;

					missing.Add(new uspGetListOfSequenceNrGaps_Result { Lane = dataReader[1].ToString().Trim(), Sequencenr = int.Parse(dataReader[2].ToString().Trim()) });
				}
			}

			Log.LogInfoMessage($"Duplicate Transactions found : {missing.Count}");
			Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

			return missing;
		}


		#region Helper methods

		static DateTime? ExtractDatetimeValue(object value)
		{
			if (value is DateTime)
				return (DateTime?)value;

			if (value is string && DateTime.TryParse(value.ToString(), out var outdt))
				return outdt;

			return null;
		}

		static int? ExtractIntegerValue(string value)
		{
			return int.TryParse(value, out var outInt) ? (int?)outInt : null;
		}

		private static string ExtractBCDValue(IfxDataReader reader, int index)
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

		static IfxConnection EstablishConnection()
		{
			try
			{
				var connection = new IfxConnection(AppSettings.MISDBConnectionString);
				connection.DatabaseLocale = "en_US.CP1252";
				connection.ClientLocale = "en_US.CP1252";
				connection.Open();

				return connection;
			}
			catch (Exception ex)
			{
				//Log.LogException(ex);
				//Log.LogTrace(ex.Message + ". Check error log for more details.");
				Console.WriteLine(ex);

				return null;
			}
		}

		#endregion
	}
}
