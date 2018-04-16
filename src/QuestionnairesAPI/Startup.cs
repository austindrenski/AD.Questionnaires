using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using AD.ApiExtensions.Configuration;
using AD.ApiExtensions.Conventions;
using AD.ApiExtensions.Filters;
using AD.ApiExtensions.Primitives;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace QuestionnairesApi
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public class Startup : IStartup
    {
        /// <summary>
        ///
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        ///
        /// </summary>
        private string EnvironmentName => Configuration["environment"];

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public Startup([NotNull] IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public IServiceProvider ConfigureServices([NotNull] IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return
                // Add framework services.
                services
                    .AddLogging(x => x.AddConsole())
                    .AddApiVersioning(
                        x =>
                        {
                            x.AssumeDefaultVersionWhenUnspecified = true;
                            x.DefaultApiVersion = new ApiVersion(1, 0);
                        })
                    .AddResponseCompression(x => x.Providers.Add<GzipCompressionProvider>())
                    .Configure<GzipCompressionProviderOptions>(x => x.Level = CompressionLevel.Fastest)
                    .AddRouting(x => x.LowercaseUrls = true)
                    .AddAntiforgery(
                        x =>
                        {
                            x.HeaderName = "x-xsrf-token";
                            x.Cookie.HttpOnly = true;
                            x.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                        })
                    .AddSwaggerGen(
                        x =>
                        {
                            x.DescribeAllEnumsAsStrings();
                            x.IncludeXmlComments($"{Path.Combine(ApplicationEnvironment.ApplicationBasePath, nameof(QuestionnairesApi))}.xml");
                            x.IgnoreObsoleteActions();
                            x.IgnoreObsoleteProperties();
                            x.MapType<GroupingValues<string, string>>(() => new Schema { Type = "string" });
                            x.SwaggerDoc("v1", new Info { Title = "Questionnaires API", Version = "v1" });
                            x.OperationFilter<SwaggerOptionalOperationFilter>();
                        })
                    .AddMvc(
                        x =>
                        {
                            x.AddDelimitedOutputFormatter();
                            x.AddXmlOutputFormatter();
                            x.Conventions.Add(new KebabControllerModelConvention());
                            x.FormatterMappings.SetMediaTypeMappingForFormat("html", "text/html");
                            x.ModelMetadataDetailsProviders.Add(new KebabBindingMetadataProvider());
//                            x.ModelMetadataDetailsProviders.Add(new RequiredBindingMetadataProvider());
                            x.RespectBrowserAcceptHeader = true;

// TODO: supports experimental custom XML style
//                            x.OutputFormatters.Add(
//                                new HtmlOutputFormatter<IEnumerable<Survey>>(
//                                    "Views/Table.cshtml",
//                                    (c, o) => Survey.CreateEnumerable((IEnumerable<XElement>) o)));
                        })
                    .AddJsonOptions(
                        x =>
                        {
                            x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                            x.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                            x.SerializerSettings.ContractResolver = new KebabContractResolver();
                        })
                    .Services
                    .BuildServiceProvider();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure([NotNull] IApplicationBuilder app)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.Use(
                   async (context, next) =>
                   {
                       context.Response.Headers.Add("Server", string.Empty);
                       context.Response.Headers.Add("referrer-policy", "no-referrer");
                       context.Response.Headers.Add("x-content-type-options", "nosniff");
                       context.Response.Headers.Add("x-frame-options", "deny");
                       context.Response.Headers.Add("x-xss-protection", "1; mode=block");
                       await next();
                   })
               .UseStaticFiles()
               .UseSwagger(x => x.RouteTemplate = "docs/{documentName}/swagger.json")
               .UseSwaggerUI(
                   x =>
                   {
                       x.RoutePrefix = "docs";
                       x.DocumentTitle = "Questionnaires API Documentation";
                       x.HeadContent = "Questionnaires API Documentation";
                       x.DocExpansion(DocExpansion.None);
                       x.SwaggerEndpoint("v1/swagger.json", "Questionnaires API Documentation");
                   })
               .UseResponseCompression()
               .UseMvc();
        }
    }
}