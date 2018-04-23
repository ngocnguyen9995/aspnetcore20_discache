using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DisCache
{
    public static class UseMiddlewareExtension
    {
        public static IApplicationBuilder UserWriteCaching(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WriteCachingMiddleware>();
        }

        public static IApplicationBuilder UserReadCaching(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ReadCachingMiddleware>();
        }
    }

    public class WriteCachingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;

        public WriteCachingMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            this.next = next;
            this.cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            await cache.AddCacheToGroupAsync("YorHa", "2B", new User{Username = "2BisBestGrill", Email = "2BFanSub@nier.com"});
            await cache.AddCacheToGroupAsync("YorHa", "9S", new User { Username = "9Sx2B4ever", Email = "9Sx2B@nier.com" });
            await cache.AddCacheToGroupAsync("YorHa", "6O", new User { Username = "Operator6O", Email = "LunarTear@nier.com" });
            await cache.AddCacheToGroupAsync("Rogue", "A2", new User { Username = "A2Kills2B", Email = "A2IsMyWaifu@nier.com" });
            await cache.AddCacheToGroupAsync("Rogue", "Emil", new User { Username = "Determination", Email = "BigHead@nier.com" });
            await cache.AddCacheToGroupAsync("God", "YokoTaro", new User { Username = "ShitSquareEnix", Email = "YokoTaro@nier.com" });
            await this.next(context);
        }
    }

    public class ReadCachingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;

        public ReadCachingMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            this.next = next;
            this.cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            /*
            var user = await cache.GetCacheAsync<User>("YorHa_2B");
            await context.Response.WriteAsync($"Username: {user.Username} --- Email: {user.Email}");
            */
            List<string> yorha = await cache.GetKeyGroupAsync("YorHa");
            List<string> rogue = await cache.GetKeyGroupAsync("Rogue");
            List<string> god = await cache.GetKeyGroupAsync("God");
            await context.Response.WriteAsync("YorHa group:\n");
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
            await cache.RemoveCacheByGroupAsync("God");
        }
    }
}
