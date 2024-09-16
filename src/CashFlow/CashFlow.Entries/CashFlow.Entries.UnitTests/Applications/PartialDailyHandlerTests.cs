using CashFlow.Entries.Application.PartialDaily;
using CashFlow.Entries.Domain.Gateways;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Extensions;
using System.Linq.Expressions;
using System.Threading;

namespace CashFlow.Entries.UnitTests.Applications
{
    public class PartialDailyHandlerTests
    {
        private IGenericRepository<Entry> _genericRepository;
        private readonly ICacheGateway _cacheGateway;
        private readonly PartialDailyHandler _handler;

        public PartialDailyHandlerTests()
        {
            _genericRepository = Substitute.For<IGenericRepository<Entry>>();
            _cacheGateway = Substitute.For<ICacheGateway>();
            _handler = new PartialDailyHandler(_genericRepository, _cacheGateway);
        }

        [Fact]
        public async Task Handle_ShouldReturnCachedResults_WhenCacheIsValid()
        {
            // Arrange
            var now = DateTime.Now;

            var timestamp = now.Ticks;
            var cachedOutput = new List<PartialDailyOutput>
            {
                new(100m, now, timestamp)
            }.ToImmutableList();

            _cacheGateway.GetCacheAsync<IReadOnlyCollection<PartialDailyOutput>>(Arg.Any<string>())
                .ReturnsForAnyArgs(cachedOutput);

            var request = new PartialDailyInput(timestamp);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert

            Assert.Equal(cachedOutput, result);

            await _genericRepository.DidNotReceive().GroupByAsync(
              Arg.Any<Expression<Func<Entry, bool>>>(),
              Arg.Any<Expression<Func<Entry, DateTime>>>(),
              Arg.Any<Expression<Func<IGrouping<DateTime, Entry>, PartialDailyOutput>>>(),
              Arg.Any<CancellationToken>()
          );

            await _cacheGateway.DidNotReceive().SetCacheAsync(
                Arg.Any<string>(),
                Arg.Any<IReadOnlyCollection<PartialDailyOutput>>(),
                Arg.Any<TimeSpan>()
            );
        }

        [Fact]
        public async Task Handle_ShouldFetchFromRepository_WhenCacheIsInvalid_AndHasNoUpdates()
        {
            // Arrange
            var timestamp = DateTimeOffset.UtcNow.Ticks;
            var repositoryOutput = new[]
            {
                new
                {
                    RegisteredAt = DateTime.Now,
                    TotalAmount = 100m,
                    Timestamp = timestamp
                }
            };

            ImmutableList<PartialDailyOutput> partialDailyOutputs = repositoryOutput
                .Select(x => new PartialDailyOutput(x.TotalAmount, x.RegisteredAt, x.Timestamp))
                .ToImmutableList();

            _cacheGateway.GetCacheAsync<IReadOnlyCollection<PartialDailyOutput>>(Arg.Any<string>())
                                       .Returns(await Task.FromResult<IReadOnlyCollection<PartialDailyOutput>>(result: null));

            var request = new PartialDailyInput(timestamp - 1);


            _genericRepository
                 .GroupByAsync(
                query => query.Timestamp > request.Timestamp,
                group => group.AccountingDate,
                selector => new
                {
                    RegisteredAt = selector
                        .Max(x => x.AccountingDate),
                    TotalAmount = selector
                        .Sum(x => x.Amount),
                    Timestamp = selector.Max(x => x.Timestamp),
                }, Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(Task.FromResult(repositoryOutput.ToList()));

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
          
            Assert.Contains(_genericRepository.ReceivedCalls(), call => call.GetMethodInfo().Name == "GroupByAsync");

        }
    }
}
