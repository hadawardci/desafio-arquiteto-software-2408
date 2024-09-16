using Bogus;
using CashFlow.Entries.Application.ViewEntries;
using System.Linq.Expressions;

namespace CashFlow.Entries.UnitTests.Applications
{
    public class ViewEntriesHandlerTests
    {
        private readonly IGenericRepository<Entry> _genericRepository;
        private readonly ICacheGateway _cacheGateway;
        private readonly ViewEntriesHandler _handler;
        private readonly Faker<Entry> _entryFaker;

        public ViewEntriesHandlerTests()
        {
            _genericRepository = Substitute.For<IGenericRepository<Entry>>();
            _cacheGateway = Substitute.For<ICacheGateway>();
            _handler = new ViewEntriesHandler(_genericRepository, _cacheGateway);

            // Inicializando gerador de dados falsos com Bogus para a classe Entry
            _entryFaker = new Faker<Entry>()
                .CustomInstantiator(f => new Entry(
                    f.Finance.Amount(1, 1000),               // amount
                    f.Lorem.Sentence(),                      // description
                    f.Date.Past(),                           // accountingDate
                    f.Date.Past(),                           // registeredAt
                    f.Random.Guid().ToString()               // id
                ));
        }

        [Fact]
        public async Task Handle_ShouldReturnDataFromCache_WhenCacheExists()
        {
            // Arrange
            var input = new ViewEntriesInput(DateOnly.FromDateTime(DateTime.Now));
            var cacheKey = $"{nameof(ViewEntriesInput)}:{input.Date}";

            // Gerando dados falsos para o cache (ViewEntriesOutput)
            var cachedEntries = new List<ViewEntriesOutput>
        {
            new ViewEntriesOutput(100.0m, "Test Entry 1", DateTime.Now),
            new ViewEntriesOutput(200.0m, "Test Entry 2", DateTime.Now)
        };

            // Simulando que o cache já contém os dados
            _cacheGateway.GetCacheAsync<IReadOnlyCollection<ViewEntriesOutput>>(cacheKey)
                .Returns(cachedEntries);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.Equal(cachedEntries, result);
            await _genericRepository.DidNotReceive().WhereAsync(Arg.Any<Expression<Func<Entry, bool>>>(), Arg.Any<CancellationToken>());
            await _cacheGateway.Received(1).GetCacheAsync<IReadOnlyCollection<ViewEntriesOutput>>(cacheKey);
        }

        [Fact]
        public async Task Handle_ShouldQueryRepositoryAndSetCache_WhenCacheDoesNotExist()
        {
            // Arrange
            var input = new ViewEntriesInput(DateOnly.FromDateTime(DateTime.Now));
            var cacheKey = $"{nameof(ViewEntriesInput)}:{input.Date}";

            _ = _cacheGateway.GetCacheAsync<IReadOnlyCollection<ViewEntriesOutput>>(cacheKey)
                .Returns(await Task.FromResult<IReadOnlyCollection<ViewEntriesOutput>>(null));

            
            var entries = _entryFaker.Generate(5);

            Expression<Func<Entry, bool>> filter = x => x.RegisteredAt >= input.Date.ToDateTime(TimeOnly.MinValue) && x.RegisteredAt <= input.Date.ToDateTime(TimeOnly.MaxValue);

            var viewEntriesOutputs = entries.Select(entry => (ViewEntriesOutput)entry).ToList();

            _genericRepository.WhereAsync(filter, Arg.Any<CancellationToken>())
                .ReturnsForAnyArgs(entries);

            // Act
            var result = await _handler.Handle(input, CancellationToken.None);

            // Assert
            Assert.Equal(viewEntriesOutputs.Count, result.Count);
            Assert.True(result.All(r => viewEntriesOutputs.Any(v => v.Equals(r))));
          
            Assert.Contains(_genericRepository.ReceivedCalls(), call => call.GetMethodInfo().Name == "WhereAsync");

            Assert.Contains(_cacheGateway.ReceivedCalls(), call => call.GetMethodInfo().Name == "SetCacheAsync");

        }
    }

}