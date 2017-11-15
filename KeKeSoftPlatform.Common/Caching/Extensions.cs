using System;

// ReSharper disable once CheckNamespace
namespace KeKeSoftPlatform.Common
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CacheExtensions
    {
        public static bool Debug { get; set; }
        static CacheExtensions()
        {
            Debug = false;
        }
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, MemoryCacheManager.DEFAULT_CACHE_TIME, acquire);
        }

        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire) 
        {
            if (Debug)
            {
                return acquire();
            }
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }
            var result = acquire();
            cacheManager.Set(key, result, cacheTime);
            return result;
        }
    }
}
