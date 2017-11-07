using System.IO.Compression;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace QuestionnairesApi
{
    [PublicAPI]
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public string EnvironmentName { get; }
        
        public Startup(IHostingEnvironment env)
        {
            EnvironmentName = env.EnvironmentName;

            Configuration =
                new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddEnvironmentVariables()
                    .Build();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services. 
            services.AddResponseCompression(
                        x => { x.Providers.Add<GzipCompressionProvider>(); })
                    .Configure<GzipCompressionProviderOptions>(
                        x => { x.Level = CompressionLevel.Fastest; })
                    .AddRouting(
                        x => { x.LowercaseUrls = true; })
                    .AddLogging()
                    .AddMvc()
                    .AddXmlSerializerFormatters();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IMemoryCache memoryCache)
        {
            app.UseMvc()
               .UseResponseCompression()
               .UseStaticFiles()
               .Use(
                   async (context, next) =>
                   {
                       context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                       context.Response.Headers.Add("X-Frame-Options", "DENY");
                       context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                       context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                       context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
                       context.Response.Headers.Remove("X-Powered-By");
                       await next();
                   })
              
               .UseWhen(
                   x => env.IsDevelopment(),
                   x => x.UseDeveloperExceptionPage())
               .UseWhen(
                   x => env.IsProduction(),
                   x => x.UseExceptionHandler(
                       y =>
                       {
                           y.Run(
                               async context =>
                               {
                                   context.Response.StatusCode = 500;
                                   context.Response.ContentType = "text/html";
                                   await context.Response.WriteAsync("An internal server error has occured. Contact Austin.Drenski@usitc.gov.");
                               });
                       }));
        }
    }
}