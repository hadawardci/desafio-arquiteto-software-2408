namespace CashFlow.Consolidated.UnitTests.Applications
{
    public class EntriesUpdateEventHandlerTests
    {
        private readonly IGenericRepository<ConsolidatedDaily> _genericRepository;
        private readonly IPartialDailyGateway _partialDailyGateway;
        private readonly EntriesUpdateEventHandler _handler;

        public EntriesUpdateEventHandlerTests()
        {
            _genericRepository = Substitute.For<IGenericRepository<ConsolidatedDaily>>();
            _partialDailyGateway = Substitute.For<IPartialDailyGateway>();
            _handler = new EntriesUpdateEventHandler(_genericRepository, _partialDailyGateway);
        }

        [Fact]
        public async Task Handle_ShouldUpdateExistingConsolidatedDaily_WhenPartialDataExists()
        {
            // Arrange

            var partialDailyOutputs = new List<PartialDailyOutput>
            {
                new(200m, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-3).Ticks),
                new(150m, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-2).Ticks),
                new(100m, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1).Ticks),
            };

            long timestamp = partialDailyOutputs.First().Timestamp - 999;

            var consolidatedDailies = partialDailyOutputs.Select(p => ConsolidatedDaily.Create(
                p.TotalAmount,
                p.Timestamp,
                p.RegisteredAt
            )).ToList();

            _genericRepository.MaxAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<Expression<Func<ConsolidatedDaily, long>>>()
            ).Returns(Task.FromResult(timestamp));

            _partialDailyGateway.GetPartialsDaily(timestamp).Returns(partialDailyOutputs);

            _genericRepository.WhereAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns(Task.FromResult((ICollection<ConsolidatedDaily>)consolidatedDailies.Take(1).Select(
                x => new ConsolidatedDaily(x.TotalAmount - 9, x.Timestamp - 999, x.Date)
                ).ToList()));

            _genericRepository.UpdateAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<ConsolidatedDaily>(),
                Arg.Any<CancellationToken>()
            ).Returns(true);

            _genericRepository.AddAsync(
                Arg.Any<ConsolidatedDaily>(),
                Arg.Any<CancellationToken>()
            ).Returns(Task.CompletedTask);

            var entriesUpdateEvent = new EntriesUpdateEvent(true);

            // Act
            await _handler.Handle(entriesUpdateEvent);

            // Assert
            await _genericRepository.Received(1).MaxAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<Expression<Func<ConsolidatedDaily, long>>>()
            );

            await _partialDailyGateway.Received(1).GetPartialsDaily(timestamp);

            await _genericRepository.Received(1).WhereAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<CancellationToken>()
            );

            await _genericRepository.Received(1).UpdateAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<ConsolidatedDaily>(),
                Arg.Any<CancellationToken>()
            );

            await _genericRepository.Received(2).AddAsync(
                Arg.Any<ConsolidatedDaily>(),
                Arg.Any<CancellationToken>()
            );
        }

        [Fact]
        public async Task Handle_ShouldAddNewConsolidatedDaily_WhenNoExistingData()
        {
            // Arrange
            var timestamp = 1234567890L;

            var partialDailyOutputs = new List<PartialDailyOutput>
            {
                new PartialDailyOutput(100m, DateTime.Now.AddDays(-1), timestamp),
                new PartialDailyOutput(150m, DateTime.Now.AddDays(-2), timestamp),
                new PartialDailyOutput(200m, DateTime.Now.AddDays(-3), timestamp)
            };

            _genericRepository.MaxAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<Expression<Func<ConsolidatedDaily, long>>>()
            ).Returns(Task.FromResult(timestamp));

            _partialDailyGateway.GetPartialsDaily(timestamp).Returns(partialDailyOutputs);

            _genericRepository.WhereAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns(Task.FromResult((ICollection<ConsolidatedDaily>)new List<ConsolidatedDaily>()));

            _genericRepository.UpdateAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<ConsolidatedDaily>(),
                Arg.Any<CancellationToken>()
            ).Returns(true);

            _genericRepository.AddAsync(
                Arg.Any<ConsolidatedDaily>(),
                Arg.Any<CancellationToken>()
            ).Returns(Task.CompletedTask);

            var entriesUpdateEvent = new EntriesUpdateEvent(true);

            // Act
            await _handler.Handle(entriesUpdateEvent);

            // Assert
            await _genericRepository.Received(1).MaxAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<Expression<Func<ConsolidatedDaily, long>>>()
            );

            await _partialDailyGateway.Received(1).GetPartialsDaily(timestamp);

            await _genericRepository.Received(1).WhereAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<CancellationToken>()
            );

            await _genericRepository.DidNotReceive().UpdateAsync(
                Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(),
                Arg.Any<ConsolidatedDaily>(),
                Arg.Any<CancellationToken>()
            );

            await _genericRepository.Received(3).AddAsync(
                Arg.Any<ConsolidatedDaily>(),
                Arg.Any<CancellationToken>()
            );
        }
    }
}
