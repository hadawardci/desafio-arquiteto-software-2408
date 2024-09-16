using CashFlow.Consolidated.Domain.ConsolidatedDailyRootAggregate;
using System.Runtime.CompilerServices;

namespace CashFlow.Consolidated.Application.ViewConsolidatedDaily
{
    public record ViewConsolidatedDailyOutput(
        DateOnly Date, decimal TotalAmount
        );

    public static class ViewConsolidatedDailyOutputExtensions
    {
        public static IReadOnlyCollection<ViewConsolidatedDailyOutput> ToOutput(this ICollection<ConsolidatedDaily> consolidateds)
        {
            return consolidateds.Select(x =>
            new ViewConsolidatedDailyOutput(
                DateOnly.FromDateTime(x.Date), x.TotalAmount))
                .ToList();
        }
    }

}