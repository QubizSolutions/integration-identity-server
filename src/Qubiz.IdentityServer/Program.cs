using Microsoft.AspNetCore.Hosting;
using System.IO;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.Extensions.Logging;

namespace Qubiz.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                   .ConfigureLogging(builder =>
                   {
                       builder.ClearProviders();
                       builder.AddSerilog();
                   })
                .UseApplicationInsights()
                .Build();


            host.Run();
        }
    }
}