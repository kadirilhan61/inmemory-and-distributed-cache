namespace CacheManagerApi.CacheManager
{
    public interface ICacheManager : IDisposable
    {
        T Get<T>(string key);
        Task<T> GetAsync<T>(string key);
        T Set<T>(string key, T value, CacheExpireType? cacheExpireType = CacheExpireType.Minute, int? expirationInTime = null);
        Task<T> SetAsync<T>(string key, T value, CacheExpireType? cacheExpireType = CacheExpireType.Minute, int? expirationInTime = null);
        void Clear(string key);
        Task ClearAsync(string key);
        bool Contains(string key);
        Task<bool> ContainsAsync(string key);
        void Reset();
    } 
}
