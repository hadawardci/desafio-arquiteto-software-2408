using CashFlow.Entries.Domain.EntryRootAggregation;
using MediatR;

namespace CashFlow.Entries.Application.ViewEntries
{
    public class ViewEntriesHandler(IGenericRepository<Entry> _genericRepository, ICacheGateway _cacheGateway) : IRequestHandler<ViewEntriesInput, IReadOnlyCollection<ViewEntriesOutput>>
    {
        public async Task<IReadOnlyCollection<ViewEntriesOutput>> Handle(ViewEntriesInput request, CancellationToken cancellationToken)
        {
            
            var cacheKey = $"{nameof(ViewEntriesInput)}:{request.Date}";
            var cache = await _cacheGateway.GetCacheAsync<IReadOnlyCollection<ViewEntriesOutput>>(cacheKey);
            if(cache != null)
            {
                return cache;
            }

            var minDate = request.Date.ToDateTime(TimeOnly.MinValue);
            var maxDate = request.Date.ToDateTime(TimeOnly.MaxValue);
            var entries = await _genericRepository.WhereAsync(
                x=> x.RegisteredAt >= minDate && x.RegisteredAt <= maxDate
                , cancellationToken);

            await _cacheGateway.SetCacheAsync(cacheKey,entries, TimeSpan.FromMinutes(5));
            return entries.Select(x => (ViewEntriesOutput)x).ToList();
        }
    }
}
