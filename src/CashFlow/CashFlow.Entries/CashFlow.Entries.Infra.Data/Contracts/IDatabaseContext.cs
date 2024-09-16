using MongoDB.Driver;

namespace CashFlow.Entries.Infra.Data.Contracts
{
    public interface IDatabaseContext
    {
        IMongoDatabase Database { get; }
    }
}
