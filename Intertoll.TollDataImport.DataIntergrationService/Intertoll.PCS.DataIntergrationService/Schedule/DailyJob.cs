using System;
using System.Data.Entity.Validation;
using Intertoll.NLogger;
using Quartz;

namespace Intertoll.PCS.DataIntergrationService.Schedule
{
    [DisallowConcurrentExecution]
    internal class UpdateCommsJob : IJob
    {
        public void Execute(IJobExecutionContext jobContext)
        {
            try
            {
                DailyProcessor.ExecuteUpdateComms();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Log.LogErrorMessage(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));

                    foreach (var ve in eve.ValidationErrors)
                    {
                        Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }
    }

    internal class SendAliveMessageJob : IJob
    {
        public void Execute(IJobExecutionContext jobContext)
        {
            try
            {
                DailyProcessor.ExecuteSendAliveMessage();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Log.LogErrorMessage(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));

                    foreach (var ve in eve.ValidationErrors)
                    {
                        Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }
    }
    
    [DisallowConcurrentExecution]
    internal class ProcessTransactionsAndIncidentsJob : IJob
    {
        public void Execute(IJobExecutionContext jobContext)
        {
            try
            {
                DailyProcessor.ProcessTransactionsAndIncidents();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Log.LogErrorMessage(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));

                    foreach (var ve in eve.ValidationErrors)
                    {
                        Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }
    }

    [DisallowConcurrentExecution]
    internal class ProcessHourlyAuditJob : IJob
    {
        public void Execute(IJobExecutionContext jobContext)
        {
            try
            {
                DailyProcessor.ProcessHourlyAudits();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Log.LogErrorMessage(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));

                    foreach (var ve in eve.ValidationErrors)
                    {
                        Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }
    }

    [DisallowConcurrentExecution]
    internal class ProcessEndOfDayTaskJob : IJob
    {
        public void Execute(IJobExecutionContext jobContext)
        {
            try
            {
                DailyProcessor.ProcessEndOfDayTask();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Log.LogErrorMessage(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));

                    foreach (var ve in eve.ValidationErrors)
                    {
                        Log.LogErrorMessage(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }
    }
}
