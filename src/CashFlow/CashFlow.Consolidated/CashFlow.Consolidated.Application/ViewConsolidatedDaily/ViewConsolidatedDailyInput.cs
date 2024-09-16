using MediatR;

namespace CashFlow.Consolidated.Application.ViewConsolidatedDaily
{
    public record ViewConsolidatedDailyInput(
        DateTime StartAt, DateTime EndAt
        ) : IRequest<IReadOnlyCollection<ViewConsolidatedDailyOutput>>
    {
        public DateTime StartDate => new(StartAt.Ticks,  DateTimeKind.Utc);
        public DateTime EndDate => new(EndAt.Ticks,  DateTimeKind.Utc);
    }
}
