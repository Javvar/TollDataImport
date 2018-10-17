using IBM.Data.Informix;
using Intertoll.DataImport.Database.Sync.Data;
using Intertoll.DataImport.Database.Sync.Data.DataContext;
//using Intertoll.DataImport.Data;
//using Intertoll.DataImport.Data.DataContext;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Intertoll.DataImport.Database.Sync
{
    public static class DatabaseSyncManager
    {
        private static IfxConnection _connection;
        private static StreamWriter _outStream;
        private static IfxCommand _command;
        private static IfxDataReader _dataReader;

        public static void StartSynchingProcess()
        {
            if (true)
            {
                while (true)
                {
                    if(EstablishConnection())
                    {
                        _command = _connection.CreateCommand();

                        try
                        {

                            using (var dataContext = new DatabaseSyncDataContext())
                            {
                                _command.CommandText = "SELECT FIRST 100 * " +
                                                       "FROM p_inc ";

                                var lastIncident = dataContext.ImportedIncidents.FirstOrDefault();

                                //if (lastIncident == null)                                
                                //    _command.CommandText = _command.CommandText 
                                //        + string.Format("WHERE dt_generated > TO_DATE('{0},'%H:%M:%S')",
                                //                        lastIncident.dt_generated?.ToString("yyyy-MM-dd hh:mm:ss"));                  

                                _dataReader = _command.ExecuteReader();               

                                while (_dataReader.Read())
                                {
                                    var inc = new StagingIncident();

                                    try
                                    {
                                        int i = 0;
                                        inc.pl_id = _dataReader[i].ToString(); 
                                        inc.ln_id = _dataReader[++i].ToString(); 
                                        inc.dt_generated = ExtractDatetimeValue(_dataReader[++i]);

                                        inc.in_seq_nr = int.TryParse(_dataReader[++i].ToString(), out var outInt) ? outInt : 
                                                                        throw new InvalidDataException("Invalid incident sequence number.");

                                        inc.ir_type = _dataReader[++i].ToString(); 
                                        inc.ir_subtype = _dataReader[++i].ToString(); 

                                        inc.tx_seq_nr = int.TryParse(_dataReader[++i].ToString(), out outInt) ? (int?)outInt : null; 
                                        inc.ts_seq_nr = int.TryParse(_dataReader[++i].ToString(), out outInt) ? (int?)outInt : null;

                                        inc.us_id = _dataReader[++i].ToString(); 

                                        inc.ft_id = int.TryParse(_dataReader[++i].ToString(), out outInt) ? (int?)outInt : null;
                                        inc.pg_group = int.TryParse(_dataReader[++i].ToString(), out outInt) ? (int?)outInt : null;
                                        inc.cg_group = int.TryParse(_dataReader[++i].ToString(), out outInt) ? (int?)outInt : null;

                                        inc.vg_group = _dataReader[++i].ToString(); 
                                        inc.mvc = _dataReader[++i].ToString(); 
                                        inc.avc = _dataReader[++i].ToString(); 
                                        inc.svc = _dataReader[++i].ToString(); 

                                        inc.er_id = int.TryParse(_dataReader[++i].ToString(), out outInt) ? (int?)outInt : null;

                                        inc.pm_id = _dataReader[++i].ToString(); 
                                        inc.card_nr = _dataReader[++i].ToString(); 
                                        inc.mask_nr = _dataReader[++i].ToString(); 
                                        inc.ca_id = _dataReader[++i].ToString(); 
                                        inc.ct_id = _dataReader[++i].ToString(); 
                                        inc.tx_indic = _dataReader[++i].ToString(); 
                                        inc.lm_id = _dataReader[++i].ToString(); 
                                        inc.as_id = _dataReader[++i].ToString(); 
                                        inc.rep_indic = _dataReader[++i].ToString(); 
                                        inc.rd_id = _dataReader[++i].ToString(); 
                                        inc.maint_indic = _dataReader[++i].ToString();
                                        inc.req_indic = int.TryParse(_dataReader[++i].ToString(), out outInt) ? (int?)outInt : null;
                                        inc.ts_dt_started = ExtractDatetimeValue(_dataReader[++i]);

                                        try
                                        {
                                            inc.in_amt = _dataReader[++i].ToString();
                                        }
                                        catch (Exception)
                                        {
                                            inc.in_amt = "-";
                                        }
                                        
                                        inc.in_data = _dataReader[++i].ToString(); 
                                        inc.tg_bl_id = _dataReader[++i].ToString(); 
                                        inc.tg_mfg_id = _dataReader[++i].ToString(); 
                                        inc.tg_card_type = _dataReader[++i].ToString(); 
                                        inc.tg_reader = _dataReader[++i].ToString();
                                        inc.tg_tx_seq_nr = int.TryParse(_dataReader[++i].ToString(), out outInt) ? (int?)outInt : null;
                                        inc.avc_in_seq_nr = int.TryParse(_dataReader[++i].ToString(), out outInt) ? (int?)outInt : null;
                                        inc.avc_in_type_id = int.TryParse(_dataReader[++i].ToString(), out outInt) ? (int?)outInt : null;
                                        inc.avc_dt_generated = ExtractDatetimeValue(_dataReader[++i]); 
                                    }
                                    catch(Exception ex)
                                    {
                                        //todo: log
                                    }

                                    dataContext.ImportedIncidents.Insert(inc);
                                }
                                
                                dataContext.Save();
                                _dataReader.Close();
                            }                     
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }

                    Thread.Sleep(10000);
                }
            }
        }

        private static DateTime? ExtractDatetimeValue(object value)
        {
            if (value is DateTime)
                return (DateTime?)value;

            return null;
        }

        public static void EndSynchingProcess()
        {

        }

        private static bool EstablishConnection()
        {
            try
            {
                _connection = new IfxConnection("Host=tongaat;Server=tongaat_tcp;Service=2000;Protocol=onsoctcp;UID=informix;Password=informix123;Database=tongaat;");
                _connection.DatabaseLocale = "en_US.CP1252";
                _connection.ClientLocale = "en_US.CP1252";
                _connection.Open();
            }
            catch (Exception excep)
            {
                return false;
            }

            return true;
        }
    }
}
