using System.Threading;
using Microsoft.Extensions.Caching.Distributed;

namespace DisCache
{
    /// <summary>
    /// Options for querying the cache database
    /// </summary>
    public class CacheQueryOptions
    {
        public string Group { get; set; }
        public string Key { get; set; }
        public CancellationToken Token { get; set; }
    }
}
