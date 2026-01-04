using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using WebPortal.Application.Abstractions.Caching;

namespace WebPortal.Infrastructure.Caching;


public sealed class HybridCacheService : ICacheService
{
    private readonly IMemoryCache _memory;
    private readonly IDatabase? _redis;
    private readonly JsonSerializerOptions _json;

    public HybridCacheService(IMemoryCache memory, IServiceProvider services)
    {
        _memory = memory;
        _redis = services.GetService<IConnectionMultiplexer>()?.GetDatabase();
        _json = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (_redis is not null)
        {
            try
            {
                var value = await _redis.StringGetAsync(key).ConfigureAwait(false);
                if (value.HasValue)
                {
                    // RedisValue is not a string; convert explicitly to avoid overload ambiguity.
                    var json = value.ToString();
                    return JsonSerializer.Deserialize<T>(json, _json);
                }
            }
            catch
            {
                
            }
        }

        return _memory.TryGetValue(key, out T? found) ? found : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        if (_redis is not null)
        {
            try
            {
                var json = JsonSerializer.Serialize(value, _json);
                await _redis.StringSetAsync(key, json, ttl).ConfigureAwait(false);
                return;
            }
            catch
            {
                
            }
        }

        _memory.Set(key, value, ttl);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        if (_redis is not null)
        {
            try
            {
                await _redis.KeyDeleteAsync(key).ConfigureAwait(false);
            }
            catch
            {
                
            }
        }

        _memory.Remove(key);
    }

    public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<CancellationToken, Task<T>> factory, CancellationToken ct = default)
    {
        var existing = await GetAsync<T>(key, ct);
        if (existing is not null)
            return existing;

        var created = await factory(ct);
        await SetAsync(key, created, ttl, ct);
        return created;
    }
}
