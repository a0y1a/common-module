namespace KeKeSoftPlatform.Common
{
    public class CacheProvider
    {
        private static ICacheManager _InstanceCacheManager;

        static CacheProvider()
        {
            _InstanceCacheManager = new MemoryCacheManager();
        }

        public static ICacheManager Instance
        {
            get { return _InstanceCacheManager; }
        }

        public static void Register(ICacheManager cache)
        {
            _InstanceCacheManager = cache;
        }
    }
}