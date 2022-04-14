
using System.Text;
using System.Text.Json;
using CacheManagerApi.CacheManager;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

public class RedisCacheManager : ICacheManager
    {
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();
        private readonly IDistributedCache _distributedCache;
        private readonly CacheSettings _cacheSettings;
        public RedisCacheManager(IDistributedCache distributedCache, IOptions<CacheSettings> cacheSettings)
        {
            this._distributedCache = distributedCache;
            this._cacheSettings = cacheSettings.Value;
        }


        public   T Set<T>(string key, T value, CacheExpireType? cacheExpireType = CacheExpireType.Minute, int? expirationInTime = null)
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
        

        public async Task<T> SetAsync<T>(string key, T value, CacheExpireType? cacheExpireType = CacheExpireType.Minute, int? expirationInTime = null)
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

            Stream utf8Json = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize<T>(value)));
    
            var bytes = Encoding.UTF8.GetBytes(  JsonSerializer.Serialize<T>(value , new JsonSerializerOptions() { WriteIndented = true }));
            await _distributedCache.SetAsync(key, bytes, options, _resetCacheToken.Token);
            return value;
        }
        

        public T Get<T>(string key)
        {
            var bytes =   _distributedCache.Get(key);
            if (bytes == null)
                return default(T);

            var value = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(bytes));
            return value?? default(T);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var bytes = await _distributedCache.GetAsync(key);
            if (bytes == null)
                return default(T);
            using(Stream utf8json = new MemoryStream(bytes))
            {
                var value = await JsonSerializer.DeserializeAsync<T>(utf8json);
                return value ?? default(T);
            };
        }

        public bool Reset(string key)
        {
            _distributedCache.Remove(key);
            return true;
        }
        public async Task<bool> ResetAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
            return true;
        }
        public bool Contains(string key)
        { 
            return _distributedCache.Get(key.ToString()) != null;
        }
        public async Task<bool> ContainsAsync(string key)
        { 
            return await _distributedCache.GetAsync(key.ToString()) != null;
        }

        public void Clear(string key)
        {
            _distributedCache.Remove(key);
        }

        public async Task ClearAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

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