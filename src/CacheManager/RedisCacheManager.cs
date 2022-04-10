
using System.Text;
using System.Text.Json;
using CacheManagerApi.CacheManager;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

public class RedisCacheManager : ICacheManager
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CacheSettings _cacheSettings;
        public RedisCacheManager(IDistributedCache distributedCache, IOptions<CacheSettings> cacheSettings)
        {
            this._distributedCache = distributedCache;
            this._cacheSettings = cacheSettings.Value;
        }

        public void Clear(string key)
        {
            throw new NotImplementedException();
        }


        public T Set<T>(string key, T value, CacheExpireType? cacheExpireType = CacheExpireType.Minute, int? expirationInTime = null)
        {
            var options = new DistributedCacheEntryOptions();
            var expireTime = expirationInTime.HasValue ? expirationInTime.Value : _cacheSettings.CacheExpirationInTime;

            TimeSpan? span = null;

            switch (cacheExpireType ?? _cacheSettings.CacheExpireType)
            {
                case  CacheExpireType.Minute:
                    span = TimeSpan.FromMinutes(expireTime);
                    break;
                case  CacheExpireType.Hour:
                    span = TimeSpan.FromHours(expireTime);
                    break;
                case  CacheExpireType.Day:
                    span = TimeSpan.FromDays(expireTime);
                    break;
                default:
                    span = TimeSpan.FromSeconds(expireTime);
                    break;
            }

            if (_cacheSettings.IsAbsoluteExpirationRelative)
                options.AbsoluteExpirationRelativeToNow = span.Value;
            else
                options.SetSlidingExpiration(span.Value);

            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<T>(value));
            _distributedCache.Set(key, bytes, options);
            return value;
        }


        public void Dispose()
        {
            this.Dispose();
        }
    }