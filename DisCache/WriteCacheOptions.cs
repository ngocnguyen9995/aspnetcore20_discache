using System.Threading;
using Microsoft.Extensions.Caching.Distributed;

namespace DisCache
{
    /// <summary>
    /// Options for UserWriteCachingMiddleware
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WriteCacheOptions<T>
    {
        public string Group { get; set; }
        public string Key { get; set; }
        public T Value { get; set; }
        public DistributedCacheEntryOptions Options { get; set; } = new DistributedCacheEntryOptions();
        public CancellationToken Token { get; set; }
    }
}
