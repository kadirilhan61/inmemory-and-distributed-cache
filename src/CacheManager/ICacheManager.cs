namespace CacheManagerApi.CacheManager
{
    public interface ICacheManager : IDisposable
    {
        T Set<T>(string key, T value, Nullable<int> expirationInHours = null);
        void Clear(string key);
    } 
}
