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
            var data = cache.GetCacheAsync<T>(options.Key, options.Token);
            await context.Response.WriteAsync($"Data: {JsonConvert.SerializeObject(data.Result)}");


            /*
            foreach (User user in yorhaGroup)
            {
                await context.Response.WriteAsync($"Username: {user.Username} --- Email: {user.Email}\n");
            }

            User[] rogueGroup = cache.GetCacheGroup<User>("Rogue");
            foreach (User user in rogueGroup)
            {
                await context.Response.WriteAsync($"Username: {user.Username} --- Email: {user.Email}\n");
            }
            foreach (string key in yorha)
            {
                await context.Response.WriteAsync($"key: {key}\n");
                var user = await cache.GetCacheAsync<User>(key);
                await context.Response.WriteAsync($"Username: {user.Username} --- Email: {user.Email}\n");
            }
            await context.Response.WriteAsync("Rogue group:\n");
            foreach (string key in rogue)
            {
                await context.Response.WriteAsync($"key: {key}\n");
                var user = await cache.GetCacheAsync<User>(key);
                await context.Response.WriteAsync($"Username: {user.Username} --- Email: {user.Email}\n");
            }
            await context.Response.WriteAsync("God group:\n");
            
            foreach (string key in god)
            {
                await context.Response.WriteAsync($"key: {key}\n");
                var user = await cache.GetCacheAsync<User>(key);
                await context.Response.WriteAsync($"Username: {user.Username} --- Email: {user.Email}\n");
            }
            */
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
            var data = cache.GetCacheGroupAsync<T>(options.Group, options.Token);
            await context.Response.WriteAsync($"Data: {JsonConvert.SerializeObject(data.Result)}");
        }
    }
}
