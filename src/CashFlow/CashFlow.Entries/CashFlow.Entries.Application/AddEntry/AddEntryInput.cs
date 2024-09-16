using MediatR;

namespace CashFlow.Entries.Application.AddEntry
{
    public record AddEntryInput(decimal Amount, string? Description, DateTime? AccountingDate = null) : IRequest
    {
    }
}
