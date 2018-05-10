using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Linq;
using System.Threading;
using System;

namespace DisCache
{
    /// <summary>
    /// Extension methods for Distributed Cache, the specific implementation is RedisCache in ASP.Net Core
    /// Include methods save generic objects in Redis cache server, reading cache from keys, adding key-value caches to groups, and remove cache.
    /// Use async methods whenever possible, as they won't block the server.
    /// </summary>
    public static class CacheExtension
    {
        private static volatile ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, allowAdmin=true"); // Should have config var
        
        /// <summary>
        /// Asyncronously save a generic object to Redis in a key-value store
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task SetCacheAsync<T>(this IDistributedCache cache, string key, T val, CancellationToken token = default(CancellationToken))
        {
            try
            {
                await cache.SetStringAsync(key, JsonConvert.SerializeObject(val), token);
            }
            catch (ArgumentNullException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Asyncronously save a generic object to Redis in a key-value store with options parameter
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="options">Options for the cache, including AbsoluteExpiration & SlidingExpiration</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task SetCacheAsync<T>(this IDistributedCache cache, string key, T val, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            try
            {
                await cache.SetStringAsync(key, JsonConvert.SerializeObject(val), options, token);
            }
            catch (ArgumentException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            
        }

        /// <summary>
        /// Syncronous version of SetCacheAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static void SetCache<T>(this IDistributedCache cache, string key, T val)
        {
            try
            {
                cache.SetString(key, JsonConvert.SerializeObject(val));
            }
            catch (ArgumentNullException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Syncronous version of SetCacheAsync with options parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="options"></param>
        public static void SetCache<T>(this IDistributedCache cache, string key, T val, DistributedCacheEntryOptions options)
        {
            try
            {
                cache.SetString(key, JsonConvert.SerializeObject(val), options);
            }
            catch(ArgumentNullException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Asyncronously add a new key-value store as a cache to a group (The naming convention for a key under a group is: <GroupName>_<KeyName>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="group"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task AddCacheToGroupAsync<T>(this IDistributedCache cache, string group, string key, T val, CancellationToken token = default(CancellationToken))
        {
            try
            {
                await cache.SetStringAsync(group + "_" + key, JsonConvert.SerializeObject(val), token);
            }
            catch (ArgumentNullException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// AddCacheToGroupAsync with options parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="group"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="options"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task AddCacheToGroupAsync<T>(this IDistributedCache cache, string group, string key, T val, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            try
            {
                await cache.SetStringAsync(group + "_" + key, JsonConvert.SerializeObject(val), options, token);
            }
            catch (ArgumentNullException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Syncronous version of AddCacheToGroupAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="group"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static void AddCacheToGroup<T>(this IDistributedCache cache, string group, string key, T val)
        {
            try
            {
                cache.SetString(group + "_" + key, JsonConvert.SerializeObject(val));
            }
            catch (ArgumentNullException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// AddCacheToGroup with options parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="group"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="options"></param>
        public static void AddCacheToGroup<T>(this IDistributedCache cache, string group, string key, T val, DistributedCacheEntryOptions options)
        {
            try
            {
                cache.SetString(group + "_" + key, JsonConvert.SerializeObject(val), options);
            }
            catch (ArgumentNullException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Asyncronously returns the value of the cache from a key in Redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T> GetCacheAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default(CancellationToken))
        {
            var val = await cache.GetStringAsync(key, token);
            return val == null ? default(T) : JsonConvert.DeserializeObject<T>(val);
        }

        /// <summary>
        /// Syncronous version of GetCacheAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetCache<T>(this IDistributedCache cache, string key)
        {
            var val = cache.GetString(key);
            return val == null ? default(T) : JsonConvert.DeserializeObject<T>(val);
        }

        /// <summary>
        /// Asyncronously returns all key-value cache stores associated with a group
        /// Returns null if there is no match with the group parameter
        /// Refrains from using this if possible, as this queries the whole keyspace and may cause block in server
        /// Use GetCacheAsync instead when possible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="group"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T[]> GetCacheGroupAsync<T>(this IDistributedCache cache, string group, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();
            string[] keyList = GetKeys(group);
            if (keyList == null || keyList.Length == 0)
                return null;
            T[] result = new T[keyList.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = await cache.GetCacheAsync<T>(keyList[i]);
            }
            return result;
        }

        /// <summary>
        /// Syncronous version of GetCacheGroupAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static T[] GetCacheGroup<T>(this IDistributedCache cache, string group)
        {
            string[] keyList = GetKeys(group);
            T[] result = new T[keyList.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = cache.GetCache<T>(keyList[i]);
            }
            return result;
        }

        /// <summary>
        /// Asyncronously returns all keys associated with a group in Redis
        /// Returns an empty list if no match
        /// Use with caution because this queries the whole keyspace, which may cause block in the server
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="group"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string[]> GetKeyGroupAsync(this IDistributedCache cache, string group, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();
            return await Task.Run(() => GetKeys(group));
        }

        /// <summary>
        /// Syncronous version of GetKeyGroupAsync
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static string[] GetKeyGroup(this IDistributedCache cache, string group)
        {
            return GetKeys(group);
        }

        /// <summary>
        /// Asyncronously removes a key-value store in Redis
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task RemoveCacheAsync(this IDistributedCache cache, string key, CancellationToken token = default(CancellationToken))
        {
            await cache.RemoveAsync(key, token);
        }

        /// <summary>
        /// Syncronous version of RemoveCacheAsync
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        public static void RemoveCache(this IDistributedCache cache, string key)
        {
            cache.Remove(key);
        }

        /// <summary>
        /// Asyncronously removes all key-value stores of a group in Redis
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="group"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        // Clear all cache by group
        public static async Task RemoveCacheByGroupAsync(this IDistributedCache cache, string group, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();
            string[] keyList = cache.GetKeyGroup(group);
            if (keyList.Length > 0)
            {
                foreach (string key in keyList)
                {
                    await cache.RemoveAsync(key);
                }
            }
        }

        /// <summary>
        /// Syncronous version of RemoveCacheByGroupAsync
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="group"></param>
        public static void RemoveCacheByGroup(this IDistributedCache cache, string group)
        {
            string[] keyList = cache.GetKeyGroup(group);
            if (keyList.Length > 0)
            {
                foreach (string key in keyList)
                {
                    cache.Remove(key);
                }
            }
        }

        /// <summary>
        /// Flush the Redis cache database
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task FlushAllKeysAsync(this IDistributedCache cache, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();
            var conn = redis.GetServer("localhost", 6379);
            await conn.FlushDatabaseAsync();
        }

        /// <summary>
        /// Syncronous version of FlushAllKeysAsync
        /// </summary>
        /// <param name="cache"></param>
        public static void FlushAllKeys(this IDistributedCache cache)
        {
            var conn = redis.GetServer("localhost", 6379);
            conn.FlushDatabase();
        }

        // Helper methods

        /// <summary>
        /// Returns all keys whose string contains the input parameter. Can be use to return all keys by passing in an empty string
        /// Use with caution as this can cause block on the server
        /// Returns an empty list if no match
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private static string[] GetKeys(string group)
        {
            var conn = redis.GetServer("localhost", 6379);
            string match = "*" + group + "*";
            var keys = conn.Keys(pattern: match);
            var keyList = keys.Select(key => (string)key).ToArray();
            return keyList;
        }
        
    }
}
