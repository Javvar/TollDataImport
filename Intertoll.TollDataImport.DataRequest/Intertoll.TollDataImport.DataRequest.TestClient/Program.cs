using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using IBM.Data.Informix;
using Intertoll.DataImport.Database.Sync.Data;
using Intertoll.DataImport.Database.Sync.Data.DataContext;

namespace Intertoll.TollDataImport.DataRequest.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting...");
            //while (true)
            {
                try
                {
	                //TableCounts();

					TransactionSyncProcess();

					Console.WriteLine("Done");
                }
                catch (Exception ex)
                {
	                Console.WriteLine(ex);
				}

	            Console.ReadLine();
			}
        }

	    private static void TableCounts()
	    {
		    using (var dataContext = new DatabaseSyncDataContext())
			using (IfxConnection connection = EstablishConnection())
		    {
			    IfxCommand command = connection.CreateCommand();
			    command.CommandText =
				    $"select count(*), ln_id, tx_seq_nr  " +
				    $"from informix.p_trans " +
				    $"where dt_concluded >= TO_DATE('2018-11-01 00:00:00', '%Y-%m-%d %H:%M:%S') " +
				    $"and dt_concluded < TO_DATE('2018-12-01 00:00:00','%Y-%m-%d %H:%M:%S') and ln_id='04RS' " +
				    $"Group by ln_id,tx_seq_nr " +
				    $"order by tx_seq_nr  ";


				

			    IfxDataReader dataReader = command.ExecuteReader();

				List<string> s = new List<string>();

			    while (dataReader.Read())
			    {
					var d = $"{dataReader[0].ToString().Trim()} : {dataReader[1].ToString().Trim()}";
				    s.Add(d);
					//if (!dataContext.ImportedTransactions.Any(x => x.ln_id == "03RS" && x.tx_seq_nr.ToString() == d))
					//	Console.WriteLine(d);
				}

				File.WriteAllLines("seqNrs.txt",s);

			    /*IfxCommand command2 = connection.CreateCommand();
			    command2.CommandText =
				    $"select count(*) from informix.p_trans where dt_concluded >= TO_DATE('2018-11-01 00:00:00', '%Y-%m-%d %H:%M:%S') and dt_concluded<TO_DATE('2018-12-01 00:00:00','%Y-%m-%d %H:%M:%S')";




			    IfxDataReader dataReader2 = command2.ExecuteReader();

			    while (dataReader2.Read())
			    {
				    Console.WriteLine(dataReader2[0].ToString().Trim());
			    }

			    IfxCommand command3 = connection.CreateCommand();
			    command3.CommandText =
				    $"select count(*) from informix.p_trans where dt_concluded >= TO_DATE('2018-12-01 00:00:00', '%Y-%m-%d %H:%M:%S') and dt_concluded<TO_DATE('2019-01-01 00:00:00','%Y-%m-%d %H:%M:%S')";




			    IfxDataReader dataReader3 = command3.ExecuteReader();

			    while (dataReader3.Read())
			    {
				    Console.WriteLine(dataReader3[0].ToString().Trim());
			    }

			    IfxCommand command4 = connection.CreateCommand();
			    command4.CommandText =
				    $"select count(*) from informix.p_trans where dt_concluded >= TO_DATE('2019-01-01 00:00:00', '%Y-%m-%d %H:%M:%S') and dt_concluded<TO_DATE('2019-02-01 00:00:00','%Y-%m-%d %H:%M:%S')";




			    IfxDataReader dataReader4 = command4.ExecuteReader();

			    while (dataReader4.Read())
			    {
				    Console.WriteLine(dataReader4[0].ToString().Trim());
			    }*/


			}
	    }

	    private static void TransactionSyncProcess()
		{
			//Log.LogInfoMessage($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

			Console.WriteLine($"[Enter] {System.Reflection.MethodBase.GetCurrentMethod().Name}");

			try
			{
				using (IfxConnection connection = EstablishConnection())
				{
					{
						using (var dataContext = new DatabaseSyncDataContext())
						{
							foreach (var reqItem in AppSettings.RequestItems)
							{
								try
								{
									IfxCommand command = connection.CreateCommand();
									command.CommandText = $"SELECT * FROM p_trans WHERE ln_id='{reqItem.Value}' AND tx_seq_nr='{reqItem.Key}'";

									Console.WriteLine(command.CommandText);

									IfxDataReader dataReader = command.ExecuteReader();

									var processedTrans = new List<string>();

									while (dataReader.Read())
									{
										Console.WriteLine($"Trans found {reqItem.Value} : {reqItem.Key} ");

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

											if (!processedTrans.Any(x => x == trans.ln_id + trans.tx_seq_nr + trans.dt_concluded))
											{
												if (!dataContext.ImportedTransactions.Any(x => x.ln_id == trans.ln_id 
																						 	   && x.tx_seq_nr == trans.tx_seq_nr 
												                                               && x.dt_concluded == trans.dt_concluded))
												{
													dataContext.ImportedTransactions.Insert(trans);
												}
											}

											processedTrans.Add(trans.ln_id + trans.tx_seq_nr + trans.dt_concluded);

											Console.WriteLine($"Trans found {trans.ln_id} : {trans.tx_seq_nr} : {trans.dt_concluded}");
										}
										catch (Exception ex)
										{
											//Log.LogException(ex);
											Console.WriteLine(ex);
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
				}
			}
			catch (Exception ex)
			{
				//Log.LogException(ex);
				Console.WriteLine(ex);
			}

			//Log.LogInfoMessage($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
			Console.WriteLine($"[Exit] {System.Reflection.MethodBase.GetCurrentMethod().Name}");
		}

		private static DateTime? ExtractDatetimeValue(object value)
		{
			if (value is DateTime)
				return (DateTime?)value;

			if (value is string && DateTime.TryParse(value.ToString(), out var outdt))
				return outdt;

			return null;
		}

		private static int? ExtractIntegerValue(string value)
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

		private static IfxConnection EstablishConnection()
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
	}
}
