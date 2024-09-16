namespace CashFlow.Consolidated.Domain.SharedKernel.Events
{
    public interface IEntriesUpdateEventHandler
    {
        Task Handle(EntriesUpdateEvent entriesUpdateEvent);
    }
}
