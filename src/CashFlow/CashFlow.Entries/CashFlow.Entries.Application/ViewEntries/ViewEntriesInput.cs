using MediatR;

namespace CashFlow.Entries.Application.ViewEntries
{
    public record ViewEntriesInput(DateOnly Date): IRequest<IReadOnlyCollection<ViewEntriesOutput>>
    {

    }
}
