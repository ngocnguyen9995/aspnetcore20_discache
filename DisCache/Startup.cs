
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace DisCache
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedRedisCache(options => 
            {
                options.Configuration = "localhost:6379";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.WriteCacheToGroup(new WriteCacheOptions<User>
            {
                Group = "YorHa", 
                Key = "2B",
                Value = new User { Username = "2BisBestGrill", Email = "2BFanSub@nier.com" },
            });
            app.WriteCache(new WriteCacheOptions<User>
            {
                Key = "YorHa_9S",
                Value = new User { Username = "9Sx2B4ever", Email = "9Sx2B4ever@nier.com"}
            });

            app.WriteCacheToGroup(new WriteCacheOptions<User>
            {
                Group = "Rogue",
                Key = "2A",
                Value = new User { Username = "A2x2B4ever", Email = "A2FanSub@nier.com"}
            });

            app.ReadCacheGroup<User>(new CacheQueryOptions
            {
                Group = "YorHa"
            });

            app.ReadCacheGroup<User>(new CacheQueryOptions
            {
                Group = "Rogue"
            });

            app.RemoveAllCache();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Done");
            });
        }
    }
}
