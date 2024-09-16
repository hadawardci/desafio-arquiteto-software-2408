
using CashFlow.Entries.Application.PartialDaily;
using CashFlow.Entries.Application.ViewEntries;
using CashFlow.Entries.Domain.EntryRootAggregation;
using CashFlow.Entries.Domain.SharedKernel.Events;
using MediatR;

namespace CashFlow.Entries.Application.AddEntry
{
    public class AddEntryHandler(IGenericRepository<Entry> _genericRepository, ICacheGateway _cacheGateway, IMessagePublisherGateway _messagePublisherGateway) : IRequestHandler<AddEntryInput>
    {
        public async Task Handle(AddEntryInput request, CancellationToken cancellationToken)
        {
            var entry = Entry.Create(request.Amount, request.Description, request.AccountingDate);
            await _genericRepository.AddAsync(entry, cancellationToken);

            var date = DateOnly.Parse(entry.RegisteredAt.ToString("yyyy-MM-dd"));
            var cacheKey = $"{nameof(ViewEntriesInput)}:{date}";

            await _cacheGateway.Clean(cacheKey);
            await _cacheGateway.Clean(nameof(PartialDailyInput));

            _messagePublisherGateway.PublishFollowUpAsync(new EntriesUpdateEvent(true));
        }
    }
}
