using Newtonsoft.Json;
using StackExchange.Redis;

namespace CashFlow.Entries.Infra.Data.Gateways
{
    public class RedisCacheService(IConnectionMultiplexer connectionMultiplexer) : ICacheGateway
    {
        private readonly IDatabase _database = connectionMultiplexer.GetDatabase();


        public async Task SetCacheAsync<T>(string key, T value, TimeSpan expiry) where T : class
        {
            var cache = JsonConvert.SerializeObject(value);
            await _database.StringSetAsync(key, cache , expiry);
        }

        public async Task<T?> GetCacheAsync<T>(string key) where T : class
        {
            var cache = await _database.StringGetAsync(key);
            if (cache.HasValue && !cache.IsNullOrEmpty && !cache.IsNull)
                return JsonConvert.DeserializeObject<T>(cache!);
            return null;
        }

        public async Task<bool> Clean(string key) =>
            await _database.KeyDeleteAsync(key);
    }
}
