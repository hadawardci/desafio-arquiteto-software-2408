using MediatR;

namespace CashFlow.Entries.Application.PartialDaily
{
    public record PartialDailyInput(long Timestamp) : IRequest<IReadOnlyCollection<PartialDailyOutput>>
    {
    }
}
