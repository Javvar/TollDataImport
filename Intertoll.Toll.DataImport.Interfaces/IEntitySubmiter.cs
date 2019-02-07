using System;
using Intertoll.Toll.DataImport.Interfaces.Entities;
using Intertoll.Toll.DataImport.Interfaces.EntityBuilders;

namespace Intertoll.Toll.DataImport.Interfaces
{
    public interface IEntitySubmitter
    {
        ITollDataProvider DataProvider { get; set; }
    }

    public interface ITransactionSubmitter : IEntitySubmitter
    {
        ITollTransaction Submit(ITollTransaction transaction);
        ITransactionEntityBuilder EntityBuilder { get; set; }
    }

    public interface IETCTransactionSubmitter 
    {
	    IETCTollTransaction Submit(IETCTollTransaction transaction);
        IETCTransactionEntityBuilder EntityBuilder { get; set; }
        IETCTransactionEntityBuilder ETCEntityBuilder { get; set; }
    }

    public interface IIncidentSubmitter : IEntitySubmitter
    {
        ITollIncident Submit(ITollIncident incident);
        IIncidentEntityBuilder EntityBuilder { get; set; }
    }

    public interface ISessionSubmitter : IEntitySubmitter
    {
        bool Submit(ITollSession incident);
    }

    public interface IStaffLoginSubmitter : IEntitySubmitter
    {
        bool Submit(ITollStaffLogin incident);
    }

    public interface IHourlyAuditSubmitter : IEntitySubmitter
    {
        bool Submit(ITollHourlyAudit audit);
    }

    public interface ILaneAliveSubmitter : IEntitySubmitter
    {
        void Submit(Guid LaneGuid);
    }

    /*public interface IIncidentSubmitter : IEntitySubmitter
    {
        ITollDataProvider DataContext { get; set; }

        bool Submit(ref ITollTransaction transaction);
        bool Submit(ITollTransaction transaction, ITollETCTransaction etcTransaction);
        bool Submit(ref ITollIncident incident);
        bool Submit(ref ITollStaffLogin staffLogin);
        bool Submit(ref ITollSession session);
        bool Submit(ref ITollHourlyAudit audit, int hour);
        bool SubmitAlive(Guid laneGuid);
    }*/
}
