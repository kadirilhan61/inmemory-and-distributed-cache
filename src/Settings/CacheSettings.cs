 
    public class CacheSettings
    {
        /// <summary>
        /// Cache aktif olma durumu.
        /// </summary>
        public bool CacheEnabled { get; set; } = false;
        public CacheType CacheType { get; set; } = CacheType.InMemoryCache;
        /// <summary>
        /// Verilen süre sonunda mutlak olarak Cache silinir. Default olarak false'dır.
        /// </summary>
        public bool IsAbsoluteExpirationRelative { get; set; } = false;
        public int CacheExpirationInTime { get; set; } = 1;
        public CacheExpireType CacheExpireType { get; set; } = CacheExpireType.Hour;
        public string RedisConfiguration { get; set; } = "127.0.0.1:6379";
        public string RedisInstanceName { get; set; } = "RedisInstance_";
    }
 