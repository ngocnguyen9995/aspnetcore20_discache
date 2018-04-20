using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

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
            await cache.SetCacheAsync("CurrentUser", new User{Username = "2BisBestGrill", Email = "2BFanSub@nier.com"});
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
            var user = await cache.GetCacheAsync<User>("CurrentUser");
            await context.Response.WriteAsync($"Username: {user.Username} --- Email: {user.Email}");
        }
    }
}
