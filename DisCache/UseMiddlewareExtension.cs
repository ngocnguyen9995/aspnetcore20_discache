using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DisCache
{
    public static class UseMiddlewareExtension
    {
        public static IApplicationBuilder WriteCache<T>(this IApplicationBuilder app, WriteCacheOptions<T> options)
        {
            return app.UseMiddleware<WriteCacheMiddleware<T>>(options);
        }

        public static IApplicationBuilder WriteCacheToGroup<T>(this IApplicationBuilder app, WriteCacheOptions<T> options)
        {
            return app.UseMiddleware<WriteCacheGroupMiddleware<T>>(options);
        }

        public static IApplicationBuilder ReadCache<T>(this IApplicationBuilder app, CacheQueryOptions options)
        {
            return app.UseMiddleware<ReadCacheMiddleware<T>>(options);
        }

        public static IApplicationBuilder ReadCacheGroup<T>(this IApplicationBuilder app, CacheQueryOptions options)
        {
            return app.UseMiddleware<ReadCacheGroupMiddlware<T>>(options);
        }

        public static IApplicationBuilder GetCacheGroupKey(this IApplicationBuilder app, CacheQueryOptions options)
        {
            return app.UseMiddleware<GetCacheGroupKeyMiddleware>(options);
        }

        public static IApplicationBuilder RemoveCache(this IApplicationBuilder app, CacheQueryOptions options)
        {
            return app.UseMiddleware<RemoveCacheMiddleware>(options);
        }

        public static IApplicationBuilder RemoveCacheGroup(this IApplicationBuilder app, CacheQueryOptions options)
        {
            return app.UseMiddleware<RemoveCacheGroupMiddleware>(options);
        }

        public static IApplicationBuilder RemoveAllCache(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RemoveAllCacheMiddlware>();
        }
    }

    public class WriteCacheMiddleware<T>
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;
        private readonly WriteCacheOptions<T> options;

        public WriteCacheMiddleware(RequestDelegate next, IDistributedCache cache, WriteCacheOptions<T> options)
        {
            this.next = next;
            this.cache = cache;
            this.options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            await cache.SetCacheAsync(options.Key, options.Value, options.Options, options.Token);
            await next(context);
        }
    }

    public class WriteCacheGroupMiddleware<T>
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;
        private readonly WriteCacheOptions<T> options;

        public WriteCacheGroupMiddleware(RequestDelegate next, IDistributedCache cache, WriteCacheOptions<T> options)
        {
            this.next = next;
            this.cache = cache;
            this.options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            await cache.AddCacheToGroupAsync(options.Group, options.Key, options.Value, options.Options, options.Token);
            await next(context);
        }
    }

    public class ReadCacheMiddleware<T>
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;
        private readonly CacheQueryOptions options;

        public ReadCacheMiddleware(RequestDelegate next, IDistributedCache cache, CacheQueryOptions options)
        {
            this.next = next;
            this.cache = cache;
            this.options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            var data = await cache.GetCacheAsync<T>(options.Key, options.Token);
            await context.Response.WriteAsync($"Data: {JsonConvert.SerializeObject(data)}\n");
            await next(context);
        }
    }

    public class ReadCacheGroupMiddlware<T>
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;
        private readonly CacheQueryOptions options;

        public ReadCacheGroupMiddlware(RequestDelegate next, IDistributedCache cache, CacheQueryOptions options)
        {
            this.next = next;
            this.cache = cache;
            this.options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            var data = await cache.GetCacheGroupAsync<T>(options.Group, options.Token);
            await context.Response.WriteAsync($"Data: {JsonConvert.SerializeObject(data)}\n");
            await next(context);
        }
    }

    public class GetCacheGroupKeyMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;
        private readonly CacheQueryOptions options;

        public GetCacheGroupKeyMiddleware(RequestDelegate next, IDistributedCache cache, CacheQueryOptions options)
        {
            this.next = next;
            this.cache = cache;
            this.options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            var keys = await cache.GetKeyGroupAsync(options.Group, options.Token);
            await context.Response.WriteAsync($"Keys: {JsonConvert.SerializeObject(keys)}\n");
            await next(context);
        }
    }

    public class RemoveCacheMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;
        private readonly CacheQueryOptions options;

        public RemoveCacheMiddleware(RequestDelegate next, IDistributedCache cache, CacheQueryOptions options)
        {
            this.next = next;
            this.cache = cache;
            this.options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            await cache.RemoveCacheAsync(options.Key, options.Token);
            await next(context);
        }
    }

    public class RemoveCacheGroupMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;
        private readonly CacheQueryOptions options;

        public RemoveCacheGroupMiddleware(RequestDelegate next, IDistributedCache cache, CacheQueryOptions options)
        {
            this.next = next;
            this.cache = cache;
            this.options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            await cache.RemoveCacheByGroupAsync(options.Group, options.Token);
            await next(context);
        }
    }

    public class RemoveAllCacheMiddlware
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;

        public RemoveAllCacheMiddlware(RequestDelegate next, IDistributedCache cache)
        {
            this.next = next;
            this.cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            await cache.FlushAllKeysAsync();
            await next(context);
        }
    }
}
