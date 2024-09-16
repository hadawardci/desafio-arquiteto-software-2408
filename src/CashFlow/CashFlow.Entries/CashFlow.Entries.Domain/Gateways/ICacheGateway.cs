namespace CashFlow.Entries.Domain.Gateways
{
    public interface ICacheGateway
    {
        Task SetCacheAsync<T>(string key, T value, TimeSpan expiry) where T : class;
        Task<T?> GetCacheAsync<T>(string key) where T : class;
        Task<bool> Clean(string key);
    }
}
