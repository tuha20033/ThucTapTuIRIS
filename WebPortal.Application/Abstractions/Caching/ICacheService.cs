using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebPortal.Application.Abstractions.Caching;

/// <summary>
/// Small cache abstraction. Backed by Redis in Infrastructure with an in-memory fallback.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);

    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default);

    Task RemoveAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Gets cached value if available; otherwise executes factory, caches and returns the result.
    /// </summary>
    Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<CancellationToken, Task<T>> factory, CancellationToken ct = default);
}
