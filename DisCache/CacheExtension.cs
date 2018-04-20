using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DisCache
{
    public static class CacheExtension
    {

        public static async Task SetCacheAsync<T>(this IDistributedCache cache, string key, T val)
        {
            await cache.SetStringAsync(key, JsonConvert.SerializeObject(val));
        }

        public static async Task<T> GetCacheAsync<T>(this IDistributedCache cache, string key)
        {
            var val = await cache.GetStringAsync(key);
            if (val == null)
                return default(T);
            return JsonConvert.DeserializeObject<T>(val);
        }

        public static async Task RemoveCacheAsync(this IDistributedCache cache, string key)
        {
            await cache.RemoveCacheAsync(key);
        }
    }
}
