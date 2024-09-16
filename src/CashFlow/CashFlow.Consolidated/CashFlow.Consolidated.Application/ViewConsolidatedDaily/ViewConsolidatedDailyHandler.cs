using CashFlow.Consolidated.Domain.ConsolidatedDailyRootAggregate;
using CashFlow.Consolidated.Domain.Gateways;
using MediatR;

namespace CashFlow.Consolidated.Application.ViewConsolidatedDaily
{
    public class ViewConsolidatedDailyHandler(
        IGenericRepository<ConsolidatedDaily> genericRepository) : IRequestHandler<ViewConsolidatedDailyInput, IReadOnlyCollection<ViewConsolidatedDailyOutput>>
    {
        private readonly IGenericRepository<ConsolidatedDaily> _genericRepository = genericRepository;

        public async Task<IReadOnlyCollection<ViewConsolidatedDailyOutput>> Handle(ViewConsolidatedDailyInput request, CancellationToken cancellationToken)
        {
            var consolidateds = await _genericRepository.WhereAsync(query =>
                query.Date >= request.StartDate
                && query.Date <= request.EndDate
             ,cancellationToken);
            //var consolidateds = await _genericRepository.GetAllAsync(cancellationToken)
            //    ;
            return consolidateds.ToOutput();
        }
    }
}
