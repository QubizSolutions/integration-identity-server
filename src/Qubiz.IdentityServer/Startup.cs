using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qubiz.IdentityServer.Data;
using Qubiz.IdentityServer.Models;
using Qubiz.IdentityServer.Services;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using System.Linq;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Qubiz.IdentityServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {

            Console.Title = "IdentityServer4";

            Log.Logger = new LoggerConfiguration()
                     .MinimumLevel.Debug()
                     .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                     .MinimumLevel.Override("System", LogEventLevel.Warning)
                     .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                     .Enrich.FromLogContext()
                     .WriteTo.Console(outputTemplate:
                     "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                        theme: AnsiConsoleTheme.Literate)
                     .CreateLogger();

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
        public object Clients { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            TelemetryConfiguration.Active.DisableTelemetry = true;

            services.AddDbContext<ApplicationDbContext>(options =>
                  options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Identity.Config.GetIdentityResources())
                .AddInMemoryApiResources(Identity.Config.GetApiResources())
                .AddInMemoryClients(Identity.Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>();


            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "950973578477-pc9tc6c99e1mpcdhh1o6jb5e13h13cmo.apps.googleusercontent.com";
                    options.ClientSecret = "OQrvEOX0y5ZJ8Hy_keFSfR5Y";
                });


            //Azure AD Auth
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                // Configure the OWIN pipeline to use cookie auth.
                .AddCookie()
                // Configure the OWIN pipeline to use OpenID Connect auth.
                .AddOpenIdConnect("Azure", option =>
                {
                    option.ClientId = "123"; // Configuration["AzureAD:ClientId"];
                    option.Authority = "https://login.microsoftonline.com/{0}"; // String.Format(Configuration["AzureAd:AadInstance"], Configuration["AzureAd:Tenant"]);
                    option.SignedOutRedirectUri = "123"; //Configuration["AzureAd:PostLogoutRedirectUri"];
                });




            //// Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();


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

            app.UseAuthentication();

            app.UseIdentityServer();

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