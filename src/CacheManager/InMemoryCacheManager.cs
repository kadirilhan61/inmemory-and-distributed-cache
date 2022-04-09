
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
        public T Set<T>(string key, T value, Nullable<int> expirationInHours = null, CacheExpireType? cacheExpireType = CacheExpireType.Minute)
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();

            var expireTime = expirationInHours.HasValue ? expirationInHours.Value : _cacheSettings.CacheExpirationInTime;

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
                    span = TimeSpan.FromMinutes(expireTime);
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
        /// <summary>
        /// Generic Type olarak Cache'e data set ediyor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationInMinutes"></param>
        /// <returns></returns>
        public T SetForExcel<T>(object key, T value)
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();

            options.SetPriority(CacheItemPriority.Normal);
            options.SetSlidingExpiration(TimeSpan.FromSeconds(120));
            options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

            _memoryCache.Set(key, value, options);
            return value;
        }

        /// <summary>
        /// Cache var mı diye kontrol ediyor.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(object key)
        {
            return _memoryCache.TryGetValue(key, out object result);
        }
        /// <summary>
        /// İlgili Cache'i dönüyor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(object key)
        { 
            return _memoryCache.TryGetValue(key, out T result) ? result : default(T);
        }
        /// <summary>
        /// İlgili Cache'i siliyor.
        /// </summary>
        /// <param name="key"></param>
        public void Clear(object key)
        {
            _memoryCache.Remove(key);
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

        public void Clear(string key)
        {
            throw new NotImplementedException();
        }
         
        public void Dispose()
        {
            this.Dispose();
        }
    }