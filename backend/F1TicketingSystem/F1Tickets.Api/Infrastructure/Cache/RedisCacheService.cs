using StackExchange.Redis;
using System.Text.Json;

namespace F1Tickets.Api.Infrastructure.Cache
{
    public class RedisCacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        // Get from Cache
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value!);
        }

        // Set into Cache
        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var json = JsonSerializer.Serialize(value);

            await _db.StringSetAsync(key, json, expiration);
        }

        // Invalidate Cache
        public async Task KeyDeleteAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        // Get Lock for Cache
        public async Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiry)
        {
            return await _db.StringSetAsync(
                key,
                value,
                expiry,
                When.NotExists
            );
        }

        // Release Lock for Cache after finished
        public async Task ReleaseLockAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}
