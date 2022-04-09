namespace CacheManagerApi.CacheManager
{
    public interface ICacheManager : IDisposable
    {
        T Set<T>(string key, T value, Nullable<int> expirationInHours = null, CacheExpireType? cacheExpireType = CacheExpireType.Minute);
        void Clear(string key);
    } 
}
