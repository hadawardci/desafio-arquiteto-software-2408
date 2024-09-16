using CashFlow.Entries.Application.AddEntry;
using CashFlow.Entries.Application.PartialDaily;
using CashFlow.Entries.Application.ViewEntries;

namespace CashFlow.Entries.UnitTests.Applications
{
    public class AddEntryHandlerTests
    {
        private readonly IGenericRepository<Entry> _genericRepository;
        private readonly ICacheGateway _cacheGateway;
        private readonly IMessagePublisherGateway _messagePublisherGateway;
        private readonly AddEntryHandler _handler;

        public AddEntryHandlerTests()
        {
            _genericRepository = Substitute.For<IGenericRepository<Entry>>();
            _cacheGateway = Substitute.For<ICacheGateway>();
            _messagePublisherGateway = Substitute.For<IMessagePublisherGateway>();
            _handler = new AddEntryHandler(_genericRepository, _cacheGateway, _messagePublisherGateway);
        }

        [Fact]
        public async Task Handle_ShouldAddEntry_AndCleanCache_AndPublishEvent()
        {
            // Arrange
            var request = new AddEntryInput(
                Amount: 100m,
                Description: "Test Entry",
                AccountingDate: DateTime.Today
            );

            var entry = Entry.Create(request.Amount, request.Description, request.AccountingDate);
            var date = DateOnly.Parse(entry.RegisteredAt.ToString("yyyy-MM-dd"));
            var cacheKey = $"{nameof(ViewEntriesInput)}:{date}";

            // Act
            await _handler.Handle(request, default);

            // Assert
            await _genericRepository.Received(1).AddAsync(
                Arg.Is<Entry>(e =>
                    e.Amount == entry.Amount &&
                    e.Description == entry.Description &&
                    e.AccountingDate == entry.AccountingDate &&
                    e.RegisteredAt.Date == DateTime.Today
                ),
                Arg.Any<CancellationToken>()
            );

            await _cacheGateway.Received(1).Clean(cacheKey);
            await _cacheGateway.Received(1).Clean(nameof(PartialDailyInput));

            _messagePublisherGateway.Received(1).PublishFollowUpAsync(Arg.Is<EntriesUpdateEvent>(e => e.MustAdd == true));
        }

    }
}
