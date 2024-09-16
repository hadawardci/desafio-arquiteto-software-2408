using CashFlow.Consolidated.Domain.ConsolidatedDailyRootAggregate;
using CashFlow.Consolidated.Domain.Gateways;
using CashFlow.Consolidated.Domain.SharedKernel.Events;

namespace CashFlow.Consolidated.Application.Events
{
    public class EntriesUpdateEventHandler(
        IGenericRepository<ConsolidatedDaily> _genericRepository,
        IPartialDailyGateway _partialDailyGateway) : IEntriesUpdateEventHandler
    {
        public async Task Handle(EntriesUpdateEvent entriesUpdateEvent)
        {
            var timestamp = await _genericRepository
                .MaxAsync(
                   query: x => x.Id != null,
                   selector: x => x.Timestamp);

            var partialDailyOutputs = await _partialDailyGateway.GetPartialsDaily(timestamp);

            var dates = partialDailyOutputs.Select(x => x.Date);
            var consolidations = await _genericRepository.WhereAsync(x => dates.Contains(x.Date), default);

            foreach (var partialDaily in partialDailyOutputs!)
            {
                var consolidatedDaily = consolidations
                    .FirstOrDefault(x => x.Date == partialDaily.Date);
                if (consolidatedDaily != null)
                {
                    if (consolidatedDaily.Update(partialDaily.Timestamp, partialDaily.TotalAmount))
                        await _genericRepository.UpdateAsync(
                            query: x => x.Id == consolidatedDaily.Id,
                            consolidatedDaily,
                            default);
                }
                else
                {
                    consolidatedDaily = ConsolidatedDaily.Create(
                        partialDaily.TotalAmount,
                        partialDaily.Timestamp,
                        partialDaily.RegisteredAt);
                    await _genericRepository.AddAsync(consolidatedDaily, default);
                }
            }
        }
    }
}
