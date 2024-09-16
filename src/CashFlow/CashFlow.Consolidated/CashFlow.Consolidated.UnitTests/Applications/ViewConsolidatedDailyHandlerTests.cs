
namespace CashFlow.Consolidated.UnitTests.Applications
{
    public class ViewConsolidatedDailyHandlerTests
    {
        private readonly IGenericRepository<ConsolidatedDaily> _genericRepository;
        private readonly ViewConsolidatedDailyHandler _handler;

        public ViewConsolidatedDailyHandlerTests()
        {
            // Substitui a dependência do repositório usando NSubstitute
            _genericRepository = Substitute.For<IGenericRepository<ConsolidatedDaily>>();
            _handler = new ViewConsolidatedDailyHandler(_genericRepository);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(1)]
        [InlineData(0)]
        public async Task Handle_ShouldReturnConsolidateds_WhenDataExists(int count)
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow.AddDays(-1);

            var faker = new Faker<ConsolidatedDaily>()
                 .CustomInstantiator(f => new ConsolidatedDaily(
                    f.Finance.Amount(),
                    f.Random.Long(),
                    f.Date.Between(startDate, endDate)));

            var fakeConsolidateds = faker.Generate(count);

            _genericRepository.WhereAsync(Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(), Arg.Any<CancellationToken>())
                  .Returns(Task.FromResult((ICollection<ConsolidatedDaily>)fakeConsolidateds));

            var input = new ViewConsolidatedDailyInput(startDate, endDate);

            // Act
            var result = await _handler.Handle(input, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(count, result.Count);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmpty_WhenNoDataExists()
        {
            // Arrange
            var startDate = new DateTime(2023, 2, 1);
            var endDate = new DateTime(2023, 2, 28);

            _genericRepository.WhereAsync(Arg.Any<Expression<Func<ConsolidatedDaily, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((ICollection<ConsolidatedDaily>)[]));

            var input = new ViewConsolidatedDailyInput(startDate, endDate);

            // Act
            var result = await _handler.Handle(input, default);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        
        }
    }
}
