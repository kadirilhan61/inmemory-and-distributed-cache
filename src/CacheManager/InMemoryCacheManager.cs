
using CacheManagerApi.CacheManager;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

public class InMemoryCacheManager : ICacheManager 
{
    private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();
    private readonly IMemoryCache _memoryCache;
    private readonly CacheSettings _cacheSettings;
    /// <summary>
    /// Cache Manager
    /// </summary>
    /// <param name="memoryCache"></param>
    public InMemoryCacheManager(IMemoryCache memoryCache, IOptions<CacheSettings> cacheSettings)
    {
        this._memoryCache = memoryCache;
        this._cacheSettings = cacheSettings.Value;
    }

    /// <summary>
    /// Generic Type olarak Cache'e data set ediyor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expirationInMinutes"></param>
    /// <returns></returns>
    public T Set<T>(string key, T value, CacheExpireType? cacheExpireType = CacheExpireType.Minute, int? expirationInTime = null)
    {
        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();

        var expireTime = expirationInTime.HasValue ? expirationInTime.Value : _cacheSettings.CacheExpirationInTime;

        TimeSpan? span = null;

        switch (cacheExpireType ?? _cacheSettings.CacheExpireType)
        {
            case CacheExpireType.Minute:
                span = TimeSpan.FromMinutes(expireTime);
                break;
            case CacheExpireType.Hour:
                span = TimeSpan.FromHours(expireTime);
                break;
            case CacheExpireType.Day:
                span = TimeSpan.FromDays(expireTime);
                break;
            default:
                span = TimeSpan.FromSeconds(expireTime);
                break;
        }

        options.SetPriority(CacheItemPriority.Normal);

        if (_cacheSettings.IsAbsoluteExpirationRelative)
            options.AbsoluteExpirationRelativeToNow = span.Value;
        else
            options.SetSlidingExpiration(span.Value);

        options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

        _memoryCache.Set(key, value, options);
        return value;
    }

    
    public Task<T> SetAsync<T>(string key, T value, CacheExpireType? cacheExpireType = CacheExpireType.Minute, int? expirationInTime = null)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// İlgili Cache'i dönüyor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T Get<T>(string key)
    { 
        return _memoryCache.TryGetValue<T>(key, out T result) ? result : default(T);
    }
 
    public Task<T> GetAsync<T>(string key)
    { 
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cache var mı diye kontrol ediyor.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(string key)
    {
        return _memoryCache.TryGetValue(key, out object result);
    }

    public Task<bool> ContainsAsync(string key)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// İlgili Cache'i siliyor.
    /// </summary>
    /// <param name="key"></param>
    public void Clear(string key)
    {
        _memoryCache.Remove(key);
    }

    public Task ClearAsync(string key)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// CancellationTokenSource iptaline dayalı olarak T önbellek girişlerinin süresi doluyor
    /// </summary>
    public void Reset()
    {
        if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested &&
            _resetCacheToken.Token.CanBeCanceled)
        {
            _resetCacheToken.Cancel();
            _resetCacheToken.Dispose();
        }
        _resetCacheToken = new CancellationTokenSource();
    }

        
    public void Dispose()
    {
        this.Dispose();
    } 
}
 