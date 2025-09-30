using Microsoft.Extensions.Caching.Memory;

namespace IPGsMock;

public class ObjectCacheStorage(IMemoryCache cache)
{
    private const int DefaultAliveTimeInMin = 70;

    private readonly IMemoryCache _cache = cache;

    public void Add(string key, object value)
    {
        Add(DefaultAliveTimeInMin, key, value);
    }

    public void Add(int aliveTimeInMin, string key, object value)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(aliveTimeInMin)
        };

        _cache.Set(key, value, cacheOptions);
    }

    public object? TryGetValue(string key)
    {
        _cache.TryGetValue(key, out object? value);
        return value;
    }
}