using System;
using System.Collections.Generic;
using System.Linq;
using Intertoll.NLogger;
using Intertoll.TollDataImport.DataRequest.Data;
using Intertoll.TollDataImport.DataRequest.Data.Model;
using Intertoll.TollDataImport.DataRequest.Managers;

namespace Intertoll.TollDataImport.DataRequest
{
    public class IncidentRequestProcessor
    {
        public static void SendIncidents(int _LaneID, List<int> _SequenceNumbers)
        {
            if (_SequenceNumbers.Count < 11)
            {
                Log.LogInfoMessage(_LaneID + ": " + _SequenceNumbers.Aggregate((a, b) => a + ',' + b));
            }

            var IncidentsFound = 0;

            using (PCSStagingDataContext dc = new PCSStagingDataContext())
            {
                foreach (var sequenceNumber in _SequenceNumbers)
                {
                    string laneCode = LookupManager.Lanes(_LaneID);
                    var foundIncident = dc.Incidents.FirstOrDefault(x => x.LaneCode == laneCode && x.IncidentSeqNr == sequenceNumber);

                    if (foundIncident != null)
                    {
                        Log.LogTrace(sequenceNumber + " found in sent items ");

                        if (SendIncident(dc, foundIncident))
                            IncidentsFound++;

                    }
                    else
                    {
                        var foundIncidentData = dc.GetIncidentByIncidentSeqNumber(_LaneID, sequenceNumber);

                        Log.LogTrace(sequenceNumber + " found in SV DB ");
                        if (foundIncidentData != null)
                        {

                            if (SendIncident(dc, foundIncidentData))
                                IncidentsFound++;
                        }
                    }
                }

                Log.LogInfoMessage("Found incidents " + "(" + _LaneID + ")" + IncidentsFound + "/" + _SequenceNumbers.Count);
            }
        }


        private static bool SendIncident(PCSStagingDataContext dc, Incident incidentData)
        {
            Log.LogInfoMessage(string.Format("[Enter] sending incident Lane{0} Sequence{1}", incidentData.LaneCode, incidentData.IncidentSeqNr));

            //StaffLogin
            var LaneCode = LookupManager.Lanes(incidentData.LaneCode);
            var staffLogin = dc.StaffLogins.FirstOrDefault(x => x.LocationGUID == LaneCode &&
                                                           x.StartDate <= incidentData.IncidentSetDate &&
                                                           x.EndDate >= incidentData.IncidentSetDate);

            if (staffLogin == null)
            {
                Log.LogErrorMessage(string.Format("Staff login not found: incidentset date ({0})", incidentData.IncidentSetDate));
                return false;
            }

            var syncIncident = new Intertoll.Data.Incident();

            //add Incident
            syncIncident.Description = incidentData.Description;
            syncIncident.IncidentGUID = Guid.NewGuid();
            syncIncident.IncidentSeqNr = (int)incidentData.IncidentSeqNr;
            syncIncident.IncidentSetDate = incidentData.IncidentSetDate;
            syncIncident.IncidentStatusGUID = Guid.Parse("F10FE5B2-7ED3-DE11-9533-001517C991CF");
            syncIncident.IncidentTypeGUID = LookupManager.IncidentTypes(incidentData.IncidentCode);
            syncIncident.LaneSeqNr = (int)incidentData.LaneSeqNr;
            syncIncident.LastIncidentSeqNr = null;
            syncIncident.StaffLoginGUID = staffLogin.StaffLoginGUID;

            long Transaction_Identifier = 0;

            if (long.TryParse(incidentData.Transaction_Identifier, out Transaction_Identifier))
            {
                if (Transaction_Identifier > 0)
                {
                    var transaction = dc.Transactions.FirstOrDefault(x => x.Transaction_Identifier == incidentData.Transaction_Identifier && x.LaneCode == incidentData.LaneCode);
                    if (transaction != null)
                    {
                        syncIncident.TransactionGUID = transaction.TransGuid == null ? (Guid?)null : transaction.TransGuid;
                    }
                    else
                    {
                        Log.LogFatal(string.Format("Incident without a transaction: Lane{0} Sequence:{1}", incidentData.LaneCode, incidentData.IncidentSeqNr));
                        return false;
                    }
                }
            }

            Sync.Client.SyncClient.SubmitIncident(syncIncident);

            Log.LogInfoMessage(string.Format("[Exit] sending incident Lane{0} Sequence{1}", incidentData.LaneCode, incidentData.IncidentSeqNr));
            
            return true;
        }

        private static bool SendIncident(PCSStagingDataContext dc, uspGetIncidentByLaneAndSeq_Result incidentData)
        {
            Log.LogInfoMessage(string.Format("[Enter] sending incident Lane{0} Sequence{1}", incidentData.LaneCode, incidentData.IncidentSeqNr));

            //StaffLogin
            var LaneCode = LookupManager.Lanes(incidentData.LaneCode);
            var staffLogin = dc.StaffLogins.FirstOrDefault(x => x.LocationGUID == LaneCode &&
                                                           x.StartDate <= incidentData.IncidentSetDate &&
                                                           x.EndDate >= incidentData.IncidentSetDate);

            if (staffLogin == null)
            {
                Log.LogErrorMessage(string.Format("Staff login not found: incidentset date ({0})", incidentData.IncidentSetDate));
                return false;
            }

            var syncIncident = new Intertoll.Data.Incident();

            //add Incident
            syncIncident.Description = incidentData.Description;
            syncIncident.IncidentGUID = Guid.NewGuid();
            syncIncident.IncidentSeqNr = (int)incidentData.IncidentSeqNr;
            syncIncident.IncidentSetDate = incidentData.IncidentSetDate;
            syncIncident.IncidentStatusGUID = Guid.Parse("F10FE5B2-7ED3-DE11-9533-001517C991CF");
            syncIncident.IncidentTypeGUID = LookupManager.IncidentTypes(incidentData.IncidentCode);
            syncIncident.LaneSeqNr = (int)incidentData.LaneSeqNr;
            syncIncident.LastIncidentSeqNr = null;
            syncIncident.StaffLoginGUID = staffLogin.StaffLoginGUID;

            if (incidentData.Transaction_Identifier > 0)
            {
                var transaction = dc.Transactions.FirstOrDefault(x => x.Transaction_Identifier == incidentData.Transaction_Identifier.ToString() && x.LaneCode == incidentData.LaneCode);
                if (transaction != null)
                {
                    syncIncident.TransactionGUID = transaction.TransGuid == null ? (Guid?)null : transaction.TransGuid;
                }
                else
                {
                    Log.LogFatal(string.Format("Incident without a transaction: Lane{0} Sequence:{1}", incidentData.LaneCode, incidentData.IncidentSeqNr));
                    return false;
                }

            }

            SaveIncident(syncIncident, incidentData, staffLogin.StaffLoginGUID, true);
            Sync.Client.SyncClient.SubmitIncident(syncIncident);

            Log.LogInfoMessage(string.Format("[Exit] sending incident Lane{0} Sequence{1}", incidentData.LaneCode, incidentData.IncidentSeqNr));

            return true;
        }

        private static void SaveIncident(Intertoll.Data.Incident realIncident, uspGetIncidentByLaneAndSeq_Result dbIncident, Guid staffLoginGuid, Boolean transactionExist)
        {
            try
            {
                //Insert into DB
                using (var context = new PCSStagingDataContext())
                {
                    context.Incidents.Insert(
                        new Incident()
                        {
                            CollectorID = dbIncident.CollectorID.HasValue ? (int)dbIncident.CollectorID : (int?)null,
                            Description = dbIncident.Description,
                            Incident_Identifier = dbIncident.Incident_Identifier.ToString(),
                            IncidentAckDate = realIncident.IncidentAckDate,
                            IncidentClearedDate = realIncident.IncidentClearedDate,
                            IncidentCode = dbIncident.IncidentCode,
                            IncidentGUID = realIncident.IncidentGUID,
                            IncidentSeqNr = realIncident.IncidentSeqNr,
                            IncidentSetDate = realIncident.IncidentSetDate,
                            IncidentStatusGUID = realIncident.IncidentStatusGUID,
                            IncidentTypeGUID = realIncident.IncidentTypeGUID,
                            LaneCode = dbIncident.LaneCode,
                            LaneSeqNr = realIncident.LaneSeqNr,
                            LastIncidentSeqNr = realIncident.LastIncidentSeqNr,
                            StaffLoginGUID = staffLoginGuid,
                            TimeStamp = DateTime.Now,
                            Transaction_Identifier = dbIncident.Transaction_Identifier.ToString(),
                            TransactionGUID = realIncident.TransactionGUID,
                            TransactionExist = transactionExist
                        });
                    context.Save();
                }
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }

        }
    }
}
