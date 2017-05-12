using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qubiz.IdentityServer.Data;
using Qubiz.IdentityServer.Models;
using Qubiz.IdentityServer.Services;
using System.IO;
using System.Threading.Tasks;

namespace Qubiz.IdentityServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // IdentityServer4
            services
                .AddIdentityServer()
                .AddInMemoryApiResources(Identity.Config.GetApiResources())
                .AddInMemoryClients(Identity.Config.GetClients());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMiddleware<IndentDiscoveryDocumentJsonMiddleware>();

            app.UseIdentity();

            app.UseIdentityServer();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public class IndentDiscoveryDocumentJsonMiddleware
        {
            private readonly RequestDelegate next;

            private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            public IndentDiscoveryDocumentJsonMiddleware(RequestDelegate next)
            {
                this.next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                if (context.Request.Path.StartsWithSegments("/.well-known/openid-configuration"))
                {
                    Stream originalResponseBody = context.Response.Body;

                    try
                    {
                        using (var memoryResponseStream = new MemoryStream())
                        {
                            context.Response.Body = memoryResponseStream;

                            await next(context);

                            memoryResponseStream.Position = 0;
                            string discoDocJsonUnindented = new StreamReader(memoryResponseStream).ReadToEnd();

                            string discoDocJsonIndented = JsonConvert.SerializeObject(JObject.Parse(discoDocJsonUnindented), settings);

                            context.Response.Body = originalResponseBody;
                            await context.Response.WriteAsync(discoDocJsonIndented);
                        }
                    }
                    finally
                    {
                        context.Response.Body = originalResponseBody;
                    }
                }
                else
                {
                    await next(context);
                }
            }
        }
    }
}