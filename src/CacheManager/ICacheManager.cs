namespace CacheManagerApi.CacheManager
{
    public interface ICacheManager : IDisposable
    {
        T Set<T>(string key, T value, CacheExpireType? cacheExpireType = CacheExpireType.Minute, int? expirationInTime = null);
        void Clear(string key);
    } 
}
