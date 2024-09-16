using MongoDB.Driver;

namespace CashFlow.Consolidated.Infra.Data.Contracts
{
    public interface IDatabaseContext
    {
        IMongoDatabase Database { get; }
    }
}
