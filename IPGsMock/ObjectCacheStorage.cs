using Microsoft.Extensions.Caching.Memory;

namespace IPGsMock;

public class ObjectCacheStorage(IMemoryCache cache)
{
    private readonly IMemoryCache _cache = cache;

    public void Add(string key, object value)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        _cache.Set(key, value, cacheOptions);
    }

    public object? TryGetValue(string key)
    {
        _cache.TryGetValue(key, out object? value);
        return value;
    }
}