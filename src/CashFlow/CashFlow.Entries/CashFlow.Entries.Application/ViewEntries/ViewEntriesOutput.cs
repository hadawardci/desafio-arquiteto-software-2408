using CashFlow.Entries.Domain.EntryRootAggregation;

namespace CashFlow.Entries.Application.ViewEntries
{
    public record ViewEntriesOutput(decimal Amount, string? Description, DateTime RegistredAt)
    {
        public static implicit operator ViewEntriesOutput(Entry entry)
            => new(entry.Amount, entry.Description, entry.RegisteredAt);
    }
}
