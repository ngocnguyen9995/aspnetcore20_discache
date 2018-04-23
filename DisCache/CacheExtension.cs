using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DisCache
{
    // Extension methods for cache
    public static class CacheExtension
    {
        private static List<string> keys = new List<string>();  // keys added to cache

        // Set cache with specific key, store value as a JSON
        public static async Task SetCacheAsync<T>(this IDistributedCache cache, string key, T val)
        {
            await AddKeyAsync(key);
            await cache.SetStringAsync(key, JsonConvert.SerializeObject(val));
        }

        // Add a new key-value cache entry to a specific group
        public static async Task AddCacheToGroupAsync<T>(this IDistributedCache cache, string group, string key, T val)
        {
            await AddKeyToGroupAsync(group, key);
            await cache.SetCacheAsync(group + "_" + key, val);
        }

        // Get cache value from a specific key, return an object
        public static async Task<T> GetCacheAsync<T>(this IDistributedCache cache, string key)
        {
            var val = await cache.GetStringAsync(key);
            if (val == null)
                return default(T);
            return JsonConvert.DeserializeObject<T>(val);
        }

        // Get all keys from a specif group, return a list of strings
        public static async Task<List<string>> GetKeyGroupAsync(this IDistributedCache cache, string group)
        {
            List<string> keyList = new List<string>();
            await Task.Run(() =>
            {
                foreach (string key in keys)
                {
                    if (key.Contains(group))
                        keyList.Add(key);
                }
            });
            return keyList;
        }

        // Get all keys, return a list of strings
        public static async Task<List<string>> GetAllKeysAsync(this IDistributedCache cache)
        {
            return await GetKeyListAsync();
        }

        // Clear cache data by key
        public static async Task RemoveCacheAsync(this IDistributedCache cache, string key)
        {
            await cache.RemoveAsync(key);
        }

        // Clear all cache by group
        public static async Task RemoveCacheByGroupAsync(this IDistributedCache cache, string group)
        {
            List<string> keyList = await cache.GetKeyGroupAsync(group);
            foreach (string key in keyList)
            {
                await cache.RemoveCacheAsync(key);
            }
        }

        // Helper methods

        private static async Task AddKeyAsync(string key)
        {
            await Task.Run(() => 
            {
                if (!keys.Contains(key)){
                    keys.Add(key);
                }
            });
        }

        private static void AddKey(string key)
        {
            if (!keys.Contains(key))
            {
                keys.Add(key);
            }
        }
        private static async Task AddKeyToGroupAsync(string group, string key)
        {
            await AddKeyAsync(group + "_" + key);
        }

        private static void AddKeyToGroup(string group, string key)
        {
            AddKey(group + "_" + key);
        }

        private static async Task<List<string>> GetKeyListAsync()
        {
            return await Task.Run(() => keys);
        }

        
    }
}
