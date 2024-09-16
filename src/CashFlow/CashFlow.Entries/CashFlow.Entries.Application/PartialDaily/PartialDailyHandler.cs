using CashFlow.Entries.Domain.EntryRootAggregation;
using MediatR;
using System.Collections.Immutable;

namespace CashFlow.Entries.Application.PartialDaily
{
    public class PartialDailyHandler(IGenericRepository<Entry> _genericRepository, ICacheGateway _cacheGateway) : IRequestHandler<PartialDailyInput, IReadOnlyCollection<PartialDailyOutput>>
    {
        public async Task<IReadOnlyCollection<PartialDailyOutput>> Handle(PartialDailyInput request, CancellationToken cancellationToken)
        {

            var cacheKey = nameof(PartialDailyInput);
            var cache = await _cacheGateway.GetCacheAsync<IReadOnlyCollection<PartialDailyOutput>>(cacheKey);
            if (cache != null && cache.Max(x => x.RegisteredAt).Ticks == request.Timestamp)
            {
                return cache;
            }

            var partialsDaily = await _genericRepository.GroupByAsync(
                query => query.Timestamp > request.Timestamp,
                group => group.AccountingDate,
                selector => new
                {
                    RegisteredAt = selector
                        .Max(x => x.AccountingDate),
                    TotalAmount = selector
                        .Sum(x => x.Amount),
                    Timestamp = selector.Max(x => x.Timestamp),
                },
                cancellationToken);
            if (partialsDaily == null) 
                return [];

            var partialDailyOutputs = partialsDaily
                .Select(x => new PartialDailyOutput(x.TotalAmount, x.RegisteredAt, x.Timestamp))
                .ToImmutableList();
            await _cacheGateway.SetCacheAsync(cacheKey, partialDailyOutputs, TimeSpan.FromMinutes(5));
            return partialDailyOutputs;
        }
    }
}
